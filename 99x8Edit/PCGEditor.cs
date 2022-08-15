using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace _99x8Edit
{
    // PCG editor window
    public partial class PCGEditor : Form
    {
        private readonly Machine dataSource;
        private readonly MainWindow mainWin;
        private readonly List<Control> tabOrder = new List<Control>();
        private Bitmap bmpPCGList = new Bitmap(512, 128);    // PCG list view
        private Bitmap bmpPalette = new Bitmap(256, 64);     // Palette view
        private Bitmap bmpSandbox = new Bitmap(512, 384);    // Sandbox view
        private Bitmap bmpPCGEdit = new Bitmap(256, 256);    // PCG Editor view
        private Bitmap bmpColorL = new Bitmap(32, 32);
        private Bitmap bmpColorR = new Bitmap(32, 32);
        private Point curPCG;               // Selected PCG in PCG list
        private Point selStartPCG;          // For multiple selection
        private Point curSand;              // Selected cell in sandbox
        private Point selStartSand;         // For multiple selection
        private Point curLine;              // Selected line in editor
        private Point selStartLine;         // For multiple selection
        private int currentDot = 0;
        private int currentColor = 0;       // Currently elected color, 0=left, 1=right
        private Point currentPal;           // Selection in palette
        String currentFile = "";
        public String CurrentFile
        {
            set { currentFile = value; }
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
            // Tab order
            tabOrder.AddRange(new Control[] { panelEditor, panelColor, panelPalette, panelPCG, panelSandbox });
            // Initialize controls
            viewPalette.Image = bmpPalette;
            viewPCG.Image = bmpPCGList;
            viewSandbox.Image = bmpSandbox;
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
        protected override bool ProcessTabKey(bool forward)
        {
            Control prev = this.ActiveControl;
            int index = tabOrder.IndexOf(prev);
            index += forward ? 1 : tabOrder.Count - 1;
            index %= tabOrder.Count;
            this.ActiveControl = tabOrder[index];
            this.RefreshAllViews();
            return true;
        }
        //------------------------------------------------------------------------------
        // Refreshing Views
        private void RefreshAllViews()
        {
            this.UpdatePaletteView();       // Palette view
            this.UpdatePCGList();           // PCG view
            this.UpdateSandbox();           // Sandbox view
            this.UpdatePCGEditView();       // PCG Editor
            this.UpdateCurrentColorView();  // Current color
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
                Color c = dataSource.ColorOf(i);
                g.FillRectangle(new SolidBrush(c), (i % 8) * 32, (i / 8) * 32, 32, 32);
            }
            // Current selection
            Utility.DrawSelection(g, currentPal.X * 32, currentPal.Y * 32, 31, 31, panelPalette.Focused);
            if(refresh) this.viewPalette.Refresh();
        }
        private void UpdatePCGEditView(bool refresh = true)
        {
            // Update PCG editor
            Utility.DrawTransparent(bmpPCGEdit);
            Graphics g = Graphics.FromImage(bmpPCGEdit);
            for (int i = 0; i < 4; ++i)      // four PCG in one editor
            {
                int pcg = curPCG.Y * 32 + curPCG.X;
                int target_pcg = (pcg + (i / 2) * 32 + (i % 2)) % 256;
                for (int j = 0; j < 8; ++j)  // Lines in one PCG
                {
                    for (int k = 0; k < 8; ++k)
                    {
                        int p = dataSource.GetPCGPixel(target_pcg, j, k);
                        int code = dataSource.GetColorTable(target_pcg, j, (p != 0));
                        if(code != 0)
                        {
                            Color c = dataSource.ColorOf(code);
                            g.FillRectangle(new SolidBrush(Color.Gray), (i % 2) * 128 + k * 16, (i / 2) * 128 + j * 16, 16, 16);
                            g.FillRectangle(new SolidBrush(c), (i % 2) * 128 + k * 16, (i / 2) * 128 + j * 16, 15, 15);
                        }
                    }
                }
            }
            Rectangle r = Utility.Point2Rect(curLine, selStartLine);
            Utility.DrawSelection(g, r.X * 128, r.Y * 16, r.Width * 128 - 1, r.Height * 16 - 1, panelEditor.Focused);
            if (panelEditor.Focused)
            {
                // One dot can be selected when focused
                Utility.DrawSubSelection(g, r.X * 128 + currentDot * 16, r.Y * 16, 14, 14);
            }
            if (refresh) viewPCGEdit.Refresh();
        }
        private void UpdateCurrentColorView(bool refresh = true)
        {
            // Update current color
            Graphics gl = Graphics.FromImage(bmpColorL);
            Graphics gr = Graphics.FromImage(bmpColorR);
            int current_pcg = curPCG.Y * 32 + curPCG.X;
            int current_target_pcg = (current_pcg + curLine.X + (curLine.Y / 8) * 32) % 256;
            int color_code_l = dataSource.GetColorTable(current_target_pcg, curLine.Y % 8, true);
            int color_code_r = dataSource.GetColorTable(current_target_pcg, curLine.Y % 8, false);
            Utility.DrawTransparent(bmpColorL);
            if(color_code_l > 0)
            {
                Color c = dataSource.ColorOf(color_code_l);
                gl.FillRectangle(new SolidBrush(c), 0, 0, 32, 32);
            }
            if (currentColor == 0)
            {
                Utility.DrawSelection(gl, 0, 0, 29, 29, panelColor.Focused);
            }
            Utility.DrawTransparent(bmpColorR);
            if (color_code_r > 0)
            {
                Color c = dataSource.ColorOf(color_code_r);
                gr.FillRectangle(new SolidBrush(c), 0, 0, 32, 32);
            }
            if (currentColor == 1)
            {
                Utility.DrawSelection(gr, 0, 0, 29, 29, panelColor.Focused);
            }
            if (refresh)
            {
                viewColorL.Refresh();
                viewColorR.Refresh();
            }
        }
        private void UpdatePCGList(bool refresh = true)
        {
            // Update all PCG list
            Graphics g = Graphics.FromImage(bmpPCGList);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            g.FillRectangle(new SolidBrush(Color.Black), 0, 0, bmpPCGList.Width, bmpPCGList.Height);
            for (int i = 0; i < 256; ++i)
            {
                g.DrawImage(dataSource.GetBitmapOfPCG(i), (i % 32) * 16, (i / 32) * 16, 17, 17);
            }
            if (refresh)
            {
                if (chkCRT.Checked)
                {
                    // CRT Filter
                    Filter.Create(Filter.Type.CRT).Process(bmpPCGList);
                }
                // Current selection
                Utility.DrawSelection(g, curPCG, selStartPCG, 16, 16, panelPCG.Focused);
                viewPCG.Refresh();
            }
        }
        private void UpdateSandbox(bool refresh = true)
        {
            // Update all sandbox
            Graphics g = Graphics.FromImage(bmpSandbox);
            g.FillRectangle(new SolidBrush(Color.Black), 0, 0, bmpSandbox.Width, bmpSandbox.Height);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            for (int i = 0; i < 768; ++i)
            {
                int ptn = dataSource.GetNameTable(i);
                g.DrawImage(dataSource.GetBitmapOfPCG(ptn), (i % 32) * 16, (i / 32) * 16, 17, 17);
            }
            if (refresh)
            {
                if (chkCRT.Checked)
                {
                    // CRT Filter
                    Filter.Create(Filter.Type.CRT).Process(bmpSandbox);
                }
                // Current selection
                Utility.DrawSelection(g, curSand, selStartSand, 16, 16, panelSandbox.Focused);
                viewSandbox.Refresh();
            }
        }
        //-----------------------------------------------------------------------------
        // Controls
        private void viewPalette_MouseClick(object sender, MouseEventArgs e)
        {
            // Palette view clicked
            panelPalette.Focus();      // Key events to parent panel
            int clicked_color_num = Math.Clamp((e.Y / 32) * 8 + (e.X / 32), 0, 15);
            currentPal.X = clicked_color_num % 8;
            currentPal.Y = clicked_color_num / 8;
            // Update color table of current line
            int current_pcg = curPCG.Y * 32 + curPCG.X;
            int current_target_pcg = (current_pcg + curLine.X + (curLine.Y / 8) * 32) % 256;
            if (e.Button == MouseButtons.Left)
            {
                // Foreground color has changed
                dataSource.SetColorTable(current_target_pcg, curLine.Y % 8, clicked_color_num, true, true);
            }
            else if (e.Button == MouseButtons.Right)
            {
                // Background color has changed
                dataSource.SetColorTable(current_target_pcg, curLine.Y % 8, clicked_color_num, false, true);
            }
            this.RefreshAllViews();
        }
        private void viewPalette_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!chkTMS.Checked)
            {
                int clicked_color_num = (e.Y / 32) * 8 + (e.X / 32);
                this.EditPalette(clicked_color_num);
            }
        }
        private void viewColorL_Click(object sender, EventArgs e)
        {
            panelColor.Focus();
            currentColor = 0;
            this.UpdateCurrentColorView();
            int current_pcg = curPCG.Y * 32 + curPCG.X;
            int current_target_pcg = (current_pcg + curLine.X + (curLine.Y / 8) * 32) % 256;
            Action<int> callback = (x) =>
            {
                dataSource.SetColorTable(current_target_pcg, curLine.Y % 8, x, true, true);
                this.RefreshAllViews();
            };
            int color_code_l = dataSource.GetColorTable(current_target_pcg, curLine.Y % 8, true);
            PaletteSelector palette_win = new PaletteSelector(bmpPalette, color_code_l, callback);
            palette_win.StartPosition = FormStartPosition.Manual;
            palette_win.Location = Cursor.Position;
            palette_win.Show();
        }
        private void viewColorR_Click(object sender, EventArgs e)
        {
            panelColor.Focus();
            currentColor = 1;
            this.UpdateCurrentColorView();
            int current_pcg = curPCG.Y * 32 + curPCG.X;
            int current_target_pcg = (current_pcg + curLine.X + (curLine.Y / 8) * 32) % 256;
            Action<int> callback = (x) =>
            {
                dataSource.SetColorTable(current_target_pcg, curLine.Y % 8, x, false, true);
                this.RefreshAllViews();
            };
            int color_code_r = dataSource.GetColorTable(current_target_pcg, curLine.Y % 8, false);
            PaletteSelector palette_win = new PaletteSelector(bmpPalette, color_code_r, callback);
            palette_win.StartPosition = FormStartPosition.Manual;
            palette_win.Location = Cursor.Position;
            palette_win.Show();
        }
        private void checkTMS_Click(object sender, EventArgs e)
        {
            if (chkTMS.Checked && !dataSource.IsTMS9918)
            {
                // Set windows color of each color code to TMS9918
                dataSource.SetPaletteToTMS9918(true);
                this.RefreshAllViews();     // Everything changes
            }
            else if (!chkTMS.Checked && dataSource.IsTMS9918)
            {
                // Set windows color of each color code to internal palette
                dataSource.SetPaletteToV9938(true);
                this.RefreshAllViews();     // Everything changes
            }
        }
        private void viewPCGEdit_MouseDown(object sender, MouseEventArgs e)
        {
            // PCG editor is clicked
            panelEditor.Focus();    // Key events are handled by parent panel
            if (e.Button == MouseButtons.Left)
            {
                int clicked_line_x = e.X / 128;
                int clicked_line_y = e.Y / 16;
                if ((curLine.X != clicked_line_x) || (curLine.Y != clicked_line_y))
                {
                    // Current selected line has changed
                    if (Control.ModifierKeys == Keys.Shift)
                    {
                        // Multiple selection
                        curLine.X = clicked_line_x;
                        curLine.Y = clicked_line_y;
                    }
                    else
                    {
                        // New selection
                        curLine.X = selStartLine.X = clicked_line_x;
                        curLine.Y = selStartLine.Y = clicked_line_y;
                    }
                    this.UpdatePCGEditView();               // Update editor view
                    this.UpdateCurrentColorView();          // Update view of current color
                    viewPCGEdit.DoDragDrop(new DnDEditor(), DragDropEffects.Copy);
                }
                else
                {
                    // Update PCG pattern
                    this.EditCurrentPCG((e.X / 16) % 8, curLine.Y % 8);
                }
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
                Point p = viewPCGEdit.PointToClient(Cursor.Position);
                curLine.X = Math.Min(p.X / 128, 1);
                curLine.Y = Math.Min(p.Y / 16, 15);
                this.UpdatePCGEditView();
                this.UpdateCurrentColorView();
            }
        }
        private void contextEditor_copy(object sender, EventArgs e)
        {
            ClipPCGLines clip = new ClipPCGLines();
            Rectangle r = Utility.Point2Rect(curLine, selStartLine);
            for (int i = r.Y; i < r.Y + r.Height; ++i)
            {
                List<Machine.PCGLine> l = new List<Machine.PCGLine>();
                for (int j = r.X; j < r.X + r.Width; ++j)
                {
                    int lefttop_pcg = curPCG.Y * 32 + curPCG.X;
                    int pcg = (lefttop_pcg + j + (i / 8) * 32) % 256;
                    l.Add(dataSource.GetPCGLine(pcg, i % 8));
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
                    List<Machine.PCGLine> l = clip.lines[i];
                    for (int j = 0; (j < l.Count) && (curLine.X + j < 2); ++j)
                    {
                        int lefttop_pcg = curPCG.Y * 32 + curPCG.X;
                        int pcg = (lefttop_pcg + curLine.X + j + ((curLine.Y + i) / 8) * 32) % 256;
                        dataSource.SetPCGLine(pcg, (curLine.Y + i) % 8, l[j], false);
                    }
                }
                this.RefreshAllViews();
            }
        }
        private void contextEditor_delete(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            Rectangle r = Utility.Point2Rect(curLine, selStartLine);
            for (int i = r.Y; i < r.Y + r.Height; ++i)
            {
                for (int j = r.X; j < r.X + r.Width; ++j)
                {
                    int lefttop_pcg = curPCG.Y * 32 + curPCG.X;
                    int pcg = (lefttop_pcg + j + (i / 8) * 32) % 256;
                    dataSource.ClearPCGLine(pcg, i % 8, false);
                }
            }
            this.RefreshAllViews();
        }
        private void contextEditor_copyDown(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            Rectangle r = Utility.Point2Rect(curLine, selStartLine);
            for (int i = r.Y + 1; i < r.Y + r.Height; ++i)
            {
                for (int j = r.X; j < r.X + r.Width; ++j)
                {
                    int lefttop_pcg = curPCG.Y * 32 + curPCG.X;
                    int pcg_src = (lefttop_pcg + (r.Y / 8) * 32 + j) % 256;
                    int pcg_dst = (lefttop_pcg + (i / 8) * 32 + j) % 256;
                    Machine.PCGLine line = dataSource.GetPCGLine(pcg_src, r.Y % 8);
                    dataSource.SetPCGLine(pcg_dst, i % 8, line, false);
                }
            }
            this.RefreshAllViews();
        }
        private void contextEditor_copyRight(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            Rectangle r = Utility.Point2Rect(curLine, selStartLine);
            for (int i = r.Y; i < r.Y + r.Height; ++i)
            {
                for (int j = r.X + 1; j < r.X + r.Width; ++j)
                {
                    int lefttop_pcg = curPCG.Y * 32 + curPCG.X;
                    int pcg_src = (lefttop_pcg + (i / 8) * 32 + r.X) % 256;
                    int pcg_dst = (lefttop_pcg + (i / 8) * 32 + j) % 256;
                    Machine.PCGLine line = dataSource.GetPCGLine(pcg_src, i % 8);
                    dataSource.SetPCGLine(pcg_dst, i % 8, line, false);
                }
            }
            this.RefreshAllViews();
        }
        private void viewPCG_MouseDown(object sender, MouseEventArgs e)
        {
            panelPCG.Focus();   // Key events are handled by parent panel
            if (e.Button == MouseButtons.Left)
            {
                int clicked_pcg_x = e.X / 16;
                int clicked_pcg_y = e.Y / 16;
                if (clicked_pcg_x > 31) clicked_pcg_x = 31;
                if (clicked_pcg_y > 7) clicked_pcg_y = 7;
                if ((clicked_pcg_x != curPCG.X) || (clicked_pcg_y != curPCG.Y))
                {
                    // Selected PCG has changed
                    if (Control.ModifierKeys == Keys.Shift)
                    {
                        // Multiple selection
                        curPCG.X = clicked_pcg_x;
                        curPCG.Y = clicked_pcg_y;
                    }
                    else
                    {
                        // New selection
                        curPCG.X = selStartPCG.X = clicked_pcg_x;
                        curPCG.Y = selStartPCG.Y = clicked_pcg_y;
                    }
                    this.UpdatePCGList();
                    this.UpdatePCGEditView();
                    this.UpdateCurrentColorView();
                    viewPCG.DoDragDrop(new DnDPCGSel(), DragDropEffects.Copy);
                    return;
                }
                viewPCG.DoDragDrop(new DnDPCG(), DragDropEffects.Copy);
            }
        }
        private void panelPCG_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DnDPCG)))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else if (e.Data.GetDataPresent(typeof(DnDPCGSel)))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else e.Effect = DragDropEffects.None;
        }
        private void panelPCG_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DnDPCGSel)))
            {
                Point p = viewPCG.PointToClient(Cursor.Position);
                curPCG.X = Math.Min(p.X / 16, 31);
                curPCG.Y = Math.Min(p.Y / 16, 7);
                this.UpdatePCGList();
                this.UpdatePCGEditView();
                this.UpdateCurrentColorView();
            }
        }
        private void panelPCG_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DnDPCG)))
            {
                Point p = viewPCG.PointToClient(Cursor.Position);
                if (p.X > viewPCG.Width - 1) p.X = viewPCG.Width - 1;
                if (p.Y > viewPCG.Height - 1) p.X = viewPCG.Height - 1;
                int target_cell = ((p.Y / 16) * 32 + p.X / 16) % 256;
                dataSource.CopyPCG(curPCG.Y * 32 + curPCG.X, target_cell, true);
                this.RefreshAllViews();
            }
        }
        private void contextPCGList_copy(object sender, EventArgs e)
        {
            ClipPCG clip = new ClipPCG();
            clip.index = (byte)(curPCG.Y * 32 + curPCG.X);
            Rectangle r = Utility.Point2Rect(curPCG, selStartPCG);
            for (int i = r.Y; i < r.Y + r.Height; ++i)
            {
                List<byte[]> gen_row = new List<byte[]>();
                List<byte[]> clr_row = new List<byte[]>();
                for (int j = r.X; j < r.X + r.Width; ++j)
                {
                    gen_row.Add(dataSource.GetPCGGen(i * 32 + j));
                    clr_row.Add(dataSource.GetPCGClr(i * 32 + j));
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
                        dataSource.SetPCG((curPCG.Y + i) * 32 + curPCG.X + j,
                                          gen_line[j], clr_line[j], false);
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
                        dataSource.SetPCG((curPCG.Y + i) * 32 + curPCG.X + j,
                                          row[j], null, false);
                    }
                }
                this.RefreshAllViews();
            }
        }
        private void contextPCGList_delete(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            Rectangle r = Utility.Point2Rect(curPCG, selStartPCG);
            for (int i = r.Y; (i < r.Y + r.Height) && (i < 24); ++i)
            {
                for (int j = r.X; (j < r.X + r.Width) && (j < 32); ++j)
                {
                    dataSource.ClearPCG(i * 32 + j);
                }
            }
            this.RefreshAllViews();
        }
        private void contextPCGList_inverse(object sender, EventArgs e)
        {
            dataSource.InversePCG(curPCG.Y * 32 + curPCG.X, true);
            this.RefreshAllViews();
        }
        private void contextPCGList_copyDown(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            Rectangle r = Utility.Point2Rect(curPCG, selStartPCG);
            for (int i = r.Y + 1; (i < r.Y + r.Height) && (i < 24); ++i)
            {
                for (int j = r.X; (j < r.X + r.Width) && (j < 32); ++j)
                {
                    dataSource.CopyPCG(r.Y * 32 + j, i * 32 + j, false);
                }
            }
            this.RefreshAllViews();
        }
        private void contextPCGList_copyRight(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            Rectangle r = Utility.Point2Rect(curPCG, selStartPCG);
            for (int i = r.Y; (i < r.Y + r.Height) && (i < 24); ++i)
            {
                for (int j = r.X + 1; (j < r.X + r.Width) && (j < 32); ++j)
                {
                    dataSource.CopyPCG(i * 32 + r.X, i * 32 + j, false);
                }
            }
            this.RefreshAllViews();
        }
        private void viewSandbox_MouseDown(object sender, MouseEventArgs e)
        {
            panelSandbox.Focus();
            if (e.Button == MouseButtons.Left)
            {
                int clicked_cell_x = e.X / 16;
                int clicled_cell_y = e.Y / 16;
                if (clicked_cell_x > 31) clicked_cell_x = 31;
                if (clicled_cell_y > 23) clicled_cell_y = 23;
                if ((clicked_cell_x != curSand.X) || (clicked_cell_x != curSand.Y))
                {
                    if (Control.ModifierKeys == Keys.Shift)
                    {
                        // Multiple selection
                        curSand.X = clicked_cell_x;
                        curSand.Y = clicled_cell_y;
                    }
                    else
                    {
                        // New selection
                        curSand.X = selStartSand.X = clicked_cell_x;
                        curSand.Y = selStartSand.Y = clicled_cell_y;
                    }
                    this.UpdateSandbox();
                }
                viewPCG.DoDragDrop(new DnDSandbox(), DragDropEffects.Copy);
            }
        }
        private void contextSandbox_copy(object sender, EventArgs e)
        {
            ClipNametable clip = new ClipNametable();
            Rectangle r = Utility.Point2Rect(curSand, selStartSand);
            for(int i = r.Y; i < r.Y + r.Height; ++i)
            {
                List<int> l = new List<int>();
                for(int j = r.X; j < r.X + r.Width; ++j)
                {
                    l.Add(dataSource.GetNameTable(i * 32 + j));
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
                int pcgIndex = clip.index;
                dataSource.SetNameTable(curSand.Y * 32 + curSand.X, pcgIndex, true);
                this.UpdateSandbox();
            }
            else if (clip is ClipNametable)
            {
                MementoCaretaker.Instance.Push();
                for(int i = 0; (i < clip.pcgID.Count) && (curSand.Y + i < 24); ++i)
                {
                    List<int> l = clip.pcgID[i];
                    for(int j = 0; (j < l.Count) && (curSand.X + j < 32); ++j)
                    {
                        dataSource.SetNameTable((curSand.Y + i) * 32 + curSand.X + j, l[j], false);
                    }
                }
                this.UpdateSandbox();
            }
        }
        private void contextSandbox_delete(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            Rectangle r = Utility.Point2Rect(curSand, selStartSand);
            for(int i = r.Y; (i < r.Y + r.Height) && (i < 24); ++i)
            {
                for(int j = r.X; (j < r.X + r.Width) && (j < 32); ++j)
                {
                    dataSource.SetNameTable(i * 32 + j, 0, false);
                }
            }
            this.UpdateSandbox();
        }
        private void contextSandbox_paint(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();   // For undo action
            this.PaintSandbox(curSand.X, curSand.Y, curPCG.Y * 32 + curPCG.X);
            this.UpdateSandbox();
        }
        private void contextSandbox_copyDown(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            Rectangle r = Utility.Point2Rect(curSand, selStartSand);
            for (int i = r.Y + 1; (i < r.Y + r.Height) && (i < 24); ++i)
            {
                for (int j = r.X; (j < r.X + r.Width) && (j < 32); ++j)
                {
                    int src = dataSource.GetNameTable(r.Y * 32 + j);
                    dataSource.SetNameTable(i * 32 + j, src, false);
                }
            }
            this.UpdateSandbox();
        }
        private void contextSandbox_copyRight(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            Rectangle r = Utility.Point2Rect(curSand, selStartSand);
            for (int i = r.Y; (i < r.Y + r.Height) && (i < 24); ++i)
            {
                for (int j = r.X + 1; (j < r.X + r.Width) && (j < 32); ++j)
                {
                    int src = dataSource.GetNameTable(i * 32 + r.X);
                    dataSource.SetNameTable(i * 32 + j, src, false);
                }
            }
            this.UpdateSandbox();
        }
        private void panelSandbox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DnDPCG)))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else if (e.Data.GetDataPresent(typeof(DnDSandbox)))
            {
                e.Effect = DragDropEffects.All;
            }
            else e.Effect = DragDropEffects.None;
        }
        private void panelSandbox_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DnDSandbox)))
            {
                Point p = viewSandbox.PointToClient(Cursor.Position);
                curSand.X = Math.Min(p.X / 16, 31);
                curSand.Y = Math.Min(p.Y / 16, 23);
                this.UpdateSandbox();
            }
        }
        private void panelSandbox_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DnDPCG)))
            {
                Point p = viewSandbox.PointToClient(Cursor.Position);
                if (p.X > viewSandbox.Width - 1) p.X = viewSandbox.Width - 1;
                if (p.Y > viewSandbox.Height - 1) p.X = viewSandbox.Height - 1;
                int target_cell = ((p.Y / 16) * 32 + p.X / 16) % 768;
                dataSource.SetNameTable(target_cell, curPCG.Y * 32 + curPCG.X, true);
                this.UpdateSandbox();
            }
        }
        private void FormPCG_Activated(object sender, EventArgs e)
        {
            this.RefreshAllViews();      // Update everything since palette may be changed
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
            if (Utility.LoadDialogAndLoad(currentFile,
                                          "PCG File(*.pcg)|*.pcg",
                                          "Load PCG settings",
                                          dataSource.LoadPCG,
                                          true,     // Push memento
                                          out _))
            {
                this.RefreshAllViews();
            }
        }
        private void menu_fileSavePCG(object sender, EventArgs e)
        {
            Utility.SaveDialogAndSave(currentFile,
                                      "PCG File(*.pcg)|*.pcg",
                                      "Save PCG settings",
                                      dataSource.SavePCG,
                                      true,
                                      out _);
        }
        private void menu_savePalette(object sender, EventArgs e)
        {
            Utility.SaveDialogAndSave(currentFile,
                                      "PLT File(*.plt)|*.plt",
                                      "Save palette",
                                      dataSource.SavePaletteSettings,
                                      true,
                                      out _);
        }
        private void menu_loadPalette(object sender, EventArgs e)
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
        private void EditCurrentPCG(int x, int y)
        {
            int current_pcg = curPCG.Y * 32 + curPCG.X;
            int current_target_pcg = (current_pcg + curLine.X + (curLine.Y / 8) * 32) % 256;
            int prev_pixel = dataSource.GetPCGPixel(current_target_pcg, y, x);
            if (prev_pixel == 0)
            {
                dataSource.SetPCGPixel(current_target_pcg, y, x, 1, true);
            }
            else
            {
                dataSource.SetPCGPixel(current_target_pcg, y, x, 0, true);
            }
            this.UpdatePCGEditView();   // PCG Editor view changes
            this.UpdatePCGList();       // PCG list view changes also
            this.UpdateSandbox();       // Update sandbox view
        }
        private void EditPalette(int index)
        {
            int R = dataSource.GetPaletteR(index);
            int G = dataSource.GetPaletteG(index);
            int B = dataSource.GetPaletteB(index);
            PaletteEditor palette_win = null ;
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
        private void PaintSandbox(int x, int y, int val)
        {
            int pcg_to_paint = dataSource.GetNameTable(y * 32 + x);
            if (pcg_to_paint == val) return;
            dataSource.SetNameTable(y * 32 + x, val, false);
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
