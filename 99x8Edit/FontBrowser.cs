using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

namespace _99x8Edit
{
    public partial class FontBrowser : Form
    {
        // Font browsing window
        private readonly Machine _dataSource;
        private readonly int[] _colorCode = new int[8];
        private Bitmap[,] _bmps;
        //----------------------------------------------------------------------
        // Initialize
        public FontBrowser(Machine source)
        {
            _dataSource = source;
            InitializeComponent();
            // Context menu
            toolStripCopy.Click += contextPCG_copy;
            // Set color code of rows
            for (int i = 0; i < 8; ++i)
            {
                _colorCode[i] = 15;
            }
            // Acquire installed fonts
            InstalledFontCollection ifc = new InstalledFontCollection();
            FontFamily[] ffs = ifc.Families;
            foreach (FontFamily ff in ffs)
            {
                if (ff.IsStyleAvailable(FontStyle.Regular))
                {
                    Font fnt = new Font(ff, 8);
                    _fontList.Items.Add(fnt.Name);
                }
            }
            _fontList.SelectedIndex = 0;
            this.RefreshAllViews();
        }
        //------------------------------------------------------------------------------
        // Views
        private void RefreshAllViews()
        {
            this.UpdateColor(refresh: false);
            this.UpdateActualView(refresh: false);
            _colorMatrix.Refresh();
            _actualView.Refresh();
            _colorPreview.Refresh();
        }
        private void UpdateColor(bool refresh)
        {
            // Update color view matrix
            for (int y = 0; y < 8; ++y)
            {
                Color c = _dataSource.ColorOf(_colorCode[y]);
                _colorMatrix.SetBackgroundColor(c, 0, y);
            }
            // Update preview
            byte[] chr = new byte[]
            {
                0b00111110,
                0b01000001,
                0b01000001,
                0b01000001,
                0b01111111,
                0b01000001,
                0b01000001,
                0b01000001,
            };
            for (int y = 0; y < 8; ++y)
            {
                Brush b = _dataSource.BrushOf(_colorCode[y]);
                for (int x = 0; x < 8; ++x)
                {
                    int bit = ((chr[y] >> (7 - x)) & 1);
                    if (bit != 0)
                    {
                        _colorPreview.SetEditingDot(b, x, y);
                    }
                }
            }
            if (refresh)
            {
                _colorMatrix.Refresh();
                _colorPreview.Refresh();
            }
        }
        private void UpdateActualView(bool refresh)
        {
            _bmps ??= new Bitmap[_actualView.ColumnNum, _actualView.RowNum];
            // Text length
            int len = _actualView.ColumnNum * _actualView.RowNum;
            int size = Decimal.ToInt32(_fontSize.Value);
            // Font
            string name = _fontList.SelectedItem.ToString();
            FontStyle style = FontStyle.Regular;
            if (_italic.Checked) style |= FontStyle.Italic;
            if (_bold.Checked) style |= FontStyle.Bold;
            using Font fnt = new Font(name, size, style, GraphicsUnit.Pixel);
            // Region
            Rectangle r = new Rectangle(0, 0, 8, 8);
            // Threshold to draw
            int threshold = 256 - Decimal.ToInt32(_density.Value);
            for (int i = 0; i < len; ++i)
            {
                // Each characters in textbox
                int col = i % _actualView.ColumnNum;
                int row = i / _actualView.ColumnNum;
                Bitmap bmp = (_bmps[col, row] ??= new Bitmap(8, 8));
                _actualView.SetBackgroundColor(Color.Black, col, row);
                if (i >= _textBox.TextLength)
                {
                    _actualView.SetImage(null, col, row);
                    continue;
                }
                // Draw one character
                char chr = _textBox.Text[i];
                Graphics g = Graphics.FromImage(bmp);
                g.Clear(Color.Black);
                TextFormatFlags flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.Bottom;
                TextRenderer.DrawText(g, chr.ToString(), fnt, r, Color.White, flags);
                for (int y = 0; y < 8; ++y)
                {
                    // Force each pixels to selected color
                    Color c = _dataSource.ColorOf(_colorCode[y]);
                    for (int x = 0; x < 8; ++x)
                    {
                        Color pc = bmp.GetPixel(x, y);
                        int brightness = pc.R + pc.G + pc.B;
                        if (brightness >= threshold * 3)
                        {
                            bmp.SetPixel(x, y, c);
                        }
                        else
                        {
                            bmp.SetPixel(x, y, Color.Transparent);
                        }
                    }
                }
                _actualView.SetImage(bmp, col, row);
            }
            if (refresh) _actualView.Refresh();
        }
        //------------------------------------------------------------------------------
        // Controls
        private void _colorMatrix_CellOnEdit(object sender, MatrixControl.EditEventArgs e)
        {
            // Color selection
            int color_index = _colorMatrix.Y;
            int color_code = _colorCode[color_index];
            // Callback from the selector window
            Action<int> callback = (code) =>
            {
                if (code != 0)
                {
                    _colorCode[color_index] = code;
                    this.RefreshAllViews();
                }
            };
            // Open the selector
            PaletteSelector win = new PaletteSelector(_dataSource, color_code, callback);
            win.StartPosition = FormStartPosition.Manual;
            win.Location = Cursor.Position;
            win.Show();
        }
        private void contextPCG_copy(object sender, EventArgs e)
        {
            // Copy selected characters to clipboard
            Rectangle r = _actualView.SelectedRect;
            ClipFont clip = new ClipFont();
            for (int y = r.Y; y < r.Y + r.Height; ++y)
            {
                // For each rows
                List<byte[]> gen_row = new List<byte[]>();
                List<byte[]> clr_row = new List<byte[]>();
                List<int> pcg_row = new List<int>();
                for (int x = r.X; x < r.X + r.Width; ++x)
                {
                    // For each cols
                    Bitmap bmp = _bmps[x, y];
                    // Create generator table and color table of each characters
                    byte[] gen = new byte[8];
                    byte[] color = new byte[8];
                    for (int i = 0; i < 8; ++i)
                    {
                        // For each lines in one character
                        for (int j = 0; j < 8; ++j)
                        {
                            // For each pixels
                            Color c = bmp.GetPixel(j, i);
                            if (c.R > 0)
                            {
                                byte bit = (byte)(1 << (7 - j));
                                gen[i] |= bit;
                            }
                        }
                        color[i] = (byte)(_colorCode[i] << 4);
                    }
                    gen_row.Add(gen);
                    clr_row.Add(color);
                    pcg_row.Add(0);
                }
                clip.pcgGen.Add(gen_row);
                clip.pcgClr.Add(clr_row);
                clip.pcgIndex.Add(pcg_row);
            }
            ClipboardWrapper.SetData(clip);
        }
        private void _fontList_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.RefreshAllViews();
        }
        private void _textBox_TextChanged(object sender, EventArgs e)
        {
            this.RefreshAllViews();
        }
        private void _fontSize_ValueChanged(object sender, EventArgs e)
        {
            this.RefreshAllViews();
        }
        private void _threshold_ValueChanged(object sender, EventArgs e)
        {
            this.RefreshAllViews();
        }
        private void _italic_CheckedChanged(object sender, EventArgs e)
        {
            this.RefreshAllViews();
        }
        private void _bold_CheckedChanged(object sender, EventArgs e)
        {
            this.RefreshAllViews();
        }
    }
}
