using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace _99x8Edit
{
    // Map editor window
    public partial class Map : Form
    {
        private readonly Machine dataSource;
        private readonly MainWindow mainWin;
        private TabOrder tabList = new TabOrder();
        private Bitmap bmpPCGList = new Bitmap(512, 128);       // PCG list view
        private Bitmap bmpMapPatterns = new Bitmap(512, 512);   // Map pattern view
        private Bitmap bmpMap = new Bitmap(512, 384);           // Map view
        private Selection curPCG = new Selection(16, 16);       // Selected character
        private Selection curPtn = new Selection(32, 32);       // Selected tile pattern
        private Selection curCellInPtn = new Selection(16, 16); // Selected cell in one tile pattern
        private Point curMapOrg;                                // Coordinate of left top corner of map
        private Selection curMap = new Selection(32, 32);       // Selected map cell 0-15
        String currentFile = "";
        public String CurrentFile
        {
            set { currentFile = value; }
        }
        // For internal drag control
        private class DnDPattern { }
        private class DnDMapPCG { }
        private class DnDMap { }
        private class DnDPtnSel { }
        //------------------------------------------------------------------------------
        // Initialize
        public Map(Machine src, MainWindow parent)
        {
            InitializeComponent();
            // Set corresponding data and owner window
            dataSource = src;
            mainWin = parent;
            // Tab order
            tabList.Add(panelPCG, curPCG);
            tabList.Add(panelPtns, curPtn);
            tabList.Add(panelMap, curMap);
            // Initialize controls
            viewPCG.Image = bmpPCGList;
            viewPatterns.Image = bmpMapPatterns;
            viewMap.Image = bmpMap;
            btnLeft.Enabled = false;
            btnRight.Enabled = (dataSource.MapWidth > 16);
            btnUp.Enabled = false;
            btnDown.Enabled = (dataSource.MapHeight > 12);
            // Refresh all views
            this.RefreshAllViews();
            // Menu bar
            toolStripFileLoad.Click += new EventHandler(menu_fileLoad);
            toolStripFileSave.Click += new EventHandler(menu_fileSave);
            toolStripFileSaveAs.Click += new EventHandler(menu_fileSaveAs);
            toolStripFileImport.Click += new EventHandler(menu_fileImport);
            toolStripFileExport.Click += new EventHandler(menu_fileExport);
            toolStripFileLoadMap.Click += new EventHandler(menu_fileLoadMap);
            toolStripFileSaveMap.Click += new EventHandler(menu_fileSaveMap);
            toolStripEditUndo.Click += new EventHandler(menu_editUndo);
            toolStripEditRedo.Click += new EventHandler(menu_editRedo);
            // Context menu
            toolStripPatternCopy.Click += new EventHandler(contextPatterns_copy);
            toolStripPatternPaste.Click += new EventHandler(contextPatterns_paste);
            toolStripPatternCopyDown.Click += new EventHandler(contextPatterns_copyDown);
            toolStripPatternCopyRight.Click += new EventHandler(contextPatterns_copyRight);
            toolStripMapCopy.Click += new EventHandler(contextMap_copy);
            toolStripMapPaste.Click += new EventHandler(contextMap_paste);
            toolStripMapDel.Click += new EventHandler(contextMap_del);
            toolStripMapPaint.Click += new EventHandler(contextMap_paint);
            toolStripMapCopyDown.Click += new EventHandler(contextMap_copyDown);
            toolStripMapCopyRight.Click += new EventHandler(contextMap_copyRight);
        }
        //------------------------------------------------------------------------------
        // Override
        protected override bool ProcessTabKey(bool forward)
        {
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
            this.UpdatePCGList();           // PCG view
            this.UpdateMapPatterns();      // Map patterns
            this.UpdateMap();              // Map
        }
        private void UpdatePCGList(bool refresh = true)
        {
            // Update all PCG list
            Utility.DrawTransparent(bmpPCGList);
            Graphics g = Graphics.FromImage(bmpPCGList);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            g.FillRectangle(new SolidBrush(Color.Black), 0, 0, bmpPCGList.Width, bmpPCGList.Height);
            for (int i = 0; i < 256; ++i)
            {
                g.DrawImage(dataSource.GetBitmapOfPCG(i), (i % 32) * 16, (i / 32) * 16, 17, 17);
            }
            // CRT Filter
            if (chkCRT.Checked)
            {
                Filter.Create(Filter.Type.CRT).Process(bmpPCGList);
            }
            // Selection
            Utility.DrawSelection(g, curPCG.X * 16, curPCG.Y * 16, 15, 15, panelPCG.Focused);
            if (refresh) viewPCG.Refresh();
        }
        private void UpdateMapPatterns(bool refresh = true)
        {
            Graphics g = Graphics.FromImage(bmpMapPatterns);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            g.FillRectangle(new SolidBrush(Color.Black), 0, 0, bmpMapPatterns.Width, bmpMapPatterns.Height);
            for (int i = 0; i < 256; ++i)
            {
                for (int j = 0; j < 4; ++j)
                {
                    int one_pcg = dataSource.GetPCGInPattern(i, j);
                    Bitmap one_bmp = dataSource.GetBitmapOfPCG(one_pcg);
                    int x = (i % 16) * 32 + (j % 2) * 16;
                    int y = (i / 16) * 32 + (j / 2) * 16;
                    g.DrawImage(one_bmp, x, y, 17, 17);
                }
            }
            // CRT Filter
            if (chkCRT.Checked)
            {
                Filter.Create(Filter.Type.CRT).Process(bmpMapPatterns);
            }
            // Selection
            Utility.DrawSelection(g, curPtn, panelPtns.Focused);
            // Selection, cell in one pattern
            int cx = curPtn.X * 32;
            int cy = curPtn.Y * 32;
            g.DrawRectangle(Pens.Yellow,
                            cx + curCellInPtn.X * 16 + (1 - curCellInPtn.X),
                            cy + curCellInPtn.Y * 16 + (1 - curCellInPtn.Y), 14, 14);
            if (refresh) viewPatterns.Refresh();
        }
        private void UpdateMap(bool refresh = true)
        {
            Graphics g = Graphics.FromImage(bmpMap);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            g.FillRectangle(new SolidBrush(Color.Black), 0, 0, bmpMap.Width, bmpMap.Height);
            for (int i = 0; i < 12; ++i)
            {
                for (int j = 0; j < 16; ++j)
                {
                    this.DrawOneMapCell(g, j, i);
                }
            }
            // CRT Filter
            if (chkCRT.Checked)
            {
                Filter.Create(Filter.Type.CRT).Process(bmpMap);
            }
            // Selection
            Utility.DrawSelection(g, curMap, panelMap.Focused);
            if (refresh) viewMap.Refresh();
            // Map size may be changed by loading, undo, etc
            if ((curMapOrg.X + 16 > dataSource.MapWidth) || (curMapOrg.Y + 12 > dataSource.MapHeight))
            {
                txtMapX.Text = (curMapOrg.X = 0).ToString();
                txtMapY.Text = (curMapOrg.Y = 0).ToString();
            }
            // Controls corresponding to map
            txtMapX.Text = curMapOrg.X.ToString();
            txtMapY.Text = curMapOrg.Y.ToString();
            btnLeft.Enabled = ((curMapOrg.X > 0) && (dataSource.MapWidth > 16));
            btnRight.Enabled = (curMapOrg.X < dataSource.MapWidth - 16);
            btnUp.Enabled = ((curMapOrg.Y > 0) && (dataSource.MapHeight > 12));
            btnDown.Enabled = (curMapOrg.Y < dataSource.MapHeight - 12);
        }
        private void DrawOneMapCell(Graphics g, int x, int y)
        {
            // Redraw map view
            int map_x = curMapOrg.X + x;
            int map_y = curMapOrg.Y + y;
            int map_ptn = dataSource.GetMapData(map_x, map_y);
            for(int i = 0; i < 2; ++i)
            {
                for(int j = 0; j < 2; ++j)
                {
                    int map_pcg = dataSource.GetPCGInPattern(map_ptn, i * 2 + j);
                    Bitmap img = dataSource.GetBitmapOfPCG(map_pcg);
                    g.DrawImage(img, x * 32 + j * 16, y * 32 + i * 16, 17, 17);
                }
            }
        }
        //-----------------------------------------------------------------
        // Controls
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
                    curPCG.X = clicked_pcg_x;
                    curPCG.Y = clicked_pcg_y;
                    this.UpdatePCGList();
                }
                else
                {
                    viewPCG.DoDragDrop(new DnDMapPCG(), DragDropEffects.Copy);
                }
            }
        }
        private void contextPatterns_copy(object sender, EventArgs e)
        {
            ClipMapPtn clip = new ClipMapPtn();
            clip.index = curPtn.Y * 16 + curPtn.X;
            Rectangle r = curPtn.Selected;
            for (int i = r.Y; i < r.Y + r.Height; ++i)
            {
                List<byte[]> l = new List<byte[]>();
                for (int j = r.X; j < r.X + r.Width; ++j)
                {
                    l.Add(dataSource.GetPattern(i * 16 + j));
                }
                clip.ptns.Add(l);
            }
            ClipboardWrapper.SetData(clip);
        }
        private void contextPatterns_paste(object sender, EventArgs e)
        {
            dynamic clip = ClipboardWrapper.GetData();
            if (clip is ClipMapPtn)
            {
                MementoCaretaker.Instance.Push();
                for (int i = 0; (i < clip.ptns.Count) && (curPtn.Y + i < 16); ++i)
                {
                    List<byte[]> l = clip.ptns[i];
                    for (int j = 0; (j < l.Count) && (curPtn.X + j < 16); ++j)
                    {
                        dataSource.SetPattern((curPtn.Y + i) * 16 + (curPtn.X + j), l[j], false);
                    }
                }
                this.UpdateMapPatterns();
                this.UpdateMap();
            }
        }
        private void contextPatterns_copyDown(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            Rectangle r = curPtn.Selected;
            for (int i = r.Y + 1; i < r.Y + r.Height; ++i)
            {
                for (int j = r.X; j < r.X + r.Width; ++j)
                {
                    byte[] src = dataSource.GetPattern(r.Y * 16 + j);
                    dataSource.SetPattern(i * 16 + j, src, false);
                }
            }
            this.RefreshAllViews();
        }
        private void contextPatterns_copyRight(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            Rectangle r = curPtn.Selected;
            for (int i = r.Y; i < r.Y + r.Height; ++i)
            {
                for (int j = r.X + 1; j < r.X + r.Width; ++j)
                {
                    byte[] src = dataSource.GetPattern(i * 16 + r.X);
                    dataSource.SetPattern(i * 16 + j, src, false);
                }
            }
            this.RefreshAllViews();
        }
        private void viewPatterns_MouseDown(object sender, MouseEventArgs e)
        {
            panelPtns.Focus();  // Key events are handled by parent panel
            if (e.Button == MouseButtons.Left)
            {
                // Tile pattern selected
                int selected_ptn_x = e.X / 32;
                int selected_ptn_y = e.Y / 32;
                int sel_cell_x = (e.X / 16) % 2;
                int sel_cell_y = (e.Y / 16) % 2;
                int selected_pattern_num = selected_ptn_x + selected_ptn_y * 16;
                int current_pattern_num = curPtn.X + curPtn.Y * 16;
                if ((sel_cell_x != curCellInPtn.X) || (sel_cell_y != curCellInPtn.Y))
                {
                    curCellInPtn.X = sel_cell_x;
                    curCellInPtn.Y = sel_cell_y;
                }
                if (selected_pattern_num != current_pattern_num)
                {
                    if (Control.ModifierKeys != Keys.Shift)
                    {
                        curPtn.ToX = selected_ptn_x;
                        curPtn.ToY = selected_ptn_y;
                    }
                    else
                    {
                        curPtn.X = selected_ptn_x;
                        curPtn.Y = selected_ptn_y;
                        curPtn.ResetSelection();        // New selection
                    }
                    this.UpdateMapPatterns();   // refresh before dragging
                    // Start multiple selections
                    viewPatterns.DoDragDrop(new DnDPtnSel(), DragDropEffects.Copy);
                }
                else
                {
                    this.UpdateMapPatterns();
                    // Start dragging when current pattern was already selected
                    viewPatterns.DoDragDrop(new DnDPattern(), DragDropEffects.Copy);
                }
            }
        }
        private void panelPatterns_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DnDPattern)))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else if (e.Data.GetDataPresent(typeof(DnDMapPCG)))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else if (e.Data.GetDataPresent(typeof(DnDPtnSel)))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else e.Effect = DragDropEffects.None;
        }
        private void panelPatterns_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DnDPtnSel)))
            {
                Point p = viewPatterns.PointToClient(Cursor.Position);
                curPtn.X = Math.Min(p.X / 32, 15);
                curPtn.Y = Math.Min(p.Y / 32, 15);
                this.UpdateMapPatterns();
            }
        }
        private void panelPatterns_DragDrop(object sender, DragEventArgs e)
        {
            Point p = viewPatterns.PointToClient(Cursor.Position);
            if (p.X > viewPatterns.Width - 1) p.X = viewPatterns.Width - 1;
            if (p.Y > viewPatterns.Height - 1) p.X = viewPatterns.Height - 1;
            if (e.Data.GetDataPresent(typeof(DnDPattern)))
            {
                dataSource.CopyMapPattern(curPtn.Y * 16 + curPtn.X, (p.Y / 32) * 16 + (p.X / 32), true);
                this.UpdateMapPatterns();
                this.UpdateMap();
            }
            else if (e.Data.GetDataPresent(typeof(DnDMapPCG)))
            {
                int target_ptn = p.X / 32 + (p.Y / 32) * 16;
                int target_cell = (p.X / 16) % 2 + ((p.Y / 16) % 2) * 2;
                dataSource.SetPCGInPattern(target_ptn, target_cell, curPCG.Y * 32 + curPCG.X, true);
                this.UpdateMapPatterns(); 
                this.UpdateMap();
            }
        }
        private void viewMap_MouseDown(object sender, MouseEventArgs e)
        {
            // Selected map cell has changed
            panelMap.Focus();   // CTRL+C and others will be cathced at parent panel
            if (e.Button == MouseButtons.Left)
            {
                int selected_x = e.X / 32;
                int selected_y = e.Y / 32;
                int previous_x = curMap.X;
                int previous_y = curMap.Y;
                if ((selected_x != previous_x) || (selected_y != previous_y))
                {
                    if (Control.ModifierKeys == Keys.Shift)
                    {
                        curMap.ToX = selected_x;
                        curMap.ToY = selected_y;
                    }
                    else
                    {
                        curMap.X = selected_x;
                        curMap.Y = selected_y;
                        curMap.ResetSelection();    // New selection
                    }
                    this.UpdateMap();
                }
                viewPCG.DoDragDrop(new DnDMap(), DragDropEffects.Copy);
            }
        }
        private void contextMap_copy(object sender, EventArgs e)
        {
            ClipMapCell clip = new ClipMapCell();
            Rectangle r = curMap.Selected;
            for(int i = r.Y; i < r.Y + r.Height; ++i)
            {
                List<int> l = new List<int>();
                for(int j = r.X; j < r.X + r.Width; ++j)
                {
                    l.Add(dataSource.GetMapData(curMapOrg.X + j, curMapOrg.Y + i));
                }
                clip.ptnID.Add(l);
            }
            ClipboardWrapper.SetData(clip);
        }
        private void contextMap_paste(object sender, EventArgs e)
        {
            dynamic clip = ClipboardWrapper.GetData();
            if (clip is ClipMapPtn)
            {
                dataSource.SetMapData(curMapOrg.X + curMap.X,
                                      curMapOrg.Y + curMap.Y,
                                      clip.index, true);
                this.UpdateMap();
            }
            else if (clip is ClipMapCell)
            {
                MementoCaretaker.Instance.Push();
                for (int i = 0; (i < clip.ptnID.Count) && (curMap.Y + i < 12); ++i)
                {
                    List<int> l = clip.ptnID[i];
                    for (int j = 0; (j < l.Count) && (curMap.X + j < 16); ++j)
                    {
                        dataSource.SetMapData(curMap.X + j, curMap.Y + i, l[j], false);
                    }
                }
                this.UpdateMap();
            }
        }
        private void contextMap_del(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            Rectangle r = curMap.Selected;
            for (int i = r.Y; (i < r.Y + r.Height) && (i < 12); ++i)
            {
                for (int j = r.X; (j < r.X + r.Width) && (j < 16); ++j)
                {
                    dataSource.SetMapData(curMapOrg.X + j, curMapOrg.Y + i, 0, false);
                }
            }
            this.UpdateMap();
        }
        private void contextMap_paint(object sender, EventArgs e)
        {
            int selected_ptn_num = curPtn.X + curPtn.Y * 16;
            MementoCaretaker.Instance.Push();   // For undo action
            this.PaintMap(curMapOrg.X + curMap.X, curMapOrg.Y + curMap.Y, selected_ptn_num);
            this.UpdateMap();
        }
        private void contextMap_copyDown(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            Rectangle r = curMap.Selected;
            for (int i = r.Y + 1; (i < r.Y + r.Height) && (i < 12); ++i)
            {
                for (int j = r.X; (j < r.X + r.Width) && (j < 16); ++j)
                {
                    int src = dataSource.GetMapData(curMapOrg.X + j, curMapOrg.Y + r.Y);
                    dataSource.SetMapData(curMapOrg.X + j, curMapOrg.Y + i, src, false);
                }
            }
            this.UpdateMap();
        }
        private void contextMap_copyRight(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            Rectangle r = curMap.Selected;
            for (int i = r.Y; (i < r.Y + r.Height) && (i < 12); ++i)
            {
                for (int j = r.X + 1; (j < r.X + r.Width) && (j < 16); ++j)
                {
                    int src = dataSource.GetMapData(curMapOrg.X + r.X, curMapOrg.Y + i);
                    dataSource.SetMapData(curMapOrg.X + j, curMapOrg.Y + i, src, false);
                }
            }
            this.UpdateMap();
        }
        private void panelMap_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DnDPattern)))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else if (e.Data.GetDataPresent(typeof(DnDMap)))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else e.Effect = DragDropEffects.None;
        }
        private void panelMap_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DnDMap)))
            {
                Point p = viewMap.PointToClient(Cursor.Position);
                curMap.X = Math.Min(p.X / 32, 15);
                curMap.Y = Math.Min(p.Y / 32, 11);
                this.UpdateMap();
            }
        }
        private void panelMap_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DnDPattern)))
            {
                Point p = viewMap.PointToClient(Cursor.Position);
                dataSource.SetMapData(Math.Min(p.X / 32, 15), Math.Min(p.Y / 32, 11),
                                      curPtn.Y * 16 + curPtn.X, true);
                this.UpdateMap();
            }
        }
        private void btnLeft_Click(object sender, EventArgs e)
        {
            curMapOrg.X = Math.Max(curMapOrg.X - 16, 0);
            this.UpdateMap();
        }
        private void btnRight_Click(object sender, EventArgs e)
        {
            curMapOrg.X = Math.Min(curMapOrg.X + 16, dataSource.MapWidth - 16);
            this.UpdateMap();
        }
        private void btnUp_Click(object sender, EventArgs e)
        {
            curMapOrg.Y = Math.Max(curMapOrg.Y - 12, 0);
            this.UpdateMap();
        }
        private void btnDown_Click(object sender, EventArgs e)
        {
            curMapOrg.Y = Math.Min(curMapOrg.Y + 12, dataSource.MapHeight - 12);
            this.UpdateMap();
        }
        private void btnMapSize_Click(object sender, EventArgs e)
        {
            MapSize dlg = new MapSize(dataSource.MapWidth, dataSource.MapHeight);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                dataSource.MapWidth = dlg.MapWidth;
                dataSource.MapHeight = dlg.MapHeight;
                txtMapX.Text = (curMapOrg.X = 0).ToString();
                txtMapY.Text = (curMapOrg.Y = 0).ToString();
                btnLeft.Enabled = false;
                btnRight.Enabled = (dataSource.MapWidth > 16);
                btnUp.Enabled = false;
                btnDown.Enabled = (dataSource.MapHeight > 12);
                this.RefreshAllViews();     // Everything changes
            }
        }
        private void Map_Activated(object sender, EventArgs e)
        {
            // Refresh everything when form has been activated, since it may be edited
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
            // Currently nothing
        }
        private void menu_fileExport(object sender, EventArgs e)
        {
            mainWin.ExportMap(sender, e);
        }
        private void menu_fileLoadMap(object sender, EventArgs e)
        {
            if (Utility.LoadDialogAndLoad(currentFile,
                                          "Map File(*.map)|*.map",
                                          "Load map settings",
                                          dataSource.LoadMap,
                                          true,     // Push memento
                                          out _))
            {
                this.RefreshAllViews();
            }
        }
        private void menu_fileSaveMap(object sender, EventArgs e)
        {
            Utility.SaveDialogAndSave(currentFile,
                                      "Map File(*.map)|*.map",
                                      "Save map settings",
                                      dataSource.SaveMap,
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
        //-------------------------------------------------------------------
        // Utility
        private void PaintMap(int x, int y, int val)
        {
            int pattern_to_paint = dataSource.GetMapData(x, y);
            if (pattern_to_paint == val) return;
            dataSource.SetMapData(x, y, val, false);
            if (y > 0)
                if (dataSource.GetMapData(x, y - 1) == pattern_to_paint)
                    this.PaintMap(x, y - 1, val);
            if (y < dataSource.MapWidth - 1)
                if (dataSource.GetMapData(x, y + 1) == pattern_to_paint)
                    this.PaintMap(x, y + 1, val);
            if (x > 0)
                if (dataSource.GetMapData(x - 1, y) == pattern_to_paint)
                    this.PaintMap(x - 1, y, val);
            if (x < dataSource.MapHeight - 1)
                if (dataSource.GetMapData(x + 1, y) == pattern_to_paint)
                    this.PaintMap(x + 1, y, val);
        }
    }
}

