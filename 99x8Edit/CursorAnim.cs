using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace _99x8Edit
{
    internal partial class CursorAnim : Form
    {
        private readonly int loopNum = 12;
        private Rectangle start;
        private Rectangle end;
        private Rectangle current;
        private Bitmap bmp;
        internal CursorAnim(Rectangle startFrom, Rectangle endTo)
        {
            start = startFrom;
            end = endTo;
            int w = Math.Max(startFrom.Width, end.Width);
            int h = Math.Max(startFrom.Height, end.Height);
            InitializeComponent();
            pict.Image = bmp = new Bitmap(w, h);
            this.Draw();
        }
        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }
        internal async void StartMoving()
        {
            await Task.Run(() => {
                for (int i = 0; i < loopNum; ++i)
                {
                    // log10(i + 1) of 12 goes nearby to 1.08
                    int distance = (int)(Math.Log10((double)i + 1.0) * 1000.0);
                    current.X = start.X + (end.X - start.X) * distance / 1080;
                    current.Y = start.Y + (end.Y - start.Y) * distance / 1080;
                    current.Width = start.Width + (end.Width - start.Width) * distance / 1080;
                    current.Height = start.Height + (end.Height - start.Height) * distance / 1080;
                    this.Invoke(new DelegateDrawCursor(this.Draw));
                    System.Threading.Thread.Sleep(1);
                }
            });
            this.Dispose();
        }
        private delegate void DelegateDrawCursor();
        private void Draw()
        {
            this.Location = new Point(current.X, current.Y);
            this.Size = pict.Size = new Size(current.Width, current.Height);
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.Transparent);
            g.DrawRectangle(Pens.White, 0, 0, current.Width, current.Height);
            g.DrawRectangle(Pens.Green, 1, 1, current.Width - 2, current.Height - 2);
            this.Refresh();
        }
    }
}
