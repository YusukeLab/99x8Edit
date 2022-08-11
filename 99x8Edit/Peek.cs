using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace _99x8Edit
{
    // Binary file viewer window
    public partial class Peek : Form
    {
        private BinaryReader reader = null;
        private long seekAddr = 0;
        private Bitmap bmpPeek = new Bitmap(512, 512);
        private int currentCol = 0;
        private int currentRow = 0;
        enum PeekType
        {
            Linear = 0,
            Sprite,
        }
        private PeekType type = PeekType.Linear;
        //----------------------------------------------------------------------
        // Initialize
        public Peek(String filename)
        {
            InitializeComponent();
            reader = new BinaryReader(new FileStream(filename, FileMode.Open));
            this.Text = "Peek - " + Path.GetFileName(filename);
            this.viewPeek.Image = bmpPeek;
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
                int chr_x = this.addressToCol(i);
                int chr_y = this.addressToRow(i);                
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
            g.DrawRectangle(new Pen(Color.Red), currentCol * 16, currentRow * 16, 31, 31);
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
            currentCol = e.X / 16;
            if (currentCol >= 31) currentCol = 30;
            currentRow = e.Y / 16;
            if (currentRow >= 31) currentRow = 31;
            this.UpdatePeek();
        }
        private void contextPeek_copy(object sender, EventArgs e)
        {
            ClipOneChrInRom clip = new ClipOneChrInRom();
            // Left top
            int addr = this.colRowToAddr(currentCol, currentRow);
            reader.BaseStream.Seek(addr, SeekOrigin.Begin);
            for (int i = 0; i < 8; ++i)
            {
                clip.leftTop[i] = reader.ReadByte();
            }
            // Left bottom
            addr = this.colRowToAddr(currentCol, currentRow + 1);
            reader.BaseStream.Seek(addr, SeekOrigin.Begin);
            for (int i = 0; i < 8; ++i)
            {
                clip.leftBottom[i] = reader.ReadByte();
            }
            // Right top
            addr = this.colRowToAddr(currentCol + 1, currentRow);
            reader.BaseStream.Seek(addr, SeekOrigin.Begin);
            for (int i = 0; i < 8; ++i)
            {
                clip.rightTop[i] = reader.ReadByte();
            }
            // Right bottom
            addr = this.colRowToAddr(currentCol + 1, currentRow + 1);
            reader.BaseStream.Seek(addr, SeekOrigin.Begin);
            for (int i = 0; i < 8; ++i)
            {
                clip.rightBottom[i] = reader.ReadByte();
            }
            ClipboardWrapper.SetData(clip);
        }
        private void panelPeek_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Up:
                    if (currentRow > 0)
                    {
                        currentRow--;
                        this.UpdatePeek();
                    }
                    else if(seekAddr > 0)
                    {
                        seekAddr -= 32 * 8 * 2;
                        if (seekAddr < 0) seekAddr = 0;
                        this.RefreshAllViews();
                    }
                    break;
                case Keys.Left:
                    if (currentCol > 0)
                    {
                        currentCol--;
                        this.UpdatePeek();
                    }
                    break;
                case Keys.Right:
                    if (currentCol < 30)
                    {
                        currentCol++;
                        this.UpdatePeek();
                    }
                    break;
                case Keys.Down:
                    if (currentRow < 30)
                    {
                        currentRow++;
                        this.UpdatePeek();
                    }
                    else if (seekAddr + 8192 < reader.BaseStream.Length)
                    {
                        seekAddr += 32 * 8 * 2;
                        if(seekAddr + 8192 >= reader.BaseStream.Length)
                        {
                            seekAddr = reader.BaseStream.Length - 8192;
                        }
                        this.RefreshAllViews();
                    }
                    break;
            }
        }
        private void txtAddr_Leave(object sender, EventArgs e)
        {
            // Address edited
            long input_addr = 0;
            long validated_addr = 0;
            if(long.TryParse(txtAddr.Text, System.Globalization.NumberStyles.HexNumber, null, out input_addr))
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
            seekAddr -= 8192;
            if(seekAddr < 0)
            {
                seekAddr = 0;
            }
            this.RefreshAllViews();
        }
        private void btnDown_Click(object sender, EventArgs e)
        {
            seekAddr += 8192;
            if(seekAddr >= reader.BaseStream.Length)
            {
                seekAddr = reader.BaseStream.Length - 8192;
                if(seekAddr < 0)
                {
                    seekAddr = 0;
                }
            }
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
        private int colRowToAddr(int col, int row)
        {
            if (type == PeekType.Linear)
            {
                // 1   2   3   4   5...    ...31
                // 32  33  34...           ...63
                return row * 8 * 32 + col * 8;
            }
            else
            {
                // 1   3   5   7   9...    ...62
                // 2   4   6   8   10...   ...63
                int base_addr_of_2rows = row * 64 * 8;
                int addr = base_addr_of_2rows + col * 16 + (row % 2) * 8;
                return addr;
            }
        }
        private int addressToCol(int addr)
        {
            if (type == PeekType.Linear)
            {
                // 1   2   3   4   5...    ...31
                // 32  33  34...           ...63
                return (addr / 8) % 32;
            }
            else
            {
                // 1   3   5   7   9...    ...62
                // 2   4   6   8   10...   ...63
                int block_x = (addr / 32) % 16;         // block for each 4 characters
                return block_x * 2 + (addr / 16) % 2;
            }
        }
        private int addressToRow(int addr)
        {
            if (type == PeekType.Linear)
            {
                // 1   2   3   4   5...    ...31
                // 32  33  34...           ...63
                return addr / 256;
            }
            else
            {
                // 1   3   5   7   9...    ...62
                // 2   4   6   8   10...   ...63
                int block_y = addr / 512;
                return block_y * 2 + (addr / 8) % 2;
            }
        }
    }
}
