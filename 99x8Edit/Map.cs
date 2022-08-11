﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace _99x8Edit
{
    // Map editor window
    public partial class Map : Form
    {
        Machine dataSource;
        private Bitmap bmpPCGList = new Bitmap(512, 128);       // PCG list view
        private Bitmap bmpMapPatterns = new Bitmap(512, 512);   // Map pattern view
        private Bitmap bmpMap = new Bitmap(512, 384);           // Map view
        private int currentPCGX = 0;            // Selected character
        private int currentPCGY = 0;
        private int currentTilePatternX = 0;    // Selected tile pattern 0-15
        private int currentTilePatternY = 0;    // Selected tile pattern 0-15
        private int currentCellInPatternX = 0;  // Selected cell in tile pattern 0-1
        private int currentCellInPatternY = 0;  // Selected cell in tile pattern 0-1
        private int currentMapOriginX = 0;      // Coordinate of left top corner 0-n
        private int currentMapOriginY = 0;      // Coordinate of left top corner 0-m
        private int currentMapX = 0;            // Selected map cell 0-15
        private int currentMapY = 0;            // Selected map cell 0-11
        private int selStartMapX = 0;           // For multiple selection
        private int selStartMapY = 0;
        //------------------------------------------------------------------------------
        // Initialize
        public Map(Machine dataSource)
        {
            InitializeComponent();
            // Set corresponding data
            this.dataSource = dataSource;
            // Initialize controls
            this.viewPCG.Image = bmpPCGList;
            this.viewPatterns.Image = bmpMapPatterns;
            this.viewMap.Image = bmpMap;
            this.btnLeft.Enabled = false;
            this.btnRight.Enabled = (dataSource.MapWidth > 16);
            this.btnUp.Enabled = false;
            this.btnDown.Enabled = (dataSource.MapHeight > 12);
            // Refresh all views
            this.RefreshAllViews();
            // Context menu
            toolStripPatternCopy.Click += new EventHandler(contextPatterns_copy);
            toolStripPatternPaste.Click += new EventHandler(contextPatterns_paste);
            toolStripMapCopy.Click += new EventHandler(contextMap_copy);
            toolStripMapPaste.Click += new EventHandler(contextMap_paste);
            toolStripMapDel.Click += new EventHandler(contextMap_del);
            toolStripMapPaint.Click += new EventHandler(contextMap_paint);
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
            g.DrawRectangle(new Pen(Color.Red), currentPCGX * 16, currentPCGY * 16, 15, 15);
            if (refresh) this.viewPCG.Refresh();
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
                    int one_pcg = dataSource.GetMapPattern(i, j);
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
            int cx = currentTilePatternX * 32;
            int cy = currentTilePatternY * 32;
            g.DrawRectangle(new Pen(Color.Red), cx, cy, 31, 31);
            g.DrawRectangle(new Pen(Color.Yellow),
                cx + currentCellInPatternX * 16 + (1 - currentCellInPatternX),
                cy + currentCellInPatternY * 16 + (1 - currentCellInPatternY), 14, 14);
            if (refresh) this.viewPatterns.Refresh();
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
            g.DrawRectangle(new Pen(Color.Red),
                            Math.Min(currentMapX, selStartMapX) * 32,
                            Math.Min(currentMapY, selStartMapY) * 32,
                            (Math.Abs(currentMapX - selStartMapX) + 1) * 32 - 1,
                            (Math.Abs(currentMapY - selStartMapY) + 1) * 32 - 1);
            if (refresh) this.viewMap.Refresh();
            // Map size may be changed by loading, undo, etc
            if ((currentMapOriginX + 16 > dataSource.MapWidth) || (currentMapOriginY + 12 > dataSource.MapHeight))
            {
                txtMapX.Text = (currentMapOriginX = 0).ToString();
                txtMapY.Text = (currentMapOriginY = 0).ToString();
            }
            // Controls corresponding to map
            if(currentMapOriginX.ToString() != txtMapX.Text)
            {
                txtMapX.Text = currentMapOriginX.ToString();
            }
            if (currentMapOriginY.ToString() != txtMapY.Text)
            {
                txtMapY.Text = currentMapOriginY.ToString();
            }
            btnLeft.Enabled = ((currentMapOriginX > 0) && (dataSource.MapWidth > 16));
            btnRight.Enabled = (currentMapOriginX < dataSource.MapWidth - 16);
            btnUp.Enabled = ((currentMapOriginY > 0) && (dataSource.MapHeight > 12));
            btnDown.Enabled = (currentMapOriginY < dataSource.MapHeight - 12);
        }
        private void DrawOneMapCell(Graphics g, int x, int y)
        {
            // Redraw map view
            int map_x = currentMapOriginX + x;
            int map_y = currentMapOriginY + y;
            int map_ptn = dataSource.GetMapData(map_x, map_y);
            for(int i = 0; i < 2; ++i)
            {
                for(int j = 0; j < 2; ++j)
                {
                    int map_pcg = dataSource.GetMapPattern(map_ptn, i * 2 + j);
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
            int clicked_pcg_x = e.X / 16;
            int clicked_pcg_y = e.Y / 16;
            if (clicked_pcg_x > 31) clicked_pcg_x = 31;
            if (clicked_pcg_y > 7) clicked_pcg_y = 7;
            if ((clicked_pcg_x != currentPCGX) || (clicked_pcg_y != currentPCGY))
            {
                // Selected PCG has changed
                currentPCGX = clicked_pcg_x;
                currentPCGY = clicked_pcg_y;
                this.UpdatePCGList();
            }
            if (e.Button == MouseButtons.Left)
            {
                viewPCG.DoDragDrop(new DnDMapPCG(), DragDropEffects.Copy);
            }
        }
        private void panelPCG_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Up:
                    if(currentPCGY > 0)
                    {
                        currentPCGY--;
                        this.UpdatePCGList();
                    }
                    break;
                case Keys.Left:
                    if(currentPCGX > 0)
                    {
                        currentPCGX--;
                        this.UpdatePCGList();
                    }
                    break;
                case Keys.Right:
                    if (currentPCGX < 31)
                    {
                        currentPCGX++;
                        this.UpdatePCGList();
                    }
                    break;
                case Keys.Down:
                    if (currentPCGY < 7)
                    {
                        currentPCGY++;
                        this.UpdatePCGList();
                    }
                    break;
                case Keys.Enter:
                    dataSource.SetMapPattern(currentTilePatternY * 16 + currentTilePatternX,
                                             currentCellInPatternX * 2 + currentCellInPatternY / 2,
                                             currentPCGY * 32 + currentPCGX);
                    this.UpdateMapPatterns();
                    this.UpdateMap();
                    break;
            }
        }
        private void contextPatterns_copy(object sender, EventArgs e)
        {
            int index = currentTilePatternX + currentTilePatternY * 16;
            dataSource.CopyMapPatternToClip(index);
        }
        private void contextPatterns_paste(object sender, EventArgs e)
        {
            int index = currentTilePatternX + currentTilePatternY * 16;
            dataSource.PasteMapPatternFromClip(index);
            this.UpdateMapPatterns();
        }
        private void viewPatterns_MouseDown(object sender, MouseEventArgs e)
        {
            panelPatterns.Focus();  // Key events are handled by parent panel
            // Tile pattern selected
            int selected_ptn_x = e.X / 32;
            int selected_ptn_y = e.Y / 32;
            int selected_cell_x = (e.X / 16) % 2;
            int selected_cell_y = (e.Y / 16) % 2;
            int selected_pattern_num = selected_ptn_x + selected_ptn_y * 16;
            int current_pattern_num = currentTilePatternX + currentTilePatternY * 16;
            if ((selected_pattern_num != current_pattern_num)
                || (selected_cell_x != currentCellInPatternX) 
                || (selected_cell_y != currentCellInPatternY))
            {
                currentTilePatternX = selected_ptn_x;
                currentTilePatternY = selected_ptn_y;
                currentCellInPatternX = selected_cell_x;
                currentCellInPatternY = selected_cell_y;
                this.UpdateMapPatterns();
            }
            if (e.Button == MouseButtons.Left)
            {
                viewPatterns.DoDragDrop(new DnDPattern(), DragDropEffects.Copy);
            }
        }
        private void panelPatterns_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Up:
                    if (currentCellInPatternY > 0)
                    {
                        currentCellInPatternY--;
                        this.UpdateMapPatterns();
                    }
                    else if(currentTilePatternY > 0)
                    {
                        currentTilePatternY--;
                        this.UpdateMapPatterns();
                    }
                    break;
                case Keys.Left:
                    if (currentCellInPatternX > 0)
                    {
                        currentCellInPatternX--;
                        this.UpdateMapPatterns();
                    }
                    else if (currentTilePatternX > 0)
                    {
                        currentTilePatternX--;
                        this.UpdateMapPatterns();
                    }
                    break;
                case Keys.Right:
                    if (currentCellInPatternX == 0)
                    {
                        currentCellInPatternX++;
                        this.UpdateMapPatterns();
                    }
                    else if (currentTilePatternX < 15)
                    {
                        currentTilePatternX++;
                        this.UpdateMapPatterns();
                    }
                    break;
                case Keys.Down:
                    if (currentCellInPatternY == 0)
                    {
                        currentCellInPatternY++;
                        this.UpdateMapPatterns();
                    }
                    else if (currentTilePatternY < 15)
                    {
                        currentTilePatternY++;
                        this.UpdateMapPatterns();
                    }
                    break;
                case Keys.Enter:
                    int current_ptn = currentTilePatternX + currentTilePatternY * 16;
                    dataSource.SetMapData(currentMapOriginX + currentMapX, currentMapOriginY + currentMapY, current_ptn);
                    int prev_x = currentMapX;
                    int prev_y = currentMapY;
                    currentMapX = (currentMapX + 1) % 16;
                    if (currentMapX == 0) currentMapY = (currentMapY + 1) % 12;
                    selStartMapX = currentMapX;
                    selStartMapY = currentMapY;
                    this.UpdateMap();
                    break;
            }
        }
        private void panelPatterns_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DnDPattern))) e.Effect = DragDropEffects.Copy;
            else if (e.Data.GetDataPresent(typeof(DnDMapPCG))) e.Effect = DragDropEffects.Copy;
            else e.Effect = DragDropEffects.None;
        }
        private void panelPatterns_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DnDPattern)))
            {
                Point p = viewPatterns.PointToClient(Cursor.Position);
                if (p.X > viewPatterns.Width - 1) p.X = viewPatterns.Width - 1;
                if (p.Y > viewPatterns.Height - 1) p.X = viewPatterns.Height - 1;
                dataSource.CopyMapPattern(currentTilePatternY * 16 + currentTilePatternX, (p.Y / 32) * 16 + (p.X / 32));
                this.UpdateMapPatterns();
                this.UpdateMap();
            }
            else if (e.Data.GetDataPresent(typeof(DnDMapPCG)))
            {
                Point p = viewPatterns.PointToClient(Cursor.Position);
                if (p.X > viewPatterns.Width - 1) p.X = viewPatterns.Width - 1;
                if (p.Y > viewPatterns.Height - 1) p.X = viewPatterns.Height - 1;
                int target_ptn = p.X / 32 + (p.Y / 32) * 16;
                int target_cell = (p.X / 16) % 2 + ((p.Y / 16) % 2) * 2;
                dataSource.SetMapPattern(target_ptn, target_cell, currentPCGY * 32 + currentPCGX);
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
                int previous_x = currentMapX;
                int previous_y = currentMapY;
                if ((selected_x != previous_x) || (selected_y != previous_y))
                {
                    currentMapX = selStartMapX = selected_x;
                    currentMapY = selStartMapY = selected_y;
                    this.UpdateMap();
                }
                viewPCG.DoDragDrop(new DnDMap(), DragDropEffects.Copy);
            }
        }
        private void panelMap_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Up | Keys.Shift:
                    if (currentMapY > 0)
                    {
                        currentMapY--;
                        this.UpdateMap();
                    }
                    break;
                case Keys.Down | Keys.Shift:
                    if(currentMapY < 11)
                    {
                        currentMapY++;
                        this.UpdateMap();
                    }
                    break;
                case Keys.Left | Keys.Shift:
                    if (currentMapX > 0)
                    {
                        currentMapX--;
                        this.UpdateMap();
                    }
                    break;
                case Keys.Right | Keys.Shift:
                    if (currentMapX < 15)
                    {
                        currentMapX++;
                        this.UpdateMap();
                    }
                    break;
                case Keys.Up:
                    if(currentMapY == 0)
                    {
                        if (currentMapOriginY > 0) currentMapOriginY--;
                    }
                    else currentMapY--;
                    selStartMapX = currentMapX;
                    selStartMapY = currentMapY;
                    this.UpdateMap();
                    break;
                case Keys.Left:
                    if(currentMapX == 0)
                    {
                        if (currentMapOriginX > 0) currentMapOriginX--;
                    }
                    else currentMapX--;
                    selStartMapX = currentMapX;
                    selStartMapY = currentMapY;
                    this.UpdateMap();
                    break;
                case Keys.Right:
                    if (currentMapX == 15)
                    {
                        if (currentMapOriginX < dataSource.MapWidth - 16) currentMapOriginX++;
                    }
                    else currentMapX++;
                    selStartMapX = currentMapX;
                    selStartMapY = currentMapY;
                    this.UpdateMap();
                    break;
                case Keys.Down:
                    if(currentMapY == 11)
                    {
                        if (currentMapOriginY < dataSource.MapHeight - 12) currentMapOriginY++;
                    }
                    else currentMapY++;
                    selStartMapX = currentMapX;
                    selStartMapY = currentMapY;
                    this.UpdateMap();
                    break;
            }
        }
        private void contextMap_copy(object sender, EventArgs e)
        {
            ClipMapCell clip = new ClipMapCell();
            int x = Math.Min(currentMapX, selStartMapX);
            int y = Math.Min(currentMapY, selStartMapY);
            int w = Math.Abs(currentMapX - selStartMapX) + 1;
            int h = Math.Abs(currentMapY - selStartMapY) + 1;
            for(int i = y; i < y + h; ++i)
            {
                List<int> l = new List<int>();
                for(int j = x; j < x + w; ++j)
                {
                    l.Add(dataSource.GetMapData(currentMapOriginX + j, currentMapOriginY + i));
                }
                clip.ptnID.Add(l);
            }
            ClipboardWrapper.SetData(clip);
        }
        private void contextMap_paste(object sender, EventArgs e)
        {
            dynamic clip = ClipboardWrapper.GetData();
            if (clip is ClipOneMapPattern)
            {
                dataSource.SetMapData(currentMapOriginX + currentMapX, currentMapOriginY + currentMapY, clip.index);
                this.UpdateMap();
            }
            else if (clip is ClipMapCell)
            {
                MementoCaretaker.Instance.Push();
                for (int i = 0; (i < clip.ptnID.Count) && (currentMapY + i < 12); ++i)
                {
                    List<int> l = clip.ptnID[i];
                    for (int j = 0; (j < l.Count) && (currentMapX + j < 16); ++j)
                    {
                        dataSource.SetMapData(currentMapX + j, currentMapY + i, l[j], false);
                    }
                }
                this.UpdateMap();
            }
        }
        private void contextMap_del(object sender, EventArgs e)
        {
            MementoCaretaker.Instance.Push();
            int x = Math.Min(currentMapX, selStartMapX);
            int y = Math.Min(currentMapY, selStartMapY);
            int w = Math.Abs(currentMapX - selStartMapX) + 1;
            int h = Math.Abs(currentMapY - selStartMapY) + 1;
            for (int i = y; (i < y + h) && (i < 12); ++i)
            {
                for (int j = x; (j < x + w) && (j < 16); ++j)
                {
                    dataSource.SetMapData(currentMapOriginX + j, currentMapOriginY + i, 0, false);
                }
            }
            this.UpdateMap();
        }
        private void contextMap_paint(object sender, EventArgs e)
        {
            int selected_ptn_num = currentTilePatternX + currentTilePatternY * 16;
            MementoCaretaker.Instance.Push();   // For undo action
            this.paintMap(currentMapOriginX + currentMapX, currentMapOriginY + currentMapY, selected_ptn_num);
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
                currentMapX = Math.Min(p.X / 32, 15);
                currentMapY = Math.Min(p.Y / 32, 11);
                this.UpdateMap();
            }
        }
        private void panelMap_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DnDPattern)))
            {
                Point p = viewMap.PointToClient(Cursor.Position);
                dataSource.SetMapData(Math.Min(p.X / 32, 15), Math.Min(p.Y / 32, 11),
                                      currentTilePatternY * 16 + currentTilePatternX);
                this.UpdateMap();
            }
        }
        private void btnLeft_Click(object sender, EventArgs e)
        {
            currentMapOriginX -= 16;
            if (currentMapOriginX <= 0)
            {
                currentMapOriginX = 0;
                this.btnLeft.Enabled = false;
            }
            if(dataSource.MapWidth > 16)
            {
                this.btnRight.Enabled = true;
            }
            this.txtMapX.Text = currentMapOriginX.ToString();
            this.UpdateMap();
        }
        private void btnRight_Click(object sender, EventArgs e)
        {
            currentMapOriginX += 16;
            if(currentMapOriginX + 16 > dataSource.MapWidth)
            {
                currentMapOriginX = dataSource.MapWidth - 16;
                this.btnRight.Enabled = false;
            }
            if(dataSource.MapWidth > 16)
            {
                this.btnLeft.Enabled = true;
            }
            this.txtMapX.Text = currentMapOriginX.ToString();
            this.UpdateMap();
        }
        private void btnUp_Click(object sender, EventArgs e)
        {
            currentMapOriginY -= 12;
            if (currentMapOriginY <= 0)
            {
                currentMapOriginY = 0;
                this.btnUp.Enabled = false;
            }
            if (dataSource.MapHeight > 12)
            {
                this.btnDown.Enabled = true;
            }
            this.txtMapY.Text = currentMapOriginY.ToString();
            this.UpdateMap();
        }
        private void btnDown_Click(object sender, EventArgs e)
        {
            currentMapOriginY += 12;
            if (currentMapOriginY + 12 > dataSource.MapHeight)
            {
                currentMapOriginY = dataSource.MapHeight - 12;
                this.btnDown.Enabled = false;
            }
            if (dataSource.MapHeight > 12)
            {
                this.btnUp.Enabled = true;
            }
            this.txtMapY.Text = currentMapOriginY.ToString();
            this.UpdateMap();
        }
        private void btnMapSize_Click(object sender, EventArgs e)
        {
            MapSize dlg = new MapSize(dataSource.MapWidth, dataSource.MapHeight);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                dataSource.MapWidth = dlg.MapWidth;
                dataSource.MapHeight = dlg.MapHeight;
                this.txtMapX.Text = (currentMapOriginX = 0).ToString();
                this.txtMapY.Text = (currentMapOriginY = 0).ToString();
                this.btnLeft.Enabled = false;
                this.btnRight.Enabled = (dataSource.MapWidth > 16);
                this.btnUp.Enabled = false;
                this.btnDown.Enabled = (dataSource.MapHeight > 12);
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
        //-------------------------------------------------------------------
        // Utility
        private void paintMap(int x, int y, int val)
        {
            int pattern_to_paint = dataSource.GetMapData(x, y);
            if (pattern_to_paint == val) return;
            dataSource.SetMapData(x, y, val, false);
            if (y > 0)
                if (dataSource.GetMapData(x, y - 1) == pattern_to_paint)
                    this.paintMap(x, y - 1, val);
            if (y < dataSource.MapWidth - 1)
                if (dataSource.GetMapData(x, y + 1) == pattern_to_paint)
                    this.paintMap(x, y + 1, val);
            if (x > 0)
                if (dataSource.GetMapData(x - 1, y) == pattern_to_paint)
                    this.paintMap(x - 1, y, val);
            if (x < dataSource.MapHeight - 1)
                if (dataSource.GetMapData(x + 1, y) == pattern_to_paint)
                    this.paintMap(x + 1, y, val);
        }
    }
    // For DnD actions
    class DnDPattern{ }
    class DnDMapPCG { }
    class DnDMap { }
}

