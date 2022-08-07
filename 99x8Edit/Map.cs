using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace _99x8Edit
{
    public partial class Map : Form
    {
        Machine dataSource;
        private Bitmap bmpPCGList = new Bitmap(512, 128);       // PCG list view
        private Bitmap bmpMapPatterns = new Bitmap(512, 512);   // Map pattern view
        private Bitmap bmpMap = new Bitmap(512, 384);           // Map view
        private int currentPCG = 0;             // Selected character(0-255)
        private int currentTilePatternX = 0;    // Selected tile pattern 0-15
        private int currentTilePatternY = 0;    // Selected tile pattern 0-15
        private int currentCellInPatternX = 0;  // Selected cell in tile pattern 0-1
        private int currentCellInPatternY = 0;  // Selected cell in tile pattern 0-1
        private int currentMapOriginX = 0;      // Coordinate of left top corner 0-n
        private int currentMapOriginY = 0;      // Coordinate of left top corner 0-m
        private int currentMapX = 0;            // Selected map cell 0-15
        private int currentMapY = 0;            // Selected map cell 0-11
        //------------------------------------------------------------------------------
        // Initialize
        public Map(Machine dataSource)
        {
            InitializeComponent();
            // Set corresponding data
            this.dataSource = dataSource;
            // Refresh all views
            this.RefreshAllViews();
            // Initialize controls
            this.viewPCG.Image = bmpPCGList;
            this.viewPatterns.Image = bmpMapPatterns;
            this.viewMap.Image = bmpMap;
            this.btnLeft.Enabled = false;
            this.btnRight.Enabled = (dataSource.MapWidth > 16);
            this.btnUp.Enabled = false;
            this.btnDown.Enabled = (dataSource.MapHeight > 12);
            // Context menu
            toolStripPatternCopy.Click += new EventHandler(contextPatterns_copy);
            toolStripPatternPaste.Click += new EventHandler(contextPatterns_paste);
            toolStripMapCopy.Click += new EventHandler(contextMap_copy);
            toolStripMapPaste.Click += new EventHandler(contextMap_paste);
            toolStripMapDel.Click += new EventHandler(contextMap_del);
            toolStripMapPaint.Click += new EventHandler(contextMap_paint);
        }
        //------------------------------------------------------------------------------
        // Refreshing Views
        private void RefreshAllViews()
        {
            this.UpdatePCGList();           // PCG view
            this.RefreshMapPatterns();      // Map patterns
            this.RefreshMap();              // Map
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
        private void RefreshMapPatterns()
        {
            for (int i = 0; i < 256; ++i)
            {
                this.RefreshMapPatterns(i);
            }
        }
        private void RefreshMapPatterns(int pattern_num)
        {
            Graphics g = Graphics.FromImage(bmpMapPatterns);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            int pattern_x = (pattern_num % 16) * 32;
            int pattern_y = (pattern_num / 16) * 32;
            for (int i = 0; i < 4; ++i)
            {
                int one_pcg = dataSource.GetMapPattern(pattern_num, i);
                Bitmap one_bmp = dataSource.GetBitmapOfPCG(one_pcg);
                int pcg_x = pattern_x + (i % 2) * 16;
                int pcg_y = pattern_y + (i / 2) * 16;
                g.DrawImage(one_bmp, pcg_x, pcg_y, 17, 17);
            }
            if ((pattern_num % 16 == currentTilePatternX) && (pattern_num / 16 == currentTilePatternY))
            {
                g.DrawRectangle(new Pen(Color.Red), pattern_x, pattern_y, 31, 31);
                g.DrawRectangle(new Pen(Color.Yellow), pattern_x + currentCellInPatternX * 16 + (1 - currentCellInPatternX),
                    pattern_y + currentCellInPatternY * 16 + (1 - currentCellInPatternY), 14, 14);
            }
            this.viewPatterns.Refresh();
        }
        private void RefreshMap()
        {
            int width = dataSource.MapWidth;
            int height = dataSource.MapHeight;
            for(int i = 0; i < 12; ++i)
            {
                for(int j = 0; j < 16; ++j)
                {
                    RefreshMap(j, i);
                }
            }
        }
        private void RefreshMap(int x, int y)
        {
            Graphics g = Graphics.FromImage(bmpMap);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            // Map size may be changed by loading, undo, etc
            if((currentMapOriginX + 16 > dataSource.MapWidth) || (currentMapOriginY + 12 > dataSource.MapHeight))
            {
                this.txtMapX.Text = (currentMapOriginX = 0).ToString();
                this.txtMapY.Text = (currentMapOriginY = 0).ToString();
                this.btnLeft.Enabled = false;
                this.btnRight.Enabled = (dataSource.MapWidth > 16);
                this.btnUp.Enabled = false;
                this.btnDown.Enabled = (dataSource.MapHeight > 12);
            }
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
            if((x == currentMapX) && (y == currentMapY))
            {
                g.DrawRectangle(new Pen(Color.Red), x * 32, y * 32, 31, 31);
            }
            this.viewMap.Refresh();
        }
        //-----------------------------------------------------------------
        // Controls
        private void viewPCG_MouseDown(object sender, MouseEventArgs e)
        {
            int clicked_pcg = (e.Y / 16) * 32 + e.X / 16;
            if (clicked_pcg != currentPCG)
            {
                // Selected PCG has changed
                int previous_pcg = currentPCG;
                currentPCG = clicked_pcg;
                this.UpdatePCGList(previous_pcg);   // Erase previous selection
                this.UpdatePCGList(currentPCG);     // Redraw current selection
            }
            if (e.Button == MouseButtons.Left)
            {
                viewPCG.DoDragDrop(new DnDMapPCG(), DragDropEffects.Copy);
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
            this.RefreshMapPatterns(index);
        }
        private void viewPatterns_MouseDown(object sender, MouseEventArgs e)
        {
            panelPatterns.Focus();  // Catch CTRL+C at the parent panel
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
                this.RefreshMapPatterns(current_pattern_num);  // Erase previous selection
                this.RefreshMapPatterns(selected_pattern_num); // Redraw current selection
            }
            if (e.Button == MouseButtons.Left)
            {
                viewPatterns.DoDragDrop(new DnDMapPattern(), DragDropEffects.Copy);
            }
        }
        private void panelPatterns_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DnDMapPattern))) e.Effect = DragDropEffects.Copy;
            else if (e.Data.GetDataPresent(typeof(DnDMapPCG))) e.Effect = DragDropEffects.Copy;
            else e.Effect = DragDropEffects.None;
        }
        private void panelPatterns_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DnDMapPattern)))
            {
                Point p = viewPatterns.PointToClient(Cursor.Position);
                if (p.X > viewPatterns.Width - 1) p.X = viewPatterns.Width - 1;
                if (p.Y > viewPatterns.Height - 1) p.X = viewPatterns.Height - 1;
                dataSource.CopyMapPattern(currentTilePatternY * 16 + currentTilePatternX, (p.Y / 32) * 16 + (p.X / 32));
                this.RefreshMapPatterns();
                this.RefreshMap();
            }
            else if (e.Data.GetDataPresent(typeof(DnDMapPCG)))
            {
                Point p = viewPatterns.PointToClient(Cursor.Position);
                if (p.X > viewPatterns.Width - 1) p.X = viewPatterns.Width - 1;
                if (p.Y > viewPatterns.Height - 1) p.X = viewPatterns.Height - 1;
                int target_ptn = p.X / 32 + (p.Y / 32) * 16;
                int target_cell = (p.X / 16) % 2 + ((p.Y / 16) % 2) * 2;
                dataSource.SetMapPattern(target_ptn, target_cell, currentPCG);
                this.RefreshMapPatterns(target_ptn);   // Redraw current selection
                this.RefreshMap();
            }
        }
        private void viewMap_MouseClick(object sender, MouseEventArgs e)
        {
            // Selected map cell has changed
            panelMap.Focus();   // CTRL+C and others will be cathced at parent panel
            int selected_x = e.X / 32;
            int selected_y = e.Y / 32;
            int previous_x = currentMapX;
            int previous_y = currentMapY;
            if((selected_x != previous_x) || (selected_y != previous_y))
            {
                currentMapX = selected_x;
                currentMapY = selected_y;
                this.RefreshMap(previous_x, previous_y);  // Erase previous selection
                this.RefreshMap(selected_x, selected_y);  // Redraw current selection
            }
        }
        private void contextMap_copy(object sender, EventArgs e)
        {
            ClipMapCell clip = new ClipMapCell();
            clip.dat = dataSource.GetMapData(currentMapOriginX + currentMapX, currentMapOriginY + currentMapY);
            Clipboard.Instance.Clip = clip;
        }
        private void contextMap_paste(object sender, EventArgs e)
        {
            dynamic clip = Clipboard.Instance.Clip;
            if(clip is ClipMapCell)
            {
                dataSource.SetMapData(currentMapOriginX + currentMapX, currentMapOriginY + currentMapY, clip.dat);
                this.RefreshMap(currentMapX, currentMapY);  // Redraw current selection of map
            }
        }
        private void contextMap_del(object sender, EventArgs e)
        {
            dataSource.SetMapData(currentMapOriginX + currentMapX, currentMapOriginY + currentMapY, 0);
            this.RefreshMap(currentMapX, currentMapY);  // Redraw current selection of map
        }
        private void contextMap_paint(object sender, EventArgs e)
        {
            int selected_ptn_num = currentTilePatternX + currentTilePatternY * 16;
            this.paintMap(currentMapOriginX + currentMapX, currentMapOriginY + currentMapY, selected_ptn_num);
            this.RefreshMap();
        }
        private void panelMap_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DnDMapPattern))) e.Effect = DragDropEffects.Copy;
            else e.Effect = DragDropEffects.None;
        }
        private void panelMap_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DnDMapPattern)))
            {
                Point p = viewMap.PointToClient(Cursor.Position);
                if (p.X > viewMap.Width - 1) p.X = viewMap.Width - 1;
                if (p.Y > viewMap.Height - 1) p.X = viewMap.Height - 1;
                dataSource.SetMapData(p.X / 32, p.Y / 32, currentTilePatternY * 16 + currentTilePatternX);
                this.RefreshMap();
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
            this.RefreshMap();
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
            this.RefreshMap();
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
            this.RefreshMap();
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
            this.RefreshMap();
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
            // Refresh everything when form has been activated, since PCGs may be edited
            this.RefreshAllViews();
        }
        public void ChangeOccuredByHost()
        {
            this.RefreshAllViews();
        }
        //-------------------------------------------------------------------
        // Utility
        private void paintMap(int x, int y, int val)
        {
            int pattern_to_paint = dataSource.GetMapData(x, y);
            dataSource.SetMapData(x, y, val);
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
    class DnDMapPattern{ }
    class DnDMapPCG { }
}

