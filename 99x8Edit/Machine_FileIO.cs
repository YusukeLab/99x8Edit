using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;

namespace _99x8Edit
{
    // Partial definition of Machine class, offers file access
    public partial class Machine
    {
        // Supported Export types
        public enum ExportType
        {
            CHeader = 0,
            CCompressed,
            ASMData,
            ASMCompressed,
            MSXBASIC,
            Raw,
            RawCompressed,
        }
        public List<String> exportTypeList = new List<String>()
        {
            {"C header"},
            {"C compressed"},
            {"ASM data"},
            {"ASM compressed"},
            {"BIN(MSX BASIC)"},
            {"Raw data"},
            {"Raw compressed"}
        };
        public List<String> exportTypeExt = new List<String>()
        {
            {".h"},
            {".h"},
            {".asm"},
            {".asm"},
            {".bin"},
            {".raw"},
            {".raw"},
        };
        public void SaveAllSettings(BinaryWriter br)
        {
            // PCG
            br.Write(ptnGen);           // Pattern generator table
            br.Write(ptnClr);           // Pattern color table
            br.Write(nameTable);        // Name table
            br.Write(pltDat);           // Palette
            br.Write(isTMS9918);        // Based on TMS9918 or not
            // Sprites
            br.Write(spriteGen);        // Sprite patten generator table
            br.Write(spriteClr1);       // Sprite color for mode1
            br.Write(spriteClr2);       // Sprite color for mode2
            br.Write(spriteOverlay);    // Sprite overlay flags
            // Map
            br.Write(mapPattern);       // Map pattern
            br.Write((Int32)mapWidth);
            br.Write((Int32)mapHeight);
            for (int i = 0; i < mapHeight; ++i)
            {
                for (int j = 0; j < mapWidth; ++j)
                {
                    br.Write((byte)mapData[j, i]);
                }
            }
        }
        public void LoadAllSettings(BinaryReader br)
        {
            // PCG
            br.Read(ptnGen);                // Pattern generator table
            br.Read(ptnClr);                // Pattern color table
            br.Read(nameTable);             // Name table
            br.Read(pltDat);                // Palette
            isTMS9918 = br.ReadBoolean();   // Based on TMS9918 or not
            // Sprites
            br.Read(spriteGen);             // Sprite patten generator table
            br.Read(spriteClr1);            // Sprite color for mode1
            br.Read(spriteClr2);            // Sprite color for mode2
            br.Read(spriteOverlay);         // Sprite overlay flags
            // Map
            br.Read(mapPattern);       // Map pattern
            mapWidth = br.ReadInt32();
            mapHeight = br.ReadInt32();
            for (int i = 0; i < mapHeight; ++i)
            {
                for (int j = 0; j < mapWidth; ++j)
                {
                    mapData[j, i] = br.ReadByte();
                }
            }
            // Update bitmaps
            this.UpdateAllViewItems();
        }
        public void SavePaletteSettings(BinaryWriter br)
        {
            br.Write(pltDat);
        }
        public void LoadPaletteSettings(BinaryReader br)
        {
            br.Read(pltDat);
            for (int i = 0; i < 16; ++i)
            {
                // Set the windows color to palette based on internal palette data
                int R = (pltDat[i * 2] >> 4);
                int G = (pltDat[i * 2 + 1]);
                int B = (pltDat[i * 2] & 0x0F);
                R = (R * 255) / 7;
                G = (G * 255) / 7;
                B = (B * 255) / 7;
                colorOf[i] = Color.FromArgb(R, G, B);
            }
            this.UpdateAllViewItems();
        }
        public void ExportPCG(ExportType type, String path)
        {
            if (type == ExportType.CHeader || type == ExportType.CCompressed)
            {
                StreamWriter sr = new StreamWriter(path, false);
                sr.WriteLine("#ifndef __PCGDAT_H__");
                sr.WriteLine("#define __PCGDAT_H__");
                String str = "";
                if (!isTMS9918)
                {
                    sr.WriteLine("// Palette");
                    sr.WriteLine("const unsigned char palette[] = {");
                    str = ArrayToCHeaderString(pltDat, false);
                    sr.WriteLine(str);
                    sr.WriteLine("};");
                }
                sr.WriteLine("// Character pattern generator table");
                if (type == 0)
                {
                    sr.WriteLine("const unsigned char ptngen[] = {");
                    str = ArrayToCHeaderString(ptnGen, false);
                }
                else
                {
                    sr.WriteLine("const unsigned char ptngen_compressed[] = {");
                    str = ArrayToCHeaderString(ptnGen, true);
                }
                sr.WriteLine(str);
                sr.WriteLine("};");
                sr.WriteLine("// Character pattern color table");
                if (type == ExportType.CHeader)
                {
                    sr.WriteLine("const unsigned char ptnclr[] = {");
                    str = ArrayToCHeaderString(ptnClr, false);
                }
                else
                {
                    sr.WriteLine("const unsigned char ptnclr_compressed[] = {");
                    str = ArrayToCHeaderString(ptnClr, true);
                }
                sr.WriteLine(str);
                sr.WriteLine("};");
                sr.WriteLine("// Name table");
                if (type == ExportType.CHeader)
                {
                    sr.WriteLine("const unsigned char nametable[] = {");
                    str = ArrayToCHeaderString(nameTable, false);
                }
                else
                {
                    sr.WriteLine("const unsigned char nametable_compressed[] = {");
                    str = ArrayToCHeaderString(nameTable, true);
                }
                sr.WriteLine(str);
                sr.WriteLine("};");
                sr.WriteLine("#endif");
                sr.Close();
            }
            else if (type == ExportType.ASMData || type == ExportType.ASMCompressed)
            {
                StreamWriter sr = new StreamWriter(path, false);
                sr.WriteLine("; PCG Data");
                sr.WriteLine("; this export data is not tested");
                String str = "";
                if (!isTMS9918)
                {
                    sr.WriteLine("; Palette r8b8g8");
                    sr.WriteLine("palette:");
                    str = ArrayToASMString(pltDat, false);
                    sr.WriteLine(str);
                }
                sr.WriteLine("; Character pattern generator table");
                if (type == ExportType.ASMData)
                {
                    sr.WriteLine("ptngen:");
                    str = ArrayToASMString(ptnGen, false);
                }
                else
                {
                    sr.WriteLine("ptngen_compressed:");
                    str = ArrayToASMString(ptnGen, true);
                }
                sr.WriteLine(str);
                sr.WriteLine("; Character pattern color table");
                if (type == ExportType.ASMData)
                {
                    sr.WriteLine("prnclr:");
                    str = ArrayToASMString(ptnClr, false);
                }
                else
                {
                    sr.WriteLine("prnclr_compressed:");
                    str = ArrayToASMString(ptnClr, true);
                }
                sr.WriteLine(str);
                sr.WriteLine("; Name table");
                if (type == ExportType.ASMData)
                {
                    sr.WriteLine("namtbl:");
                    str = ArrayToASMString(nameTable, false);
                }
                else
                {
                    sr.WriteLine("namtbl_compressed:");
                    str = ArrayToASMString(nameTable, true);
                }
                sr.WriteLine(str);
                sr.Close();
            }
            else if (type == ExportType.MSXBASIC)
            {
                BinaryWriter br = new BinaryWriter(new FileStream(path + "_GEN", FileMode.Create));
                br.Write((byte)0xFE);       // BSAVE/BLOAD header
                br.Write((byte)0);          // Start address is 0
                br.Write((byte)0);
                br.Write((byte)0xFF);       // End address is 0x07FF
                br.Write((byte)0x07);
                br.Write((byte)0);          // Execution address
                br.Write((byte)0);
                br.Write(ptnGen);
                br.Close();
                br = new BinaryWriter(new FileStream(path + "_CLR", FileMode.Create));
                br.Write((byte)0xFE);       // BSAVE/BLOAD header
                br.Write((byte)0x00);       // Start address is 0x2000
                br.Write((byte)0x20);
                br.Write((byte)0xFF);       // End address is 0x27FF
                br.Write((byte)0x27);
                br.Write((byte)0);          // Execution address
                br.Write((byte)0);
                br.Write(ptnClr);
                br.Close();
            }
            else if (type == ExportType.Raw)
            {
                BinaryWriter br = new BinaryWriter(new FileStream(path + "_GEN", FileMode.Create));
                br.Write(ptnGen);
                br.Close();
                br = new BinaryWriter(new FileStream(path + "_CLR", FileMode.Create));
                br.Write(ptnClr);
                br.Close();
            }
            else if (type == ExportType.RawCompressed)
            {
                BinaryWriter br = new BinaryWriter(new FileStream(path + "_GEN", FileMode.Create));
                byte[] comp = CompressionBase.CreateInstance(CompressionBase.Type.BytePair).Compress(ptnGen);
                br.Write(comp);
                br.Close();
                br = new BinaryWriter(new FileStream(path + "_CLR", FileMode.Create));
                comp = CompressionBase.CreateInstance(CompressionBase.Type.BytePair).Compress(ptnClr);
                br.Write(comp);
                br.Close();
            }
        }
        public void ExportMap(ExportType type, String path)
        {
            if (type == ExportType.CHeader || type == ExportType.CCompressed)
            {
                StreamWriter sr = new StreamWriter(path, false);
                sr.WriteLine("#ifndef __MAPDAT_H__");
                sr.WriteLine("#define __MAPDAT_H__");
                sr.WriteLine("// this export data is not tested");
                sr.WriteLine(String.Format("#define MAP_W   ({0})", mapWidth));
                sr.WriteLine(String.Format("#define MAP_H   ({0})", mapHeight));
                String str = "\t";
                sr.WriteLine("// Map patterns");
                sr.WriteLine("const unsigned char mapptn[] = {");
                str = ArrayToCHeaderString(mapPattern, false);
                sr.WriteLine(str);
                sr.WriteLine("};");
                str = "\t";
                sr.WriteLine("// Map data");
                if (type == ExportType.CHeader)
                {
                    sr.WriteLine("const unsigned char mapData[] = {");
                    for (int i = 0; i < mapWidth * mapHeight; ++i)
                    {
                        if ((i != 0) && (i % 16) == 0) str += "\r\n\t";
                        str += String.Format("{0}, ", mapData[i % mapWidth, i / mapWidth]);
                    }
                }
                else
                {
                    sr.WriteLine("const unsigned char mapData_compressed[] = {");
                    byte[] comp = this.CompressMapData();
                    for (int i = 0; i < comp.Length; ++i)
                    {
                        if ((i != 0) && (i % 16) == 0) str += "\r\n\t";
                        str += String.Format("{0}, ", comp[i]);
                    }
                }
                sr.WriteLine(str);
                sr.WriteLine("};");
                sr.WriteLine("#endif");
                sr.Close();
            }
            else if (type == ExportType.ASMData || type == ExportType.ASMCompressed)
            {
                StreamWriter sr = new StreamWriter(path, false);
                sr.WriteLine("; Map Data");
                sr.WriteLine("; this export data is not tested");
                String str = "";
                sr.WriteLine("mapwidth:");
                sr.WriteLine(String.Format("\tdb\t{0}", mapWidth));
                sr.WriteLine("mapheight:");
                sr.WriteLine(String.Format("\tdb\t{0}", mapHeight));
                sr.WriteLine("; Map patterns");
                sr.WriteLine("mapptn:");
                str = ArrayToASMString(mapPattern, false);
                sr.WriteLine(str);
                sr.WriteLine("; Map data");
                str = "";
                if (type == ExportType.ASMData)
                {
                    sr.WriteLine("mapdat:");
                    for (int i = 0; i < mapWidth * mapHeight; ++i)
                    {
                        if (i % 16 == 0) str += "\tdb\t";
                        str += String.Format("{0}", mapData[i % mapWidth, i / mapWidth]);
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
                        str += String.Format("{0}", comp[i]);
                        if (i % 16 == 15) str += "\r\n";
                        else str += ", ";
                    }
                }
                sr.WriteLine(str);
                sr.Close();
            }
            else if (type == ExportType.MSXBASIC)
            {
                throw new Exception("Map data cannot be BLOADed in BASIC");
            }
            else if (type == ExportType.Raw)
            {
                BinaryWriter br = new BinaryWriter(new FileStream(path + "_PTN", FileMode.Create));
                br.Write(mapPattern);
                br.Close();
                br = new BinaryWriter(new FileStream(path + "_DAT", FileMode.Create));
                for(int i = 0; i < mapHeight; ++i)
                {
                    for(int j = 0; j < mapWidth; ++j)
                    {
                        br.Write(mapData[j, i]);
                    }
                }
                br.Close();
            }
            else if (type == ExportType.RawCompressed)
            {
                BinaryWriter br = new BinaryWriter(new FileStream(path + "_PTN", FileMode.Create));
                byte[] comp = CompressionBase.CreateInstance(CompressionBase.Type.BytePair).Compress(mapPattern);
                br.Write(comp);
                br.Close();
                br = new BinaryWriter(new FileStream(path + "_DAT", FileMode.Create));
                br.Write(this.CompressMapData());
                br.Close();
            }
        }
        public void ExportSprites(ExportType type, String path)
        {
            if (type == ExportType.CHeader || type == ExportType.CCompressed)
            {
                StreamWriter sr = new StreamWriter(path, false);
                sr.WriteLine("#ifndef __SPRITEDAT_H__");
                sr.WriteLine("#define __SPRITEDAT_H__");
                sr.WriteLine("// this export data is not tested");
                String str = "";
                sr.WriteLine("// Sprite generator table");
                if (type == ExportType.CHeader)
                {
                    sr.WriteLine("const unsigned char sprptn[] = {");
                    str = ArrayToCHeaderString(spriteGen, false);
                }
                else
                {
                    sr.WriteLine("const unsigned char sprptn_compressed[] = {");
                    str = ArrayToCHeaderString(spriteGen, true);
                }
                sr.WriteLine(str);
                sr.WriteLine("};");
                if (!isTMS9918)
                {
                    sr.WriteLine("// Sprite color table");
                    if (type == ExportType.CHeader)
                    {
                        sr.WriteLine("const unsigned char sprclr[] = {");
                        str = ArrayToCHeaderString(spriteClr2, false);
                    }
                    else
                    {
                        sr.WriteLine("const unsigned char sprclr_compressed[] = {");
                        str = ArrayToCHeaderString(spriteClr2, true);
                    }
                    sr.WriteLine(str);
                    sr.WriteLine("};");
                }
                sr.WriteLine("#endif");
                sr.Close();
            }
            else if (type == ExportType.ASMData || type == ExportType.ASMCompressed)
            {
                StreamWriter sr = new StreamWriter(path, false);
                sr.WriteLine("; Sprite Data");
                sr.WriteLine("; this export data is not tested");
                String str = "";
                sr.WriteLine("; Sprite generator table");
                str = "";
                if (type == ExportType.ASMData)
                {
                    sr.WriteLine("sprgen:");
                    str = ArrayToASMString(spriteGen, false);
                }
                else
                {
                    sr.WriteLine("sprgen_compressed:");
                    str = ArrayToASMString(spriteGen, true);
                }
                sr.WriteLine(str);
                if (!isTMS9918)
                {
                    sr.WriteLine("; Sprite color table");
                    str = "";
                    if (type == ExportType.ASMData)
                    {
                        sr.WriteLine("sprclr:");
                        str = ArrayToASMString(spriteClr2, false);
                    }
                    else
                    {
                        sr.WriteLine("sprclr_compressed:");
                        str = ArrayToASMString(spriteClr2, true);
                    }
                    sr.WriteLine(str);
                }
                sr.Close();
            }
            else if (type == ExportType.MSXBASIC)
            {
                BinaryWriter br = new BinaryWriter(new FileStream(path, FileMode.Create));
                br.Write((byte)0xFE);       // BSAVE/BLOAD header
                br.Write((byte)0x00);       // Start address is 0x3800
                br.Write((byte)0x38);
                br.Write((byte)0xFF);       // End address is 0x3FFF
                br.Write((byte)0x3F);
                br.Write((byte)0);          // Execution address
                br.Write((byte)0);
                br.Write(spriteGen);
                br.Close();
            }
            else if (type == ExportType.Raw)
            {
                BinaryWriter br = new BinaryWriter(new FileStream(path, FileMode.Create));
                br.Write(spriteGen);
                br.Close();
            }
            else if (type == ExportType.RawCompressed)
            {
                BinaryWriter br = new BinaryWriter(new FileStream(path, FileMode.Create));
                byte[] comp = CompressionBase.CreateInstance(CompressionBase.Type.BytePair).Compress(spriteGen);
                br.Write(comp);
                br.Close();
            }
        }
        private String ArrayToCHeaderString(byte[] src, bool compress)
        {
            String ret = "\t";
            if (compress)
            {
                byte[] comp = CompressionBase.CreateInstance(CompressionBase.Type.BytePair).Compress(src);
                for (int i = 0; i < comp.Length; ++i)
                {
                    if ((i != 0) && (i % 16) == 0) ret += "\r\n\t";
                    ret += String.Format("{0}, ", comp[i]);
                }
            }
            else
            {
                for (int i = 0; i < src.Length; ++i)
                {
                    if ((i != 0) && (i % 16) == 0) ret += "\r\n\t";
                    ret += String.Format("{0}, ", src[i]);
                }
            }
            return ret;
        }
        private String ArrayToASMString(byte[] src, bool compress)
        {
            String ret = "";
            if (compress)
            {
                byte[] comp = CompressionBase.CreateInstance(CompressionBase.Type.BytePair).Compress(src);
                for (int i = 0; i < comp.Length; ++i)
                {
                    if (i % 16 == 0) ret += "\tdb\t";
                    ret += String.Format("{0}", comp[i]);
                    if (i % 16 == 15) ret += "\r\n";
                    else ret += ", ";
                }
            }
            else
            {
                for (int i = 0; i < src.Length; ++i)
                {
                    if (i % 16 == 0) ret += "\tdb\t";
                    ret += String.Format("{0}", src[i]);
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
            ushort[] offset_to_row = new ushort[mapHeight];
            ushort offset = (ushort)(mapHeight * 2);
            // Create data
            for(int y = 0; y < mapHeight; ++y)
            {
                // Offset to the row data
                offset_to_row[y] = offset;
                // Compress each row
                List<byte> src_row = new List<byte>();
                for(int i = 0; i < mapWidth; ++i)
                {
                    src_row.Add(mapData[i, y]);
                }
                CompressionBase encoder = CompressionBase.CreateInstance(CompressionBase.Type.RunLength);
                byte[] comp = encoder.Compress(src_row.ToArray() as byte[]);
                comp_data.Add(comp);
                offset += (ushort)comp.Length;
            }
            // Make one compressed data
            List<byte> ret = new List<byte>();
            for(int i = 0; i < mapHeight; ++i)
            {
                // Offset is stored as 2 byte little endian data
                ret.Add((byte)(offset_to_row[i] & 0xFF));
                ret.Add((byte)((offset_to_row[i]) >> 8 & 0xFF));
            }
            for (int i = 0; i < mapHeight; ++i)
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
