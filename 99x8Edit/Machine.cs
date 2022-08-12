using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace _99x8Edit
{
    // All VDP related data will be wrapped here
    public partial class Machine
    {
        // Data, of PCG
        private byte[] ptnGen = new byte[256 * 8];    // Pattern generator table
        private byte[] ptnClr = new byte[256 * 8];    // Pattern color table
        private byte[] nameTable = new byte[768];     // Sandbox(Pattern name table)
        private byte[] pltDat = { 0x00, 0x00, 0x00, 0x00, 0x11, 0x06, 0x33, 0x07,
                                  0x17, 0x01, 0x27, 0x03, 0x51, 0x01, 0x27, 0x06,
                                  0x71, 0x01, 0x73, 0x03, 0x61, 0x06, 0x64, 0x06,
                                  0x11, 0x04, 0x65, 0x02, 0x55, 0x05, 0x77, 0x07};  // Palette [RB][xG][RB][xG][RB]...
        private bool isTMS9918 = false;
        // Data, of map
        private byte[] mapPattern = new byte[256 * 4];  // One pattern mede by four characters
        private byte[,] mapData = new byte[64, 64];     // Map data[x, y](0..255)
        Int32 mapWidth = 64;
        Int32 mapHeight = 64;
        // Data, of sprites
        private byte[] spriteGen = new byte[256 * 8];   // Sprite pattern generator table
        private byte[] spriteClr1 = new byte[256];      // Sprite color(mode1)
        private byte[] spriteClr2 = new byte[256 * 8];  // Sprite color(mode2)
        private byte[] spriteOverlay = new byte[64];    // Will overlay next sprite(1) or not(0)
        // View
        private Bitmap[] bmpOneChr = new Bitmap[256];       // PCG
        private Bitmap[] bmpOneSprite = new Bitmap[256];    // Sprite
        private Color[] colorOf = new Color[16];            // Windows color corresponding to color code
        // Consts(For TMS9918 view, we need higher resolution than RGB8)
        private int[] palette9918 = { 0x000000, 0x000000, 0x3eb849, 0x74d07d,
                                      0x5955e0, 0x8076f1, 0xb95e51, 0x65dbef,
                                      0xdb6559, 0xff897d, 0xccc35e, 0xded087,
                                      0x3aa241, 0xb766b5, 0xcccccc, 0xffffff };
        //--------------------------------------------------------------------
        // Initialize
        public void Initialize()
        {
            // Pattern generator, color, name table
            ptnGen = Properties.Resources.gentable;
            for (int i = 0; i < 256 * 8; ++i)
            {
                ptnClr[i] = 0xF0;
            }
            for (int i = 0; i < 768; ++i)
            {
                nameTable[i] = 0;
            }
            isTMS9918 = true;
            // Map patterns
            for (int i = 0; i < 256; ++i)
            {
                mapPattern[i * 4 + 0] = (byte)(i);
                mapPattern[i * 4 + 1] = (byte)((i + 1) % 256);
                mapPattern[i * 4 + 2] = (byte)((i + 32) % 256);
                mapPattern[i * 4 + 3] = (byte)((i + 33) % 256);
            }
            // Map
            for (int i = 0; i < MapHeight; ++i)
            {
                for (int j = 0; j < MapWidth; ++j)
                {
                    mapData[j, i] = 0;
                }
            }
            // Sprite patterns and colors
            for (int i = 0; i < 256; ++i)
            {
                byte color_code = (byte)(((i / 4) % 15) + 1);
                for (int j = 0; j < 8; ++j)
                {
                    spriteGen[i * 8 + j] = ptnGen[i * 8 + j];   // characters as default
                    spriteClr2[i * 8 + j] = color_code;
                }
                spriteClr1[i] = color_code;
            }
            for (int i = 0; i < 64; ++i)
            {
                spriteOverlay[i] = 0;
            }
            // Create bitmaps
            this.updateAllViewItems();
        }
        //--------------------------------------------------------------------
        // For Mementos
        public Machine CreateCopy()
        {
            Machine m = new Machine();
            m.ptnGen = ptnGen.Clone() as byte[];
            m.ptnClr = ptnClr.Clone() as byte[];
            m.nameTable = nameTable.Clone() as byte[];
            m.pltDat = pltDat.Clone() as byte[];
            m.isTMS9918 = isTMS9918;
            m.mapPattern = mapPattern.Clone() as byte[];
            m.mapData = mapData.Clone() as byte[,];
            m.mapWidth = mapWidth;
            m.mapHeight = mapHeight;
            m.spriteGen = spriteGen.Clone() as byte[];
            m.spriteClr1 = spriteClr1.Clone() as byte[];
            m.spriteClr2 = spriteClr2.Clone() as byte[];
            m.spriteOverlay = spriteOverlay.Clone() as byte[];
            return m;
        }
        public void SetAllData(Machine m)
        {
            ptnGen = m.ptnGen.Clone() as byte[];
            ptnClr = m.ptnClr.Clone() as byte[];
            nameTable = m.nameTable.Clone() as byte[];
            pltDat = m.pltDat.Clone() as byte[];
            isTMS9918 = m.isTMS9918;
            mapPattern = m.mapPattern.Clone() as byte[];
            mapData = m.mapData.Clone() as byte[,];
            mapWidth = m.mapWidth;
            mapHeight = m.mapHeight;
            spriteGen = m.spriteGen.Clone() as byte[];
            spriteClr1 = m.spriteClr1.Clone() as byte[];
            spriteClr2 = m.spriteClr2.Clone() as byte[];
            spriteOverlay = m.spriteOverlay.Clone() as byte[];
            this.updateAllViewItems();  // Update bitmaps
        }
        //--------------------------------------------------------------------
        // Palette methods
        public void SetPaletteToTMS9918(bool push)
        {
            // Set the windows color to palette based on TMS9918
            if(push) MementoCaretaker.Instance.Push();
            isTMS9918 = true;
            this.updateAllViewItems();  // Update bitmaps
        }
        public void SetPaletteToV9938(bool push)
        {
            // Set the windows color to palette based on internal palette data
            if(push) MementoCaretaker.Instance.Push();
            isTMS9918 = false;
            this.updateAllViewItems();  // Update bitmaps
        }
        //--------------------------------------------------------------------
        // Properties
        public bool IsTMS9918()
        {
            return isTMS9918;
        }
        public void SetPalette(int colorCode, int R, int G, int B, bool push)
        {
            if(push) MementoCaretaker.Instance.Push();
            // Update palette
            pltDat[colorCode * 2] = (byte)((R << 4) | B);
            pltDat[colorCode * 2 + 1] = (byte)(G);
            // Update windows color corresponding to the color code
            colorOf[colorCode] = Color.FromArgb((R * 255) / 7, (G * 255) / 7, (B * 255) / 7);
            // Update bitmaps
            this.updateAllViewItems();
        }
        public int GetPaletteR(int colorCode)
        {
            return pltDat[colorCode * 2] >> 4;
        }
        public int GetPaletteG(int colorCode)
        {
            return pltDat[colorCode * 2 + 1];
        }
        public int GetPaletteB(int colorCode)
        {
            return pltDat[colorCode * 2] & 0x0F;
        }
        public int GetPCGPixel(int pcg, int line, int x)
        {
            int addr = pcg * 8 + line;
            return (ptnGen[addr] >> (7 - x)) & 1;
        }
        public void SetPCGPixel(int pcg, int line, int x, int data, bool push)
        {
            if(push) MementoCaretaker.Instance.Push();
            // Update PCG pattern
            int addr = pcg * 8 + line;
            int bitcol = 7 - x;
            ptnGen[addr] &= (byte)(~(1 << bitcol));     // Set target bit to 0
            if (data != 0)
            {
                ptnGen[addr] |= (byte)(1 << bitcol);    // Set target bit
            }
            // Update PCG bitmap
            this.updatePCGBitmap(pcg);
        }
        public Bitmap GetBitmapOfPCG(int pcg)
        {
            return bmpOneChr[pcg];
        }
        public void CopyPCG(int src, int dst, bool push)
        {
            if(push) MementoCaretaker.Instance.Push();
            for (int i = 0; i < 8; ++i)
            {
                ptnGen[dst * 8 + i] = ptnGen[src * 8 + i];
                ptnClr[dst * 8 + i] = ptnClr[src * 8 + i];
            }
            this.updatePCGBitmap(dst);
        }
        public byte[] GetPCGGen(int index)
        {
            byte[] ret = new byte[8];
            for (int i = 0; i < 8; ++i)
            {
                ret[i] = ptnGen[index * 8 + i];
            }
            return ret;
        }
        public byte[] GetPCGClr(int index)
        {
            byte[] ret = new byte[8];
            for (int i = 0; i < 8; ++i)
            {
                ret[i] = ptnClr[index * 8 + i];
            }
            return ret;
        }
        public void SetPCG(int index, byte[] gen, byte[] clr, bool push)
        {
            if(push) MementoCaretaker.Instance.Push();
            for (int i = 0; i < 8; ++i)
            {
                if (gen != null)  ptnGen[index * 8 + i] = gen[i];
                if (clr != null) ptnClr[index * 8 + i] = clr[i];
            }
            this.updatePCGBitmap(index);
        }
        public void ClearPCG(int index)
        {
            MementoCaretaker.Instance.Push();
            for (int i = 0; i < 8; ++i)
            {
                ptnGen[index * 8 + i] = 0;
                ptnClr[index * 8 + i] = 0xF0;
            }
            this.updatePCGBitmap(index);
        }
        public byte GetPCGGenLine(int index, int line)
        {
            return ptnGen[index * 8 + line];
        }
        public byte GetPCGClrLine(int index, int line)
        {
            return ptnClr[index * 8 + line];
        }
        public void SetPCGLine(int index, int line, byte gen, byte clr, bool push)
        {
            if (push) MementoCaretaker.Instance.Push();
            ptnGen[index * 8 + line] = gen;
            ptnClr[index * 8 + line] = clr;
            this.updatePCGBitmap(index);
        }
        public void ClearPCGLine(int index, int line, bool push)
        {
            if(push) MementoCaretaker.Instance.Push();
            ptnGen[index * 8 + line] = 0;
            this.updatePCGBitmap(index);
        }
        public void InversePCG(int index, bool push)
        {
            if (push) MementoCaretaker.Instance.Push();
            for(int i = 0; i < 8; ++i)
            {
                ptnGen[index * 8 + i] = (byte)(~ptnGen[index * 8 + i]);
                byte color = ptnClr[index * 8 + i];
                ptnClr[index * 8 + i] = (byte)((color >> 4) | ((color << 4) & 0xF0));
            }
            this.updatePCGBitmap(index);
        }
        public int GetNameTable(int addr)
        {
            return nameTable[addr];
        }
        public void SetNameTable(int addr, int data, bool push)
        {
            if(push)
            {
                MementoCaretaker.Instance.Push();
            }
            nameTable[addr] = (byte)data;
        }
        public int GetColorTable(int pcg, int line, bool isForeGround)
        {
            int addr = pcg * 8 + line;
            if (isForeGround)
            {
                return ptnClr[addr] >> 4;
            }
            else
            {
                return ptnClr[addr] & 0x0F;
            }
        }
        public void SetColorTable(int pcg, int line, int color_code, bool isForeGround, bool push)
        {
            int addr = pcg * 8 + line;
            // Update color table
            if(push) MementoCaretaker.Instance.Push();
            if (isForeGround)
            {
                ptnClr[addr] &= 0x0F;
                ptnClr[addr] |= (byte)(color_code << 4);
            }
            else
            {
                ptnClr[addr] &= 0xF0;
                ptnClr[addr] |= (byte)(color_code);

            }
            // Update PCG bitmap
            this.updatePCGBitmap(addr / 8);
        }
        public Color ColorCodeToWindowsColor(int color_code)
        {
            return colorOf[color_code];
        }
        public int GetPCGInPattern(int index, int no)
        {
            return mapPattern[index * 4 + no];
        }
        public void SetPCGInPattern(int index, int no, int value, bool push)
        {
            if(push) MementoCaretaker.Instance.Push();
            mapPattern[index * 4 + no] = (byte)value;
        }
        public void CopyMapPattern(int src, int dst, bool push)
        {
            if(push) MementoCaretaker.Instance.Push();
            mapPattern[dst * 4 + 0] = mapPattern[src * 4 + 0];
            mapPattern[dst * 4 + 1] = mapPattern[src * 4 + 1];
            mapPattern[dst * 4 + 2] = mapPattern[src * 4 + 2];
            mapPattern[dst * 4 + 3] = mapPattern[src * 4 + 3];
        }
        public byte[] GetPattern(int index)
        {
            byte[] ret = new byte[4];
            ret[0] = mapPattern[index * 4 + 0];
            ret[1] = mapPattern[index * 4 + 1];
            ret[2] = mapPattern[index * 4 + 2];
            ret[3] = mapPattern[index * 4 + 3];
            return ret;
        }
        public void SetPattern(int index, byte[] val, bool push)
        {
            if(push) MementoCaretaker.Instance.Push();
            mapPattern[index * 4 + 0] = val[0];
            mapPattern[index * 4 + 1] = val[1];
            mapPattern[index * 4 + 2] = val[2];
            mapPattern[index * 4 + 3] = val[3];
        }
        public int GetMapData(int x, int y)
        {
            return mapData[x, y];
        }
        public void SetMapData(int x, int y, int value, bool push)
        {
            if(push)
            {
                MementoCaretaker.Instance.Push();
            }
            mapData[x, y] = (byte)value;
        }
        public int MapWidth
        {
            get
            {
                return mapWidth;
            }
            set
            {
                MementoCaretaker.Instance.Push();
                byte[,] m = new byte[value, mapHeight];
                for(int y = 0; y < mapHeight; ++y)
                {
                    for(int x = 0; x < mapWidth && x < value; ++x)
                    {
                        m[x, y] = mapData[x, y];
                    }
                }
                mapData = m;
                mapWidth = value;
            }
        }
        public int MapHeight
        {
            get
            {
                return mapHeight;
            }
            set
            {
                MementoCaretaker.Instance.Push();
                byte[,] m = new byte[mapWidth, value];
                for (int y = 0; y < value && y < mapHeight; ++y)
                {
                    for (int x = 0; x < mapWidth; ++x)
                    {
                        m[x, y] = mapData[x, y];
                    }
                }
                mapData = m;
                mapHeight = value;
            }
        }
        public Bitmap GetBitmapOfSprite(int index)
        {
            return bmpOneSprite[index];
        }
        public bool GetSpriteOverlay(int indexBy16x16)
        {
            return (spriteOverlay[indexBy16x16] != 0);
        }
        public void SetSpriteOverlay(int indexBy16x16, bool value, bool push)
        {
            if(push) MementoCaretaker.Instance.Push();
            // Set overlay flag of the sprite
            spriteOverlay[indexBy16x16] = value ? (byte)1 : (byte)0;
            // Set sprite attributes
            int overlay_target_8x8 = (indexBy16x16 * 4 + 4) % 256;
        }
        public int GetSpriteColorCode(int index, int line)
        {
            if (isTMS9918)
            {
                return spriteClr1[index];
            }
            else
            {
                return spriteClr2[index * 8 + line] & 0x0F;
            }
        }
        public void SetSpriteColorCode(int index, int line, int value, bool push)
        {
            if(push) MementoCaretaker.Instance.Push();
            if (isTMS9918)
            {
                spriteClr1[index] = (byte)value;
            }
            else
            {
                // internal data have color for each 8x1 dots
                int index_base = (index / 4) * 4;
                int index_target_l = index_base + (index % 2);
                int index_target_r= index_base + (index % 2) + 2;
                spriteClr2[index_target_l * 8 + line] &= 0xF0;
                spriteClr2[index_target_l * 8 + line] |= (byte)(value);
                spriteClr2[index_target_r * 8 + line] &= 0xF0;
                spriteClr2[index_target_r * 8 + line] |= (byte)(value);
            }
            this.updateSpriteBitmap();
        }
        [Serializable]
        public class One16x16Sprite
        {
            public byte[] genData = new byte[32];   // 16x16 sprite
            public byte[] clr2Data = new byte[32];
            public byte clr = 0;
            public byte overlay = 0;
        }
        public One16x16Sprite Get16x16Sprite(int index16x16)
        {
            int target8x8 = index16x16 * 4;
            One16x16Sprite spr = new One16x16Sprite();
            for (int i = 0; i < 32; ++i)
            {
                spr.genData[i] = spriteGen[target8x8 * 8 + i];
                spr.clr2Data[i] = spriteClr2[target8x8 * 8 + i];
            }
            spr.clr = spriteClr1[target8x8];
            spr.overlay = spriteOverlay[index16x16];
            return spr;
        }
        public void Set16x16Sprite(int index16x16, One16x16Sprite spr, bool push)
        {
            if (push) MementoCaretaker.Instance.Push();
            for(int i = 0; i < 4; ++i)
            {
                int target8x8 = (index16x16 * 4 + i) % 256;
                for (int j = 0; j < 8; ++j)
                {
                    spriteGen[target8x8 * 8 + j] = spr.genData[i * 8 + j];
                    spriteClr2[target8x8 * 8 + j] = spr.clr2Data[i * 8 + j];
                }
                spriteClr1[target8x8] = spr.clr;
                this.updateSpriteBitmap(target8x8);
            }
            spriteOverlay[index16x16] = spr.overlay;
        }
        public void SetSpriteGen(int index8x8, byte[] gen, bool push)
        {
            if(push) MementoCaretaker.Instance.Push();
            for (int i = 0; i < 8; ++i)
            {
                spriteGen[index8x8 * 8 + i] = gen[i];
            }
            this.updateSpriteBitmap();
        }
        public void Clear16x16Sprite(int index16x16, bool push)
        {
            if(push) MementoCaretaker.Instance.Push();
            spriteOverlay[index16x16] = 0;
            for (int i = 0; i < 4; ++i)
            {
                int dst = (index16x16 * 4 + i) % 256;
                spriteClr1[dst] = 0x0F;
                for (int j = 0; j < 8; ++j)
                {
                    spriteGen[dst * 8 + j] = 0;
                    spriteClr2[dst * 8 + j] = 0x0F;
                }
                this.updateSpriteBitmap(dst);
            }
        }
        public int GetSpritePixel(int index, int line, int x)
        {
            int target_dat = spriteGen[index * 8 + line];
            int target = (target_dat >> (7 - x)) & 1;       // left side is LSB of data source
            return target;
        }
        public void SetSpritePixel(int index, int line, int x, int val, bool push)
        {
            if(push) MementoCaretaker.Instance.Push();
            // Update Sprite pattern
            int addr = index * 8 + line;
            int bitcol = 7 - x;
            spriteGen[addr] &= (byte)(~(1 << bitcol));     // Set target bit to 0
            if (val != 0)
            {
                spriteGen[addr] |= (byte)(1 << bitcol);    // Set target bit
            }
            // Update Sprite bitmap
            this.updateSpriteBitmap(index);
        }
        [Serializable]
        public class SpriteLine
        {
            public byte genData = 0;
            public byte clrData = 0;
            public byte overlayed = 0;
            public byte genDataOv = 0;
            public byte clrDataOv = 0;
        }
        public SpriteLine GetSpriteLine(int index, int line)
        {
            SpriteLine ret = new SpriteLine();
            ret.genData = spriteGen[index * 8 + line];
            ret.clrData = spriteClr2[index * 8 + line];
            if ((ret.overlayed = spriteOverlay[index / 4]) != 0)
            {
                index = (index + 4) % 256;
                ret.genDataOv = spriteGen[index * 8 + line];
                ret.clrDataOv = spriteClr2[index * 8 + line];
            }
            return ret;
        }
        public void SetSpriteLine(int index, int line, SpriteLine val, bool push)
        {
            if (push) MementoCaretaker.Instance.Push();
            spriteGen[index * 8 + line] = val.genData;
            spriteClr2[index * 8 + line] = val.clrData;
            this.updateSpriteBitmap(index);
            if (val.overlayed != 0)
            {
                index = (index + 4) % 256;
                spriteGen[index * 8 + line] = val.genDataOv;
                spriteClr2[index * 8 + line] = val.clrDataOv;
                this.updateSpriteBitmap(index);
            }
        }
        public void ClearSpriteLine(int index, int line, bool push)
        {
            if(push) MementoCaretaker.Instance.Push();
            spriteGen[index * 8 + line] = 0;
            this.updateSpriteBitmap(index);
            if (spriteOverlay[index / 4] != 0)
            {
                index = (index + 4) % 256;
                spriteGen[index * 8 + line] = 0;
                this.updateSpriteBitmap(index);
            }
        }
        //--------------------------------------------------------------------
        // Internal methods
        private void updateAllViewItems()
        {
            this.updatePaletteView();
            this.updatePCGBitmap();
            this.updateSpriteBitmap();
        }
        private void updatePaletteView()
        {
            if (isTMS9918)
            {
                for (int i = 0; i < 16; ++i)
                {
                    int R = palette9918[i] >> 16;
                    int G = (palette9918[i] & 0xffff) >> 8;
                    int B = palette9918[i] & 0xff;
                    colorOf[i] = Color.FromArgb(R, G, B);
                }
            }
            else
            {
                for (int i = 0; i < 16; ++i)
                {
                    int R = (pltDat[i * 2] >> 4);
                    int G = (pltDat[i * 2 + 1]);
                    int B = (pltDat[i * 2] & 0x0F);
                    R = (R * 255) / 7;
                    G = (G * 255) / 7;
                    B = (B * 255) / 7;
                    colorOf[i] = Color.FromArgb(R, G, B);
                }
            }
        }
        private void updatePCGBitmap()
        {
            for (int i = 0; i < 256; ++i)
            {
                this.updatePCGBitmap(i);
            }
        }
        private void updatePCGBitmap(int pcg)
        {
            // PCG data had been changed, so remake coresponding bitmap
            bmpOneChr[pcg] = new Bitmap(8, 8);
            for (int i = 0; i < 8; ++i)          // Each line
            {
                byte pattern_per_line = ptnGen[pcg * 8 + i];    // Pattern of the line
                int fore_color = ptnClr[pcg * 8 + i] >> 4;      // Color of the line
                int back_color = ptnClr[pcg * 8 + i] & 0xF;
                Color fore = Color.Transparent;
                Color back = Color.Transparent;
                // Color code to windows color
                if (fore_color != 0) fore = colorOf[fore_color];
                if (back_color != 0) back = colorOf[back_color];
                for (int j = 0; j < 8; ++j)                     // Each pixel right to left
                {
                    int x = (pattern_per_line >> j) & 1;        // Get one bit of pattern
                    if (x != 0)
                        bmpOneChr[pcg].SetPixel(7 - j, i, fore);
                    else
                        bmpOneChr[pcg].SetPixel(7 - j, i, back);
                }
            }
        }
        private void updateSpriteBitmap()
        {
            for (int i = 0; i < 256; ++i)
            {
                this.updateSpriteBitmap(i);
            }
        }
        private void updateSpriteBitmap(int index)
        {
            // Sprite data had been changed, so remake coresponding bitmap
            bmpOneSprite[index] = new Bitmap(8, 8);
            int color_code = spriteClr1[index];
            for (int i = 0; i < 8; ++i)          // Each line
            {
                byte pattern_per_line = spriteGen[index * 8 + i];   // Pattern of the line
                if (!isTMS9918)
                {
                    color_code = spriteClr2[index * 8 + i] & 0x0F;  // On V9938 the color is set to each line
                }
                Color Fore = colorOf[color_code];               // Color code to windows color
                Color Back = Color.Transparent;
                for (int j = 0; j < 8; ++j)                     // Each pixel right to left
                {
                    int x = (pattern_per_line >> j) & 1;        // Get one bit of pattern
                    if (x != 0)
                        bmpOneSprite[index].SetPixel(7 - j, i, Fore);
                    else
                        bmpOneSprite[index].SetPixel(7 - j, i, Back);
                }
            }
        }
        private void setSpriteCCFlags()
        {
            // Set the CC flags of sprite colors before making export data
            for(int i = 1; i < 64; ++i)
            {
                if(spriteOverlay[i - 1] != 0)
                {
                    for(int j = 0; j < 32; ++j)
                    {
                        spriteClr2[i * 4 * 8 + j] |= 0x40;
                    }
                }
                else
                {
                    for (int j = 0; j < 32; ++j)
                    {
                        spriteClr2[i * 4 * 8 + j] &= 0x0F;
                    }
                }
            }
        }
    }
}


