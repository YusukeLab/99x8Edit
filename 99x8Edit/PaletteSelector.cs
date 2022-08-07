using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace _99x8Edit
{
    // Color selection window
    public partial class PaletteSelector : Form
    {
        private Func<int, int> functionColorSelected;
        public PaletteSelector(Bitmap pict, Func<int, int>callback)
        {
            InitializeComponent();
            pictureBox.Image = pict;
            functionColorSelected = callback;
        }
        private void PaletteSelector_Deactivate(object sender, EventArgs e)
        {
            this.Dispose();
        }
        private void pictureBox_Click(object sender, MouseEventArgs e)
        {
            int clicked_color_num = (e.Y / 32) * 8 + (e.X / 32);
            functionColorSelected(clicked_color_num);   // Callback
            this.Dispose();
        }
    }
}
