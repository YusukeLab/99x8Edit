using System;
using System.Collections.Generic;
using System.Text;

namespace _99x8Edit
{
    internal class ClipboardWrapper
    {
        // For copy and paste actions
        internal static void SetData(ClipBase clip)
        {
            System.Windows.Forms.Clipboard.SetData("99x8Edit", clip);
        }
        internal static ClipBase GetData()
        {
            if (System.Windows.Forms.Clipboard.ContainsData("99x8Edit"))
            {
                return System.Windows.Forms.Clipboard.GetData("99x8Edit") as ClipBase;
            }
            return null;
        }
    }
    // Classes for each clipping types
    [Serializable]
    internal abstract class ClipBase
    {
    }
    [Serializable]
    internal class ClipPCGLines : ClipBase
    {
        // PCG line(gen, color) * col width * row height
        internal List<List<(byte, byte)>> lines = new List<List<(byte, byte)>>();
    }
    [Serializable]
    internal class ClipPCG : ClipBase
    {
        internal List<List<byte[]>> pcgGen = new List<List<byte[]>>();
        internal List<List<byte[]>> pcgClr = new List<List<byte[]>>();
        internal List<List<int>> pcgIndex = new List<List<int>>();
    }
    [Serializable]
    internal class ClipPCGIndex : ClipBase
    {
        internal int pcgIndex;
    }
    [Serializable]
    internal class ClipNametable : ClipBase
    {
        internal List<List<int>> pcgID = new List<List<int>>();
    }
    [Serializable]
    internal class ClipMapPtn : ClipBase
    {
        internal int index;
        internal List<List<byte[]>> ptns = new List<List<byte[]>>();
    }
    [Serializable]
    internal class ClipMapCell : ClipBase
    {
        internal List<List<int>> ptnID = new List<List<int>>();
    }
    [Serializable]
    internal class ClipSprite : ClipBase
    {
        internal List<List<Machine.One16x16Sprite>> sprites = new List<List<Machine.One16x16Sprite>>();
    }
    [Serializable]
    internal class ClipOneSpriteLine : ClipBase
    {
        internal List<List<Machine.SpriteLine>> lines = new List<List<Machine.SpriteLine>>();
    }
    [Serializable]
    internal class ClipPeekedData : ClipBase
    {
        internal List<List<byte[]>> peeked = new List<List<byte[]>>();
    }
}
