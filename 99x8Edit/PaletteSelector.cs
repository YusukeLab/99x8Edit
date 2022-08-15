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
        Bitmap bmpOrg;
        Bitmap bmpCur;
        Point current;
        public PaletteSelector(Bitmap pict, int previous, Action<int> callback)
        {
            InitializeComponent();
            bmpOrg = pict;
            viewPlt.Image = bmpCur = (Bitmap)bmpOrg.Clone();
            current.X = previous % 8;
            current.Y = previous / 8;
            colorSelected = callback;
            this.UpdateView();
        }
        private void PaletteSelector_Deactivate(object sender, EventArgs e)
        {
            this.Dispose();
        }
        private void pictureBox_Click(object sender, MouseEventArgs e)
        {
            int clicked_color_num = (e.Y / 32) * 8 + (e.X / 32);
            colorSelected?.Invoke(clicked_color_num);
            this.Dispose();
        }
        private void PaletteSelector_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Up:
                    current.Y = Math.Max(current.Y - 1, 0);
                    break;
                case Keys.Down:
                    current.Y = Math.Min(current.Y + 1, 1);
                    break;
                case Keys.Left:
                    current.X = Math.Max(current.X - 1, 0);
                    break;
                case Keys.Right:
                    current.X = Math.Min(current.X + 1, 7);
                    break;
                case Keys.Space:
                case Keys.Enter:
                    colorSelected(current.Y * 8 + current.X);
                    this.Dispose();
                    break;
                case Keys.Escape:
                    this.Dispose();
                    break;
            }
            this.UpdateView();
        }
        private void UpdateView()
        {
            Graphics g = Graphics.FromImage(bmpCur);
            g.DrawImage(bmpOrg, 0, 0, viewPlt.Width, viewPlt.Height);
            int x = current.X * 32;
            int y = current.Y * 32;
            Utility.DrawSelection(g, x, y, 31, 31, true);
            viewPlt.Refresh();
        }
    }
}
