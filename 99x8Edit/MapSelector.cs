using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace _99x8Edit
{
    public partial class MapSelector : Form
    {
        private readonly Machine _dataSource;
        private readonly Action<int, int> _changed;
        //------------------------------------------------------------------------------
        // Initialize
        public MapSelector(Machine src, int x, int y, Action<int, int> changed)
        {
            _dataSource = src;
            _changed = changed;
            // Initialization
            InitializeComponent();
            // Views
            _txtW.Text = (_view.X = x).ToString();
            _txtH.Text = (_view.Y = y).ToString();
            this.RefreshAllViews();
            // Context menu
            _menuCopy.Click += _contextCopy;
            _menuPaste.Click += _contextPaste;
        }
        //------------------------------------------------------------------------------
        // Override
        protected override bool ProcessDialogKey(Keys keyData)
        {
            switch (keyData)
            {
                // prevent focus movement by the cursor and catch cursor keys
                case Keys.Down:
                case Keys.Right:
                case Keys.Up:
                case Keys.Left:
                case Keys.Down | Keys.Shift:
                case Keys.Right | Keys.Shift:
                case Keys.Up | Keys.Shift:
                case Keys.Left | Keys.Shift:
                    break;
                default:
                    return base.ProcessDialogKey(keyData);
            }

            return true;
        }
        //------------------------------------------------------------------------------
        // Views
        private void RefreshAllViews()
        {
            _txtW.Text = _dataSource.NameTableMapW.ToString();
            _txtH.Text = _dataSource.NameTableMapH.ToString();
            _view.ColumnNum = _dataSource.NameTableMapW;
            _view.RowNum = _dataSource.NameTableMapH;
            for (int row = 0; row < _dataSource.NameTableMapH; ++row)
            {
                for (int col = 0; col < _dataSource.NameTableMapW; ++col)
                {
                    _view.SetBackgroundColor(Color.Gray, col, row);
                }
            }
            _view.Refresh();
        }
        //------------------------------------------------------------------------------
        // Controls
        private void _txtW_Leave(object sender, EventArgs e)
        {
            int.TryParse(_txtW.Text, out int w);
            w = Math.Clamp(w, 1, 16);
            _dataSource.NameTableMapW = w;
            _txtW.Text = w.ToString();
            _view.X = Math.Min(_view.X, w - 1);
            // Update parent view
            _changed?.Invoke(_view.X, _view.Y);
            // Update myself
            this.RefreshAllViews();
        }
        private void _txtH_Leave(object sender, EventArgs e)
        {
            int.TryParse(_txtH.Text, out int h);
            h = Math.Clamp(h, 1, 16);
            _dataSource.NameTableMapH = h;
            _txtH.Text = h.ToString();
            _view.Y = Math.Min(_view.Y, h - 1);
            // Update parent view
            _changed?.Invoke(_view.X, _view.Y);
            // Update myself
            this.RefreshAllViews();
        }
        private void _view_SelectionChanged(object sender, EventArgs e)
        {
            // Update parent view
            _changed?.Invoke(_view.X, _view.Y);
            // Update myself
            this.RefreshAllViews();
        }
        private void _contextCopy(object sender, EventArgs e)
        {
            var clip = new ClipNametableBanks();
            Rectangle r = _view.SelectedRect;
            for (int row = r.Y; row < r.Y + r.Height; ++row)
            {
                List<byte[]> l = new List<byte[]>();
                for (int col = r.X; col < r.X + r.Width; ++col)
                {
                    var one_bank = _dataSource.GetNameTableBank(col, row);
                    l.Add(one_bank.ToArray());
                }

                clip.banks.Add(l);
            }
            ClipboardWrapper.SetData(clip);
        }
        private void _contextPaste(object sender, EventArgs e)
        {
            dynamic clip = ClipboardWrapper.GetData();
            switch (clip)
            {
                case ClipNametableBanks cbanks:
                    void paste_one_bank(int col, int row, int colcnt, int rowcnt)
                    {
                        // Paste each copied lines
                        byte[] data = cbanks.banks[rowcnt][colcnt];
                        _dataSource.SetNameTableBank(col, row, data, false);
                    }
                    MementoCaretaker.Instance.Push();
                    _view.ForEachSelection(_view.X, _view.Y,
                                           cbanks.banks[0].Count, cbanks.banks.Count,
                                           paste_one_bank);
                    // Update parent view
                    _changed?.Invoke(_view.X, _view.Y);
                    // Update myself
                    this.RefreshAllViews();
                    break;
            }
        }
    }
}
