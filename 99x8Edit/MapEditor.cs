using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace _99x8Edit
{
    // Map editor window
    public partial class MapEditor : Form
    {
        private readonly Machine _dataSource;
        private readonly MainWindow _mainWin;
        private readonly TabOrder _tabList = new TabOrder();
        private Point _curMapOrg;       // Coordinate of left top corner of map
        // For internal drag control
        private class DnDPattern : DnDBase
        {
            internal DnDPattern(Control c) : base(c) { }
        }
        private class DnDPCG : DnDBase 
        {
            internal DnDPCG(Control c) : base(c) { }
        }
        //------------------------------------------------------------------------------
        // Initialize
        public MapEditor(Machine src, MainWindow parent)
        {
            InitializeComponent();
            // Set corresponding data and owner window
            _dataSource = src;
            _mainWin = parent;
            // Tab order for the customed control
            _tabList.Add(viewPCG, viewPCG.Selector);
            _tabList.Add(viewPtn, viewPtn.Selector);
            _tabList.Add(viewMap, viewMap.Selector);
            // Initialize controls
            btnLeft.Enabled = false;
            btnRight.Enabled = (_dataSource.MapWidth > viewMap.SelectionColNum);
            btnUp.Enabled = false;
            btnDown.Enabled = (_dataSource.MapHeight > viewMap.SelectionRowNum);
            // Refresh all views
            RefreshAllViews();
            // Menu bar
            toolStripFileLoad.Click += menu_fileLoad;
            toolStripFileSave.Click += menu_fileSave;
            toolStripFileSaveAs.Click += menu_fileSaveAs;
            toolStripFileImport.Click += menu_fileImport;
            toolStripFileExport.Click += menu_fileExport;
            toolStripFileLoadMap.Click += menu_fileLoadMap;
            toolStripFileSaveMap.Click += menu_fileSaveMap;
            toolStripEditUndo.Click += menu_editUndo;
            toolStripEditRedo.Click += menu_editRedo;
            // Context menu
            toolStripPCGCopy.Click += contextPCG_Copy;
            toolStripPatternCopy.Click += contextPtn_copy;
            toolStripPatternPaste.Click += contextPtn_paste;
            toolStripPatternCopyDown.Click += contextPtn_copyDown;
            toolStripPatternCopyRight.Click += contextPtn_copyRight;
            toolStripMapCopy.Click += contextMap_copy;
            toolStripMapPaste.Click += contextMap_paste;
            toolStripMapDel.Click += contextMap_del;
            toolStripMapPaint.Click += contextMap_paint;
            toolStripMapCopyDown.Click += contextMap_copyDown;
            toolStripMapCopyRight.Click += contextMap_copyRight;
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
                    viewPCG.SetImage(_dataSource.GetBitmapOfPCG(index), x, y);
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
                        int pcg = _dataSource.GetPCGInPattern(viewPtn.IndexOf(col, row),
                                                              i * 2 + j);
                        Bitmap bmp = _dataSource.GetBitmapOfPCG(pcg);
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
            if ((_curMapOrg.X + 16 > _dataSource.MapWidth)
                || (_curMapOrg.Y + 12 > _dataSource.MapHeight))
            {
                txtMapX.Text = (_curMapOrg.X = 0).ToString();
                txtMapY.Text = (_curMapOrg.Y = 0).ToString();
            }
            // Draw cells in the map
            Action<int, int> one_map_pattern = (x, y) =>
            {
                // Draw one map pattern in map
                int map_x = _curMapOrg.X + x;
                int map_y = _curMapOrg.Y + y;
                int map_ptn = _dataSource.GetMapData(map_x, map_y);
                for (int i = 0; i < 2; ++i)
                {
                    for (int j = 0; j < 2; ++j)
                    {
                        int map_pcg = _dataSource.GetPCGInPattern(map_ptn, i * 2 + j);
                        Bitmap img = _dataSource.GetBitmapOfPCG(map_pcg);
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
            txtMapX.Text = _curMapOrg.X.ToString();
            txtMapY.Text = _curMapOrg.Y.ToString();
            btnLeft.Enabled = ((_curMapOrg.X > 0) && (_dataSource.MapWidth > viewMap.SelectionColNum));
            btnRight.Enabled = (_curMapOrg.X < _dataSource.MapWidth - viewMap.SelectionColNum);
            btnUp.Enabled = ((_curMapOrg.Y > 0) && (_dataSource.MapHeight > viewMap.SelectionRowNum));
            btnDown.Enabled = (_curMapOrg.Y < _dataSource.MapHeight - viewMap.SelectionRowNum);
        }
        //-----------------------------------------------------------------
        // Controls
        //---------------------------------------------
        // PCG list
        private void viewPCG_CellDragStart(object sender, EventArgs e)
        {
            viewPCG.DoDragDrop(new DnDPCG(this), DragDropEffects.Copy);
        }
        private void contextPCG_Copy(object sender, EventArgs e)
        {
            ClipPCGIndex clip = new ClipPCGIndex()
            {
                pcgIndex = viewPCG.Index
            };
            ClipboardWrapper.SetData(clip);
        }
        //---------------------------------------------
        // Patterns
        private void viewPtn_CellDragStart(object sender, EventArgs e)
        {
            viewPtn.DoDragDrop(new DnDPattern(this), DragDropEffects.Copy);
        }
        private void contextPtn_copy(object sender, EventArgs e)
        {
            ClipMapPtn clip = new ClipMapPtn()
            {
                index = viewPtn.Index
            };
            Rectangle r = viewPtn.SelectedRect;
            for (int i = r.Y; i < r.Y + r.Height; ++i)
            {
                List<byte[]> l = new List<byte[]>();
                for (int j = r.X; j < r.X + r.Width; ++j)
                {
                    // For each selected patterns
                    l.Add(_dataSource.GetPattern(i * 16 + j));
                }
                clip.ptns.Add(l);
            }
            ClipboardWrapper.SetData(clip);
        }
        private void contextPtn_paste(object sender, EventArgs e)
        {
            dynamic clip = ClipboardWrapper.GetData();
            switch (clip)
            {
                case ClipMapPtn cm:
                    MementoCaretaker.Instance.Push();
                    Action<int, int, int, int> callback = (col, row, colcnt, rowcnt) =>
                    {
                        // For each copied patterns
                        int target = viewPtn.IndexOf(col, row);
                        _dataSource.SetPattern(target, cm.ptns[rowcnt][colcnt], push: false);
                    };
                    viewPtn.ForEachSelection(viewPtn.X, viewPtn.Y,
                                             clip.ptns?[0]?.Count, clip.ptns?.Count, callback);
                    this.UpdateMapPatterns(refresh: true);
                    this.UpdateMap(refresh: true);
                    break;
                case ClipPCGIndex cp:
                    int src = cp.pcgIndex;
                    int target_ptn = viewPtn.Index;
                    int target_no = viewPtn.SubY * 2 + viewPtn.SubX;
                    _dataSource.SetPCGInPattern(target_ptn, target_no, src, push: true);
                    this.UpdateMapPatterns(refresh: true);
                    this.UpdateMap(refresh: true);
                    break;
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
                byte[] dat = _dataSource.GetPattern(src);
                _dataSource.SetPattern(dst, dat, push: false);
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
                byte[] dat = _dataSource.GetPattern(src);
                _dataSource.SetPattern(dst, dat, push: false);
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
            else if (e.Data.GetDataPresent(typeof(DnDPCG)))
            {
                // Dragged from PCG list
                e.Effect = DragDropEffects.Copy;
            }
        }
        private void panelPtn_DragDrop(object sender, DragEventArgs e)
        {
            (int col, int row) = viewPtn.ScreenCoordinateToSelection(Cursor.Position);
            int target = viewPtn.IndexOf(col, row);
            if (e.Data.GetDataPresent(typeof(DnDPattern)))
            {
                // Other map pattern has been dropped
                _dataSource.CopyMapPattern(viewPtn.Index, target, push: true);
                this.UpdateMapPatterns(refresh: true);
                this.UpdateMap(refresh: true);
            }
            else if (e.Data.GetDataPresent(typeof(DnDPCG)))
            {
                // Character has been dropped from PCG list
                (int col_sub, int row_sub) = viewPtn.ScreenCoordinateToCell(Cursor.Position);
                int target_no = col_sub % 2 + (row_sub % 2) * 2;
                _dataSource.SetPCGInPattern(target, target_no, viewPCG.Index, push: true);
                this.UpdateMapPatterns(refresh: true); 
                this.UpdateMap(refresh: true);
            }
        }
        private void viewPtn_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            // Key events in pattern list
            switch (e.KeyData)
            {
                case Keys.Enter:
                case Keys.Space:
                    // Keyboard space and enter to send pattern to map
                    _dataSource.SetMapData(_curMapOrg.X + viewMap.X,
                                           _curMapOrg.Y + viewMap.Y,
                                           viewPtn.Index, push: true);
                    viewMap.IncrementSelection();
                    this.UpdateMap(refresh: true);
                    break;
            }
        }
        //---------------------------------------------
        // Map
        private void viewMap_MatrixOnScroll(object sender, MatrixControl.ScrollEventArgs e)
        {
            // Scrolled request from control
            if((e.DY < 0) && (_curMapOrg.Y + e.DY >= 0))
            {
                // Up
                _curMapOrg.Y += e.DY;
                this.UpdateMap(refresh: true);
            }
            if((e.DY > 0) && (_curMapOrg.Y + e.DY <= _dataSource.MapHeight - viewMap.SelectionRowNum))
            {
                // Down
                _curMapOrg.Y += e.DY;
                this.UpdateMap(refresh: true);
            }
            if ((e.DX < 0) && (_curMapOrg.X + e.DX >= 0))
            {
                // Left
                _curMapOrg.X += e.DX;
                this.UpdateMap(refresh: true);
            }
            if ((e.DX > 0) && (_curMapOrg.X + e.DX <= _dataSource.MapWidth - viewMap.SelectionColNum))
            {
                // Right
                _curMapOrg.X += e.DX;
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
                    l.Add(_dataSource.GetMapData(_curMapOrg.X + j, _curMapOrg.Y + i));
                }
                clip.ptnID.Add(l);
            }
            ClipboardWrapper.SetData(clip);
        }
        private void contextMap_paste(object sender, EventArgs e)
        {
            dynamic clip = ClipboardWrapper.GetData();
            switch (clip)
            {
                case ClipMapPtn _:
                    // One map pattern has been pasted
                    _dataSource.SetMapData(_curMapOrg.X + viewMap.X,
                                           _curMapOrg.Y + viewMap.Y,
                                           clip.index, push: true);
                    this.UpdateMap(refresh: true);
                    break;
                case ClipMapCell _:
                    // Map data has been pasted
                    MementoCaretaker.Instance.Push();
                    Action<int, int, int, int> callback = (col, row, colcnt, rowcnt) =>
                    {
                        // For each map tiles
                        _dataSource.SetMapData(_curMapOrg.X + col,
                            _curMapOrg.Y + row,
                            clip.ptnID[rowcnt][colcnt], push: false);
                    };
                    viewMap.ForEachSelection(viewMap.X, viewMap.Y,
                        clip.ptnID?[0]?.Count, clip.ptnID?.Count, callback);
                    this.UpdateMap(refresh: true);
                    break;
            }
        }
        private void contextMap_del(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            Action<int, int> callback = (col, row) =>
            {
                // Delete each map patterns
                _dataSource.SetMapData(_curMapOrg.X + col, _curMapOrg.Y + row, 0, push: false);
            };
            viewMap.ForEachSelection(callback);
            this.UpdateMap(refresh: true);
        }
        private void contextMap_paint(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            this.PaintMap(_curMapOrg.X + viewMap.X, _curMapOrg.Y + viewMap.Y, viewPtn.Index);
            this.UpdateMap(refresh: true);
        }
        private void contextMap_copyDown(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            Rectangle r = viewMap.SelectedRect;
            Action<int, int> callback = (col, row) =>
            {
                // For each selected map tiles
                int src = _dataSource.GetMapData(_curMapOrg.X + col, _curMapOrg.Y + r.Y);
                _dataSource.SetMapData(_curMapOrg.X + col, _curMapOrg.Y + row, src, push: false);
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
                int src = _dataSource.GetMapData(_curMapOrg.X + r.X, _curMapOrg.Y + row);
                _dataSource.SetMapData(_curMapOrg.X + col, _curMapOrg.Y + row, src, push: false);
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
                (int col, int row) = viewMap.ScreenCoordinateToSelection(Cursor.Position);
                _dataSource.SetMapData(col, row, viewPtn.Index, push: true);
                this.UpdateMap(refresh: true);
            }
        }
        //---------------------------------------------
        // Misc
        private void btnLeft_Click(object sender, EventArgs e)
        {
            // Scrolling map leftward
            _curMapOrg.X = Math.Max(_curMapOrg.X - viewMap.SelectionColNum, 0);
            this.UpdateMap(refresh: true);
        }
        private void btnRight_Click(object sender, EventArgs e)
        {
            // Scrolling map rightward
            _curMapOrg.X = Math.Min(_curMapOrg.X + viewMap.SelectionColNum,
                                   _dataSource.MapWidth - viewMap.SelectionColNum);
            this.UpdateMap(refresh: true);
        }
        private void btnUp_Click(object sender, EventArgs e)
        {
            // Scrolling map upward
            _curMapOrg.Y = Math.Max(_curMapOrg.Y - viewMap.SelectionRowNum, 0);
            this.UpdateMap(refresh: true);
        }
        private void btnDown_Click(object sender, EventArgs e)
        {
            // Scrolling map downward
            _curMapOrg.Y = Math.Min(_curMapOrg.Y + viewMap.SelectionRowNum,
                                   _dataSource.MapHeight - viewMap.SelectionRowNum);
            this.UpdateMap(refresh: true);
        }
        private void btnMapSize_Click(object sender, EventArgs e)
        {
            // Show the map size window
            MapSize dlg = new MapSize(_dataSource.MapWidth, _dataSource.MapHeight);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                _dataSource.MapWidth = dlg.MapWidth;
                _dataSource.MapHeight = dlg.MapHeight;
                txtMapX.Text = (_curMapOrg.X = 0).ToString();
                txtMapY.Text = (_curMapOrg.Y = 0).ToString();
                btnLeft.Enabled = false;
                btnRight.Enabled = (_dataSource.MapWidth > viewMap.SelectionColNum);
                btnUp.Enabled = false;
                btnDown.Enabled = (_dataSource.MapHeight > viewMap.SelectionRowNum);
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
            // Currently nothing
        }
        private void menu_fileExport(object sender, EventArgs e)
        {
            _mainWin.ExportMap(sender, e);
        }
        private void menu_fileLoadMap(object sender, EventArgs e)
        {
            if (Utility.LoadDialogAndLoad(Config.Setting.MapFileDirectory,
                                          "Map File(*.map)|*.map",
                                          "Load map settings",
                                          _dataSource.LoadMap,
                                          push: true,
                                          out string loaded_filename))
            {
                Config.Setting.MapFileDirectory = Path.GetDirectoryName(loaded_filename);
                this.RefreshAllViews();
            }
        }
        private void menu_fileSaveMap(object sender, EventArgs e)
        {
            if(Utility.SaveDialogAndSave(Config.Setting.MapFileDirectory,
                                        "Map File(*.map)|*.map",
                                        "Save map settings",
                                        _dataSource.SaveMap,
                                        save_as: true,
                                        out string saved_filename))
            {
                Config.Setting.MapFileDirectory = Path.GetDirectoryName(saved_filename);
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
        //-------------------------------------------------------------------
        // Utility
        private void PaintMap(int x, int y, int val)
        {
            int pattern_to_paint = _dataSource.GetMapData(x, y);
            if (pattern_to_paint == val) return;
            _dataSource.SetMapData(x, y, val, push: false);
            if (y > 0)
                if (_dataSource.GetMapData(x, y - 1) == pattern_to_paint)
                    this.PaintMap(x, y - 1, val);
            if (y < _dataSource.MapWidth - 1)
                if (_dataSource.GetMapData(x, y + 1) == pattern_to_paint)
                    this.PaintMap(x, y + 1, val);
            if (x > 0)
                if (_dataSource.GetMapData(x - 1, y) == pattern_to_paint)
                    this.PaintMap(x - 1, y, val);
            if (x < _dataSource.MapHeight - 1)
                if (_dataSource.GetMapData(x + 1, y) == pattern_to_paint)
                    this.PaintMap(x + 1, y, val);
        }
    }
}

