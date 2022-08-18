using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace _99x8Edit
{
    // Map editor window
    public partial class MapEditor : Form
    {
        private readonly Machine dataSource;
        private readonly MainWindow mainWin;
        private TabOrder tabList = new TabOrder();
        private Point curMapOrg;       // Coordinate of left top corner of map
        internal String CurrentFile { get; set; }
        // For internal drag control
        private class DnDPattern { }
        private class DnDMapPCG { }
        //------------------------------------------------------------------------------
        // Initialize
        public MapEditor(Machine src, MainWindow parent)
        {
            InitializeComponent();
            // Set corresponding data and owner window
            dataSource = src;
            mainWin = parent;
            // Tab order for the customed control
            tabList.Add(viewPCG, viewPCG.Selector);
            tabList.Add(viewPtn, viewPtn.Selector);
            tabList.Add(viewMap, viewMap.Selector);
            // Initialize controls
            btnLeft.Enabled = false;
            btnRight.Enabled = (dataSource.MapWidth > viewMap.SelectionColNum);
            btnUp.Enabled = false;
            btnDown.Enabled = (dataSource.MapHeight > viewMap.SelectionRowNum);
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
            toolStripPatternCopy.Click += new EventHandler(contextPtn_copy);
            toolStripPatternPaste.Click += new EventHandler(contextPtn_paste);
            toolStripPatternCopyDown.Click += new EventHandler(contextPtn_copyDown);
            toolStripPatternCopyRight.Click += new EventHandler(contextPtn_copyRight);
            toolStripMapCopy.Click += new EventHandler(contextMap_copy);
            toolStripMapPaste.Click += new EventHandler(contextMap_paste);
            toolStripMapDel.Click += new EventHandler(contextMap_del);
            toolStripMapPaint.Click += new EventHandler(contextMap_paint);
            toolStripMapCopyDown.Click += new EventHandler(contextMap_copyDown);
            toolStripMapCopyRight.Click += new EventHandler(contextMap_copyRight);
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
            this.UpdatePCGList(refresh: false);          // PCG view
            this.UpdateMapPatterns(refresh: false);      // Map patterns
            this.UpdateMap(refresh: false);              // Map
            this.Refresh();
        }
        private void UpdatePCGList(bool refresh)
        {
            // Update all PCG list
            for (int y = 0; y < viewPCG.RowNum; ++y)
            {
                for (int x = 0; x < viewPCG.ColumnNum; ++x)
                {
                    int index = viewPCG.IndexOf(x, y);
                    viewPCG.SetImage(dataSource.GetBitmapOfPCG(index), x, y);
                }
            }
            // CRT Filter
            viewPCG.Filter = (chkCRT.Checked) ? Filter.Create(Filter.Type.CRT) : null;
            if (refresh) viewPCG.Refresh();
        }
        private void UpdateMapPatterns(bool refresh)
        {
            // Draw one map pattern
            Action<int, int> draw_one_ptn = (col, row) =>
            {
                for(int i = 0; i < 2; ++i)
                {
                    for(int j = 0; j < 2; ++j)
                    {
                        int pcg = dataSource.GetPCGInPattern(row * viewPtn.SelectionColNum + col, i * 2 + j);
                        Bitmap bmp = dataSource.GetBitmapOfPCG(pcg);
                        viewPtn.SetImage(bmp, col * 2 + j, row * 2 + i);
                    }
                }
            };
            for(int y = 0; y < viewPtn.SelectionRowNum; ++y)
            {
                for(int x = 0; x < viewPtn.SelectionColNum; ++x)
                {
                    draw_one_ptn(x, y);
                }
            }
            // CRT Filter
            viewPtn.Filter = (chkCRT.Checked) ? Filter.Create(Filter.Type.CRT) : null;
            if (refresh) viewPtn.Refresh();
        }
        private void UpdateMap(bool refresh)
        {
            // Map size may have been changed
            if ((curMapOrg.X + 16 > dataSource.MapWidth) || (curMapOrg.Y + 12 > dataSource.MapHeight))
            {
                txtMapX.Text = (curMapOrg.X = 0).ToString();
                txtMapY.Text = (curMapOrg.Y = 0).ToString();
            }
            // Draw cells in the map
            Action<int, int> one_map_pattern = (x, y) =>
            {
                // Draw one map pattern in map
                int map_x = curMapOrg.X + x;
                int map_y = curMapOrg.Y + y;
                int map_ptn = dataSource.GetMapData(map_x, map_y);
                for (int i = 0; i < 2; ++i)
                {
                    for (int j = 0; j < 2; ++j)
                    {
                        int map_pcg = dataSource.GetPCGInPattern(map_ptn, i * 2 + j);
                        Bitmap img = dataSource.GetBitmapOfPCG(map_pcg);
                        viewMap.SetImage(img, x * 2 + j, y * 2 + i);
                    }
                }
            };
            for (int i = 0; i < viewMap.SelectionRowNum; ++i)
            {
                for (int j = 0; j < viewMap.SelectionColNum; ++j)
                {
                    one_map_pattern(j, i);
                }
            }
            // CRT Filter
            viewMap.Filter = (chkCRT.Checked) ? Filter.Create(Filter.Type.CRT) : null;
            if (refresh) viewMap.Refresh();
            // Controls corresponding to map
            txtMapX.Text = curMapOrg.X.ToString();
            txtMapY.Text = curMapOrg.Y.ToString();
            btnLeft.Enabled = ((curMapOrg.X > 0) && (dataSource.MapWidth > viewMap.SelectionColNum));
            btnRight.Enabled = (curMapOrg.X < dataSource.MapWidth - viewMap.SelectionColNum);
            btnUp.Enabled = ((curMapOrg.Y > 0) && (dataSource.MapHeight > viewMap.SelectionRowNum));
            btnDown.Enabled = (curMapOrg.Y < dataSource.MapHeight - viewMap.SelectionRowNum);
        }
        //-----------------------------------------------------------------
        // Controls
        //---------------------------------------------
        // PCG list
        private void viewPCG_CellDragStart(object sender, EventArgs e)
        {
            viewPCG.DoDragDrop(new DnDMapPCG(), DragDropEffects.Copy);
        }
        //---------------------------------------------
        // Patterns
        private void viewPtn_CellDragStart(object sender, EventArgs e)
        {
            viewPtn.DoDragDrop(new DnDPattern(), DragDropEffects.Copy);
        }
        private void contextPtn_copy(object sender, EventArgs e)
        {
            ClipMapPtn clip = new ClipMapPtn();
            clip.index = viewPtn.Index;
            Rectangle r = viewPtn.SelectedRect;
            for (int i = r.Y; i < r.Y + r.Height; ++i)
            {
                List<byte[]> l = new List<byte[]>();
                for (int j = r.X; j < r.X + r.Width; ++j)
                {
                    // For each selected patterns
                    l.Add(dataSource.GetPattern(i * 16 + j));
                }
                clip.ptns.Add(l);
            }
            ClipboardWrapper.SetData(clip);
        }
        private void contextPtn_paste(object sender, EventArgs e)
        {
            dynamic clip = ClipboardWrapper.GetData();
            if (clip is ClipMapPtn)
            {
                MementoCaretaker.Instance.Push();
                Action<int, int, int, int> callback = (col, row, colcnt, rowcnt) =>
                {
                    // For each copied patterns
                    int target = viewPtn.IndexOf(col, row);
                    dataSource.SetPattern(target, clip.ptns[rowcnt][colcnt], push: false);
                };
                viewPtn.ForEachSelection(viewPtn.X, viewPtn.Y,
                                         clip?.ptns?[0]?.Count, clip.ptns?.Count, callback);
                this.UpdateMapPatterns(refresh: true);
                this.UpdateMap(refresh: true);
            }
        }
        private void contextPtn_copyDown(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            Rectangle r = viewPtn.SelectedRect;
            Action<int, int> callback = (col, row) =>
            {
                // For each selected patterns
                int src = viewPtn.IndexOf(col, r.Y);
                int dst = viewPtn.IndexOf(col, row);
                byte[] dat = dataSource.GetPattern(src);
                dataSource.SetPattern(dst, dat, push: false);
            };
            viewPtn.ForEachSelection(r.X, r.Y + 1, r.Width, r.Height - 1, callback);
            this.RefreshAllViews();
        }
        private void contextPtn_copyRight(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            Rectangle r = viewPtn.SelectedRect;
            Action<int, int> callback = (col, row) =>
            {
                // For each selected patterns
                int src = viewPtn.IndexOf(r.X, row);
                int dst = viewPtn.IndexOf(col, row);
                byte[] dat = dataSource.GetPattern(src);
                dataSource.SetPattern(dst, dat, push: false);
            };
            viewPtn.ForEachSelection(r.X + 1, r.Y, r.Width - 1, r.Height, callback);
            this.RefreshAllViews();
        }
        private void panelPtn_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DnDPattern)))
            {
                // Dragging map pattern
                e.Effect = DragDropEffects.Copy;
            }
            else if (e.Data.GetDataPresent(typeof(DnDMapPCG)))
            {
                // Dragged from PCG list
                e.Effect = DragDropEffects.Copy;
            }
        }
        private void panelPtn_DragDrop(object sender, DragEventArgs e)
        {
            (int col, int row) = viewPtn.ScreenCoodinateToSelection(Cursor.Position);
            int target = viewPtn.IndexOf(col, row);
            if (e.Data.GetDataPresent(typeof(DnDPattern)))
            {
                // Other map pattern has been dropped
                dataSource.CopyMapPattern(viewPtn.Index, target, push: true);
                this.UpdateMapPatterns(refresh: true);
                this.UpdateMap(refresh: true);
            }
            else if (e.Data.GetDataPresent(typeof(DnDMapPCG)))
            {
                // Character has been dropped from PCG list
                (int col_sub, int row_sub) = viewPtn.ScreenCoodinateToCell(Cursor.Position);
                int target_no = col_sub % 2 + (row_sub % 2) * 2;
                dataSource.SetPCGInPattern(target, target_no, viewPCG.Index, push: true);
                this.UpdateMapPatterns(refresh: true); 
                this.UpdateMap(refresh: true);
            }
        }
        //---------------------------------------------
        // Map
        private void viewMap_MatrixOnScroll(object sender, MatrixControl.ScrollEventArgs e)
        {
            // Scrolled request from control
            if((e.DY < 0) && (curMapOrg.Y + e.DY >= 0))
            {
                curMapOrg.Y += e.DY;
                this.UpdateMap(refresh: true);
            }
            if((e.DY > 0) && (curMapOrg.Y + e.DY <= dataSource.MapHeight - viewMap.SelectionRowNum))
            {
                curMapOrg.Y += e.DY;
                this.UpdateMap(refresh: true);
            }
            if ((e.DX < 0) && (curMapOrg.X + e.DX >= 0))
            {
                curMapOrg.X += e.DX;
                this.UpdateMap(refresh: true);
            }
            if ((e.DX > 0) && (curMapOrg.X + e.DX <= dataSource.MapWidth - viewMap.SelectionColNum))
            {
                curMapOrg.X += e.DX;
                this.UpdateMap(refresh: true);
            }
        }
        private void contextMap_copy(object sender, EventArgs e)
        {
            ClipMapCell clip = new ClipMapCell();
            Rectangle r = viewMap.SelectedRect;
            for(int i = r.Y; i < r.Y + r.Height; ++i)
            {
                List<int> l = new List<int>();
                for(int j = r.X; j < r.X + r.Width; ++j)
                {
                    // For each selected map tiles
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
                // One map pattern has been pasted
                dataSource.SetMapData(curMapOrg.X + viewMap.X,
                                      curMapOrg.Y + viewMap.Y,
                                      clip.index, push: true);
                this.UpdateMap(refresh: true);
            }
            else if (clip is ClipMapCell)
            {
                // Map data has been pasted
                MementoCaretaker.Instance.Push();
                Action<int, int, int, int> callback = (col, row, colcnt, rowcnt) =>
                {
                    // For each map tiles
                    dataSource.SetMapData(curMapOrg.X + col,
                                          curMapOrg.Y + row,
                                          clip.ptnID[rowcnt][colcnt], push: false);
                };
                viewMap.ForEachSelection(viewMap.X, viewMap.Y,
                                         clip.ptnID?[0]?.Count, clip.ptnID?.Count, callback);
                this.UpdateMap(refresh: true);
            }
        }
        private void contextMap_del(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            Rectangle r = viewMap.SelectedRect;
            Action<int, int> callback = (col, row) =>
            {
                // Delete each map patterns
                dataSource.SetMapData(curMapOrg.X + col, curMapOrg.Y + row, 0, push: false);
            };
            viewMap.ForEachSelection(r.X, r.Y, r.Width, r.Height, callback);
            this.UpdateMap(refresh: true);
        }
        private void contextMap_paint(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            this.PaintMap(curMapOrg.X + viewMap.X, curMapOrg.Y + viewMap.Y, viewPtn.Index);
            this.UpdateMap(refresh: true);
        }
        private void contextMap_copyDown(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            Rectangle r = viewMap.SelectedRect;
            Action<int, int> callback = (col, row) =>
            {
                // For each selected map tiles
                int src = dataSource.GetMapData(curMapOrg.X + col, curMapOrg.Y + r.Y);
                dataSource.SetMapData(curMapOrg.X + col, curMapOrg.Y + row, src, push: false);
            };
            viewMap.ForEachSelection(r.X, r.Y + 1, r.Width, r.Height - 1, callback);
            this.UpdateMap(refresh: true);
        }
        private void contextMap_copyRight(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            Rectangle r = viewMap.SelectedRect;
            Action<int, int> callback = (col, row) =>
            {
                // For each selected map tiles
                int src = dataSource.GetMapData(curMapOrg.X + r.X, curMapOrg.Y + row);
                dataSource.SetMapData(curMapOrg.X + col, curMapOrg.Y + row, src, push: false);
            };
            viewMap.ForEachSelection(r.X + 1, r.Y, r.Width - 1, r.Height, callback);
            this.UpdateMap(refresh: true);
        }
        private void panelMap_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DnDPattern)))
            {
                // Map pattern has been dragged into
                e.Effect = DragDropEffects.Copy;
            }
        }
        private void panelMap_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DnDPattern)))
            {
                // Map pattern has been dropped
                Point p = viewMap.PointToClient(Cursor.Position);
                dataSource.SetMapData(Math.Min(p.X / 32, 15), Math.Min(p.Y / 32, 11),
                                      viewPtn.Index, push: true);
                this.UpdateMap(refresh: true);
            }
        }
        //---------------------------------------------
        // Misc
        private void btnLeft_Click(object sender, EventArgs e)
        {
            // Scrolling map leftward
            curMapOrg.X = Math.Max(curMapOrg.X - 16, 0);
            this.UpdateMap(refresh: true);
        }
        private void btnRight_Click(object sender, EventArgs e)
        {
            // Scrolling map rightward
            curMapOrg.X = Math.Min(curMapOrg.X + 16, dataSource.MapWidth - 16);
            this.UpdateMap(refresh: true);
        }
        private void btnUp_Click(object sender, EventArgs e)
        {
            // Scrolling map upward
            curMapOrg.Y = Math.Max(curMapOrg.Y - 12, 0);
            this.UpdateMap(refresh: true);
        }
        private void btnDown_Click(object sender, EventArgs e)
        {
            // Scrolling map downward
            curMapOrg.Y = Math.Min(curMapOrg.Y + 12, dataSource.MapHeight - 12);
            this.UpdateMap(refresh: true);
        }
        private void btnMapSize_Click(object sender, EventArgs e)
        {
            // Show the map size window
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
            // Redraw the views according to data at this timing
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
            if (Utility.LoadDialogAndLoad(CurrentFile,
                                          "Map File(*.map)|*.map",
                                          "Load map settings",
                                          dataSource.LoadMap,
                                          push: true,
                                          out _))
            {
                this.RefreshAllViews();
            }
        }
        private void menu_fileSaveMap(object sender, EventArgs e)
        {
            Utility.SaveDialogAndSave(CurrentFile,
                                      "Map File(*.map)|*.map",
                                      "Save map settings",
                                      dataSource.SaveMap,
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
        //-------------------------------------------------------------------
        // Utility
        private void PaintMap(int x, int y, int val)
        {
            int pattern_to_paint = dataSource.GetMapData(x, y);
            if (pattern_to_paint == val) return;
            dataSource.SetMapData(x, y, val, push: false);
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

