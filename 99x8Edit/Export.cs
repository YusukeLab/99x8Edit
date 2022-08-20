using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;

namespace _99x8Edit
{
    // Exporting data
    public class Export
    {
        // Supported Export types
        internal static string PCGTypeFilter = "C Header(*.h)|*.h"
                                             + "|C Header compressed(*.h)|*.h"
                                             + "|Assembly Source(*.asm)|*.asm"
                                             + "|Assembly Source compressed(*.asm)|*.asm"
                                             + "|MSX BSAVE format, pattern data, for VRAM(*.bin)|*.bin"
                                             + "|MSX BSAVE format, color data, for VRAM(*.bin)|*.bin"
                                             + "|Raw Binary, pattern data(*.raw)|*.raw"
                                             + "|Raw Binary, color data(*.raw)|*.raw"
                                             + "|Raw Binary compressed, pattern data(*.raw)|*.raw"
                                             + "|Raw Binary compressed, color data(*.raw)|*.raw";
        internal static string SpriteTypeFilter = "C Header(*.h)|*.h"
                                             + "|C Header compressed(*.h)|*.h"
                                             + "|Assembly Source(*.asm)|*.asm"
                                             + "|Assembly Source compressed(*.asm)|*.asm"
                                             + "|MSX BSAVE format, pattern data, for VRAM(*.bin)|*.bin"
                                             + "|MSX BSAVE format, color data, for RAM(*.bin)|*.bin"
                                             + "|Raw Binary, pattern data(*.raw)|*.raw"
                                             + "|Raw Binary, color data(*.raw)|*.raw"
                                             + "|Raw Binary compressed, pattern data(*.raw)|*.raw"
                                             + "|Raw Binary compressed, color data(*.raw)|*.raw";
        internal static string MapTypeFilter = "C Header(*.h)|*.h"
                                             + "|C Header compressed(*.h)|*.h"
                                             + "|Assembly Source(*.asm)|*.asm"
                                             + "|Assembly Source compressed(*.asm)|*.asm"
                                             + "|MSX BSAVE format, pattern data, for RAM(*.bin)|*.bin"
                                             + "|MSX BSAVE format, map data, for RAM(*.bin)|*.bin"
                                             + "|Raw Binary, pattern data(*.raw)|*.raw"
                                             + "|Raw Binary, map data(*.raw)|*.raw"
                                             + "|Raw Binary compressed, pattern data(*.raw)|*.raw"
                                             + "|Raw Binary compressed, map data(*.raw)|*.raw";
        internal enum PCGType
        {
            CHeader = 0,
            CCompressed,
            ASMData,
            ASMCompressed,
            MSXBASIC_Pattern,
            MSXBASIC_Color,
            Raw_Pattern,
            Raw_Color,
            RawCompressed_Pattern,
            RawCompressed_Color,
        };
        internal enum SpriteType
        {
            CHeader = 0,
            CCompressed,
            ASMData,
            ASMCompressed,
            MSXBASIC_Pattern,
            MSXBASIC_Color,
            Raw_Pattern,
            Raw_Color,
            RawCompressed_Pattern,
            RawCompressed_Color,
        };
        internal enum MapType
        {
            CHeader = 0,
            CCompressed,
            ASMData,
            ASMCompressed,
            MSXBASIC_Pattern,
            MSXBASIC_Map,
            Raw_Pattern,
            Raw_Map,
            RawCompressed_Pattern,
            RawCompressed_Map,
        };
        // Data to be exported, to be set by host
        private byte[] _ptnGen = new byte[256 * 8];    // Pattern generator table
        private byte[] _ptnClr = new byte[256 * 8];    // Pattern color table
        private byte[] _nameTable = new byte[768];     // Sandbox(Pattern name table)
        private byte[] _pltDat = { 0x00, 0x00, 0x00, 0x00, 0x11, 0x06, 0x33, 0x07,
                                   0x17, 0x01, 0x27, 0x03, 0x51, 0x01, 0x27, 0x06,
                                   0x71, 0x01, 0x73, 0x03, 0x61, 0x06, 0x64, 0x06,
                                   0x11, 0x04, 0x65, 0x02, 0x55, 0x05, 0x77, 0x07};  // Palette [RB][xG][RB][xG][RB]...
        private bool _isTMS9918 = false;
        private byte[] _mapPattern = new byte[256 * 4];  // One pattern mede by four characters
        private byte[,] _mapData = new byte[64, 64];     // Map data[x, y](0..255)
        private Int32 _mapWidth = 64;
        private Int32 _mapHeight = 64;
        private byte[] _spriteGen = new byte[256 * 8];   // Sprite pattern generator table
        private byte[] _spriteClr = new byte[64 * 16];   // Sprite color(mode2)
        //------------------------------------------------------------------------
        // Initialize
        public Export(byte[] pcggen,
                      byte[] pcgclr,
                      byte[] sandbox,
                      byte[] plt,
                      bool is9918,
                      byte[] mapptn,
                      byte[,] mapdat,
                      Int32 mapw,
                      Int32 maph,
                      byte[] sprgen,
                      byte[] sprclr)
        {
            _ptnGen = pcggen.Clone() as byte[];
            _ptnClr = pcgclr.Clone() as byte[];
            _nameTable = sandbox.Clone() as byte[];
            _pltDat = plt.Clone() as byte[];
            _isTMS9918 = is9918;
            _mapPattern = mapptn.Clone() as byte[];
            _mapData = mapdat.Clone() as byte[,];
            _mapWidth = mapw;
            _mapHeight = maph;
            _spriteGen = sprgen.Clone() as byte[];
            _spriteClr = sprclr.Clone() as byte[];
        }
        //------------------------------------------------------------------------
        // Methods
        internal void ExportPCG(PCGType type, string path)
        {
            if (type == PCGType.CHeader || type == PCGType.CCompressed)
            {
                using StreamWriter sr = new StreamWriter(path, false);
                sr.WriteLine("#ifndef __PCGDAT_H__");
                sr.WriteLine("#define __PCGDAT_H__");
                string str = "";
                if (!_isTMS9918)
                {
                    sr.WriteLine("// Palette");
                    sr.WriteLine("const unsigned char palette[] = {");
                    str = ArrayToCHeaderString(_pltDat, false);
                    sr.WriteLine(str);
                    sr.WriteLine("};");
                }
                sr.WriteLine("// Character pattern generator table");
                if (type == 0)
                {
                    sr.WriteLine("const unsigned char ptngen[] = {");
                    str = ArrayToCHeaderString(_ptnGen, false);
                }
                else
                {
                    sr.WriteLine("const unsigned char ptngen_compressed[] = {");
                    str = ArrayToCHeaderString(_ptnGen, true);
                }
                sr.WriteLine(str);
                sr.WriteLine("};");
                sr.WriteLine("// Character pattern color table");
                if (type == PCGType.CHeader)
                {
                    sr.WriteLine("const unsigned char ptnclr[] = {");
                    str = ArrayToCHeaderString(_ptnClr, false);
                }
                else
                {
                    sr.WriteLine("const unsigned char ptnclr_compressed[] = {");
                    str = ArrayToCHeaderString(_ptnClr, true);
                }
                sr.WriteLine(str);
                sr.WriteLine("};");
                sr.WriteLine("// Name table");
                if (type == PCGType.CHeader)
                {
                    sr.WriteLine("const unsigned char nametable[] = {");
                    str = ArrayToCHeaderString(_nameTable, false);
                }
                else
                {
                    sr.WriteLine("const unsigned char nametable_compressed[] = {");
                    str = ArrayToCHeaderString(_nameTable, true);
                }
                sr.WriteLine(str);
                sr.WriteLine("};");
                sr.WriteLine("#endif");
            }
            else if (type == PCGType.ASMData || type == PCGType.ASMCompressed)
            {
                using StreamWriter sr = new StreamWriter(path, false);
                sr.WriteLine("; PCG Data");
                sr.WriteLine("; this export data is not tested");
                string str = "";
                if (!_isTMS9918)
                {
                    sr.WriteLine("; Palette r8b8g8");
                    sr.WriteLine("palette:");
                    str = ArrayToASMString(_pltDat, false);
                    sr.WriteLine(str);
                }
                sr.WriteLine("; Character pattern generator table");
                if (type == PCGType.ASMData)
                {
                    sr.WriteLine("ptngen:");
                    str = ArrayToASMString(_ptnGen, false);
                }
                else
                {
                    sr.WriteLine("ptngen_compressed:");
                    str = ArrayToASMString(_ptnGen, true);
                }
                sr.WriteLine(str);
                sr.WriteLine("; Character pattern color table");
                if (type == PCGType.ASMData)
                {
                    sr.WriteLine("prnclr:");
                    str = ArrayToASMString(_ptnClr, false);
                }
                else
                {
                    sr.WriteLine("prnclr_compressed:");
                    str = ArrayToASMString(_ptnClr, true);
                }
                sr.WriteLine(str);
                sr.WriteLine("; Name table");
                if (type == PCGType.ASMData)
                {
                    sr.WriteLine("namtbl:");
                    str = ArrayToASMString(_nameTable, false);
                }
                else
                {
                    sr.WriteLine("namtbl_compressed:");
                    str = ArrayToASMString(_nameTable, true);
                }
                sr.WriteLine(str);
            }
            else if (type == PCGType.MSXBASIC_Pattern)
            {
                using BinaryWriter br = new BinaryWriter(new FileStream(path, FileMode.Create));
                br.Write((byte)0xFE);       // BSAVE/BLOAD header
                br.Write((byte)0);          // Start address is 0
                br.Write((byte)0);
                br.Write((byte)0xFF);       // End address is 0x07FF
                br.Write((byte)0x07);
                br.Write((byte)0);          // Execution address
                br.Write((byte)0);
                br.Write(_ptnGen);
            }
            else if (type == PCGType.MSXBASIC_Color)
            {
                using BinaryWriter br = new BinaryWriter(new FileStream(path, FileMode.Create));
                br.Write((byte)0xFE);       // BSAVE/BLOAD header
                br.Write((byte)0x00);       // Start address is 0x2000
                br.Write((byte)0x20);
                br.Write((byte)0xFF);       // End address is 0x27FF
                br.Write((byte)0x27);
                br.Write((byte)0);          // Execution address
                br.Write((byte)0);
                br.Write(_ptnClr);
            }
            else if (type == PCGType.Raw_Pattern)
            {
                using BinaryWriter br = new BinaryWriter(new FileStream(path, FileMode.Create));
                br.Write(_ptnGen);
            }
            else if (type == PCGType.Raw_Color)
            {
                using BinaryWriter br = new BinaryWriter(new FileStream(path, FileMode.Create));
                br.Write(_ptnClr);
            }
            else if (type == PCGType.RawCompressed_Pattern)
            {
                using BinaryWriter br = new BinaryWriter(new FileStream(path, FileMode.Create));
                byte[] comp = Compression.Create(Compression.Type.BytePair).Encode(_ptnGen);
                br.Write(comp);
            }
            else if (type == PCGType.RawCompressed_Color)
            {
                using BinaryWriter br = new BinaryWriter(new FileStream(path, FileMode.Create));
                byte[] comp = Compression.Create(Compression.Type.BytePair).Encode(_ptnClr);
                br.Write(comp);
            }
        }
        internal void ExportMap(MapType type, string path)
        {
            if (type == MapType.CHeader || type == MapType.CCompressed)
            {
                using StreamWriter sr = new StreamWriter(path, false);
                sr.WriteLine("#ifndef __MAPDAT_H__");
                sr.WriteLine("#define __MAPDAT_H__");
                sr.WriteLine($"#define MAP_W   ({_mapWidth})");
                sr.WriteLine($"#define MAP_H   ({_mapHeight})");
                string str = "\t";
                sr.WriteLine("// Map patterns");
                if (type == MapType.CHeader)
                {
                    sr.WriteLine("const unsigned char mapptn[] = {");
                    str = ArrayToCHeaderString(_mapPattern, false);
                }
                else
                {
                    sr.WriteLine("const unsigned char mapptn_compressed[] = {");
                    str = ArrayToCHeaderString(_mapPattern, true);
                }
                sr.WriteLine(str);
                sr.WriteLine("};");
                str = "\t";
                sr.WriteLine("// Map data");
                if (type == MapType.CHeader)
                {
                    sr.WriteLine("const unsigned char mapData[] = {");
                    for (int i = 0; i < _mapWidth * _mapHeight; ++i)
                    {
                        if ((i != 0) && (i % 16) == 0) str += "\r\n\t";
                        str += $"{_mapData[i % _mapWidth, i / _mapWidth]}, ";
                    }
                }
                else
                {
                    sr.WriteLine("const unsigned char mapData_compressed[] = {");
                    byte[] comp = this.CompressMapData();
                    for (int i = 0; i < comp.Length; ++i)
                    {
                        if ((i != 0) && (i % 16) == 0) str += "\r\n\t";
                        str += $"{comp[i]}, ";
                    }
                }
                sr.WriteLine(str);
                sr.WriteLine("};");
                sr.WriteLine("#endif");
            }
            else if (type == MapType.ASMData || type == MapType.ASMCompressed)
            {
                using StreamWriter sr = new StreamWriter(path, false);
                sr.WriteLine("; Map Data");
                sr.WriteLine("; this export data is not tested");
                string str = "";
                sr.WriteLine("mapwidth:");
                sr.WriteLine($"\tdb\t{_mapWidth}");
                sr.WriteLine("mapheight:");
                sr.WriteLine($"\tdb\t{_mapHeight}");
                sr.WriteLine("; Map patterns");
                if (type == MapType.ASMData)
                {
                    sr.WriteLine("mapptn:");
                    str = ArrayToASMString(_mapPattern, false);
                }
                else
                {
                    sr.WriteLine("mapptn_compressed:");
                    str = ArrayToASMString(_mapPattern, true);
                }
                sr.WriteLine(str);
                sr.WriteLine("; Map data");
                str = "";
                if (type == MapType.ASMData)
                {
                    sr.WriteLine("mapdat:");
                    for (int i = 0; i < _mapWidth * _mapHeight; ++i)
                    {
                        if (i % 16 == 0) str += "\tdb\t";
                        str += $"{_mapData[i % _mapWidth, i / _mapWidth]}";
                        if (i % 16 == 15) str += "\r\n";
                        else str += ", ";
                    }
                }
                else
                {
                    sr.WriteLine("mapdat_compressed:");
                    byte[] comp = this.CompressMapData();
                    for (int i = 0; i < comp.Length; ++i)
                    {
                        if (i % 16 == 0) str += "\tdb\t";
                        str += $"{comp[i]}";
                        if (i % 16 == 15) str += "\r\n";
                        else str += ", ";
                    }
                }
                sr.WriteLine(str);
            }
            else if (type == MapType.MSXBASIC_Pattern)
            {
                using BinaryWriter br = new BinaryWriter(new FileStream(path, FileMode.Create));
                int pattern_size = 4 * 256;
                br.Write((byte)0xFE);                   // BSAVE/BLOAD header
                br.Write((byte)0x00);                   // Start address is 0x0000
                br.Write((byte)0x00);
                br.Write((byte)(pattern_size & 0xFF));  // End address depends on size
                br.Write((byte)(pattern_size >> 8));
                br.Write((byte)0);                      // Execution address
                br.Write((byte)0);
                br.Write(_mapPattern);
            }
            else if (type == MapType.MSXBASIC_Map)
            {
                using BinaryWriter br = new BinaryWriter(new FileStream(path, FileMode.Create));
                int map_size = _mapWidth * _mapHeight;
                br.Write((byte)0xFE);                   // BSAVE/BLOAD header
                br.Write((byte)0x00);                   // Start address is 0x0000
                br.Write((byte)0x00);
                br.Write((byte)(map_size & 0xFF));      // End address depends on size
                br.Write((byte)(map_size >> 8));
                br.Write((byte)0);                      // Execution address
                br.Write((byte)0);
                for (int i = 0; i < _mapHeight; ++i)
                {
                    for (int j = 0; j < _mapWidth; ++j)
                    {
                        br.Write(_mapData[j, i]);
                    }
                }
            }
            else if (type == MapType.Raw_Pattern)
            {
                using BinaryWriter br = new BinaryWriter(new FileStream(path, FileMode.Create));
                br.Write(_mapPattern);
            }
            else if (type == MapType.Raw_Map)
            {
                using BinaryWriter br = new BinaryWriter(new FileStream(path, FileMode.Create));
                for (int i = 0; i < _mapHeight; ++i)
                {
                    for (int j = 0; j < _mapWidth; ++j)
                    {
                        br.Write(_mapData[j, i]);
                    }
                }
            }
            else if (type == MapType.RawCompressed_Pattern)
            {
                using BinaryWriter br = new BinaryWriter(new FileStream(path, FileMode.Create));
                byte[] comp = Compression.Create(Compression.Type.BytePair).Encode(_mapPattern);
                br.Write(comp);
            }
            else if (type == MapType.RawCompressed_Map)
            {
                using BinaryWriter br = new BinaryWriter(new FileStream(path, FileMode.Create));
                br.Write(this.CompressMapData());
            }
        }
        internal void ExportSprites(SpriteType type, string path)
        {
            // Start exporting
            if (type == SpriteType.CHeader || type == SpriteType.CCompressed)
            {
                using StreamWriter sr = new StreamWriter(path, false);
                sr.WriteLine("#ifndef __SPRITEDAT_H__");
                sr.WriteLine("#define __SPRITEDAT_H__");
                string str = "";
                sr.WriteLine("// Sprite generator table");
                if (type == SpriteType.CHeader)
                {
                    sr.WriteLine("const unsigned char sprptn[] = {");
                    str = ArrayToCHeaderString(_spriteGen, false);
                }
                else
                {
                    sr.WriteLine("const unsigned char sprptn_compressed[] = {");
                    str = ArrayToCHeaderString(_spriteGen, true);
                }
                sr.WriteLine(str);
                sr.WriteLine("};");
                if (!_isTMS9918)
                {
                    sr.WriteLine("// Sprite color table");
                    if (type == SpriteType.CHeader)
                    {
                        sr.WriteLine("const unsigned char sprclr[] = {");
                        str = ArrayToCHeaderString(_spriteClr, false);
                    }
                    else
                    {
                        sr.WriteLine("const unsigned char sprclr_compressed[] = {");
                        str = ArrayToCHeaderString(_spriteClr, true);
                    }
                    sr.WriteLine(str);
                    sr.WriteLine("};");
                }
                sr.WriteLine("#endif");
            }
            else if (type == SpriteType.ASMData || type == SpriteType.ASMCompressed)
            {
                using StreamWriter sr = new StreamWriter(path, false);
                sr.WriteLine("; Sprite Data");
                sr.WriteLine("; this export data is not tested");
                string str = "";
                sr.WriteLine("; Sprite generator table");
                str = "";
                if (type == SpriteType.ASMData)
                {
                    sr.WriteLine("sprgen:");
                    str = ArrayToASMString(_spriteGen, false);
                }
                else
                {
                    sr.WriteLine("sprgen_compressed:");
                    str = ArrayToASMString(_spriteGen, true);
                }
                sr.WriteLine(str);
                if (!_isTMS9918)
                {
                    sr.WriteLine("; Sprite color table");
                    str = "";
                    if (type == SpriteType.ASMData)
                    {
                        sr.WriteLine("sprclr:");
                        str = ArrayToASMString(_spriteClr, false);
                    }
                    else
                    {
                        sr.WriteLine("sprclr_compressed:");
                        str = ArrayToASMString(_spriteClr, true);
                    }
                    sr.WriteLine(str);
                }
            }
            else if (type == SpriteType.MSXBASIC_Pattern)
            {
                using BinaryWriter br = new BinaryWriter(new FileStream(path, FileMode.Create));
                br.Write((byte)0xFE);       // BSAVE/BLOAD header
                br.Write((byte)0x00);       // Start address is 0x3800
                br.Write((byte)0x38);
                br.Write((byte)0xFF);       // End address is 0x3FFF
                br.Write((byte)0x3F);
                br.Write((byte)0);          // Execution address
                br.Write((byte)0);
                br.Write(_spriteGen);
            }
            else if (type == SpriteType.MSXBASIC_Color)
            {
                using BinaryWriter br = new BinaryWriter(new FileStream(path, FileMode.Create));
                // Color table can't be registered to VRAM, so it would be loaded on RAM
                br.Write((byte)0xFE);       // BSAVE/BLOAD header
                br.Write((byte)0x00);       // Start address is 0x0000
                br.Write((byte)0x00);
                br.Write((byte)0x04);       // End address is 0x0400
                br.Write((byte)0x00);
                br.Write((byte)0);          // Execution address
                br.Write((byte)0);
                br.Write(_spriteClr);
            }
            else if (type == SpriteType.Raw_Pattern)
            {
                using BinaryWriter br = new BinaryWriter(new FileStream(path, FileMode.Create));
                br.Write(_spriteGen);
            }
            else if (type == SpriteType.Raw_Color)
            {
                using BinaryWriter br = new BinaryWriter(new FileStream(path, FileMode.Create));
                br.Write(_spriteClr);
            }
            else if (type == SpriteType.RawCompressed_Pattern)
            {
                using BinaryWriter br = new BinaryWriter(new FileStream(path, FileMode.Create));
                byte[] comp = Compression.Create(Compression.Type.BytePair).Encode(_spriteGen);
                br.Write(comp);
            }
            else if (type == SpriteType.RawCompressed_Color)
            {
                using BinaryWriter br = new BinaryWriter(new FileStream(path, FileMode.Create));
                byte[] comp = Compression.Create(Compression.Type.BytePair).Encode(_spriteClr);
                br.Write(comp);
            }
        }
        //------------------------------------------------------------------------
        // Utilities
        private string ArrayToCHeaderString(byte[] src, bool compress)
        {
            string ret = "\t";
            if (compress)
            {
                byte[] comp = Compression.Create(Compression.Type.BytePair).Encode(src);
                for (int i = 0; i < comp.Length; ++i)
                {
                    if ((i != 0) && (i % 16) == 0) ret += "\r\n\t";
                    ret += $"{comp[i]}, ";
                }
            }
            else
            {
                for (int i = 0; i < src.Length; ++i)
                {
                    if ((i != 0) && (i % 16) == 0) ret += "\r\n\t";
                    ret += $"{src[i]}, ";
                }
            }
            return ret;
        }
        private string ArrayToASMString(byte[] src, bool compress)
        {
            string ret = "";
            if (compress)
            {
                byte[] comp = Compression.Create(Compression.Type.BytePair).Encode(src);
                for (int i = 0; i < comp.Length; ++i)
                {
                    if (i % 16 == 0) ret += $"\tdb\t";
                    ret += $"{comp[i]}";
                    if (i % 16 == 15) ret += "\r\n";
                    else ret += ", ";
                }
            }
            else
            {
                for (int i = 0; i < src.Length; ++i)
                {
                    if (i % 16 == 0) ret += "\tdb\t";
                    ret += $"{src[i]}";
                    if (i % 16 == 15) ret += "\r\n";
                    else ret += ", ";
                }
            }
            return ret;
        }
        private byte[] CompressMapData()
        {
            // Map will be compressed by run length encode, to be decoded realtime
            List<byte[]> comp_data = new List<byte[]>();
            // Head of data will be [offset to the data of row] * [number of rows]
            ushort[] offset_to_row = new ushort[_mapHeight];
            ushort offset = (ushort)(_mapHeight * 2);
            // Create data
            for(int y = 0; y < _mapHeight; ++y)
            {
                // Offset to the row data
                offset_to_row[y] = offset;
                // Compress each row
                List<byte> src_row = new List<byte>();
                for(int i = 0; i < _mapWidth; ++i)
                {
                    src_row.Add(_mapData[i, y]);
                }
                CompressionBase encoder = Compression.Create(Compression.Type.RunLength);
                byte[] comp = encoder.Encode(src_row.ToArray() as byte[]);
                comp_data.Add(comp);
                offset += (ushort)comp.Length;
            }
            // Make one compressed data
            List<byte> ret = new List<byte>();
            for(int i = 0; i < _mapHeight; ++i)
            {
                // Offset is stored as 2 byte little endian data
                ret.Add((byte)(offset_to_row[i] & 0xFF));
                ret.Add((byte)((offset_to_row[i]) >> 8 & 0xFF));
            }
            for (int i = 0; i < _mapHeight; ++i)
            {
                for(int j = 0; j < comp_data[i].Length; ++j)
                {
                    ret.Add(comp_data[i][j]);
                }
            }
            return ret.ToArray() as byte[];
        }
    }
}
