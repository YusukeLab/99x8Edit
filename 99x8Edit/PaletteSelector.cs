using System;
using System.Drawing;
using System.Windows.Forms;

namespace _99x8Edit
{
    // Color selection window
    public partial class PaletteSelector : Form
    {
        private readonly Action<int> _colorSelected;
        public PaletteSelector(Machine source, int previous, Action<int> callback)
        {
            InitializeComponent();
            for (int i = 1; i < 16; ++i)
            {
                Color c = source.ColorOf(i);
                _viewPalette.SetBackgroundColor(c, i % _viewPalette.ColumnNum,
                                               i / _viewPalette.ColumnNum);
            }
            _viewPalette.Index = previous;
            _colorSelected = callback;
        }
        private void PaletteSelector_Deactivate(object sender, EventArgs e)
        {
            this.Dispose();
        }
        private void viewPalette_CellOnEdit(object sender, MatrixControl.EditEventArgs e)
        {
            int color = _viewPalette.Index;
            _colorSelected?.Invoke(color);
            this.Dispose();
        }
        private void viewPalette_MouseDown(object sender, MouseEventArgs e)
        {
            int color = _viewPalette.Index;
            _colorSelected?.Invoke(color);
            this.Dispose();
        }
    }
}
