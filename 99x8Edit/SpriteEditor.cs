using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace _99x8Edit
{
    // Sprite editor window
    public partial class SpriteEditor : Form
    {
        private readonly Machine dataSource;
        private readonly MainWindow mainWin;
        private TabOrder tabList = new TabOrder();
        private Bitmap bmpPreview = new Bitmap(32, 32);         // Edit preview
        internal String CurrentFile { get; set; }
        // For internal drag control
        private class DnDEditor { }
        //----------------------------------------------------------------------
        // Initialize
        public SpriteEditor(Machine src, MainWindow parent)
        {
            InitializeComponent();
            // Set corresponding data and owner window
            dataSource = src;
            mainWin = parent;
            // Tab order for the customed control
            tabList.Add(viewEdit, viewEdit.Selector);
            tabList.Add(viewColor, viewColor.Selector);
            tabList.Add(viewPalette, viewPalette.Selector);
            tabList.Add(viewSprite, viewSprite.Selector);
            // Initialize controls
            viewPreview.Image = bmpPreview;
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
            toolStripSprCopy.Click += new EventHandler(contextSprite_copy);
            toolStripSprPaste.Click += new EventHandler(contextSprite_paste);
            toolStripSprDel.Click += new EventHandler(contextSprite_del);
            toolStripSprReverse.Click += new EventHandler(contextSprite_reverse);
            toolStripSprCopyDown.Click += new EventHandler(contextSprite_copyDown);
            toolStripSprCopyRight.Click += new EventHandler(contextSprite_copyRight);
            toolStripRotateUp.Click += new EventHandler(contextSprite_rotate);
            toolStripRotateDown.Click += new EventHandler(contextSprite_rotate);
            toolStripRotateLeft.Click += new EventHandler(contextSprite_rotate);
            toolStripRotateRight.Click += new EventHandler(contextSprite_rotate);
            toolStripEditorCopy.Click += new EventHandler(contextEditor_copy);
            toolStripEditorPaste.Click += new EventHandler(contextEditor_paste);
            toolStripEditorDel.Click += new EventHandler(contextEditor_del);
            toolStripEditorCopyDown.Click += new EventHandler(contextEditor_copyDown);
            toolStripEditorCopyRight.Click += new EventHandler(contextEditor_copyRight);
            toolStripEditorCopyColor.Click += new EventHandler(contextEditor_copyColor);
            toolStripEditorInverse.Click += new EventHandler(contextEditor_inverse);
        }
        //------------------------------------------------------------------------------
        // Overrides
        protected override bool ProcessDialogKey(Keys keyData)
        {
            switch (keyData)
            {
                // prevent focus movement by the cursor and catch cursor keys
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
            // Set to next control
            Control prev = this.ActiveControl;
            Control next = tabList.NextOf(prev, forward);
            this.ActiveControl = next;
            // Animation
            Rectangle r_prev = tabList.SelectionOf(prev).GetScreenPos(prev);
            Rectangle r_next = tabList.SelectionOf(next).GetScreenPos(next);
            CursorAnimation win = new CursorAnimation(r_prev, r_next);
            win.Show();
            win.StartMoving();
            // Refresh views
            this.RefreshAllViews();
            return true;
        }
        //------------------------------------------------------------------------------
        // Views
        private void RefreshAllViews()
        {
            this.UpdatePaletteView(refresh: false);       // Palette view
            this.UpdateSpriteView(refresh: false);        // Sprites view
            this.UpdateSpriteEditView(refresh: false);    // Sprite edit view
            this.UpdateCurrentColorView(refresh: false);  // Current color
            this.UpdateOverlayCheck(refresh: false);
            this.chkTMS.Checked = dataSource.IsTMS9918;
            this.toolStripFileLoadPal.Enabled = !dataSource.IsTMS9918;
            this.toolStripFileSavePal.Enabled = !dataSource.IsTMS9918;
            this.Refresh();
        }
        private void UpdatePaletteView(bool refresh)
        {
            // Update palette view
            for (int i = 1; i < 16; ++i)
            {
                Color c = dataSource.ColorOf(i);
                viewPalette.SetBackground(c, i % viewPalette.ColumnNum, i / viewPalette.ColumnNum);
            }
            if (refresh) this.viewPalette.Refresh();
        }
        private void UpdateSpriteView(bool refresh)
        {
            for (int row = 0; row < 8; ++row)
            {
                for (int col = 0; col < 8; ++col)
                {
                    // Set four sprites in one 16x16 sprites
                    var bmps = dataSource.GetBitmapsForSprite16(row * 8 + col);
                    int index = 0;
                    foreach(Bitmap b in bmps)
                    {
                        int x = col * 2 + (index / 2);
                        int y = row * 2 + (index % 2);
                        viewSprite.SetImage(b, x, y);
                        ++index;
                    }
                }
            }
            // CRT Filter
            viewSprite.Filter = (chkCRT.Checked) ? Filter.Create(Filter.Type.CRT) : null;
            // Selection overlayed
            viewSprite.DrawOverlayedSelection = dataSource.GetSpriteOverlay(viewSprite.Index);
            if (refresh) viewSprite.Refresh();
        }
        private void UpdateSpriteEditView(bool refresh)
        {
            Utility.DrawTransparent(bmpPreview);
            Graphics preview = Graphics.FromImage(bmpPreview);
            int index16 = viewSprite.Index;
            bool overlayed = dataSource.GetSpriteOverlay(index16);
            for (int y = 0; y < 16; ++y)
            {
                for (int x = 0; x < 16; ++x)
                {
                    int color_code = 0;     // transparent as default
                    int ptn_but = dataSource.GetSpritePixel(index16, x, y, true);
                    if (ptn_but != 0)
                    {
                        // pixel exists, so get the color code
                        color_code = dataSource.GetSpriteColorCode(index16, y);
                    }
                    if (overlayed)
                    {
                        // Overlayed sprite
                        int index16ov = (index16 + 1) % 64;
                        int ptn_over = dataSource.GetSpritePixel(index16ov, x, y, true);
                        if (ptn_over != 0)
                        {
                            if (dataSource.IsTMS9918)
                            {
                                if (color_code == 0)
                                {
                                    // The color of overlayed sprite
                                    color_code = dataSource.GetSpriteColorCode(index16ov, y);
                                }
                                else
                                {
                                    // On TMS9918, the pixel of first sprite have priority
                                }
                            }
                            else
                            {
                                // On V9938, take OR of the colors of two sprites
                                color_code |= dataSource.GetSpriteColorCode(index16ov, y);
                            }
                        }
                    }
                    if (color_code != 0)
                    {
                        Brush b = dataSource.BrushOf(color_code);
                        viewEdit.SetBrush(b, x, y);
                        preview.FillRectangle(b, x * 2, y * 2, 2, 2);
                    }
                    else
                    {
                        viewEdit.SetBrush(null, x, y);
                    }
                }
            }
            // CRT Filter
            viewEdit.Filter = (chkCRT.Checked) ? Filter.Create(Filter.Type.CRT) : null;
            if (refresh) viewEdit.Refresh();
            if (refresh) viewPreview.Refresh();
        }
        void UpdateCurrentColorView(bool refresh)
        {
            int length = 1;
            // Draw current color of primary sprite
            int index16 = viewSprite.Index;
            int color_code_l = dataSource.GetSpriteColorCode(index16, viewEdit.Y);
            Color cl = dataSource.ColorOf(color_code_l);
            viewColor.SetBackground(cl, 0, 0);
            if (dataSource.GetSpriteOverlay(index16))
            {
                // Draw current color of overlayed sprite
                index16 = (index16 + 1) % 64;
                int color_code_r = dataSource.GetSpriteColorCode(index16, viewEdit.Y);
                Color cr = dataSource.ColorOf(color_code_r);
                viewColor.SetBackground(cr, 1, 0);
                length = 2;
                if (!dataSource.IsTMS9918)
                {
                    // Draw OR color of two sprites (V9938)
                    int color_code_or = color_code_l | color_code_r;
                    Color co = dataSource.ColorOf(color_code_or);
                    viewColor.SetBackground(co, 2, 0);
                    length = 3;
                    labelColorOR.Visible = true;
                }
                else
                {
                    labelColorOR.Visible = false;
                }
                labelColorR.Visible = true;
            }
            else
            {
                labelColorR.Visible = false;
            }
            // Width of color view is for available colors
            viewColor.Width = length * viewColor.CellWidth + 2;
            if(viewColor.X >= length)
            {
                viewColor.X = length - 1;   // Limit the selection
            }
            if (refresh) viewColor.Refresh();
        }
        private void UpdateOverlayCheck(bool refresh)
        {
            this.checkOverlay.Checked = dataSource.GetSpriteOverlay(viewSprite.Index);
            if (refresh) checkOverlay.Refresh();
        }
        //------------------------------------------------------------------------------
        // Controls
        //-------------------------------------------------------
        // Misc
        private void checkTMS_Click(object sender, EventArgs e)
        {
            if (chkTMS.Checked && !dataSource.IsTMS9918)
            {
                // Set windows color of each color code to TMS9918
                dataSource.SetPaletteToTMS9918(push: true);
            }
            else if (!chkTMS.Checked && dataSource.IsTMS9918)
            {
                // Set windows color of each color code to internal palette
                dataSource.SetPaletteToV9938(push: true);
            }
            this.RefreshAllViews();     // Everything changes
        }
        private void viewColor_Click(object sender, EventArgs e)
        {
            if(viewColor.X == 2)
            {
                // When OR color is clicked, open the list of OR colors 
                PaletteOrColors or_win = new PaletteOrColors(dataSource);
                or_win.Show();
                return;
            }
            // Color selection
            int index16 = viewSprite.Index;
            if(viewColor.X == 1)
            {
                index16 = (index16 + 1) % 64;  // For overlayed
            }
            int color_code = dataSource.GetSpriteColorCode(index16, viewEdit.Y);
            // Callback from the selector window
            Action<int> callback = (x) =>
            {
                if (x != 0)
                {
                    dataSource.SetSpriteColorCode(index16, viewEdit.Y, x, push: true);
                    this.RefreshAllViews();
                }
            };
            // Open the selector
            PaletteSelector win = new PaletteSelector(dataSource, color_code, callback);
            win.StartPosition = FormStartPosition.Manual;
            win.Location = Cursor.Position;
            win.Show();
        }
        private void viewPalette_MouseClick(object sender, MouseEventArgs e)
        {
            int index16 = 0;
            // Palette view has been clicked            
            int clicked_color_num = Math.Clamp((e.Y / viewPalette.CellHeight)
                                              * viewPalette.ColumnNum
                                              + (e.X / viewPalette.CellWidth), 0, 15);
            // Update selection
            viewPalette.X = clicked_color_num % 8;
            viewPalette.Y = clicked_color_num / 8;
            this.UpdatePaletteView(true);
            // Update color table of current line
            if (e.Button == MouseButtons.Left)
            {
                // Left click is for primary sprite
                index16 = viewSprite.Index;
            }
            else if ((e.Button == MouseButtons.Right) && (checkOverlay.Checked == true))
            {
                // Right click is for overlayed sprite
                index16 = (viewSprite.Index + 1) % 64;
            }
            dataSource.SetSpriteColorCode(index16, viewEdit.Y, clicked_color_num, push: true);
            this.RefreshAllViews();
        }
        private void viewPalette_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!chkTMS.Checked)
            {
                // Palette editing window
                int clicked_color_num = (e.Y / 32) * 8 + (e.X / 32);
                this.EditPalette(clicked_color_num);
            }
        }
        private void viewPalette_CellOnEdit(object sender, EventArgs e)
        {
            if (!chkTMS.Checked)
            {
                // Open the palette editor window
                this.EditPalette(viewPalette.Index);
            }
        }
        private void checkOverlay_Click(object sender, EventArgs e)
        {
            int index16 = viewSprite.Index;
            if (checkOverlay.Checked)
            {
                dataSource.SetSpriteOverlay(index16, overlay: true, push: true);
            }
            else
            {
                dataSource.SetSpriteOverlay(index16, overlay: false, push: true);
            }
            this.RefreshAllViews();
        }
        private void Sprites_Activated(object sender, EventArgs e)
        {
            // Redraw the views according to data at this timing
            this.RefreshAllViews();
        }
        private void chkCRT_CheckedChanged(object sender, EventArgs e)
        {
            this.RefreshAllViews();
        }
        //-------------------------------------------------------
        // Sprite selector
        private void viewSprite_SelectionChanged(object sender, EventArgs e)
        {
            this.RefreshAllViews();
        }
        private void contextSprite_copy(object sender, EventArgs e)
        {
            ClipSprite clip = new ClipSprite();
            // Copy selected sprites
            Rectangle r = viewSprite.SelectedRect;
            for (int i = r.Y; i < r.Y + r.Height; ++i)
            {
                List<Machine.OneSprite> l = new List<Machine.OneSprite>();
                for (int j = r.X; j < r.X + r.Width; ++j)
                {
                    // For each selected sprites
                    l.Add(dataSource.GetSpriteData(i * 8 + j));
                }
                clip.sprites.Add(l);
            }
            ClipboardWrapper.SetData(clip);
        }
        private void contextSprite_paste(object sender, EventArgs e)
        {
            dynamic clip = ClipboardWrapper.GetData();
            if (clip is ClipSprite)
            {
                MementoCaretaker.Instance.Push();
                Action<int, int, int, int> callback = (col, row, colcnt, rowcnt) =>
                {
                    // Paste each copied sprites
                    int index16 = viewSprite.IndexOf(col, row);
                    dataSource.SetSpriteData(index16, clip.sprites[rowcnt][colcnt], push: false);
                };
                viewSprite.ForEachSelection(viewSprite.X, viewSprite.Y,
                                       clip.sprites?[0]?.Count, clip.sprites?.Count, callback);
                this.RefreshAllViews();
            }
            else if (clip is ClipPeekedData)
            {
                MementoCaretaker.Instance.Push();
                // Copied from peek window
                for (int i = 0; (i < clip.peeked.Count / 2) && (viewSprite.Y + i < 8); ++i)
                {
                    // One row in peek window is 8 dots so we need a trick
                    List<byte[]> first_row = clip.peeked[i * 2 + 0];
                    List<byte[]> second_row = clip.peeked[i * 2 + 1];
                    for (int j = 0; (j < first_row.Count / 2) && (viewSprite.X + j < 8); ++j)
                    {
                        int index16 = (viewSprite.Y + i) * 8 + (viewSprite.X + j);
                        dataSource.SetSpriteOverlay(index16, overlay: false, push: false);
                        List<byte> gendata_16 = new List<byte>();
                        gendata_16.AddRange(first_row[j * 2 + 0]);
                        gendata_16.AddRange(second_row[j * 2 + 0]);
                        gendata_16.AddRange(first_row[j * 2 + 1]);
                        gendata_16.AddRange(second_row[j * 2 + 1]);
                        dataSource.SetSpriteGen(index16, gendata_16, push: false);
                    }
                }
                this.RefreshAllViews();
            }
        }
        private void contextSprite_del(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            Rectangle r = viewSprite.SelectedRect;
            Action<int, int> callback = (col, row) =>
            {
                // Delete each selected sprites
                int index16 = viewSprite.IndexOf(col, row);
                dataSource.Clear16x16Sprite(index16, push: false);
            };
            viewSprite.ForEachSelection(r, callback);
            this.RefreshAllViews();
        }
        private void contextSprite_reverse(object sender, EventArgs e)
        {
            int current = viewSprite.Index;
            int loop_cnt = dataSource.GetSpriteOverlay(current) ? 2 : 1;
            // Loop count for primary sprite and overlayed sprite
            for (int i = 0; i < loop_cnt; ++i)
            {
                // Each sprites, primary and overlayed
                int target16 = (current + i) % 64;
                for (int y = 0; y < 16; ++y)
                {
                    // Each rows
                    List<int> bits = new List<int>();   // Bits to write in one line
                    for (int x = 15; x >= 0; --x)
                    {
                        // Read from right to left
                        bits.Add(dataSource.GetSpritePixel(target16, x, y, true));
                    }
                    for (int x = 0; x < 16; ++x)
                    {
                        // Write from left to right
                        dataSource.SetSpritePixel(target16, x, y, bits[x], push: true);
                    }
                }
            }
            this.RefreshAllViews();
        }
        private void contextSprite_copyDown(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            Rectangle r = viewSprite.SelectedRect;
            Action<int, int> callback = (col, row) =>
            {
                // For each sprites
                int src = viewSprite.IndexOf(col, r.Y);
                int dst = viewSprite.IndexOf(col, row);
                Machine.OneSprite spr = dataSource.GetSpriteData(src);
                dataSource.SetSpriteData(dst, spr, push: false);

            };
            viewSprite.ForEachSelection(r.X, r.Y + 1, r.Width, r.Height - 1, callback);
            this.RefreshAllViews();
        }
        private void contextSprite_copyRight(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            Rectangle r = viewSprite.SelectedRect;
            Action<int, int> callback = (col, row) =>
            {
                // For each sprites
                int src = viewSprite.IndexOf(r.X, row);
                int dst = viewSprite.IndexOf(col, row);
                Machine.OneSprite spr = dataSource.GetSpriteData(src);
                dataSource.SetSpriteData(dst, spr, push: false);
            };
            viewSprite.ForEachSelection(r.X + 1, r.Y, r.Width - 1, r.Height, callback);
            this.RefreshAllViews();
        }
        private void contextSprite_rotate(object sender, EventArgs e)
        {
            if (sender == toolStripRotateUp)
            {
                dataSource.RotateSprite(viewSprite.Index, 0, -1, push: true);
                this.RefreshAllViews();
            }
            if (sender == toolStripRotateDown)
            {
                dataSource.RotateSprite(viewSprite.Index, 0, 1, push: true);
                this.RefreshAllViews();
            }
            if (sender == toolStripRotateLeft)
            {
                dataSource.RotateSprite(viewSprite.Index, -1, 0, push: true);
                this.RefreshAllViews();
            }
            if (sender == toolStripRotateRight)
            {
                dataSource.RotateSprite(viewSprite.Index, 1, 0, push: true);
                this.RefreshAllViews();
            }
        }
        //-------------------------------------------------------
        // Sprite editor
        private void viewEditor_CellOnEdit(object sender, EventArgs e)
        {
            // Toggle the color of selected pixel
            (int x, int y) = viewEdit.PosInEditor();
            this.EditCurrentSprite(x, y);
        }
        private void contextEditor_copy(object sender, EventArgs e)
        {
            ClipOneSpriteLine clip = new ClipOneSpriteLine();
            Rectangle r = viewEdit.SelectedRect;
            for (int i = r.Y; i < r.Y + r.Height; ++i)
            {
                List<Machine.SpriteLine> l = new List<Machine.SpriteLine>();
                for (int j = r.X; j < r.X + r.Width; ++j)
                {
                    // Copy each lines
                    int index16 = viewSprite.Index;
                    l.Add(dataSource.GetSpriteLine(index16, j, i));
                }
                clip.lines.Add(l);
            }
            ClipboardWrapper.SetData(clip);
        }
        private void contextEditor_copyColor(object sender, EventArgs e)
        {
            ClipOneSpriteLine clip = new ClipOneSpriteLine();
            Rectangle r = viewEdit.SelectedRect;
            for (int i = r.Y; i < r.Y + r.Height; ++i)
            {
                List<Machine.SpriteLine> l = new List<Machine.SpriteLine>();
                for (int j = r.X; j < r.X + r.Width; ++j)
                {
                    // Copy each lines
                    int index16 = viewSprite.Index;
                    Machine.SpriteLine line = dataSource.GetSpriteLine(index16, j, i);
                    line.colorOnly = true;
                    l.Add(line);
                }
                clip.lines.Add(l);
            }
            ClipboardWrapper.SetData(clip);
        }
        private void contextEditor_inverse(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            Rectangle r = viewEdit.SelectedRect;
            for (int i = r.Y; i < r.Y + r.Height; ++i)
            {
                for (int j = r.X; j < r.X + r.Width; ++j)
                {
                    // Copy each lines
                    int index16 = viewSprite.Index;
                    Machine.SpriteLine line = dataSource.GetSpriteLine(index16, j, i);
                    line.genData = (byte)~line.genData;
                    line.genDataOv = (byte)~line.genDataOv;
                    dataSource.SetSpriteLine(index16, j, i, line, false);
                }
            }
            this.UpdateSpriteEditView(refresh: true);
            this.UpdateSpriteView(refresh: true);
        }
        private void contextEditor_paste(object sender, EventArgs e)
        {
            dynamic clip = ClipboardWrapper.GetData();
            if (clip is ClipOneSpriteLine)
            {
                MementoCaretaker.Instance.Push();
                Action<int, int, int, int> callback = (col, row, colcnt, rowcnt) =>
                {
                    // Paste to each lines
                    dataSource.SetSpriteLine(viewSprite.Index, col, row,
                                             clip.lines[rowcnt][colcnt], push: false);
                };
                viewEdit.ForEachSelection(viewEdit.X, viewEdit.Y,
                                          clip.lines?[0]?.Count, clip.lines?.Count, callback);
                this.UpdateSpriteEditView(refresh: true);
                this.UpdateSpriteView(refresh: true);
            }
        }
        private void contextEditor_del(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            Rectangle r = viewEdit.SelectedRect;
            Action<int, int> callback = (col, row) =>
            {
                // Delete each lines
                dataSource.ClearSpriteLine(viewSprite.Index, col, row, push: false);
            };
            viewEdit.ForEachSelection(r, callback);
            this.UpdateSpriteEditView(refresh: true);
            this.UpdateSpriteView(refresh: true);
        }
        private void contextEditor_copyDown(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            Rectangle r = viewEdit.SelectedRect;
            Action<int, int> callback = (col, row) =>
            {
                // For each lines
                Machine.SpriteLine line = dataSource.GetSpriteLine(viewSprite.Index, col, r.Y);
                dataSource.SetSpriteLine(viewSprite.Index, col, row, line, push: false);
            };
            viewEdit.ForEachSelection(r.X, r.Y + 1, r.Width, r.Height - 1, callback);
            this.UpdateSpriteEditView(refresh: true);
            this.UpdateSpriteView(refresh: true);
        }
        private void contextEditor_copyRight(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            Rectangle r = viewEdit.SelectedRect;
            Action<int, int> callback = (col, row) =>
            {
                // For each lines
                Machine.SpriteLine line = dataSource.GetSpriteLine(viewSprite.Index, r.X, row);
                dataSource.SetSpriteLine(viewSprite.Index, col, row, line, push: false);
            };
            viewEdit.ForEachSelection(r.X + 1, r.Y, r.Width - 1, r.Height, callback);
            this.UpdateSpriteEditView(refresh: true);
            this.UpdateSpriteView(refresh: true);
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
            if (Utility.ImportDialogAndImport(CurrentFile,
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
            if (Utility.LoadDialogAndLoad(CurrentFile,
                                          "Sprite File(*.spr)|*.spr",
                                          "Load sprite settings",
                                          dataSource.LoadSprites,
                                          push: true,
                                          out _))
            {
                this.RefreshAllViews();
            }
        }
        private void menu_fileSaveSprite(object sender, EventArgs e)
        {
            Utility.SaveDialogAndSave(CurrentFile,
                                      "Sprite File(*.spr)|*.spr",
                                      "Save sprite settings",
                                      dataSource.SaveSprites,
                                      save_as: true,
                                      out _);
        }
        private void menu_fileLoadPalette(object sender, EventArgs e)
        {
            if (Utility.LoadDialogAndLoad(CurrentFile,
                                         "PLT File(*.plt)|*.plt",
                                         "Load palette",
                                         dataSource.LoadPaletteSettings,
                                         push: true,     // Push memento
                                         out _))
            {
                this.RefreshAllViews();
            }
        }
        private void menu_fileSavePalette(object sender, EventArgs e)
        {
            Utility.SaveDialogAndSave(CurrentFile,
                                      "PLT File(*.plt)|*.plt",
                                      "Save palette",
                                      dataSource.SavePaletteSettings,
                                      save_as: true,
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
        //---------------------------------------------------------------------
        // For the main window
        public void ChangeOccuredByHost()
        {
            this.RefreshAllViews();
        }
        //----------------------------------------------------------------------
        // Private use
        private void EditPalette(int index)
        {
            (int R, int G, int B) = dataSource.GetPalette(index);
            PaletteEditor palette_win = null;
            Action callback = () =>
            {
                dataSource.SetPalette(index,
                                      palette_win.R,
                                      palette_win.G,
                                      palette_win.B,
                                      push: true);
                this.RefreshAllViews();     // Everything changes
            };
            palette_win = new PaletteEditor(R, G, B, callback);
            palette_win.StartPosition = FormStartPosition.Manual;
            palette_win.Location = Cursor.Position;
            palette_win.Show();
        }
        private void EditCurrentSprite(int x, int y)
        {
            // check pixel of first sprite
            int index16 = viewSprite.Y * 8 + viewSprite.X;
            int target_prev_pixel = dataSource.GetSpritePixel(index16, x, y, true);
            // check pixel of overlayed sprite
            int index16ov = (index16 + 1) % 64;
            int target_prev_pixel_ov = dataSource.GetSpritePixel(index16ov, x, y, true);
            // current_status will be: 0:transparent, 1:first sprite, 2:second sprie, 3:both
            int current_stat = target_prev_pixel + (target_prev_pixel_ov << 1);
            // Toggle the status
            int target_stat = current_stat + 1;
            if (dataSource.GetSpriteOverlay(index16))   // Depends on overlay and vdp settings
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
            // So toggled status is, 0:transparent, 1:first sprite, 2:second sprie, 3:both
            // set pixel of first sprite when above is 1 or 3
            int pixel = ((target_stat == 1) || (target_stat == 3)) ? 1 : 0;
            dataSource.SetSpritePixel(index16, x, y, pixel, push: true);
            if (dataSource.GetSpriteOverlay(index16))
            {
                // set pixel of overlayed sprite when above is 2 or 3
                pixel = ((target_stat == 2) || (target_stat == 3)) ? 1 : 0;
                dataSource.SetSpritePixel(index16ov, x, y, pixel, push: true);
            }
            // Update views
            this.UpdateSpriteEditView(refresh: true);
            this.UpdateSpriteView(refresh: true);
        }

        private void viewEdit_AddKeyPressed(object sender, EditorControl.AddKeyEventArgs e)
        {
            if (e.KeyType == EditorControl.AddKeyEventArgs.Type.PlusMinus)
            {
                // Increment/Decrement the color of primary sprite
                int index16 = viewSprite.Index;
                int color_code = dataSource.GetSpriteColorCode(index16, viewEdit.Y);
                color_code = Math.Clamp(color_code + e.Value, 1, 15);   // Avoid transparent
                dataSource.SetSpriteColorCode(index16, viewEdit.Y, color_code, push: true);
                this.RefreshAllViews();
            }
            else if (e.KeyType == EditorControl.AddKeyEventArgs.Type.Brackets)
            {
                // Increment/Decrement the color of overlayed sprite
                int index16 = (viewSprite.Index + 1) % 64; // For overlayed
                int color_code = dataSource.GetSpriteColorCode(index16, viewEdit.Y);
                color_code = Math.Clamp(color_code + e.Value, 1, 15);   // Avoid transparent
                dataSource.SetSpriteColorCode(index16, viewEdit.Y, color_code, push: true);
                this.RefreshAllViews();
            }
        }
    }
}
