using System;
using System.Drawing;
using System.Windows.Forms;

namespace _99x8Edit
{
    // RGB888 Palette editor window
    public partial class PaletteEditor : Form
    {
        private readonly Action<int, int, int> _paletteEdited;
        public PaletteEditor(int r, int g, int b, Action<int, int, int> callback)
        {
            InitializeComponent();
            _textBoxR.Text = r.ToString();
            _textBoxG.Text = g.ToString();
            _textBoxB.Text = b.ToString();
            _trackBarR.Value = r;
            _trackBarG.Value = g;
            _trackBarB.Value = b;
            _paletteEdited = callback;
            this.UpdateColor();
        }
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
            Color c = Color.FromArgb((_trackBarR.Value * 255) / 7,
                                     (_trackBarG.Value * 255) / 7,
                                     (_trackBarB.Value * 255) / 7);
            _pictColor.BackColor = c;
        }
        private void textBoxR_Leave(object sender, EventArgs e)
        {
            int.TryParse(_textBoxR.Text, out int val);
            val = Math.Clamp(val, 0, 7);
            _textBoxR.Text = val.ToString();
            _trackBarR.Value = val;
            this.UpdateColor();
        }
        private void textBoxG_Leave(object sender, EventArgs e)
        {
            int.TryParse(_textBoxG.Text, out int val);
            val = Math.Clamp(val, 0, 7);
            _textBoxG.Text = val.ToString();
            _trackBarG.Value = val;
            this.UpdateColor();
        }
        private void textBoxB_Leave(object sender, EventArgs e)
        {
            int.TryParse(_textBoxB.Text, out int val);
            val = Math.Clamp(val, 0, 7);
            _textBoxB.Text = val.ToString();
            _trackBarB.Value = val;
            this.UpdateColor();
        }
        private void trackBarR_ValueChanged(object sender, EventArgs e)
        {
            _textBoxR.Text = _trackBarR.Value.ToString();
            this.UpdateColor();
        }
        private void trackBarG_ValueChanged(object sender, EventArgs e)
        {
            _textBoxG.Text = _trackBarG.Value.ToString();
            this.UpdateColor();
        }
        private void trackBarB_ValueChanged(object sender, EventArgs e)
        {
            _textBoxB.Text = _trackBarB.Value.ToString();
            this.UpdateColor();
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            _paletteEdited?.Invoke(_trackBarR.Value,
                                   _trackBarG.Value,
                                   _trackBarB.Value);
            this.Dispose();
        }
    }
}
