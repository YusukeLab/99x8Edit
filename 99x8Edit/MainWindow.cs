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
        private String peekPath = "";
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
                BinaryReader br = new BinaryReader(new FileStream(dnd_path, FileMode.Open));
                try
                {
                    dataSource.LoadAllSettings(br);
                    currentFile = dnd_path;
                    PCGWin.CurrentFile = dnd_path;
                    spriteWin.CurrentFile = dnd_path;
                    mapWin.CurrentFile = dnd_path;
                    PCGWin.ChangeOccuredByHost();
                    mapWin.ChangeOccuredByHost();
                    spriteWin.ChangeOccuredByHost();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    currentFile = "";
                }
                finally
                {
                    br.Close();
                }
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
        private void btnSave_Click(object sender, EventArgs e)
        {
            string saved_filename = "";
            if (Utility.SaveDialogAndSave(currentFile,
                                          "VDP File(*.vdp)|*.vdp",
                                          "Save settings",
                                          dataSource.SaveAllSettings,
                                          false,    // overwrite
                                          out saved_filename))
            {
                currentFile = saved_filename;
                PCGWin.CurrentFile = saved_filename;
                mapWin.CurrentFile = saved_filename;
                spriteWin.CurrentFile = saved_filename;
            }
        }
        private void btnSaveAs_Click(object sender, EventArgs e)
        {
            string saved_filename = "";
            if (Utility.SaveDialogAndSave(currentFile,
                                          "VDP File(*.vdp)|*.vdp",
                                          "Save settings",
                                          dataSource.SaveAllSettings,
                                          true,     // save as
                                          out saved_filename))
            {
                currentFile = saved_filename;
                PCGWin.CurrentFile = saved_filename;
                mapWin.CurrentFile = saved_filename;
                spriteWin.CurrentFile = saved_filename;
            }
        }
        private void btnLoad_Click(object sender, EventArgs e)
        {
            String loaded_filename;
            if (Utility.LoadDialogAndLoad(currentFile,
                                          "VDP File(*.vdp)|*.vdp",
                                          "Load settings",
                                          dataSource.LoadAllSettings,
                                          false,       // Won't push memento
                                          out loaded_filename))
            {
                currentFile = loaded_filename;
                PCGWin.CurrentFile = loaded_filename;
                spriteWin.CurrentFile = loaded_filename;
                mapWin.CurrentFile = loaded_filename;
                // Update UI
                PCGWin.ChangeOccuredByHost();
                mapWin.ChangeOccuredByHost();
                spriteWin.ChangeOccuredByHost();
                // Clear mementos
                MementoCaretaker.Instance.Clear();
            }
        }
        private void btnPCGExport_Click(object sender, EventArgs e)
        {
            Utility.ExportDialogAndExport(currentFile,
                                          "Export PCG data to",
                                          (Export.Type)comboExportType.SelectedIndex,
                                          dataSource.ExportPCG);
        }
        private void btnExportMap_Click(object sender, EventArgs e)
        {
            Utility.ExportDialogAndExport(currentFile,
                                          "Export map data to",
                                          (Export.Type)comboExportType.SelectedIndex,
                                          dataSource.ExportMap);
        }
        private void btnExportSprites_Click(object sender, EventArgs e)
        {
            Utility.ExportDialogAndExport(currentFile,
                                          "Export sprite data to",
                                          (Export.Type)comboExportType.SelectedIndex,
                                          dataSource.ExportSprites);
        }
        private void btnUndo_Click(object sender, EventArgs e)
        {
            this.Undo();
        }
        private void btnRedo_Click(object sender, EventArgs e)
        {
            this.Redo();
        }
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
    }
}
