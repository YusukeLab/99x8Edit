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
        private MapEditor mapWin;
        private SpriteEditor spriteWin;
        private PeekWindow peekWin;
        private About aboutWin;
        private String peekPath = "";
        public MainWindow()
        {
            InitializeComponent();
        }
        private void MainWindow_Load(object sender, EventArgs e)
        {
            // Initialize VDP settings
            dataSource = new Machine();
            // Check drag and drop of project file
            bool project_loaded = false;
            String[] args = System.Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                // Project file dropped
                String dnd_path = args[args.Length - 1];
                using BinaryReader br = new BinaryReader(new FileStream(dnd_path, FileMode.Open));
                try
                {
                    dataSource.LoadAllSettings(br);
                    Config.Setting.ProjectFile = dnd_path;
                    project_loaded = true;
                }
                catch (Exception ex) when (!System.Diagnostics.Debugger.IsAttached)
                {
                    MessageBox.Show(ex.Message, "Error reading project file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            // Load configuration
            Config.Load();
            if(!project_loaded)
            {
                try
                {
                    // Read the previous project file
                    using Stream s = new FileStream(Config.Setting.ProjectFile, FileMode.Open);
                    dataSource.LoadAllSettings(new BinaryReader(s));
                }
                catch
                {
                    // If failed, initialize by setting within the resource
                    Config.Setting.ProjectFile = "";
                    try
                    {
                        // Read from built in resource
                        using Stream s = new MemoryStream(Properties.Resources._default);
                        dataSource.LoadAllSettings(new BinaryReader(s));
                    }
                    catch
                    {
                        // Everything failed
                        dataSource.SetToDefault();
                    }
                }
            }
            // Undo/Redo            
            MementoCaretaker.Instance.SetCallback(MementoStateChanged);
            MementoCaretaker.Instance.AddTarget(dataSource);  // One Machine class will be managed
            // Editors
            PCGWin = new PCGEditor(dataSource, this);
            mapWin = new MapEditor(dataSource, this);
            spriteWin = new SpriteEditor(dataSource, this);
            // Open PCG editor as default
            PCGWin.Show();
        }
        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            Config.Save();
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
        internal void MementoStateChanged()
        {
            // For MementoCaretaker
            btnUndo.Enabled = MementoCaretaker.Instance.UndoEnable;
            btnRedo.Enabled = MementoCaretaker.Instance.RedoEnable;
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
                mapWin = new MapEditor(dataSource, this);
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
                spriteWin = new SpriteEditor(dataSource, this);
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
        internal void SaveProject(object sender, EventArgs e)
        {
            // Called from button and child window
            string saved_filename = "";
            if (Utility.SaveDialogAndSave(Config.Setting.ProjectFile,
                                          "VDP File(*.vdp)|*.vdp",
                                          "Save settings",
                                          dataSource.SaveAllSettings,
                                          false,    // overwrite
                                          out saved_filename))
            {
                Config.Setting.ProjectFile = saved_filename;
            }
        }
        internal void SaveAsProject(object sender, EventArgs e)
        {
            // Called from button and child window
            string saved_filename = "";
            if (Utility.SaveDialogAndSave(Config.Setting.ProjectFile,
                                          "VDP File(*.vdp)|*.vdp",
                                          "Save settings",
                                          dataSource.SaveAllSettings,
                                          true,     // save as
                                          out saved_filename))
            {
                Config.Setting.ProjectFile = saved_filename;
            }
        }
        internal void LoadProject(object sender, EventArgs e)
        {
            // Called from button and child window
            String loaded_filename;
            if (Utility.LoadDialogAndLoad(Config.Setting.ProjectFile,
                                          "VDP File(*.vdp)|*.vdp",
                                          "Load settings",
                                          dataSource.LoadAllSettings,
                                          false,       // Won't push memento
                                          out loaded_filename))
            {
                Config.Setting.ProjectFile = loaded_filename;
                // Update UI
                PCGWin.ChangeOccuredByHost();
                mapWin.ChangeOccuredByHost();
                spriteWin.ChangeOccuredByHost();
                // Clear mementos
                MementoCaretaker.Instance.Clear();
            }
        }
        internal void ExportPCG(object sender, EventArgs e)
        {
            // Called from button and child window
            string exported_file = "";
            if(Utility.ExportDialogAndExport(Config.Setting.ExportDirectory,
                                            "Export PCG data to",
                                            Export.PCGTypeFilter,
                                            dataSource.ExportPCG,
                                            out exported_file))
            {
                Config.Setting.ExportDirectory = Path.GetDirectoryName(exported_file);
            }
        }
        internal void ExportMap(object sender, EventArgs e)
        {
            // Called from button and child window
            string exported_file = "";
            if(Utility.ExportDialogAndExport(Config.Setting.ExportDirectory,
                                            "Export map data to",
                                            Export.MapTypeFilter,
                                            dataSource.ExportMap,
                                            out exported_file))
            {
                Config.Setting.ExportDirectory = Path.GetDirectoryName(exported_file);
            }
        }
        internal void ExportSprite(object sender, EventArgs e)
        {
            // Called from button and child window
            string exported_file = "";
            if(Utility.ExportDialogAndExport(Config.Setting.ExportDirectory,
                                            "Export sprite data to",
                                            Export.SpriteTypeFilter,
                                            dataSource.ExportSprites,
                                            out exported_file))
            {
                Config.Setting.ExportDirectory = Path.GetDirectoryName(exported_file);
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
            String dir = Path.GetDirectoryName(Config.Setting.PeekDirectory);
            dir ??= System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = dir;
            dlg.Filter = "Rom(*.rom)|*.rom|All files(*.*)|*.*";
            dlg.FilterIndex = 1;
            dlg.Title = "Open rom or other file to peek in";
            dlg.RestoreDirectory = true;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                Config.Setting.PeekDirectory = Path.GetDirectoryName(dlg.FileName);
                peekWin = new PeekWindow(dlg.FileName);
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
