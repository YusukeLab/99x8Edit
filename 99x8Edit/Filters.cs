using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace _99x8Edit
{
    public abstract class FilterBase
    {
        public abstract void Process(Bitmap src);
    }
    public class FilterCRT : FilterBase
    {
        public override void Process(Bitmap src)
        {
            // Lock the bitmap and see in the array
            BitmapData bd = src.LockBits(new Rectangle(0, 0, src.Width, src.Height), ImageLockMode.ReadWrite, src.PixelFormat);
            int stride = Math.Abs(bd.Stride);
            int ch_num = Bitmap.GetPixelFormatSize(src.PixelFormat) / 8;
            // Create byte arrays of source and destination
            Byte[] adst = new byte[stride * bd.Height];
            Byte[] asrc = new byte[stride * bd.Height];
            System.Runtime.InteropServices.Marshal.Copy(bd.Scan0, adst, 0, adst.Length);
            System.Runtime.InteropServices.Marshal.Copy(bd.Scan0, asrc, 0, asrc.Length);
            // Make new image with RGB shift
            for (int i = ch_num; i < adst.Length - ch_num; i += ch_num)
            {
                adst[i + 0] = asrc[i + 0 + ch_num]; // R
                adst[i + 1] = asrc[i + 1 - ch_num]; // G
                adst[i + 2] = asrc[i + 2];          // B
            }
            // Darken scanning lines
            for (int y = 1; y < bd.Height - 1; y += 2)
            {
                int t = y * stride;
                for (int x = 0; x < stride; x += ch_num)
                {
                    adst[t + x + 0] = (byte)(asrc[t + x + 0 - stride] >> 3 + asrc[t + x + 0 + stride] >> 3);
                    adst[t + x + 1] = (byte)(asrc[t + x + 1 - stride] >> 3 + asrc[t + x + 1 + stride] >> 3);
                    adst[t + x + 2] = (byte)(asrc[t + x + 2 - stride] >> 3 + asrc[t + x + 2 + stride] >> 3);
                }
            }
            // Write back into the bitmap
            System.Runtime.InteropServices.Marshal.Copy(adst, 0, bd.Scan0, adst.Length);
            src.UnlockBits(bd);
        }
    }
}
