using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace _99x8Edit
{
    // All VDP related data will be wrapped here
    public class Machine : IMementoTarget
    {
        // Data, of PCG
        private byte[] _ptnGen = new byte[256 * 8];    // Pattern generator table
        private byte[] _ptnClr = new byte[256 * 8];    // Pattern color table
        private byte[] _nameTable = new byte[768];     // Sandbox (Pattern name table)
        private byte[] _pltDat = { 0x00, 0x00, 0x00, 0x00, 0x11, 0x06, 0x33, 0x07,
                                   0x17, 0x01, 0x27, 0x03, 0x51, 0x01, 0x27, 0x06,
                                   0x71, 0x01, 0x73, 0x03, 0x61, 0x06, 0x64, 0x06,
                                   0x11, 0x04, 0x65, 0x02, 0x55, 0x05, 0x77, 0x07};  // Palette [RB][xG][RB][xG][RB]...
        private bool _isTMS9918;
        // Data, of map
        private byte[] _mapPattern = new byte[256 * 4];  // One pattern mede by four characters
        private byte[,] _mapData = new byte[64, 64];     // Map data[x, y](0..255)
        private Int32 _mapWidth = 64;
        private Int32 _mapHeight = 64;
        // Data, of sprites
        private byte[] _spriteGen = new byte[256 * 8];   // Sprite pattern generator table
        private byte[] _spriteClr16 = new byte[64];      // Sorute color (for mode 1)
        private byte[] _spriteClr = new byte[64 * 16];   // Sprite color (mode 2), colors for 16 lines
        private byte[] _spriteOverlay = new byte[64];    // Will overlay next sprite(1) or not(0)
        // View
        private Bitmap[] _bmpOneChr = new Bitmap[256];       // PCG
        private Bitmap[] _bmpOneSprite = new Bitmap[256];    // Sprite
        private Color[] _colorOf = new Color[16];            // Windows color corresponding to color code
        private Brush[] _brushOf = new Brush[16];
        // Consts(For TMS9918 view, we need higher resolution than RGB8)
        private static readonly int[] _palette9918 = { 0x000000, 0x000000, 0x3eb849, 0x74d07d,
                                                       0x5955e0, 0x8076f1, 0xb95e51, 0x65dbef,
                                                       0xdb6559, 0xff897d, 0xccc35e, 0xded087,
                                                       0x3aa241, 0xb766b5, 0xcccccc, 0xffffff };
        //--------------------------------------------------------------------
        // Initialize
        public Machine()
        {
            // Nothing at constructor,
            // since this class will be created on each action, e.g.undo/redo
        }
        internal void SetToDefault()
        {
            // Pattern generator, color, name table
            _ptnGen = Properties.Resources.gentable;
            for (int i = 0; i < 256 * 8; ++i)
            {
                _ptnClr[i] = 0xF0;
            }
            for (int i = 0; i < 768; ++i)
            {
                _nameTable[i] = 0;
            }
            _isTMS9918 = true;
            // Map patterns
            for (int i = 0; i < 256; ++i)
            {
                _mapPattern[i * 4 + 0] = (byte)(i);
                _mapPattern[i * 4 + 1] = (byte)((i + 1) % 256);
                _mapPattern[i * 4 + 2] = (byte)((i + 32) % 256);
                _mapPattern[i * 4 + 3] = (byte)((i + 33) % 256);
            }
            // Map
            for (int i = 0; i < MapHeight; ++i)
            {
                for (int j = 0; j < MapWidth; ++j)
                {
                    _mapData[j, i] = 0;
                }
            }
            // Sprite patterns and colors
            for (int i = 0; i < 256; ++i)
            {
                for (int j = 0; j < 8; ++j)
                {
                    _spriteGen[i * 8 + j] = _ptnGen[i * 8 + j];   // characters as default
                }
            }
            for (int i = 0; i < 64; ++i)
            {
                byte color_code = (byte)(i % 8 + 8);
                _spriteClr16[i] = color_code;
                for(int j = 0; j < 16; ++j)
                {
                    _spriteClr[i * 16 + j] = color_code;
                }
                _spriteOverlay[i] = 0;
            }
            this.UpdateAllViewItems();         // Create bitmaps
        }
        //--------------------------------------------------------------------
        // For Mementos
        IMementoTarget IMementoTarget.CreateCopy()
        {
            Machine m = new Machine()
            {
                _ptnGen = _ptnGen.Clone() as byte[],
                _ptnClr = _ptnClr.Clone() as byte[],
                _nameTable = _nameTable.Clone() as byte[],
                _pltDat = _pltDat.Clone() as byte[],
                _isTMS9918 = _isTMS9918,
                _mapPattern = _mapPattern.Clone() as byte[],
                _mapData = _mapData.Clone() as byte[,],
                _mapWidth = _mapWidth,
                _mapHeight = _mapHeight,
                _spriteGen = _spriteGen.Clone() as byte[],
                _spriteClr16 = _spriteClr16.Clone() as byte[],
                _spriteClr = _spriteClr.Clone() as byte[],
                _spriteOverlay = _spriteOverlay.Clone() as byte[]
            };
            return m;
        }
        void IMementoTarget.Restore(IMementoTarget m)
        {
            if (m is Machine src)
            {
                _ptnGen = src._ptnGen.Clone() as byte[];
                _ptnClr = src._ptnClr.Clone() as byte[];
                _nameTable = src._nameTable.Clone() as byte[];
                _pltDat = src._pltDat.Clone() as byte[];
                _isTMS9918 = src._isTMS9918;
                _mapPattern = src._mapPattern.Clone() as byte[];
                _mapData = src._mapData.Clone() as byte[,];
                _mapWidth = src._mapWidth;
                _mapHeight = src._mapHeight;
                _spriteGen = src._spriteGen.Clone() as byte[];
                _spriteClr16 = src._spriteClr16.Clone() as byte[];
                _spriteClr = src._spriteClr.Clone() as byte[];
                _spriteOverlay = src._spriteOverlay.Clone() as byte[];
                this.UpdateAllViewItems();  // Update bitmaps
            }
        }
        //------------------------------------------------------------------------
        // File IO
        internal void SaveAllSettings(BinaryWriter br)
        {
            this.SavePCG(br);
            this.SaveSprites(br);
            this.SaveMap(br);
        }
        internal void LoadAllSettings(BinaryReader br)
        {
            this.LoadPCG(br);
            this.LoadSprites(br);
            this.LoadMap(br);
        }
        //--------------------------------------------------------------------
        // Export methods
        internal void ExportPCG(int type, string path)
        {
            Export e = new Export(_ptnGen, _ptnClr, _nameTable, _pltDat, _isTMS9918,
                                  _mapPattern, _mapData, _mapWidth, _mapHeight,
                                  _spriteGen, _spriteClr);
            e.ExportPCG((Export.PCGType)type, path);
        }
        internal void ExportMap(int type, string path)
        {
            Export e = new Export(_ptnGen, _ptnClr, _nameTable, _pltDat, _isTMS9918,
                                  _mapPattern, _mapData, _mapWidth, _mapHeight,
                                  _spriteGen, _spriteClr);
            e.ExportMap((Export.MapType)type, path);
        }
        internal void ExportSprites(int type, string path)
        {
            // Set the CC flags of the sprite color
            /*
                We don't update the CC flags when editing, since it will be a quite mess
                when there are copy, paste and other actions to overlayed sprite.
                Anyway, we only need the CC flags for exporting, so we're going to update here.
             */
            this.SetSpriteCCFlags();
            Export e = new Export(_ptnGen, _ptnClr, _nameTable, _pltDat, _isTMS9918,
                                  _mapPattern, _mapData, _mapWidth, _mapHeight,
                                  _spriteGen, _spriteClr);
            e.ExportSprites((Export.SpriteType)type, path);
        }
        //--------------------------------------------------------------------
        // Import
        internal void ImportPCG(string filename, int type)
        {
            Import i = new Import()
            {
                Palette = _colorOf        // Import with current palette(temp)
            };
            i.ImportPCG(filename, type, _ptnGen, _ptnClr);
            Array.Clear(_nameTable, index: 0, length: 768);
            this.UpdateAllViewItems();
        }
        internal void ImportSprite(String filename, int type)
        {
            Import i = new Import()
            {
                Palette = _colorOf        // Import with current palette(temp)
            };
            i.ImportSprite(filename, type, _spriteGen, _spriteClr, _spriteOverlay);
            Array.Fill<byte>(_spriteClr16, 0x0F);
            this.UpdateAllViewItems();
        }
        //--------------------------------------------------------------------
        // File IO for indivisual settings
        internal void SavePaletteSettings(BinaryWriter br)
        {
            br.Write(_pltDat);
        }
        internal void LoadPaletteSettings(BinaryReader br)
        {
            br.Read((Span<byte>)_pltDat);
            for (int i = 0; i < 16; ++i)
            {
                // Set the windows color to palette based on internal palette data
                int r = (_pltDat[i * 2] >> 4);
                int g = (_pltDat[i * 2 + 1]);
                int b = (_pltDat[i * 2] & 0x0F);
                r = (r * 255) / 7;
                g = (g * 255) / 7;
                b = (b * 255) / 7;
                _colorOf[i] = Color.FromArgb(r, g, b);
                _brushOf[i] = new SolidBrush(_colorOf[i]);
            }
            this.UpdateAllViewItems();
        }
        internal void SavePCG(BinaryWriter br)
        {
            br.Write(_ptnGen);           // Pattern generator table
            br.Write(_ptnClr);           // Pattern color table
            br.Write(_nameTable);        // Name table
            br.Write(_pltDat);           // Palette
            br.Write(_isTMS9918);        // Based on TMS9918 or not
        }
        internal void LoadPCG(BinaryReader br)
        {
            br.Read((Span<byte>)_ptnGen);                // Pattern generator table
            br.Read((Span<byte>)_ptnClr);                // Pattern color table
            br.Read((Span<byte>)_nameTable);             // Name table
            br.Read((Span<byte>)_pltDat);                // Palette
            _isTMS9918 = br.ReadBoolean();   // Based on TMS9918 or not
            this.UpdateColorsByPalette();
            this.UpdatePCGBitmap();
        }
        internal void SaveSprites(BinaryWriter br)
        {
            br.Write(_spriteGen);            // Sprite patten generator table
            for (int i = 0; i < 64; ++i)    // Sprite color for mode1
            {
                for (int j = 0; j < 4; ++j)
                {
                    // In project file data, we have colors for 256 sprites so convert
                    br.Write(_spriteClr16[i]);
                }
            }
            // 64sprites*16line color to 256sprites*8line color
            for (int i = 0; i < 64; ++i)
            {
                for (int j = 0; j < 16; ++j)
                {
                    br.Write(_spriteClr[i * 16 + j]);    // sprite 0, 1 in file
                }
                for (int j = 0; j < 16; ++j)
                {
                    br.Write(_spriteClr[i * 16 + j]);    // sprite 2, 3 in file
                }
            }
            br.Write(_spriteOverlay);    // Sprite overlay flags
        }
        internal void LoadSprites(BinaryReader br)
        {
            br.Read((Span<byte>)_spriteGen);             // Sprite patten generator table
            for (int i = 0; i < 64; ++i)    // Sprite color for mode1
            {
                // In project file data, we have colors for 256 sprites so convert
                _spriteClr16[i] = br.ReadByte();
                _ = br.ReadBytes(3);
            }
            // 256sprites*8line color = 64sprites*16line color
            for (int i = 0; i < 64; ++i)
            {
                for (int j = 0; j < 16; ++j)
                {
                    _spriteClr[i * 16 + j] = br.ReadByte();  // sprite 0, 1 in file
                }
                _ = br.ReadBytes(16);                       // sprite 2, 3 in file
            }
            br.Read(_spriteOverlay);         // Sprite overlay flags
            this.UpdateSpriteBitmap();
        }
        internal void SaveMap(BinaryWriter br)
        {
            br.Write(_mapPattern);
            br.Write(_mapWidth);
            br.Write(_mapHeight);
            for (int i = 0; i < _mapHeight; ++i)
            {
                for (int j = 0; j < _mapWidth; ++j)
                {
                    br.Write(_mapData[j, i]);
                }
            }
        }
        internal void LoadMap(BinaryReader br)
        {
            br.Read((Span<byte>)_mapPattern);
            _mapWidth = br.ReadInt32();
            _mapHeight = br.ReadInt32();
            for (int i = 0; i < _mapHeight; ++i)
            {
                for (int j = 0; j < _mapWidth; ++j)
                {
                    _mapData[j, i] = br.ReadByte();
                }
            }
        }
        //--------------------------------------------------------------------
        // For user interface
        //------------------------------------------------
        // Palettes and colors
        internal bool IsTMS9918 => _isTMS9918;
        internal void SetPaletteToTMS9918(bool push)
        {
            // Set the windows color to palette based on TMS9918
            if(push) MementoCaretaker.Instance.Push();
            _isTMS9918 = true;
            this.UpdateAllViewItems();  // Update bitmaps
        }
        internal void SetPaletteToV9938(bool push)
        {
            // Set the windows color to palette based on internal palette data
            if(push) MementoCaretaker.Instance.Push();
            _isTMS9918 = false;
            this.UpdateAllViewItems();  // Update bitmaps
        }
        internal (int R, int G, int B) GetPalette(int color_code)
        {
            color_code = Math.Clamp(color_code, 0, 15);
            int r = _pltDat[color_code * 2] >> 4;
            int g = _pltDat[color_code * 2 + 1];
            int b = _pltDat[color_code * 2] & 0x0F;
            return (r, g, b);
        }
        internal void SetPalette(int color_code, int r, int g, int b, bool push)
        {
            if (push) MementoCaretaker.Instance.Push();
            color_code = Math.Clamp(color_code, 0, 15);
            r = Math.Clamp(r, 0, 7);
            g = Math.Clamp(g, 0, 7);
            b = Math.Clamp(b, 0, 7);
            // Update palette
            _pltDat[color_code * 2] = (byte)((r << 4) | b);
            _pltDat[color_code * 2 + 1] = (byte)(g);
            // Update windows color corresponding to the color code
            Color c = Color.FromArgb((r * 255) / 7, (g * 255) / 7, (b * 255) / 7);
            _colorOf[color_code] = c;
            _brushOf[color_code] = new SolidBrush(c);
            // Update bitmaps
            this.UpdateAllViewItems();
        }
        internal Color ColorOf(int color_code)
        {
            // Color code(0-15) to windows color
            color_code = Math.Clamp(color_code, 0, 15);
            return _colorOf[color_code];
        }
        internal Brush BrushOf(int color_code)
        {
            // Color code(0-15) to windows brush, to avoid creating brushed everytime
            color_code = Math.Clamp(color_code, 0, 15);
            return _brushOf[color_code];
        }
        //------------------------------------------------
        // Programmable characters
        internal Bitmap GetBitmapOfPCG(int pcg)
        {
            pcg = Math.Clamp(pcg, 0, 255);
            return _bmpOneChr[pcg];
        }
        internal int GetPCGPixel(int pcg, int line, int x)
        {
            pcg = Math.Clamp(pcg, 0, 255);
            line = Math.Clamp(line, 0, 7);
            x = Math.Clamp(x, 0, 7);
            int addr = pcg * 8 + line;
            return (_ptnGen[addr] >> (7 - x)) & 1;
        }
        internal void SetPCGPixel(int pcg, int line, int x, int data, bool push)
        {
            pcg = Math.Clamp(pcg, 0, 255);
            line = Math.Clamp(line, 0, 7);
            x = Math.Clamp(x, 0, 7);
            if (push) MementoCaretaker.Instance.Push();
            // Update PCG pattern
            int addr = pcg * 8 + line;
            int bitcol = 7 - x;
            _ptnGen[addr] &= (byte)(~(1 << bitcol));     // Set target bit to 0
            if (data != 0)
            {
                _ptnGen[addr] |= (byte)(1 << bitcol);    // Set target bit
            }
            // Update PCG bitmap
            this.UpdatePCGBitmap(pcg);
        }
        internal void CopyPCG(int src, int dst, bool push)
        {
            src = Math.Clamp(src, 0, 255);
            dst = Math.Clamp(dst, 0, 255);
            if (push) MementoCaretaker.Instance.Push();
            for (int i = 0; i < 8; ++i)
            {
                _ptnGen[dst * 8 + i] = _ptnGen[src * 8 + i];
                _ptnClr[dst * 8 + i] = _ptnClr[src * 8 + i];
            }
            this.UpdatePCGBitmap(dst);
        }
        internal (byte[] gen, byte[] color) GetPCGData(int pcg)
        {
            pcg = Math.Clamp(pcg, 0, 255);
            byte[] gen = new byte[8];
            byte[] color = new byte[8];
            for (int i = 0; i < 8; ++i)
            {
                gen[i] = _ptnGen[pcg * 8 + i];
                color[i] = _ptnClr[pcg * 8 + i];
            }
            return (gen, color);
        }
        internal void SetPCGData(int pcg, byte[] gen, byte[] clr, bool push)
        {
            pcg = Math.Clamp(pcg, 0, 255);
            for (int i = 0; i < 8; ++i)
            {
                _ptnGen[pcg * 8 + i] = gen?[i] ?? 0;
                _ptnClr[pcg * 8 + i] = clr?[i] ?? 0xF0;
            }
            this.UpdatePCGBitmap(pcg);
        }
        internal void ClearPCG(int pcg)
        {
            pcg = Math.Clamp(pcg, 0, 255);
            MementoCaretaker.Instance.Push();
            for (int i = 0; i < 8; ++i)
            {
                _ptnGen[pcg * 8 + i] = 0;
                _ptnClr[pcg * 8 + i] = 0xF0;
            }
            this.UpdatePCGBitmap(pcg);
        }
        internal (byte gen, byte color) GetPCGLine(int pcg, int line)
        {
            pcg = Math.Clamp(pcg, 0, 255);
            line = Math.Clamp(line, 0, 7);
            return (_ptnGen[pcg * 8 + line], _ptnClr[pcg * 8 + line]);
        }
        internal void SetPCGLine(int pcg, int line, byte gen, byte color, bool push)
        {
            pcg = Math.Clamp(pcg, 0, 255);
            line = Math.Clamp(line, 0, 7);
            if (push) MementoCaretaker.Instance.Push();
            _ptnGen[pcg * 8 + line] = gen;
            _ptnClr[pcg * 8 + line] = color;
            this.UpdatePCGBitmap(pcg);
        }
        internal void ClearPCGLine(int pcg, int line, bool push)
        {
            pcg = Math.Clamp(pcg, 0, 255);
            line = Math.Clamp(line, 0, 7);
            if (push) MementoCaretaker.Instance.Push();
            _ptnGen[pcg * 8 + line] = 0;
            _ptnClr[pcg * 8 + line] = 0xF0;
            this.UpdatePCGBitmap(pcg);
        }
        internal void InversePCG(int pcg, bool push)
        {
            pcg = Math.Clamp(pcg, 0, 255);
            if (push) MementoCaretaker.Instance.Push();
            for(int i = 0; i < 8; ++i)
            {
                _ptnGen[pcg * 8 + i] = (byte)(~_ptnGen[pcg * 8 + i]);
                byte color = _ptnClr[pcg * 8 + i];
                _ptnClr[pcg * 8 + i] = (byte)((color >> 4) | ((color << 4) & 0xF0));
            }
            this.UpdatePCGBitmap(pcg);
        }
        internal int GetPCGColor(int pcg, int line, bool foreground)
        {
            pcg = Math.Clamp(pcg, 0, 255);
            line = Math.Clamp(line, 0, 7);
            int addr = pcg * 8 + line;
            if (foreground)
            {
                return _ptnClr[addr] >> 4;
            }
            else
            {
                return _ptnClr[addr] & 0x0F;
            }
        }
        internal void SetPCGColor(int pcg, int line, int color_code, bool isForeGround, bool push)
        {
            pcg = Math.Clamp(pcg, 0, 255);
            line = Math.Clamp(line, 0, 7);
            color_code = Math.Clamp(color_code, 0, 15);
            int addr = pcg * 8 + line;
            // Update color table
            if (push) MementoCaretaker.Instance.Push();
            if (isForeGround)
            {
                _ptnClr[addr] &= 0x0F;
                _ptnClr[addr] |= (byte)(color_code << 4);
            }
            else
            {
                _ptnClr[addr] &= 0xF0;
                _ptnClr[addr] |= (byte)(color_code);

            }
            // Update PCG bitmap
            this.UpdatePCGBitmap(addr / 8);
        }
        //------------------------------------------------
        // Name table
        internal int GetNameTable(int addr)
        {
            return _nameTable[addr];
        }
        internal void SetNameTable(int addr, int data, bool push)
        {
            addr = Math.Clamp(addr, 0, 767);
            if (push)
            {
                MementoCaretaker.Instance.Push();
            }
            _nameTable[addr] = (byte)data;
        }
        //------------------------------------------------
        // Map pattern (4 pcg in each 256 patterns)
        internal int GetPCGInPattern(int ptn, int index)
        {
            ptn = Math.Clamp(ptn, 0, 255);
            index = Math.Clamp(index, 0, 3);
            return _mapPattern[ptn * 4 + index];
        }
        internal void SetPCGInPattern(int ptn, int index, int pcg, bool push)
        {
            ptn = Math.Clamp(ptn, 0, 255);
            index = Math.Clamp(index, 0, 3);
            pcg = Math.Clamp(pcg, 0, 255);
            if (push) MementoCaretaker.Instance.Push();
            _mapPattern[ptn * 4 + index] = (byte)pcg;
        }
        internal void CopyMapPattern(int src, int dst, bool push)
        {
            src = Math.Clamp(src, 0, 255);
            dst = Math.Clamp(dst, 0, 255);
            if (push) MementoCaretaker.Instance.Push();
            _mapPattern[dst * 4 + 0] = _mapPattern[src * 4 + 0];
            _mapPattern[dst * 4 + 1] = _mapPattern[src * 4 + 1];
            _mapPattern[dst * 4 + 2] = _mapPattern[src * 4 + 2];
            _mapPattern[dst * 4 + 3] = _mapPattern[src * 4 + 3];
        }
        internal byte[] GetPattern(int index)
        {
            index = Math.Clamp(index, 0, 255);
            byte[] ret = new byte[4];
            ret[0] = _mapPattern[index * 4 + 0];
            ret[1] = _mapPattern[index * 4 + 1];
            ret[2] = _mapPattern[index * 4 + 2];
            ret[3] = _mapPattern[index * 4 + 3];
            return ret;
        }
        internal void SetPattern(int index, byte[] val, bool push)
        {
            index = Math.Clamp(index, 0, 255);
            if (push) MementoCaretaker.Instance.Push();
            _mapPattern[index * 4 + 0] = val[0];
            _mapPattern[index * 4 + 1] = val[1];
            _mapPattern[index * 4 + 2] = val[2];
            _mapPattern[index * 4 + 3] = val[3];
        }
        //------------------------------------------------
        // Map data
        internal int GetMapData(int x, int y)
        {
            x = Math.Clamp(x, 0, _mapWidth - 1);
            y = Math.Clamp(y, 0, _mapHeight - 1);
            return _mapData[x, y];
        }
        internal void SetMapData(int x, int y, int ptn, bool push)
        {
            x = Math.Clamp(x, 0, _mapWidth - 1);
            y = Math.Clamp(y, 0, _mapHeight - 1);
            ptn = Math.Clamp(ptn, 0, 255);
            if (push)
            {
                MementoCaretaker.Instance.Push();
            }
            _mapData[x, y] = (byte)ptn;
        }
        internal int MapWidth
        {
            get => _mapWidth;
            set
            {
                MementoCaretaker.Instance.Push();
                byte[,] m = new byte[value, _mapHeight];
                for(int y = 0; y < _mapHeight; ++y)
                {
                    for(int x = 0; x < _mapWidth && x < value; ++x)
                    {
                        m[x, y] = _mapData[x, y];
                    }
                }
                _mapData = m;
                _mapWidth = value;
            }
        }
        internal int MapHeight
        {
            get => _mapHeight;
            set
            {
                MementoCaretaker.Instance.Push();
                byte[,] m = new byte[_mapWidth, value];
                for (int y = 0; y < value && y < _mapHeight; ++y)
                {
                    for (int x = 0; x < _mapWidth; ++x)
                    {
                        m[x, y] = _mapData[x, y];
                    }
                }
                _mapData = m;
                _mapHeight = value;
            }
        }
        //------------------------------------------------
        // Sprite (data:256 of 8x8sprite, UI:64 of 16x16sprite
        internal IEnumerable<Bitmap> GetBitmapsForSprite16(int index16)
        {
            index16 = Math.Clamp(index16, 0, 63);
            yield return _bmpOneSprite[index16 * 4 + 0];
            yield return _bmpOneSprite[index16 * 4 + 1];
            yield return _bmpOneSprite[index16 * 4 + 2];
            yield return _bmpOneSprite[index16 * 4 + 3];
        }
        internal bool GetSpriteOverlay(int index16)
        {
            index16 = Math.Clamp(index16, 0, 63);
            return (_spriteOverlay[index16] != 0);
        }
        internal void SetSpriteOverlay(int index16, bool overlay, bool push)
        {
            index16 = Math.Clamp(index16, 0, 63);
            if (push) MementoCaretaker.Instance.Push();
            // Set overlay flag of the sprite
            _spriteOverlay[index16] = overlay ? (byte)1 : (byte)0;
        }
        internal int GetSpriteColorCode(int index16, int line)
        {
            index16 = Math.Clamp(index16, 0, 63);
            line = Math.Clamp(line, 0, 15);
            if (_isTMS9918)
            {
                return _spriteClr16[index16];
            }
            else
            {
                return _spriteClr[index16 * 16 + line] & 0x0F;  // Mask CC Flags
            }
        }
        internal void SetSpriteColorCode(int index16, int line, int code, bool push)
        {
            index16 = Math.Clamp(index16, 0, 63);
            line = Math.Clamp(line, 0, 15);
            code = Math.Clamp(code, 0, 15);
            if (push) MementoCaretaker.Instance.Push();
            if (_isTMS9918)
            {
                _spriteClr16[index16] = (byte)code;
            }
            else
            {
                _spriteClr[index16 * 16 + line] = (byte)code;
            }
            this.UpdateSpriteBitmap();
        }
        [Serializable]
        internal class OneSprite
        {
            public byte[] genData = new byte[32];   // 16x16 sprite
            public byte[] clrData = new byte[16];
            public byte clr;
            public byte overlay;
        }
        internal OneSprite GetSpriteData(int index16)
        {
            index16 = Math.Clamp(index16, 0, 63);
            OneSprite spr = new OneSprite();
            for (int i = 0; i < 32; ++i)
            {
                spr.genData[i] = _spriteGen[index16 * 32 + i];
            }
            for (int i = 0; i < 16; ++i)
            {
                spr.clrData[i] = _spriteClr[index16 * 16 + i];
            }
            spr.clr = _spriteClr16[index16];
            spr.overlay = _spriteOverlay[index16];
            return spr;
        }
        internal void SetSpriteData(int index16, OneSprite spr, bool push)
        {
            index16 = Math.Clamp(index16, 0, 63);
            if (push) MementoCaretaker.Instance.Push();
            for (int i = 0; i < 16; ++i)
            {
                _spriteClr[index16 * 16 + i] = spr.clrData[i];
            }
            for (int i = 0; i < 4; ++i)
            {
                int target8 = (index16 * 4 + i) % 256;
                for (int j = 0; j < 8; ++j)
                {
                    _spriteGen[target8 * 8 + j] = spr.genData[i * 8 + j];
                }
                this.UpdateSpriteBitmap(target8);
            }
            for(int i = 0; i < 16; ++i)
            {
                _spriteClr[index16 * 16 + i] = spr.clrData[i];
            }
            _spriteClr16[index16] = spr.clr;
            _spriteOverlay[index16] = spr.overlay;
        }
        internal void SetSpriteGen(int index16, List<byte> gen, bool push)
        {
            index16 = Math.Clamp(index16, 0, 63);
            if (gen?.Count == 32)
            {
                if (push) MementoCaretaker.Instance.Push();
                for (int i = 0; i < 32; ++i)
                {
                    _spriteGen[index16 * 32 + i] = gen[i];
                }
                this.UpdateSpriteBitmap();
            }
        }
        internal void ClearSprite(int index16, bool push)
        {
            index16 = Math.Clamp(index16, 0, 63);
            if (push) MementoCaretaker.Instance.Push();
            _spriteOverlay[index16] = 0;
            _spriteClr16[index16] = 0x0F;
            for (int i = 0; i < 16; ++i)
            {
                _spriteClr[index16 * 16 + i] = 0x0F;
            }
            for (int i = 0; i < 4; ++i)
            {
                int dst = (index16 * 4 + i) % 256;
                for (int j = 0; j < 8; ++j)
                {
                    _spriteGen[dst * 8 + j] = 0;
                }
                this.UpdateSpriteBitmap(dst);
            }
        }
        internal int GetSpritePixel(int index16, int x, int y, bool dummy)
        {
            index16 = Math.Clamp(index16, 0, 63);
            x = Math.Clamp(x, 0, 15);
            y = Math.Clamp(y, 0, 15);
            int index8 = index16 * 4 + (x / 8) * 2 + (y / 8);
            int line = y % 8;
            int bitcol = 7 - (x % 8);
            int target_dat = _spriteGen[index8 * 8 + line];
            int target = (target_dat >> bitcol) & 1;       // left side is LSB of data source
            return target;
        }
        internal void SetSpritePixel(int index16, int x, int y, int val, bool push)
        {
            index16 = Math.Clamp(index16, 0, 63);
            x = Math.Clamp(x, 0, 15);
            y = Math.Clamp(y, 0, 15);
            if (push) MementoCaretaker.Instance.Push();
            // Update Sprite pattern
            int index8 = index16 * 4 + (x / 8) * 2 + (y / 8);
            int line = y % 8;
            int bitcol = 7 - (x % 8);
            _spriteGen[index8 * 8 + line] &= (byte)(~(1 << bitcol));     // Set target bit to 0
            if (val != 0)
            {
                _spriteGen[index8 * 8 + line] |= (byte)(1 << bitcol);    // Set target bit
            }
            this.UpdateSpriteBitmap(index8);
        }
        [Serializable]
        internal class SpriteLine
        {
            public byte genData;
            public byte colorData;
            public byte overlayed;
            public byte genDataOv;
            public byte colorDataOv;
            public bool colorOnly = false;
        }
        internal SpriteLine GetSpriteLine(int index16, int line_x, int line_y)
        {
            index16 = Math.Clamp(index16, 0, 63);
            line_x = Math.Clamp(line_x, 0, 1);
            line_y = Math.Clamp(line_y, 0, 15);
            // Generator table is 8 lines per 8x8 sprite
            int index8 = index16 * 4 + (line_x) * 2 + (line_y / 8);
            int line = line_y % 8;
            // Color address is 16 lines per 16x16 sprite
            int color_addr = index16 * 16 + line_y;
            // Return corresponding data of one line
            SpriteLine ret = new SpriteLine()
            {
                genData = _spriteGen[index8 * 8 + line],
                colorData = _spriteClr[color_addr]
            };
            if ((ret.overlayed = _spriteOverlay[index16]) != 0)
            {
                index8 = (index8 + 4) % 256;
                color_addr = ((index16 + 1) % 64) * 16 + line_y;
                ret.genDataOv = _spriteGen[index8 * 8 + line];
                ret.colorDataOv = _spriteClr[color_addr];
            }
            return ret;
        }
        internal void SetSpriteLine(int index16, int line_x, int line_y, SpriteLine val, bool push)
        {
            if (push) MementoCaretaker.Instance.Push();
            index16 = Math.Clamp(index16, 0, 63);
            line_x = Math.Clamp(line_x, 0, 1);
            line_y = Math.Clamp(line_y, 0, 15);
            // Generator table is 8 lines per 8x8 sprite
            int index8 = index16 * 4 + (line_x) * 2 + (line_y / 8);
            int line = line_y % 8;
            // Color address is 16 lines per 16x16 sprite
            int color_addr = index16 * 16 + line_y;
            if(!val.colorOnly)
            {
                _spriteGen[index8 * 8 + line] = val.genData;
            }
            _spriteClr[color_addr] = val.colorData;
            this.UpdateSpriteBitmap(index8);
            if ((val.overlayed != 0) && (_spriteOverlay[index16] != 0))
            {
                index8 = (index8 + 4) % 256;
                color_addr = ((index16 + 1) % 64) * 16 + line_y;
                _spriteGen[index8 * 8 + line] = val.genDataOv;
                _spriteClr[color_addr] = val.colorDataOv;
                this.UpdateSpriteBitmap(index8);
            }
        }
        internal void ClearSpriteLine(int index16, int line_x, int line_y, bool push)
        {
            if (push) MementoCaretaker.Instance.Push();
            index16 = Math.Clamp(index16, 0, 63);
            line_x = Math.Clamp(line_x, 0, 1);
            line_y = Math.Clamp(line_y, 0, 15);
            int index8 = index16 * 4 + (line_x) * 2 + (line_y / 8);
            int line = line_y % 8;
            // Clear the generator table and keep the color data
            _spriteGen[index8 * 8 + line] = 0;
            this.UpdateSpriteBitmap(index8);
            if (_spriteOverlay[index16] != 0)
            {
                index8 = (index8 + 4) % 256;
                _spriteGen[index8 * 8 + line] = 0;
                this.UpdateSpriteBitmap(index8);
            }
        }
        internal void RotateSprite(int index16, int ver, int hor, bool push)
        {
            index16 = Math.Clamp(index16, 0, 63);
            ver = Math.Clamp(ver, -15, 15);
            hor = Math.Clamp(hor, -15, 15);
            Action<int> right_shift = (id) =>
            {
                // Right shift one 16x16 sprite
                for(int i = 0; i < 16; ++i)
                {
                    Utility.Rotate16(ref _spriteGen[id * 32 + 0 + i],
                                     ref _spriteGen[id * 32 + 16 + i]);
                }
            };
            Action<int> down_shift = (id) =>
            {
                // Down shift one 16x16 sprite
                byte carry_gen_l = _spriteGen[id * 32 + 15];
                byte carry_gen_r = _spriteGen[id * 32 + 31];
                byte carry_clr = _spriteClr[id * 16 + 15];
                for (int i = 15; i >= 1; --i)
                {
                    _spriteGen[id * 32 + i] = _spriteGen[id * 32 + i - 1];
                    _spriteGen[id * 32 + i + 16] = _spriteGen[id * 32 + i - 1 + 16];
                    _spriteClr[id * 16 + i] = _spriteClr[id * 16 + i - 1];
                }
                _spriteGen[id * 32] = carry_gen_l;
                _spriteGen[id * 32 + 16] = carry_gen_r;
                _spriteClr[id * 16] = carry_clr;
            };
            if (push) MementoCaretaker.Instance.Push();
            if (ver < 0) { ver += 16; }    // Left shift to right shift
            if (hor < 0) { hor += 16; }    // Up shift to down shift
            for(int cnt = 0; cnt < ver; ++cnt)
            {
                right_shift(index16);
            }
            for (int cnt = 0; cnt < hor; ++cnt)
            {
                down_shift(index16);
            }
            if (_spriteOverlay[index16] != 0)
            {
                index16 = (index16 + 1) % 64;
                for (int cnt = 0; cnt < ver; ++cnt)
                {
                    right_shift(index16);
                }
                for (int cnt = 0; cnt < hor; ++cnt)
                {
                    down_shift(index16);
                }
            }
            this.UpdateSpriteBitmap();
        }
        //--------------------------------------------------------------------
        // Private use
        private void UpdateAllViewItems()
        {
            // Update the bitmaps corresponding to data
            this.UpdateColorsByPalette();
            this.UpdatePCGBitmap();
            this.UpdateSpriteBitmap();
        }
        private void UpdateColorsByPalette()
        {
            // Update windows colors and brushes corresponding to data
            if (_isTMS9918)
            {
                for (int i = 0; i < 16; ++i)
                {
                    int r = _palette9918[i] >> 16;
                    int g = (_palette9918[i] & 0xffff) >> 8;
                    int b = _palette9918[i] & 0xff;
                    Color c = Color.FromArgb(r, g, b);
                    if(c != _colorOf[i])
                    {
                        _colorOf[i] = Color.FromArgb(r, g, b);
                        _brushOf[i] = new SolidBrush(c);
                    }
                }
            }
            else
            {
                for (int i = 0; i < 16; ++i)
                {
                    int r = (_pltDat[i * 2] >> 4);
                    int g = (_pltDat[i * 2 + 1]);
                    int b = (_pltDat[i * 2] & 0x0F);
                    r = (r * 255) / 7;
                    g = (g * 255) / 7;
                    b = (b * 255) / 7;
                    Color c = Color.FromArgb(r, g, b);
                    if (c != _colorOf[i])
                    {
                        _colorOf[i] = Color.FromArgb(r, g, b);
                        _brushOf[i] = new SolidBrush(c);
                    }
                }
            }
        }
        private void UpdatePCGBitmap()
        {
            for (int i = 0; i < 256; ++i)
            {
                this.UpdatePCGBitmap(i);
            }
        }
        private void UpdatePCGBitmap(int pcg)
        {
            _bmpOneChr[pcg] = _bmpOneChr[pcg] ?? new Bitmap(8, 8);
            for (int i = 0; i < 8; ++i)          // Each line
            {
                byte pattern_per_line = _ptnGen[pcg * 8 + i];    // Pattern of the line
                int fore_color = _ptnClr[pcg * 8 + i] >> 4;      // Color of the line
                int back_color = _ptnClr[pcg * 8 + i] & 0xF;
                Color fore = Color.Transparent;
                Color back = Color.Transparent;
                // Color code to windows color
                if (fore_color != 0) fore = _colorOf[fore_color];
                if (back_color != 0) back = _colorOf[back_color];
                for (int j = 0; j < 8; ++j)                     // Each pixel right to left
                {
                    int x = (pattern_per_line >> j) & 1;        // Get one bit of pattern
                    if (x != 0)
                        _bmpOneChr[pcg].SetPixel(x: 7 - j, y: i, color: fore);
                    else
                        _bmpOneChr[pcg].SetPixel(x: 7 - j, y: i, color: back);
                }
            }
        }
        private void UpdateSpriteBitmap()
        {
            for (int i = 0; i < 256; ++i)
            {
                this.UpdateSpriteBitmap(i);
            }
        }
        private void UpdateSpriteBitmap(int index8)
        {
            _bmpOneSprite[index8] = _bmpOneSprite[index8] ?? new Bitmap(8, 8);
            int color_code = _spriteClr16[index8 / 4];
            int addr_color = (index8 / 4) * 16 + (index8 % 2) * 8;
            for (int i = 0; i < 8; ++i)    // Each line
            {
                byte pattern = _spriteGen[index8 * 8 + i];   // Pattern of the line
                if (!_isTMS9918)
                {
                    color_code = _spriteClr[addr_color + i] & 0x0F;
                }
                Color fore = _colorOf[color_code];   // Color code to windows color
                Color back = Color.Transparent;
                for (int j = 0; j < 8; ++j)         // Each pixel right to left
                {
                    int x = (pattern >> j) & 1;     // Get one bit of pattern
                    if (x != 0)
                        _bmpOneSprite[index8].SetPixel(7 - j, i, fore);
                    else
                        _bmpOneSprite[index8].SetPixel(7 - j, i, back);
                }
            }
        }
        private void SetSpriteCCFlags()
        {
            // Set the CC flags of sprite colors before making export data
            for(int i = 1; i < 64; ++i)
            {
                if(_spriteOverlay[i - 1] != 0)
                {
                    for(int j = 0; j < 16; ++j)
                    {
                        _spriteClr[i * 16 + j] |= 0x40;
                    }
                }
                else
                {
                    for (int j = 0; j < 16; ++j)
                    {
                        _spriteClr[i * 16 + j] &= 0x0F;
                    }
                }
            }
        }
    }
}
