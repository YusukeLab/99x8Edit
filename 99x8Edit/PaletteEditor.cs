using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace _99x8Edit
{
    // RGB888 Palette editor window
    public partial class PaletteEditor : Form
    {
        Action _paletteEdited;
        public PaletteEditor(int R, int G, int B, Action callback)
        {
            InitializeComponent();
            textBoxR.Text = R.ToString();
            textBoxG.Text = G.ToString();
            textBoxB.Text = B.ToString();
            trackBarR.Value = R;
            trackBarG.Value = G;
            trackBarB.Value = B;
            _paletteEdited = callback;
            this.UpdateColor();
        }
        public int R => trackBarR.Value;
        public int G => trackBarG.Value;
        public int B => trackBarB.Value;
        protected override bool ProcessDialogKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Escape:
                    this.Dispose();
                    break;
                default:
                    return base.ProcessDialogKey(keyData);
            }
            return true;
        }
        private void PaletteEditor_Deactivate(object sender, EventArgs e)
        {
            this.Dispose();
        }
        private void UpdateColor()
        {
            Color c = Color.FromArgb((trackBarR.Value * 255) / 7,
                                     (trackBarG.Value * 255) / 7,
                                     (trackBarB.Value * 255) / 7);
            pictColor.BackColor = c;
        }
        private void textBoxR_Leave(object sender, EventArgs e)
        {
            int.TryParse(textBoxR.Text, out int val);
            val = Math.Clamp(val, 0, 7);
            textBoxR.Text = val.ToString();
            trackBarR.Value = val;
            this.UpdateColor();
        }
        private void textBoxG_Leave(object sender, EventArgs e)
        {
            int.TryParse(textBoxG.Text, out int val);
            val = Math.Clamp(val, 0, 7);
            textBoxG.Text = val.ToString();
            trackBarG.Value = val;
            this.UpdateColor();
        }
        private void textBoxB_Leave(object sender, EventArgs e)
        {
            int.TryParse(textBoxB.Text, out int val);
            val = Math.Clamp(val, 0, 7);
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
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            _paletteEdited?.Invoke();
            this.Dispose();
        }
    }
}
