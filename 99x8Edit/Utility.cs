using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace _99x8Edit
{
    public class Utility
    {
        internal static void DrawTransparent(Bitmap bmp)
        {
            Graphics g = Graphics.FromImage(bmp);
            for (int y = 0; y < bmp.Height / 16; ++y)
            {
                for (int x = 0; x < bmp.Width / 16; ++x)
                {
                    g.FillRectangle(new SolidBrush(Color.DarkGray), x * 16, y * 16, 8, 8);
                    g.FillRectangle(new SolidBrush(Color.Gray), x * 16 + 8, y * 16, 8, 8);
                    g.FillRectangle(new SolidBrush(Color.Gray), x * 16, y * 16 + 8, 8, 8);
                    g.FillRectangle(new SolidBrush(Color.DarkGray), x * 16 + 8, y * 16 + 8, 8, 8);
                }
            }
        }
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
                BinaryWriter br = new BinaryWriter(new FileStream(file_name, FileMode.Create));
                try
                {
                    exec_save(br);
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    br.Close();
                }
                return false;
            };
            String dir = Path.GetDirectoryName(current_file);
            if (dir == null)
            {
                dir = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            }
            else if(!save_as)
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
                                               bool push_before_loading,
                                               out string loaded_file_name)
        {
            loaded_file_name = "";
            String dir = Path.GetDirectoryName(current_file);
            if (dir == null)
            {
                dir = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            }
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = dir;
            dlg.Filter = dialog_filter;
            dlg.FilterIndex = 1;
            dlg.Title = dialog_title;
            dlg.RestoreDirectory = true;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                BinaryReader br = new BinaryReader(new FileStream(dlg.FileName, FileMode.Open));
                try
                {
                    if(push_before_loading)
                    {
                        MementoCaretaker.Instance.Push();
                    }
                    exec_load(br);
                    loaded_file_name = dlg.FileName;
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    br.Close();
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
            if (dir == null)
            {
                dir = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            }
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
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return false;
        }
        internal static bool ExportDialogAndExport(string current_file,
                                                   string dialog_title,
                                                   Export.Type type,
                                                   Action<Export.Type, string> exec_save)
        {
            String dir = Path.GetDirectoryName(current_file);
            if (dir == null)
            {
                dir = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            }
            SaveFileDialog dlg = new SaveFileDialog();
            String ext = Export.TypeExt[(int)type];
            String filter = ext + " files(*" + ext + ")|*" + ext;
            dlg.FileName = "";
            dlg.InitialDirectory = dir;
            dlg.Filter = Export.Filter;
            dlg.FilterIndex = (int)type + 1;
            dlg.Title = dialog_title;
            dlg.RestoreDirectory = true;
            dlg.OverwritePrompt = true;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    type = (Export.Type)dlg.FilterIndex;
                    exec_save(type, dlg.FileName);
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return false;
        }
    }
}
