using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace _99x8Edit
{
    // PCG editor window
    public partial class PCGEditor : Form
    {
        private readonly Machine dataSource;
        private readonly MainWindow mainWin;
        private TabOrder tabList = new TabOrder();
        private Bitmap bmpPalette = new Bitmap(256, 64);    // Palette view
        private Bitmap bmpPCGEdit = new Bitmap(256, 256);   // PCG Editor view
        private Bitmap bmpColorL = new Bitmap(32, 32);
        private Bitmap bmpColorR = new Bitmap(32, 32);
        private Selection curLine = new Selection(128, 16); // Selected line
        private int currentDot = 0;
        private Selection curColor = new Selection(16, 16); // Currently elected color, 0=left, 1=right
        private Selection curPal = new Selection(32, 32);   // Selection in palette
        internal String CurrentFile
        {
            get;
            set;
        }
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
            tabList.Add(panelEditor, curLine);
            tabList.Add(panelColor, curColor);
            tabList.Add(panelPalette, curPal);
            tabList.Add(viewPCG, viewPCG.Selected);
            tabList.Add(viewSand, viewSand.Selected);
            // Initialize controls
            viewPalette.Image = bmpPalette;
            viewPCGEdit.Image = bmpPCGEdit;
            viewColorL.Image = bmpColorL;
            viewColorR.Image = bmpColorR;
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
            CursorAnim win = new CursorAnim(r_prev, r_next);
            win.Show();
            win.StartMoving();
            // Refresh views
            this.RefreshAllViews();
            return true;
        }
        //------------------------------------------------------------------------------
        // Refreshing Views
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
            Utility.DrawTransparent(bmpPalette);
            Graphics g = Graphics.FromImage(bmpPalette);
            for (int i = 1; i < 16; ++i)
            {
                Brush b = dataSource.BrushOf(i);
                g.FillRectangle(b, (i % 8) * 32, (i / 8) * 32, 32, 32);
            }
            // Current selection
            Utility.DrawSelection(g, curPal, panelPalette.Focused);
            if (refresh) this.viewPalette.Refresh();
        }
        private void UpdatePCGEditView(bool refresh)
        {
            // Update PCG editor
            Utility.DrawTransparent(bmpPCGEdit);
            Graphics g = Graphics.FromImage(bmpPCGEdit);
            for (int i = 0; i < 4; ++i)     // Draw four characters
            {
                int index = viewPCG.Index;
                int target = (index + (i / 2) * 32 + (i % 2)) % 256;
                for (int j = 0; j < 8; ++j)     // 8 lines in one character
                {
                    for (int k = 0; k < 8; ++k) // 8 dots in one line
                    {
                        // Draw one cell
                        int p = dataSource.GetPCGPixel(target, j, k);
                        int code = dataSource.GetPCGColor(target, j, foreground: (p != 0));
                        if(code != 0)
                        {
                            // Outline of one dot
                            g.FillRectangle(Brushes.Gray,
                                            (i % 2) * 128 + k * 16,
                                            (i / 2) * 128 + j * 16, 16, 16);
                            // One magnified dot
                            Brush b = dataSource.BrushOf(code);
                            g.FillRectangle(b, (i % 2) * 128 + k * 16,
                                            (i / 2) * 128 + j * 16, 15, 15);
                        }
                    }
                }
            }
            // Draw selection
            Utility.DrawSelection(g, curLine, panelEditor.Focused);
            if (panelEditor.Focused)
            {
                // One dot can be selected when focused
                Utility.DrawSubSelection(g, curLine.Display.X + currentDot * 16,
                                         curLine.Display.Y, 14, 14);
            }
            if (refresh) viewPCGEdit.Refresh();
        }
        private void UpdateCurrentColorView(bool refresh)
        {
            Graphics gl = Graphics.FromImage(bmpColorL);
            Graphics gr = Graphics.FromImage(bmpColorR);
            // Character to refer depends on current selected line position
            int index = viewPCG.Index;
            int target = (index + curLine.X + (curLine.Y / 8) * 32) % 256;
            // Get the corresponding color code
            int color_code_l = dataSource.GetPCGColor(target, curLine.Y % 8, foreground: true);
            int color_code_r = dataSource.GetPCGColor(target, curLine.Y % 8, foreground: false);
            Utility.DrawTransparent(bmpColorL);
            // Draw foreground and background colors
            if(color_code_l > 0)
            {
                Brush b = dataSource.BrushOf(color_code_l);
                gl.FillRectangle(b, 0, 0, 32, 32);
            }
            Utility.DrawTransparent(bmpColorR);
            if (color_code_r > 0)
            {
                Brush b = dataSource.BrushOf(color_code_r);
                gr.FillRectangle(b, 0, 0, 32, 32);
            }
            // Draw selection
            Utility.DrawSelection((curColor.X == 0) ? gl : gr,
                                  0, 0, 29, 29, panelColor.Focused);
            if (refresh)
            {
                viewColorL.Refresh();
                viewColorR.Refresh();
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
        private void viewPalette_MouseClick(object sender, MouseEventArgs e)
        {
            // Palette view has been clicked
            panelPalette.Focus();      // Catch key events at parent control
            int clicked_color_num = Math.Clamp((e.Y / 32) * 8 + (e.X / 32), 0, 15);
            // Update selection
            curPal.X = clicked_color_num % 8;
            curPal.Y = clicked_color_num / 8;
            // Update color table of current line
            int index = viewPCG.Index;
            int current_target_pcg = (index + curLine.X + (curLine.Y / 8) * 32) % 256;
            if (e.Button == MouseButtons.Left)
            {
                // Foreground color has changed
                dataSource.SetPCGColor(current_target_pcg,
                                       curLine.Y % 8,
                                       clicked_color_num,
                                       isForeGround: true, push: true);
            }
            else if (e.Button == MouseButtons.Right)
            {
                // Background color has changed
                dataSource.SetPCGColor(current_target_pcg,
                                       curLine.Y % 8,
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
                int clicked_color_num = (e.Y / 32) * 8 + (e.X / 32);
                this.EditPalette(clicked_color_num);
            }
        }
        private void viewColorL_Click(object sender, EventArgs e)
        {
            // Foreground color has been clicked
            panelColor.Focus();
            // Update the selection before opening color selection window
            curColor.X = 0;
            this.UpdateCurrentColorView(refresh: true);
            int index = viewPCG.Index;
            int target = (index + curLine.X + (curLine.Y / 8) * 32) % 256;
            // Callback for the color selection window
            Action<int> callback = (x) =>
            {
                dataSource.SetPCGColor(target,
                                       curLine.Y % 8, x,
                                       isForeGround: true, push: true);
                this.RefreshAllViews();
            };
            // Open the color selection window
            int color_code_l = dataSource.GetPCGColor(target,
                                                      curLine.Y % 8,
                                                      foreground: true);
            PaletteSelector palette_win = new PaletteSelector(bmpPalette, color_code_l, callback);
            palette_win.StartPosition = FormStartPosition.Manual;
            palette_win.Location = Cursor.Position;
            palette_win.Show();
        }
        private void viewColorR_Click(object sender, EventArgs e)
        {
            // Background color has been clicked
            panelColor.Focus();
            // Update the selection before opening color selection window
            curColor.X = 1;
            this.UpdateCurrentColorView(refresh: true);
            int index = viewPCG.Index;
            int target = (index + curLine.X + (curLine.Y / 8) * 32) % 256;
            // Callback for the color selection window
            Action<int> callback = (x) =>
            {
                dataSource.SetPCGColor(target,
                                       curLine.Y % 8, x,
                                       isForeGround: false, push: true);
                this.RefreshAllViews();
            };
            // Open the color selection window
            int color_code_r = dataSource.GetPCGColor(target,
                                                      curLine.Y % 8,
                                                      foreground: false);
            PaletteSelector palette_win = new PaletteSelector(bmpPalette, color_code_r, callback);
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
        // Editor
        private void viewPCGEdit_MouseDown(object sender, MouseEventArgs e)
        {
            // Editor has been clicked
            panelEditor.Focus();    // Catch key events at parent control
            if (e.Button == MouseButtons.Left)
            {
                int clicked_line_x = e.X / 128;
                int clicked_line_y = e.Y / 16;
                if ((curLine.X != clicked_line_x) || (curLine.Y != clicked_line_y))
                {
                    // Selected line has been changed
                    if (Control.ModifierKeys == Keys.Shift)
                    {
                        // Multiple selection
                        curLine.ToX = clicked_line_x;
                        curLine.ToY = clicked_line_y;
                    }
                    else
                    {
                        // New selection
                        curLine.X = clicked_line_x;
                        curLine.Y = clicked_line_y;
                    }
                    this.UpdatePCGEditView(refresh: true);         // Update editor view
                    this.UpdateCurrentColorView(refresh: true);    // Update view of current color
                    // Drag for multiple selection
                    viewPCGEdit.DoDragDrop(new DnDEditor(), DragDropEffects.Copy);
                }
                else
                {
                    // Update PCG pattern
                    this.EditCurrentPCG((e.X / 16) % 8);
                }
            }
        }
        private void panelEditor_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DnDEditor)))
            {
                // Multiple selection
                e.Effect = DragDropEffects.Copy;
            }
            else e.Effect = DragDropEffects.None;
        }
        private void panelEditor_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DnDEditor)))
            {
                // Multiple selection
                Point p = viewPCGEdit.PointToClient(Cursor.Position);
                curLine.ToX = Math.Min(p.X / 128, 1);
                curLine.ToY = Math.Min(p.Y / 16, 15);
                this.UpdatePCGEditView(refresh: true);
                this.UpdateCurrentColorView(refresh: true);
            }
        }
        private void contextEditor_copy(object sender, EventArgs e)
        {
            ClipPCGLines clip = new ClipPCGLines();
            Rectangle r = curLine.Selected;
            for (int i = r.Y; i < r.Y + r.Height; ++i)
            {
                List<(byte, byte)> l = new List<(byte, byte)>();
                for (int j = r.X; j < r.X + r.Width; ++j)
                {
                    // Copy each selected lines
                    int index = viewPCG.Index;
                    int target = index + j + (i / 8) * 32;
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
                for (int i = 0; (i < clip.lines.Count) && (curLine.Y + i < 16); ++i)
                {
                    List<(byte, byte)> l = clip.lines[i];
                    for (int j = 0; (j < l.Count) && (curLine.X + j < 2); ++j)
                    {
                        // Paste each copied lines
                        int index = viewPCG.Index;
                        int pcg = index + curLine.X + j + ((curLine.Y + i) / 8) * 32;
                        (byte gen, byte color) = l[j];
                        dataSource.SetPCGLine(pcg, (curLine.Y + i) % 8, gen, color, push: false);
                    }
                }
                this.RefreshAllViews();
            }
        }
        private void contextEditor_delete(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            Rectangle r = curLine.Selected;
            for (int i = r.Y; i < r.Y + r.Height; ++i)
            {
                for (int j = r.X; j < r.X + r.Width; ++j)
                {
                    // Delete each selected lines
                    int index = viewPCG.Index;
                    int target = (index + j + (i / 8) * 32) % 256;
                    dataSource.ClearPCGLine(target, i % 8, push: false);
                }
            }
            this.RefreshAllViews();
        }
        private void contextEditor_copyDown(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            Rectangle r = curLine.Selected;
            for (int i = r.Y + 1; i < r.Y + r.Height; ++i)
            {
                for (int j = r.X; j < r.X + r.Width; ++j)
                {
                    // For each selected lines
                    int index = viewPCG.Index;
                    int pcg_src = (index + (r.Y / 8) * 32 + j) % 256;
                    int pcg_dst = (index + (i / 8) * 32 + j) % 256;
                    (byte gen, byte color) = dataSource.GetPCGLine(pcg_src, r.Y % 8);
                    dataSource.SetPCGLine(pcg_dst, i % 8, gen, color, push: false);
                }
            }
            this.RefreshAllViews();
        }
        private void contextEditor_copyRight(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            Rectangle r = curLine.Selected;
            for (int i = r.Y; i < r.Y + r.Height; ++i)
            {
                for (int j = r.X + 1; j < r.X + r.Width; ++j)
                {
                    // For each selected lines
                    int index = viewPCG.Index;
                    int pcg_src = (index + (i / 8) * 32 + r.X) % 256;
                    int pcg_dst = (index + (i / 8) * 32 + j) % 256;
                    (byte gen, byte color) = dataSource.GetPCGLine(pcg_src, r.Y % 8);
                    dataSource.SetPCGLine(pcg_dst, i % 8, gen, color, push: false);
                }
            }
            this.RefreshAllViews();
        }
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
                (int coldst, int rowdst) = viewPCG.ScreenCoodinateToColRow(Cursor.Position);
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
                viewPCG.ForEachCells(coldst, rowdst, viewPCG.SelectedRect.Width,
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
                viewPCG.ForEachCells(viewPCG.X, viewPCG.Y,
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
                viewPCG.ForEachCells(viewPCG.X, viewPCG.Y,
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
            viewPCG.ForEachCells(r, callback);
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
            viewSand.ForEachCells(r.X, r.Y + 1, r.Width, r.Height - 1, callback);
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
            viewSand.ForEachCells(r.X + 1, r.Y, r.Width - 1, r.Height, callback);
            this.RefreshAllViews();
        }
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
                (int dstcol, int dstrow) = viewSand.ScreenCoodinateToColRow(Cursor.Position);
                Rectangle r_src = viewPCG.SelectedRect;
                Action<int, int, int, int> callback = (col, row, colcnt, rowcnt) =>
                {
                    // Paste each copied characters
                    int src = clip.Data.pcgIndex[rowcnt][colcnt];
                    int dst_index = viewSand.IndexOf(col, row);
                    dataSource.SetNameTable(dst_index, src, push: false);
                };
                viewSand.ForEachCells(dstcol, dstrow, r_src.Width, r_src.Height, callback);
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
                viewSand.ForEachCells(viewSand.X, viewSand.Y,
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
            viewSand.ForEachCells(r, callback);
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
            viewSand.ForEachCells(r.X, r.Y + 1, r.Width, r.Height - 1, callback);
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
            viewSand.ForEachCells(r.X + 1, r.Y, r.Width - 1, r.Height, callback);
            this.UpdateSandbox(refresh: true);
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
        private void EditCurrentPCG(int x)
        {
            int line = curLine.Y % 8;               // Line in one character
            int index = viewPCG.Index;             // Selected character
            // The target character depends on current selected line
            int current_target_pcg = (index + curLine.X + (curLine.Y / 8) * 32) % 256;
            int prev_pixel = dataSource.GetPCGPixel(current_target_pcg, line, x);
            if (prev_pixel == 0)
            {
                dataSource.SetPCGPixel(current_target_pcg, line, x, 1, push: true);
            }
            else
            {
                dataSource.SetPCGPixel(current_target_pcg, line, x, 0, push: true);
            }
            this.UpdatePCGEditView(refresh: true);   // PCG Editor view changes
            this.UpdatePCGList(refresh: true);       // PCG list view changes also
            this.UpdateSandbox(refresh: true);       // Update sandbox view
        }
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
    }
}
