using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Collections.Generic;

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
        protected FilterBase _filter;       // Filter to be applied
        protected Bitmap[,] _cellImg;       // Images for each cells
        protected Color[,] _background;     // Background colors
        private readonly PressedKeys _pressedKeys = new PressedKeys();  // For multiple key input
        private static readonly Pen _dashedGray = new Pen(Color.Gray)   // For overlayed selection
        {
            DashStyle = DashStyle.Dash,
            Width = 2
        };
        // Internal DnD object for multiple selections
        internal class DragSelection : DnDBase {
            internal DragSelection(Control sender) : base(sender) { }
        }
        internal class DragEditing : DnDBase
        {
            internal DragEditing(Control sender) : base(sender) { }
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
            set => _columnNum = value;
        }
        [Browsable(true)]
        [Description("Number of rows to be controlled")]
        public int RowNum
        {
            get => _rowNum;
            set => _rowNum = value;
        }
        [Browsable(true)]
        [Description("Number of columns in one selection")]
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
        [Description("Number of rows in one selection")]
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
        [Description("Sub selection inside one selection")]
        public bool AllowSelection
        {
            get;
            set;
        } = true;
        [Browsable(true)]
        [Description("Sub selection inside one selection")]
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
        [Browsable(true)]
        [Description("Allow editing with mouse drag")]
        public bool AllowOneStrokeEditing
        {
            get;
            set;
        } = false;
        [Browsable(true)]
        [Description("Draw transparent color for background")]
        public bool DrawTransparentColor
        {
            get;
            set;
        } = false;
        //--------------------------------------------------------------------
        // Event handlers
        [Browsable(true)]
        [Description("Called when selection has been changed")]
        public event EventHandler<EventArgs> SelectionChanged;
        [Browsable(true)]
        [Description("Called when cell was dragged")]
        public event EventHandler<EventArgs> CellDragStart;
        public class EditEventArgs : EventArgs
        {
            public EditEventArgs(bool should_push)
            {
                ShouldPush = should_push;
            }
            public bool ShouldPush { get; }
        }
        [Browsable(true)]
        [Description("Called when a cell was going to be edited")]
        public event EventHandler<EditEventArgs> CellOnEdit;
        public class ScrollEventArgs : EventArgs
        {
            public int DX { get; set; }
            public int DY { get; set; }
        }
        [Browsable(true)]
        [Description("Called on scroll actions")]
        public event EventHandler<ScrollEventArgs> MatrixOnScroll;
        //--------------------------------------------------------------------
        // Properties for hosts
        [Browsable(false)]
        public Selection Selector => _selection;
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
        public Rectangle SelectedRect => _selection.Selected;
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
            // Get linear index of selection
            get => _selection.Y * SelectionColNum + _selection.X;
            set
            {
                _selection.X = value % SelectionColNum;
                _selection.Y = value / SelectionColNum;
            }
        }
        [Browsable(false)]
        // Columns of selectable tiles
        // E.g. map has 32 columns and 16 selectable tiles within
        public int SelectionColNum => _columnNum / _selectionWidth;
        [Browsable(false)]
        // Rows of selectable tiles
        public int SelectionRowNum => _rowNum / _selectionHeight;
        [Browsable(false)]
        public int SubX => _sub.X;
        [Browsable(false)]
        public int SubY => _sub.Y;
        [Browsable(false)]
        // For overlayed sprite
        public bool DrawOverlayedSelection { get; set; } = false;
        //--------------------------------------------------------------------
        // Methods for hosts
        public void SetImage(Bitmap img, int col, int row)
        {
            // Set images to be drawn
            _cellImg ??= new Bitmap[_columnNum, _rowNum];
            if ((_cellImg.GetLength(0) < _columnNum) || (_cellImg.GetLength(1) < _rowNum))
            {
                _cellImg = new Bitmap[_columnNum, _rowNum];
            }
            _cellImg[col, row] = img;
            _updated = true;
        }
        public void SetBackgroundColor(Color c, int col, int row)
        {
            // Set the background colors of each cells
            _background ??= new Color[_columnNum, _rowNum];
            if((_background.GetLength(0) < _columnNum) || (_background.GetLength(1) < _rowNum))
            {
                _background = new Color[_columnNum, _rowNum];
            }
            _background[col, row] = c;
            _updated = true;
        }
        public (int col, int row) ScreenCoordinateToCell(Point screen_p)
        {
            // Screen coordinate to col row index of each cells
            Point p = this.PointToClient(screen_p);
            int col = Math.Clamp(p.X / _cellWidth, 0, _columnNum);
            int row = Math.Clamp(p.Y / _cellHeight, 0, _rowNum);
            return (col, row);
        }
        public (int col, int row) ScreenCoordinateToSelection(Point screen_p)
        {
            // Screen coordinate to col row index of selection
            Point p = this.PointToClient(screen_p);
            int col = Math.Clamp(p.X / (_cellWidth * _selectionWidth), 0, _columnNum);
            int row = Math.Clamp(p.Y / (_cellHeight * _selectionHeight), 0, _rowNum);
            return (col, row);
        }
        public int ScreenCoordinateToIndex(Point screen_p)
        {
            // Screen coordinate to linear index
            (int col, int row) = ScreenCoordinateToSelection(screen_p);
            return IndexOf(col, row);
        }
        public int IndexOf(int col, int row)
        {
            // Columns and rows to linear index
            return row * (_columnNum / _selectionWidth) + col;
        }
        public void IncrementSelection()
        {
            // Increment current selection rightward
            if(_selection.X < SelectionColNum - 1)
            {
                _selection.X++;
                _updated = true;
            }
        }
        public void ForEachSelection(Action<int, int> callback)
        {
            // Callback for each selection if its cols and rows are valid
            this.ForEachSelection(_selection.Selected.X, _selection.Selected.Y,
                                  _selection.Selected.Width, _selection.Selected.Height, callback);
        }
        public void ForEachSelection(int col, int row, int w, int h,
                                     Action<int, int> callback)
        {
            // Callback for each selection if its cols and rows are valid
            for (int y = row; (y < row + h) && (y < SelectionRowNum); ++y)
            {
                for (int x = col; (x < col + w) && (x < SelectionColNum); ++x)
                {
                    callback?.Invoke(x, y);
                }
            }
        }
        public void ForEachSelection(int col, int row, int w, int h,
                                     Action<int, int, int, int> callback)
        {
            // Callback for each selection if its cols and rows are valid
            int y_cnt = 0;
            for (int y = row; (y < row + h) && (y < SelectionRowNum); ++y)
            {
                int x_cnt = 0;
                for (int x = col; (x < col + w) && (x < SelectionColNum); ++x)
                {
                    callback?.Invoke(x, y, x_cnt, y_cnt);
                    x_cnt++;
                }
                y_cnt++;
            }
        }
        public void ResetMultipleSelection()
        {
            // Cancel the current multiple selection
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
            if((_bmp.Width != ColumnNum * CellWidth) || (_bmp.Height != RowNum * CellHeight))
            {
                _bmp = new Bitmap(ColumnNum * CellWidth, RowNum * CellHeight);
            }
            if (_updated)
            {
                // If something has been changed, redraw the buffer
                Graphics g = Graphics.FromImage(_bmp);
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                if (DrawTransparentColor)
                {
                    Utility.DrawTransparent(_bmp);
                }
                // Draw background colors of each cells
                if (_background != null)
                {
                    for (int y = 0; y < RowNum; ++y)
                    {
                        for (int x = 0; x < ColumnNum; ++x)
                        {
                            if (_background[x, y] == Color.Empty) continue;
                            SolidBrush b = new SolidBrush(_background[x, y]);
                            g.FillRectangle(b, x * _cellWidth, y * _cellHeight,
                                            _cellWidth, _cellHeight);
                        }
                    }
                }
                // Draw images of each cells
                if (_cellImg != null)
                {
                    for (int y = 0; y < RowNum; ++y)
                    {
                        for (int x = 0; x < ColumnNum; ++x)
                        {
                            if (_cellImg[x, y] == null) continue;
                            g.FillRectangle(Brushes.Black, x * _cellWidth, y * _cellHeight,
                                            _cellWidth, _cellHeight);
                            g.DrawImage(_cellImg[x, y], x * _cellWidth, y * _cellHeight,
                                        _cellWidth + 1, _cellHeight + 1);
                        }
                    }
                }
                // Apply filter
                _filter?.Process(_bmp);
                // Draw Selection
                if (AllowSelection)
                {
                    Utility.DrawSelection(g, _selection, this.Focused);
                    if (this.Focused && this.AllowSubSelection)
                    {
                        Utility.DrawSubSelection(g,
                            (_selection.X * _selectionWidth + _sub.X) * _cellWidth,
                            (_selection.Y * _selectionHeight + _sub.Y) * _cellHeight,
                            _cellWidth - 2, CellHeight - 2);
                    }
                    // Overlayed selection
                    if (DrawOverlayedSelection)
                    {
                        // For overlayed sprite selection
                        int index = (Index + 1) % 64;
                        int x = index % SelectionColNum;
                        int y = index / SelectionColNum;
                        g.DrawRectangle(_dashedGray, x * _selectionWidth * _cellWidth,
                            y * _selectionHeight * _cellHeight,
                            _selectionWidth * _cellWidth - 1,
                            _selectionHeight * _cellHeight - 1);
                    }
                }
                _updated = false;
            }
            // Copy from buffer
            e.Graphics.DrawImage(_bmp, 0, 0);
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            // Grab key events afterward
            // Needed because this override prevents the base method call,
            // and have to do this here since other window may open below
            this.Focus();
            if (e.Button == MouseButtons.Left)
            {
                // Coordinate to selection col/row
                int clicked_selection_x = Math.Min(e.X / (_cellWidth * _selectionWidth),
                                                   _columnNum / _selectionWidth - 1);
                int clicked_selection_y = Math.Min(e.Y / (_cellHeight * _selectionHeight),
                                                   _rowNum / _selectionHeight - 1);
                // Coordinate to sub selection col/row
                int clicked__sub_x = Math.Min((e.X / _cellWidth) % _selectionWidth,
                                              _selectionWidth - 1);
                int clicked__sub_y = Math.Min((e.Y / _cellHeight) % _selectionHeight,
                                              _selectionHeight - 1);
                if ((clicked_selection_x != _selection.X)
                    || (clicked_selection_y != _selection.Y))
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
                        SelectionChanged?.Invoke(this, EventArgs.Empty);
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
                    this.InvokeOnEdit(should_push: true);
                    // Cell to be dragged
                    CellDragStart?.Invoke(this, EventArgs.Empty);
                    if(AllowOneStrokeEditing)
                    {
                        this.DoDragDrop(new DragEditing(this), DragDropEffects.Copy);
                    }
                }
                // Base method deactivates the new opened window at above
                //base.OnMouseDown(e);
            }
        }
        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            //drgevent.Effect = DragDropEffects.None;
            if (drgevent.Data.GetDataPresent(typeof(DragSelection)))
            {
                // Multiple selection
                drgevent.Effect = DragDropEffects.Copy;
            }
            else if (drgevent.Data.GetDataPresent(typeof(DragEditing)))
            {
                // One stroke editing
                drgevent.Effect = DragDropEffects.Copy;
            }
            base.OnDragEnter(drgevent);
        }
        protected override void OnDragOver(DragEventArgs drgevent)
        {
            Point p = this.PointToClient(Cursor.Position);
            int hovered_selection_x = Math.Min(p.X / (_cellWidth * _selectionWidth),
                                               _columnNum / _selectionWidth - 1);
            int hovered_selection_y = Math.Min(p.Y / (_cellHeight * _selectionHeight),
                                               _rowNum / _selectionHeight - 1);
            if (drgevent.Data.GetDataPresent(typeof(DragSelection)))
            {
                // Multiple selection
                if ((hovered_selection_x != _selection.ToX)
                    || (hovered_selection_y != _selection.ToY))
                {
                    _selection.ToX = hovered_selection_x;
                    _selection.ToY = hovered_selection_y;
                    _updated = true;
                    this.Refresh();
                }
            }
            else if (drgevent.Data.GetDataPresent(typeof(DragEditing)))
            {
                // On one stroke editing, update current selection also
                int hovered_cell_x = Math.Min(p.X / _cellWidth, _columnNum - 1);
                int hovered_cell_y = Math.Min(p.Y / _cellHeight, _rowNum - 1);
                int sub_x = hovered_cell_x % _selectionWidth;
                int sub_y = hovered_cell_y % _selectionHeight;
                if((_selection.X != hovered_selection_x) || (_selection.Y != hovered_selection_y)
                    || (_sub.X != sub_x) || (_sub.Y != sub_y))
                {
                    // Selection changed
                    _selection.X = hovered_selection_x;
                    _selection.Y = hovered_selection_y;
                    _sub.X = sub_x;
                    _sub.Y = sub_y;
                    _updated = true;
                    this.Refresh();
                    // Selection changed
                    SelectionChanged?.Invoke(this, EventArgs.Empty);
                    // Cell edited
                    this.InvokeOnEdit(should_push: false);
                }
            }
            base.OnDragOver(drgevent);
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            _pressedKeys.Add(e.KeyCode);
        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            _pressedKeys.Remove(e.KeyCode);
        }
        protected override void OnGotFocus(EventArgs e)
        {
            _pressedKeys.Clear();
        }
        protected override void OnLostFocus(EventArgs e)
        {
            _pressedKeys.Clear();
        }
        protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
        {
            // Key events in sandbox
            switch (e.KeyData)
            {
                case Keys.Space:
                case Keys.Enter:
                    // Space and enter for editing
                    this.InvokeOnEdit(should_push: true);
                    _updated = true;
                    break;
                case Keys.Up | Keys.Shift:
                    // Multiple selection
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
                            // Selection upward
                            _selection.Y--;
                            _sub.Y = _selectionHeight - 1;
                            _updated = true;
                            SelectionChanged?.Invoke(this, EventArgs.Empty);
                            this.Refresh();
                        }
                        else if (_sub.Y > 0)
                        {
                            // Sub selection upward
                            _sub.Y--;
                            _updated = true;
                            this.Refresh();
                        }
                    }
                    else
                    {
                        if (_selection.Y > 0)
                        {
                            // Selection upward
                            _selection.Y--;
                            _updated = true;
                            SelectionChanged?.Invoke(this, EventArgs.Empty);
                            this.Refresh();
                        }
                        else
                        {
                            // Scroll upward if needed
                            ScrollEventArgs se = new ScrollEventArgs()
                            {
                                DX = 0,
                                DY = -1
                            };
                            MatrixOnScroll?.Invoke(this, se);
                        }
                    }
                    if (_pressedKeys.IsPressed(Keys.Space))
                    {
                        // When space is pressed while moving, invoke editing
                        this.InvokeOnEdit(should_push: true);
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
                            SelectionChanged?.Invoke(this, EventArgs.Empty);
                            this.Refresh();
                        }
                        else if (_sub.Y < _selectionHeight - 1)
                        {
                            _sub.Y++;
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
                            SelectionChanged?.Invoke(this, EventArgs.Empty);
                            this.Refresh();
                        }
                        else
                        {
                            ScrollEventArgs se = new ScrollEventArgs()
                            {
                                DX = 0,
                                DY = 1
                            };
                            MatrixOnScroll?.Invoke(this, se);
                        }
                    }
                    if (_pressedKeys.IsPressed(Keys.Space))
                    {
                        // When space is pressed while moving, invoke editing
                        this.InvokeOnEdit(should_push: true);
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
                            SelectionChanged?.Invoke(this, EventArgs.Empty);
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
                            SelectionChanged?.Invoke(this, EventArgs.Empty);
                            this.Refresh();
                        }
                        else
                        {
                            ScrollEventArgs se = new ScrollEventArgs()
                            {
                                DX = -1,
                                DY = 0
                            };
                            MatrixOnScroll?.Invoke(this, se);
                        }
                    }
                    if (_pressedKeys.IsPressed(Keys.Space))
                    {
                        // When space is pressed while moving, invoke editing
                        this.InvokeOnEdit(should_push: true);
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
                            SelectionChanged?.Invoke(this, EventArgs.Empty);
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
                            SelectionChanged?.Invoke(this, EventArgs.Empty);
                            this.Refresh();
                        }
                        else
                        {
                            ScrollEventArgs se = new ScrollEventArgs()
                            {
                                DX = 1,
                                DY = 0
                            };
                            MatrixOnScroll?.Invoke(this, se);
                        }
                    }
                    if (_pressedKeys.IsPressed(Keys.Space))
                    {
                        // When space is pressed while moving, invoke editing
                        this.InvokeOnEdit(should_push: true);
                    }
                    break;
            }
            base.OnPreviewKeyDown(e);
        }
        // For derived classes
        protected void InvokeOnEdit(bool should_push)
        {
            CellOnEdit?.Invoke(this, new EditEventArgs(should_push));
        }
        // For multiple key input
        internal class PressedKeys
        {
            private readonly List<Keys> _pressed = new List<Keys>();
            public void Add(Keys key) 
            {
                _pressed.Add(key);
            }
            public void Remove(Keys key)
            {
                bool val = _pressed.Remove(key);
                while (val)
                {
                    val = _pressed.Remove(key);
                }
            }
            public bool IsPressed(Keys key)
            {
                return _pressed.Contains(key);
            }
            public void Clear()
            {
                _pressed.Clear();
            }
        }
    }
}
