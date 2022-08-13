using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Linq;

namespace _99x8Edit
{
    // Importing data
    public class Import
    {
        // VRAMs and so on
        internal byte[] ptnGen = new byte[256 * 8];    // Pattern generator table
        internal byte[] ptnClr = new byte[256 * 8];    // Pattern color table
        internal byte[] nameTable = new byte[768];     // Sandbox(Pattern name table)
        internal byte[] pltDat = { 0x00, 0x00, 0x00, 0x00, 0x11, 0x06, 0x33, 0x07,
                                  0x17, 0x01, 0x27, 0x03, 0x51, 0x01, 0x27, 0x06,
                                  0x71, 0x01, 0x73, 0x03, 0x61, 0x06, 0x64, 0x06,
                                  0x11, 0x04, 0x65, 0x02, 0x55, 0x05, 0x77, 0x07};  // Palette [RB][xG][RB][xG][RB]...
        internal bool isTMS9918 = false;
        internal byte[] mapPattern = new byte[256 * 4];  // One pattern mede by four characters
        internal byte[,] mapData = new byte[64, 64];     // Map data[x, y](0..255)
        internal Int32 mapWidth = 64;
        internal Int32 mapHeight = 64;
        internal byte[] spriteGen = new byte[256 * 8];   // Sprite pattern generator table
        internal byte[] spriteClr1 = new byte[256];      // Sprite color(mode1)
        internal byte[] spriteClr2 = new byte[256 * 8];  // Sprite color(mode2)
        internal byte[] spriteOverlay = new byte[64];    // Will overlay next sprite(1) or not(0)
        // Palette
        private Color[] palette = new Color[16];         // Windows color corresponding to color code

        //------------------------------------------------------------------------
        // Initialize
        public Import()
        {
        }
        //------------------------------------------------------------------------
        // Properties
        internal Color[] Palette
        {
            get { return palette; }
            set { palette = value.Clone() as Color[]; }
        }
        //------------------------------------------------------------------------
        // Methods
        internal void OpenPCG(string filename)
        {
            string ext = Path.GetExtension(filename);
            if(ext == ".png")
            {
                this.PNGtoPCG(filename);
            }
        }
        //------------------------------------------------------------------------
        // Utility
        private void PNGtoPCG(string filename)
        {
            Bitmap bmp = (Bitmap)Image.FromFile(filename);
            if(bmp == null)
            {
                return;
            }
            int w = Math.Min(bmp.Width, 256);
            int h = Math.Min(bmp.Height, 64);
            for(int col = 0; col < w / 8; ++col)
            {
                for(int row = 0; row < h / 8; ++row)
                {
                    // Each line in each characters
                    for(int line = 0; line < 8; ++line)
                    {
                        int[] nearest = new int[8]; // Pixel of each line to color code
                        int[] count = new int[16];  // How mane times each color appeared
                        for(int x = 0; x < 8; ++x)
                        {
                            Color c = bmp.GetPixel(col * 8 + x, row * 8 + line);
                            nearest[x] = this.NearestColorCodeOf(c);
                            count[nearest[x]]++;
                        }
                        // Get the frequent color
                        int max1_num = count.Max();
                        int max1_index = Array.IndexOf(count, max1_num);
                        count[max1_index] = 0;
                        int max2_num = count.Max();
                        int max2_index = Array.IndexOf(count, max2_num);
                        // Color table of the line will be made of two frequent colors
                        ptnClr[row * 256 + col * 8 + line] = (byte)((max1_index << 4) | max2_index);
                        // Make pattern generator table
                        for(int x = 0; x < 8; ++x)
                        {
                            Color c = bmp.GetPixel(col * 8 + x, row * 8 + line);
                            int code = this.WhichColorCodeIsNear(c, max1_index, max2_index);
                            if(code == max1_index)
                            {
                                // Foreground color is near, so set the corresponding bit
                                ptnGen[row * 256 + col * 8 + line] |= (byte)(1 << (7 - x));
                            }
                            // Ignore nackground color since the corresponding value is 0
                        }
                    }
                }
            }
        }
        private int NearestColorCodeOf(Color c)
        {
            int[] distance = new int[16];
            for(int i = 0; i < 16; ++i)
            {
                int d = Math.Abs(c.R - palette[i].R)
                      + Math.Abs(c.G - palette[i].G)
                      + Math.Abs(c.B - palette[i].B);
                distance[i] = d;
            }
            return Array.IndexOf(distance, distance.Min());
        }
        private int WhichColorCodeIsNear(Color c, int code1, int code2)
        {
            int d1 = Math.Abs(c.R - palette[code1].R)
                   + Math.Abs(c.G - palette[code1].G)
                   + Math.Abs(c.B - palette[code1].B);
            int d2 = Math.Abs(c.R - palette[code2].R)
                   + Math.Abs(c.G - palette[code2].G)
                   + Math.Abs(c.B - palette[code2].B);
            if(d1 < d2)
            {
                return code1;
            }
            return code2;
        }
    }
}
