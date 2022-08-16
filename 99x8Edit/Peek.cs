using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace _99x8Edit
{
    // Binary file viewer window
    public partial class Peek : Form
    {
        private BinaryReader reader = null;
        private long seekAddr = 0;
        private Bitmap bmpPeek = new Bitmap(512, 512);
        private Selection current = new Selection(32, 32);
        enum PeekType
        {
            Linear = 0,
            Sprite,
        }
        private PeekType type = PeekType.Linear;
        // For internal drag control
        private class DnDPeek { }
        //----------------------------------------------------------------------
        // Initialize
        public Peek(String filename)
        {
            InitializeComponent();
            reader = new BinaryReader(new FileStream(filename, FileMode.Open));
            Text = "Peek - " + Path.GetFileName(filename);
            viewPeek.Image = bmpPeek;
            toolStripCopy.Click += new EventHandler(contextPeek_copy);
            RefreshAllViews();
        }
        private void Peek_FormClosing(object sender, FormClosingEventArgs e)
        {
            reader.Close();
            reader = null;
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
            btnLinear.Checked = (type == PeekType.Linear);
            btnSprites.Checked = (type == PeekType.Sprite);
        }
        private void UpdatePeek()
        {
            long length_left = reader.BaseStream.Length - seekAddr;
            Graphics g = Graphics.FromImage(bmpPeek);
            g.Clear(Color.Gray);
            reader.BaseStream.Seek(seekAddr, SeekOrigin.Begin);
            Brush b = new SolidBrush(Color.Black);
            for(int i = 0; i < 8192 && i < length_left; ++i)
            {
                // Address to col, row
                byte dat = reader.ReadByte();
                int chr_x = this.AddressToCol(seekAddr + i);
                int chr_y = this.AddressToRow(seekAddr + i);                
                int pix_y = i % 8;      // y coorinate in row
                for(int j = 0; j < 8; ++j)
                {
                    int bit = (dat >> (7 - j)) & 1;
                    if(bit != 0)
                    {
                        g.FillRectangle(b, chr_x * 16 + j * 2, chr_y * 16 + pix_y * 2, 2, 2);
                    }
                }
            }
            Rectangle r = current.Selected;
            r.Width++;      // Selection is by two cells
            r.Height++;
            Utility.DrawSelection(g, r.X * 16, r.Y * 16, r.Width * 16 - 1, r.Height * 16 - 1, true);
            viewPeek.Refresh();
        }
        private void UpdateAddr()
        {
            btnUp.Enabled = (seekAddr > 0);
            btnDown.Enabled = (seekAddr < reader.BaseStream.Length - 2);
            {
                btnUp.Enabled = true;
            }
            txtAddr.Text = seekAddr.ToString("x6");
        }
        //----------------------------------------------------------------------
        // Controls
        private void viewPeek_MouseDown(object sender, MouseEventArgs e)
        {
            panelPeek.Focus();  // Key events are handled by parent panel
            if (e.Button == MouseButtons.Left)
            {
                int col = e.X / 16;
                int row = e.Y / 16;
                if((col != current.X) || (row != current.Y))
                {
                    if (Control.ModifierKeys == Keys.Shift)
                    {
                        // Limit multiple selections to 16x16 unit
                        int col_sel = (col - current.X % 2) / 2 * 2 + (current.X % 2);
                        int row_sel = (row - current.Y % 2) / 2 * 2 + (current.Y % 2);
                        if ((col_sel <= 30) && (row_sel <= 30))
                        {
                            current.ToX = col_sel;
                            current.ToY = row_sel;
                        }
                    }
                    else
                    {
                        // New selection
                        current.X = current.ToX = Math.Min(e.X / 16, 30);
                        current.Y = current.ToY = Math.Min(e.Y / 16, 30);
                    }
                    this.UpdatePeek();
                    viewPeek.DoDragDrop(new DnDPeek(), DragDropEffects.Copy);
                }
            }
        }
        private void contextPeek_copy(object sender, EventArgs e)
        {
            ClipPeekedData clip = new ClipPeekedData();
            // Copy selected sprites
            Rectangle r = current.Selected;
            r.Width++;      // Selection is by two cells
            r.Height++;
            for (int i = r.Y; i < r.Y + r.Height; ++i)
            {
                List<byte[]> l = new List<byte[]>();
                for (int j = r.X; j < r.X + r.Width; ++j)
                {
                    byte[] one_cell = new byte[8];
                    long addr = this.ColRowToAddr(j, i);
                    if (addr <= reader.BaseStream.Length - 8)
                    {
                        reader.BaseStream.Seek(addr, SeekOrigin.Begin);
                        for (int line = 0; line < 8; ++line)
                        {
                            one_cell[line] = reader.ReadByte();
                        }
                    }
                    l.Add(one_cell);
                }
                clip.peeked.Add(l);
            }
            ClipboardWrapper.SetData(clip);
        }
        private void panelPeek_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Up | Keys.Shift:
                    if (current.ToY >= 2)
                    {
                        current.ToY -= 2;
                        this.UpdatePeek();
                    }
                    break;
                case Keys.Down | Keys.Shift:
                    if (current.ToY <= 28)
                    {
                        current.ToY += 2;
                        this.UpdatePeek();
                    }
                    break;
                case Keys.Left | Keys.Shift:
                    if (current.ToX >= 2)
                    {
                        current.ToX -= 2;
                        this.UpdatePeek();
                    }
                    break;
                case Keys.Right | Keys.Shift:
                    if (current.ToX <= 28)
                    {
                        current.ToX += 2;
                        this.UpdatePeek();
                    }
                    break;
                case Keys.Up:
                    if (current.Y > 0)
                    {
                        current.Y--;
                        this.UpdatePeek();
                    }
                    else if(seekAddr > 0)
                    {
                        seekAddr -= 32 * 8 * 2;
                        if (seekAddr < 0) seekAddr = 0;
                        this.RefreshAllViews();
                    }
                    break;
                case Keys.Down:
                    if (current.Y < 30)
                    {
                        current.Y++;
                        this.UpdatePeek();
                    }
                    else if (seekAddr + 8192 < reader.BaseStream.Length)
                    {
                        seekAddr += 32 * 8 * 2;
                        if (seekAddr + 8192 >= reader.BaseStream.Length)
                        {
                            seekAddr = reader.BaseStream.Length - 8192;
                        }
                        this.RefreshAllViews();
                    }
                    break;
                case Keys.Left:
                    if (current.X > 0)
                    {
                        current.X--;
                        this.UpdatePeek();
                    }
                    break;
                case Keys.Right:
                    if (current.X < 30)
                    {
                        current.X++;
                        this.UpdatePeek();
                    }
                    break;
            }
        }
        private void panelPeek_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DnDPeek)))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else e.Effect = DragDropEffects.None;
        }
        private void panelPeek_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DnDPeek)))
            {
                Point p = viewPeek.PointToClient(Cursor.Position);
                // Limit multiple selections to 16x16 unit
                int col = Math.Min(p.X / 16, 31);
                int row = Math.Min(p.Y / 16, 31);
                int col_sel = (col - current.X % 2) / 2 * 2 + (current.X % 2);
                int row_sel = (row - current.Y % 2) / 2 * 2 + (current.Y % 2);
                if ((col_sel <= 30) && (row_sel <= 30))
                {
                    current.ToX = col_sel;
                    current.ToY = row_sel;
                    this.RefreshAllViews();
                }
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
                if((input_addr > 0) && (input_addr < reader.BaseStream.Length))
                {
                    validated_addr = input_addr;
                }
            }
            txtAddr.Text = validated_addr.ToString("x6");
            seekAddr = validated_addr;
            this.RefreshAllViews();
        }
        private void btnUp_Click(object sender, EventArgs e)
        {
            seekAddr = Math.Max(seekAddr - 8192, 0);
            this.RefreshAllViews();
        }
        private void btnDown_Click(object sender, EventArgs e)
        {
            seekAddr = Math.Min(seekAddr + 8192, reader.BaseStream.Length - 8192);
            this.RefreshAllViews();
        }
        private void btnLinear_Click(object sender, EventArgs e)
        {
            type = PeekType.Linear;
            btnSprites.Checked = false;
            this.RefreshAllViews();
        }
        private void btnSprites_Click(object sender, EventArgs e)
        {
            type = PeekType.Sprite;
            btnLinear.Checked = false;
            this.RefreshAllViews();
        }
        //----------------------------------------------------------------------
        // Utilities
        private long ColRowToAddr(int col, int row)
        {
            if (type == PeekType.Linear)
            {
                // 1   2   3   4   5...    ...31
                // 32  33  34...           ...63
                int addr_in_screen = row * 8 * 32 + col * 8;
                return (long)addr_in_screen + seekAddr;
            }
            else
            {
                // 1   3   5   7   9...    ...62
                // 2   4   6   8   10...   ...63
                int base_addr_of_2rows = row * 64 * 8;
                int addr_in_screen = base_addr_of_2rows + col * 16 + (row % 2) * 8;
                return (long)addr_in_screen + seekAddr;
            }
        }
        private int AddressToCol(long addr)
        {
            addr -= seekAddr;
            if (type == PeekType.Linear)
            {
                // 1   2   3   4   5...    ...31
                // 32  33  34...           ...63
                return (int)((addr / 8) % 32);
            }
            else
            {
                // 1   3   5   7   9...    ...62
                // 2   4   6   8   10...   ...63
                int block_x = ((int)addr / 32) % 16;         // block for each 4 characters
                return block_x * 2 + ((int)addr / 16) % 2;
            }
        }
        private int AddressToRow(long addr)
        {
            addr -= seekAddr;
            if (type == PeekType.Linear)
            {
                // 1   2   3   4   5...    ...31
                // 32  33  34...           ...63
                return (int)(addr / 256);
            }
            else
            {
                // 1   3   5   7   9...    ...62
                // 2   4   6   8   10...   ...63
                int block_y = (int)addr / 512;
                return block_y * 2 + ((int)addr / 8) % 2;
            }
        }
    }
}
