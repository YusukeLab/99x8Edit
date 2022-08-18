using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace _99x8Edit
{
    // Multiple cell selection for user interface
    public class Selection
    {
        private int _cellW;         // Size of cells to display
        private int _cellH;
        private int _x;             // Current col and row
        private int _y;
        private int _tx;            // Multiple selection to
        private int _ty;
        Rectangle _selection;       // Selected cols and rows
        Rectangle _display;         // Selection on display coodinate
        public Selection(int cell_width, int cell_height)
        {
            // Width and height are for calculating coodinates
            _display.Width = _cellW = cell_width;
            _display.Height = _cellH = cell_height;
        }
        public int X
        {
            get { return _x; }
            set
            {
                _x = value;
                this.ResetSelectionAndUpdate();  // Multiple selection cancelled
            }
        }
        public int Y
        {
            get { return _y; }
            set
            {
                _y = value;
                this.ResetSelectionAndUpdate();  // Multiple selection cancelled
            }
        }
        public int ToX
        {
            get { return _tx; }
            set
            {
                _tx = value;
                this.Update();
            }
        }
        public int ToY
        {
            get { return _ty; }
            set
            {
                _ty = value;
                this.Update();
            }
        }
        // Col and row, width and height of the selection
        public Rectangle Selected
        {
            get { return _selection; }
        }
        // Coodinates in one control
        internal Rectangle Display
        {
            get{ return _display; }
        }
        // Coodinates in whole screen
        internal Rectangle GetScreenPos(Control c)
        {
            Point p = new Point(_display.X, _display.Y);
            p = c.PointToScreen(p);
            return new Rectangle(p.X, p.Y, _display.Width, _display.Height);
        }
        internal void ResetSelectionAndUpdate()
        {
            _tx = _x;
            _ty = _y;
            this.Update();
        }
        private void Update()
        {
            _display.X = (_selection.X = Math.Min(_x, _tx)) * _cellW;
            _display.Y = (_selection.Y = Math.Min(_y, _ty)) * _cellH;
            _display.Width = (_selection.Width = Math.Abs(_x - _tx) + 1) * _cellW - 1;
            _display.Height = (_selection.Height = Math.Abs(_y - _ty) + 1) * _cellH - 1;
        }
    }
    internal class TabOrder
    {
        // Customized tab order and the selection corresponding to
        private List<Control> _ctrl = new List<Control>();      // Controls to be added
        private List<Selection> _sel = new List<Selection>();   // Selected cell in the control
        internal void Add(Control c, Selection s)
        {
            _ctrl.Add(c);
            _sel.Add(s);
        }
        internal Control NextOf(Control c, bool forward)
        {
            int index = _ctrl.IndexOf(c);
            index += forward ? 1 : _ctrl.Count - 1;
            index %= _ctrl.Count;
            return _ctrl[index];
        }
        internal Selection SelectionOf(Control c)
        {
            int index = Math.Max(_ctrl.IndexOf(c), 0);
            return _sel[index];
        }
    }
    internal class Utility
    {
        internal static Pen DashedGray = new Pen(Color.Gray) { DashStyle = 
                                       System.Drawing.Drawing2D.DashStyle.Dash, Width = 2 };
        //--------------------------------------------------------------------
        // Utility functions
        //--------------------------------------------------------------------
        // Misc
        internal static void Rotate16(ref byte srcL, ref byte srcR)
        {
            byte c = (byte)(srcR & 1);
            srcR >>= 1;
            srcR |= (byte)((srcL & 1) << 7);
            srcL >>= 1;
            srcL |= (byte)(c << 7);
        }
        //--------------------------------------------------------------------
        // User interface
        internal static void DrawSelection(Graphics g, Selection s, bool focused)
        {
            DrawSelection(g, s.Display.X, s.Display.Y, s.Display.Width, s.Display.Height, focused);
        }
        internal static void DrawSelection(Graphics g, int x, int y, int w, int h, bool focused)
        {
            g.DrawRectangle(Pens.Green, x, y, w, h);
            g.DrawRectangle(Pens.Green, x + 1, y + 1, w - 2, h - 2);
            // Draw dot on bottom right
            if (focused)
            {
                g.FillRectangle(Brushes.White, x + w - 4, y + h - 4, 4, 4);
                Brush b = focused ? Brushes.Green : Brushes.DarkGreen;
                g.FillRectangle(b, x + w - 3, y + h - 3, 3, 3);
            }
        }
        internal static void DrawSubSelection(Graphics g, int x, int y, int w, int h)
        {
            g.DrawRectangle(Pens.Yellow, x, y, w, h);
        }
        internal static void DrawTransparent(Bitmap bmp)
        {
            Graphics g = Graphics.FromImage(bmp);
            for (int y = 0; y < bmp.Height / 16; ++y)
            {
                for (int x = 0; x < bmp.Width / 16; ++x)
                {
                    g.FillRectangle(Brushes.DarkGray, x * 16, y * 16, 8, 8);
                    g.FillRectangle(Brushes.Gray, x * 16 + 8, y * 16, 8, 8);
                    g.FillRectangle(Brushes.Gray, x * 16, y * 16 + 8, 8, 8);
                    g.FillRectangle(Brushes.DarkGray, x * 16 + 8, y * 16 + 8, 8, 8);
                }
            }
        }
        //--------------------------------------------------------------------
        // File
        internal static bool SaveDialogAndSave(string current_file,
                                               string dialog_filter,
                                               string dialog_title,
                                               Action<BinaryWriter> exec_save,
                                               bool save_as,
                                               out string saved_file)
        {
            saved_file = "";
            Func<string, bool> do_save = (file_name) =>
            {
                using BinaryWriter br = new BinaryWriter(new FileStream(file_name, FileMode.Create));

                try
                {
                    exec_save(br);
                    return true;
                }
                catch (Exception ex) when (!System.Diagnostics.Debugger.IsAttached)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return false;
            };
            String dir = Path.GetDirectoryName(current_file);
            dir ??= System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            if(!save_as)
            {
                // Directory exists and want to overwrite
                if(do_save(current_file))
                {
                    saved_file = current_file;
                    return true;
                }
                return false;
            }
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.InitialDirectory = dir;
            dlg.Filter = dialog_filter;
            dlg.FilterIndex = 1;
            dlg.Title = dialog_title;
            dlg.RestoreDirectory = true;
            dlg.OverwritePrompt = true;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                if (do_save(dlg.FileName))
                {
                    saved_file = dlg.FileName;
                    return true;
                }
            }
            return false;
        }
        internal static bool LoadDialogAndLoad(string current_file,
                                               string dialog_filter,
                                               string dialog_title,
                                               Action<BinaryReader> exec_load,
                                               bool push,
                                               out string loaded_file_name)
        {
            loaded_file_name = "";
            String dir = Path.GetDirectoryName(current_file);
            dir ??= System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = dir;
            dlg.Filter = dialog_filter;
            dlg.FilterIndex = 1;
            dlg.Title = dialog_title;
            dlg.RestoreDirectory = true;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                using BinaryReader br = new BinaryReader(new FileStream(dlg.FileName, FileMode.Open));
                try
                {
                    if(push)
                    {
                        MementoCaretaker.Instance.Push();
                    }
                    exec_load(br);
                    loaded_file_name = dlg.FileName;
                    return true;
                }
                catch (Exception ex) when (!System.Diagnostics.Debugger.IsAttached)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return false;
        }
        internal static bool ImportDialogAndImport(string current_file,
                                                   string filter,
                                                   string title,
                                                   Action<string, int> exec_import)
        {
            String dir = Path.GetDirectoryName(current_file);
            dir ??= System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = dir;
            dlg.Filter = filter;
            dlg.FilterIndex = 1;
            dlg.Title = title;
            dlg.RestoreDirectory = true;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    MementoCaretaker.Instance.Push();
                    exec_import(dlg.FileName, dlg.FilterIndex - 1);
                    return true;
                }
                catch (Exception ex) when (!System.Diagnostics.Debugger.IsAttached)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return false;
        }
        internal static bool ExportDialogAndExport(string current_file,
                                                   string dialog_title,
                                                   string filter,
                                                   Action<int, string> exec_save)
        {
            String dir = Path.GetDirectoryName(current_file);
            dir ??= System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "";
            dlg.InitialDirectory = dir;
            dlg.Filter = filter;
            dlg.FilterIndex = 1;
            dlg.Title = dialog_title;
            dlg.RestoreDirectory = true;
            dlg.OverwritePrompt = true;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    exec_save(dlg.FilterIndex - 1, dlg.FileName);
                    return true;
                }
                catch (Exception ex) when (!System.Diagnostics.Debugger.IsAttached)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return false;
        }
    }
}
