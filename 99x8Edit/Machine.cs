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
        public void SetPaletteToTMS9918()
        {
            // Set the windows color to palette based on TMS9918
            MementoCaretaker.Instance.Push();
            isTMS9918 = true;
            this.updateAllViewItems();  // Update bitmaps
        }
        public void SetPaletteToV9938()
        {
            // Set the windows color to palette based on internal palette data
            MementoCaretaker.Instance.Push();
            isTMS9918 = false;
            this.updateAllViewItems();  // Update bitmaps
        }
        //--------------------------------------------------------------------
        // Properties
        public bool IsTMS9918()
        {
            return isTMS9918;
        }
        public void SetPalette(int colorCode, int R, int G, int B)
        {
            MementoCaretaker.Instance.Push();
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
        public void SetPCGPixel(int pcg, int line, int x, int data)
        {
            MementoCaretaker.Instance.Push();
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
        public void CopyPCG(int src, int dst)
        {
            MementoCaretaker.Instance.Push();
            for (int i = 0; i < 8; ++i)
            {
                ptnGen[dst * 8 + i] = ptnGen[src * 8 + i];
                ptnClr[dst * 8 + i] = ptnClr[src * 8 + i];
            }
            this.updatePCGBitmap(dst);
        }
        public void CopyPCGToClip(int index)
        {
            ClipOnePCG clip = new ClipOnePCG();
            clip.index = (byte)index;
            for(int i = 0; i < 8; ++i)
            {
                clip.genData[i] = ptnGen[index * 8 + i];
                clip.clrData[i] = ptnClr[index * 8 + i];
            }
            Clipboard.Instance.Clip = clip;
        }
        public void PastePCGFromClip(int index)
        {
            dynamic clip = Clipboard.Instance.Clip;
            if (clip is ClipOnePCG)
            {
                MementoCaretaker.Instance.Push();
                for (int i = 0; i < 8; ++i)
                {
                    ptnGen[index * 8 + i] = clip.genData[i];
                    ptnClr[index * 8 + i] = clip.clrData[i];
                }
                this.updatePCGBitmap(index);
            }
            else if(clip is ClipOneChrInRom)
            {
                MementoCaretaker.Instance.Push();
                for (int i = 0; i < 8; ++i)
                {
                    ptnGen[index * 8 + i] = clip.leftTop[i];                    // Left top
                    ptnGen[((index + 32) % 256) * 8 + i] = clip.leftBottom[i];  // Left bottom
                    ptnGen[((index + 1) % 256) * 8 + i] = clip.rightTop[i];     // Right top
                    ptnGen[((index + 33) % 256) * 8 + i] = clip.rightBottom[i]; // Right bottom
                }
                this.updatePCGBitmap();
            }
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
        public void CopyPCGLineToClip(int index, int line)
        {
            ClipPCGLine clip = new ClipPCGLine();
            clip.genData = ptnGen[index * 8 + line];
            clip.clrData = ptnClr[index * 8 + line];
            Clipboard.Instance.Clip = clip;
        }
        public void PastePCGLineFromClip(int index, int line)
        {
            dynamic clip = Clipboard.Instance.Clip;
            if (clip is ClipPCGLine)
            {
                MementoCaretaker.Instance.Push();
                ptnGen[index * 8 + line] = clip.genData;
                ptnClr[index * 8 + line] = clip.clrData;
                this.updatePCGBitmap(index);
            }
        }
        public void ClearPCGLine(int index, int line)
        {
            MementoCaretaker.Instance.Push();
            ptnGen[index * 8 + line] = 0;
            this.updatePCGBitmap(index);
        }
        public int GetNameTable(int addr)
        {
            return nameTable[addr];
        }
        public void SetNameTable(int addr, int data, bool push = true)
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
        public void SetColorTable(int pcg, int line, int color_code, bool isForeGround)
        {
            int addr = pcg * 8 + line;
            // Update color table
            MementoCaretaker.Instance.Push();
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
        public int GetMapPattern(int index, int no)
        {
            return mapPattern[index * 4 + no];
        }
        public void SetMapPattern(int index, int no, int value)
        {
            MementoCaretaker.Instance.Push();
            mapPattern[index * 4 + no] = (byte)value;
        }
        public void CopyMapPattern(int src, int dst)
        {
            MementoCaretaker.Instance.Push();
            mapPattern[dst * 4 + 0] = mapPattern[src * 4 + 0];
            mapPattern[dst * 4 + 1] = mapPattern[src * 4 + 1];
            mapPattern[dst * 4 + 2] = mapPattern[src * 4 + 2];
            mapPattern[dst * 4 + 3] = mapPattern[src * 4 + 3];
        }
        public void CopyMapPatternToClip(int index)
        {
            ClipOneMapPattern clip = new ClipOneMapPattern();
            clip.index = (byte)index;
            clip.pattern[0] = mapPattern[index * 4 + 0];
            clip.pattern[1] = mapPattern[index * 4 + 1];
            clip.pattern[2] = mapPattern[index * 4 + 2];
            clip.pattern[3] = mapPattern[index * 4 + 3];
            Clipboard.Instance.Clip = clip;
        }
        public void PasteMapPatternFromClip(int index)
        {
            dynamic clip = Clipboard.Instance.Clip;
            if(clip is ClipOneMapPattern)
            {
                MementoCaretaker.Instance.Push();
                mapPattern[index * 4 + 0] = clip.pattern[0];
                mapPattern[index * 4 + 1] = clip.pattern[1];
                mapPattern[index * 4 + 2] = clip.pattern[2];
                mapPattern[index * 4 + 3] = clip.pattern[3];
            }
        }
        public int GetMapData(int x, int y)
        {
            return mapData[x, y];
        }
        public void SetMapData(int x, int y, int value, bool push = true)
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
        public void SetSpriteOverlay(int indexBy16x16, bool value)
        {
            MementoCaretaker.Instance.Push();
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
        public void SetSpriteColorCode(int index, int line, int value, bool push = true)
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
            this.updateSpriteBitmap(index);
        }
        public void Copy16x16SpriteToClip(int index16x16)
        {
            int target8x8 = index16x16 * 4;
            Clip16x16Sprite clip = new Clip16x16Sprite();
            for(int i = 0; i < 32; ++i)
            {
                clip.genData[i] = spriteGen[target8x8 * 8 + i];
                clip.clr2Data[i] = spriteClr2[target8x8 * 8 + i];
            }
            clip.clr = spriteClr1[target8x8];
            if((clip.overlayed = spriteOverlay[index16x16]) != 0)
            {
                target8x8 = (target8x8 + 4) % 256;
                for (int i = 0; i < 32; ++i)
                {
                    clip.genData_ov[i] = spriteGen[target8x8 * 8 + i];
                    clip.clr2Data_ov[i] = spriteClr2[target8x8 * 8 + i];
                }
                clip.clr_ov = spriteClr1[target8x8];
            }
            Clipboard.Instance.Clip = clip;
        }
        public void Paste16x16SpriteFromClip(int index16x16)
        {
            dynamic clip = Clipboard.Instance.Clip;
            if (clip is Clip16x16Sprite)
            {
                MementoCaretaker.Instance.Push();
                int left = (index16x16 + 1) % 64;
                int overlayed = spriteOverlay[left];    // we have to keep the overlayed flag
                for(int i = 0; i < 4; ++i)
                {
                    int index8x8 = (index16x16 * 4) + i;
                    spriteClr1[index8x8] = clip.clr;
                    for (int j = 0; j < 8; ++j)
                    {
                        spriteGen[index8x8 * 8 + j] = clip.genData[i * 8 + j];
                        spriteClr2[index8x8 * 8 + j] = (byte)((clip.clr2Data[i * 8 + j] & 0xF) | (overlayed << 5));
                    }
                    this.updateSpriteBitmap(index8x8);
                }
                if((spriteOverlay[index16x16] = clip.overlayed) != 0)
                {
                    for (int i = 0; i < 4; ++i)
                    {
                        int index8x8 = (((index16x16 + 1) * 4) + i) % 256;
                        spriteClr1[index8x8] = clip.clr_ov;
                        for (int j = 0; j < 8; ++j)
                        {
                            spriteGen[index8x8 * 8 + j] = clip.genData_ov[i * 8 + j];
                            spriteClr2[index8x8 * 8 + j] = (byte)((clip.clr2Data_ov[i * 8 + j] & 0xF) | (1 << 5));
                        }
                        this.updateSpriteBitmap(index8x8);
                    }
                    spriteOverlay[(index16x16 + 1) % 64] = 0;
                }
            }
            else if(clip is ClipOneChrInRom)
            {
                MementoCaretaker.Instance.Push();
                spriteOverlay[index16x16] = 0;
                for(int i = 0; i < 8; ++i)
                {
                    spriteGen[index16x16 * 4 * 8 + 0 + i] = clip.leftTop[i];
                    spriteGen[index16x16 * 4 * 8 + 8 + i] = clip.leftBottom[i];
                    spriteGen[index16x16 * 4 * 8 + 16 + i] = clip.rightTop[i];
                    spriteGen[index16x16 * 4 * 8 + 24 + i] = clip.rightTop[i];
                }
                this.updateSpriteBitmap();
            }
        }
        public void Clear16x16Sprite(int index16x16)
        {
            MementoCaretaker.Instance.Push();
            int left = (index16x16 + 1) % 64;
            for (int i = 0; i < 4; ++i)
            {
                int dst = index16x16 * 4 + i;
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
        public void SetSpritePixel(int index, int line, int x, int val)
        {
            MementoCaretaker.Instance.Push();
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
        public void CopySpriteLineToClip(int index, int line)
        {
            ClipOneSpriteLine clip = new ClipOneSpriteLine();
            clip.genData = spriteGen[index * 8 + line];
            clip.clrData = spriteClr2[index * 8 + line];
            if (spriteOverlay[index / 4] != 0)
            {
                index = (index + 4) % 256;
                clip.overlayed = true;
                clip.genData2 = spriteGen[index * 8 + line];
                clip.clrData2 = spriteClr2[index * 8 + line];
            }
            Clipboard.Instance.Clip = clip;
        }
        public void PasteSpriteLineFromClip(int index, int line)
        {
            dynamic clip = Clipboard.Instance.Clip;
            if (clip is ClipOneSpriteLine)
            {
                MementoCaretaker.Instance.Push();
                spriteGen[index * 8 + line] = clip.genData;
                spriteClr2[index * 8 + line] &= 0xF0;
                spriteClr2[index * 8 + line] |= (byte)(clip.clrData & 0x0F);
                if (clip.overlayed && (spriteOverlay[index / 4] != 0))
                {
                    index = (index + 4) % 256;
                    spriteGen[index * 8 + line] = clip.genData2;
                    spriteClr2[index * 8 + line] &= 0xF0;
                    spriteClr2[index * 8 + line] |= (byte)(clip.clrData2 & 0x0F);
                }
                this.updateSpriteBitmap(index);
            }
        }
        public void ClearSpriteLine(int index, int line)
        {
            MementoCaretaker.Instance.Push();
            spriteGen[index * 8 + line] = 0;
            if(spriteOverlay[index / 4] != 0)
            {
                index = (index + 4) % 256;
                spriteGen[index * 8 + line] = 0;
            }
            this.updateSpriteBitmap(index);
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


