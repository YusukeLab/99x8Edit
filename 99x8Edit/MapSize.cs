using System;
using System.Windows.Forms;

namespace _99x8Edit
{
    // Map size dialog
    public partial class MapSize : Form
    {
        private int _width;
        private int _height;
        public MapSize(int w, int h)
        {
            InitializeComponent();
            txtW.Text = (_width = w).ToString();
            txtH.Text = (_height = h).ToString();
        }
        public int MapWidth => _width;
        public int MapHeight => _height;
        // Handle leave events of textboxes to limit the range of numbers
        private void txtW_Leave(object sender, EventArgs e)
        {
            int.TryParse(txtW.Text, out int val);
            val = Math.Clamp(val, 16, 128);
            txtW.Text = (_width = val).ToString();
        }
        private void txtH_Leave(object sender, EventArgs e)
        {
            int.TryParse(txtH.Text, out int val);
            val = Math.Clamp(val, 12, 128);
            txtH.Text = (_height = val).ToString();
        }
    }
}
