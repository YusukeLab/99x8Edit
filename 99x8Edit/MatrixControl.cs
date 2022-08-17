using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace _99x8Edit
{
    public partial class MatrixControl : UserControl
    {
        private int _cellWidth;         // Width of one cell
        private int _cellHeight;        // Height of one cell
        private int _columnNum;         // Number of columns
        private int _rowNum;            // Number of rows
        private Bitmap _bmp;            // Buffer to draw images
        private bool _updated;          // Should be drawn to buffer
        private Bitmap[,] _cellImg;     // Images for each cells
        private Selection _selection;   // Current selection
        private FilterBase _filter;     // Filter to be applyed
        // For dragging action of multiple selection
        public class DragSelection {
            private UserControl _sender;
            public DragSelection(UserControl sender)
            {
                _sender = sender;
            } 
            public UserControl Sender
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
        public int CellWidth
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
        public int CellHeight
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
        // Methods and prioerties for hosts
        public Selection Selection
        {
            get => _selection;
        }
        public void SetImage(Bitmap img, int col, int row)
        {
            if (_cellImg == null)
            {
                _cellImg = new Bitmap[ColumnNum, RowNum];
            }
            _cellImg[col, row] = img;
            _updated = true;
        }
        public FilterBase Filter
        {
            set
            {
                _filter = value;
                _updated = true;
            }
        }
        public int SelectedIndex
        {
            get => _selection.Y * ColumnNum + _selection.X;
        }
        public void SelectNext()
        {
            if (_selection.X < ColumnNum - 1)
            {
                _selection.X++;
                _updated = true;
            }
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
            if (_bmp == null)
            {
                _bmp = new Bitmap(ColumnNum * CellWidth, RowNum * CellHeight);
            }
            if (_updated)
            {
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
                int clicled_cell_y = Math.Min(e.Y / CellHeight, RowNum - 1);
                if ((clicked_cell_x != _selection.X) || (clicked_cell_x != _selection.Y))
                {
                    // Selection changed
                    if (Control.ModifierKeys == Keys.Shift)
                    {
                        // Multiple selection
                        _selection.ToX = clicked_cell_x;
                        _selection.ToY = clicled_cell_y;
                    }
                    else
                    {
                        // New selection
                        _selection.X = clicked_cell_x;
                        _selection.Y = clicled_cell_y;
                        // For host
                        SelectionChanged?.Invoke(this, new EventArgs());
                    }
                    _updated = true;
                    this.Refresh();
                    // Drag for multiple selection
                    this.DoDragDrop(new DragSelection(this), DragDropEffects.Copy);
                }
                else
                {
                    // Cell to be dragged
                    CellDragStart?.Invoke(this, e);
                }
            }
            base.OnMouseDown(e);
        }
        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            drgevent.Effect = DragDropEffects.None;
            if (drgevent.Data.GetDataPresent(typeof(DragSelection)))
            {
                // Accept only the drag objects created by itself
                dynamic obj = drgevent.Data.GetData(typeof(DragSelection));
                if(obj.Sender == this)
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
                // Multiple selection
                Point p = this.PointToClient(Cursor.Position);
                int hoverd_x = Math.Min(p.X / CellWidth, ColumnNum - 1);
                int hoverd_y = Math.Min(p.Y / CellHeight, RowNum - 1);
                if((hoverd_x != _selection.ToX) || (hoverd_y != Selection.ToY))
                {
                    _selection.ToX = hoverd_x;
                    _selection.ToY = hoverd_y;
                    _updated = true;
                    this.Refresh();
                }
            }
            base.OnDragOver(drgevent);
        }
        protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
        {
            // Key events in sandbox
            switch (e.KeyData)
            {
                case Keys.Up | Keys.Shift:
                    if (_selection.ToY > 0)
                    {
                        _selection.ToY--;
                        _updated = true;
                        this.Refresh();
                    }
                    break;
                case Keys.Down | Keys.Shift:
                    if (_selection.ToY < RowNum - 1)
                    {
                        _selection.ToY++;
                        _updated = true;
                        this.Refresh();
                    }
                    break;
                case Keys.Left | Keys.Shift:
                    if (_selection.ToX > 0)
                    {
                        _selection.ToX--;
                        _updated = true;
                        this.Refresh();
                    }
                    break;
                case Keys.Right | Keys.Shift:
                    if (_selection.ToX < ColumnNum - 1)
                    {
                        _selection.ToX++;
                        _updated = true;
                        this.Refresh();
                    }
                    break;
                case Keys.Up:
                    if (_selection.Y > 0)
                    {
                        _selection.Y--;
                        SelectionChanged?.Invoke(this, new EventArgs());
                        _updated = true;
                        this.Refresh();
                    }
                    break;
                case Keys.Down:
                    if (_selection.Y < RowNum - 1)
                    {
                        _selection.Y++;
                        SelectionChanged?.Invoke(this, new EventArgs());
                        _updated = true;
                        this.Refresh();
                    }
                    break;
                case Keys.Left:
                    if (_selection.X > 0)
                    {
                        _selection.X--;
                        SelectionChanged?.Invoke(this, new EventArgs());
                        _updated = true;
                        this.Refresh();
                    }
                    break;
                case Keys.Right:
                    if (_selection.X < ColumnNum - 1)
                    {
                        _selection.X++;
                        SelectionChanged?.Invoke(this, new EventArgs());
                        _updated = true;
                        this.Refresh();
                    }
                    break;
            }
            base.OnPreviewKeyDown(e);
        }
    }
}
