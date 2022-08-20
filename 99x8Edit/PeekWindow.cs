using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace _99x8Edit
{
    // Binary file viewer window
    public partial class PeekWindow : Form
    {
        private BinaryReader _reader = null;
        private long _seekAddr = 0;
        private Bitmap[,] _bmps = new Bitmap[32, 32];
        enum PeekType
        {
            Linear = 0,
            Sprite,
        }
        private PeekType _type = PeekType.Linear;
        // For internal drag control
        private class DnDPeek { }
        //----------------------------------------------------------------------
        // Initialize
        public PeekWindow(string filename)
        {
            InitializeComponent();
            _reader = new BinaryReader(new FileStream(filename, FileMode.Open));
            this.Text = "Peek - " + Path.GetFileName(filename);
            toolStripCopy.Click += new EventHandler(contextPeek_copy);
            RefreshAllViews();
        }
        private void Peek_FormClosing(object sender, FormClosingEventArgs e)
        {
            _reader.Close();
            _reader = null;
        }
        //----------------------------------------------------------------------
        // Overrides
        protected override bool ProcessDialogKey(Keys keyData)
        {
            switch (keyData)
            {
                // prevent focus movement by the cursor
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
        //----------------------------------------------------------------------
        // Refreshing views
        private void RefreshAllViews()
        {
            this.UpdatePeek();
            this.UpdateAddr();
            btnLinear.Checked = (_type == PeekType.Linear);
            btnSprites.Checked = (_type == PeekType.Sprite);
        }
        private void UpdatePeek()
        {
            long length_left = _reader.BaseStream.Length - _seekAddr;
            _reader.BaseStream.Seek(_seekAddr, SeekOrigin.Begin);
            for (int row = 0; row < 32; ++row)
            {
                for(int col = 0; col < 32; ++col)
                {
                    _bmps[col, row] ??= new Bitmap(8, 8);
                    Graphics g = Graphics.FromImage(_bmps[col, row]);
                    g.Clear(Color.Gray);
                    viewPeek.SetImage(_bmps[col, row], col, row);
                }
            }
            for(int i = 0; i < 8192 && i < length_left; ++i)
            {
                // Address to col, row
                byte dat = _reader.ReadByte();
                (int chr_x, int chr_y) = this.AddressToColRow(_seekAddr + i);
                int pix_y = i % 8;          // row in 8x8 character
                for(int j = 0; j < 8; ++j)  // for each columns in one 8x1 line
                {
                    int bit = (dat >> (7 - j)) & 1;
                    if(bit != 0)
                    {
                        Graphics g = Graphics.FromImage(_bmps[chr_x, chr_y]);
                        _bmps[chr_x, chr_y].SetPixel(j, pix_y, Color.Black);
                    }
                }
            }
            viewPeek.Refresh();
        }
        private void UpdateAddr()
        {
            btnUp.Enabled = (_seekAddr > 0);
            btnDown.Enabled = (_seekAddr < _reader.BaseStream.Length - 2);
            {
                btnUp.Enabled = true;
            }
            txtAddr.Text = _seekAddr.ToString("x6");
        }
        //----------------------------------------------------------------------
        // Controls
        private void contextPeek_copy(object sender, EventArgs e)
        {
            ClipPeekedData clip = new ClipPeekedData();
            // Copy selected sprites
            Rectangle r = viewPeek.SelectedRect;
            // 16x16 Selection to 8x8 cell index
            r.X *= 2;
            r.Y *= 2;
            r.Width *= 2;
            r.Height *= 2;
            for (int i = r.Y; i < r.Y + r.Height; ++i)
            {
                List<byte[]> l = new List<byte[]>();
                for (int j = r.X; j < r.X + r.Width; ++j)
                {
                    byte[] one_cell = new byte[8];
                    long addr = this.ColRowToAddr(j, i);
                    if (addr <= _reader.BaseStream.Length - 8)
                    {
                        _reader.BaseStream.Seek(addr, SeekOrigin.Begin);
                        for (int line = 0; line < 8; ++line)
                        {
                            one_cell[line] = _reader.ReadByte();
                        }
                    }
                    l.Add(one_cell);
                }
                clip.peeked.Add(l);
            }
            ClipboardWrapper.SetData(clip);
        }
        private void viewPtn_MatrixOnScroll(object sender, MatrixControl.ScrollEventArgs e)
        {
            int dy = e.DY;
            if((e.DY < 0) && (_seekAddr > 0))
            {
                _seekAddr -= 32 * 8 * 2;
                if (_seekAddr < 0) _seekAddr = 0;
                this.RefreshAllViews();
            }
            if((e.DY > 0) && (_seekAddr + 8192 < _reader.BaseStream.Length))
            {
                _seekAddr += 32 * 8 * 2;
                if (_seekAddr + 8192 >= _reader.BaseStream.Length)
                {
                    _seekAddr = _reader.BaseStream.Length - 8192;
                }
                this.RefreshAllViews();
            }
        }
        private void txtAddr_Leave(object sender, EventArgs e)
        {
            // Address edited
            long input_addr = 0;
            long validated_addr = 0;
            if(long.TryParse(txtAddr.Text,
                             System.Globalization.NumberStyles.HexNumber,
                             null,
                             out input_addr))
            {
                if((input_addr > 0) && (input_addr < _reader.BaseStream.Length))
                {
                    validated_addr = input_addr;
                }
            }
            txtAddr.Text = validated_addr.ToString("x6");
            _seekAddr = validated_addr;
            this.RefreshAllViews();
        }
        private void btnUp_Click(object sender, EventArgs e)
        {
            _seekAddr = Math.Max(_seekAddr - 8192, 0);
            this.RefreshAllViews();
        }
        private void btnDown_Click(object sender, EventArgs e)
        {
            _seekAddr = Math.Min(_seekAddr + 8192, _reader.BaseStream.Length - 8192);
            this.RefreshAllViews();
        }
        private void btnLinear_Click(object sender, EventArgs e)
        {
            _type = PeekType.Linear;
            btnSprites.Checked = false;
            this.RefreshAllViews();
        }
        private void btnSprites_Click(object sender, EventArgs e)
        {
            _type = PeekType.Sprite;
            btnLinear.Checked = false;
            this.RefreshAllViews();
        }
        //----------------------------------------------------------------------
        // Utilities
        private long ColRowToAddr(int col, int row)
        {
            if (_type == PeekType.Linear)
            {
                // 1   2   3   4   5...    ...31
                // 32  33  34...           ...63
                int addr_in_screen = row * 8 * 32 + col * 8;
                return (long)addr_in_screen + _seekAddr;
            }
            else
            {
                // 1   3   5   7   9...    ...62
                // 2   4   6   8   10...   ...63
                int base_addr_of_2rows = row * 64 * 8;
                int addr_in_screen = base_addr_of_2rows + col * 16 + (row % 2) * 8;
                return (long)addr_in_screen + _seekAddr;
            }
        }
        private (int col, int row) AddressToColRow(long addr)
        {
            int col = 0;
            int row = 0;
            addr -= _seekAddr;
            if (_type == PeekType.Linear)
            {
                // 1   2   3   4   5...    ...31
                // 32  33  34...           ...63
                col =  (int)((addr / 8) % 32);
                row = (int)(addr / 256);
            }
            else
            {
                // 1   3   5   7   9...    ...62
                // 2   4   6   8   10...   ...63
                int block_x = ((int)addr / 32) % 16;         // block for each 4 characters
                col = block_x * 2 + ((int)addr / 16) % 2;
                int block_y = (int)addr / 512;
                row = block_y * 2 + ((int)addr / 8) % 2;
            }
            return (col, row);
        }
    }
}
