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
        //--------------------------------------------------------------------
        // Initialize
        public EditorControl()
        {
            InitializeComponent();
        }
        //--------------------------------------------------------------------
        // Properties
        //--------------------------------------------------------------------
        // Event handlers
        public class AddKeyEventArgs : EventArgs
        {
            public enum Type
            {
                PlusMinus,
                Brackets
            }
            public Type KeyType { get; set; } 
            public int Value { get; set; }
            public AddKeyEventArgs(Type type, int val)
            {
                KeyType = type;
                Value = val;
            }
        }
        [Browsable(true)]
        [Description("Called on additional key event")]
        public event EventHandler<AddKeyEventArgs> AddKeyPressed;
        //--------------------------------------------------------------------
        // Methods and properties for hosts
        public void SetBrush(Brush b, int col, int row)
        {
            // Set brush to not to be created everytime
            _brush ??= new Brush[ColumnNum, RowNum];
            _brush[col, row] = b;
            _updated = true;
        }
        [Browsable(false)]
        public (int x, int y) PosInChr()
        {
            // Position in one 8x8 character
            return (_sub.X, _selection.Y % 8);
        }
        public (int x, int y) PosInEditor()
        {
            // Position in whole editor control
            int x = _selection.X * _selectionWidth + _sub.X;
            int y = _selection.Y * _selectionHeight + _sub.Y;
            return (x, y);
        }
        public int ChrColNum
        {
            // The number of horizontal characters in one edit control
            get => _columnNum / 8;
        }
        public int ChrRowNum
        {
            // The number of vertical characters in one edit control
            get => _rowNum / 8;
        }
        //--------------------------------------------------------------------
        // Overrides
        protected override void OnPaint(PaintEventArgs e)
        {
            // Draw to cells to buffer
            _bmp ??= new Bitmap(ColumnNum * CellWidth, RowNum * CellHeight);
            if (_updated)
            {
                // If something has been changed, redraw the buffer
                if(this.DrawTranparentColor)
                {
                    Utility.DrawTransparent(_bmp);
                }
                Graphics g = Graphics.FromImage(_bmp);
                if (_brush != null)
                {
                    for (int y = 0; y < RowNum; ++y)
                    {
                        for (int x = 0; x < ColumnNum; ++x)
                        {
                            if (_brush[x, y] != null)
                            {
                                // Draw outline
                                g.FillRectangle(Brushes.Gray, x * _cellWidth, y * _cellHeight,
                                                _cellWidth, _cellHeight);
                                // Draw one dot
                                g.FillRectangle(_brush[x, y], x * _cellWidth, y * _cellHeight,
                                                _cellWidth - 1, _cellHeight - 1);
                            }
                        }
                    }
                }
                // Draw selection
                Utility.DrawSelection(g, _selection, this.Focused);
                if (this.Focused && this.AllowSubSelection)
                {
                    // One dot can be selected when focused
                    Utility.DrawSubSelection(g,
                                             (_selection.X * _selectionWidth + _sub.X) * _cellWidth,
                                             (_selection.Y * _selectionHeight + _sub.Y) * _cellHeight,
                                             _cellWidth - 2, CellHeight - 2);
                }
                _updated = false;
            }
            // Copy from buffer
            e.Graphics.DrawImage(_bmp, 0, 0);
        }
        protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
        {
            // Key events
            switch (e.KeyData)
            {
                case Keys.D1:
                case Keys.NumPad1:
                    // Events for 1-8 key
                    _sub.X = 0;
                    _updated = true;
                    this.InvokeOnEdit();
                    break;
                case Keys.D2:
                case Keys.NumPad2:
                    _sub.X = 1;
                    _updated = true;
                    this.InvokeOnEdit();
                    break;
                case Keys.D3:
                case Keys.NumPad3:
                    _sub.X = 2;
                    _updated = true;
                    this.InvokeOnEdit();
                    break;
                case Keys.D4:
                case Keys.NumPad4:
                    _sub.X = 3;
                    _updated = true;
                    this.InvokeOnEdit();
                    break;
                case Keys.D5:
                case Keys.NumPad5:
                    _sub.X = 4;
                    _updated = true;
                    this.InvokeOnEdit();
                    break;
                case Keys.D6:
                case Keys.NumPad6:
                    _sub.X = 5;
                    _updated = true;
                    this.InvokeOnEdit();
                    break;
                case Keys.D7:
                case Keys.NumPad7:
                    _sub.X = 6;
                    _updated = true;
                    this.InvokeOnEdit();
                    break;
                case Keys.D8:
                case Keys.NumPad8:
                    _sub.X = 7;
                    _updated = true;
                    this.InvokeOnEdit();
                    break;
                case Keys.Oemplus:
                case Keys.Add:
                    {
                        // Event for + key
                        AddKeyEventArgs ke = new AddKeyEventArgs(AddKeyEventArgs.Type.PlusMinus, 1);
                        AddKeyPressed?.Invoke(this, ke);
                    }
                    break;
                case Keys.OemMinus:
                case Keys.Subtract:
                    {
                        // Event for - key
                        AddKeyEventArgs ke = new AddKeyEventArgs(AddKeyEventArgs.Type.PlusMinus, -1);
                        AddKeyPressed?.Invoke(this, ke);
                    }
                    break;
                case Keys.OemCloseBrackets:
                    {
                        // Event for ] key
                        AddKeyEventArgs ke = new AddKeyEventArgs(AddKeyEventArgs.Type.Brackets, 1);
                        AddKeyPressed?.Invoke(this, ke);
                    }
                    break;
                case Keys.OemOpenBrackets:
                    {
                        // Event for [ key
                        AddKeyEventArgs ke = new AddKeyEventArgs(AddKeyEventArgs.Type.Brackets, -1);
                        AddKeyPressed?.Invoke(this, ke);
                    }
                    break;
            }
            base.OnPreviewKeyDown(e);
        }
    }
}
