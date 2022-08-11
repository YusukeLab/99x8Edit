using System;
using System.Collections.Generic;
using System.Text;

namespace _99x8Edit
{
    public class ClipboardWrapper
    {
        // For copy and paste actions
        public static void SetData(ClipBase clip)
        {
            System.Windows.Forms.Clipboard.SetData("99x8Edit", clip);
        }
        public static ClipBase GetData()
        {
            if (System.Windows.Forms.Clipboard.ContainsData("99x8Edit"))
            {
                return System.Windows.Forms.Clipboard.GetData("99x8Edit") as ClipBase;
            }
            return null;
        }
    }
    [Serializable]
    public abstract class ClipBase
    {
    }
    [Serializable]
    public class ClipPCGLine : ClipBase
    {
        public byte genData;
        public byte clrData;
    }
    [Serializable]
    public class ClipPCG : ClipBase
    {
        public byte index;
        public List<List<Byte[]>> pcgGen = new List<List<byte[]>>();
        public List<List<Byte[]>> pcgClr = new List<List<byte[]>>();
    }
    [Serializable]
    public class ClipNametable : ClipBase
    {
        public List<List<int>> pcgID = new List<List<int>>();
    }
    [Serializable]
    public class ClipOneMapPattern : ClipBase
    {
        public byte index;
        public byte[] pattern = new byte[4];
    }
    [Serializable]
    public class ClipMapCell : ClipBase
    {
        public List<List<int>> ptnID = new List<List<int>>();
    }
    [Serializable]
    public class Clip16x16Sprite : ClipBase
    {
        public byte[] genData = new byte[32];   // 16x16 sprite
        public byte[] clr2Data = new byte[32];
        public byte clr = 0;
        public byte overlayed = 0;
        public byte[] genData_ov = new byte[32];   // overlayed 16x16 sprite
        public byte[] clr2Data_ov = new byte[32];
        public byte clr_ov = 0;
    }
    [Serializable]
    public class ClipOneSpriteLine : ClipBase
    {
        public byte genData = 0;
        public byte clrData = 0;
        public bool overlayed = false;
        public byte genData2 = 0;
        public byte clrData2 = 0;
    }
    [Serializable]
    public class ClipOneChrInRom : ClipBase
    {
        public byte[] leftTop = new byte[8];
        public byte[] leftBottom = new byte[8];
        public byte[] rightTop = new byte[8];
        public byte[] rightBottom = new byte[8];
    }
}
