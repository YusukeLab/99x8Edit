using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace _99x8Edit
{
    // Sprite editor window
    public partial class Sprites : Form
    {
        private readonly Machine dataSource;
        private readonly MainWindow mainWin;
        private readonly List<Control> tabOrder = new List<Control>();
        private Bitmap bmpPalette = new Bitmap(256, 64);        // Palette view
        private Bitmap bmpSprites = new Bitmap(256, 256);       // Sprites view
        private Bitmap bmpSpriteEdit = new Bitmap(256, 256);    // Sprite edit view
        private Bitmap bmpPreview = new Bitmap(32, 32);         // Edit preview
        private Bitmap bmpColorL = new Bitmap(32, 32);
        private Bitmap bmpColorR = new Bitmap(32, 32);
        private Bitmap bmpColorOR = new Bitmap(32, 32);
        private Point curSpr;           // Selected sprite
        private Point selStartSpr;      // For multiple selection
        private Point curLine;          // Selected line in editor
        private Point selStartLine;     // For multiple selection
        private int currentDot;         // Selected dot in line(0-7)
        private int currentColor;       // Currently elected color, 0=left, 1=right
        private Point curPal;           // Selection in palette
        String currentFile = "";
        public String CurrentFile
        {
            set { currentFile = value; }
        }
        // For internal drag control
        private class DnDSprite { }
        private class DnDEditor { }
        //----------------------------------------------------------------------
        // Initialize
        public Sprites(Machine src, MainWindow parent)
        {
            InitializeComponent();
            // Set corresponding data and owner window
            dataSource = src;
            mainWin = parent;
            // Tab order
            tabOrder.AddRange(new Control[] { panelEditor, panelColor, panelPalette, panelSprites });
            // Initialize controls
            viewPalette.Image = bmpPalette;
            viewSprites.Image = bmpSprites;
            viewSpriteEdit.Image = bmpSpriteEdit;
            viewPreview.Image = bmpPreview;
            viewColorL.Image = bmpColorL;
            viewColorR.Image = bmpColorR;
            viewColorOR.Image = bmpColorOR;
            // Refresh all views
            this.RefreshAllViews();
            // Menu bar
            toolStripFileLoad.Click += new EventHandler(menu_fileLoad);
            toolStripFileSave.Click += new EventHandler(menu_fileSave);
            toolStripFileSaveAs.Click += new EventHandler(menu_fileSaveAs);
            toolStripFileImport.Click += new EventHandler(menu_fileImport);
            toolStripFileExport.Click += new EventHandler(menu_fileExport);
            toolStripFileLoadSprite.Click += new EventHandler(menu_fileLoadSprite);
            toolStripFileSaveSprite.Click += new EventHandler(menu_fileSaveSprite);
            toolStripFileLoadPal.Click += new EventHandler(menu_fileLoadPalette);
            toolStripFileSavePal.Click += new EventHandler(menu_fileSavePalette);
            toolStripEditUndo.Click += new EventHandler(menu_editUndo);
            toolStripEditRedo.Click += new EventHandler(menu_editRedo);
            // context menu
            toolStripSprCopy.Click += new EventHandler(contextSprites_copy);
            toolStripSprPaste.Click += new EventHandler(contextSprites_paste);
            toolStripSprDel.Click += new EventHandler(contextSprites_del);
            toolStripSprReverse.Click += new EventHandler(contextSprites_reverse);
            toolStripSprCopyDown.Click += new EventHandler(contextSprites_copyDown);
            toolStripSprCopyRight.Click += new EventHandler(contextSprites_copyRight);
            toolStripEditorCopy.Click += new EventHandler(contextEditor_copy);
            toolStripEditorPaste.Click += new EventHandler(contextEditor_paste);
            toolStripEditorDel.Click += new EventHandler(contextEditor_del);
            toolStripEditorCopyDown.Click += new EventHandler(contextEditor_copyDown);
            toolStripEditorCopyRight.Click += new EventHandler(contextEditor_copyRight);
            toolStripRotateUp.Click += new EventHandler(contextEditor_rotate);
            toolStripRotateDown.Click += new EventHandler(contextEditor_rotate);
            toolStripRotateLeft.Click += new EventHandler(contextEditor_rotate);
            toolStripRotateRight.Click += new EventHandler(contextEditor_rotate);
        }
        //------------------------------------------------------------------------------
        // Overrides
        protected override bool ProcessDialogKey(Keys keyData)
        {
            switch (keyData)
            {
                // prevent focus movement by the cursor
                case Keys.Down:
                case Keys.Right:
                case Keys.Up:
                case Keys.Left:
                case Keys.Down | Keys.Shift:
                case Keys.Right | Keys.Shift:
                case Keys.Up | Keys.Shift:
                case Keys.Left | Keys.Shift:
                    break;
                default:
                    return base.ProcessDialogKey(keyData);
            }
            return true;
        }
        protected override bool ProcessTabKey(bool forward)
        {
            Control prev = this.ActiveControl;
            int index = tabOrder.IndexOf(prev) + (forward ? 1 : tabOrder.Count - 1);
            index %= tabOrder.Count;
            this.ActiveControl = tabOrder[index];
            this.RefreshAllViews();     // Current focused selection will be hilighted
            return true;
        }
        //------------------------------------------------------------------------------
        // Refreshing Views
        private void RefreshAllViews()
        {
            this.UpdatePaletteView();       // Palette view
            this.UpdateSpriteView();        // Sprites view
            this.UpdateSpriteEditView();    // Sprite edit view
            this.UpdateCurrentColorView();  // Current color
            this.UpdateOverlayCheck();
            this.chkTMS.Checked = dataSource.IsTMS9918;
            this.toolStripFileLoadPal.Enabled = !dataSource.IsTMS9918;
            this.toolStripFileSavePal.Enabled = !dataSource.IsTMS9918;
        }
        private void UpdatePaletteView(bool refresh = true)
        {
            // Update palette view
            Utility.DrawTransparent(bmpPalette);
            Graphics g = Graphics.FromImage(bmpPalette);
            for (int i = 1; i < 16; ++i)
            {
                Brush b = dataSource.BrushOf(i);
                g.FillRectangle(b, (i % 8) * 32, (i / 8) * 32, 32, 32);
            }
            Utility.DrawSelection(g, curPal.X * 32, curPal.Y * 32, 31, 31, panelPalette.Focused);
            if (refresh) viewPalette.Refresh();
        }
        private void UpdateSpriteView(bool refresh = true)
        {
            Graphics g = Graphics.FromImage(bmpSprites);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            g.FillRectangle(Brushes.Black, 0, 0, bmpSprites.Width, bmpSprites.Height);
            for (int i = 0; i < 8; ++i)
            {
                for (int j = 0; j < 8; ++j)
                {
                    // Four sprites in one 16x16 sprites
                    for (int dx = 0; dx < 2; ++dx)
                    {
                        for (int dy = 0; dy < 2; ++dy)
                        {
                            int target_num8x8 = (i * 8 + j) * 4 + dx * 2 + dy;
                            int target_x = j * 32 + dx * 16;
                            int target_y = i * 32 + dy * 16;
                            Bitmap b = dataSource.GetBitmapOfSprite(target_num8x8);
                            g.DrawImage(b, target_x, target_y, 17, 17);
                        }
                    }
                }
            }
            // CRT Filter
            if (chkCRT.Checked)
            {
                Filter.Create(Filter.Type.CRT).Process(bmpSprites);
            }
            // Selection overlay
            int index_of_16x16 = curSpr.X + curSpr.Y * 8;
            if (dataSource.GetSpriteOverlay(index_of_16x16))
            {
                // Overlayed sprite, right side
                int index_r_16x16 = (index_of_16x16 + 1) % 64;
                int xr = index_r_16x16 % 8;
                int yr = index_r_16x16 / 8;
                g.DrawRectangle(Utility.DashedGray, xr * 32, yr * 32, 31, 31);
            }
            // Selection
            int x = Math.Min(curSpr.X, selStartSpr.X);
            int y = Math.Min(curSpr.Y, selStartSpr.Y);
            int w = Math.Abs(curSpr.X - selStartSpr.X) + 1;
            int h = Math.Abs(curSpr.Y - selStartSpr.Y) + 1;
            Utility.DrawSelection(g, x * 32, y * 32, w * 32 - 1, h * 32 - 1, panelSprites.Focused);
            if (refresh) viewSprites.Refresh();
        }
        private void UpdateSpriteEditView(bool refresh = true)
        {
            Utility.DrawTransparent(bmpSpriteEdit);
            Utility.DrawTransparent(bmpPreview);
            Graphics g = Graphics.FromImage(bmpSpriteEdit);
            Graphics preview = Graphics.FromImage(bmpPreview);
            int index_of_16x16 = curSpr.X + curSpr.Y * 8;
            bool overlayed = dataSource.GetSpriteOverlay(index_of_16x16);
            // Four sprites are in one 16x16 sprite
            for (int x = 0; x < 2; ++x)
            {
                for (int y = 0; y < 2; ++y)
                {
                    // One sprite in 16x16 sprite
                    int target_sprite = index_of_16x16 * 4 + x * 2 + y;

                    for (int j = 0; j < 8; ++j)         // Line
                    {
                        for (int k = 0; k < 8; ++k)     // Pixel
                        {
                            int color_code = 0;     // transparent as default
                            int ptn_but = dataSource.GetSpritePixel(target_sprite, j, k);
                            if (ptn_but != 0)
                            {
                                // pixel exists, so get the color code
                                color_code = dataSource.GetSpriteColorCode(target_sprite, j);
                            }
                            if (overlayed)
                            {
                                // Overlay sprite exists
                                int over_index = (target_sprite + 4) % 256;
                                int ptn_over = dataSource.GetSpritePixel(over_index, j, k);
                                if (ptn_over != 0)
                                {
                                    // pixel of overlayed sprite exists, so get or of the color code
                                    if (dataSource.IsTMS9918)
                                    {
                                        if (color_code == 0)
                                        {
                                            // On TMS9918 \, if the pixel of primary sprite exists, don't draw
                                            color_code = dataSource.GetSpriteColorCode(over_index, j);
                                        }
                                    }
                                    else
                                    {
                                        // Take or on V9938
                                        color_code |= dataSource.GetSpriteColorCode(over_index, j);
                                    }
                                }
                            }
                            if (color_code != 0)
                            {
                                g.FillRectangle(Brushes.Gray, x * 128 + k * 16, y * 128 + j * 16, 16, 16);
                                Brush b = dataSource.BrushOf(color_code);
                                g.FillRectangle(b, x * 128 + k * 16, y * 128 + j * 16, 15, 15);
                                preview.FillRectangle(b, x * 16 + k * 2, y * 16 + j * 2, 2, 2);
                            }
                        }
                    }
                }
            }
            // Draw selection rectangle
            int sx = Math.Min(curLine.X, selStartLine.X);
            int sy = Math.Min(curLine.Y, selStartLine.Y);
            int sw = Math.Abs(curLine.X - selStartLine.X) + 1;
            int sh = Math.Abs(curLine.Y - selStartLine.Y) + 1;
            Utility.DrawSelection(g, sx * 128, sy * 16, sw * 128 - 1, sh * 16 - 1, panelEditor.Focused);
            if (panelEditor.Focused)
            {
                Utility.DrawSubSelection(g, sx * 128 + currentDot * 16, sy * 16, 14, 14);
            }
            // CRT Filter
            if (chkCRT.Checked)
            {
                Filter.Create(Filter.Type.CRT).Process(bmpPreview);
            }
            if (refresh) viewSpriteEdit.Refresh();
            if (refresh) viewPreview.Refresh();
        }
        void UpdateCurrentColorView(bool refresh = true)
        {
            Graphics gl = Graphics.FromImage(bmpColorL);
            Graphics gr = Graphics.FromImage(bmpColorR);
            Graphics go = Graphics.FromImage(bmpColorOR);
            // Update current color of primary sprite
            int sprite_num_16x16 = curSpr.Y * 8 + curSpr.X;
            int sprite_num_8x8 = sprite_num_16x16 * 4 + curLine.X * 2 + curLine.Y / 8;
            int color_code_primary = dataSource.GetSpriteColorCode(sprite_num_8x8, curLine.Y % 8);
            Brush color_primary = dataSource.BrushOf(color_code_primary);
            gl.FillRectangle(color_primary, 0, 0, 32, 32);
            // Check overlays
            if (dataSource.GetSpriteOverlay(sprite_num_16x16))
            {
                int sprite_num_8x8_secondary = (sprite_num_8x8 + 4) % 256;
                int color_code_secondary = dataSource.GetSpriteColorCode(sprite_num_8x8_secondary, curLine.Y % 8);
                Brush color_secondary = dataSource.BrushOf(color_code_secondary);
                gr.FillRectangle(color_secondary, 0, 0, 32, 32);
                viewColorR.Visible = true;
                labelColorR.Visible = true;
                if (dataSource.IsTMS9918)
                {
                    // Overlayed, but no OR color(TMS9918)
                    viewColorOR.Visible = false;
                    labelColorOR.Visible = false;
                }
                else
                {
                    // Overlayed with or color(V9938)
                    int color_code_or = color_code_primary | color_code_secondary;
                    Brush color_or = dataSource.BrushOf(color_code_or);
                    go.FillRectangle(color_or, 0, 0, 32, 32);
                    viewColorOR.Visible = true;
                    labelColorOR.Visible = true;
                }
            }
            else
            {
                // No overlay
                viewColorR.Visible = false;
                labelColorR.Visible = false;
                viewColorOR.Visible = false;
                labelColorOR.Visible = false;
            }
            // Current selection
            if (!viewColorR.Visible && currentColor > 0) currentColor = 0;
            if (currentColor == 0)
            {
                Utility.DrawSelection(gl, 0, 0, 29, 29, panelColor.Focused);
            }
            else if (currentColor == 1)
            {
                Utility.DrawSelection(gr, 0, 0, 29, 29, panelColor.Focused);
            }
            if (refresh)
            {
                viewColorL.Refresh();
                viewColorR.Refresh();
                viewColorOR.Refresh();
            }
        }
        private void UpdateOverlayCheck(bool refresh = true)
        {
            int sprite_num16x16 = curSpr.Y * 8 + curSpr.X;
            this.checkOverlay.Checked = dataSource.GetSpriteOverlay(sprite_num16x16);
            if (refresh) this.UpdateSpriteView();
        }
        //------------------------------------------------------------------------------
        // Controls
        private void checkOverlay_Click(object sender, EventArgs e)
        {
            int current_index = curSpr.Y * 8 + curSpr.X;
            int overlay_index = (current_index + 1) % 64;
            if (checkOverlay.Checked)
            {
                dataSource.SetSpriteOverlay(current_index, true, true);           // Update VRAM
            }
            else
            {
                dataSource.SetSpriteOverlay(current_index, false, true);          // Update VRAM
            }
            this.RefreshAllViews();
        }
        private void viewSprites_MouseDown(object sender, MouseEventArgs e)
        {
            panelSprites.Focus();   // Key events are handled by parent panel
            if (e.Button == MouseButtons.Left)
            {
                int x = Math.Min(e.X / 32, 7);
                int y = Math.Min(e.Y / 32, 7);
                if ((x != curSpr.X) || (y != curSpr.Y))
                {
                    // Sprite selected
                    if (Control.ModifierKeys == Keys.Shift)
                    {
                        // Multiple selection
                        curSpr.X = x;
                        curSpr.Y = y;
                    }
                    else
                    {
                        // New selection
                        curSpr.X = selStartSpr.X = x;
                        curSpr.Y = selStartSpr.Y = y;
                    }
                    this.RefreshAllViews();
                    viewSprites.DoDragDrop(new DnDSprite(), DragDropEffects.Copy);
                }
            }
        }
        private void panelSprites_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Up | Keys.Shift:
                    if (curSpr.Y > 0)
                    {
                        curSpr.Y--;
                        this.RefreshAllViews();
                    }
                    break;
                case Keys.Down | Keys.Shift:
                    if (curSpr.Y < 7)
                    {
                        curSpr.Y++;
                        this.RefreshAllViews();
                    }
                    break;
                case Keys.Left | Keys.Shift:
                    if (curSpr.X > 0)
                    {
                        curSpr.X--;
                        this.RefreshAllViews();
                    }
                    break;
                case Keys.Right | Keys.Shift:
                    if (curSpr.X < 7)
                    {
                        curSpr.X++;
                        this.RefreshAllViews();
                    }
                    break;
                case Keys.Up:
                    if (curSpr.Y > 0)
                    {
                        curSpr.Y--;
                        selStartSpr.X = curSpr.X;
                        selStartSpr.Y = curSpr.Y;
                        this.RefreshAllViews();
                    }
                    break;
                case Keys.Down:
                    if (curSpr.Y < 7)
                    {
                        curSpr.Y++;
                        selStartSpr.X = curSpr.X;
                        selStartSpr.Y = curSpr.Y;
                        this.RefreshAllViews();
                    }
                    break;
                case Keys.Left:
                    if (curSpr.X > 0)
                    {
                        curSpr.X--;
                        selStartSpr.X = curSpr.X;
                        selStartSpr.Y = curSpr.Y;
                        this.RefreshAllViews();
                    }
                    break;
                case Keys.Right:
                    if (curSpr.X < 7)
                    {
                        curSpr.X++;
                        selStartSpr.X = curSpr.X;
                        selStartSpr.Y = curSpr.Y;
                        this.RefreshAllViews();
                    }
                    break;
            }
        }
        private void panelSprites_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DnDSprite)))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else e.Effect = DragDropEffects.None;
        }

        private void panelSprites_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DnDSprite)))
            {
                Point p = viewSprites.PointToClient(Cursor.Position);
                curSpr.X = Math.Min(p.X / 32, 7);
                curSpr.Y = Math.Min(p.Y / 32, 7);
                this.RefreshAllViews();
            }
        }
        private void contextSprites_copy(object sender, EventArgs e)
        {
            ClipSprite clip = new ClipSprite();
            // Copy selected sprites
            int x = Math.Min(curSpr.X, selStartSpr.X);
            int y = Math.Min(curSpr.Y, selStartSpr.Y);
            int w = Math.Abs(curSpr.X - selStartSpr.X) + 1;
            int h = Math.Abs(curSpr.Y - selStartSpr.Y) + 1;
            for (int i = y; i < y + h; ++i)
            {
                List<Machine.One16x16Sprite> l = new List<Machine.One16x16Sprite>();
                for (int j = x; j < x + w; ++j)
                {
                    l.Add(dataSource.Get16x16Sprite(i * 8 + j));
                }
                clip.sprites.Add(l);
            }
            ClipboardWrapper.SetData(clip);
        }
        private void contextSprites_paste(object sender, EventArgs e)
        {
            dynamic clip = ClipboardWrapper.GetData();
            if (clip is ClipSprite)
            {
                MementoCaretaker.Instance.Push();
                // Paste multiple sprites
                for (int i = 0; (i < clip.sprites.Count) && (curSpr.Y + i < 8); ++i)
                {
                    List<Machine.One16x16Sprite> l = clip.sprites[i];
                    for (int j = 0; (j < l.Count) && (curSpr.X + j < 8); ++j)
                    {
                        int index16x16 = (curSpr.Y + i) * 8 + (curSpr.X + j);
                        dataSource.Set16x16Sprite(index16x16, l[j], false);
                    }
                }
                this.RefreshAllViews();
            }
            else if (clip is ClipPeekedData)
            {
                MementoCaretaker.Instance.Push();
                // Copied from peek window
                for (int i = 0; (i < clip.peeked.Count / 2) && (curSpr.Y + i < 8); ++i)
                {
                    // One row in peek window is 8 dots
                    List<byte[]> first_row = clip.peeked[i * 2 + 0];
                    List<byte[]> second_row = clip.peeked[i * 2 + 1];
                    for (int j = 0; (j < first_row.Count / 2) && (curSpr.X + j < 8); ++j)
                    {
                        int index16x16 = (curSpr.Y + i) * 8 + (curSpr.X + j);
                        dataSource.SetSpriteOverlay(index16x16, false, false);
                        int lefttop8x8 = index16x16 * 4;
                        dataSource.SetSpriteGen(lefttop8x8 + 0, first_row[j * 2 + 0], false);
                        dataSource.SetSpriteGen(lefttop8x8 + 1, second_row[j * 2 + 0], false);
                        dataSource.SetSpriteGen(lefttop8x8 + 2, first_row[j * 2 + 1], false);
                        dataSource.SetSpriteGen(lefttop8x8 + 3, second_row[j * 2 + 1], false);
                    }
                }
                this.RefreshAllViews();
            }
        }
        private void contextSprites_del(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            int x = Math.Min(curSpr.X, selStartSpr.X);
            int y = Math.Min(curSpr.Y, selStartSpr.Y);
            int w = Math.Abs(curSpr.X - selStartSpr.X) + 1;
            int h = Math.Abs(curSpr.Y - selStartSpr.Y) + 1;
            for (int i = y; i < y + h; ++i)
            {
                for (int j = x; j < x + w; ++j)
                {
                    dataSource.Clear16x16Sprite(i * 8 + j, false);
                }
            }
            this.RefreshAllViews();
        }
        private void contextSprites_reverse(object sender, EventArgs e)
        {
            int current = curSpr.Y * 8 + curSpr.X;
            int loop_cnt = dataSource.GetSpriteOverlay(current) ? 2 : 1;
            for (int i = 0; i < loop_cnt; ++i)
            {
                int target16x16 = (current + i) % 64;
                int target8x8 = target16x16 * 4;
                for (int y = 0; y < 16; ++y)
                {
                    List<int> one_line = new List<int>();
                    for (int x = 0; x < 16; ++x)
                    {
                        // Read from right to left
                        int src8x8 = target8x8 + ((1 - x / 8) * 2) + (y / 8);
                        one_line.Add(dataSource.GetSpritePixel(src8x8, y % 8, 7 - (x % 8)));
                    }
                    for (int x = 0; x < 16; ++x)
                    {
                        // Write from left to right
                        int dst8x8 = target8x8 + ((x / 8) * 2) + (y / 8);
                        dataSource.SetSpritePixel(dst8x8, y % 8, x % 8, one_line[x], true);
                    }
                }
            }
            this.RefreshAllViews();
        }
        private void contextSprites_copyDown(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            int x = Math.Min(curSpr.X, selStartSpr.X);
            int y = Math.Min(curSpr.Y, selStartSpr.Y);
            int w = Math.Abs(curSpr.X - selStartSpr.X) + 1;
            int h = Math.Abs(curSpr.Y - selStartSpr.Y) + 1;
            for (int i = y + 1; i < y + h; ++i)
            {
                for (int j = x; j < x + w; ++j)
                {
                    Machine.One16x16Sprite spr = dataSource.Get16x16Sprite(y * 8 + j);
                    dataSource.Set16x16Sprite(i * 8 + j, spr, false);
                }
            }
            this.RefreshAllViews();
        }
        private void contextSprites_copyRight(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            int x = Math.Min(curSpr.X, selStartSpr.X);
            int y = Math.Min(curSpr.Y, selStartSpr.Y);
            int w = Math.Abs(curSpr.X - selStartSpr.X) + 1;
            int h = Math.Abs(curSpr.Y - selStartSpr.Y) + 1;
            for (int i = y; i < y + h; ++i)
            {
                for (int j = x + 1; j < x + w; ++j)
                {
                    Machine.One16x16Sprite spr = dataSource.Get16x16Sprite(i * 8 + x);
                    dataSource.Set16x16Sprite(i * 8 + j, spr, false);
                }
            }
            this.RefreshAllViews();
        }
        private void viewSpriteEdit_MouseDown(object sender, MouseEventArgs e)
        {
            panelEditor.Focus();    // Key events are handled by parent panel
            if (e.Button == MouseButtons.Left)
            {
                int clicked_x = e.X / 128;
                int clicled_y = e.Y / 16;
                // selected line had changed
                if ((curLine.X != clicked_x) || (curLine.Y != clicled_y))
                {
                    if (Control.ModifierKeys == Keys.Shift)
                    {
                        // Multiple selection
                        curLine.X = clicked_x;
                        curLine.Y = clicled_y;
                    }
                    else
                    {
                        // New selection
                        curLine.X = selStartLine.X = clicked_x;
                        curLine.Y = selStartLine.Y = clicled_y;
                    }
                    this.UpdateSpriteEditView();
                    this.UpdateCurrentColorView();
                    viewSpriteEdit.DoDragDrop(new DnDEditor(), DragDropEffects.Copy);
                    return;
                }
                else
                {
                    // toggle the color of selected pixel
                    this.EditCurrentSprite((e.X / 16) % 8, curLine.Y % 8);
                }
            }
        }
        private void panelEditor_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            Action refresh = () =>
            {
                this.UpdateSpriteEditView();
                this.UpdateCurrentColorView();
            };
            switch (e.KeyData)
            {
                // Multiple selection
                case Keys.Up | Keys.Shift:
                    if (curLine.Y > 0)
                    {
                        curLine.Y--;
                        refresh();
                    }
                    break;
                case Keys.Down | Keys.Shift:
                    if (curLine.Y < 15)
                    {
                        curLine.Y++;
                        refresh();
                    }
                    break;
                case Keys.Left | Keys.Shift:
                    if (curLine.X > 0)
                    {
                        curLine.X--;
                        refresh();
                    }
                    break;
                case Keys.Right | Keys.Shift:
                    if (curLine.X < 1)
                    {
                        curLine.X++;
                        refresh();
                    }
                    break;
                // Moving current selection
                case Keys.Up:
                    if (curLine.Y > 0)
                    {
                        curLine.Y--;
                        selStartLine.X = curLine.X;
                        selStartLine.Y = curLine.Y;
                        refresh();
                    }
                    break;
                case Keys.Down:
                    if (curLine.Y < 15)
                    {
                        curLine.Y++;
                        selStartLine.X = curLine.X;
                        selStartLine.Y = curLine.Y;
                        refresh();
                    }
                    break;
                case Keys.Left:
                    if ((currentDot == 0) && (curLine.X > 0))
                    {
                        curLine.X--;
                        currentDot = 7;
                        selStartLine.X = curLine.X;
                        selStartLine.Y = curLine.Y;
                        refresh();
                    }
                    else if (currentDot > 0)
                    {
                        currentDot--;
                        refresh();
                    }
                    break;
                case Keys.Right:
                    if ((currentDot == 7) && (curLine.X < 1))
                    {
                        curLine.X++;
                        currentDot = 0;
                        selStartLine.X = curLine.X;
                        selStartLine.Y = curLine.Y;
                        refresh();
                    }
                    else if (currentDot < 7)
                    {
                        currentDot++;
                        refresh();
                    }
                    break;
                // Editing
                case Keys.Space:
                    // toggle the color of selected pixel
                    this.EditCurrentSprite(currentDot, curLine.Y % 8);
                    break;
                // Changing colors
                case Keys.D1:
                case Keys.NumPad1:
                    this.EditCurrentSprite(0, curLine.Y % 8);
                    break;
                case Keys.D2:
                case Keys.NumPad2:
                    this.EditCurrentSprite(1, curLine.Y % 8);
                    break;
                case Keys.D3:
                case Keys.NumPad3:
                    this.EditCurrentSprite(2, curLine.Y % 8);
                    break;
                case Keys.D4:
                case Keys.NumPad4:
                    this.EditCurrentSprite(3, curLine.Y % 8);
                    break;
                case Keys.D5:
                case Keys.NumPad5:
                    this.EditCurrentSprite(4, curLine.Y % 8);
                    break;
                case Keys.D6:
                case Keys.NumPad6:
                    this.EditCurrentSprite(5, curLine.Y % 8);
                    break;
                case Keys.D7:
                case Keys.NumPad7:
                    this.EditCurrentSprite(6, curLine.Y % 8);
                    break;
                case Keys.D8:
                case Keys.NumPad8:
                    this.EditCurrentSprite(7, curLine.Y % 8);
                    break;
                case Keys.Oemplus:
                case Keys.Add:
                case Keys.OemMinus:
                case Keys.Subtract:
                    {
                        int sprite_num_16x16 = curSpr.Y * 8 + curSpr.X;
                        int sprite_num_8x8 = sprite_num_16x16 * 4 + curLine.X * 2 + curLine.Y / 8;
                        int color_code_primary = dataSource.GetSpriteColorCode(sprite_num_8x8, curLine.Y % 8);
                        if ((e.KeyData == Keys.Oemplus) || (e.KeyData == Keys.Add))
                        {
                            // Increment color of the primary sprite
                            if (color_code_primary < 15) color_code_primary++;
                        }
                        else
                        {
                            // Decrement color of the primary sprite
                            if (color_code_primary > 1) color_code_primary--;
                        }
                        this.SetSpriteColor(sprite_num_16x16, color_code_primary);
                        this.RefreshAllViews();
                    }
                    break;
                case Keys.OemCloseBrackets:
                case Keys.OemOpenBrackets:
                    // Check overlays
                    if (dataSource.GetSpriteOverlay(curSpr.Y * 8 + curSpr.X))
                    {
                        int sprite_num_16x16 = curSpr.Y * 8 + curSpr.X;
                        int sprite_num_8x8 = sprite_num_16x16 * 4 + curLine.X * 2 + curLine.Y / 8;
                        int sprite_num_8x8_secondary = (sprite_num_8x8 + 4) % 256;
                        int color_code_secondary = dataSource.GetSpriteColorCode(sprite_num_8x8_secondary, curLine.Y % 8);
                        if (e.KeyData == Keys.OemCloseBrackets)
                        {
                            // Increment color of the secondary sprite
                            if (color_code_secondary < 15) color_code_secondary++;
                        }
                        else
                        {
                            // Decrement color of the secondary sprite
                            if (color_code_secondary > 1) color_code_secondary--;
                        }
                        this.SetSpriteColor(sprite_num_8x8_secondary / 4, color_code_secondary);
                        this.RefreshAllViews();
                    }
                    break;
            }
        }
        private void panelEditor_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DnDEditor)))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else e.Effect = DragDropEffects.None;
        }

        private void panelEditor_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DnDEditor)))
            {
                Point p = viewSpriteEdit.PointToClient(Cursor.Position);
                curLine.X = Math.Min(p.X / 128, 1);
                curLine.Y = Math.Min(p.Y / 16, 15);
                this.UpdateSpriteEditView();
                this.UpdateCurrentColorView();
            }
        }
        private void contextEditor_copy(object sender, EventArgs e)
        {
            ClipOneSpriteLine clip = new ClipOneSpriteLine();
            int x = Math.Min(curLine.X, selStartLine.X);
            int y = Math.Min(curLine.Y, selStartLine.Y);
            int w = Math.Abs(curLine.X - selStartLine.X) + 1;
            int h = Math.Abs(curLine.Y - selStartLine.Y) + 1;
            for (int i = y; i < y + h; ++i)
            {
                List<Machine.SpriteLine> l = new List<Machine.SpriteLine>();
                for (int j = x; j < x + w; ++j)
                {
                    int lefttop16x16 = curSpr.Y * 8 + curSpr.X;
                    int target8x8 = lefttop16x16 * 4 + j * 2 + i / 8;
                    l.Add(dataSource.GetSpriteLine(target8x8, i % 8));
                }
                clip.lines.Add(l);
            }
            ClipboardWrapper.SetData(clip);
        }
        private void contextEditor_paste(object sender, EventArgs e)
        {
            dynamic clip = ClipboardWrapper.GetData();
            if (clip is ClipOneSpriteLine)
            {
                MementoCaretaker.Instance.Push();
                for (int i = 0; (i < clip.lines.Count) && (curLine.Y + i < 16); ++i)
                {
                    List<Machine.SpriteLine> l = clip.lines[i];
                    for (int j = 0; (j < l.Count) && (curLine.X + j < 2); ++j)
                    {
                        int lefttop16x16 = curSpr.Y * 8 + curSpr.X;
                        int target8x8 = lefttop16x16 * 4 + (curLine.X + j) * 2 + ((curLine.Y + i) / 8);
                        dataSource.SetSpriteLine(target8x8, (i + curLine.Y) % 8, l[j], false);
                    }
                }
                this.UpdateSpriteEditView();
                this.UpdateSpriteView();
            }
        }
        private void contextEditor_del(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            int x = Math.Min(curLine.X, selStartLine.X);
            int y = Math.Min(curLine.Y, selStartLine.Y);
            int w = Math.Abs(curLine.X - selStartLine.X) + 1;
            int h = Math.Abs(curLine.Y - selStartLine.Y) + 1;
            for (int i = y; i < y + h; ++i)
            {
                for (int j = x; j < x + w; ++j)
                {
                    int lefttop16x16 = curSpr.Y * 8 + curSpr.X;
                    int target8x8 = lefttop16x16 * 4 + j * 2 + i / 8;
                    dataSource.ClearSpriteLine(target8x8, i % 8, false);
                }
            }
            this.UpdateSpriteEditView();
            this.UpdateSpriteView();
        }
        private void contextEditor_copyDown(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            int x = Math.Min(curLine.X, selStartLine.X);
            int y = Math.Min(curLine.Y, selStartLine.Y);
            int w = Math.Abs(curLine.X - selStartLine.X) + 1;
            int h = Math.Abs(curLine.Y - selStartLine.Y) + 1;
            for (int i = y + 1; i < y + h; ++i)
            {
                for (int j = x; j < x + w; ++j)
                {
                    int lefttop16x16 = curSpr.Y * 8 + curSpr.X;
                    int src = lefttop16x16 * 4 + (j * 2) + (y / 8);
                    Machine.SpriteLine line = dataSource.GetSpriteLine(src, y % 8);
                    int dst = lefttop16x16 * 4 + (j * 2) + (i / 8);
                    dataSource.SetSpriteLine(dst, i % 8, line, false);
                }
            }
            this.UpdateSpriteEditView();
            this.UpdateSpriteView();
        }
        private void contextEditor_copyRight(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            int x = Math.Min(curLine.X, selStartLine.X);
            int y = Math.Min(curLine.Y, selStartLine.Y);
            int w = Math.Abs(curLine.X - selStartLine.X) + 1;
            int h = Math.Abs(curLine.Y - selStartLine.Y) + 1;
            for (int i = y; i < y + h; ++i)
            {
                for (int j = x + 1; j < x + w; ++j)
                {
                    int lefttop16x16 = curSpr.Y * 8 + curSpr.X;
                    int src = lefttop16x16 * 4 + (x * 2) + (i / 8);
                    Machine.SpriteLine line = dataSource.GetSpriteLine(src, i % 8);
                    int dst = lefttop16x16 * 4 + (j * 2) + (i / 8);
                    dataSource.SetSpriteLine(dst, i % 8, line, false);
                }
            }
            this.UpdateSpriteEditView();
            this.UpdateSpriteView();
        }
        private void contextEditor_rotate(object sender, EventArgs e)
        {
            if (sender == toolStripRotateUp)
            {
                dataSource.RotateSprite(curSpr.Y * 8 + curSpr.X, 0, -1, true);
                this.RefreshAllViews();
            }
            if (sender == toolStripRotateDown)
            {
                dataSource.RotateSprite(curSpr.Y * 8 + curSpr.X, 0, 1, true);
                this.RefreshAllViews();
            }
            if (sender == toolStripRotateLeft)
            {
                dataSource.RotateSprite(curSpr.Y * 8 + curSpr.X, -1, 0, true);
                this.RefreshAllViews();
            }
            if (sender == toolStripRotateRight)
            {
                dataSource.RotateSprite(curSpr.Y * 8 + curSpr.X, 1, 0, true);
                this.RefreshAllViews();
            }
        }
        private void checkTMS_Click(object sender, EventArgs e)
        {
            if (chkTMS.Checked && !dataSource.IsTMS9918)
            {
                // Set windows color of each color code to TMS9918
                dataSource.SetPaletteToTMS9918(true);
            }
            else if (!chkTMS.Checked && dataSource.IsTMS9918)
            {
                // Set windows color of each color code to internal palette
                dataSource.SetPaletteToV9938(true);
            }
            this.RefreshAllViews();     // Everything changes
        }
        private void viewColorL_Click(object sender, EventArgs e)
        {
            // Update selection and controls
            panelColor.Focus();
            currentColor = 0;
            this.UpdateCurrentColorView();
            // Set color to target
            int current_target16x16 = curSpr.Y * 8 + curSpr.X;
            int sprite_num_8x8 = current_target16x16 * 4 + curLine.X * 2 + curLine.Y / 8;
            int color_code = dataSource.GetSpriteColorCode(sprite_num_8x8, curLine.Y % 8);
            Action<int> callback = (x) =>
            {
                if (x != 0)
                {
                    this.SetSpriteColor(current_target16x16, x);
                }
            };
            PaletteSelector win = new PaletteSelector(bmpPalette, color_code, callback);
            win.StartPosition = FormStartPosition.Manual;
            win.Location = Cursor.Position;
            win.Show();
        }
        private void viewColorR_Click(object sender, EventArgs e)
        {
            // Update selection and controls
            panelColor.Focus();
            currentColor = 1;
            this.UpdateCurrentColorView();
            // Set color to target
            int current_target16x16 = (curSpr.Y * 8 + curSpr.X + 1) % 64;
            int sprite_num_8x8 = current_target16x16 * 4 + curLine.X * 2 + curLine.Y / 8;
            int color_code = dataSource.GetSpriteColorCode(sprite_num_8x8, curLine.Y % 8);

            Action<int> callback = (x) =>
            {
                if(x > 0)   // Don't select transparent
                {
                    this.SetSpriteColor(current_target16x16, x);
                }
            };
            PaletteSelector win = new PaletteSelector(bmpPalette, color_code, callback);
            win.StartPosition = FormStartPosition.Manual;
            win.Location = Cursor.Position;
            win.Show();
        }
        private void viewColorOR_Click(object sender, EventArgs e)
        {
            PaletteOrColors win = new PaletteOrColors(dataSource);
            win.Show();
        }
        private void panelColor_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Left:
                    if (currentColor > 0)
                    {
                        currentColor--;
                        this.UpdateCurrentColorView();
                    }
                    break;
                case Keys.Right:
                    if (currentColor < 1 && viewColorR.Visible)
                    {
                        currentColor++;
                        this.UpdateCurrentColorView();
                    }
                    break;
                case Keys.Space:
                case Keys.Enter:
                    if (currentColor == 0)
                    {
                        this.viewColorL_Click(null, null);
                    }
                    else if (currentColor == 1)
                    {
                        this.viewColorR_Click(null, null);
                    }
                    break;
            }
        }
        private void viewPalette_MouseClick(object sender, MouseEventArgs e)
        {
            // Palette view clicked
            panelPalette.Focus();
            int clicked_color_num = Math.Clamp((e.Y / 32) * 8 + (e.X / 32), 0, 15);
            curPal.X = clicked_color_num % 8;
            curPal.Y = clicked_color_num / 8;
            // Update color table of current line
            int current_target16x16 = 0;
            if (e.Button == MouseButtons.Left)          // To current sprite
            {
                current_target16x16 = (curSpr.Y * 8 + curSpr.X) % 64;
            }
            else if (e.Button == MouseButtons.Right)    // To overlayed sprite
            {
                if (checkOverlay.Checked == true)
                {
                    current_target16x16 = (curSpr.Y * 8 + curSpr.X + 1) % 64;
                }
                else return;
            }
            this.SetSpriteColor(current_target16x16, clicked_color_num);
        }
        private void viewPalette_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!chkTMS.Checked)
            {
                int clicked_color_num = (e.Y / 32) * 8 + (e.X / 32);
                this.EditPalette(clicked_color_num);
            }
        }
        private void panelPalette_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    if (curPal.Y > 0)
                    {
                        curPal.Y--;
                    }
                    this.UpdatePaletteView();
                    break;
                case Keys.Down:
                    if (curPal.Y < 1)
                    {
                        curPal.Y++;
                    }
                    this.UpdatePaletteView();
                    break;
                case Keys.Left:
                    if (curPal.X > 0)
                    {
                        curPal.X--;
                    }
                    this.UpdatePaletteView();
                    break;
                case Keys.Right:
                    if (curPal.X < 7)
                    {
                        curPal.X++;
                    }
                    this.UpdatePaletteView();
                    break;
                case Keys.Space:
                case Keys.Enter:
                    this.EditPalette(curPal.Y * 8 + curPal.X);
                    break;
            }
        }
        private void Sprites_Activated(object sender, EventArgs e)
        {
            // Refresh all since palette may have been changed
            this.RefreshAllViews();
        }
        public void ChangeOccuredByHost()
        {
            this.RefreshAllViews();
        }
        private void chkCRT_CheckedChanged(object sender, EventArgs e)
        {
            this.RefreshAllViews();
        }
        //---------------------------------------------------------------------
        // Menu controls
        private void menu_fileLoad(object sender, EventArgs e)
        {
            mainWin.LoadProject(sender, e);
        }
        private void menu_fileSave(object sender, EventArgs e)
        {
            mainWin.SaveProject(sender, e);
        }
        private void menu_fileSaveAs(object sender, EventArgs e)
        {
            mainWin.SaveAsProject(sender, e);
        }
        private void menu_fileImport(object sender, EventArgs e)
        {
            if (Utility.ImportDialogAndImport(currentFile,
                                              Import.SpriteTypeFilter,
                                              "Select file to import",
                                              dataSource.ImportSprite))
            {
                this.RefreshAllViews();
            }
        }
        private void menu_fileExport(object sender, EventArgs e)
        {
            mainWin.ExportSprite(sender, e);
        }
        private void menu_fileLoadSprite(object sender, EventArgs e)
        {
            if (Utility.LoadDialogAndLoad(currentFile,
                                          "Sprite File(*.spr)|*.spr",
                                          "Load sprite settings",
                                          dataSource.LoadSprites,
                                          true,     // Push memento
                                          out _))
            {
                this.RefreshAllViews();
            }
        }
        private void menu_fileSaveSprite(object sender, EventArgs e)
        {
            Utility.SaveDialogAndSave(currentFile,
                                      "Sprite File(*.spr)|*.spr",
                                      "Save sprite settings",
                                      dataSource.SaveSprites,
                                      true,
                                      out _);
        }
        private void menu_fileLoadPalette(object sender, EventArgs e)
        {
            if (Utility.LoadDialogAndLoad(currentFile,
                                         "PLT File(*.plt)|*.plt",
                                         "Load palette",
                                         dataSource.LoadPaletteSettings,
                                         true,     // Push memento
                                         out _))
            {
                this.RefreshAllViews();
            }
        }
        private void menu_fileSavePalette(object sender, EventArgs e)
        {
            Utility.SaveDialogAndSave(currentFile,
                                      "PLT File(*.plt)|*.plt",
                                      "Save palette",
                                      dataSource.SavePaletteSettings,
                                      true,
                                      out _);
        }
        private void menu_editUndo(object sender, EventArgs e)
        {
            mainWin.Undo();
        }
        private void menu_editRedo(object sender, EventArgs e)
        {
            mainWin.Redo();
        }
        //----------------------------------------------------------------------
        // Utility
        private void EditPalette(int index)
        {
            int R = dataSource.GetPaletteR(index);
            int G = dataSource.GetPaletteG(index);
            int B = dataSource.GetPaletteB(index);
            PaletteEditor palette_win = null;
            Action callback = () =>
            {
                dataSource.SetPalette(index,
                                      palette_win.R, palette_win.G, palette_win.B, true);
                this.RefreshAllViews();     // Everything changes
            };
            palette_win = new PaletteEditor(R, G, B, callback);
            palette_win.StartPosition = FormStartPosition.Manual;
            palette_win.Location = Cursor.Position;
            palette_win.Show();
        }
        private void SetSpriteColor(int target16x16, int val)
        {
            int target8x8 = target16x16 * 4;
            if (dataSource.IsTMS9918)
            {
                // Set color to four current sprites
                MementoCaretaker.Instance.Push();
                dataSource.SetSpriteColorCode(target8x8 + 0, 0, val, false);
                dataSource.SetSpriteColorCode(target8x8 + 1, 0, val, false);
                dataSource.SetSpriteColorCode(target8x8 + 2, 0, val, false);
                dataSource.SetSpriteColorCode(target8x8 + 3, 0, val, false);
            }
            else
            {
                // Set color to current line of current sprite
                target8x8 = target8x8 + curLine.X * 2 + curLine.Y / 8;
                dataSource.SetSpriteColorCode(target8x8, curLine.Y % 8, val, true);
            }
            this.RefreshAllViews();     // Everything changes
        }
        private void EditCurrentSprite(int x, int y)
        {
            int col = curLine.X;
            int row = curLine.Y / 8;
            int target_lefttop16x16 = curSpr.Y * 8 + curSpr.X;
            int target_sprite8x8 = target_lefttop16x16 * 4 + col * 2 + row;
            int target_prev_pixel = dataSource.GetSpritePixel(target_sprite8x8, y, x);
            // check pixel of second sprite
            int target_lefttop16x16_ov = (target_lefttop16x16 + 1) % 64;
            int target_sprite8x8_ov = target_lefttop16x16_ov * 4 + col * 2 + row;
            int target_prev_pixel_ov = dataSource.GetSpritePixel(target_sprite8x8_ov, y, x);
            // current status 0:transparent, 1:first sprite, 2:second sprie, 3:both
            int current_stat = target_prev_pixel + (target_prev_pixel_ov << 1);
            // so the next status will be....
            int target_stat = current_stat + 1;
            if (dataSource.GetSpriteOverlay(target_lefttop16x16))
            {
                if (dataSource.IsTMS9918)
                {
                    target_stat %= 3;       // Overlayed, no OR color
                }
                else
                {
                    target_stat %= 4;       // Overlayed, OR color available
                }
            }
            else
            {
                target_stat %= 2;           // No overlay
            }
            // set pixel 0:transparent, 1:first sprite, 2:second sprie, 3:both
            int pixel = ((target_stat & 0x01) > 0) ? 1 : 0;
            dataSource.SetSpritePixel(target_sprite8x8, y, x, pixel, true);
            if (dataSource.GetSpriteOverlay(target_lefttop16x16))
            {
                pixel = ((target_stat & 0x02) > 0) ? 1 : 0;
                dataSource.SetSpritePixel(target_sprite8x8_ov, y, x, pixel, true);
            }
            this.UpdateSpriteEditView();
            this.UpdateSpriteView();
        }
    }
}
