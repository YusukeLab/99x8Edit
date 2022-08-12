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
        private Machine dataSource;
        private MainWindow mainWin;
        private Bitmap bmpPalette = new Bitmap(256, 64);        // Palette view
        private Bitmap bmpSprites = new Bitmap(256, 256);       // Sprites view
        private Bitmap bmpSpriteEdit = new Bitmap(256, 256);    // Sprite edit view
        private Bitmap bmpPreview = new Bitmap(32, 32);         // Edit preview
        private int currentSpriteX = 0;     // 0-7
        private int currentSpriteY = 0;     // 0-7
        private int selStartSprX = 0;       // For multiple selection
        private int selStartSprY = 0;
        private int currentLineX = 0;       // Selected line in editor(0-1)
        private int currentLineY = 0;       // Selected line in editor(0-15)
        private int selStartLineX = 0;      // For multiple selection
        private int selStartLineY = 0;
        String currentFile = "";
        public String CurrentFile
        {
            set
            {
                currentFile = value;
            }
        }
        // For internal drag control
        private class DnDSprite { }
        private class DnDEditor { }
        //----------------------------------------------------------------------
        // Initialize
        public Sprites(Machine dataSource, MainWindow parent)
        {
            InitializeComponent();
            // Set corresponding data and owner window
            this.dataSource = dataSource;
            this.mainWin = parent;
            // Initialize controls
            this.viewPalette.Image = bmpPalette;
            this.viewSprites.Image = bmpSprites;
            this.viewSpriteEdit.Image = bmpSpriteEdit;
            this.viewPreview.Image = bmpPreview;
            // Refresh all views
            this.RefreshAllViews();
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
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            Keys mod = keyData & Keys.Modifiers;
            Keys code = keyData & Keys.KeyCode;
            if (mod == Keys.Control)
            {
                switch (code)
                {
                    case Keys.Z:
                        mainWin.Undo();
                        return true;
                    case Keys.Y:
                        mainWin.Redo();
                        return true;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
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
            this.checkTMS.Checked = dataSource.IsTMS9918();
            this.btnOpenPalette.Enabled = !dataSource.IsTMS9918();
            this.btnSavePalette.Enabled = !dataSource.IsTMS9918();
        }
        private void UpdatePaletteView(bool refresh = true)
        {
            // Update palette view
            Utility.DrawTransparent(bmpPalette);
            Graphics g = Graphics.FromImage(bmpPalette);
            for (int i = 1; i < 16; ++i)
            {
                Color c = dataSource.ColorCodeToWindowsColor(i);
                g.FillRectangle(new SolidBrush(c), new Rectangle((i % 8) * 32, (i / 8) * 32, 32, 32));
            }
            if (refresh) this.viewPalette.Refresh();
        }
        private void UpdateSpriteView(bool refresh = true)
        {
            Graphics g = Graphics.FromImage(bmpSprites);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            g.FillRectangle(new SolidBrush(Color.Black), 0, 0, bmpSprites.Width, bmpSprites.Height);
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
            int index_of_16x16 = currentSpriteX + currentSpriteY * 8;
            if (dataSource.GetSpriteOverlay(index_of_16x16))
            {
                // Overlayed sprite, right side
                int index_r_16x16 = (index_of_16x16 + 1) % 64;
                int xr = index_r_16x16 % 8;
                int yr = index_r_16x16 / 8;
                g.DrawRectangle(new Pen(Color.Gray), xr * 32, yr * 32, 31, 31);
            }
            // Selection
            int x = Math.Min(currentSpriteX, selStartSprX);
            int y = Math.Min(currentSpriteY, selStartSprY);
            int w = Math.Abs(currentSpriteX - selStartSprX) + 1;
            int h = Math.Abs(currentSpriteY - selStartSprY) + 1;
            g.DrawRectangle(new Pen(Color.Red), x * 32, y * 32, w * 32 - 1, h * 32 - 1);
            if (refresh) this.viewSprites.Refresh();
        }
        private void UpdateSpriteEditView(bool refresh = true)
        {
            Utility.DrawTransparent(bmpSpriteEdit);
            Utility.DrawTransparent(bmpPreview);
            Graphics g = Graphics.FromImage(bmpSpriteEdit);
            Graphics preview = Graphics.FromImage(bmpPreview);
            int index_of_16x16 = currentSpriteX + currentSpriteY * 8;
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
                                    if (dataSource.IsTMS9918())
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
                                g.FillRectangle(new SolidBrush(Color.Gray), x * 128 + k * 16, y * 128 + j * 16, 16, 16);
                                Color c = dataSource.ColorCodeToWindowsColor(color_code);
                                g.FillRectangle(new SolidBrush(c), x * 128 + k * 16, y * 128 + j * 16, 15, 15);
                                preview.FillRectangle(new SolidBrush(c), x * 16 + k * 2, y * 16 + j * 2, 2, 2);
                            }
                        }
                    }
                }
            }
            // Draw selection rectangle
            int sx = Math.Min(currentLineX, selStartLineX);
            int sy = Math.Min(currentLineY, selStartLineY);
            int sw = Math.Abs(currentLineX - selStartLineX) + 1;
            int sh = Math.Abs(currentLineY - selStartLineY) + 1;
            g.DrawRectangle(new Pen(Color.Red), sx * 128, sy * 16, sw * 128 - 1, sh * 16 - 1);
            // CRT Filter
            if (chkCRT.Checked)
            {
                Filter.Create(Filter.Type.CRT).Process(bmpPreview);
            }
            if (refresh) this.viewSpriteEdit.Refresh();
            if (refresh) this.viewPreview.Refresh();
        }
        void UpdateCurrentColorView(bool refresh = true)
        {
            // Update current color of primary sprite
            int sprite_num_16x16 = currentSpriteY * 8 + currentSpriteX;
            int sprite_num_8x8 = sprite_num_16x16 * 4 + currentLineX * 2 + currentLineY / 8;
            int color_code_primary = dataSource.GetSpriteColorCode(sprite_num_8x8, currentLineY % 8);
            Color color_primary = dataSource.ColorCodeToWindowsColor(color_code_primary);
            viewColorL.BackColor = color_primary;
            if (refresh) viewColorL.Refresh();
            // Check overlays
            if (dataSource.GetSpriteOverlay(sprite_num_16x16))
            {
                int sprite_num_8x8_secondary = (sprite_num_8x8 + 4) % 256;
                int color_code_secondary = dataSource.GetSpriteColorCode(sprite_num_8x8_secondary, currentLineY % 8);
                Color color_secondary = dataSource.ColorCodeToWindowsColor(color_code_secondary);
                viewColorR.BackColor = color_secondary;
                viewColorR.Visible = true;
                labelColorR.Visible = true;
                if (dataSource.IsTMS9918())
                {
                    // Overlayed, but no OR color(TMS9918)
                    viewColorOR.Visible = false;
                    labelColorOR.Visible = false;
                }
                else
                {
                    // Overlayed with or color(V9938)
                    int color_code_or = color_code_primary | color_code_secondary;
                    Color color_or = dataSource.ColorCodeToWindowsColor(color_code_or);
                    viewColorOR.BackColor = color_or;
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
        }
        private void UpdateOverlayCheck(bool refresh = true)
        {
            int sprite_num16x16 = currentSpriteY * 8 + currentSpriteX;
            checkOverlay.Checked = dataSource.GetSpriteOverlay(sprite_num16x16);
            if (refresh) this.UpdateSpriteView();
        }
        //------------------------------------------------------------------------------
        // Controls
        private void checkOverlay_Click(object sender, EventArgs e)
        {
            int current_index = currentSpriteY * 8 + currentSpriteX;
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
                if ((x != currentSpriteX) || (y != currentSpriteY))
                {
                    // Sprite selected
                    if (Control.ModifierKeys == Keys.Shift)
                    {
                        // Multiple selection
                        currentSpriteX = x;
                        currentSpriteY = y;
                    }
                    else
                    {
                        // New selection
                        currentSpriteX = selStartSprX = x;
                        currentSpriteY = selStartSprY = y;
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
                    if (currentSpriteY > 0)
                    {
                        currentSpriteY--;
                        this.RefreshAllViews();
                    }
                    break;
                case Keys.Down | Keys.Shift:
                    if (currentSpriteY < 7)
                    {
                        currentSpriteY++;
                        this.RefreshAllViews();
                    }
                    break;
                case Keys.Left | Keys.Shift:
                    if (currentSpriteX > 0)
                    {
                        currentSpriteX--;
                        this.RefreshAllViews();
                    }
                    break;
                case Keys.Right | Keys.Shift:
                    if (currentSpriteX < 7)
                    {
                        currentSpriteX++;
                        this.RefreshAllViews();
                    }
                    break;
                case Keys.Up:
                    if (currentSpriteY > 0)
                    {
                        currentSpriteY--;
                        selStartSprX = currentSpriteX;
                        selStartSprY = currentSpriteY;
                        this.RefreshAllViews();
                    }
                    break;
                case Keys.Down:
                    if (currentSpriteY < 7)
                    {
                        currentSpriteY++;
                        selStartSprX = currentSpriteX;
                        selStartSprY = currentSpriteY;
                        this.RefreshAllViews();
                    }
                    break;
                case Keys.Left:
                    if (currentSpriteX > 0)
                    {
                        currentSpriteX--;
                        selStartSprX = currentSpriteX;
                        selStartSprY = currentSpriteY;
                        this.RefreshAllViews();
                    }
                    break;
                case Keys.Right:
                    if (currentSpriteX < 7)
                    {
                        currentSpriteX++;
                        selStartSprX = currentSpriteX;
                        selStartSprY = currentSpriteY;
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
                currentSpriteX = Math.Min(p.X / 32, 7);
                currentSpriteY = Math.Min(p.Y / 32, 7);
                this.RefreshAllViews();
            }
        }
        private void contextSprites_copy(object sender, EventArgs e)
        {
            ClipSprite clip = new ClipSprite();
            // Copy selected sprites
            int x = Math.Min(currentSpriteX, selStartSprX);
            int y = Math.Min(currentSpriteY, selStartSprY);
            int w = Math.Abs(currentSpriteX - selStartSprX) + 1;
            int h = Math.Abs(currentSpriteY - selStartSprY) + 1;
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
                for (int i = 0; (i < clip.sprites.Count) && (currentSpriteY + i < 8); ++i)
                {
                    List<Machine.One16x16Sprite> l = clip.sprites[i];
                    for (int j = 0; (j < l.Count) && (currentSpriteX + j < 8); ++j)
                    {
                        int index16x16 = (currentSpriteY + i) * 8 + (currentSpriteX + j);
                        dataSource.Set16x16Sprite(index16x16, l[j], false);
                    }
                }
                this.RefreshAllViews();
            }
            else if (clip is ClipOneChrInRom)
            {
                MementoCaretaker.Instance.Push();
                // Copied from peek window
                int current8x8 = (currentSpriteY * 8 + currentSpriteX) * 4;
                dataSource.SetSpriteOverlay(currentSpriteY * 8 + currentSpriteX, false, false);
                dataSource.SetSpriteGen(current8x8 + 0, clip.leftTop, false);
                dataSource.SetSpriteGen(current8x8 + 1, clip.leftBottom, false);
                dataSource.SetSpriteGen(current8x8 + 2, clip.rightTop, false);
                dataSource.SetSpriteGen(current8x8 + 3, clip.rightBottom, false);
                this.RefreshAllViews();
            }
        }
        private void contextSprites_del(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            int x = Math.Min(currentSpriteX, selStartSprX);
            int y = Math.Min(currentSpriteY, selStartSprY);
            int w = Math.Abs(currentSpriteX - selStartSprX) + 1;
            int h = Math.Abs(currentSpriteY - selStartSprY) + 1;
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
            int current = currentSpriteY * 8 + currentSpriteX;
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
            int x = Math.Min(currentSpriteX, selStartSprX);
            int y = Math.Min(currentSpriteY, selStartSprY);
            int w = Math.Abs(currentSpriteX - selStartSprX) + 1;
            int h = Math.Abs(currentSpriteY - selStartSprY) + 1;
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
            int x = Math.Min(currentSpriteX, selStartSprX);
            int y = Math.Min(currentSpriteY, selStartSprY);
            int w = Math.Abs(currentSpriteX - selStartSprX) + 1;
            int h = Math.Abs(currentSpriteY - selStartSprY) + 1;
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
                if ((currentLineX != clicked_x) || (currentLineY != clicled_y))
                {
                    if (Control.ModifierKeys == Keys.Shift)
                    {
                        // Multiple selection
                        currentLineX = clicked_x;
                        currentLineY = clicled_y;
                    }
                    else
                    {
                        // New selection
                        currentLineX = selStartLineX = clicked_x;
                        currentLineY = selStartLineY = clicled_y;
                    }
                    this.UpdateSpriteEditView();
                    this.UpdateCurrentColorView();
                    viewSpriteEdit.DoDragDrop(new DnDEditor(), DragDropEffects.Copy);
                    return;
                }
                else
                {
                    // toggle the color of selected pixel
                    this.editCurrentSprite((e.X / 16) % 8, currentLineY % 8);
                }
            }
        }
        private void panelEditor_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Up | Keys.Shift:
                    if (currentLineY > 0)
                    {
                        currentLineY--;
                        this.UpdateSpriteEditView();
                        this.UpdateCurrentColorView();
                    }
                    break;
                case Keys.Down | Keys.Shift:
                    if (currentLineY < 15)
                    {
                        currentLineY++;
                        this.UpdateSpriteEditView();
                        this.UpdateCurrentColorView();
                    }
                    break;
                case Keys.Left | Keys.Shift:
                    if (currentLineX > 0)
                    {
                        currentLineX--;
                        this.UpdateSpriteEditView();
                        this.UpdateCurrentColorView();
                    }
                    break;
                case Keys.Right | Keys.Shift:
                    if (currentLineX < 1)
                    {
                        currentLineX++;
                        this.UpdateSpriteEditView();
                        this.UpdateCurrentColorView();
                    }
                    break;
                case Keys.Up:
                    if (currentLineY > 0)
                    {
                        currentLineY--;
                        selStartLineX = currentLineX;
                        selStartLineY = currentLineY;
                        this.UpdateSpriteEditView();
                        this.UpdateCurrentColorView();
                    }
                    break;
                case Keys.Down:
                    if (currentLineY < 15)
                    {
                        currentLineY++;
                        selStartLineX = currentLineX;
                        selStartLineY = currentLineY;
                        this.UpdateSpriteEditView();
                        this.UpdateCurrentColorView();
                    }
                    break;
                case Keys.Left:
                    if (currentLineX > 0)
                    {
                        currentLineX--;
                        selStartLineX = currentLineX;
                        selStartLineY = currentLineY;
                        this.UpdateSpriteEditView();
                        this.UpdateCurrentColorView();
                    }
                    break;
                case Keys.Right:
                    if (currentLineX < 1)
                    {
                        currentLineX++;
                        selStartLineX = currentLineX;
                        selStartLineY = currentLineY;
                        this.UpdateSpriteEditView();
                        this.UpdateCurrentColorView();
                    }
                    break;
                case Keys.D1:
                case Keys.NumPad1:
                    this.editCurrentSprite(0, currentLineY % 8);
                    break;
                case Keys.D2:
                case Keys.NumPad2:
                    this.editCurrentSprite(1, currentLineY % 8);
                    break;
                case Keys.D3:
                case Keys.NumPad3:
                    this.editCurrentSprite(2, currentLineY % 8);
                    break;
                case Keys.D4:
                case Keys.NumPad4:
                    this.editCurrentSprite(3, currentLineY % 8);
                    break;
                case Keys.D5:
                case Keys.NumPad5:
                    this.editCurrentSprite(4, currentLineY % 8);
                    break;
                case Keys.D6:
                case Keys.NumPad6:
                    this.editCurrentSprite(5, currentLineY % 8);
                    break;
                case Keys.D7:
                case Keys.NumPad7:
                    this.editCurrentSprite(6, currentLineY % 8);
                    break;
                case Keys.D8:
                case Keys.NumPad8:
                    this.editCurrentSprite(7, currentLineY % 8);
                    break;
                case Keys.Oemplus:
                case Keys.Add:
                case Keys.OemMinus:
                case Keys.Subtract:
                    {
                        int sprite_num_16x16 = currentSpriteY * 8 + currentSpriteX;
                        int sprite_num_8x8 = sprite_num_16x16 * 4 + currentLineX * 2 + currentLineY / 8;
                        int color_code_primary = dataSource.GetSpriteColorCode(sprite_num_8x8, currentLineY % 8);
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
                        this.setSpriteColor(sprite_num_16x16, color_code_primary);
                        this.RefreshAllViews();
                    }
                    break;
                case Keys.OemCloseBrackets:
                case Keys.OemOpenBrackets:
                    // Check overlays
                    if (dataSource.GetSpriteOverlay(currentSpriteY * 8 + currentSpriteX))
                    {
                        int sprite_num_16x16 = currentSpriteY * 8 + currentSpriteX;
                        int sprite_num_8x8 = sprite_num_16x16 * 4 + currentLineX * 2 + currentLineY / 8;
                        int sprite_num_8x8_secondary = (sprite_num_8x8 + 4) % 256;
                        int color_code_secondary = dataSource.GetSpriteColorCode(sprite_num_8x8_secondary, currentLineY % 8);
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
                        this.setSpriteColor(sprite_num_8x8_secondary / 4, color_code_secondary);
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
                currentLineX = Math.Min(p.X / 128, 1);
                currentLineY = Math.Min(p.Y / 16, 15);
                this.UpdateSpriteEditView();
                this.UpdateCurrentColorView();
            }
        }
        private void contextEditor_copy(object sender, EventArgs e)
        {
            ClipOneSpriteLine clip = new ClipOneSpriteLine();
            int x = Math.Min(currentLineX, selStartLineX);
            int y = Math.Min(currentLineY, selStartLineY);
            int w = Math.Abs(currentLineX - selStartLineX) + 1;
            int h = Math.Abs(currentLineY - selStartLineY) + 1;
            for (int i = y; i < y + h; ++i)
            {
                List<Machine.SpriteLine> l = new List<Machine.SpriteLine>();
                for (int j = x; j < x + w; ++j)
                {
                    int lefttop16x16 = currentSpriteY * 8 + currentSpriteX;
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
                for (int i = 0; (i < clip.lines.Count) && (currentLineY + i < 16); ++i)
                {
                    List<Machine.SpriteLine> l = clip.lines[i];
                    for (int j = 0; (j < l.Count) && (currentLineX + j < 2); ++j)
                    {
                        int lefttop16x16 = currentSpriteY * 8 + currentSpriteX;
                        int target8x8 = lefttop16x16 * 4 + (currentLineX + j) * 2 + ((currentLineY + i) / 8);
                        dataSource.SetSpriteLine(target8x8, (i + currentLineY) % 8, l[j], false);
                    }
                }
                this.UpdateSpriteEditView();
                this.UpdateSpriteView();
            }
        }
        private void contextEditor_del(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            int x = Math.Min(currentLineX, selStartLineX);
            int y = Math.Min(currentLineY, selStartLineY);
            int w = Math.Abs(currentLineX - selStartLineX) + 1;
            int h = Math.Abs(currentLineY - selStartLineY) + 1;
            for (int i = y; i < y + h; ++i)
            {
                for (int j = x; j < x + w; ++j)
                {
                    int lefttop16x16 = currentSpriteY * 8 + currentSpriteX;
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
            int x = Math.Min(currentLineX, selStartLineX);
            int y = Math.Min(currentLineY, selStartLineY);
            int w = Math.Abs(currentLineX - selStartLineX) + 1;
            int h = Math.Abs(currentLineY - selStartLineY) + 1;
            for (int i = y + 1; i < y + h; ++i)
            {
                for (int j = x; j < x + w; ++j)
                {
                    int lefttop16x16 = currentSpriteY * 8 + currentSpriteX;
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
            int x = Math.Min(currentLineX, selStartLineX);
            int y = Math.Min(currentLineY, selStartLineY);
            int w = Math.Abs(currentLineX - selStartLineX) + 1;
            int h = Math.Abs(currentLineY - selStartLineY) + 1;
            for (int i = y; i < y + h; ++i)
            {
                for (int j = x + 1; j < x + w; ++j)
                {
                    int lefttop16x16 = currentSpriteY * 8 + currentSpriteX;
                    int src = lefttop16x16 * 4 + (x * 2) + (i / 8);
                    Machine.SpriteLine line = dataSource.GetSpriteLine(src, i % 8);
                    int dst = lefttop16x16 * 4 + (j * 2) + (i / 8);
                    dataSource.SetSpriteLine(dst, i % 8, line, false);
                }
            }
            this.UpdateSpriteEditView();
            this.UpdateSpriteView();
        }
        private void checkTMS_Click(object sender, EventArgs e)
        {
            if (this.checkTMS.Checked && !dataSource.IsTMS9918())
            {
                // Set windows color of each color code to TMS9918
                dataSource.SetPaletteToTMS9918(true);
            }
            else if (!this.checkTMS.Checked && dataSource.IsTMS9918())
            {
                // Set windows color of each color code to internal palette
                dataSource.SetPaletteToV9938(true);
            }
            this.RefreshAllViews();     // Everything changes
        }
        private void viewColorL_Click(object sender, EventArgs e)
        {
            PaletteSelector palette_win = new PaletteSelector(bmpPalette, viewColorL_ColorSelectionCallback);
            palette_win.StartPosition = FormStartPosition.Manual;
            palette_win.Location = Cursor.Position;
            palette_win.Show();
        }
        private int viewColorL_ColorSelectionCallback(int val)
        {
            if (val == 0) return 0;
            int current_target16x16 = (currentSpriteY * 8 + currentSpriteX) % 64;
            this.setSpriteColor(current_target16x16, val);
            return 0;
        }
        private void viewColorR_Click(object sender, EventArgs e)
        {
            PaletteSelector palette_win = new PaletteSelector(bmpPalette, viewColorR_ColorSelectionCallback);
            palette_win.StartPosition = FormStartPosition.Manual;
            palette_win.Location = Cursor.Position;
            palette_win.Show();
        }
        private int viewColorR_ColorSelectionCallback(int val)
        {
            if (val == 0) return 0;
            int current_target16x16 = (currentSpriteY * 8 + currentSpriteX + 1) % 64;
            this.setSpriteColor(current_target16x16, val);
            return 0;
        }
        private void viewPalette_MouseClick(object sender, MouseEventArgs e)
        {
            // Palette view clicked
            int clicked_color_num = (e.Y / 32) * 8 + (e.X / 32);
            if (clicked_color_num == 0) return;
            // Update color table of current line
            int current_target16x16 = 0;
            if (e.Button == MouseButtons.Left)          // To current sprite
            {
                current_target16x16 = (currentSpriteY * 8 + currentSpriteX) % 64;
            }
            else if (e.Button == MouseButtons.Right)    // To overlayed sprite
            {
                if (this.checkOverlay.Checked == true)
                {
                    current_target16x16 = (currentSpriteY * 8 + currentSpriteX + 1) % 64;
                }
                else return;
            }
            this.setSpriteColor(current_target16x16, clicked_color_num);
        }
        private void viewPalette_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!checkTMS.Checked)
            {
                int clicked_color_num = (e.Y / 32) * 8 + (e.X / 32);
                int R = dataSource.GetPaletteR(clicked_color_num);
                int G = dataSource.GetPaletteG(clicked_color_num);
                int B = dataSource.GetPaletteB(clicked_color_num);
                PaletteEditor palette_win = new PaletteEditor(R, G, B);
                if (palette_win.ShowDialog() == DialogResult.OK)
                {
                    dataSource.SetPalette(clicked_color_num, palette_win.R,
                                          palette_win.G, palette_win.B, true);
                    this.RefreshAllViews();     // Everything changes
                }
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
        private void btnSavePalette_Click(object sender, EventArgs e)
        {
            String dir = Path.GetDirectoryName(currentFile);
            if (dir == null)
            {
                dir = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            }
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.InitialDirectory = dir;
            dlg.Filter = "PLT File(*.plt)|*.plt";
            dlg.FilterIndex = 1;
            dlg.Title = "Save palette";
            dlg.RestoreDirectory = true;
            dlg.OverwritePrompt = true;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                BinaryWriter br = new BinaryWriter(new FileStream(dlg.FileName, FileMode.Create));
                try
                {
                    dataSource.SavePaletteSettings(br);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    br.Close();
                }
            }
        }
        private void btnOpenPalette_Click(object sender, EventArgs e)
        {
            String dir = Path.GetDirectoryName(currentFile);
            if (dir == null)
            {
                dir = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            }
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = dir;
            dlg.Filter = "PLT File(*.plt)|*.plt";
            dlg.FilterIndex = 1;
            dlg.Title = "Load palette";
            dlg.RestoreDirectory = true;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                BinaryReader br = new BinaryReader(new FileStream(dlg.FileName, FileMode.Open));
                try
                {
                    dataSource.LoadPaletteSettings(br);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    br.Close();
                }
                this.RefreshAllViews();
            }
        }
        private void chkCRT_CheckedChanged(object sender, EventArgs e)
        {
            this.RefreshAllViews();
        }
        //----------------------------------------------------------------------
        // Utility

        private void setSpriteColor(int target16x16, int val)
        {
            int target8x8 = target16x16 * 4;
            if (dataSource.IsTMS9918())
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
                target8x8 = target8x8 + currentLineX * 2 + currentLineY / 8;
                dataSource.SetSpriteColorCode(target8x8, currentLineY % 8, val, true);
            }
            this.RefreshAllViews();     // Everything changes
        }
        private void editCurrentSprite(int x, int y)
        {
            int col = currentLineX;
            int row = currentLineY / 8;
            int target_lefttop16x16 = currentSpriteY * 8 + currentSpriteX;
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
                if (dataSource.IsTMS9918())
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
