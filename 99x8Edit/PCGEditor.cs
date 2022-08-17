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
        private Bitmap bmpPCGList = new Bitmap(512, 128);   // PCG list view
        private Bitmap bmpPalette = new Bitmap(256, 64);    // Palette view
        private Bitmap bmpSandbox = new Bitmap(512, 384);   // Sandbox view
        private Bitmap bmpPCGEdit = new Bitmap(256, 256);   // PCG Editor view
        private Bitmap bmpColorL = new Bitmap(32, 32);
        private Bitmap bmpColorR = new Bitmap(32, 32);
        private Selection curPCG = new Selection(16, 16);   // Selection in PCG list
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
        private class DnDPCG { }
        private class DnDPCGSel { }
        private class DnDSandbox { }
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
            tabList.Add(panelPCG, curPCG);
            tabList.Add(viewSand, viewSand.Selection);
            // Initialize controls
            viewPalette.Image = bmpPalette;
            viewPCG.Image = bmpPCGList;
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
            toolStripPCGCopy.Click += new EventHandler(contextPCGList_copy);
            toolStripPCGPaste.Click += new EventHandler(contextPCGList_paste);
            toolStripPCGDel.Click += new EventHandler(contextPCGList_delete);
            toolStripPCGInverse.Click += new EventHandler(contextPCGList_inverse);
            toolStripPCGCopyDown.Click += new EventHandler(contextPCGList_copyDown);
            toolStripPCGCopyRight.Click += new EventHandler(contextPCGList_copyRight);
            toolStripSandboxCopy.Click += new EventHandler(contextSandbox_copy);
            toolStripSandboxPaste.Click += new EventHandler(contextSandbox_paste);
            toolStripSandboxDel.Click += new EventHandler(contextSandbox_delete);
            toolStripSandboxPaint.Click += new EventHandler(contextSandbox_paint);
            toolStripSandboxCopyDown.Click += new EventHandler(contextSandbox_copyDown);
            toolStripSandboxCopyRight.Click += new EventHandler(contextSandbox_copyRight);
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
                int pcg = curPCG.Y * 32 + curPCG.X;
                int target_pcg = (pcg + (i / 2) * 32 + (i % 2)) % 256;
                for (int j = 0; j < 8; ++j)     // 8 lines in one character
                {
                    for (int k = 0; k < 8; ++k) // 8 dots in one line
                    {
                        // Draw one cell
                        int p = dataSource.GetPCGPixel(target_pcg, j, k);
                        int code = dataSource.GetPCGColor(target_pcg, j, foreground: (p != 0));
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
            int current_pcg = curPCG.Y * 32 + curPCG.X;
            int current_target_pcg = (current_pcg + curLine.X + (curLine.Y / 8) * 32) % 256;
            // Get the corresponding color code
            int color_code_l = dataSource.GetPCGColor(current_target_pcg,
                                                      curLine.Y % 8, foreground: true);
            int color_code_r = dataSource.GetPCGColor(current_target_pcg,
                                                      curLine.Y % 8, foreground: false);
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
            Graphics g = Graphics.FromImage(bmpPCGList);
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.FillRectangle(Brushes.Black, 0, 0, bmpPCGList.Width, bmpPCGList.Height);
            for (int i = 0; i < 256; ++i)
            {
                g.DrawImage(dataSource.GetBitmapOfPCG(i), (i % 32) * 16, (i / 32) * 16, 17, 17);
            }
            // CRT Filter
            if (chkCRT.Checked)
            {
                Filter.Create(Filter.Type.CRT).Process(bmpPCGList);
            }
            // Current selection
            Utility.DrawSelection(g, curPCG, panelPCG.Focused);
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
            int current_pcg = curPCG.Y * 32 + curPCG.X;
            int current_target_pcg = (current_pcg + curLine.X + (curLine.Y / 8) * 32) % 256;
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
            int current_pcg = curPCG.Y * 32 + curPCG.X;
            int current_target_pcg = (current_pcg + curLine.X + (curLine.Y / 8) * 32) % 256;
            // Callback for the color selection window
            Action<int> callback = (x) =>
            {
                dataSource.SetPCGColor(current_target_pcg,
                                       curLine.Y % 8, x,
                                       isForeGround: true, push: true);
                this.RefreshAllViews();
            };
            // Open the color selection window
            int color_code_l = dataSource.GetPCGColor(current_target_pcg,
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
            int current_pcg = curPCG.Y * 32 + curPCG.X;
            int current_target_pcg = (current_pcg + curLine.X + (curLine.Y / 8) * 32) % 256;
            // Callback for the color selection window
            Action<int> callback = (x) =>
            {
                dataSource.SetPCGColor(current_target_pcg,
                                       curLine.Y % 8, x,
                                       isForeGround: false, push: true);
                this.RefreshAllViews();
            };
            // Open the color selection window
            int color_code_r = dataSource.GetPCGColor(current_target_pcg,
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
                    int lefttop_pcg = curPCG.Y * 32 + curPCG.X;
                    int pcg = (lefttop_pcg + j + (i / 8) * 32) % 256;
                    (byte gen, byte color) = dataSource.GetPCGLine(pcg, i % 8);
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
                        int lefttop_pcg = curPCG.Y * 32 + curPCG.X;
                        int pcg = (lefttop_pcg + curLine.X + j + ((curLine.Y + i) / 8) * 32) % 256;
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
                    int lefttop_pcg = curPCG.Y * 32 + curPCG.X;
                    int pcg = (lefttop_pcg + j + (i / 8) * 32) % 256;
                    dataSource.ClearPCGLine(pcg, i % 8, push: false);
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
                    int lefttop_pcg = curPCG.Y * 32 + curPCG.X;
                    int pcg_src = (lefttop_pcg + (r.Y / 8) * 32 + j) % 256;
                    int pcg_dst = (lefttop_pcg + (i / 8) * 32 + j) % 256;
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
                    int lefttop_pcg = curPCG.Y * 32 + curPCG.X;
                    int pcg_src = (lefttop_pcg + (i / 8) * 32 + r.X) % 256;
                    int pcg_dst = (lefttop_pcg + (i / 8) * 32 + j) % 256;
                    (byte gen, byte color) = dataSource.GetPCGLine(pcg_src, r.Y % 8);
                    dataSource.SetPCGLine(pcg_dst, i % 8, gen, color, push: false);
                }
            }
            this.RefreshAllViews();
        }
        private void viewPCG_MouseDown(object sender, MouseEventArgs e)
        {
            // PCG list has been clicked
            panelPCG.Focus();   // Catch key events at parent control
            if (e.Button == MouseButtons.Left)
            {
                int clicked_pcg_x = Math.Min(e.X / 16, 31);
                int clicked_pcg_y = Math.Min(e.Y / 16, 7);
                if ((clicked_pcg_x != curPCG.X) || (clicked_pcg_y != curPCG.Y))
                {
                    // Selected character has been changed
                    if (Control.ModifierKeys == Keys.Shift)
                    {
                        // Multiple selection
                        curPCG.ToX = clicked_pcg_x;
                        curPCG.ToY = clicked_pcg_y;
                    }
                    else
                    {
                        // New selection
                        curPCG.X = clicked_pcg_x;
                        curPCG.Y = clicked_pcg_y;
                    }
                    this.UpdatePCGList(refresh: true);
                    this.UpdatePCGEditView(refresh: true);
                    this.UpdateCurrentColorView(refresh: true);
                    // Drag for multiple selection
                    viewPCG.DoDragDrop(new DnDPCGSel(), DragDropEffects.Copy);
                }
                else
                {
                    // Drag one character
                    viewPCG.DoDragDrop(new DnDPCG(), DragDropEffects.Copy);
                }
            }
        }
        private void panelPCG_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DnDPCG)))
            {
                // Dragging one character
                e.Effect = DragDropEffects.Copy;
            }
            else if (e.Data.GetDataPresent(typeof(DnDPCGSel)))
            {
                // Multiple selection
                e.Effect = DragDropEffects.Copy;
            }
            else e.Effect = DragDropEffects.None;
        }
        private void panelPCG_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DnDPCGSel)))
            {
                // Multiple selection
                Point p = viewPCG.PointToClient(Cursor.Position);
                curPCG.ToX = Math.Min(p.X / 16, 31);
                curPCG.ToY = Math.Min(p.Y / 16, 7);
                this.UpdatePCGList(refresh: true);
                this.UpdatePCGEditView(refresh: true);
                this.UpdateCurrentColorView(refresh: true);
            }
        }
        private void panelPCG_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DnDPCG)))
            {
                // One character has been dropped
                Point p = viewPCG.PointToClient(Cursor.Position);
                if (p.X > viewPCG.Width - 1) p.X = viewPCG.Width - 1;
                if (p.Y > viewPCG.Height - 1) p.X = viewPCG.Height - 1;
                int target_cell = ((p.Y / 16) * 32 + p.X / 16) % 256;
                dataSource.CopyPCG(curPCG.Y * 32 + curPCG.X, target_cell, push: true);
                this.RefreshAllViews();
            }
        }
        private void contextPCGList_copy(object sender, EventArgs e)
        {
            ClipPCG clip = new ClipPCG();
            clip.index = (byte)(curPCG.Y * 32 + curPCG.X);
            Rectangle r = curPCG.Selected;
            for (int i = r.Y; i < r.Y + r.Height; ++i)
            {
                List<byte[]> gen_row = new List<byte[]>();
                List<byte[]> clr_row = new List<byte[]>();
                for (int j = r.X; j < r.X + r.Width; ++j)
                {
                    // Copy selected characters
                    (byte[] gen, byte[] color) = dataSource.GetPCGData(i * 32 + j);
                    gen_row.Add(gen);
                    clr_row.Add(color);
                }
                clip.pcgGen.Add(gen_row);
                clip.pcgClr.Add(clr_row);
            }
            ClipboardWrapper.SetData(clip);
        }
        private void contextPCGList_paste(object sender, EventArgs e)
        {
            dynamic clip = ClipboardWrapper.GetData();
            if (clip is ClipPCG)
            {
                MementoCaretaker.Instance.Push();
                for (int i = 0; (i < clip.pcgGen.Count) && (curPCG.Y + i < 8); ++i)
                {
                    List<byte[]> gen_line = clip.pcgGen[i];
                    List<byte[]> clr_line = clip.pcgClr[i];
                    for (int j = 0; (j < gen_line.Count) && (curPCG.X + j < 32); ++j)
                    {
                        // Paste copied characters
                        dataSource.SetPCGData((curPCG.Y + i) * 32 + curPCG.X + j,
                                          gen_line[j], clr_line[j], push: false);
                    }
                }
                this.RefreshAllViews();
            }
            else if(clip is ClipPeekedData)
            {
                MementoCaretaker.Instance.Push();
                for (int i = 0; (i < clip.peeked.Count) && (curPCG.Y + i < 8); ++i)
                {
                    List<byte[]> row = clip.peeked[i];
                    for (int j = 0; (j < row.Count) && (curPCG.X + j < 32); ++j)
                    {
                        // Paste the data copied in peek window
                        dataSource.SetPCGData((curPCG.Y + i) * 32 + curPCG.X + j,
                                          row[j], null, push: false);
                    }
                }
                this.RefreshAllViews();
            }
        }
        private void contextPCGList_delete(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            Rectangle r = curPCG.Selected;
            for (int i = r.Y; (i < r.Y + r.Height) && (i < 24); ++i)
            {
                for (int j = r.X; (j < r.X + r.Width) && (j < 32); ++j)
                {
                    // Delete selected characters
                    dataSource.ClearPCG(i * 32 + j);
                }
            }
            this.RefreshAllViews();
        }
        private void contextPCGList_inverse(object sender, EventArgs e)
        {
            // Inverse current character
            dataSource.InversePCG(curPCG.Y * 32 + curPCG.X, push: true);
            this.RefreshAllViews();
        }
        private void contextPCGList_copyDown(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            Rectangle r = curPCG.Selected;
            for (int i = r.Y + 1; (i < r.Y + r.Height) && (i < 24); ++i)
            {
                for (int j = r.X; (j < r.X + r.Width) && (j < 32); ++j)
                {
                    // For each selections
                    dataSource.CopyPCG(r.Y * 32 + j, i * 32 + j, push: false);
                }
            }
            this.RefreshAllViews();
        }
        private void contextPCGList_copyRight(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            Rectangle r = curPCG.Selected;
            for (int i = r.Y; (i < r.Y + r.Height) && (i < 24); ++i)
            {
                for (int j = r.X + 1; (j < r.X + r.Width) && (j < 32); ++j)
                {
                    // For each selections
                    dataSource.CopyPCG(i * 32 + r.X, i * 32 + j, push: false);
                }
            }
            this.RefreshAllViews();
        }
        private void contextSandbox_copy(object sender, EventArgs e)
        {
            ClipNametable clip = new ClipNametable();
            Rectangle r = viewSand.Selection.Selected;
            for(int i = r.Y; i < r.Y + r.Height; ++i)
            {
                List<int> l = new List<int>();
                for(int j = r.X; j < r.X + r.Width; ++j)
                {
                    // Copy each selected cells
                    l.Add(dataSource.GetNameTable(i * viewSand.ColumnNum + j));
                }
                clip.pcgID.Add(l);
            }
            ClipboardWrapper.SetData(clip);
        }
        private void contextSandbox_paste(object sender, EventArgs e)
        {
            dynamic clip = ClipboardWrapper.GetData();
            if (clip is ClipPCG)
            {
                // Pasted from character list
                int pcgIndex = clip.index;
                dataSource.SetNameTable(viewSand.SelectedIndex, pcgIndex, true);
                this.UpdateSandbox(refresh: true);
            }
            else if (clip is ClipNametable)
            {
                MementoCaretaker.Instance.Push();
                for(int i = 0; (i < clip.pcgID.Count)
                    && (viewSand.Selection.Y + i < viewSand.RowNum); ++i)
                {
                    List<int> l = clip.pcgID[i];
                    for(int j = 0; (j < l.Count)
                        && (viewSand.Selection.X + j < viewSand.ColumnNum); ++j)
                    {
                        // Paste each copied cells
                        dataSource.SetNameTable((viewSand.Selection.Y + i) * viewSand.ColumnNum
                            + viewSand.Selection.X + j, l[j], push: false);
                    }
                }
                this.UpdateSandbox(refresh: true);
            }
        }
        private void contextSandbox_delete(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            Rectangle r = viewSand.Selection.Selected;
            for (int i = r.Y; (i < r.Y + r.Height) && (i < viewSand.RowNum); ++i)
            {
                for(int j = r.X; (j < r.X + r.Width) && (j < viewSand.ColumnNum); ++j)
                {
                    // Delete each selected cells
                    dataSource.SetNameTable(i * viewSand.ColumnNum + j, 0, push: false);
                }
            }
            this.UpdateSandbox(refresh: true);
        }
        private void contextSandbox_paint(object sender, EventArgs e)
        {
            // Paint action
            MementoCaretaker.Instance.Push();   // For undo action
            this.PaintSandbox(viewSand.Selection.X, viewSand.Selection.Y, curPCG.Y * 32 + curPCG.X);
            this.UpdateSandbox(refresh: true);
        }
        private void contextSandbox_copyDown(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            Rectangle r = viewSand.Selection.Selected;
            for (int i = r.Y + 1; (i < r.Y + r.Height) && (i < viewSand.RowNum); ++i)
            {
                for (int j = r.X; (j < r.X + r.Width) && (j < viewSand.ColumnNum); ++j)
                {
                    // For each selected cells
                    int src = dataSource.GetNameTable(r.Y * 32 + j);
                    dataSource.SetNameTable(i * viewSand.ColumnNum + j, src, push: false);
                }
            }
            this.UpdateSandbox(refresh: true);
        }
        private void contextSandbox_copyRight(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            Rectangle r = viewSand.Selection.Selected;
            for (int i = r.Y; (i < r.Y + r.Height) && (i < viewSand.RowNum); ++i)
            {
                for (int j = r.X + 1; (j < r.X + r.Width) && (j < viewSand.ColumnNum); ++j)
                {
                    // For each selected cells
                    int src = dataSource.GetNameTable(i * viewSand.ColumnNum + r.X);
                    dataSource.SetNameTable(i * viewSand.ColumnNum + j, src, push: false);
                }
            }
            this.UpdateSandbox(refresh: true);
        }
        private void panelSandbox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DnDPCG)))
            {
                // Dragged from character list
                e.Effect = DragDropEffects.Copy;
            }
            // Let the effect changed by the control also
            //else e.Effect = DragDropEffects.None;
        }
        private void panelSandbox_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DnDPCG)))
            {
                // Dropped from character list
                Point p = viewSand.PointToClient(Cursor.Position);
                if (p.X > viewSand.Width - 1) p.X = viewSand.Width - 1;
                if (p.Y > viewSand.Height - 1) p.X = viewSand.Height - 1;
                int target_cell = ((p.Y / 16) * 32 + p.X / 16) % 768;
                dataSource.SetNameTable(target_cell, curPCG.Y * 32 + curPCG.X, push: true);
                this.UpdateSandbox(refresh: true);
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
        private void EditCurrentPCG(int x)
        {
            int line = curLine.Y % 8;                       // Line in one character
            int current_pcg = curPCG.Y * 32 + curPCG.X;     // Selected character
            // The target character depends on current selected line
            int current_target_pcg = (current_pcg + curLine.X + (curLine.Y / 8) * 32) % 256;
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
    }
}
