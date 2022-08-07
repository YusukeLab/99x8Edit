using System;
using System.Collections.Generic;
using System.Text;

namespace _99x8Edit
{
    class Clipboard
    {
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
    abstract class ClipBase
    {
    }
    class ClipPCGLine : ClipBase
    {
        public byte genData;
        public byte clrData;
    }
    class ClipOnePCG : ClipBase
    {
        public byte[] genData = new byte[8];
        public byte[] clrData = new byte[8];
    }
    class ClipOneChrOfNametable : ClipBase
    {
        public int pcgIndex;
    }
    class ClipOneMapPattern : ClipBase
    {
        public byte[] pattern = new byte[4];
    }
    class ClipMapCell : ClipBase
    {
        public int dat;
    }
    class Clip16x16Sprite : ClipBase
    {
        public byte[] genData = new byte[32];   // 16x16 sprite
        public byte[] clr2Data = new byte[32];
        public byte clr = 0;
        public byte overlayed = 0;
        public byte[] genData_ov = new byte[32];   // overlayed 16x16 sprite
        public byte[] clr2Data_ov = new byte[32];
        public byte clr_ov = 0;
    }
    class ClipOneSpriteLine : ClipBase
    {
        public byte genData = 0;
        public byte clrData = 0;
        public bool overlayed = false;
        public byte genData2 = 0;
        public byte clrData2 = 0;
    }




}
