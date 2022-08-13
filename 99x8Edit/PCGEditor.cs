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
        Machine dataSource;
        MainWindow mainWin;
        private Bitmap bmpPCGList = new Bitmap(512, 128);    // PCG list view
        private Bitmap bmpPalette = new Bitmap(256, 64);     // Palette view
        private Bitmap bmpSandbox = new Bitmap(512, 384);    // Sandbox view
        private Bitmap bmpPCGEdit = new Bitmap(256, 256);    // PCG Editor view
        private Bitmap bmpColorL = new Bitmap(32, 32);
        private Bitmap bmpColorR = new Bitmap(32, 32);
        private int currentPCGX = 0;        // Selected PCG in PCG list
        private int currentPCGY = 0;
        private int selStartPCGX = 0;       // For multiple selection
        private int selStartPCGY = 0;
        private int currentSandboxX = 0;    // Selected cell in sandbox
        private int currentSandboxY = 0;
        private int selStartSandX = 0;      // For multiple selection
        private int selStartSandY = 0;
        private int currentLineX = 0;       // Selected line in editor(0-1)
        private int currentLineY = 0;       // Selected line in editor(0-15)
        private int selStartLineX = 0;      // For multiple selection
        private int selStartLineY = 0;
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
            // Initialize controls
            viewPalette.Image = bmpPalette;
            viewPCG.Image = bmpPCGList;
            viewSandbox.Image = bmpSandbox;
            viewPCGEdit.Image = bmpPCGEdit;
            viewColorL.Image = bmpColorL;
            viewColorR.Image = bmpColorR;
            checkTMS.Checked = this.dataSource.IsTMS9918;
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
            this.UpdatePCGList();           // PCG view
            this.UpdateSandbox();           // Sandbox view
            this.UpdatePCGEditView();       // PCG Editor
            this.UpdateCurrentColorView();  // Current color
            this.checkTMS.Checked = dataSource.IsTMS9918;
            this.btnOpenPalette.Enabled = !dataSource.IsTMS9918;
            this.btnSavePalette.Enabled = !dataSource.IsTMS9918;
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
            if(refresh) this.viewPalette.Refresh();
        }
        private void UpdatePCGEditView(bool refresh = true)
        {
            // Update PCG editor
            Utility.DrawTransparent(bmpPCGEdit);
            Graphics g = Graphics.FromImage(bmpPCGEdit);
            for (int i = 0; i < 4; ++i)      // four PCG in one editor
            {
                int pcg = currentPCGY * 32 + currentPCGX;
                int target_pcg = (pcg + (i / 2) * 32 + (i % 2)) % 256;
                for (int j = 0; j < 8; ++j)  // Lines in one PCG
                {
                    for (int k = 0; k < 8; ++k)
                    {
                        int p = dataSource.GetPCGPixel(target_pcg, j, k);
                        int code = dataSource.GetColorTable(target_pcg, j, (p != 0));
                        if(code != 0)
                        {
                            Color c = dataSource.ColorCodeToWindowsColor(code);
                            g.FillRectangle(new SolidBrush(Color.Gray), (i % 2) * 128 + k * 16, (i / 2) * 128 + j * 16, 16, 16);
                            g.FillRectangle(new SolidBrush(c), (i % 2) * 128 + k * 16, (i / 2) * 128 + j * 16, 15, 15);
                        }
                    }
                }
            }
            Color sel_color = panelEditor.Focused ? Consts.ColorSelectionFocused
                                                  : Consts.ColorSelectionUnfocus;
            g.DrawRectangle(new Pen(sel_color),
                            Math.Min(currentLineX, selStartLineX) * 128,
                            Math.Min(currentLineY, selStartLineY) * 16,
                            (Math.Abs(currentLineX - selStartLineX) + 1) * 128 - 1,
                            (Math.Abs(currentLineY - selStartLineY) + 1) * 16 - 1);
            if(refresh) viewPCGEdit.Refresh();
        }
        private void UpdateCurrentColorView(bool refresh = true)
        {
            // Update current color
            int current_pcg = currentPCGY * 32 + currentPCGX;
            int current_target_pcg = (current_pcg + currentLineX + (currentLineY / 8) * 32) % 256;
            int color_code_l = dataSource.GetColorTable(current_target_pcg, currentLineY % 8, true);
            int color_code_r = dataSource.GetColorTable(current_target_pcg, currentLineY % 8, false);
            Utility.DrawTransparent(bmpColorL);
            if(color_code_l > 0)
            {
                Graphics g = Graphics.FromImage(bmpColorL);
                Color c = dataSource.ColorCodeToWindowsColor(color_code_l);
                g.FillRectangle(new SolidBrush(c), 0, 0, 32, 32);
            }
            Utility.DrawTransparent(bmpColorR);
            if (color_code_r > 0)
            {
                Graphics g = Graphics.FromImage(bmpColorR);
                Color c = dataSource.ColorCodeToWindowsColor(color_code_r);
                g.FillRectangle(new SolidBrush(c), 0, 0, 32, 32);
            }
            if (refresh) viewColorL.Refresh();
            if(refresh) viewColorR.Refresh();
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
                Color sel_color = panelPCG.Focused ? Consts.ColorSelectionFocused
                                                   : Consts.ColorSelectionUnfocus;
                g.DrawRectangle(new Pen(sel_color),
                                Math.Min(currentPCGX, selStartPCGX) * 16,
                                Math.Min(currentPCGY, selStartPCGY) * 16,
                                (Math.Abs(currentPCGX - selStartPCGX) + 1) * 16 - 1,
                                (Math.Abs(currentPCGY - selStartPCGY) + 1) * 16 - 1);
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
                Color sel_color = panelSandbox.Focused ? Consts.ColorSelectionFocused
                                                       : Consts.ColorSelectionUnfocus;
                g.DrawRectangle(new Pen(sel_color),
                                Math.Min(currentSandboxX, selStartSandX) * 16,
                                Math.Min(currentSandboxY, selStartSandY) * 16,
                                (Math.Abs(currentSandboxX - selStartSandX) + 1) * 16 - 1,
                                (Math.Abs(currentSandboxY - selStartSandY) + 1) * 16 - 1);
                viewSandbox.Refresh();
            }
        }
        //-----------------------------------------------------------------------------
        // Controls
        private void viewPalette_MouseClick(object sender, MouseEventArgs e)
        {
            // Palette view clicked
            int clicked_color_num = (e.Y / 32) * 8 + (e.X / 32);
            // Update color table of current line
            int current_pcg = currentPCGY * 32 + currentPCGX;
            int current_target_pcg = (current_pcg + currentLineX + (currentLineY / 8) * 32) % 256;
            if (e.Button == MouseButtons.Left)
            {
                // Foreground color has changed
                dataSource.SetColorTable(current_target_pcg, currentLineY % 8, clicked_color_num, true, true);
            }
            else if (e.Button == MouseButtons.Right)
            {
                // Background color has changed
                dataSource.SetColorTable(current_target_pcg, currentLineY % 8, clicked_color_num, false, true);
            }
            this.RefreshAllViews();
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
                    dataSource.SetPalette(clicked_color_num,
                                          palette_win.R, palette_win.G, palette_win.B, true);
                    this.RefreshAllViews();     // Everything changes
                }
            }
        }
        private void viewColorL_Click(object sender, EventArgs e)
        {
            Action<int> callback = (x) =>
            {
                int current_pcg = currentPCGY * 32 + currentPCGX;
                int current_target_pcg = (current_pcg + currentLineX + (currentLineY / 8) * 32) % 256;
                dataSource.SetColorTable(current_target_pcg, currentLineY % 8, x, true, true);
                this.RefreshAllViews();
            };
            PaletteSelector palette_win = new PaletteSelector(bmpPalette, callback);
            palette_win.StartPosition = FormStartPosition.Manual;
            palette_win.Location = Cursor.Position;
            palette_win.Show();
        }
        private void viewColorR_Click(object sender, EventArgs e)
        {
            Action<int> callback = (x) =>
            {
                int current_pcg = currentPCGY * 32 + currentPCGX;
                int current_target_pcg = (current_pcg + currentLineX + (currentLineY / 8) * 32) % 256;
                dataSource.SetColorTable(current_target_pcg, currentLineY % 8, x, false, true);
                this.RefreshAllViews();
            };
            PaletteSelector palette_win = new PaletteSelector(bmpPalette, callback);
            palette_win.StartPosition = FormStartPosition.Manual;
            palette_win.Location = Cursor.Position;
            palette_win.Show();
        }
        private void checkTMS_Click(object sender, EventArgs e)
        {
            if (checkTMS.Checked && !dataSource.IsTMS9918)
            {
                // Set windows color of each color code to TMS9918
                dataSource.SetPaletteToTMS9918(true);
                this.RefreshAllViews();     // Everything changes
            }
            else if (!checkTMS.Checked && dataSource.IsTMS9918)
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
                if ((currentLineX != clicked_line_x) || (currentLineY != clicked_line_y))
                {
                    // Current selected line has changed
                    if (Control.ModifierKeys == Keys.Shift)
                    {
                        // Multiple selection
                        currentLineX = clicked_line_x;
                        currentLineY = clicked_line_y;
                    }
                    else
                    {
                        // New selection
                        currentLineX = selStartLineX = clicked_line_x;
                        currentLineY = selStartLineY = clicked_line_y;
                    }
                    this.UpdatePCGEditView();               // Update editor view
                    this.UpdateCurrentColorView();          // Update view of current color
                    viewPCGEdit.DoDragDrop(new DnDEditor(), DragDropEffects.Copy);
                }
                else
                {
                    // Update PCG pattern
                    this.EditCurrentPCG((e.X / 16) % 8, currentLineY % 8);
                }
            }
        }
        private void panelEditor_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            Action refresh = () =>
            {
                this.UpdatePCGEditView();               // Update editor view
                this.UpdateCurrentColorView();          // Update view of current color
            };
            switch (e.KeyData)
            {
                case Keys.Up | Keys.Shift:
                    if(currentLineY > 0)
                    {
                        currentLineY--;
                        refresh();
                    }
                    break;
                case Keys.Down | Keys.Shift:
                    if (currentLineY < 15)
                    {
                        currentLineY++;
                        refresh();
                    }
                    break;
                case Keys.Left | Keys.Shift:
                    if (currentLineX > 0)
                    {
                        currentLineX--;
                        refresh();
                    }
                    break;
                case Keys.Right | Keys.Shift:
                    if (currentLineX < 1)
                    {
                        currentLineX++;
                        refresh();
                    }
                    break;
                case Keys.Up:
                    if (currentLineY > 0)
                    {
                        currentLineY--;
                        selStartLineX = currentLineX;
                        selStartLineY = currentLineY;
                        refresh();
                    }
                    break;
                case Keys.Down:
                    if (currentLineY < 15)
                    {
                        currentLineY++;
                        selStartLineX = currentLineX;
                        selStartLineY = currentLineY;
                        refresh();
                    }
                    break;
                case Keys.Left:
                    if (currentLineX > 0)
                    {
                        currentLineX--;
                        selStartLineX = currentLineX;
                        selStartLineY = currentLineY;
                        refresh();
                    }
                    break;
                case Keys.Right:
                    if (currentLineX < 1)
                    {
                        currentLineX++;
                        selStartLineX = currentLineX;
                        selStartLineY = currentLineY;
                        refresh();
                    }
                    break;
                case Keys.D1:
                case Keys.NumPad1:
                    this.EditCurrentPCG(0, currentLineY % 8);
                    break;
                case Keys.D2:
                case Keys.NumPad2:
                    this.EditCurrentPCG(1, currentLineY % 8);
                    break;
                case Keys.D3:
                case Keys.NumPad3:
                    this.EditCurrentPCG(2, currentLineY % 8);
                    break;
                case Keys.D4:
                case Keys.NumPad4:
                    this.EditCurrentPCG(3, currentLineY % 8);
                    break;
                case Keys.D5:
                case Keys.NumPad5:
                    this.EditCurrentPCG(4, currentLineY % 8);
                    break;
                case Keys.D6:
                case Keys.NumPad6:
                    this.EditCurrentPCG(5, currentLineY % 8);
                    break;
                case Keys.D7:
                case Keys.NumPad7:
                    this.EditCurrentPCG(6, currentLineY % 8);
                    break;
                case Keys.D8:
                case Keys.NumPad8:
                    this.EditCurrentPCG(7, currentLineY % 8);
                    break;
                case Keys.Oemplus:
                case Keys.Add:
                case Keys.OemMinus:
                case Keys.Subtract:
                case Keys.OemCloseBrackets:
                case Keys.OemOpenBrackets:
                    int current_pcg = currentPCGY * 32 + currentPCGX;
                    int current_target_pcg = (current_pcg + currentLineX + (currentLineY / 8) * 32) % 256;
                    if((e.KeyData == Keys.Oemplus) || (e.KeyData == Keys.Add))
                    {
                        // Increment foreground color
                        int color = dataSource.GetColorTable(current_target_pcg, currentLineY % 8, true);
                        color = (color + 1) % 16;
                        dataSource.SetColorTable(current_target_pcg, currentLineY % 8, color, true, true);
                    }
                    if ((e.KeyData == Keys.OemMinus) || (e.KeyData == Keys.Subtract))
                    {
                        // Decrement foreground color
                        int color = dataSource.GetColorTable(current_target_pcg, currentLineY % 8, true);
                        color = (color + 15) % 16;
                        dataSource.SetColorTable(current_target_pcg, currentLineY % 8, color, true, true);
                    }
                    if (e.KeyData == Keys.OemCloseBrackets)
                    {
                        // Increment backgroundcolor
                        int color = dataSource.GetColorTable(current_target_pcg, currentLineY % 8, false);
                        color = (color + 1) % 16;
                        dataSource.SetColorTable(current_target_pcg, currentLineY % 8, color, false, true);
                    }
                    if (e.KeyData == Keys.OemOpenBrackets)
                    {
                        // Decrement background color
                        int color = dataSource.GetColorTable(current_target_pcg, currentLineY % 8, false);
                        color = (color + 15) % 16;
                        dataSource.SetColorTable(current_target_pcg, currentLineY % 8, color, false, true);
                    }
                    this.RefreshAllViews();
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
                Point p = viewPCGEdit.PointToClient(Cursor.Position);
                currentLineX = Math.Min(p.X / 128, 1);
                currentLineY = Math.Min(p.Y / 16, 15);
                this.UpdatePCGEditView();
                this.UpdateCurrentColorView();
            }
        }
        private void contextEditor_copy(object sender, EventArgs e)
        {
            ClipPCGLines clip = new ClipPCGLines();
            int x = Math.Min(currentLineX, selStartLineX);
            int y = Math.Min(currentLineY, selStartLineY);
            int w = Math.Abs(currentLineX - selStartLineX) + 1;
            int h = Math.Abs(currentLineY - selStartLineY) + 1;
            for (int i = y; i < y + h; ++i)
            {
                List<Machine.PCGLine> l = new List<Machine.PCGLine>();
                for (int j = x; j < x + w; ++j)
                {
                    int lefttop_pcg = currentPCGY * 32 + currentPCGX;
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
                for (int i = 0; (i < clip.lines.Count) && (currentLineY + i < 16); ++i)
                {
                    List<Machine.PCGLine> l = clip.lines[i];
                    for (int j = 0; (j < l.Count) && (currentLineX + j < 2); ++j)
                    {
                        int lefttop_pcg = currentPCGY * 32 + currentPCGX;
                        int pcg = (lefttop_pcg + currentLineX + j + ((currentLineY + i) / 8) * 32) % 256;
                        dataSource.SetPCGLine(pcg, (currentLineY + i) % 8, l[j], false);
                    }
                }
                this.RefreshAllViews();
            }
        }
        private void contextEditor_delete(object sender, EventArgs e)
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
                    int lefttop_pcg = currentPCGY * 32 + currentPCGX;
                    int pcg = (lefttop_pcg + j + (i / 8) * 32) % 256;
                    dataSource.ClearPCGLine(pcg, i % 8, false);
                }
            }
            this.RefreshAllViews();
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
                    int lefttop_pcg = currentPCGY * 32 + currentPCGX;
                    int pcg_src = (lefttop_pcg + (y / 8) * 32 + j) % 256;
                    int pcg_dst = (lefttop_pcg + (i / 8) * 32 + j) % 256;
                    Machine.PCGLine line = dataSource.GetPCGLine(pcg_src, y % 8);
                    dataSource.SetPCGLine(pcg_dst, i % 8, line, false);
                }
            }
            this.RefreshAllViews();
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
                    int lefttop_pcg = currentPCGY * 32 + currentPCGX;
                    int pcg_src = (lefttop_pcg + (i / 8) * 32 + x) % 256;
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
                if ((clicked_pcg_x != currentPCGX) || (clicked_pcg_y != currentPCGY))
                {
                    // Selected PCG has changed
                    if (Control.ModifierKeys == Keys.Shift)
                    {
                        // Multiple selection
                        currentPCGX = clicked_pcg_x;
                        currentPCGY = clicked_pcg_y;
                    }
                    else
                    {
                        // New selection
                        currentPCGX = selStartPCGX = clicked_pcg_x;
                        currentPCGY = selStartPCGY = clicked_pcg_y;
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
        private void panelPCG_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            Action refresh = () =>
            {
                this.UpdatePCGList();
                this.UpdatePCGEditView();
                this.UpdateCurrentColorView();
            };
            switch (e.KeyData)
            {
                case Keys.Up | Keys.Shift:
                    if (currentPCGY > 0)
                    {
                        currentPCGY--;
                        refresh();
                    }
                    break;
                case Keys.Down | Keys.Shift:
                    if (currentPCGY < 7)
                    {
                        currentPCGY++;
                        refresh();
                    }
                    break;
                case Keys.Left | Keys.Shift:
                    if (currentPCGX > 0)
                    {
                        currentPCGX--;
                        refresh();
                    }
                    break;
                case Keys.Right | Keys.Shift:
                    if (currentPCGX < 31)
                    {
                        currentPCGX++;
                        refresh();
                    }
                    break;
                case Keys.Up:
                    if (currentPCGY > 0)
                    {
                        currentPCGY--;
                        selStartPCGX = currentPCGX;
                        selStartPCGY = currentPCGY;
                        refresh();
                    }
                    break;
                case Keys.Down:
                    if (currentPCGY < 7)
                    {
                        currentPCGY++;
                        selStartPCGX = currentPCGX;
                        selStartPCGY = currentPCGY;
                        refresh();
                    }
                    break;
                case Keys.Left:
                    if (currentPCGX > 0)
                    {
                        currentPCGX--;
                        selStartPCGX = currentPCGX;
                        selStartPCGY = currentPCGY;
                        refresh();
                    }
                    break;
                case Keys.Right:
                    if (currentPCGX < 31)
                    {
                        currentPCGX++;
                        selStartPCGX = currentPCGX;
                        selStartPCGY = currentPCGY;
                        refresh();
                    }
                    break;
                case Keys.Enter:
                    dataSource.SetNameTable(currentSandboxY * 32 + currentSandboxX,
                                            currentPCGY * 32 + currentPCGX, true);
                    if (currentSandboxX < 31) currentSandboxX++;
                    this.UpdateSandbox();
                    break;
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
                currentPCGX = Math.Min(p.X / 16, 31);
                currentPCGY = Math.Min(p.Y / 16, 7);
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
                dataSource.CopyPCG(currentPCGY * 32 + currentPCGX, target_cell, true);
                this.RefreshAllViews();
            }
        }
        private void contextPCGList_copy(object sender, EventArgs e)
        {
            ClipPCG clip = new ClipPCG();
            clip.index = (byte)(currentPCGY * 32 + currentPCGX);
            int x = Math.Min(currentPCGX, selStartPCGX);
            int y = Math.Min(currentPCGY, selStartPCGY);
            int w = Math.Abs(currentPCGX - selStartPCGX) + 1;
            int h = Math.Abs(currentPCGY - selStartPCGY) + 1;
            for (int i = y; i < y + h; ++i)
            {
                List<byte[]> gen_row = new List<byte[]>();
                List<byte[]> clr_row = new List<byte[]>();
                for (int j = x; j < x + w; ++j)
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
                for (int i = 0; (i < clip.pcgGen.Count) && (currentPCGY + i < 8); ++i)
                {
                    List<byte[]> gen_line = clip.pcgGen[i];
                    List<byte[]> clr_line = clip.pcgClr[i];
                    for (int j = 0; (j < gen_line.Count) && (currentPCGX + j < 32); ++j)
                    {
                        dataSource.SetPCG((currentPCGY + i) * 32 + currentPCGX + j,
                                          gen_line[j], clr_line[j], false);
                    }
                }
                this.RefreshAllViews();
            }
            else if(clip is ClipPeekedData)
            {
                MementoCaretaker.Instance.Push();
                for (int i = 0; (i < clip.peeked.Count) && (currentPCGY + i < 8); ++i)
                {
                    List<byte[]> row = clip.peeked[i];
                    for (int j = 0; (j < row.Count) && (currentPCGX + j < 32); ++j)
                    {
                        dataSource.SetPCG((currentPCGY + i) * 32 + currentPCGX + j,
                                          row[j], null, false);
                    }
                }
                this.RefreshAllViews();
            }
        }
        private void contextPCGList_delete(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            int x = Math.Min(currentPCGX, selStartPCGX);
            int y = Math.Min(currentPCGY, selStartPCGY);
            int w = Math.Abs(currentPCGX - selStartPCGX) + 1;
            int h = Math.Abs(currentPCGY - selStartPCGY) + 1;
            for (int i = y; (i < y + h) && (i < 24); ++i)
            {
                for (int j = x; (j < x + w) && (j < 32); ++j)
                {
                    dataSource.ClearPCG(i * 32 + j);
                }
            }
            this.RefreshAllViews();
        }
        private void contextPCGList_inverse(object sender, EventArgs e)
        {
            dataSource.InversePCG(currentPCGY * 32 + currentPCGX, true);
            this.RefreshAllViews();
        }
        private void contextPCGList_copyDown(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            int x = Math.Min(currentPCGX, selStartPCGX);
            int y = Math.Min(currentPCGY, selStartPCGY);
            int w = Math.Abs(currentPCGX - selStartPCGX) + 1;
            int h = Math.Abs(currentPCGY - selStartPCGY) + 1;
            for (int i = y + 1; (i < y + h) && (i < 24); ++i)
            {
                for (int j = x; (j < x + w) && (j < 32); ++j)
                {
                    dataSource.CopyPCG(y * 32 + j, i * 32 + j, false);
                }
            }
            this.RefreshAllViews();
        }
        private void contextPCGList_copyRight(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            int x = Math.Min(currentPCGX, selStartPCGX);
            int y = Math.Min(currentPCGY, selStartPCGY);
            int w = Math.Abs(currentPCGX - selStartPCGX) + 1;
            int h = Math.Abs(currentPCGY - selStartPCGY) + 1;
            for (int i = y; (i < y + h) && (i < 24); ++i)
            {
                for (int j = x + 1; (j < x + w) && (j < 32); ++j)
                {
                    dataSource.CopyPCG(i * 32 + x, i * 32 + j, false);
                }
            }
            this.RefreshAllViews();
        }
        private void viewSandbox_MouseDown(object sender, MouseEventArgs e)
        {
            panelSandbox.Focus();   // Need this to catch CTRL+C and others
            if (e.Button == MouseButtons.Left)
            {
                int clicked_cell_x = e.X / 16;
                int clicled_cell_y = e.Y / 16;
                if (clicked_cell_x > 31) clicked_cell_x = 31;
                if (clicled_cell_y > 23) clicled_cell_y = 23;
                if ((clicked_cell_x != currentSandboxX) || (clicked_cell_x != currentSandboxY))
                {
                    if (Control.ModifierKeys == Keys.Shift)
                    {
                        // Multiple selection
                        currentSandboxX = clicked_cell_x;
                        currentSandboxY = clicled_cell_y;
                    }
                    else
                    {
                        // New selection
                        currentSandboxX = selStartSandX = clicked_cell_x;
                        currentSandboxY = selStartSandY = clicled_cell_y;
                    }
                    this.UpdateSandbox();
                }
                viewPCG.DoDragDrop(new DnDSandbox(), DragDropEffects.Copy);
            }
        }
        private void panelSandbox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Up | Keys.Shift:
                    if (currentSandboxY > 0)
                    {
                        currentSandboxY--;
                        this.UpdateSandbox();
                    }
                    break;
                case Keys.Down | Keys.Shift:
                    if (currentSandboxY < 23)
                    {
                        currentSandboxY++;
                        this.UpdateSandbox();
                    }
                    break;
                case Keys.Left | Keys.Shift:
                    if (currentSandboxX > 0)
                    {
                        currentSandboxX--;
                        this.UpdateSandbox();
                    }
                    break;
                case Keys.Right | Keys.Shift:
                    if (currentSandboxX < 31)
                    {
                        currentSandboxX++;
                        this.UpdateSandbox();
                    }
                    break;
                case Keys.Up:
                    if (currentSandboxY > 0)
                    {
                        currentSandboxY--;
                        selStartSandX = currentSandboxX;
                        selStartSandY = currentSandboxY;
                        this.UpdateSandbox();
                    }
                    break;
                case Keys.Down:
                    if (currentSandboxY < 23)
                    {
                        currentSandboxY++;
                        selStartSandX = currentSandboxX;
                        selStartSandY = currentSandboxY;
                        this.UpdateSandbox();
                    }
                    break;
                case Keys.Left:
                    if (currentSandboxX > 0)
                    {
                        currentSandboxX--;
                        selStartSandX = currentSandboxX;
                        selStartSandY = currentSandboxY;
                        this.UpdateSandbox();
                    }
                    break;
                case Keys.Right:
                    if (currentSandboxX < 31)
                    {
                        currentSandboxX++;
                        selStartSandX = currentSandboxX;
                        selStartSandY = currentSandboxY;
                        this.UpdateSandbox();
                    }
                    break;
                case Keys.Enter:
                    dataSource.SetNameTable(currentSandboxY * 32 + currentSandboxX,
                                            currentPCGY * 32 + currentPCGX, true);
                    if (currentSandboxX < 31) ++currentSandboxX;
                    this.UpdateSandbox();
                    break;
            }
        }
        private void contextSandbox_copy(object sender, EventArgs e)
        {
            ClipNametable clip = new ClipNametable();
            int x = Math.Min(currentSandboxX, selStartSandX);
            int y = Math.Min(currentSandboxY, selStartSandY);
            int w = Math.Abs(currentSandboxX - selStartSandX) + 1;
            int h = Math.Abs(currentSandboxY - selStartSandY) + 1;
            for(int i = y; i < y + h; ++i)
            {
                List<int> l = new List<int>();
                for(int j = x; j < x + w; ++j)
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
                dataSource.SetNameTable(currentSandboxY * 32 + currentSandboxX, pcgIndex, true);
                this.UpdateSandbox();
            }
            else if (clip is ClipNametable)
            {
                MementoCaretaker.Instance.Push();
                for(int i = 0; (i < clip.pcgID.Count) && (currentSandboxY + i < 24); ++i)
                {
                    List<int> l = clip.pcgID[i];
                    for(int j = 0; (j < l.Count) && (currentSandboxX + j < 32); ++j)
                    {
                        dataSource.SetNameTable((currentSandboxY + i) * 32 + currentSandboxX + j, l[j], false);
                    }
                }
                this.UpdateSandbox();
            }
        }
        private void contextSandbox_delete(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            int x = Math.Min(currentSandboxX, selStartSandX);
            int y = Math.Min(currentSandboxY, selStartSandY);
            int w = Math.Abs(currentSandboxX - selStartSandX) + 1;
            int h = Math.Abs(currentSandboxY - selStartSandY) + 1;
            for(int i = y; (i < y + h) && (i < 24); ++i)
            {
                for(int j = x; (j < x + w) && (j < 32); ++j)
                {
                    dataSource.SetNameTable(i * 32 + j, 0, false);
                }
            }
            this.UpdateSandbox();
        }
        private void contextSandbox_paint(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();   // For undo action
            this.PaintSandbox(currentSandboxX, currentSandboxY, currentPCGY * 32 + currentPCGX);
            this.UpdateSandbox();
        }
        private void contextSandbox_copyDown(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            int x = Math.Min(currentSandboxX, selStartSandX);
            int y = Math.Min(currentSandboxY, selStartSandY);
            int w = Math.Abs(currentSandboxX - selStartSandX) + 1;
            int h = Math.Abs(currentSandboxY - selStartSandY) + 1;
            for (int i = y + 1; (i < y + h) && (i < 24); ++i)
            {
                for (int j = x; (j < x + w) && (j < 32); ++j)
                {
                    int src = dataSource.GetNameTable(y * 32 + j);
                    dataSource.SetNameTable(i * 32 + j, src, false);
                }
            }
            this.UpdateSandbox();
        }
        private void contextSandbox_copyRight(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            int x = Math.Min(currentSandboxX, selStartSandX);
            int y = Math.Min(currentSandboxY, selStartSandY);
            int w = Math.Abs(currentSandboxX - selStartSandX) + 1;
            int h = Math.Abs(currentSandboxY - selStartSandY) + 1;
            for (int i = y; (i < y + h) && (i < 24); ++i)
            {
                for (int j = x + 1; (j < x + w) && (j < 32); ++j)
                {
                    int src = dataSource.GetNameTable(i * 32 + x);
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
                currentSandboxX = Math.Min(p.X / 16, 31);
                currentSandboxY = Math.Min(p.Y / 16, 23);
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
                dataSource.SetNameTable(target_cell, currentPCGY * 32 + currentPCGX, true);
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
        private void btnSavePalette_Click(object sender, EventArgs e)
        {
            Utility.SaveDialogAndSave(currentFile,
                                      "PLT File(*.plt)|*.plt",
                                      "Save palette",
                                      dataSource.SavePaletteSettings,
                                      true,
                                      out _);
        }
        private void btnOpenPalette_Click(object sender, EventArgs e)
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
        //---------------------------------------------------------------------
        // Utility
        private void EditCurrentPCG(int x, int y)
        {
            int current_pcg = currentPCGY * 32 + currentPCGX;
            int current_target_pcg = (current_pcg + currentLineX + (currentLineY / 8) * 32) % 256;
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
