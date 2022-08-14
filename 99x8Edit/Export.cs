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
        public enum Type
        {
            CHeader = 0,
            CCompressed,
            ASMData,
            ASMCompressed,
            MSXBASIC,
            Raw,
            RawCompressed,
        }
        static public List<String> TypeList = new List<String>()
        {
            {"C header"},
            {"C compressed"},
            {"ASM data"},
            {"ASM compressed"},
            {"BIN(MSX BASIC)"},
            {"Raw data"},
            {"Raw compressed"}
        };
        static public List<String> TypeExt = new List<String>()
        {
            {".h"},
            {".h"},
            {".asm"},
            {".asm"},
            {".bin"},
            {".raw"},
            {".raw"},
        };
        static public string Filter = "C header(*.h)|*.h|"
                                    + "C header compressed(*.h)|*.h|"
                                    + "ASM data(*.asm)|*.asm|"
                                    + "ASM data compressed(*.asm)|*.asm|"
                                    + "MSX BASIC(*.bin)|*.bin|"
                                    + "Raw data(*.raw)|*.raw|"
                                    + "Raw data compressed(*.raw)|*.raw";
        private byte[] ptnGen = new byte[256 * 8];    // Pattern generator table
        private byte[] ptnClr = new byte[256 * 8];    // Pattern color table
        private byte[] nameTable = new byte[768];     // Sandbox(Pattern name table)
        private byte[] pltDat = { 0x00, 0x00, 0x00, 0x00, 0x11, 0x06, 0x33, 0x07,
                                  0x17, 0x01, 0x27, 0x03, 0x51, 0x01, 0x27, 0x06,
                                  0x71, 0x01, 0x73, 0x03, 0x61, 0x06, 0x64, 0x06,
                                  0x11, 0x04, 0x65, 0x02, 0x55, 0x05, 0x77, 0x07};  // Palette [RB][xG][RB][xG][RB]...
        private bool isTMS9918 = false;
        private byte[] mapPattern = new byte[256 * 4];  // One pattern mede by four characters
        private byte[,] mapData = new byte[64, 64];     // Map data[x, y](0..255)
        private Int32 mapWidth = 64;
        private Int32 mapHeight = 64;
        private byte[] spriteGen = new byte[256 * 8];   // Sprite pattern generator table
        private byte[] spriteClr1 = new byte[256];      // Sprite color(mode1)
        private byte[] spriteClr2 = new byte[256 * 8];  // Sprite color(mode2)
        private byte[] spriteOverlay = new byte[64];    // Will overlay next sprite(1) or not(0)
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
                      byte[] sprclr,
                      byte[] sprclr2,
                      byte[] sprover)
        {
            ptnGen = pcggen.Clone() as byte[];
            ptnClr = pcgclr.Clone() as byte[];
            nameTable = sandbox.Clone() as byte[];
            pltDat = plt.Clone() as byte[];
            isTMS9918 = is9918;
            mapPattern = mapptn.Clone() as byte[];
            mapData = mapdat.Clone() as byte[,];
            mapWidth = mapw;
            mapHeight = maph;
            spriteGen = sprgen.Clone() as byte[];
            spriteClr1 = sprclr.Clone() as byte[];
            spriteClr2 = sprclr2.Clone() as byte[];
            spriteOverlay = sprover.Clone() as byte[];
        }
        //------------------------------------------------------------------------
        // Methods
        internal void ExportPCG(Type type, String path)
        {
            if (type == Type.CHeader || type == Type.CCompressed)
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
                    if (type == Type.CHeader)
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
                    if (type == Type.CHeader)
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
            else if (type == Type.ASMData || type == Type.ASMCompressed)
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
                        str = ArrayToASMString(pltDat, false);
                        sr.WriteLine(str);
                    }
                    sr.WriteLine("; Character pattern generator table");
                    if (type == Type.ASMData)
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
                    if (type == Type.ASMData)
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
                    if (type == Type.ASMData)
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
            else if (type == Type.MSXBASIC)
            {
                BinaryWriter br = new BinaryWriter(new FileStream(path + "_GeneratorTable", FileMode.Create));
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
                br = new BinaryWriter(new FileStream(path + "_ColorTable", FileMode.Create));
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
            else if (type == Type.Raw)
            {
                BinaryWriter br = new BinaryWriter(new FileStream(path + "_GeneratorTable", FileMode.Create));
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
                br = new BinaryWriter(new FileStream(path + "_ColorTable", FileMode.Create));
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
            else if (type == Type.RawCompressed)
            {
                BinaryWriter br = new BinaryWriter(new FileStream(path + "_GeneratorTable", FileMode.Create));
                try
                {
                    byte[] comp = Compression.Create(Compression.Type.BytePair).Encode(ptnGen);
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
                br = new BinaryWriter(new FileStream(path + "_ColorTable", FileMode.Create));
                try
                {
                    byte[] comp = Compression.Create(Compression.Type.BytePair).Encode(ptnClr);
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
        internal void ExportMap(Type type, String path)
        {
            if (type == Type.CHeader || type == Type.CCompressed)
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
                    str = ArrayToCHeaderString(mapPattern, false);
                    sr.WriteLine(str);
                    sr.WriteLine("};");
                    str = "\t";
                    sr.WriteLine("// Map data");
                    if (type == Type.CHeader)
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
            else if (type == Type.ASMData || type == Type.ASMCompressed)
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
                    str = ArrayToASMString(mapPattern, false);
                    sr.WriteLine(str);
                    sr.WriteLine("; Map data");
                    str = "";
                    if (type == Type.ASMData)
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
            else if (type == Type.MSXBASIC)
            {
                throw new Exception("Map data cannot be BLOADed in BASIC");
            }
            else if (type == Type.Raw)
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
            else if (type == Type.RawCompressed)
            {
                BinaryWriter br = new BinaryWriter(new FileStream(path + "_PTN", FileMode.Create));
                try
                {
                    byte[] comp = Compression.Create(Compression.Type.BytePair).Encode(mapPattern);
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
                    br.Write(this.CompressMapData());
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
        internal void ExportSprites(Type type, String path)
        {
            // Start exporting
            if (type == Type.CHeader || type == Type.CCompressed)
            {
                StreamWriter sr = new StreamWriter(path, false);
                try
                {
                    sr.WriteLine("#ifndef __SPRITEDAT_H__");
                    sr.WriteLine("#define __SPRITEDAT_H__");
                    String str = "";
                    sr.WriteLine("// Sprite generator table");
                    if (type == Type.CHeader)
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
                        byte[] sprite_clr16x16 = this.SpriteColorsBy16x16();
                        sr.WriteLine("// Sprite color table");
                        if (type == Type.CHeader)
                        {
                            sr.WriteLine("const unsigned char sprclr[] = {");
                            str = ArrayToCHeaderString(sprite_clr16x16, false);
                        }
                        else
                        {
                            sr.WriteLine("const unsigned char sprclr_compressed[] = {");
                            str = ArrayToCHeaderString(sprite_clr16x16, true);
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
            else if (type == Type.ASMData || type == Type.ASMCompressed)
            {
                StreamWriter sr = new StreamWriter(path, false);
                try
                {
                    sr.WriteLine("; Sprite Data");
                    sr.WriteLine("; this export data is not tested");
                    String str = "";
                    sr.WriteLine("; Sprite generator table");
                    str = "";
                    if (type == Type.ASMData)
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
                        byte[] sprite_clr16x16 = this.SpriteColorsBy16x16();
                        sr.WriteLine("; Sprite color table");
                        str = "";
                        if (type == Type.ASMData)
                        {
                            sr.WriteLine("sprclr:");
                            str = ArrayToASMString(sprite_clr16x16, false);
                        }
                        else
                        {
                            sr.WriteLine("sprclr_compressed:");
                            str = ArrayToASMString(sprite_clr16x16, true);
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
            else if (type == Type.MSXBASIC)
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
            else if (type == Type.Raw)
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
            else if (type == Type.RawCompressed)
            {
                BinaryWriter br = new BinaryWriter(new FileStream(path, FileMode.Create));
                try
                {
                    byte[] comp = Compression.Create(Compression.Type.BytePair).Encode(spriteGen);
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
                    byte[] comp = Compression.Create(Compression.Type.BytePair).Encode(sprite_clr16x16);
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
        private byte[] SpriteColorsBy16x16()
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
            return sprite_clr16x16;
        }
        private String ArrayToCHeaderString(byte[] src, bool compress)
        {
            String ret = "\t";
            if (compress)
            {
                byte[] comp = Compression.Create(Compression.Type.BytePair).Encode(src);
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
                byte[] comp = Compression.Create(Compression.Type.BytePair).Encode(src);
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
                CompressionBase encoder = Compression.Create(Compression.Type.RunLength);
                byte[] comp = encoder.Encode(src_row.ToArray() as byte[]);
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
