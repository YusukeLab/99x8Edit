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
        Action<int> colorSelected;
        public PaletteSelector(Bitmap pict, Action<int> callback)
        {
            InitializeComponent();
            pictureBox.Image = pict;
            colorSelected = callback;
            //functionColorSelected = callback;
        }
        private void PaletteSelector_Deactivate(object sender, EventArgs e)
        {
            this.Dispose();
        }
        private void pictureBox_Click(object sender, MouseEventArgs e)
        {
            int clicked_color_num = (e.Y / 32) * 8 + (e.X / 32);
            colorSelected(clicked_color_num);
            this.Dispose();
        }
    }
}
