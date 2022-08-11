using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace _99x8Edit
{
    public class Utility
    {
        public static void DrawTransparent(Bitmap bmp)
        {
            Graphics g = Graphics.FromImage(bmp);
            for (int y = 0; y < bmp.Height / 16; ++y)
            {
                for (int x = 0; x < bmp.Width / 16; ++x)
                {
                    g.FillRectangle(new SolidBrush(Color.DarkGray), x * 16, y * 16, 8, 8);
                    g.FillRectangle(new SolidBrush(Color.Gray), x * 16 + 8, y * 16, 8, 8);
                    g.FillRectangle(new SolidBrush(Color.Gray), x * 16, y * 16 + 8, 8, 8);
                    g.FillRectangle(new SolidBrush(Color.DarkGray), x * 16 + 8, y * 16 + 8, 8, 8);
                }
            }
        }
    }
}
