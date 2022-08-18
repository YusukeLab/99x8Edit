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
        protected int _cellWidth = 16;      // Width of one cell
        protected int _cellHeight = 16;     // Height of one cell
        protected int _columnNum = 32;      // Number of columns
        protected int _rowNum = 32;         // Number of rows
        protected int _selectionWidth = 1;  // Columns of one selection
        protected int _selectionHeight = 1; // Rows of one selection
        protected Bitmap _bmp;              // Buffer to draw images
        protected bool _updated;            // Should be drawn to buffer
        protected Selection _selection;     // Current selection
        protected Selection _sub;           // Current sub selection
        protected FilterBase _filter;       // Filter to be applyed
        protected Bitmap[,] _cellImg;       // Images for each cells
        protected Color[,] _background;     // Background colors
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
                _selection = new Selection(_cellWidth * _selectionWidth,
                                           _cellHeight * _selectionHeight);
                _sub = new Selection(_cellWidth, _cellHeight);
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
                _selection = new Selection(_cellWidth * _selectionWidth,
                                           _cellHeight * _selectionHeight);
                _sub = new Selection(_cellWidth, _cellHeight);
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
                _background = null;
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
                _background = null;
            }
        }
        [Browsable(true)]
        [Description("Number of columns of one selection")]
        public int SelectionWidth
        {
            get => _selectionWidth;
            set
            {
                _selectionWidth = value;
                _selection = new Selection(_cellWidth * _selectionWidth,
                                           _cellHeight * _selectionHeight);
                _sub = new Selection(_cellWidth, _cellHeight);
            }
        } 
        [Browsable(true)]
        [Description("Number of rows of one selection")]
        public int SelectionHeight
        {
            get => _selectionHeight;
            set
            {
                _selectionHeight = value;
                _selection = new Selection(_cellWidth * _selectionWidth,
                                           _cellHeight * _selectionHeight);
                _sub = new Selection(_cellWidth, _cellHeight);
            }
        }
        [Browsable(true)]
        [Description("Has sub selection inside one selection")]
        public bool AllowSubSelection
        {
            get;
            set;
        } = false;
        [Browsable(true)]
        [Description("Allow multiple selection")]
        public bool AllowMultipleSelection
        {
            get;
            set;
        } = true;
        //--------------------------------------------------------------------
        // Event handlers
        [Browsable(true)]
        [Description("Called when selection has been changed")]
        public event EventHandler<EventArgs> SelectionChanged;
        [Browsable(true)]
        [Description("Called when cell was dragged")]
        public event EventHandler<EventArgs> CellDragStart;
        [Browsable(true)]
        [Description("Called when a cel has been clicked")]
        public event EventHandler<EventArgs> CellOnEdit;
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
            set
            {
                _selection.X = value;
                _updated = true;
            }
        }
        [Browsable(false)]
        public int Y
        {
            get => _selection.Y;
            set
            {
                _selection.Y = value;
                _updated = true;
            }
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
            // Get linear index of selecion
            get => _selection.Y * (_columnNum / _selectionWidth) + _selection.X;
            set
            {
                if ((ColumnNum != 0) && (RowNum != 0))
                {
                    int id = Math.Clamp(value, 0, _columnNum * _rowNum - 1);
                    _selection.X = id % _columnNum;
                    _selection.Y = id / _columnNum;
                    _updated = true;
                }
            }
        }
        //--------------------------------------------------------------------
        // Methods for hosts
        public void SetImage(Bitmap img, int col, int row)
        {
            _cellImg ??= new Bitmap[_columnNum, _rowNum];
            _cellImg[col, row] = img;
            _updated = true;
        }
        public void SetBackground(Color c, int col, int row)
        {
            _background ??= new Color[_columnNum, _rowNum];
            _background[col, row] = c;
            _updated = true;
        }
        public (int col, int row) ScreenCoodinateToColRow(Point screen_p)
        {
            Point p = this.PointToClient(screen_p);
            int col = Math.Clamp(p.X / _cellWidth, 0, _columnNum);
            int row = Math.Clamp(p.Y / _cellHeight, 0, _rowNum);
            return (col, row);
        }
        public int ScreenCoodinateToIndex(Point screen_p)
        {
            // Screen coorinate to linear index
            (int col, int row) = ScreenCoodinateToColRow(screen_p);
            return IndexOf(col, row);
        }
        public int IndexOf(int col, int row)
        {
            // Columns and rows to linear index
            return row * (_columnNum / _selectionWidth) + col;
        }
        public void ForEachSelection(Rectangle selection, Action<int, int> callback)
        {
            this.ForEachSelection(selection.X, selection.Y,
                                  selection.Width, selection.Height, callback);
        }
        public void ForEachSelection(int col, int row, int w, int h,
                                 Action<int, int> callback)
        {
            for (int y = row; (y < row + h) && (y < _rowNum); ++y)
            {
                for (int x = col; (x < col + w) && (x < _columnNum); ++x)
                {
                    callback?.Invoke(x, y);
                }
            }
        }
        public void ForEachSelection(int col, int row, int w, int h,
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
            _bmp ??= new Bitmap(ColumnNum * CellWidth, RowNum * CellHeight);
            if (_updated)
            {
                // If something has been changed, redraw the buffer
                Graphics g = Graphics.FromImage(_bmp);
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                if (_background != null)
                {
                    for (int y = 0; y < RowNum; ++y)
                    {
                        for (int x = 0; x < ColumnNum; ++x)
                        {
                            if (_background[x, y] != null)
                            {
                                SolidBrush b = new SolidBrush(_background[x, y]);
                                g.FillRectangle(b, x * _cellWidth, y * _cellHeight, _cellWidth, _cellHeight);
                            }
                        }
                    }
                }
                if (_cellImg != null)
                {
                    for (int y = 0; y < RowNum; ++y)
                    {
                        for (int x = 0; x < ColumnNum; ++x)
                        {
                            if(_cellImg[x, y] != null)
                            {
                                g.FillRectangle(Brushes.Black, x * _cellWidth, y * _cellHeight,
                                                _cellWidth, _cellHeight);
                                g.DrawImage(_cellImg[x, y], x * _cellWidth, y * _cellHeight,
                                            _cellWidth + 1, _cellHeight + 1);
                            }
                        }
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
                int clicked_selection_x = Math.Min(e.X / (_cellWidth * _selectionWidth),
                                                   _columnNum / _selectionWidth - 1);
                int clicked_selection_y = Math.Min(e.Y / (_cellHeight * _selectionHeight),
                                                   _rowNum / _selectionHeight - 1);
                int clicked__sub_x = Math.Min((e.X / _cellWidth) % _selectionWidth,
                                              _selectionWidth - 1);
                int clicked__sub_y = Math.Min((e.Y / _cellHeight) % _selectionHeight,
                                              _selectionHeight - 1);
                if ((clicked_selection_x != _selection.X) || (clicked_selection_y != _selection.Y))
                {
                    // Selection changed
                    if ((Control.ModifierKeys == Keys.Shift) && AllowMultipleSelection)
                    {
                        // Multiple selection
                        _selection.ToX = clicked_selection_x;
                        _selection.ToY = clicked_selection_y;
                    }
                    else
                    {
                        // New selection
                        _selection.X = clicked_selection_x;
                        _selection.Y = clicked_selection_y;
                        // For host
                        SelectionChanged?.Invoke(this, new EventArgs());
                    }
                    // Move sub selection
                    _sub.X = clicked__sub_x;
                    _sub.Y = clicked__sub_y;
                    // Update
                    _updated = true;    // Redraw buffer at the timing of OnPaint
                    this.Refresh();
                    // Drag for multiple selection
                    if (AllowMultipleSelection)
                    {
                        this.DoDragDrop(new DragSelection(this), DragDropEffects.Copy);
                    }
                }
                else
                {
                    // Move sub selection
                    _sub.X = clicked__sub_x;
                    _sub.Y = clicked__sub_y;
                    // Cell edited
                    this.InvokeOnEdit();
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
                    int hovered_selection_x = Math.Min(p.X / (_cellWidth * _selectionWidth),
                                                       _columnNum / _selectionWidth - 1);
                    int hovered_selection_y = Math.Min(p.Y / (_cellHeight * _selectionHeight),
                                                       _rowNum / _selectionHeight - 1);
                    if ((hovered_selection_x != _selection.ToX)
                        || (hovered_selection_y != _selection.ToY))
                    {
                        _selection.ToX = hovered_selection_x;
                        _selection.ToY = hovered_selection_y;
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
            switch (e.KeyData)
            {
                case Keys.Space:
                case Keys.Enter:
                    this.InvokeOnEdit();
                    _updated = true;
                    break;
                case Keys.Up | Keys.Shift:
                    if (_selection.ToY > 0 && AllowMultipleSelection)
                    {
                        _selection.ToY--;
                        _updated = true;
                        this.Refresh();
                    }
                    break;
                case Keys.Down | Keys.Shift:
                    if (_selection.ToY < _rowNum / _selectionHeight - 1 && AllowMultipleSelection)
                    {
                        _selection.ToY++;
                        _updated = true;
                        this.Refresh();
                    }
                    break;
                case Keys.Left | Keys.Shift:
                    if (_selection.ToX > 0 && AllowMultipleSelection)
                    {
                        _selection.ToX--;
                        _updated = true;
                        this.Refresh();
                    }
                    break;
                case Keys.Right | Keys.Shift:
                    if (_selection.ToX < _columnNum / _selectionWidth - 1 && AllowMultipleSelection)
                    {
                        _selection.ToX++;
                        _updated = true;
                        this.Refresh();
                    }
                    break;
                case Keys.Up:
                    if (AllowSubSelection)
                    {
                        if ((_sub.Y == 0) && (_selection.Y > 0))
                        {
                            _selection.Y--;
                            _sub.Y = _selectionHeight - 1;
                            _updated = true;
                            SelectionChanged?.Invoke(this, new EventArgs());
                            this.Refresh();
                        }
                        else if (_sub.Y > 0)
                        {
                            _sub.Y--;
                            _updated = true;
                            this.Refresh();
                        }
                    }
                    else
                    {
                        if (_selection.Y > 0)
                        {
                            _selection.Y--;
                            _updated = true;
                            SelectionChanged?.Invoke(this, new EventArgs());
                            this.Refresh();
                        }
                    }
                    break;
                case Keys.Down:
                    if (AllowSubSelection)
                    {
                        if ((_sub.Y == _selectionHeight - 1)
                            && (_selection.Y < _rowNum / _selectionHeight - 1))
                        {
                            _selection.Y++;
                            _sub.Y = 0;
                            _updated = true;
                            SelectionChanged?.Invoke(this, new EventArgs());
                            this.Refresh();
                        }
                        else if (_sub.Y > 0)
                        {
                            _sub.Y--;
                            _updated = true;
                            this.Refresh();
                        }
                    }
                    else
                    {
                        if (_selection.Y < _rowNum / _selectionHeight - 1)
                        {
                            _selection.Y++;
                            _updated = true;
                            SelectionChanged?.Invoke(this, new EventArgs());
                            this.Refresh();
                        }
                    }
                    break;
                case Keys.Left:
                    if (AllowSubSelection)
                    {
                        if ((_sub.X == 0) && (_selection.X > 0))
                        {
                            _selection.X--;
                            _sub.X = _selectionWidth - 1;
                            _updated = true;
                            SelectionChanged?.Invoke(this, new EventArgs());
                            this.Refresh();
                        }
                        else if (_sub.X > 0)
                        {
                            _sub.X--;
                            _updated = true;
                            this.Refresh();
                        }
                    }
                    else
                    {
                        if (_selection.X > 0)
                        {
                            _selection.X--;
                            _updated = true;
                            SelectionChanged?.Invoke(this, new EventArgs());
                            this.Refresh();
                        }
                    }
                    break;
                case Keys.Right:
                    if (AllowSubSelection)
                    {
                        if ((_sub.X == _selectionWidth - 1)
                            && (_selection.X < _columnNum / _selectionWidth - 1))
                        {
                            _selection.X++;
                            _sub.X = 0;
                            _updated = true;
                            SelectionChanged?.Invoke(this, new EventArgs());
                            this.Refresh();
                        }
                        else if (_sub.X < _selectionWidth - 1)
                        {
                            _sub.X++;
                            _updated = true;
                            this.Refresh();
                        }
                    }
                    else
                    {
                        if (_selection.X < _columnNum / _selectionWidth - 1)
                        {
                            _selection.X++;
                            _updated = true;
                            SelectionChanged?.Invoke(this, new EventArgs());
                            this.Refresh();
                        }
                    }
                    break;
            }
            base.OnPreviewKeyDown(e);
        }
        protected void InvokeOnEdit()
        {
            CellOnEdit?.Invoke(this, new EventArgs());
        }
    }
}
