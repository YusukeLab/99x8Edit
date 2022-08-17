using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace _99x8Edit
{
    public partial class MatrixControl : UserControl
    {
        // Control for the ordinal matrix view
        protected int _cellWidth;       // Width of one cell
        protected int _cellHeight;      // Height of one cell
        protected int _columnNum;       // Number of columns
        protected int _rowNum;          // Number of rows
        protected Bitmap _bmp;          // Buffer to draw images
        protected bool _updated;        // Should be drawn to buffer
        protected Selection _selection; // Current selection
        protected FilterBase _filter;   // Filter to be applyed
        protected Bitmap[,] _cellImg;   // Images for each cells
        // For multiple selections
        internal class DragSelection {
            internal UserControl _sender;
            internal DragSelection(UserControl sender)
            {
                _sender = sender;
            }
            internal UserControl Sender
            {
                get => _sender;
            }
        }
        //--------------------------------------------------------------------
        // Initialize
        public MatrixControl()
        {
            // Prevent flickering
            this.SetStyle(ControlStyles.DoubleBuffer |
                          ControlStyles.UserPaint |
                          ControlStyles.AllPaintingInWmPaint,
                          true);
            InitializeComponent();
        }
        //--------------------------------------------------------------------
        // Properties
        [Browsable(true)]
        [Description("Width of one cell")]
        public virtual int CellWidth
        {
            get => _cellWidth;
            set
            {
                _cellWidth = value;
                _selection = new Selection(_cellWidth, _cellHeight);
                _cellImg = null;
                _bmp = null;
            }
        }
        [Browsable(true)]
        [Description("Height of one cell")]
        public virtual int CellHeight
        {
            get => _cellHeight;
            set
            {
                _cellHeight = value;
                _selection = new Selection(_cellWidth, _cellHeight);
                _cellImg = null;
                _bmp = null;
            }
        }
        [Browsable(true)]
        [Description("Number of columns to be controlled")]
        public int ColumnNum
        {
            get => _columnNum;
            set
            {
                _columnNum = value;
                _cellImg = null;
                _bmp = null;
            }
        }
        [Browsable(true)]
        [Description("Number of rows to be controlled")]
        public int RowNum
        {
            get => _rowNum;
            set
            {
                _rowNum = value;
                _cellImg = null;
                _bmp = null;
            }
        }
        //--------------------------------------------------------------------
        // Event handlers
        [Browsable(true)]
        [Description("Called when selection has been changed")]
        public event EventHandler<EventArgs> SelectionChanged;
        [Browsable(true)]
        [Description("Called when cell was dragged")]
        public event EventHandler<EventArgs> CellDragStart;
        //--------------------------------------------------------------------
        // Properties for hosts
        [Browsable(false)]
        public Selection Selector
        {
            get => _selection;
        }
        [Browsable(false)]
        public int X
        {
            get => _selection.X;
        }
        [Browsable(false)]
        public int Y
        {
            get => _selection.Y;
        }
        [Browsable(false)]
        public Rectangle SelectedRect
        {
            get => _selection.Selected;
        }
        [Browsable(false)]
        public FilterBase Filter
        {
            set
            {
                _filter = value;
                _updated = true;
            }
        }
        [Browsable(false)]
        public int Index
        {
            get => _selection.Y * ColumnNum + _selection.X;
            set
            {
                if ((ColumnNum != 0) && (RowNum != 0))
                {
                    int id = Math.Clamp(value, 0, ColumnNum * RowNum - 1);
                    _selection.X = id % ColumnNum;
                    _selection.Y = id / ColumnNum;
                    _updated = true;
                }
            }
        }
        //--------------------------------------------------------------------
        // Methods for hosts
        public void SetImage(Bitmap img, int col, int row)
        {
            _cellImg ??= new Bitmap[ColumnNum, RowNum];
            _cellImg[col, row] = img;
            _updated = true;
        }
        public (int col, int row) ScreenCoodinateToColRow(Point screen_p)
        {
            Point p = this.PointToClient(screen_p);
            int col = Math.Clamp(p.X / CellWidth, 0, ColumnNum);
            int row = Math.Clamp(p.Y / CellHeight, 0, RowNum);
            return (col, row);
        }
        public int ScreenCoodinateToIndex(Point screen_p)
        {
            (int col, int row) = ScreenCoodinateToColRow(screen_p);
            return IndexOf(col, row);
        }
        public int IndexOf(int col, int row)
        {
            return row * ColumnNum + col;
        }
        public void ForEachCells(Rectangle selection, Action<int, int> callback)
        {
            this.ForEachCells(selection.X, selection.Y,
                              selection.Width, selection.Height, callback);
        }
        public void ForEachCells(int col, int row, int w, int h,
                                 Action<int, int> callback)
        {
            for (int y = row; (y < row + h) && (y < RowNum); ++y)
            {
                for (int x = col; (x < col + w) && (x < ColumnNum); ++x)
                {
                    callback?.Invoke(x, y);
                }
            }
        }
        public void ForEachCells(int col, int row, int w, int h,
                                 Action<int, int, int, int> callback)
        {
            int y_cnt = 0;
            for (int y = row; (y < row + h) && (y < RowNum); ++y)
            {
                int x_cnt = 0;
                for (int x = col; (x < col + w) && (x < ColumnNum); ++x)
                {
                    callback?.Invoke(x, y, x_cnt, y_cnt);
                    x_cnt++;
                }
                y_cnt++;
            }
        }
        public void ResetMultipleSelection()
        {
            _selection.ResetSelectionAndUpdate();
            _updated = true;
        }
        //--------------------------------------------------------------------
        // Overrides
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            // Draw to cells to buffer
            if (_cellImg == null)
            {
                return;
            }
            _bmp ??= new Bitmap(ColumnNum * CellWidth, RowNum * CellHeight);
            if (_updated)
            {
                // If something has been changed, redraw the buffer
                Graphics g = Graphics.FromImage(_bmp);
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                for (int y = 0; y < RowNum; ++y)
                {
                    for (int x = 0; x < ColumnNum; ++x)
                    {
                        g.FillRectangle(Brushes.Black, x * CellWidth, y * CellHeight, CellWidth, CellHeight);
                        g.DrawImage(_cellImg[x, y], x * CellWidth, y * CellHeight, CellWidth + 1, CellHeight + 1);
                    }
                }
                // Apply filter
                _filter?.Process(_bmp);
                // Draw Selection
                Utility.DrawSelection(g, _selection, this.Focused);
                _updated = false;
            }
            // Copy from buffer
            e.Graphics.DrawImage(_bmp, 0, 0);
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int clicked_cell_x = Math.Min(e.X / CellWidth, ColumnNum - 1);
                int clicked_cell_y = Math.Min(e.Y / CellHeight, RowNum - 1);
                if ((clicked_cell_x != _selection.X) || (clicked_cell_y != _selection.Y))
                {
                    // Selection changed
                    if (Control.ModifierKeys == Keys.Shift)
                    {
                        // Multiple selection
                        _selection.ToX = clicked_cell_x;
                        _selection.ToY = clicked_cell_y;
                    }
                    else
                    {
                        // New selection
                        _selection.X = clicked_cell_x;
                        _selection.Y = clicked_cell_y;
                        // For host
                        SelectionChanged?.Invoke(this, new EventArgs());
                    }
                    _updated = true;    // Redraw buffer at the timing of OnPaint
                    this.Refresh();
                    // Drag for multiple selection
                    this.DoDragDrop(new DragSelection(this), DragDropEffects.Copy);
                }
                else
                {
                    // Cell to be dragged
                    CellDragStart?.Invoke(this, new EventArgs());
                }
                base.OnMouseDown(e);
            }
        }
        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            drgevent.Effect = DragDropEffects.None;
            if (drgevent.Data.GetDataPresent(typeof(DragSelection)))
            {
                // Accept only the drag objects created by itself
                dynamic obj = drgevent.Data.GetData(typeof(DragSelection));
                if (obj.Sender == this)
                {
                    // Multiple selection
                    drgevent.Effect = DragDropEffects.Copy;
                }
            }
            base.OnDragEnter(drgevent);
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
                    int hoverd_x = Math.Min(p.X / CellWidth, ColumnNum - 1);
                    int hoverd_y = Math.Min(p.Y / CellHeight, RowNum - 1);
                    if ((hoverd_x != _selection.ToX) || (hoverd_y != _selection.ToY))
                    {
                        _selection.ToX = hoverd_x;
                        _selection.ToY = hoverd_y;
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
                    if (_selection.ToY > 0)
                    {
                        _selection.ToY--;
                        handled();
                        this.Refresh();
                    }
                    break;
                case Keys.Down | Keys.Shift:
                    if (_selection.ToY < RowNum - 1)
                    {
                        _selection.ToY++;
                        handled();
                        this.Refresh();
                    }
                    break;
                case Keys.Left | Keys.Shift:
                    if (_selection.ToX > 0)
                    {
                        _selection.ToX--;
                        handled();
                        this.Refresh();
                    }
                    break;
                case Keys.Right | Keys.Shift:
                    if (_selection.ToX < ColumnNum - 1)
                    {
                        _selection.ToX++;
                        handled();
                        this.Refresh();
                    }
                    break;
                case Keys.Up:
                    if (_selection.Y > 0)
                    {
                        _selection.Y--;
                        handled();
                        SelectionChanged?.Invoke(this, new EventArgs());
                        this.Refresh();
                    }
                    break;
                case Keys.Down:
                    if (_selection.Y < RowNum - 1)
                    {
                        _selection.Y++;
                        handled();
                        SelectionChanged?.Invoke(this, new EventArgs());
                        this.Refresh();
                    }
                    break;
                case Keys.Left:
                    if (_selection.X > 0)
                    {
                        _selection.X--;
                        handled();
                        SelectionChanged?.Invoke(this, new EventArgs());
                        this.Refresh();
                    }
                    break;
                case Keys.Right:
                    if (_selection.X < ColumnNum - 1)
                    {
                        _selection.X++;
                        handled();
                        SelectionChanged?.Invoke(this, new EventArgs());
                        this.Refresh();
                    }
                    break;
            }
            base.OnPreviewKeyDown(e);
        }
    }
}
