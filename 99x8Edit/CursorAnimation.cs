using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace _99x8Edit
{
    internal partial class CursorAnimation : Form
    {
        // Transparent window for the cursor effect, flying from control to control
        private static Bitmap _bmp;
        private readonly int _loop = 12;
        private Rectangle _start;
        private Rectangle _end;
        private Rectangle _current;
        internal CursorAnimation(Rectangle startFrom, Rectangle endTo)
        {
            _start = startFrom;
            _end = endTo;
            int w = Math.Max(startFrom.Width, _end.Width);
            int h = Math.Max(startFrom.Height, _end.Height);
            if((_bmp == null) || (_bmp?.Width < w) || (_bmp?.Height < h))
            {
                _bmp = new Bitmap(w, h);
            }
            InitializeComponent();
            _pict.Image = _bmp;
            this.Draw();
        }

        // Ignore controls
        protected override bool ShowWithoutActivation => true;
        internal async void StartMoving()
        {
            await Task.Run(() => {
                for (int i = 0; i < _loop; ++i)
                {
                    // Use log to avoid constant speed
                    int distance = (int)(Math.Log10(i + 1.0) * 1000.0);
                    int max = (int)(Math.Log10(_loop) * 1000.0);
                    _current.X = _start.X + (_end.X - _start.X) * distance / max;
                    _current.Y = _start.Y + (_end.Y - _start.Y) * distance / max;
                    _current.Width = _start.Width + (_end.Width - _start.Width) * distance / max;
                    _current.Height = _start.Height + (_end.Height - _start.Height) * distance / max;
                    this.Invoke(new DelegateDrawCursor(this.Draw));
                    System.Threading.Thread.Sleep(1);
                }
            });
            this.Dispose();
        }
        private delegate void DelegateDrawCursor();
        private void Draw()
        {
            this.Location = new Point(_current.X, _current.Y);
            this.Size = _pict.Size = new Size(_current.Width, _current.Height);
            Graphics g = Graphics.FromImage(_bmp);
            g.Clear(Color.Transparent);
            g.DrawRectangle(Pens.LightGreen, 0, 0, _current.Width, _current.Height);
            g.DrawRectangle(Pens.Green, 1, 1, _current.Width - 2, _current.Height - 2);
            this.Refresh();
        }
    }
}
