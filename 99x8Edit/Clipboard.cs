﻿using System;
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
    [Serializable]
    internal abstract class ClipBase
    {
    }
    [Serializable]
    internal class ClipPCGLines : ClipBase
    {
        internal List<List<Machine.PCGLine>> lines = new List<List<Machine.PCGLine>>();
    }
    [Serializable]
    internal class ClipPCG : ClipBase
    {
        internal byte index;
        internal List<List<byte[]>> pcgGen = new List<List<byte[]>>();
        internal List<List<byte[]>> pcgClr = new List<List<byte[]>>();
    }
    [Serializable]
    internal class ClipNametable : ClipBase
    {
        internal List<List<int>> pcgID = new List<List<int>>();
    }
    [Serializable]
    internal class ClipMapPtn : ClipBase
    {
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
