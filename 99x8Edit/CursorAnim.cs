using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace _99x8Edit
{
    internal partial class CursorAnim : Form
    {
        private readonly int loopNum = 8;
        private Rectangle start;
        private Rectangle end;
        private Bitmap bmp;
        internal CursorAnim(Rectangle startFrom, Rectangle endTo)
        {
            start = startFrom;
            end = endTo;
            int w = Math.Max(startFrom.Width, end.Width);
            int h = Math.Max(startFrom.Height, end.Height);
            InitializeComponent();
            pict.Image = bmp = new Bitmap(w, h);
        }
        internal void StartMoving()
        {
            for (int i = 0; i < loopNum; ++i)
            {
                Rectangle r = new Rectangle();
                r.X = start.X + (end.X - start.X) * i / loopNum;
                r.Y = start.Y + (end.Y - start.Y) * i / loopNum;
                r.Width = start.Width + (end.Width - start.Width) * i / loopNum;
                r.Height = start.Height + (end.Height - start.Height) * i / loopNum;
                this.Location = new Point(r.X, r.Y);
                this.Size = pict.Size = new Size(r.Width, r.Height);

                Graphics g = Graphics.FromImage(bmp);
                g.Clear(Color.Transparent);
                g.DrawRectangle(Pens.White, 0, 0, r.Width, r.Height);
                g.DrawRectangle(Pens.Green, 1, 1, r.Width - 2, r.Height - 2);

                this.Refresh();
                System.Threading.Thread.Sleep(1);
            }
            this.Dispose();
        }
    }
}
