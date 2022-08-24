using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace _99x8Edit
{
    // Sprite editor window
    public partial class SpriteEditor : Form
    {
        private readonly Machine _dataSource;
        private readonly MainWindow _mainWin;
        private readonly TabOrder _tabList = new TabOrder();
        private readonly Bitmap _bmpPreview = new Bitmap(32, 32);         // Edit preview
        //----------------------------------------------------------------------
        // Initialize
        public SpriteEditor(Machine src, MainWindow parent)
        {
            InitializeComponent();
            // Set corresponding data and owner window
            _dataSource = src;
            _mainWin = parent;
            // Tab order for the customed control
            _tabList.Add(_viewEdit, _viewEdit.Selector);
            _tabList.Add(_viewColor, _viewColor.Selector);
            _tabList.Add(_viewPalette, _viewPalette.Selector);
            _tabList.Add(_viewSprite, _viewSprite.Selector);
            // Initialize controls
            _viewPreview.Image = _bmpPreview;
            // Refresh all views
            RefreshAllViews();
            // Menu bar
            _toolStripFileLoad.Click += menu_fileLoad;
            _toolStripFileSave.Click += menu_fileSave;
            _toolStripFileSaveAs.Click += menu_fileSaveAs;
            _toolStripFileImport.Click += menu_fileImport;
            _toolStripFileExport.Click += menu_fileExport;
            _toolStripFileLoadSprite.Click += menu_fileLoadSprite;
            _toolStripFileSaveSprite.Click += menu_fileSaveSprite;
            _toolStripFileLoadPal.Click += menu_fileLoadPalette;
            _toolStripFileSavePal.Click += menu_fileSavePalette;
            _toolStripEditUndo.Click += menu_editUndo;
            _toolStripEditRedo.Click += menu_editRedo;
            _toolStripEditCurrent.Click += menu_editColorCurrent;
            _toolStripEditToggle.Click += menu_editColorToggle;
            // context menu
            _toolStripSprCopy.Click += contextSprite_copy;
            _toolStripSprPaste.Click += contextSprite_paste;
            _toolStripSprDel.Click += contextSprite_del;
            _toolStripSprReverse.Click += contextSprite_reverse;
            _toolStripSprCopyDown.Click += contextSprite_copyDown;
            _toolStripSprCopyRight.Click += contextSprite_copyRight;
            _toolStripRotateUp.Click += contextSprite_rotate;
            _toolStripRotateDown.Click += contextSprite_rotate;
            _toolStripRotateLeft.Click += contextSprite_rotate;
            _toolStripRotateRight.Click += contextSprite_rotate;
            _toolStripEditorCopy.Click += contextEditor_copy;
            _toolStripEditorPaste.Click += contextEditor_paste;
            _toolStripEditorDel.Click += contextEditor_del;
            _toolStripEditorCopyDown.Click += contextEditor_copyDown;
            _toolStripEditorCopyRight.Click += contextEditor_copyRight;
            _toolStripEditorCopyColor.Click += contextEditor_copyColor;
            _toolStripEditorInverse.Click += contextEditor_inverse;
            _toolStripEditorPaint.Click += contextEditor_paint;
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
            Control next = _tabList.NextOf(prev, forward);
            this.ActiveControl = next;
            // Animation
            Rectangle r_prev = _tabList.SelectionOf(prev).GetScreenPos(prev);
            Rectangle r_next = _tabList.SelectionOf(next).GetScreenPos(next);
            var win = new CursorAnimation(r_prev, r_next);
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
            _chkTMS.Checked = _dataSource.Is9918;
            _toolStripFileLoadPal.Enabled = !_dataSource.Is9918;
            _toolStripFileSavePal.Enabled = !_dataSource.Is9918;
            _toolStripEditCurrent.Checked = (Config.Setting.EditControlType == EditType.Current);
            _toolStripEditToggle.Checked = (Config.Setting.EditControlType == EditType.Toggle);
            this.Refresh();
        }
        private void UpdatePaletteView(bool refresh)
        {
            // Update palette view
            for (int i = 1; i < 16; ++i)
            {
                Color c = _dataSource.ColorOf(i);
                _viewPalette.SetBackgroundColor(c, i % _viewPalette.ColumnNum,
                                               i / _viewPalette.ColumnNum);
            }
            if (refresh) this._viewPalette.Refresh();
        }
        private void UpdateSpriteView(bool refresh)
        {
            for (int row = 0; row < _viewSprite.SelectionColNum; ++row)
            {
                for (int col = 0; col < _viewSprite.SelectionRowNum; ++col)
                {
                    // Set four sprites in one 16x16 sprites
                    int index16 = _viewSprite.IndexOf(col, row);
                    var bmps = _dataSource.GetBitmapsForSprite16(index16);
                    int index_offset = 0;
                    foreach(Bitmap b in bmps)
                    {
                        int x = col * 2 + (index_offset / 2);
                        int y = row * 2 + (index_offset % 2);
                        _viewSprite.SetImage(b, x, y);
                        ++index_offset;
                    }
                }
            }
            // CRT Filter
            _viewSprite.Filter = (_chkCRT.Checked) ? Filter.Create(Filter.Type.CRT) : null;
            // Selection overlayed
            _viewSprite.DrawOverlayedSelection = _dataSource.GetSpriteOverlay(_viewSprite.Index);
            if (refresh) _viewSprite.Refresh();
        }
        private void UpdateSpriteEditView(bool refresh)
        {
            Utility.DrawTransparent(_bmpPreview);
            Graphics preview = Graphics.FromImage(_bmpPreview);
            int index16 = _viewSprite.Index;
            bool overlayed = _dataSource.GetSpriteOverlay(index16);
            for (int y = 0; y < _viewEdit.ColumnNum; ++y)
            {
                for (int x = 0; x < _viewEdit.RowNum; ++x)
                {
                    int color_code = 0;     // transparent as default
                    int ptn_but = _dataSource.GetSpritePixel(index16, x, y, true);
                    if (ptn_but != 0)
                    {
                        // pixel exists, so get the color code
                        color_code = _dataSource.GetSpriteColorCode(index16, y);
                    }
                    if (overlayed)
                    {
                        // Overlayed sprite
                        int index16ov = (index16 + 1) % 64;
                        int ptn_over = _dataSource.GetSpritePixel(index16ov, x, y, true);
                        if (ptn_over != 0)
                        {
                            if (_dataSource.Is9918)
                            {
                                if (color_code == 0)
                                {
                                    // The color of overlayed sprite
                                    color_code = _dataSource.GetSpriteColorCode(index16ov, y);
                                }
                                else
                                {
                                    // On TMS9918, the pixel of first sprite have priority
                                }
                            }
                            else
                            {
                                // On V9938, take OR of the colors of two sprites
                                color_code |= _dataSource.GetSpriteColorCode(index16ov, y);
                            }
                        }
                    }
                    if (color_code != 0)
                    {
                        Brush b = _dataSource.BrushOf(color_code);
                        _viewEdit.SetEditingDot(b, x, y);
                        preview.FillRectangle(b, x * 2, y * 2, 2, 2);
                    }
                    else
                    {
                        _viewEdit.SetEditingDot(null, x, y);
                    }
                }
            }
            // CRT Filter
            _viewEdit.Filter = (_chkCRT.Checked) ? Filter.Create(Filter.Type.CRT) : null;
            if (refresh) _viewEdit.Refresh();
            if (refresh) _viewPreview.Refresh();
        }
        void UpdateCurrentColorView(bool refresh)
        {
            int length = 2;
            // Draw current color of primary sprite
            int index16 = _viewSprite.Index;
            int color_code_l = _dataSource.GetSpriteColorCode(index16, _viewEdit.Y);
            Color cl = _dataSource.ColorOf(color_code_l);
            _viewColor.SetBackgroundColor(cl, 1, 0);
            if (_dataSource.GetSpriteOverlay(index16))
            {
                // Draw current color of overlayed sprite
                index16 = (index16 + 1) % 64;
                int color_code_r = _dataSource.GetSpriteColorCode(index16, _viewEdit.Y);
                Color cr = _dataSource.ColorOf(color_code_r);
                _viewColor.SetBackgroundColor(cr, 2, 0);
                length = 3;
                if (!_dataSource.Is9918)
                {
                    // Draw OR color of two sprites (V9938)
                    int color_code_or = color_code_l | color_code_r;
                    Color co = _dataSource.ColorOf(color_code_or);
                    _viewColor.SetBackgroundColor(co, 3, 0);
                    length = 4;
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
            _viewColor.Width = length * _viewColor.CellWidth + 2;
            if(_viewColor.X >= length)
            {
                _viewColor.X = length - 1;   // Limit the selection
            }
            if (refresh) _viewColor.Refresh();
        }
        private void UpdateOverlayCheck(bool refresh)
        {
            this._chkOverlay.Checked = _dataSource.GetSpriteOverlay(_viewSprite.Index);
            if (refresh) _chkOverlay.Refresh();
        }
        //------------------------------------------------------------------------------
        // Controls
        //-------------------------------------------------------
        // Misc
        private void checkTMS_Click(object sender, EventArgs e)
        {
            if (_chkTMS.Checked && !_dataSource.Is9918)
            {
                // Set windows color of each color code to TMS9918
                _dataSource.SetPaletteTo9918(push: true);
            }
            else if (!_chkTMS.Checked && _dataSource.Is9918)
            {
                // Set windows color of each color code to internal palette
                _dataSource.SetPaletteTo9938(push: true);
            }
            this.RefreshAllViews();     // Everything changes
        }
        private void viewColor_CellOnEdit(object sender, MatrixControl.EditEventArgs e)
        {
            if (_viewColor.X == 0)
            {
                // Current color is transparent
                return;
            }
            if (_viewColor.X == 3)
            {
                // When OR color is clicked, open the list of OR colors 
                PaletteOrColors or_win = new PaletteOrColors(_dataSource);
                or_win.Show();
                return;
            }
            // Color selection
            int index16 = _viewSprite.Index;
            if (_viewColor.X == 2)
            {
                index16 = (index16 + 1) % 64;  // For overlayed
            }
            int color_code = _dataSource.GetSpriteColorCode(index16, _viewEdit.Y);
            // Callback from the selector window
            Action<int> callback = (x) =>
            {
                if (x != 0)
                {
                    _dataSource.SetSpriteColorCode(index16, _viewEdit.Y, x, push: true);
                    this.RefreshAllViews();
                }
            };
            // Open the selector
            PaletteSelector win = new PaletteSelector(_dataSource, color_code, callback);
            win.StartPosition = FormStartPosition.Manual;
            win.Location = Cursor.Position;
            win.Show();
        }
        private void viewPalette_MouseClick(object sender, MouseEventArgs e)
        {
            int index16ov = 0;
            // Palette view has been clicked
            int clicked_color_num = _viewPalette.ScreenCoordinateToIndex(Cursor.Position);
            // Update selection
            _viewPalette.Index = clicked_color_num;
            this.UpdatePaletteView(true);
            // Update color table of current line
            if (e.Button == MouseButtons.Left)
            {
                // Left click is for primary sprite
                index16ov = _viewSprite.Index;
            }
            else if ((e.Button == MouseButtons.Right) && _chkOverlay.Checked)
            {
                // Right click is for overlayed sprite
                index16ov = (_viewSprite.Index + 1) % 64;
            }
            if(clicked_color_num != 0)  // Ignore transparent
            {
                _dataSource.SetSpriteColorCode(index16ov, _viewEdit.Y, clicked_color_num, push: true);
            }
            this.RefreshAllViews();
        }
        private void viewPalette_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!_chkTMS.Checked)
            {
                // Open the palette editor window
                this.EditPalette(_viewPalette.Index);
            }
        }
        private void viewPalette_CellOnEdit(object sender, MatrixControl.EditEventArgs e)
        {
            if (!_chkTMS.Checked)
            {
                // Open the palette editor window
                this.EditPalette(_viewPalette.Index);
            }
        }
        private void checkOverlay_Click(object sender, EventArgs e)
        {
            int index16 = _viewSprite.Index;
            _dataSource.SetSpriteOverlay(index16,
                                         overlay: (_chkOverlay.Checked),
                                         push: true);
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
            Rectangle r = _viewSprite.SelectedRect;
            for (int i = r.Y; i < r.Y + r.Height; ++i)
            {
                List<Machine.OneSprite> l = new List<Machine.OneSprite>();
                for (int j = r.X; j < r.X + r.Width; ++j)
                {
                    // For each selected sprites
                    l.Add(_dataSource.GetSpriteData(_viewSprite.IndexOf(j, i)));
                }
                clip.sprites.Add(l);
            }
            ClipboardWrapper.SetData(clip);
        }
        private void contextSprite_paste(object sender, EventArgs e)
        {
            dynamic clip = ClipboardWrapper.GetData();
            switch (clip)
            {
                case ClipSprite _:
                    MementoCaretaker.Instance.Push();
                    Action<int, int, int, int> callback = (col, row, colcnt, rowcnt) =>
                    {
                        // Paste each copied sprites
                        int index16 = _viewSprite.IndexOf(col, row);
                        _dataSource.SetSpriteData(index16, clip.sprites[rowcnt][colcnt], push: false);
                    };
                    _viewSprite.ForEachSelection(_viewSprite.X, _viewSprite.Y,
                        clip.sprites?[0]?.Count, clip.sprites?.Count, callback);
                    this.RefreshAllViews();
                    break;
                case ClipPeekedData _:
                    MementoCaretaker.Instance.Push();
                    // Copied from peek window
                    for (int i = 0; (i < clip.peeked.Count / 2)
                                    && (_viewSprite.Y + i < _viewSprite.SelectionRowNum); ++i)
                    {
                        // One row in peek window is 8 dots so we need a trick
                        List<byte[]> first_row = clip.peeked[i * 2 + 0];
                        List<byte[]> second_row = clip.peeked[i * 2 + 1];
                        for (int j = 0; (j < first_row.Count / 2)
                                        && (_viewSprite.X + j < _viewSprite.SelectionColNum); ++j)
                        {
                            int index16 = _viewSprite.IndexOf(_viewSprite.X + j, _viewSprite.Y + i);
                            _dataSource.SetSpriteOverlay(index16, overlay: false, push: false);
                            List<byte> gendata_16 = new List<byte>();
                            gendata_16.AddRange(first_row[j * 2 + 0]);
                            gendata_16.AddRange(second_row[j * 2 + 0]);
                            gendata_16.AddRange(first_row[j * 2 + 1]);
                            gendata_16.AddRange(second_row[j * 2 + 1]);
                            _dataSource.SetSpriteGen(index16, gendata_16, push: false);
                        }
                    }
                    this.RefreshAllViews();
                    break;
            }
        }
        private void contextSprite_del(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            Action<int, int> callback = (col, row) =>
            {
                // Delete each selected sprites
                int index16 = _viewSprite.IndexOf(col, row);
                _dataSource.ClearSprite(index16, push: false);
            };
            _viewSprite.ForEachSelection(callback);
            this.RefreshAllViews();
        }
        private void contextSprite_reverse(object sender, EventArgs e)
        {
            int current = _viewSprite.Index;
            // Loop count for primary sprite and overlayed sprite
            int loop_cnt = _dataSource.GetSpriteOverlay(current) ? 2 : 1;
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
                        bits.Add(_dataSource.GetSpritePixel(target16, x, y, true));
                    }
                    for (int x = 0; x < 16; ++x)
                    {
                        // Write from left to right
                        _dataSource.SetSpritePixel(target16, x, y, bits[x], push: true);
                    }
                }
            }
            this.RefreshAllViews();
        }
        private void contextSprite_copyDown(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            Rectangle r = _viewSprite.SelectedRect;
            Action<int, int> callback = (col, row) =>
            {
                // For each sprites
                int src = _viewSprite.IndexOf(col, r.Y);
                int dst = _viewSprite.IndexOf(col, row);
                Machine.OneSprite spr = _dataSource.GetSpriteData(src);
                _dataSource.SetSpriteData(dst, spr, push: false);

            };
            _viewSprite.ForEachSelection(r.X, r.Y + 1, r.Width, r.Height - 1, callback);
            this.RefreshAllViews();
        }
        private void contextSprite_copyRight(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            Rectangle r = _viewSprite.SelectedRect;
            Action<int, int> callback = (col, row) =>
            {
                // For each sprites
                int src = _viewSprite.IndexOf(r.X, row);
                int dst = _viewSprite.IndexOf(col, row);
                Machine.OneSprite spr = _dataSource.GetSpriteData(src);
                _dataSource.SetSpriteData(dst, spr, push: false);
            };
            _viewSprite.ForEachSelection(r.X + 1, r.Y, r.Width - 1, r.Height, callback);
            this.RefreshAllViews();
        }
        private void contextSprite_rotate(object sender, EventArgs e)
        {
            if (sender == _toolStripRotateUp)
            {
                _dataSource.RotateSprite(_viewSprite.Index, 0, -1, push: true);
                this.RefreshAllViews();
            }
            if (sender == _toolStripRotateDown)
            {
                _dataSource.RotateSprite(_viewSprite.Index, 0, 1, push: true);
                this.RefreshAllViews();
            }
            if (sender == _toolStripRotateLeft)
            {
                _dataSource.RotateSprite(_viewSprite.Index, -1, 0, push: true);
                this.RefreshAllViews();
            }
            if (sender == _toolStripRotateRight)
            {
                _dataSource.RotateSprite(_viewSprite.Index, 1, 0, push: true);
                this.RefreshAllViews();
            }
        }
        //-------------------------------------------------------
        // Sprite editor
        private void viewEditor_CellOnEdit(object sender, MatrixControl.EditEventArgs e)
        {
            // Won't let undo when dragging
            bool push = e.ShouldPush;
            // current_status is: 0:transparent, 1:first sprite, 2:second sprie, 3:both
            (int x, int y) = _viewEdit.PosInEditor();
            int current_stat = this.GetDotStatus(x, y);
            int updated_stat;
            if (Config.Setting.EditControlType == EditType.Current)
            {
                // Set the pixel to current color
                updated_stat = _viewColor.X;
            }
            else
            {
                // Toggle the status of pixel
                updated_stat = current_stat + 1;
                int index16 = _viewSprite.Index;
                if (_dataSource.GetSpriteOverlay(index16))   // Depends on overlay and vdp settings
                {
                    // Limit the max value which depends on overlay and VDP
                    if (_dataSource.Is9918)
                    {
                        updated_stat %= 3;       // Overlayed, no OR color
                    }
                    else
                    {
                        updated_stat %= 4;       // Overlayed, OR color available
                    }
                }
                else
                {
                    updated_stat %= 2;           // No overlay
                }
            }
            if (updated_stat != current_stat)
            {
                // Update the dot status
                this.SetDotStatus(x, y, updated_stat, push);
                // Update views
                this.UpdateSpriteEditView(refresh: true);
                this.UpdateSpriteView(refresh: true);
            }
        }
        private void viewEdit_SelectionChanged(object sender, EventArgs e)
        {
            // Current line has changed
            this.UpdateCurrentColorView(true);
        }
        private void contextEditor_paint(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            (int x, int y) = _viewEdit.PosInEditor();
            // Acquire current color code to set
            int color_l = _dataSource.GetSpriteColorCode(_viewSprite.Index, y);
            bool overlay = _dataSource.GetSpriteOverlay(_viewSprite.Index);
            int index16ov = (_viewSprite.Index + 1) % 64;
            int color_r = _dataSource.GetSpriteColorCode(index16ov, y);
            this.PaintSprite(x, y, color_l, color_r, overlay, _viewColor.X);
            this.RefreshAllViews();
        }
        private void contextEditor_copy(object sender, EventArgs e)
        {
            ClipOneSpriteLine clip = new ClipOneSpriteLine();
            Rectangle r = _viewEdit.SelectedRect;
            for (int i = r.Y; i < r.Y + r.Height; ++i)
            {
                List<Machine.SpriteLine> l = new List<Machine.SpriteLine>();
                for (int j = r.X; j < r.X + r.Width; ++j)
                {
                    // Copy each lines
                    int index16 = _viewSprite.Index;
                    l.Add(_dataSource.GetSpriteLine(index16, j, i));
                }
                clip.lines.Add(l);
            }
            ClipboardWrapper.SetData(clip);
        }
        private void contextEditor_copyColor(object sender, EventArgs e)
        {
            ClipOneSpriteLine clip = new ClipOneSpriteLine();
            Rectangle r = _viewEdit.SelectedRect;
            for (int i = r.Y; i < r.Y + r.Height; ++i)
            {
                List<Machine.SpriteLine> l = new List<Machine.SpriteLine>();
                for (int j = r.X; j < r.X + r.Width; ++j)
                {
                    // Copy each lines
                    int index16 = _viewSprite.Index;
                    Machine.SpriteLine line = _dataSource.GetSpriteLine(index16, j, i);
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
            Rectangle r = _viewEdit.SelectedRect;
            for (int i = r.Y; i < r.Y + r.Height; ++i)
            {
                for (int j = r.X; j < r.X + r.Width; ++j)
                {
                    // Copy each lines
                    int index16 = _viewSprite.Index;
                    Machine.SpriteLine line = _dataSource.GetSpriteLine(index16, j, i);
                    line.genData = (byte)~line.genData;
                    line.genDataOv = (byte)~line.genDataOv;
                    _dataSource.SetSpriteLine(index16, j, i, line, false);
                }
            }
            this.UpdateSpriteEditView(refresh: true);
            this.UpdateSpriteView(refresh: true);
        }
        private void contextEditor_paste(object sender, EventArgs e)
        {
            dynamic clip = ClipboardWrapper.GetData();
            switch (clip)
            {
                case ClipOneSpriteLine _:
                    MementoCaretaker.Instance.Push();
                    Action<int, int, int, int> callback = (col, row, colcnt, rowcnt) =>
                    {
                        // Paste to each lines
                        _dataSource.SetSpriteLine(_viewSprite.Index, col, row,
                            clip.lines[rowcnt][colcnt], push: false);
                    };
                    _viewEdit.ForEachSelection(_viewEdit.X, _viewEdit.Y,
                        clip.lines?[0]?.Count, clip.lines?.Count, callback);
                    this.UpdateSpriteEditView(refresh: true);
                    this.UpdateSpriteView(refresh: true);
                    break;
            }
        }
        private void contextEditor_del(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            Action<int, int> callback = (col, row) =>
            {
                // Delete each lines
                _dataSource.ClearSpriteLine(_viewSprite.Index, col, row, push: false);
            };
            _viewEdit.ForEachSelection(callback);
            this.UpdateSpriteEditView(refresh: true);
            this.UpdateSpriteView(refresh: true);
        }
        private void contextEditor_copyDown(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            Rectangle r = _viewEdit.SelectedRect;
            Action<int, int> callback = (col, row) =>
            {
                // For each lines
                Machine.SpriteLine line = _dataSource.GetSpriteLine(_viewSprite.Index, col, r.Y);
                _dataSource.SetSpriteLine(_viewSprite.Index, col, row, line, push: false);
            };
            _viewEdit.ForEachSelection(r.X, r.Y + 1, r.Width, r.Height - 1, callback);
            this.UpdateSpriteEditView(refresh: true);
            this.UpdateSpriteView(refresh: true);
        }
        private void contextEditor_copyRight(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            Rectangle r = _viewEdit.SelectedRect;
            Action<int, int> callback = (col, row) =>
            {
                // For each lines
                Machine.SpriteLine line = _dataSource.GetSpriteLine(_viewSprite.Index, r.X, row);
                _dataSource.SetSpriteLine(_viewSprite.Index, col, row, line, push: false);
            };
            _viewEdit.ForEachSelection(r.X + 1, r.Y, r.Width - 1, r.Height, callback);
            this.UpdateSpriteEditView(refresh: true);
            this.UpdateSpriteView(refresh: true);
        }
        //---------------------------------------------------------------------
        // Menu controls
        private void menu_fileLoad(object sender, EventArgs e)
        {
            _mainWin.LoadProject(sender, e);
        }
        private void menu_fileSave(object sender, EventArgs e)
        {
            _mainWin.SaveProject(sender, e);
        }
        private void menu_fileSaveAs(object sender, EventArgs e)
        {
            _mainWin.SaveAsProject(sender, e);
        }
        private void menu_fileImport(object sender, EventArgs e)
        {
            if (Utility.ImportDialogAndImport(Config.Setting.ImportDirectory,
                                              Import.SpriteTypeFilter,
                                              "Select file to import",
                                              _dataSource.ImportSprite,
                                              out string imported_file))
            {
                Config.Setting.ImportDirectory = Path.GetDirectoryName(imported_file);
                this.RefreshAllViews();
            }
        }
        private void menu_fileExport(object sender, EventArgs e)
        {
            _mainWin.ExportSprite(sender, e);
        }
        private void menu_fileLoadSprite(object sender, EventArgs e)
        {
            if (Utility.LoadDialogAndLoad(Config.Setting.SpriteFileDirectory,
                                          "Sprite File(*.spr)|*.spr",
                                          "Load sprite settings",
                                          _dataSource.LoadSprites,
                                          push: true,
                                          out string loaded_file))
            {
                Config.Setting.SpriteFileDirectory = Path.GetDirectoryName(loaded_file);
                this.RefreshAllViews();
            }
        }
        private void menu_fileSaveSprite(object sender, EventArgs e)
        {
            if(Utility.SaveDialogAndSave(Config.Setting.SpriteFileDirectory,
                                        "Sprite File(*.spr)|*.spr",
                                        "Save sprite settings",
                                        _dataSource.SaveSprites,
                                        save_as: true,
                                        out string saved_file))
            {
                Config.Setting.SpriteFileDirectory = Path.GetDirectoryName(saved_file);
            }
        }
        private void menu_fileLoadPalette(object sender, EventArgs e)
        {
            if (Utility.LoadDialogAndLoad(Config.Setting.PaletteDirectory,
                                         "PLT File(*.plt)|*.plt",
                                         "Load palette",
                                         _dataSource.LoadPaletteSettings,
                                         push: true,     // Push memento
                                         out string loaded_file))
            {
                Config.Setting.PaletteDirectory = Path.GetDirectoryName(loaded_file);
                this.RefreshAllViews();
            }
        }
        private void menu_fileSavePalette(object sender, EventArgs e)
        {
            if(Utility.SaveDialogAndSave(Config.Setting.PaletteDirectory,
                                      "PLT File(*.plt)|*.plt",
                                      "Save palette",
                                      _dataSource.SavePaletteSettings,
                                      save_as: true,
                                      out string saved_file))
            {
                Config.Setting.PaletteDirectory = Path.GetDirectoryName(saved_file);
            }
        }
        private void menu_editUndo(object sender, EventArgs e)
        {
            _mainWin.Undo();
        }
        private void menu_editRedo(object sender, EventArgs e)
        {
            _mainWin.Redo();
        }
        private void menu_editColorCurrent(object sender, EventArgs e)
        {
            _toolStripEditCurrent.Checked = true;
            _toolStripEditToggle.Checked = false;
            Config.Setting.EditControlType = EditType.Current;
        }
        private void menu_editColorToggle(object sender, EventArgs e)
        {
            _toolStripEditCurrent.Checked = false;
            _toolStripEditToggle.Checked = true;
            Config.Setting.EditControlType = EditType.Toggle;
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
            Action<int, int, int> callback = (r, g, b) =>
            {
                _dataSource.SetPalette(index, r, g, b, push: true);
                this.RefreshAllViews();     // Everything changes
            };
            (int R, int G, int B) = _dataSource.GetPalette(index);
            PaletteEditor palette_win = new PaletteEditor(R, G, B, callback);
            palette_win.StartPosition = FormStartPosition.Manual;
            palette_win.Location = Cursor.Position;
            palette_win.Show();
        }
        private int GetDotStatus(int x, int y)
        {
            // check pixel of first sprite
            int index16 = _viewSprite.Index;
            int target_prev_pixel = _dataSource.GetSpritePixel(index16, x, y, true);
            // check pixel of overlayed sprite
            int index16ov = (index16 + 1) % 64;
            int target_prev_pixel_ov = _dataSource.GetSpritePixel(index16ov, x, y, true);
            // current_status will be: 0:transparent, 1:first sprite, 2:second sprie, 3:both
            int current_stat = target_prev_pixel + (target_prev_pixel_ov << 1);
            return current_stat;
        }
        private void SetDotStatus(int x, int y, int val, bool push)
        {
            if (push) MementoCaretaker.Instance.Push();
            int index16 = _viewSprite.Index;
            int index16ov = (index16 + 1) % 64;
            // Updated status is, 0:transparent, 1:first sprite, 2:second sprie, 3:both
            // set pixel of first sprite when above is 1 or 3
            int pixel = ((val == 1) || (val == 3)) ? 1 : 0;
            _dataSource.SetSpritePixel(index16, x, y, pixel, push: false);
            if (_dataSource.GetSpriteOverlay(index16))
            {
                // set pixel of overlayed sprite when above is 2 or 3
                pixel = ((val == 2) || (val == 3)) ? 1 : 0;
                _dataSource.SetSpritePixel(index16ov, x, y, pixel, false);
            }
        }
        private void PaintSprite(int x, int y, int color_l, int color_r, bool overlay, int status)
        {
            // This won't be intuitive since the colors depend on line, but still useful
            int prev_dot_status = this.GetDotStatus(x, y);
            if (prev_dot_status == status) return;
            // Paint dot status
            this.SetDotStatus(x, y, status, push: false);
            // Overwrite color code also
            _dataSource.SetSpriteColorCode(_viewSprite.Index, y, color_l, false);
            if(overlay)
            {
                int index16ov = (_viewSprite.Index + 1) % 64;
                _dataSource.SetSpriteColorCode(index16ov, y, color_r, false);
            }
            if (y > 0)
                if (this.GetDotStatus(x, y - 1) == prev_dot_status)
                    this.PaintSprite(x, y - 1, color_l, color_r, overlay, status);
            if (y < 15)
                if (this.GetDotStatus(x, y + 1) == prev_dot_status)
                    this.PaintSprite(x, y + 1, color_l, color_r, overlay, status);
            if (x > 0)
                if (this.GetDotStatus(x - 1, y) == prev_dot_status)
                    this.PaintSprite(x - 1, y, color_l, color_r, overlay, status);
            if (x < 15)
                if (this.GetDotStatus(x + 1, y) == prev_dot_status)
                    this.PaintSprite(x + 1, y, color_l, color_r, overlay, status);
        }

        private void viewEdit_AddKeyPressed(object sender, EditorControl.AddKeyEventArgs e)
        {
            if (e.KeyType == EditorControl.AddKeyEventArgs.Type.PlusMinus)
            {
                // Increment/Decrement the color of primary sprite
                int index16 = _viewSprite.Index;
                int color_code = _dataSource.GetSpriteColorCode(index16, _viewEdit.Y);
                color_code = Math.Clamp(color_code + e.Value, 1, 15);   // Avoid transparent
                _dataSource.SetSpriteColorCode(index16, _viewEdit.Y, color_code, push: true);
                this.RefreshAllViews();
            }
            else if (e.KeyType == EditorControl.AddKeyEventArgs.Type.Brackets)
            {
                // Increment/Decrement the color of overlayed sprite
                int index16 = (_viewSprite.Index + 1) % 64; // For overlayed
                int color_code = _dataSource.GetSpriteColorCode(index16, _viewEdit.Y);
                color_code = Math.Clamp(color_code + e.Value, 1, 15);   // Avoid transparent
                _dataSource.SetSpriteColorCode(index16, _viewEdit.Y, color_code, push: true);
                this.RefreshAllViews();
            }
        }
    }
}
