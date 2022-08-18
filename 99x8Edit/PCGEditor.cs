using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace _99x8Edit
{
    // PCG editor window
    public partial class PCGEditor : Form
    {
        private readonly Machine dataSource;
        private readonly MainWindow mainWin;
        private TabOrder tabList = new TabOrder();
        private Bitmap bmpColorL = new Bitmap(32, 32);      // Color view
        private Bitmap bmpColorR = new Bitmap(32, 32);
        internal String CurrentFile { get; set; }
        // For internal drag control
        private class DnDPCG {
            internal ClipPCG Data { get; set; }
        }
        private class DnDEditor { }
        //------------------------------------------------------------------------------
        // Initialize
        public PCGEditor(Machine src, MainWindow parent)
        {
            InitializeComponent();
            // Set corresponding data and owner window
            dataSource = src;
            mainWin = parent;
            // Tab order for the customed control
            tabList.Add(viewEdit, viewEdit.Selector);
            tabList.Add(viewColor, viewColor.Selector);
            tabList.Add(viewPalette, viewPalette.Selector);
            tabList.Add(viewPCG, viewPCG.Selector);
            tabList.Add(viewSand, viewSand.Selector);
            // Initialize controls
            chkTMS.Checked = this.dataSource.IsTMS9918;
            // Refresh all views
            this.RefreshAllViews();
            // Menu bar
            toolStripFileLoad.Click += new EventHandler(menu_fileLoad);
            toolStripFileSave.Click += new EventHandler(menu_fileSave);
            toolStripFileSaveAs.Click += new EventHandler(menu_fileSaveAs);
            toolStripFileImport.Click += new EventHandler(menu_fileImport);
            toolStripFileExport.Click += new EventHandler(menu_fileExport);
            toolStripFileLoadPCG.Click += new EventHandler(menu_fileLoadPCG);
            toolStripFileSavePCG.Click += new EventHandler(menu_fileSavePCG);
            toolStripFileLoadPal.Click += new EventHandler(menu_loadPalette);
            toolStripFileSavePal.Click += new EventHandler(menu_savePalette);
            toolStripEditUndo.Click += new EventHandler(menu_editUndo);
            toolStripEditRedo.Click += new EventHandler(menu_editRedo);
            // Context menu
            toolStripPCGCopy.Click += new EventHandler(contextPCG_copy);
            toolStripPCGPaste.Click += new EventHandler(contextPCG_paste);
            toolStripPCGDel.Click += new EventHandler(contextPCG_delete);
            toolStripPCGInverse.Click += new EventHandler(contextPCG_inverse);
            toolStripPCGCopyDown.Click += new EventHandler(contextPCG_copyDown);
            toolStripPCGCopyRight.Click += new EventHandler(contextPCG_copyRight);
            toolStripSandboxCopy.Click += new EventHandler(contextSand_copy);
            toolStripSandboxPaste.Click += new EventHandler(contextSand_paste);
            toolStripSandboxDel.Click += new EventHandler(contextSand_delete);
            toolStripSandboxPaint.Click += new EventHandler(contextSand_paint);
            toolStripSandboxCopyDown.Click += new EventHandler(contextSand_copyDown);
            toolStripSandboxCopyRight.Click += new EventHandler(contextSand_copyRight);
            toolStripEditorCopy.Click += new EventHandler(contextEditor_copy);
            toolStripEditorPaste.Click += new EventHandler(contextEditor_paste);
            toolStripEditorDel.Click += new EventHandler(contextEditor_delete);
            toolStripEditorCopyDown.Click += new EventHandler(contextEditor_copyDown);
            toolStripEditorCopyRight.Click += new EventHandler(contextEditor_copyRight);
        }
        //------------------------------------------------------------------------------
        // Override
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
            this.UpdatePCGList(refresh: false);           // PCG view
            this.UpdateSandbox(refresh: false);           // Sandbox view
            this.UpdatePCGEditView(refresh: false);       // PCG Editor
            this.UpdateCurrentColorView(refresh: false);  // Current color
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
        private void UpdatePCGEditView(bool refresh)
        {
            // Update PCG editor
            for (int i = 0; i < viewEdit.ChrRowNum; ++i)
            {
                for (int j = 0; j < viewEdit.ChrColNum; ++j)
                {
                    // Draw each characters in one editor
                    int index = viewPCG.Index + i * viewPCG.ColumnNum + j;
                    for (int y = 0; y < 8; ++y)
                    {
                        for (int x = 0; x < 8; ++x)
                        {
                            // One dot in one pcg
                            int p = dataSource.GetPCGPixel(index, y, x);
                            int code = dataSource.GetPCGColor(index, y, foreground: (p != 0));
                            Brush b = dataSource.BrushOf(code);
                            viewEdit.SetBrush((code != 0) ? b : null, j * 8 + x, i * 8 + y);
                        }
                    }
                }
            }
            if (refresh) viewEdit.Refresh();
        }
        private void UpdateCurrentColorView(bool refresh)
        {
            int target = this.TargetPCG();
            // Draw foreground color
            Graphics gl = Graphics.FromImage(bmpColorL);
            int color_code_l = dataSource.GetPCGColor(target, viewEdit.Y % 8, foreground: true);
            if (color_code_l > 0)
            {
                Brush b = dataSource.BrushOf(color_code_l);
                gl.FillRectangle(b, 0, 0, 32, 32);
            }
            else
            {
                Utility.DrawTransparent(bmpColorL);
            }
            viewColor.SetImage(bmpColorL, 0, 0);
            // Draw background color
            Graphics gr = Graphics.FromImage(bmpColorR);
            int color_code_r = dataSource.GetPCGColor(target, viewEdit.Y % 8, foreground: false);
            if (color_code_r > 0)
            {
                Brush b = dataSource.BrushOf(color_code_r);
                gr.FillRectangle(b, 0, 0, 32, 32);
            }
            else
            {
                Utility.DrawTransparent(bmpColorR);
            }
            viewColor.SetImage(bmpColorR, 1, 0);
            if (refresh)
            {
                viewColor.Refresh();
            }
        }
        private void UpdatePCGList(bool refresh)
        {
            // Draw the PCG list
            for (int y = 0; y < viewPCG.RowNum; ++y)
            {
                for (int x = 0; x < viewPCG.ColumnNum; ++x)
                {
                    int pcg = y * viewPCG.ColumnNum + x;
                    viewPCG.SetImage(dataSource.GetBitmapOfPCG(pcg), x, y);
                }
            }
            // CRT Filter
            viewPCG.Filter = (chkCRT.Checked) ? Filter.Create(Filter.Type.CRT) : null;
            if (refresh)
            {
                viewPCG.Refresh();
            }
        }
        private void UpdateSandbox(bool refresh)
        {
            // Draw the sandbox
            for (int y = 0; y < viewSand.RowNum; ++y)
            {
                for (int x = 0; x < viewSand.ColumnNum; ++x)
                {
                    int pcg = dataSource.GetNameTable(y * viewSand.ColumnNum + x);
                    Bitmap src = dataSource.GetBitmapOfPCG(pcg);
                    viewSand.SetImage(src, x, y);
                }
            }
            // CRT Filter
            viewSand.Filter = (chkCRT.Checked) ? Filter.Create(Filter.Type.CRT) : null;
            if (refresh)
            {
                viewSand.Refresh();
            }
        }
        //-----------------------------------------------------------------------------
        // Controls
        //------------------------------------------------
        // Editor
        private void viewEdit_CellOnEdit(object sender, EventArgs e)
        {
            (int x, int y) = viewEdit.PosInChr();              // Dot in one PCG
            int index = viewPCG.Index
                      + (viewEdit.Y / 8) * viewPCG.ColumnNum
                      + viewEdit.X;                            // Selected character
            int prev_pixel = dataSource.GetPCGPixel(index, y, x);
            // Toggle On/Off of the pixel
            dataSource.SetPCGPixel(index, y, x, (prev_pixel == 0) ? 1 : 0, push: true);
            this.UpdatePCGEditView(refresh: true);   // PCG Editor view changes
            this.UpdatePCGList(refresh: true);       // PCG list view changes also
            this.UpdateSandbox(refresh: true);       // Update sandbox view
        }
        private void viewEdit_SelectionChanged(object sender, EventArgs e)
        {
            this.UpdateCurrentColorView(refresh: true);
        }
        private void contextEditor_copy(object sender, EventArgs e)
        {
            ClipPCGLines clip = new ClipPCGLines();
            Rectangle r = viewEdit.SelectedRect;
            for (int i = r.Y; i < r.Y + r.Height; ++i)
            {
                List<(byte, byte)> l = new List<(byte, byte)>();
                for (int j = r.X; j < r.X + r.Width; ++j)
                {
                    // Copy each selected lines
                    int index = viewPCG.Index;
                    int target = this.TargetPCG(index, j, i);
                    (byte gen, byte color) = dataSource.GetPCGLine(target, i % 8);
                    l.Add((gen, color));
                }
                clip.lines.Add(l);
            }
            ClipboardWrapper.SetData(clip);
        }
        private void contextEditor_paste(object sender, EventArgs e)
        {
            dynamic clip = ClipboardWrapper.GetData();
            if (clip is ClipPCGLines)
            {
                MementoCaretaker.Instance.Push();
                Rectangle r = viewEdit.SelectedRect;
                Action<int, int, int, int> callback = (col, row, colcnt, rowcnt) =>
                {
                    // Paste each copied lines
                    int target = this.TargetPCG(viewPCG.Index, col, row);
                    List<(byte, byte)> l = clip.lines[rowcnt];
                    (byte gen, byte color) = l[colcnt];
                    dataSource.SetPCGLine(target, row % 8, gen, color, push: false);
                };
                viewEdit.ForEachSelection(r.X, r.Y, clip.lines?[0]?.Count, clip.lines?.Count,  callback);
                this.RefreshAllViews();
            }
        }
        private void contextEditor_delete(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            Rectangle r = viewEdit.SelectedRect;
            Action<int, int> callback = (col, row) =>
            {
                // Delete each selected lines
                int target = this.TargetPCG(viewPCG.Index, col, row);
                dataSource.ClearPCGLine(target, row % 8, push: false);
            };
            viewEdit.ForEachSelection(r.X, r.Y, r.Width, r.Height, callback);
            this.RefreshAllViews();
        }
        private void contextEditor_copyDown(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            Rectangle r = viewEdit.SelectedRect;
            Action<int, int> callback = (col, row) =>
            {
                // For each selected lines
                int pcg_src = this.TargetPCG(viewPCG.Index, col, r.Y);
                int pcg_dst = this.TargetPCG(viewPCG.Index, col, row);
                (byte gen, byte color) = dataSource.GetPCGLine(pcg_src, r.Y % 8);
                dataSource.SetPCGLine(pcg_dst, row % 8, gen, color, push: false);
            };
            viewEdit.ForEachSelection(r.X, r.Y + 1, r.Width, r.Height - 1, callback);
            this.RefreshAllViews();
        }
        private void contextEditor_copyRight(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            Rectangle r = viewEdit.SelectedRect;
            Action<int, int> callback = (col, row) =>
            {
                // For each selected lines
                int pcg_src = this.TargetPCG(viewPCG.Index, r.X, row);
                int pcg_dst = this.TargetPCG(viewPCG.Index, col, row);
                (byte gen, byte color) = dataSource.GetPCGLine(pcg_src, row % 8);
                dataSource.SetPCGLine(pcg_dst, row % 8, gen, color, push: false);
            };
            viewEdit.ForEachSelection(r.X + 1, r.Y, r.Width - 1, r.Height, callback);
            this.RefreshAllViews();
        }
        private void viewEdit_AddKeyPressed(object sender, EditorControl.AddKeyEventArgs e)
        {
            int target = viewPCG.Index + (viewEdit.Y / 8) * viewPCG.ColumnNum
                       + viewEdit.X;        // Target character
            int line = viewEdit.Y % 8;      // Target line
            if (e.KeyType == EditorControl.AddKeyEventArgs.Type.PlusMinus)
            {
                // Increment/Decrement foreground color
                int fore = dataSource.GetPCGColor(target, line, foreground: true);
                fore = Math.Clamp(fore + e.Value, 0, 15);
                dataSource.SetPCGColor(target, line, fore, isForeGround: true, push: true);
                this.RefreshAllViews();
            }
            else if (e.KeyType == EditorControl.AddKeyEventArgs.Type.Brackets)
            {
                // Increment/Decrement backgroundcolor
                int back = dataSource.GetPCGColor(target, line, foreground: false);
                back = Math.Clamp(back + e.Value, 0, 15);
                dataSource.SetPCGColor(target, line, back, isForeGround: false, push: true);
                this.RefreshAllViews();
            }
        }
        //------------------------------------------------
        // PCG
        private void viewPCG_CellDragStart(object sender, EventArgs e)
        {
            // Drag characters
            Rectangle r = viewPCG.SelectedRect;
            DnDPCG d = new DnDPCG();
            d.Data = this.CopyMultiplePCG(r);
            viewPCG.DoDragDrop(d, DragDropEffects.Copy);
        }
        private void viewPCG_SelectionChanged(object sender, EventArgs e)
        {
            // Current PCG has been changed
            this.UpdatePCGList(refresh: true);
            this.UpdatePCGEditView(refresh: true);
            this.UpdateCurrentColorView(refresh: true);
        }
        private void viewPCG_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DnDPCG)))
            {
                // Dragging one character
                e.Effect = DragDropEffects.Copy;
            }
        }
        private void viewPCG_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DnDPCG)))
            {
                (int coldst, int rowdst) = viewPCG.ScreenCoodinateToSelection(Cursor.Position);
                // Characters has been dropped
                MementoCaretaker.Instance.Push();
                dynamic d = e.Data.GetData(typeof(DnDPCG));
                ClipPCG clip = d.Data;
                Action<int, int, int, int> callback = (col, row, colcnt, rowcnt) =>
                {
                    // Set each dropped characters
                    byte[] gen = clip.pcgGen[rowcnt][colcnt];
                    byte[] color = clip.pcgClr[rowcnt][colcnt];
                    int target = viewPCG.IndexOf(col, row);
                    dataSource.SetPCGData(target, gen, color, push: false);
                };
                viewPCG.ForEachSelection(coldst, rowdst, viewPCG.SelectedRect.Width,
                                     viewPCG.SelectedRect.Height, callback);
                this.RefreshAllViews();
            }
        }
        private void contextPCG_copy(object sender, EventArgs e)
        {
            Rectangle r = viewPCG.SelectedRect;
            ClipPCG clip = this.CopyMultiplePCG(r);
            ClipboardWrapper.SetData(clip);
        }
        private void contextPCG_paste(object sender, EventArgs e)
        {
            dynamic clip = ClipboardWrapper.GetData();
            if (clip is ClipPCG)
            {
                MementoCaretaker.Instance.Push();
                Action<int, int, int, int> callback = (col, row, colcnt, rowcnt) =>
                {
                    // Paste copied characters
                    int target = viewPCG.IndexOf(col, row);
                    dataSource.SetPCGData(target, clip.pcgGen[rowcnt][colcnt],
                                          clip.pcgClr[rowcnt][colcnt], push: false);
                };
                viewPCG.ForEachSelection(viewPCG.X, viewPCG.Y,
                                         clip.pcgGen?[0]?.Count, clip.pcgGen?.Count, callback);
                this.RefreshAllViews();
            }
            else if(clip is ClipPeekedData)
            {
                MementoCaretaker.Instance.Push();
                Action<int, int, int, int> callback = (col, row, colcnt, rowcnt) =>
                {
                    // Paste the data copied in peek window
                    int target = viewPCG.IndexOf(col, row);
                    dataSource.SetPCGData(target, clip.peeked[rowcnt][colcnt], null, push: false);
                };
                viewPCG.ForEachSelection(viewPCG.X, viewPCG.Y,
                                     clip.peeked?[0]?.Count, clip.peeked?.Count, callback);
                this.RefreshAllViews();
            }
        }
        private void contextPCG_delete(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            Rectangle r = viewPCG.SelectedRect;
            Action<int, int> callback = (col, row) =>
            {
                // Delete each selected characters
                int index = viewPCG.IndexOf(col, row);
                dataSource.ClearPCG(index);
            };
            viewPCG.ForEachSelection(r, callback);
            this.RefreshAllViews();
        }
        private void contextPCG_inverse(object sender, EventArgs e)
        {
            // Force current selection to single selection
            viewPCG.ResetMultipleSelection();
            // Inverse current character
            dataSource.InversePCG(viewPCG.Index, push: true);
            this.RefreshAllViews();
        }
        private void contextPCG_copyDown(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            Rectangle r = viewPCG.SelectedRect;
            Action<int, int> callback = (col, row) =>
            {
                // For each selections
                int src = viewPCG.IndexOf(col, r.Y);
                int dst = viewPCG.IndexOf(col, row);
                dataSource.CopyPCG(src, dst, push: false);
            };
            viewSand.ForEachSelection(r.X, r.Y + 1, r.Width, r.Height - 1, callback);
            this.RefreshAllViews();
        }
        private void contextPCG_copyRight(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            Rectangle r = viewPCG.SelectedRect;
            Action<int, int> callback = (col, row) =>
            {
                // For each selections
                int src = viewPCG.IndexOf(r.X, row);
                int dst = viewPCG.IndexOf(col, row);
                dataSource.CopyPCG(src, dst, push: false);
            };
            viewSand.ForEachSelection(r.X + 1, r.Y, r.Width - 1, r.Height, callback);
            this.RefreshAllViews();
        }
        private void viewPCG_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            // Key events in character list
            switch (e.KeyData)
            {
                case Keys.Enter:
                    dataSource.SetNameTable(viewSand.Index, viewPCG.Index, push: true);
                    viewSand.IncrementSelection();
                    this.UpdateSandbox(refresh: true);
                    break;
            }
        }
        //------------------------------------------------
        // Sandbox
        private void viewSand_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DnDPCG)))
            {
                // Dragged from character list
                e.Effect = DragDropEffects.Copy;
            }
        }
        private void viewSand_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DnDPCG)))
            {
                // Dropped from character list
                MementoCaretaker.Instance.Push();
                dynamic clip = e.Data.GetData(typeof(DnDPCG));
                (int dstcol, int dstrow) = viewSand.ScreenCoodinateToSelection(Cursor.Position);
                Rectangle r_src = viewPCG.SelectedRect;
                Action<int, int, int, int> callback = (col, row, colcnt, rowcnt) =>
                {
                    // Paste each copied characters
                    int src = clip.Data.pcgIndex[rowcnt][colcnt];
                    int dst_index = viewSand.IndexOf(col, row);
                    dataSource.SetNameTable(dst_index, src, push: false);
                };
                viewSand.ForEachSelection(dstcol, dstrow, r_src.Width, r_src.Height, callback);
                this.UpdateSandbox(refresh: true);
            }
        }
        private void contextSand_copy(object sender, EventArgs e)
        {
            ClipNametable clip = new ClipNametable();
            Rectangle r = viewSand.SelectedRect;
            for(int X = r.Y; X < r.Y + r.Height; ++X)
            {
                List<int> l = new List<int>();
                for(int y = r.X; y < r.X + r.Width; ++y)
                {
                    // Copy each selected cells
                    int index = viewSand.IndexOf(y, X);
                    l.Add(dataSource.GetNameTable(index));
                }
                clip.pcgID.Add(l);
            }
            ClipboardWrapper.SetData(clip);
        }
        private void contextSand_paste(object sender, EventArgs e)
        {
            dynamic clip = ClipboardWrapper.GetData();
            if (clip is ClipPCG)
            {
                // Pasted from character list
                int pcgIndex = clip.index;
                dataSource.SetNameTable(viewSand.Index, pcgIndex, true);
                this.UpdateSandbox(refresh: true);
            }
            else if (clip is ClipNametable)
            {
                MementoCaretaker.Instance.Push();
                Action<int, int, int, int> callback = (col, row, colcnt, rowcnt) =>
                {
                    // Paste each copied cells
                    int index = viewSand.IndexOf(col, row);
                    dataSource.SetNameTable(index, clip.pcgID[rowcnt][colcnt], push: false);
                };
                viewSand.ForEachSelection(viewSand.X, viewSand.Y,
                                      clip.pcgID?[0]?.Count, clip.pcgID?.Count, callback);
                this.UpdateSandbox(refresh: true);
            }
        }
        private void contextSand_delete(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            Rectangle r = viewSand.SelectedRect;
            Action<int, int> callback = (col, row) =>
            {
                // Delete each selected cells
                int index = viewSand.IndexOf(col, row);
                dataSource.SetNameTable(index, 0, push: false);
            };
            viewSand.ForEachSelection(r, callback);
            this.UpdateSandbox(refresh: true);
        }
        private void contextSand_paint(object sender, EventArgs e)
        {
            // Paint action
            MementoCaretaker.Instance.Push();   // For undo action
            // Force the PCG selection to single cell
            viewPCG.ResetMultipleSelection();
            this.UpdatePCGList(refresh: true);
            // After updating selection, start paint
            this.PaintSandbox(viewSand.X, viewSand.Y, viewPCG.Index);
            this.UpdateSandbox(refresh: true);
        }
        private void contextSand_copyDown(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            Rectangle r = viewSand.SelectedRect;
            Action<int, int> callback = (col, row) =>
            {
                // For each selected cells
                int src_index = viewSand.IndexOf(col, r.Y);
                int src_dat = dataSource.GetNameTable(src_index);
                int dst_index = viewSand.IndexOf(col, row);
                dataSource.SetNameTable(dst_index, src_dat, push: false);
            };
            viewSand.ForEachSelection(r.X, r.Y + 1, r.Width, r.Height - 1, callback);
            this.UpdateSandbox(refresh: true);
        }
        private void contextSand_copyRight(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            Rectangle r = viewSand.SelectedRect;
            Action<int, int> callback = (col, row) =>
            {
                // For each selected cells
                int src_index = viewSand.IndexOf(r.X, row);
                int src_dat = dataSource.GetNameTable(src_index);
                int dst_index = viewSand.IndexOf(col, row);
                dataSource.SetNameTable(dst_index, src_dat, push: false);
            };
            viewSand.ForEachSelection(r.X + 1, r.Y, r.Width - 1, r.Height, callback);
            this.UpdateSandbox(refresh: true);
        }
        private void viewSand_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            // Key events in sandbox
            switch (e.KeyData)
            {
                case Keys.Enter:
                    dataSource.SetNameTable(viewSand.Index, viewPCG.Index, push: true);
                    viewSand.IncrementSelection();
                    this.UpdateSandbox(refresh: true);
                    break;
            }
        }
        //------------------------------------------------
        // Misc
        private void viewPalette_CellOnEdit(object sender, EventArgs e)
        {
            if (!chkTMS.Checked)
            {
                // Open the palette editor window
                this.EditPalette(viewPalette.Index);
            }
        }
        private void viewPalette_MouseClick(object sender, MouseEventArgs e)
        {
            // Palette view has been clicked
            int clicked_color_num = Math.Clamp((e.Y / viewPalette.CellHeight)
                                              * viewPalette.ColumnNum
                                              + (e.X / viewPalette.CellWidth), 0, 15);
            // Update selection
            viewPalette.X = clicked_color_num % 8;
            viewPalette.Y = clicked_color_num / 8;
            this.UpdatePaletteView(true);
            // Update color table of current line
            int target = this.TargetPCG();
            if (e.Button == MouseButtons.Left)
            {
                // Foreground color has changed
                dataSource.SetPCGColor(target,
                                       viewEdit.Y % 8,
                                       clicked_color_num,
                                       isForeGround: true, push: true);
            }
            else if (e.Button == MouseButtons.Right)
            {
                // Background color has changed
                dataSource.SetPCGColor(target,
                                       viewEdit.Y % 8,
                                       clicked_color_num,
                                       isForeGround: false, push: true);
            }
            this.RefreshAllViews();
        }
        private void viewPalette_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!chkTMS.Checked)
            {
                // Open the palette editor window
                this.EditPalette(viewPalette.Y * 8 + viewPalette.X);
            }
        }
        private void viewColor_Click(object sender, EventArgs e)
        {
            bool foreground = (viewColor.X == 0);
            int target = this.TargetPCG();
            // Callback for the color selection window
            Action<int> callback = (x) =>
            {
                dataSource.SetPCGColor(target, viewEdit.Y % 8, x, foreground, push: true);
                this.RefreshAllViews();
            };
            // Open the color selection window
            int color_code = dataSource.GetPCGColor(target, viewEdit.Y % 8, foreground);
            PaletteSelector palette_win = new PaletteSelector(dataSource, color_code, callback);
            palette_win.StartPosition = FormStartPosition.Manual;
            palette_win.Location = Cursor.Position;
            palette_win.Show();
        }
        private void checkTMS_Click(object sender, EventArgs e)
        {
            if (chkTMS.Checked && !dataSource.IsTMS9918)
            {
                // Set to TMS9918 and update palettes
                dataSource.SetPaletteToTMS9918(push: true);
                this.RefreshAllViews();     // Everything changes
            }
            else if (!chkTMS.Checked && dataSource.IsTMS9918)
            {
                // Set to V9938 and update palettes
                dataSource.SetPaletteToV9938(push: true);
                this.RefreshAllViews();     // Everything changes
            }
        }
        private void FormPCG_Activated(object sender, EventArgs e)
        {
            // Redraw the views according to data at this timing
            this.RefreshAllViews();
        }
        private void chkCRT_CheckedChanged(object sender, EventArgs e)
        {
            this.RefreshAllViews();
        }
        public void ChangeOccuredByHost()
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
            if (Utility.ImportDialogAndImport(CurrentFile,
                                              Import.PCGTypeFilter,
                                              "Select file to import",
                                              dataSource.ImportPCG))
            {
                this.RefreshAllViews();
            }
        }
        private void menu_fileExport(object sender, EventArgs e)
        {
            mainWin.ExportPCG(sender, e);
        }
        private void menu_fileLoadPCG(object sender, EventArgs e)
        {
            if (Utility.LoadDialogAndLoad(CurrentFile,
                                          "PCG File(*.pcg)|*.pcg",
                                          "Load PCG settings",
                                          dataSource.LoadPCG,
                                          push: true,
                                          out _))
            {
                this.RefreshAllViews();
            }
        }
        private void menu_fileSavePCG(object sender, EventArgs e)
        {
            Utility.SaveDialogAndSave(CurrentFile,
                                      "PCG File(*.pcg)|*.pcg",
                                      "Save PCG settings",
                                      dataSource.SavePCG,
                                      save_as: true,
                                      out _); ;
        }
        private void menu_savePalette(object sender, EventArgs e)
        {
            Utility.SaveDialogAndSave(CurrentFile,
                                      "PLT File(*.plt)|*.plt",
                                      "Save palette",
                                      dataSource.SavePaletteSettings,
                                      save_as: true,
                                      out _);
        }
        private void menu_loadPalette(object sender, EventArgs e)
        {
            if (Utility.LoadDialogAndLoad(CurrentFile,
                                         "PLT File(*.plt)|*.plt",
                                         "Load palette",
                                         dataSource.LoadPaletteSettings,
                                         push:  true,
                                         out _))
            {
                this.RefreshAllViews();
            }
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
        // Utility
        private void EditPalette(int index)
        {
            (int R, int G, int B) = dataSource.GetPalette(index);
            PaletteEditor palette_win = null ;
            Action callback = () =>
            {
                dataSource.SetPalette(index,
                                      palette_win.R, palette_win.G, palette_win.B, push: true);
                this.RefreshAllViews();     // Everything changes
            };
            palette_win = new PaletteEditor(R, G, B, callback);
            palette_win.StartPosition = FormStartPosition.Manual;
            palette_win.Location = Cursor.Position;
            palette_win.Show();
        }
        private void PaintSandbox(int x, int y, int val)
        {
            int pcg_to_paint = dataSource.GetNameTable(y * 32 + x);
            if (pcg_to_paint == val) return;
            dataSource.SetNameTable(y * 32 + x, val, push: false);
            if (y > 0)
                if (dataSource.GetNameTable((y - 1) * 32 + x) == pcg_to_paint)
                    this.PaintSandbox(x, y - 1, val);
            if (y < 23)
                if (dataSource.GetNameTable((y + 1) * 32 + x) == pcg_to_paint)
                    this.PaintSandbox(x, y + 1, val);
            if (x > 0)
                if (dataSource.GetNameTable(y * 32 + x - 1) == pcg_to_paint)
                    this.PaintSandbox(x - 1, y, val);
            if (x < 31)
                if (dataSource.GetNameTable(y * 32 + x + 1) == pcg_to_paint)
                    this.PaintSandbox(x + 1, y, val);
        }
        private ClipPCG CopyMultiplePCG(Rectangle r)
        {
            ClipPCG clip = new ClipPCG();
            for (int i = r.Y; i < r.Y + r.Height; ++i)
            {
                List<byte[]> gen_row = new List<byte[]>();
                List<byte[]> clr_row = new List<byte[]>();
                List<int> pcg_row = new List<int>();
                for (int j = r.X; j < r.X + r.Width; ++j)
                {
                    // Copy selected characters
                    int index = viewPCG.IndexOf(j, i);
                    (byte[] gen, byte[] color) = dataSource.GetPCGData(index);
                    gen_row.Add(gen);
                    clr_row.Add(color);
                    pcg_row.Add(index);
                }
                clip.pcgGen.Add(gen_row);
                clip.pcgClr.Add(clr_row);
                clip.pcgIndex.Add(pcg_row);
            }
            return clip;
        }
        private int TargetPCG()
        {
            // Index of currently editing PCG, according to PCG selection
            // and the current line in editor.
            return viewPCG.Index + (viewEdit.Y / 8) * viewPCG.ColumnNum
                                 + viewEdit.X;        // Target character
        }
        private int TargetPCG(int index, int line_x, int line_y)
        {
            return index + (line_y / 8) * viewPCG.ColumnNum + line_x;
        }
    }
}
