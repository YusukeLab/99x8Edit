using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace _99x8Edit 
{
    // Main window
    public partial class MainWindow : Form
    {
        private Machine dataSource;
        private PCGEditor PCGWin;
        private Map mapWin;
        private Sprites spriteWin;
        private Peek peekWin;
        private About aboutWin;
        private String currentFile = @"";
        public MainWindow()
        {
            InitializeComponent();
        }
        public bool UndoEnable
        {
            set
            {
                btnUndo.Enabled = value;
            }
        }
        public bool RedoEnable
        {
            set
            {
                btnRedo.Enabled = value;
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            // Initialize VDP settings
            dataSource = new Machine();
            dataSource.Initialize();
            Stream s = new MemoryStream(Properties.Resources._default);
            try
            {
                dataSource.LoadAllSettings(new BinaryReader(s));    // Init by resource
            }
            finally
            {
                s.Close();
            }
            // Undo/Redo            
            MementoCaretaker.Instance.Initialize(this, dataSource);
            // Export menu
            foreach (String item in Export.TypeList)
            {
                comboExportType.Items.Add(item);   // Code it since items may be expanded
            }
            comboExportType.SelectedIndex = 0;
            // Editors
            PCGWin = new PCGEditor(dataSource, this);
            mapWin = new Map(dataSource, this);
            spriteWin = new Sprites(dataSource, this);
            // Check drag and drop of files
            String[] args = System.Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                // See the last args since many files may have been dropped
                String dnd_path = args[args.Length - 1];
                this.LoadFile(dnd_path);
            }
            // Open PCG editor as default
            PCGWin.Show();
        }
        //----------------------------------------------------------------------
        // For other forms and internal use
        public void Undo()
        {
            // Undo
            MementoCaretaker.Instance.Undo();
            // Update UI
            PCGWin.ChangeOccuredByHost();
            mapWin.ChangeOccuredByHost();
            spriteWin.ChangeOccuredByHost();
        }
        public void Redo()
        {
            // Redo
            MementoCaretaker.Instance.Redo();
            // Update UI
            PCGWin.ChangeOccuredByHost();
            mapWin.ChangeOccuredByHost();
            spriteWin.ChangeOccuredByHost();
        }
        //----------------------------------------------------------------------
        // Controls
        private void btnPCGWin_Click(object sender, EventArgs e)
        {
            if (PCGWin.IsDisposed)
            {
                PCGWin = new PCGEditor(dataSource, this);
            }
            if (PCGWin.Visible)
            {
                PCGWin.Hide();
            }
            else
            {
                PCGWin.Show();
                PCGWin.BringToFront();
            }
        }
        private void btnMapWin_MouseClick(object sender, MouseEventArgs e)
        {
            if (mapWin.IsDisposed)
            {
                mapWin = new Map(dataSource, this);
            }
            if (mapWin.Visible)
            {
                mapWin.Hide();
            }
            else
            {
                mapWin.Show();
                mapWin.BringToFront();
            }
        }
        private void btnSpritesWin_Click(object sender, EventArgs e)
        {
            if (spriteWin.IsDisposed)
            {
                spriteWin = new Sprites(dataSource, this);
            }
            if (spriteWin.Visible)
            {
                spriteWin.Hide();
            }
            else
            {
                spriteWin.Show();
                spriteWin.BringToFront();
            }
        }
        private void btnSavePCG_Click(object sender, EventArgs e)
        {
            String dir = Path.GetDirectoryName(currentFile);
            if (dir == null)
            {
                String target = this.SaveDialog(dir);
                if (target != null)
                {
                    this.SaveFile(target);      // Save and update current file
                }
            }
            else
            {
                this.SaveFile(currentFile);
            }
        }
        private void btnSaveAs_Click(object sender, EventArgs e)
        {
            String dir = Path.GetDirectoryName(currentFile);
            if (dir == null)
            {
                dir = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            }
            String target = this.SaveDialog(dir);
            if (target != null)
            {
                this.SaveFile(target);
            }
        }
        private void btnLoadPCG_Click(object sender, EventArgs e)
        {
            String dir = Path.GetDirectoryName(currentFile);
            if (dir == null)
            {
                dir = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            }
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = dir;
            dlg.Filter = "VDP File(*.vdp)|*.vdp";
            dlg.FilterIndex = 1;
            dlg.Title = "Load settings";
            dlg.RestoreDirectory = true;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                this.LoadFile(dlg.FileName);
            }
        }
        private void btnPCGExport_Click(object sender, EventArgs e)
        {
            String dir = Path.GetDirectoryName(currentFile);
            if (dir == null)
            {
                dir = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            }
            String ext = Export.TypeExt[comboExportType.SelectedIndex];
            SaveFileDialog dlg = new SaveFileDialog();
            String filter = ext + " files(*" + ext + ")|*" + ext;
            dlg.FileName = "";
            dlg.InitialDirectory = dir;
            dlg.Filter = filter;
            dlg.FilterIndex = 1;
            dlg.Title = "Save to";
            dlg.RestoreDirectory = true;
            dlg.OverwritePrompt = true;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    dataSource.ExportPCG((Export.Type)comboExportType.SelectedIndex, dlg.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void btnExportMap_Click(object sender, EventArgs e)
        {
            String dir = Path.GetDirectoryName(currentFile);
            if (dir == null)
            {
                dir = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            }
            String ext = Export.TypeExt[comboExportType.SelectedIndex];
            SaveFileDialog dlg = new SaveFileDialog();
            String filter = ext + " files(*" + ext + ")|*" + ext;
            dlg.FileName = "";
            dlg.InitialDirectory = dir;
            dlg.Filter = filter;
            dlg.FilterIndex = 1;
            dlg.Title = "Save to";
            dlg.RestoreDirectory = true;
            dlg.OverwritePrompt = true;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    dataSource.ExportMap((Export.Type)comboExportType.SelectedIndex, dlg.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void btnExportSprites_Click(object sender, EventArgs e)
        {
            String dir = Path.GetDirectoryName(currentFile);
            if (dir == null)
            {
                dir = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            }
            String ext = Export.TypeExt[comboExportType.SelectedIndex];
            SaveFileDialog dlg = new SaveFileDialog();
            String filter = ext + " files(*" + ext + ")|*" + ext;
            dlg.FileName = "";
            dlg.InitialDirectory = dir;
            dlg.Filter = filter;
            dlg.FilterIndex = 1;
            dlg.Title = "Save to";
            dlg.RestoreDirectory = true;
            dlg.OverwritePrompt = true;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    dataSource.ExportSprites((Export.Type)comboExportType.SelectedIndex, dlg.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void btnUndo_Click(object sender, EventArgs e)
        {
            this.Undo();
        }
        private void btnRedo_Click(object sender, EventArgs e)
        {
            this.Redo();
        }
        String peekPath = "";
        private void btnPeek_Click(object sender, EventArgs e)
        {
            if (peekWin != null)
            {
                if (peekWin.IsDisposed == false)
                {
                    if (peekWin.Visible)
                    {
                        peekWin.Hide();
                    }
                    else
                    {
                        peekWin.Show();
                        peekWin.BringToFront();
                    }
                    return;
                }
            }
            // Create window
            String dir = Path.GetDirectoryName(peekPath);
            if (dir == null)
            {
                dir = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            }
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = dir;
            dlg.Filter = "Rom(*.rom)|*.rom|All files(*.*)|*.*";
            dlg.FilterIndex = 1;
            dlg.Title = "Open rom or other file to peek in";
            dlg.RestoreDirectory = true;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                peekWin = new Peek(dlg.FileName);
                peekWin.Show();
                peekPath = dlg.FileName;
            }
        }
        private void btnAbout_Click(object sender, EventArgs e)
        {
            if (aboutWin != null)
            {
                if (aboutWin.IsDisposed == false)
                {
                    aboutWin.Show();
                    aboutWin.BringToFront();
                    return;
                }
            }
            aboutWin = new About();
            aboutWin.Show();
        }
        //----------------------------------------------------------------------
        // Utilities
        private void SaveFile(String path)
        {
            // Save and update current file path if OK
            BinaryWriter br = new BinaryWriter(new FileStream(path, FileMode.Create));
            try
            {
                dataSource.SaveAllSettings(br);
                currentFile = path;
                PCGWin.CurrentFile = currentFile;
                spriteWin.CurrentFile = currentFile;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                currentFile = "";
                PCGWin.CurrentFile = currentFile;
                spriteWin.CurrentFile = currentFile;
            }
            finally
            {
                br.Close();
            }
        }
        private String SaveDialog(String dir)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "";
            dlg.InitialDirectory = dir;
            dlg.Filter = "VDP File(*.vdp)|*.vdp";
            dlg.FilterIndex = 1;
            dlg.Title = "Save settings";
            dlg.RestoreDirectory = true;
            dlg.OverwritePrompt = true;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                return dlg.FileName;
            }
            return null;
        }
        private void LoadFile(String path)
        {
            BinaryReader br = new BinaryReader(new FileStream(path, FileMode.Open));
            try
            {
                dataSource.LoadAllSettings(br);
                currentFile = path;
                PCGWin.CurrentFile = currentFile;
                spriteWin.CurrentFile = currentFile;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                currentFile = "";
                PCGWin.CurrentFile = "";
                spriteWin.CurrentFile = "";
            }
            finally
            {
                br.Close();
            }
            // Update UI
            PCGWin.ChangeOccuredByHost();
            mapWin.ChangeOccuredByHost();
            spriteWin.ChangeOccuredByHost();
            // Clear mementos
            MementoCaretaker.Instance.Clear();
        }
    }
}
