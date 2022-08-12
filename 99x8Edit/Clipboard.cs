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
    public class ClipPCGLines : ClipBase
    {
        public List<List<Machine.PCGLine>> lines = new List<List<Machine.PCGLine>>();
    }
    [Serializable]
    public class ClipPCG : ClipBase
    {
        public byte index;
        public List<List<byte[]>> pcgGen = new List<List<byte[]>>();
        public List<List<byte[]>> pcgClr = new List<List<byte[]>>();
    }
    [Serializable]
    public class ClipNametable : ClipBase
    {
        public List<List<int>> pcgID = new List<List<int>>();
    }
    [Serializable]
    public class ClipMapPtn : ClipBase
    {
        public List<List<byte[]>> ptns = new List<List<byte[]>>();
    }
    [Serializable]
    public class ClipMapCell : ClipBase
    {
        public List<List<int>> ptnID = new List<List<int>>();
    }
    [Serializable]
    public class ClipSprite : ClipBase
    {
        public List<List<Machine.One16x16Sprite>> sprites = new List<List<Machine.One16x16Sprite>>();
    }
    [Serializable]
    public class ClipOneSpriteLine : ClipBase
    {
        public List<List<Machine.SpriteLine>> lines = new List<List<Machine.SpriteLine>>();
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
