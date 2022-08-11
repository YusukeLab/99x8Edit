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
        //------------------------------------------------------------------------
        // File IO
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
            this.updateAllViewItems();
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
            this.updateAllViewItems();
        }
        public void ExportPCG(ExportType type, String path)
        {
            if (type == ExportType.CHeader || type == ExportType.CCompressed)
            {
                StreamWriter sr = new StreamWriter(path, false);
                try
                {
                    sr.WriteLine("#ifndef __PCGDAT_H__");
                    sr.WriteLine("#define __PCGDAT_H__");
                    String str = "";
                    if (!isTMS9918)
                    {
                        sr.WriteLine("// Palette");
                        sr.WriteLine("const unsigned char palette[] = {");
                        str = arrayToCHeaderString(pltDat, false);
                        sr.WriteLine(str);
                        sr.WriteLine("};");
                    }
                    sr.WriteLine("// Character pattern generator table");
                    if (type == 0)
                    {
                        sr.WriteLine("const unsigned char ptngen[] = {");
                        str = arrayToCHeaderString(ptnGen, false);
                    }
                    else
                    {
                        sr.WriteLine("const unsigned char ptngen_compressed[] = {");
                        str = arrayToCHeaderString(ptnGen, true);
                    }
                    sr.WriteLine(str);
                    sr.WriteLine("};");
                    sr.WriteLine("// Character pattern color table");
                    if (type == ExportType.CHeader)
                    {
                        sr.WriteLine("const unsigned char ptnclr[] = {");
                        str = arrayToCHeaderString(ptnClr, false);
                    }
                    else
                    {
                        sr.WriteLine("const unsigned char ptnclr_compressed[] = {");
                        str = arrayToCHeaderString(ptnClr, true);
                    }
                    sr.WriteLine(str);
                    sr.WriteLine("};");
                    sr.WriteLine("// Name table");
                    if (type == ExportType.CHeader)
                    {
                        sr.WriteLine("const unsigned char nametable[] = {");
                        str = arrayToCHeaderString(nameTable, false);
                    }
                    else
                    {
                        sr.WriteLine("const unsigned char nametable_compressed[] = {");
                        str = arrayToCHeaderString(nameTable, true);
                    }
                    sr.WriteLine(str);
                    sr.WriteLine("};");
                    sr.WriteLine("#endif");
                }
                catch(Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    sr.Close();
                }
            }
            else if (type == ExportType.ASMData || type == ExportType.ASMCompressed)
            {
                StreamWriter sr = new StreamWriter(path, false);
                try
                {
                    sr.WriteLine("; PCG Data");
                    sr.WriteLine("; this export data is not tested");
                    String str = "";
                    if (!isTMS9918)
                    {
                        sr.WriteLine("; Palette r8b8g8");
                        sr.WriteLine("palette:");
                        str = arrayToASMString(pltDat, false);
                        sr.WriteLine(str);
                    }
                    sr.WriteLine("; Character pattern generator table");
                    if (type == ExportType.ASMData)
                    {
                        sr.WriteLine("ptngen:");
                        str = arrayToASMString(ptnGen, false);
                    }
                    else
                    {
                        sr.WriteLine("ptngen_compressed:");
                        str = arrayToASMString(ptnGen, true);
                    }
                    sr.WriteLine(str);
                    sr.WriteLine("; Character pattern color table");
                    if (type == ExportType.ASMData)
                    {
                        sr.WriteLine("prnclr:");
                        str = arrayToASMString(ptnClr, false);
                    }
                    else
                    {
                        sr.WriteLine("prnclr_compressed:");
                        str = arrayToASMString(ptnClr, true);
                    }
                    sr.WriteLine(str);
                    sr.WriteLine("; Name table");
                    if (type == ExportType.ASMData)
                    {
                        sr.WriteLine("namtbl:");
                        str = arrayToASMString(nameTable, false);
                    }
                    else
                    {
                        sr.WriteLine("namtbl_compressed:");
                        str = arrayToASMString(nameTable, true);
                    }
                    sr.WriteLine(str);
                }
                catch(Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    sr.Close();
                }
            }
            else if (type == ExportType.MSXBASIC)
            {
                BinaryWriter br = new BinaryWriter(new FileStream(path + "_GEN", FileMode.Create));
                try
                {
                    br.Write((byte)0xFE);       // BSAVE/BLOAD header
                    br.Write((byte)0);          // Start address is 0
                    br.Write((byte)0);
                    br.Write((byte)0xFF);       // End address is 0x07FF
                    br.Write((byte)0x07);
                    br.Write((byte)0);          // Execution address
                    br.Write((byte)0);
                    br.Write(ptnGen);
                }
                catch(Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    br.Close();
                }
                br = new BinaryWriter(new FileStream(path + "_CLR", FileMode.Create));
                try
                {
                    br.Write((byte)0xFE);       // BSAVE/BLOAD header
                    br.Write((byte)0x00);       // Start address is 0x2000
                    br.Write((byte)0x20);
                    br.Write((byte)0xFF);       // End address is 0x27FF
                    br.Write((byte)0x27);
                    br.Write((byte)0);          // Execution address
                    br.Write((byte)0);
                    br.Write(ptnClr);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    br.Close();
                }
            }
            else if (type == ExportType.Raw)
            {
                BinaryWriter br = new BinaryWriter(new FileStream(path + "_GEN", FileMode.Create));
                try
                {
                    br.Write(ptnGen);
                }
                catch(Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    br.Close();
                }
                br = new BinaryWriter(new FileStream(path + "_CLR", FileMode.Create));
                try
                {
                    br.Write(ptnClr);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    br.Close();
                }
            }
            else if (type == ExportType.RawCompressed)
            {
                BinaryWriter br = new BinaryWriter(new FileStream(path + "_GEN", FileMode.Create));
                try
                {
                    byte[] comp = Compression.Create(Compression.Type.BytePair).Compress(ptnGen);
                    br.Write(comp);
                }
                catch(Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    br.Close();
                }
                br = new BinaryWriter(new FileStream(path + "_CLR", FileMode.Create));
                try
                {
                    byte[] comp = Compression.Create(Compression.Type.BytePair).Compress(ptnClr);
                    br.Write(comp);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    br.Close();
                }
            }
        }
        public void ExportMap(ExportType type, String path)
        {
            if (type == ExportType.CHeader || type == ExportType.CCompressed)
            {
                StreamWriter sr = new StreamWriter(path, false);
                try
                {
                    sr.WriteLine("#ifndef __MAPDAT_H__");
                    sr.WriteLine("#define __MAPDAT_H__");
                    sr.WriteLine(String.Format("#define MAP_W   ({0})", mapWidth));
                    sr.WriteLine(String.Format("#define MAP_H   ({0})", mapHeight));
                    String str = "\t";
                    sr.WriteLine("// Map patterns");
                    sr.WriteLine("const unsigned char mapptn[] = {");
                    str = arrayToCHeaderString(mapPattern, false);
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
                        byte[] comp = this.compressMapData();
                        for (int i = 0; i < comp.Length; ++i)
                        {
                            if ((i != 0) && (i % 16) == 0) str += "\r\n\t";
                            str += String.Format("{0}, ", comp[i]);
                        }
                    }
                    sr.WriteLine(str);
                    sr.WriteLine("};");
                    sr.WriteLine("#endif");
                }
                catch(Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    sr.Close();
                }
            }
            else if (type == ExportType.ASMData || type == ExportType.ASMCompressed)
            {
                StreamWriter sr = new StreamWriter(path, false);
                try
                {
                    sr.WriteLine("; Map Data");
                    sr.WriteLine("; this export data is not tested");
                    String str = "";
                    sr.WriteLine("mapwidth:");
                    sr.WriteLine(String.Format("\tdb\t{0}", mapWidth));
                    sr.WriteLine("mapheight:");
                    sr.WriteLine(String.Format("\tdb\t{0}", mapHeight));
                    sr.WriteLine("; Map patterns");
                    sr.WriteLine("mapptn:");
                    str = arrayToASMString(mapPattern, false);
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
                        byte[] comp = this.compressMapData();
                        for (int i = 0; i < comp.Length; ++i)
                        {
                            if (i % 16 == 0) str += "\tdb\t";
                            str += String.Format("{0}", comp[i]);
                            if (i % 16 == 15) str += "\r\n";
                            else str += ", ";
                        }
                    }
                    sr.WriteLine(str);
                }
                catch(Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    sr.Close();
                }
            }
            else if (type == ExportType.MSXBASIC)
            {
                throw new Exception("Map data cannot be BLOADed in BASIC");
            }
            else if (type == ExportType.Raw)
            {
                BinaryWriter br = new BinaryWriter(new FileStream(path + "_PTN", FileMode.Create));
                try
                {
                    br.Write(mapPattern);
                }
                catch(Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    br.Close();
                }
                br = new BinaryWriter(new FileStream(path + "_DAT", FileMode.Create));
                try
                {
                    for (int i = 0; i < mapHeight; ++i)
                    {
                        for (int j = 0; j < mapWidth; ++j)
                        {
                            br.Write(mapData[j, i]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    br.Close();
                }
            }
            else if (type == ExportType.RawCompressed)
            {
                BinaryWriter br = new BinaryWriter(new FileStream(path + "_PTN", FileMode.Create));
                try
                {
                    byte[] comp = Compression.Create(Compression.Type.BytePair).Compress(mapPattern);
                    br.Write(comp);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    br.Close();
                }
                br = new BinaryWriter(new FileStream(path + "_DAT", FileMode.Create));
                try
                {
                    br.Write(this.compressMapData());
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    br.Close();
                }
            }
        }
        public void ExportSprites(ExportType type, String path)
        {
            // Set the CC flags of the sprite color
            /*
                We don't update the CC flags when editing, since it will be a quite mess
                when there are copy, paste and other actions to overlayed sprite.
                Anyway, we only need the CC flags for exporting, so we're going to update here.
             */
            this.setSpriteCCFlags();
            // Start exporting
            if (type == ExportType.CHeader || type == ExportType.CCompressed)
            {
                StreamWriter sr = new StreamWriter(path, false);
                try
                {
                    sr.WriteLine("#ifndef __SPRITEDAT_H__");
                    sr.WriteLine("#define __SPRITEDAT_H__");
                    String str = "";
                    sr.WriteLine("// Sprite generator table");
                    if (type == ExportType.CHeader)
                    {
                        sr.WriteLine("const unsigned char sprptn[] = {");
                        str = arrayToCHeaderString(spriteGen, false);
                    }
                    else
                    {
                        sr.WriteLine("const unsigned char sprptn_compressed[] = {");
                        str = arrayToCHeaderString(spriteGen, true);
                    }
                    sr.WriteLine(str);
                    sr.WriteLine("};");
                    if (!isTMS9918)
                    {
                        byte[] sprite_clr16x16 = new byte[16 * 64];
                        for (int i = 0; i < 64; ++i)
                        {
                            for (int j = 0; j < 16; ++j)
                            {
                                // 8x8 color table to 16x16 color table
                                sprite_clr16x16[i * 16 + j] = spriteClr2[i * 32 + j];
                            }
                        }
                        sr.WriteLine("// Sprite color table");
                        if (type == ExportType.CHeader)
                        {
                            sr.WriteLine("const unsigned char sprclr[] = {");
                            str = arrayToCHeaderString(sprite_clr16x16, false);
                        }
                        else
                        {
                            sr.WriteLine("const unsigned char sprclr_compressed[] = {");
                            str = arrayToCHeaderString(sprite_clr16x16, true);
                        }
                        sr.WriteLine(str);
                        sr.WriteLine("};");
                    }
                    sr.WriteLine("#endif");
                }
                catch(Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    sr.Close();
                }
            }
            else if (type == ExportType.ASMData || type == ExportType.ASMCompressed)
            {
                StreamWriter sr = new StreamWriter(path, false);
                try
                {
                    sr.WriteLine("; Sprite Data");
                    sr.WriteLine("; this export data is not tested");
                    String str = "";
                    sr.WriteLine("; Sprite generator table");
                    str = "";
                    if (type == ExportType.ASMData)
                    {
                        sr.WriteLine("sprgen:");
                        str = arrayToASMString(spriteGen, false);
                    }
                    else
                    {
                        sr.WriteLine("sprgen_compressed:");
                        str = arrayToASMString(spriteGen, true);
                    }
                    sr.WriteLine(str);
                    if (!isTMS9918)
                    {
                        byte[] sprite_clr16x16 = new byte[16 * 64];
                        for (int i = 0; i < 64; ++i)
                        {
                            for (int j = 0; j < 16; ++j)
                            {
                                // 8x8 color table to 16x16 color table
                                sprite_clr16x16[i * 16 + j] = spriteClr2[i * 32 + j];
                            }
                        }
                        sr.WriteLine("; Sprite color table");
                        str = "";
                        if (type == ExportType.ASMData)
                        {
                            sr.WriteLine("sprclr:");
                            str = arrayToASMString(sprite_clr16x16, false);
                        }
                        else
                        {
                            sr.WriteLine("sprclr_compressed:");
                            str = arrayToASMString(sprite_clr16x16, true);
                        }
                        sr.WriteLine(str);
                    }
                }
                catch(Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    sr.Close();
                }
            }
            else if (type == ExportType.MSXBASIC)
            {
                BinaryWriter br = new BinaryWriter(new FileStream(path, FileMode.Create));
                try
                {
                    br.Write((byte)0xFE);       // BSAVE/BLOAD header
                    br.Write((byte)0x00);       // Start address is 0x3800
                    br.Write((byte)0x38);
                    br.Write((byte)0xFF);       // End address is 0x3FFF
                    br.Write((byte)0x3F);
                    br.Write((byte)0);          // Execution address
                    br.Write((byte)0);
                    br.Write(spriteGen);
                }
                catch(Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    br.Close();
                }
            }
            else if (type == ExportType.Raw)
            {
                BinaryWriter br = new BinaryWriter(new FileStream(path, FileMode.Create));
                try
                {
                    br.Write(spriteGen);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    br.Close();
                }
                br = new BinaryWriter(new FileStream(path + "_color", FileMode.Create));
                try
                {
                    byte[] sprite_clr16x16 = new byte[16 * 64];
                    for (int i = 0; i < 64; ++i)
                    {
                        for (int j = 0; j < 16; ++j)
                        {
                            // 8x8 color table to 16x16 color table
                            sprite_clr16x16[i * 16 + j] = spriteClr2[i * 32 + j];
                        }
                    }
                    br.Write(sprite_clr16x16);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    br.Close();
                }
            }
            else if (type == ExportType.RawCompressed)
            {
                BinaryWriter br = new BinaryWriter(new FileStream(path, FileMode.Create));
                try
                {
                    byte[] comp = Compression.Create(Compression.Type.BytePair).Compress(spriteGen);
                    br.Write(comp);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    br.Close();
                }
                br = new BinaryWriter(new FileStream(path + "_color", FileMode.Create));
                try
                {
                    byte[] sprite_clr16x16 = new byte[16 * 64];
                    for (int i = 0; i < 64; ++i)
                    {
                        for (int j = 0; j < 16; ++j)
                        {
                            // 8x8 color table to 16x16 color table
                            sprite_clr16x16[i * 16 + j] = spriteClr2[i * 32 + j];
                        }
                    }
                    byte[] comp = Compression.Create(Compression.Type.BytePair).Compress(sprite_clr16x16);
                    br.Write(comp);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    br.Close();
                }
            }
        }
        //------------------------------------------------------------------------
        // Utilities
        private String arrayToCHeaderString(byte[] src, bool compress)
        {
            String ret = "\t";
            if (compress)
            {
                byte[] comp = Compression.Create(Compression.Type.BytePair).Compress(src);
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
        private String arrayToASMString(byte[] src, bool compress)
        {
            String ret = "";
            if (compress)
            {
                byte[] comp = Compression.Create(Compression.Type.BytePair).Compress(src);
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
        private byte[] compressMapData()
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
                CompressionBase encoder = Compression.Create(Compression.Type.RunLength);
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
