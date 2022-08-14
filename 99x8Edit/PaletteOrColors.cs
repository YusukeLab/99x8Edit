using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;

namespace _99x8Edit
{
    public partial class PaletteOrColors : Form
    {
        // List the OR color pairs of V9938
        private const int colWidth = 128;
        private const int colMax = 5;
        private const int rowHeight = 38;
        private const int rowMax = 11;
        private readonly Machine dataSource;
        private Bitmap bmpColorList = new Bitmap(colWidth * colMax, rowHeight * rowMax);
        private Bitmap bmpPalette = new Bitmap(256, 64);        // Palette view
        private Dictionary<int, int> orgDict = new Dictionary<int, int>();
        private Dictionary<int, int> filteredDict = new Dictionary<int, int>();
        private int currentFilter = 0;
        public PaletteOrColors(Machine src)
        {
            InitializeComponent();
            dataSource = src;
            viewColor.Image = bmpColorList;
            viewPalette.Image = bmpPalette;
            // Make list of the elements of OR colors
            for (int i = 1; i < 16; ++i)
            {
                for (int j = 1; j < 16; ++j)
                {
                    if (i == j)
                    {
                        // Igonore the same pair
                        continue;
                    }
                    int key = i << 4 | j;
                    int or_color_code = i | j;
                    if ((or_color_code == i) || (or_color_code == j))
                    {
                        // Ignore if OR color equals to another element
                        continue;
                    }
                    int key2 = j << 4 | i;
                    if (orgDict.ContainsKey(key2))
                    {
                        // Ignore the pair of same element
                        continue;
                    }
                    orgDict.Add(key, or_color_code);
                }
            }
            filteredDict = new Dictionary<int, int>(orgDict);
            this.UpdateColorList();
            this.UpdatePaletteView();
        }
        private void PaletteOrColors_Deactivate(object sender, EventArgs e)
        {
            this.Dispose();
        }
        private void UpdateColorList()
        {
            Graphics g = Graphics.FromImage(bmpColorList);
            g.Clear(Color.Transparent);
            Font f = new Font(label1.Font, FontStyle.Regular);
            int col = 0;
            int row = 0;
            foreach (KeyValuePair<int, int> kvp in filteredDict)
            {
                int x = col * colWidth + 8;
                int y = row * rowHeight + 8;
                // Draw left color
                int left_color_code = kvp.Key >> 4;
                Brush left = dataSource.BrushOf(left_color_code);
                g.FillRectangle(left, x, y, 23, 23);
                g.DrawString(left_color_code.ToString(), f, Brushes.White, x + 0, y + 0);
                g.DrawString(left_color_code.ToString(), f, Brushes.Black, x + 1, y + 1);
                g.DrawString("+", f, Brushes.Black, x + 24, y);
                // Draw right color
                x += 40;
                int right_color_code = kvp.Key & 0x0F;
                Brush right = dataSource.BrushOf(right_color_code);
                g.FillRectangle(right, x, y, 23, 23);
                g.DrawString(right_color_code.ToString(), f, Brushes.White, x + 0, y + 0);
                g.DrawString(right_color_code.ToString(), f, Brushes.Black, x + 1, y + 1);
                g.DrawString("=", f, Brushes.Black, x + 24, y);
                // Draw OR color
                x += 40;
                int or_color_code = kvp.Value;
                Brush cor = dataSource.BrushOf(or_color_code);
                g.FillRectangle(cor, x, y, 23, 23);
                g.DrawString(or_color_code.ToString(), f, Brushes.White, x + 0, y + 0);
                g.DrawString(or_color_code.ToString(), f, Brushes.Black, x + 1, y + 1);
                // To next col
                if (++col == colMax)
                {
                    col = 0;
                    ++row;
                }
            }
            viewColor.Refresh();
        }
        private void UpdatePaletteView()
        {
            // Draw palette
            Utility.DrawTransparent(bmpPalette);
            Graphics g = Graphics.FromImage(bmpPalette);
            for (int i = 1; i < 16; ++i)
            {
                Brush b = dataSource.BrushOf(i);
                g.FillRectangle(b, (i % 8) * 32, (i / 8) * 32, 32, 32);
            }
            int x = currentFilter % 8 * 32;
            int y = currentFilter / 8 * 32;
            Utility.DrawSelection(g, x, y, 31, 31, true);
            viewPalette.Refresh();
        }
        private void viewPalette_MouseMove(object sender, MouseEventArgs e)
        {
            // Filter the elements with the hovered color
            int color_num = Math.Clamp((e.Y / 32) * 8 + (e.X / 32), 0, 15);
            if(color_num != currentFilter)
            {
                currentFilter = color_num;
                filteredDict = orgDict.Where(p => (p.Value == currentFilter)
                                            || ((p.Key >> 4) == currentFilter)
                                            || ((p.Key & 0x0F) == currentFilter)
                                            || currentFilter == 0)
                                      .ToDictionary(p => p.Key, p => p.Value);
                this.UpdatePaletteView();
                this.UpdateColorList();
            }
        }
    }
}
