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
    public partial class PCGEditor : Form
    {
        Machine dataSource;
        private Bitmap bmpPCGList = new Bitmap(512, 128);    // PCG list view
        private Bitmap bmpPalette = new Bitmap(256, 64);     // Palette view
        private Bitmap bmpSandbox = new Bitmap(512, 384);    // Sandbox view
        private Bitmap bmpPCGEdit = new Bitmap(256, 256);    // PCG Editor view
        private int currentPCG = 0;         // Selected Character(0-255)
        private int currentSandbox = 0;     // Selected Coorinate in sandbox(0-767)
        private int currentLineX = 0;       // Selected line in editor(0-1)
        private int currentLineY = 0;       // Selected line in editor(0-15)
        String currentFile = "";
        public String CurrentFile
        {
            set
            {
                currentFile = value;
            }
        }
        //------------------------------------------------------------------------------
        // Initialize
        public PCGEditor(Machine dataSource)
        {
            InitializeComponent();
            // Set corresponding data
            this.dataSource = dataSource;
            // Refresh all views
            this.RefreshAllViews();
            // Initialize controls
            this.viewPalette.Image = bmpPalette;
            this.viewPCG.Image = bmpPCGList;
            this.viewSandbox.Image = bmpSandbox;
            this.viewPCGEdit.Image = bmpPCGEdit;
            checkTMS.Checked = this.dataSource.IsTMS9918();
            // Context menu
            toolStripPCGCopy.Click += new EventHandler(contextPCGList_MouseClick_copy);
            toolStripPCGPaste.Click += new EventHandler(contextPCGList_MouseClick_paste);
            toolStripPCGDel.Click += new EventHandler(contextPCGList_MouseClick_delete);
            toolStripSandboxCopy.Click += new EventHandler(contextSandbox_MouseClick_copy);
            toolStripSandboxPaste.Click += new EventHandler(contextSandbox_MouseClick_paste);
            toolStripSandboxDel.Click += new EventHandler(contextSandbox_MouseClick_delete);
            toolStripSandboxPaint.Click += new EventHandler(contextSandbox_MouseClick_paint);
            toolStripEditorCopy.Click += new EventHandler(contextEditor_MouseClick_copy);
            toolStripEditorPaste.Click += new EventHandler(contextEditor_MouseClick_paste);
            toolStripEditorDel.Click += new EventHandler(contextEditor_MouseClick_delete);
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
            if (dataSource.IsTMS9918() && !this.checkTMS.Checked)
            {
                this.checkTMS.Checked = true;
            }
            if (!dataSource.IsTMS9918() && this.checkTMS.Checked)
            {
                this.checkTMS.Checked = false;
            }
            this.btnOpenPalette.Enabled = !dataSource.IsTMS9918();
            this.btnSavePalette.Enabled = !dataSource.IsTMS9918();
        }
        private void UpdatePaletteView()
        {
            // Update palette view
            Graphics g = Graphics.FromImage(bmpPalette);
            for (int i = 0; i < 16; ++i)
            {
                Color c = dataSource.ColorCodeToWindowsColor(i);
                g.FillRectangle(new SolidBrush(c), new Rectangle((i % 8) * 32, (i / 8) * 32, 32, 32));
            }
            this.viewPalette.Refresh();
        }
        private void UpdatePCGEditView()
        {
            // Update PCG editor
            Graphics g = Graphics.FromImage(bmpPCGEdit);
            g.FillRectangle(new SolidBrush(Color.Gray), 0, 0, 256, 256);
            for (int i = 0; i < 4; ++i)      // four PCG in one editor
            {
                int target_pcg = (currentPCG + (i / 2) * 32 + (i % 2)) % 256;
                for (int j = 0; j < 8; ++j)  // Lines in one PCG
                {
                    for (int k = 0; k < 8; ++k)
                    {
                        Color c = dataSource.GetBitmapOfPCG(target_pcg).GetPixel(k, j);
                        g.FillRectangle(new SolidBrush(c), (i % 2) * 128 + k * 16, (i / 2) * 128 + j * 16, 15, 15);
                    }
                }
            }
            g.DrawRectangle(new Pen(Color.Red), currentLineX * 128, currentLineY * 16, 127, 15);
            this.viewPCGEdit.Refresh();
        }
        private void UpdateCurrentColorView()
        {
            // Update current color
            int current_target_pcg = (currentPCG + currentLineX + (currentLineY / 8) * 32) % 256;
            int color_code_l = dataSource.GetColorTable(current_target_pcg, currentLineY % 8, true);
            int color_code_r = dataSource.GetColorTable(current_target_pcg, currentLineY % 8, false);
            viewColorL.BackColor = dataSource.ColorCodeToWindowsColor(color_code_l);
            viewColorR.BackColor = dataSource.ColorCodeToWindowsColor(color_code_r);
            this.viewColorL.Refresh();
            this.viewColorR.Refresh();
        }
        private void UpdatePCGList()
        {
            // Update all PCG list
            for (int i = 0; i < 256; ++i)
            {
                this.UpdatePCGList(i);
            }
        }
        private void UpdatePCGList(int pcg)
        {
            // Update one PCG of PCG list
            Graphics g = Graphics.FromImage(bmpPCGList);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            g.DrawImage(dataSource.GetBitmapOfPCG(pcg), (pcg % 32) * 16, (pcg / 32) * 16, 17, 17);
            if (pcg == currentPCG)
                g.DrawRectangle(new Pen(Color.Red), (pcg % 32) * 16, (pcg / 32) * 16, 15, 15);
            this.viewPCG.Refresh();
        }
        private void UpdateSandbox()
        {
            // Update all sandbox
            Graphics g = Graphics.FromImage(bmpSandbox);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            for (int i = 0; i < 768; ++i)
            {
                int ptn = dataSource.GetNameTable(i);
                g.DrawImage(dataSource.GetBitmapOfPCG(ptn), (i % 32) * 16, (i / 32) * 16, 17, 17);
                if (i == currentSandbox)
                    g.DrawRectangle(new Pen(Color.Red), (i % 32) * 16, (i / 32) * 16, 15, 15);
            }
            this.viewSandbox.Refresh();
        }
        private void UpdateSandbox(int pcg)
        {
            // Update sandbox by updated PCG
            Graphics g = Graphics.FromImage(bmpSandbox);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            for (int i = 0; i < 768; ++i)
            {
                int ptn = dataSource.GetNameTable(i);
                if (ptn == pcg)
                {
                    g.DrawImage(dataSource.GetBitmapOfPCG(ptn), (i % 32) * 16, (i / 32) * 16, 17, 17);
                    if (i == currentSandbox)
                        g.DrawRectangle(new Pen(Color.Red), (i % 32) * 16, (i / 32) * 16, 15, 15);
                }
            }
            this.viewSandbox.Refresh();
        }
        //-----------------------------------------------------------------------------
        // Controls
        private void viewPalette_MouseClick(object sender, MouseEventArgs e)
        {
            // Palette view clicked
            int clicked_color_num = (e.Y / 32) * 8 + (e.X / 32);
            // Update color table of current line
            int current_target_pcg = (currentPCG + currentLineX + (currentLineY / 8) * 32) % 256;
            if (e.Button == MouseButtons.Left)
            {
                // Foreground color has changed
                dataSource.SetColorTable(current_target_pcg, currentLineY % 8, clicked_color_num, true);
            }
            else if (e.Button == MouseButtons.Right)
            {
                // Background color has changed
                dataSource.SetColorTable(current_target_pcg, currentLineY % 8, clicked_color_num, false);
            }
            this.UpdateCurrentColorView();          // Update view of current color
            this.UpdatePCGEditView();               // PCG Editor view changes also
            this.UpdatePCGList(current_target_pcg); // PCG list view changes also
            this.UpdateSandbox(current_target_pcg); // Update sandbox view
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
                    dataSource.SetPalette(clicked_color_num, palette_win.R, palette_win.G, palette_win.B);
                    this.RefreshAllViews();     // Everything changes
                }
            }
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
            int current_target_pcg = (currentPCG + currentLineX + (currentLineY / 8) * 32) % 256;
            dataSource.SetColorTable(current_target_pcg, currentLineY % 8, val, true);
            this.UpdateCurrentColorView();          // Update view of current color
            this.UpdatePCGEditView();               // PCG Editor view changes also
            this.UpdatePCGList(current_target_pcg); // PCG list view changes also
            this.UpdateSandbox(current_target_pcg); // Update sandbox view
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
            int current_target_pcg = (currentPCG + currentLineX + (currentLineY / 8) * 32) % 256;
            dataSource.SetColorTable(current_target_pcg, currentLineY % 8, val, false);
            this.UpdateCurrentColorView();          // Update view of current color
            this.UpdatePCGEditView();               // PCG Editor view changes also
            this.UpdatePCGList(current_target_pcg); // PCG list view changes also
            this.UpdateSandbox(current_target_pcg); // Update sandbox view
            return 0;
        }
        private void checkTMS_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkTMS.Checked && !dataSource.IsTMS9918())
            {
                // Set windows color of each color code to TMS9918
                dataSource.SetPaletteToTMS9918();
                this.RefreshAllViews();     // Everything changes
            }
            else if (!this.checkTMS.Checked && dataSource.IsTMS9918())
            {
                // Set windows color of each color code to internal palette
                dataSource.SetPaletteToV9938();
                this.RefreshAllViews();     // Everything changes
            }
        }
        private void viewPCGEdit_MouseClick(object sender, MouseEventArgs e)
        {
            // PCG editor is clicked
            panelEditor.Focus();    // To catch CTRL+C and others in parent panel
            int clicked_line_x = e.X / 128;
            int clicked_line_y = e.Y / 16;
            if ((currentLineX != clicked_line_x) || (currentLineY != clicked_line_y))
            {
                // Current selected line has changed
                currentLineX = clicked_line_x;
                currentLineY = clicked_line_y;
                this.UpdatePCGEditView();               // Update editor view
                this.UpdateCurrentColorView();          // Update view of current color
            }
            else
            {
                // Update PCG pattern
                int current_target_pcg = (currentPCG + currentLineX + (currentLineY / 8) * 32) % 256;
                int y = currentLineY % 8;
                int x = (e.X / 16) % 8;
                int prev_pixel = dataSource.GetPCGPixel(current_target_pcg, y, x);
                if (prev_pixel == 0)
                {
                    dataSource.SetPCGPixel(current_target_pcg, y, x, 1);

                }
                else
                {
                    dataSource.SetPCGPixel(current_target_pcg, y, x, 0);
                }
                this.UpdatePCGEditView();               // PCG Editor view changes
                this.UpdatePCGList(current_target_pcg); // PCG list view changes also
                this.UpdateSandbox(current_target_pcg); // Update sandbox view
            }
        }
        private void contextEditor_MouseClick_copy(object sender, EventArgs e)
        {
            int pcg = (currentPCG + currentLineX + (currentLineY / 8) * 32) % 256;
            dataSource.CopyPCGLineToClip(pcg, currentLineY % 8);
        }
        private void contextEditor_MouseClick_paste(object sender, EventArgs e)
        {
            int dst_pcg = (currentPCG + currentLineX + (currentLineY / 8) * 32) % 256;
            dataSource.PastePCGLineFromClip(dst_pcg, currentLineY % 8);
            this.UpdatePCGEditView();               // PCG Editor view changes
            this.UpdatePCGList(dst_pcg); // PCG list view changes also
            this.UpdateSandbox(dst_pcg); // Update sandbox view
        }
        private void contextEditor_MouseClick_delete(object sender, EventArgs e)
        {
            int dst_pcg = (currentPCG + currentLineX + (currentLineY / 8) * 32) % 256;
            dataSource.ClearPCGLine(dst_pcg, currentLineY % 8);
            this.UpdatePCGEditView();               // PCG Editor view changes
            this.UpdatePCGList(dst_pcg); // PCG list view changes also
            this.UpdateSandbox(dst_pcg); // Update sandbox view
        }
        private void viewPCG_MouseDown(object sender, MouseEventArgs e)
        {
            panelPCG.Focus();   // Need this to catch CTRL+C and others
            int clicked_pcg = (e.Y / 16) * 32 + e.X / 16;
            if (clicked_pcg != currentPCG)
            {
                // Selected PCG has changed
                int previous_pcg = currentPCG;
                currentPCG = clicked_pcg;
                this.UpdatePCGList(previous_pcg);   // Erase previous selection
                this.UpdatePCGList(currentPCG);     // Redraw current selection
                this.UpdatePCGEditView();           // Update editor view
                this.UpdateCurrentColorView();      // Update view of current color
            }
            if (e.Button == MouseButtons.Left)
            {
                viewPCG.DoDragDrop(new DnDPCG(), DragDropEffects.Copy);
            }
        }
        private void panelPCG_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DnDPCG))) e.Effect = DragDropEffects.Copy;
            else e.Effect = DragDropEffects.None;
        }
        private void panelPCG_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DnDPCG)))
            {
                Point p = viewPCG.PointToClient(Cursor.Position);
                if (p.X > viewPCG.Width - 1) p.X = viewPCG.Width - 1;
                if (p.Y > viewPCG.Height - 1) p.X = viewPCG.Height - 1;
                int target_cell = ((p.Y / 16) * 32 + p.X / 16) % 256;
                dataSource.CopyPCG(currentPCG, target_cell);
                this.RefreshAllViews();
            }
        }
        private void contextPCGList_MouseClick_copy(object sender, EventArgs e)
        {
            dataSource.CopyPCGToClip(currentPCG);
        }
        private void contextPCGList_MouseClick_paste(object sender, EventArgs e)
        {
            dataSource.PastePCGFromClip(currentPCG);
            this.RefreshAllViews();
        }
        private void contextPCGList_MouseClick_delete(object sender, EventArgs e)
        {
            dataSource.ClearPCG(currentPCG);
            this.RefreshAllViews();
        }
        private void viewSandbox_MouseClick(object sender, MouseEventArgs e)
        {
            panelSandbox.Focus();   // Need this to catch CTRL+C and others
            int clicked_cell = (e.Y / 16) * 32 + e.X / 16;
            if (clicked_cell != currentSandbox)
            {
                // Selected sandbox cell have changed
                currentSandbox = clicked_cell;
                this.UpdateSandbox();
            }
        }
        private void contextSandbox_MouseClick_copy(object sender, EventArgs e)
        {
            ClipOneChrOfNametable clip = new ClipOneChrOfNametable();
            clip.pcgIndex = dataSource.GetNameTable(currentSandbox);
            Clipboard.Instance.Clip = clip;
        }
        private void contextSandbox_MouseClick_paste(object sender, EventArgs e)
        {
            dynamic clip = Clipboard.Instance.Clip;
            if(clip is ClipOneChrOfNametable)
            {
                int pcgIndex = clip.pcgIndex;
                dataSource.SetNameTable(currentSandbox, pcgIndex);
                this.UpdateSandbox();
            }
        }
        private void contextSandbox_MouseClick_delete(object sender, EventArgs e)
        {
            dataSource.SetNameTable(currentSandbox, 0);
            this.UpdateSandbox();
        }
        private void contextSandbox_MouseClick_paint(object sender, EventArgs e)
        {
            this.paintSandbox(currentSandbox % 32, currentSandbox / 32, currentPCG);
            this.UpdateSandbox();
        }
        private void panelSandbox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DnDPCG))) e.Effect = DragDropEffects.Copy;
            else e.Effect = DragDropEffects.None;
        }
        private void panelSandbox_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DnDPCG)))
            {
                Point p = viewSandbox.PointToClient(Cursor.Position);
                if (p.X > viewSandbox.Width - 1) p.X = viewSandbox.Width - 1;
                if (p.Y > viewSandbox.Height - 1) p.X = viewSandbox.Height - 1;
                int target_cell = ((p.Y / 16) * 32 + p.X / 16) % 768;
                dataSource.SetNameTable(target_cell, currentPCG);
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
        //---------------------------------------------------------------------
        // Utility
        private void paintSandbox(int x, int y, int val)
        {
            int pcg_to_paint = dataSource.GetNameTable(y * 32 + x);
            dataSource.SetNameTable(y * 32 + x, val);
            if (y > 0)
                if (dataSource.GetNameTable((y - 1) * 32 + x) == pcg_to_paint)
                    this.paintSandbox(x, y - 1, val);
            if (y < 23)
                if (dataSource.GetNameTable((y + 1) * 32 + x) == pcg_to_paint)
                    this.paintSandbox(x, y + 1, val);
            if (x > 0)
                if (dataSource.GetNameTable(y * 32 + x - 1) == pcg_to_paint)
                    this.paintSandbox(x - 1, y, val);
            if (x < 31)
                if (dataSource.GetNameTable(y * 32 + x + 1) == pcg_to_paint)
                    this.paintSandbox(x + 1, y, val);
        }
    }
    public class DnDPCG
    {
    }
}
