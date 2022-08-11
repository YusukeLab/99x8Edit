using System;
using System.Collections.Generic;
using System.Text;

namespace _99x8Edit
{
    public class Clipboard
    {
        // Classes for internal copy and paste actions
        private static Clipboard _singleInstance = new Clipboard();
        private ClipBase copiedObj = null;
        public static Clipboard Instance
        {
            get
            {
                return _singleInstance;
            }
        }
        public ClipBase Clip
        {
            get
            {
                return copiedObj;
            }
            set
            {
                copiedObj = value;
            }
        }
    }
    public abstract class ClipBase
    {
    }
    public class ClipPCGLine : ClipBase
    {
        public byte genData;
        public byte clrData;
    }
    public class ClipOnePCG : ClipBase
    {
        public byte index;
        public byte[] genData = new byte[8];
        public byte[] clrData = new byte[8];
    }
    public class ClipNametable : ClipBase
    {
        public List<List<int>> pcgID = new List<List<int>>();
    }
    public class ClipOneMapPattern : ClipBase
    {
        public byte index;
        public byte[] pattern = new byte[4];
    }
    public class ClipMapCell : ClipBase
    {
        public List<List<int>> ptnID = new List<List<int>>();
    }
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
    public class ClipOneSpriteLine : ClipBase
    {
        public byte genData = 0;
        public byte clrData = 0;
        public bool overlayed = false;
        public byte genData2 = 0;
        public byte clrData2 = 0;
    }
    public class ClipOneChrInRom : ClipBase
    {
        public byte[] leftTop = new byte[8];
        public byte[] leftBottom = new byte[8];
        public byte[] rightTop = new byte[8];
        public byte[] rightBottom = new byte[8];
    }
}
