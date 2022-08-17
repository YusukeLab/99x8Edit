using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace _99x8Edit
{
    public partial class EditorControl : MatrixControl
    {
        // Control for the editor matrix view
        protected Brush[,] _brush;  // Brushes to be drawn to each cell
        private Selection _line;    // Selection of one line
        private int _dot;           // Dot editing in one line, 0-7
        //--------------------------------------------------------------------
        // Initialize
        public EditorControl()
        {
            InitializeComponent();
        }
        //--------------------------------------------------------------------
        // Properties
        [Browsable(true)]
        [Description("Width of one cell")]
        public override int CellWidth
        {
            get => base.CellWidth;
            set
            {
                base.CellWidth = value;
                _line = new Selection(_cellWidth * 8, _cellHeight);
            }
        }
        [Browsable(true)]
        [Description("Height of one cell")]
        public override int CellHeight
        {
            get => base.CellHeight;
            set
            {
                base.CellHeight = value;
                _line = new Selection(_cellWidth * 8, _cellHeight);
            }
        }
        public enum Type
        {
            PCG,
            Sprite,
            Other
        }
        [Browsable(true)]
        [Description("Target data type")]
        public Type DataType
        {
            get;
            set;
        }
        //--------------------------------------------------------------------
        // Event handlers
        [Browsable(true)]
        [Description("Called when a pixel has been edited")]
        public event EventHandler<EventArgs> Edited;
        //--------------------------------------------------------------------
        // Methods and properties for hosts
        public Selection LineSelector
        {
            get => _line;
        }
        public void SetBrush(Brush b, int col, int row)
        {
            _brush ??= new Brush[ColumnNum, RowNum];
            _brush[col, row] = b;
            _updated = true;
        }
        public int LineX
        {
            get => _line.X;
        }
        public int LineY
        {
            get => _line.Y;
        }
        public Rectangle SelectedLine
        {
            get => _line.Selected;
        }
        [Browsable(false)]
        public (int x, int y) PosInTile()
        {
            return (_dot, _line.Y % 8);
        }
        public int TileColNum
        {
            get => _columnNum / 8;
        }
        public int TileRowNum
        {
            get => _rowNum / 8;
        }
        public void ForEachLines(int col, int row, int w, int h,
                                 Action<int, int> callback)
        {
            for (int y = row; (y < row + h) && (y < RowNum); ++y)
            {
                for (int x = col; (x < col + w) && (x < ColumnNum / 8); ++x)
                {
                    callback?.Invoke(x, y);
                }
            }
        }
        public void ForEachLines(int col, int row, int w, int h,
                                 Action<int, int, int, int> callback)
        {
            int y_cnt = 0;
            for (int y = row; (y < row + h) && (y < RowNum); ++y)
            {
                int x_cnt = 0;
                for (int x = col; (x < col + w) && (x < ColumnNum / 8); ++x)
                {
                    callback?.Invoke(x, y, x_cnt, y_cnt);
                    x_cnt++;
                }
                y_cnt++;
            }
        }
        //--------------------------------------------------------------------
        // Overrides
        protected override void OnPaint(PaintEventArgs e)
        {
            // Draw to cells to buffer
            if (_brush == null)
            {
                return;
            }
            _bmp ??= new Bitmap(ColumnNum * CellWidth, RowNum * CellHeight);
            if (_updated)
            {
                // If something has been changed, redraw the buffer
                Utility.DrawTransparent(_bmp);
                Graphics g = Graphics.FromImage(_bmp);

                for (int y = 0; y < RowNum; ++y)
                {
                    for (int x = 0; x < ColumnNum; ++x)
                    {
                        if (_brush[x, y] != null)
                        {
                            // Draw outline
                            g.FillRectangle(Brushes.Gray, x * CellWidth, y * CellHeight, CellWidth, CellHeight);
                            // Draw one dot
                            g.FillRectangle(_brush[x, y], x * CellWidth, y * CellHeight, CellWidth - 1, CellHeight - 1);
                        }
                    }
                }
                // Draw selection
                Utility.DrawSelection(g, _line, this.Focused);
                if (this.Focused)
                {
                    // One dot can be selected when focused
                    Utility.DrawSubSelection(g, _line.Display.X + _dot * CellWidth,
                                             _line.Display.Y, CellWidth - 2, CellHeight - 2);
                }
                _updated = false;
            }
            // Copy from buffer
            e.Graphics.DrawImage(_bmp, 0, 0);
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            this.Focus();
            if (e.Button == MouseButtons.Left)
            {
                int clicked_line_x = e.X / (CellWidth * 8);
                int clicked_line_y = e.Y / CellHeight;
                int clicked_dot = Math.Clamp((e.X / CellWidth) % 8, 0, 7);
                if ((_line.X != clicked_line_x) || (_line.Y != clicked_line_y))
                {
                    // Selected line has changed
                    if (Control.ModifierKeys == Keys.Shift)
                    {
                        // Multiple selection
                        _line.ToX = clicked_line_x;
                        _line.ToY = clicked_line_y;
                    }
                    else
                    {
                        // New selection
                        _line.X = clicked_line_x;
                        _line.Y = clicked_line_y;
                    }
                    // Move the dot selection
                    _dot = clicked_dot;
                    _updated = true;
                    this.Refresh();
                    // Drag for multiple selection
                    this.DoDragDrop(new DragSelection(this), DragDropEffects.Copy);
                }
                else
                {
                    // Move the dot selection
                    _dot = clicked_dot;
                    _updated = true;
                    this.Refresh();
                    // Cell to be dragged
                    Edited?.Invoke(this, new EventArgs());
                }
            }
        }
        protected override void OnDragOver(DragEventArgs drgevent)
        {
            if (drgevent.Data.GetDataPresent(typeof(DragSelection)))
            {
                // Accept only the drag objects created by itself
                dynamic obj = drgevent.Data.GetData(typeof(DragSelection));
                if (obj.Sender == this)
                {
                    // Multiple selection
                    Point p = this.PointToClient(Cursor.Position);
                    int hoverd_x = Math.Min(p.X / (CellWidth * 8), (ColumnNum / 8) - 1);
                    int hoverd_y = Math.Min(p.Y / CellHeight, RowNum - 1);
                    if ((hoverd_x != _line.ToX) || (hoverd_y != _line.ToY))
                    {
                        _line.ToX = hoverd_x;
                        _line.ToY = hoverd_y;
                        _updated = true;
                        this.Refresh();
                    }
                }
            }
            base.OnDragOver(drgevent);
        }
        protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
        {
            // Key events in sandbox
            Action handled = () =>
            {
                _updated = true;
            };
            switch (e.KeyData)
            {
                case Keys.Up | Keys.Shift:
                    if (_line.ToY > 0)
                    {
                        _line.ToY--;
                        handled();
                        this.Refresh();
                    }
                    break;
                case Keys.Down | Keys.Shift:
                    if (_line.ToY < RowNum - 1)
                    {
                        _line.ToY++;
                        handled();
                        this.Refresh();
                    }
                    break;
                case Keys.Left | Keys.Shift:
                    if (_line.ToX > 0)
                    {
                        _line.ToX--;
                        handled();
                        this.Refresh();
                    }
                    break;
                case Keys.Right | Keys.Shift:
                    if (_line.ToX < ColumnNum / 8 - 1)
                    {
                        _line.ToX++;
                        handled();
                        this.Refresh();
                    }
                    break;
                case Keys.Up:
                    if (_line.Y > 0)
                    {
                        _line.Y--;
                        handled();
                        this.Refresh();
                    }
                    break;
                case Keys.Down:
                    if (_line.Y < RowNum - 1)
                    {
                        _line.Y++;
                        handled();
                        this.Refresh();
                    }
                    break;
                case Keys.Left:
                    if ((_dot == 0) && (_line.X > 0))
                    {
                        _line.X--;
                        _dot = 7;
                        handled();
                        this.Refresh();
                    }
                    else if (_dot > 0)
                    {
                        _dot--;
                        handled();
                        this.Refresh();
                    }
                    break;
                case Keys.Right:
                    if ((_dot == 7) && (_line.X < 1))
                    {
                        _line.X++;
                        _dot = 0;
                        handled();
                        this.Refresh();
                    }
                    else if (_dot < 7)
                    {
                        _dot++;
                        handled();
                        this.Refresh();
                    }
                    break;
                case Keys.Space:
                    // toggle the color of selected pixel
                    Edited?.Invoke(this, new EventArgs());
                    handled();
                    break;
                case Keys.D1:
                case Keys.NumPad1:
                    _dot = 0;
                    handled();
                    Edited?.Invoke(this, new EventArgs());
                    break;
                case Keys.D2:
                case Keys.NumPad2:
                    _dot = 1;
                    handled();
                    Edited?.Invoke(this, new EventArgs());
                    break;
                case Keys.D3:
                case Keys.NumPad3:
                    _dot = 2;
                    handled();
                    Edited?.Invoke(this, new EventArgs());
                    break;
                case Keys.D4:
                case Keys.NumPad4:
                    _dot = 3;
                    handled();
                    Edited?.Invoke(this, new EventArgs());
                    break;
                case Keys.D5:
                case Keys.NumPad5:
                    _dot = 4;
                    handled();
                    Edited?.Invoke(this, new EventArgs());
                    break;
                case Keys.D6:
                case Keys.NumPad6:
                    _dot = 5;
                    handled();
                    Edited?.Invoke(this, new EventArgs());
                    break;
                case Keys.D7:
                case Keys.NumPad7:
                    _dot = 6;
                    handled();
                    Edited?.Invoke(this, new EventArgs());
                    break;
                case Keys.D8:
                case Keys.NumPad8:
                    _dot = 7;
                    handled();
                    Edited?.Invoke(this, new EventArgs());
                    break;
            }
            base.OnPreviewKeyDown(e);
        }
    }
}
