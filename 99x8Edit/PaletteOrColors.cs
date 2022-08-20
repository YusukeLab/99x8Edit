using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace _99x8Edit
{
    public partial class PaletteOrColors : Form
    {
        // List the OR color pairs of V9938
        private const int _colWidth = 128;
        private const int _colMax = 5;
        private const int _rowHeight = 38;
        private const int _rowMax = 11;
        private readonly Machine _dataSource;
        private readonly Bitmap _bmpColorList = new Bitmap(_colWidth * _colMax, _rowHeight * _rowMax);
        private readonly Bitmap _bmpPalette = new Bitmap(256, 64);        // Palette view
        private readonly Dictionary<int, int> _orgDict = new Dictionary<int, int>();
        private Dictionary<int, int> _filteredDict;
        private int _currentFilter;
        public PaletteOrColors(Machine src)
        {
            InitializeComponent();
            _dataSource = src;
            viewColor.Image = _bmpColorList;
            viewPalette.Image = _bmpPalette;
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
                    if (_orgDict.ContainsKey(key2))
                    {
                        // Ignore the pair of same element
                        continue;
                    }
                    _orgDict.Add(key, or_color_code);
                }
            }
            _filteredDict = new Dictionary<int, int>(_orgDict);
            this.UpdateColorList();
            this.UpdatePaletteView();
        }
        private void PaletteOrColors_Deactivate(object sender, EventArgs e)
        {
            this.Dispose();
        }
        private void UpdateColorList()
        {
            Graphics g = Graphics.FromImage(_bmpColorList);
            g.Clear(Color.Transparent);
            Font f = new Font(label1.Font, FontStyle.Regular);
            int col = 0;
            int row = 0;
            foreach (KeyValuePair<int, int> kvp in _filteredDict)
            {
                int x = col * _colWidth + 8;
                int y = row * _rowHeight + 8;
                // Draw left color
                int left_color_code = kvp.Key >> 4;
                Brush left = _dataSource.BrushOf(left_color_code);
                g.FillRectangle(left, x, y, 23, 23);
                g.DrawString(left_color_code.ToString(), f, Brushes.White, x + 0, y + 0);
                g.DrawString(left_color_code.ToString(), f, Brushes.Black, x + 1, y + 1);
                g.DrawString("+", f, Brushes.Black, x + 24, y);
                // Draw right color
                x += 40;
                int right_color_code = kvp.Key & 0x0F;
                Brush right = _dataSource.BrushOf(right_color_code);
                g.FillRectangle(right, x, y, 23, 23);
                g.DrawString(right_color_code.ToString(), f, Brushes.White, x + 0, y + 0);
                g.DrawString(right_color_code.ToString(), f, Brushes.Black, x + 1, y + 1);
                g.DrawString("=", f, Brushes.Black, x + 24, y);
                // Draw OR color
                x += 40;
                int or_color_code = kvp.Value;
                Brush cor = _dataSource.BrushOf(or_color_code);
                g.FillRectangle(cor, x, y, 23, 23);
                g.DrawString(or_color_code.ToString(), f, Brushes.White, x + 0, y + 0);
                g.DrawString(or_color_code.ToString(), f, Brushes.Black, x + 1, y + 1);
                // To next col
                if (++col == _colMax)
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
            Utility.DrawTransparent(_bmpPalette);
            Graphics g = Graphics.FromImage(_bmpPalette);
            for (int i = 1; i < 16; ++i)
            {
                Brush b = _dataSource.BrushOf(i);
                g.FillRectangle(b, (i % 8) * 32, (i / 8) * 32, 32, 32);
            }
            int x = _currentFilter % 8 * 32;
            int y = _currentFilter / 8 * 32;
            Utility.DrawSelection(g, x, y, 31, 31, true);
            viewPalette.Refresh();
        }
        private void viewPalette_MouseMove(object sender, MouseEventArgs e)
        {
            // Filter the elements with the hovered color
            int color_num = Math.Clamp((e.Y / 32) * 8 + (e.X / 32), 0, 15);
            if(color_num != _currentFilter)
            {
                _currentFilter = color_num;
                _filteredDict = _orgDict.Where(p => (p.Value == _currentFilter)
                                            || ((p.Key >> 4) == _currentFilter)
                                            || ((p.Key & 0x0F) == _currentFilter)
                                            || _currentFilter == 0)
                                      .ToDictionary(p => p.Key, p => p.Value);
                this.UpdatePaletteView();
                this.UpdateColorList();
            }
        }
    }
}
