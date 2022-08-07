using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace _99x8Edit
{
    public partial class PaletteEditor : Form
    {
        public PaletteEditor(int R, int G, int B)
        {
            InitializeComponent();
            textBoxR.Text = R.ToString();
            textBoxG.Text = G.ToString();
            textBoxB.Text = B.ToString();
            trackBarR.Value = R;
            trackBarG.Value = G;
            trackBarB.Value = B;
            this.UpdateColor();
        }
        public int R
        {
            get
            {
                return trackBarR.Value;
            }
        }
        public int G
        {
            get
            {
                return trackBarG.Value;
            }
        }
        public int B
        {
            get
            {
                return trackBarB.Value;
            }
        }
        private void UpdateColor()
        {
            Color c = Color.FromArgb((trackBarR.Value * 255) / 7, (trackBarG.Value * 255) / 7, (trackBarB.Value * 255) / 7);
            pictColor.BackColor = c;
        }
        private void textBoxR_Leave(object sender, EventArgs e)
        {
            int val = 0;
            if (int.TryParse(textBoxR.Text, out val))
            {
                if (val > 7) val = 7;
            }
            textBoxR.Text = val.ToString();
            trackBarR.Value = val;
            this.UpdateColor();
        }
        private void textBoxG_Leave(object sender, EventArgs e)
        {
            int val = 0;
            if (int.TryParse(textBoxG.Text, out val))
            {
                if (val > 7) val = 7;
            }
            textBoxG.Text = val.ToString();
            trackBarG.Value = val;
            this.UpdateColor();
        }
        private void textBoxB_Leave(object sender, EventArgs e)
        {
            int val = 0;
            if (int.TryParse(textBoxB.Text, out val))
            {
                if (val > 7) val = 7;
            }
            textBoxB.Text = val.ToString();
            trackBarB.Value = val;
            this.UpdateColor();
        }
        private void trackBarR_ValueChanged(object sender, EventArgs e)
        {
            textBoxR.Text = trackBarR.Value.ToString();
            this.UpdateColor();
        }
        private void trackBarG_ValueChanged(object sender, EventArgs e)
        {
            textBoxG.Text = trackBarG.Value.ToString();
            this.UpdateColor();
        }
        private void trackBarB_ValueChanged(object sender, EventArgs e)
        {
            textBoxB.Text = trackBarB.Value.ToString();
            this.UpdateColor();
        }
    }
}
