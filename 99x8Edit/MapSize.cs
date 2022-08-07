using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace _99x8Edit
{
    public partial class MapSize : Form
    {
        private int width;
        private int height;
        public MapSize(int w, int h)
        {
            InitializeComponent();
            txtW.Text = (width = w).ToString();
            txtH.Text = (height = h).ToString();
        }
        public int MapWidth
        {
            get
            {
                return width;
            }
        }
        public int MapHeight
        {
            get
            {
                return height;
            }
        }
        private void txtW_Leave(object sender, EventArgs e)
        {
            int val = 0;
            int.TryParse(txtW.Text, out val);
            if (val < 16) val = 16;
            if (val > 128) val = 128;
            txtW.Text = (width = val).ToString();
        }
        private void txtH_Leave(object sender, EventArgs e)
        {
            int val = 0;
            int.TryParse(txtH.Text, out val);
            if (val < 12) val = 12;
            if (val > 128) val = 128;
            txtH.Text = (height = val).ToString();
        }
    }
}
