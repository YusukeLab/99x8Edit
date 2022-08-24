using System;
using System.Collections.Generic;
using System.IO;

namespace _99x8Edit
{
    internal interface IExportable
    {
        internal byte[] PtnGen { get; }        // Pattern generator table
        internal byte[] PtnClr { get; }        // Pattern color table
        internal byte[] NameTable { get; }     // Sandbox(Pattern name table)
        internal bool HasThreeBanks { get; }
        internal byte[] PltDat { get; }
        internal bool Is9918 { get; }
        internal byte[] MapPattern { get; }    // One pattern made by four characters
        internal byte[,] MapData { get; }      // Map data[x, y](0..255)
        internal Int32 MapWidth { get; }
        internal Int32 MapHeight { get; }
        internal byte[] SpriteGen { get; }     // Sprite pattern generator table
        internal byte[] SpriteClr { get; }     // Sprite color(mode2)
    }
    // Exporting data
    internal class Export
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
        // Data to be exported
        private readonly IExportable _src;
        //------------------------------------------------------------------------
        // Initialize
        public Export(IExportable src)
        {
            _src = src;
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
                string str;
                if (!_src.Is9918)
                {
                    sr.WriteLine("// Palette");
                    sr.WriteLine("const unsigned char palette[] = {");
                    str = ArrayToCHeaderString(_src.PltDat, false);
                    sr.WriteLine(str);
                    sr.WriteLine("};");
                }
                sr.WriteLine("// Character pattern generator table");
                if (type == PCGType.CHeader)
                {
                    sr.WriteLine("const unsigned char ptngen[] = {");
                    int length = _src.HasThreeBanks ? 768 * 8 : 256 * 8;
                        str = ArrayToCHeaderString(_src.PtnGen, compress: false, index: 0, length);
                }
                else
                {
                    sr.WriteLine("const unsigned char ptngen_compressed[] = {");
                    int length = _src.HasThreeBanks ? 768 * 8 : 256 * 8;
                    str = ArrayToCHeaderString(_src.PtnGen, compress: true, index: 0, length);
                }
                sr.WriteLine(str);
                sr.WriteLine("};");
                sr.WriteLine("// Character pattern color table");
                if (type == PCGType.CHeader)
                {
                    sr.WriteLine("const unsigned char ptnclr[] = {");
                    int length = _src.HasThreeBanks ? 768 * 8 : 256 * 8;
                    str = ArrayToCHeaderString(_src.PtnClr, compress: false, index: 0, length);
                }
                else
                {
                    sr.WriteLine("const unsigned char ptnclr_compressed[] = {");
                    int length = _src.HasThreeBanks ? 768 * 8 : 256 * 8;
                    str = ArrayToCHeaderString(_src.PtnClr, compress: true, index: 0, length);
                }
                sr.WriteLine(str);
                sr.WriteLine("};");
                sr.WriteLine("// Name table");
                if (type == PCGType.CHeader)
                {
                    sr.WriteLine("const unsigned char nametable[] = {");
                    str = ArrayToCHeaderString(_src.NameTable, compress: false);
                }
                else
                {
                    sr.WriteLine("const unsigned char nametable_compressed[] = {");
                    str = ArrayToCHeaderString(_src.NameTable, compress: true);
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
                string str;
                if (!_src.Is9918)
                {
                    sr.WriteLine("; Palette r8b8g8");
                    sr.WriteLine("palette:");
                    str = ArrayToASMString(_src.PltDat, compress: false);
                    sr.WriteLine(str);
                }
                sr.WriteLine("; Character pattern generator table");
                if (type == PCGType.ASMData)
                {
                    sr.WriteLine("ptngen:");
                    int length = _src.HasThreeBanks ? 768 * 8 : 256 * 8;
                    str = ArrayToASMString(_src.PtnGen, compress: false, index: 0, length);
                }
                else
                {
                    sr.WriteLine("ptngen_compressed:");
                    int length = _src.HasThreeBanks ? 768 * 8 : 256 * 8;
                    str = ArrayToASMString(_src.PtnGen, compress: true, index: 0, length);
                }
                sr.WriteLine(str);
                sr.WriteLine("; Character pattern color table");
                if (type == PCGType.ASMData)
                {
                    sr.WriteLine("prnclr:");
                    int length = _src.HasThreeBanks ? 768 * 8 : 256 * 8;
                    str = ArrayToASMString(_src.PtnClr, compress: false, index: 0, length);
                }
                else
                {
                    sr.WriteLine("prnclr_compressed:");
                    int length = _src.HasThreeBanks ? 768 * 8 : 256 * 8;
                    str = ArrayToASMString(_src.PtnClr, compress: true, index: 0, length);
                }
                sr.WriteLine(str);
                sr.WriteLine("; Name table");
                if (type == PCGType.ASMData)
                {
                    sr.WriteLine("namtbl:");
                    str = ArrayToASMString(_src.NameTable, compress: false);
                }
                else
                {
                    sr.WriteLine("namtbl_compressed:");
                    str = ArrayToASMString(_src.NameTable, compress: true);
                }
                sr.WriteLine(str);
            }
            else if (type == PCGType.MSXBASIC_Pattern)
            {
                using BinaryWriter br = new BinaryWriter(new FileStream(path, FileMode.Create));
                br.Write((byte)0xFE);       // BSAVE/BLOAD header
                br.Write((byte)0);          // Start address is 0
                br.Write((byte)0);
                if (_src.HasThreeBanks)
                {
                    br.Write((byte)0xFF);   // End address is 0x17FF
                    br.Write((byte)0x17);
                }
                else
                {
                    br.Write((byte)0xFF);   // End address is 0x07FF
                    br.Write((byte)0x07);
                }
                br.Write((byte)0);          // Execution address
                br.Write((byte)0);
                if (_src.HasThreeBanks)
                {
                    br.Write(_src.PtnGen);
                }
                else
                {
                    byte[] dst = new byte[256 * 8];
                    Array.Copy(_src.PtnGen, 0, dst, 0, 256 * 8);
                    br.Write(dst);
                }
            }
            else if (type == PCGType.MSXBASIC_Color)
            {
                using BinaryWriter br = new BinaryWriter(new FileStream(path, FileMode.Create));
                br.Write((byte)0xFE);       // BSAVE/BLOAD header
                br.Write((byte)0x00);       // Start address is 0x2000
                br.Write((byte)0x20);
                if (_src.HasThreeBanks)
                {
                    br.Write((byte)0xFF);       // End address is 0x37FF
                    br.Write((byte)0x37);
                }
                else
                {
                    br.Write((byte)0xFF);       // End address is 0x27FF
                    br.Write((byte)0x27);
                }
                br.Write((byte)0);          // Execution address
                br.Write((byte)0);
                if (_src.HasThreeBanks)
                {
                    br.Write(_src.PtnClr);
                }
                else
                {
                    byte[] dst = new byte[256 * 8];
                    Array.Copy(_src.PtnClr, 0, dst, 0, 256 * 8);
                    br.Write(dst);
                }
            }
            else if (type == PCGType.Raw_Pattern)
            {
                using BinaryWriter br = new BinaryWriter(new FileStream(path, FileMode.Create));
                int len = _src.HasThreeBanks ? 768 * 8 : 256 * 8;
                byte[] dst = new byte[len];
                Array.Copy(_src.PtnGen, 0, dst, 0, len);
                br.Write(dst);
            }
            else if (type == PCGType.Raw_Color)
            {
                using BinaryWriter br = new BinaryWriter(new FileStream(path, FileMode.Create));
                int len = _src.HasThreeBanks ? 768 * 8 : 256 * 8;
                byte[] dst = new byte[len];
                Array.Copy(_src.PtnClr, 0, dst, 0, len);
                br.Write(dst);
            }
            else if (type == PCGType.RawCompressed_Pattern)
            {
                using BinaryWriter br = new BinaryWriter(new FileStream(path, FileMode.Create));
                int len = _src.HasThreeBanks ? 768 * 8 : 256 * 8;
                byte[] dst = new byte[len];
                Array.Copy(_src.PtnGen, 0, dst, 0, len);
                byte[] comp = Compression.Create(Compression.Type.BytePair).Encode(dst);
                br.Write(comp);
            }
            else if (type == PCGType.RawCompressed_Color)
            {
                using BinaryWriter br = new BinaryWriter(new FileStream(path, FileMode.Create));
                int len = _src.HasThreeBanks ? 768 * 8 : 256 * 8;
                byte[] dst = new byte[len];
                Array.Copy(_src.PtnClr, 0, dst, 0, len);
                byte[] comp = Compression.Create(Compression.Type.BytePair).Encode(dst);
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
                sr.WriteLine($"#define MAP_W   ({_src.MapWidth})");
                sr.WriteLine($"#define MAP_H   ({_src.MapHeight})");
                string str;
                sr.WriteLine("// Map patterns");
                if (type == MapType.CHeader)
                {
                    sr.WriteLine("const unsigned char mapptn[] = {");
                    str = ArrayToCHeaderString(_src.MapPattern, compress: false);
                }
                else
                {
                    sr.WriteLine("const unsigned char mapptn_compressed[] = {");
                    str = ArrayToCHeaderString(_src.MapPattern, compress: true);
                }
                sr.WriteLine(str);
                sr.WriteLine("};");
                str = "\t";
                sr.WriteLine("// Map data");
                if (type == MapType.CHeader)
                {
                    sr.WriteLine("const unsigned char mapData[] = {");
                    for (int i = 0; i < _src.MapWidth * _src.MapHeight; ++i)
                    {
                        if ((i != 0) && (i % 16) == 0) str += "\r\n\t";
                        str += $"{_src.MapData[i % _src.MapWidth, i / _src.MapWidth]}, ";
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
                string str;
                sr.WriteLine("mapwidth:");
                sr.WriteLine($"\tdb\t{_src.MapWidth}");
                sr.WriteLine("mapheight:");
                sr.WriteLine($"\tdb\t{_src.MapHeight}");
                sr.WriteLine("; Map patterns");
                if (type == MapType.ASMData)
                {
                    sr.WriteLine("mapptn:");
                    str = ArrayToASMString(_src.MapPattern, compress: false);
                }
                else
                {
                    sr.WriteLine("mapptn_compressed:");
                    str = ArrayToASMString(_src.MapPattern, compress: true);
                }
                sr.WriteLine(str);
                sr.WriteLine("; Map data");
                str = "";
                if (type == MapType.ASMData)
                {
                    sr.WriteLine("mapdat:");
                    for (int i = 0; i < _src.MapWidth * _src.MapHeight; ++i)
                    {
                        if (i % 16 == 0) str += "\tdb\t";
                        str += $"{_src.MapData[i % _src.MapWidth, i / _src.MapWidth]}";
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
                br.Write(_src.MapPattern);
            }
            else if (type == MapType.MSXBASIC_Map)
            {
                using BinaryWriter br = new BinaryWriter(new FileStream(path, FileMode.Create));
                int map_size = _src.MapWidth * _src.MapHeight;
                br.Write((byte)0xFE);                   // BSAVE/BLOAD header
                br.Write((byte)0x00);                   // Start address is 0x0000
                br.Write((byte)0x00);
                br.Write((byte)(map_size & 0xFF));      // End address depends on size
                br.Write((byte)(map_size >> 8));
                br.Write((byte)0);                      // Execution address
                br.Write((byte)0);
                for (int i = 0; i < _src.MapHeight; ++i)
                {
                    for (int j = 0; j < _src.MapWidth; ++j)
                    {
                        br.Write(_src.MapData[j, i]);
                    }
                }
            }
            else if (type == MapType.Raw_Pattern)
            {
                using BinaryWriter br = new BinaryWriter(new FileStream(path, FileMode.Create));
                br.Write(_src.MapPattern);
            }
            else if (type == MapType.Raw_Map)
            {
                using BinaryWriter br = new BinaryWriter(new FileStream(path, FileMode.Create));
                for (int i = 0; i < _src.MapHeight; ++i)
                {
                    for (int j = 0; j < _src.MapWidth; ++j)
                    {
                        br.Write(_src.MapData[j, i]);
                    }
                }
            }
            else if (type == MapType.RawCompressed_Pattern)
            {
                using BinaryWriter br = new BinaryWriter(new FileStream(path, FileMode.Create));
                byte[] comp = Compression.Create(Compression.Type.BytePair).Encode(_src.MapPattern);
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
                string str;
                sr.WriteLine("// Sprite generator table");
                if (type == SpriteType.CHeader)
                {
                    sr.WriteLine("const unsigned char sprptn[] = {");
                    str = ArrayToCHeaderString(_src.SpriteGen, compress: false);
                }
                else
                {
                    sr.WriteLine("const unsigned char sprptn_compressed[] = {");
                    str = ArrayToCHeaderString(_src.SpriteGen, compress: true);
                }
                sr.WriteLine(str);
                sr.WriteLine("};");
                if (!_src.Is9918)
                {
                    sr.WriteLine("// Sprite color table");
                    if (type == SpriteType.CHeader)
                    {
                        sr.WriteLine("const unsigned char sprclr[] = {");
                        str = ArrayToCHeaderString(_src.SpriteClr, compress: false);
                    }
                    else
                    {
                        sr.WriteLine("const unsigned char sprclr_compressed[] = {");
                        str = ArrayToCHeaderString(_src.SpriteClr, compress: true);
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
                string str;
                sr.WriteLine("; Sprite generator table");
                if (type == SpriteType.ASMData)
                {
                    sr.WriteLine("sprgen:");
                    str = ArrayToASMString(_src.SpriteGen, compress: false);
                }
                else
                {
                    sr.WriteLine("sprgen_compressed:");
                    str = ArrayToASMString(_src.SpriteGen, compress: true);
                }
                sr.WriteLine(str);
                if (!_src.Is9918)
                {
                    sr.WriteLine("; Sprite color table");
                    if (type == SpriteType.ASMData)
                    {
                        sr.WriteLine("sprclr:");
                        str = ArrayToASMString(_src.SpriteClr, compress: false);
                    }
                    else
                    {
                        sr.WriteLine("sprclr_compressed:");
                        str = ArrayToASMString(_src.SpriteClr, compress: true);
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
                br.Write(_src.SpriteGen);
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
                br.Write(_src.SpriteClr);
            }
            else if (type == SpriteType.Raw_Pattern)
            {
                using BinaryWriter br = new BinaryWriter(new FileStream(path, FileMode.Create));
                br.Write(_src.SpriteGen);
            }
            else if (type == SpriteType.Raw_Color)
            {
                using BinaryWriter br = new BinaryWriter(new FileStream(path, FileMode.Create));
                br.Write(_src.SpriteClr);
            }
            else if (type == SpriteType.RawCompressed_Pattern)
            {
                using BinaryWriter br = new BinaryWriter(new FileStream(path, FileMode.Create));
                byte[] comp = Compression.Create(Compression.Type.BytePair).Encode(_src.SpriteGen);
                br.Write(comp);
            }
            else if (type == SpriteType.RawCompressed_Color)
            {
                using BinaryWriter br = new BinaryWriter(new FileStream(path, FileMode.Create));
                byte[] comp = Compression.Create(Compression.Type.BytePair).Encode(_src.SpriteClr);
                br.Write(comp);
            }
        }
        //------------------------------------------------------------------------
        // Utilities
        private string ArrayToCHeaderString(byte[] src, bool compress, int index = 0, int length = -1)
        {
            if (length == -1) length = src.Length;
            byte[] src_partial = new byte[length];
            Array.Copy(src, index, src_partial, 0, length);
            string ret = "\t";
            if (compress)
            {
                byte[] comp = Compression.Create(Compression.Type.BytePair).Encode(src_partial);
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
        private string ArrayToASMString(byte[] src, bool compress, int index = 0, int length = -1)
        {
            if (length == -1) length = src.Length;
            byte[] src_partial = new byte[length];
            Array.Copy(src, index, src_partial, 0, length);
            string ret = "";
            if (compress)
            {
                byte[] comp = Compression.Create(Compression.Type.BytePair).Encode(src_partial);
                for (int i = 0; i < comp.Length; ++i)
                {
                    if (i % 16 == 0) ret += "\tdb\t";
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
            ushort[] offset_to_row = new ushort[_src.MapHeight];
            ushort offset = (ushort)(_src.MapHeight * 2);
            // Create data
            for(int y = 0; y < _src.MapHeight; ++y)
            {
                // Offset to the row data
                offset_to_row[y] = offset;
                // Compress each row
                List<byte> src_row = new List<byte>();
                for(int i = 0; i < _src.MapWidth; ++i)
                {
                    src_row.Add(_src.MapData[i, y]);
                }
                CompressionBase encoder = Compression.Create(Compression.Type.RunLength);
                byte[] comp = encoder.Encode(src_row.ToArray());
                comp_data.Add(comp);
                offset += (ushort)comp.Length;
            }
            // Make one compressed data
            List<byte> ret = new List<byte>();
            for(int i = 0; i < _src.MapHeight; ++i)
            {
                // Offset is stored as 2 byte little endian data
                ret.Add((byte)(offset_to_row[i] & 0xFF));
                ret.Add((byte)((offset_to_row[i]) >> 8 & 0xFF));
            }
            for (int i = 0; i < _src.MapHeight; ++i)
            {
                for(int j = 0; j < comp_data[i].Length; ++j)
                {
                    ret.Add(comp_data[i][j]);
                }
            }
            return ret.ToArray();
        }
    }
}
