using System;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace _99x8Edit 
{
    // Main window
    public partial class MainWindow : Form
    {
        private Machine _dataSource;
        private PCGEditor _PCGWin;
        private MapEditor _mapWin;
        private SpriteEditor _spriteWin;
        private PeekWindow _peekWin;
        private About _aboutWin;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void MainWindow_Load(object sender, EventArgs e)
        {
            // Initialize VDP settings
            _dataSource = new Machine();
            _dataSource.SetToDefault();
            // Check drag and drop of project file
            bool project_loaded = false;
            String[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                // Project file dropped
                String dnd_path = args.Last();
                using BinaryReader br = new BinaryReader(
                    new FileStream(dnd_path, FileMode.Open));
                try
                {
                    _dataSource.LoadAllSettings(br);
                    Config.Setting.ProjectFile = dnd_path;
                    project_loaded = true;
                }
                catch (Exception ex) when (!System.Diagnostics.Debugger.IsAttached)
                {
                    MessageBox.Show(ex.Message, "Error reading project file",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    _dataSource.LoadAllSettings(new BinaryReader(s));
                }
                catch
                {
                    // If failed, initialize by setting within the resource
                    Config.Setting.ProjectFile = "";
                    try
                    {
                        // Read from built in resource
                        using Stream s = new MemoryStream(Properties.Resources._default);
                        _dataSource.LoadAllSettings(new BinaryReader(s));
                    }
                    catch
                    {
                        // Everything failed
                        _dataSource.SetToDefault();
                    }
                }
            }
            // Undo/Redo            
            MementoCaretaker.Instance.SetCallback(MementoStateChanged);
            MementoCaretaker.Instance.AddTarget(_dataSource);  // One Machine class will be managed
            // Editors
            _PCGWin = new PCGEditor(_dataSource, this);
            _mapWin = new MapEditor(_dataSource, this);
            _spriteWin = new SpriteEditor(_dataSource, this);
            // Open PCG editor as default
            _PCGWin.Show();
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
            _PCGWin.ChangeOccuredByHost();
            _mapWin.ChangeOccuredByHost();
            _spriteWin.ChangeOccuredByHost();
        }
        public void Redo()
        {
            // Redo
            MementoCaretaker.Instance.Redo();
            // Update UI
            _PCGWin.ChangeOccuredByHost();
            _mapWin.ChangeOccuredByHost();
            _spriteWin.ChangeOccuredByHost();
        }
        internal void MementoStateChanged()
        {
            // For MementoCaretaker
            _btnUndo.Enabled = MementoCaretaker.Instance.UndoEnable;
            _btnRedo.Enabled = MementoCaretaker.Instance.RedoEnable;
        }
        //----------------------------------------------------------------------
        // Controls
        private void btnPCGWin_Click(object sender, EventArgs e)
        {
            if (_PCGWin.IsDisposed)
            {
                _PCGWin = new PCGEditor(_dataSource, this);
            }
            if (_PCGWin.Visible)
            {
                _PCGWin.Hide();
            }
            else
            {
                _PCGWin.Show();
                _PCGWin.BringToFront();
            }
        }
        private void btnMapWin_MouseClick(object sender, MouseEventArgs e)
        {
            if (_mapWin.IsDisposed)
            {
                _mapWin = new MapEditor(_dataSource, this);
            }
            if (_mapWin.Visible)
            {
                _mapWin.Hide();
            }
            else
            {
                _mapWin.Show();
                _mapWin.BringToFront();
            }
        }
        private void btnSpritesWin_Click(object sender, EventArgs e)
        {
            if (_spriteWin.IsDisposed)
            {
                _spriteWin = new SpriteEditor(_dataSource, this);
            }
            if (_spriteWin.Visible)
            {
                _spriteWin.Hide();
            }
            else
            {
                _spriteWin.Show();
                _spriteWin.BringToFront();
            }
        }
        internal void SaveProject(object sender, EventArgs e)
        {
            // Called from button and child window
            if (Utility.SaveDialogAndSave(Config.Setting.ProjectFile,
                                          "VDP File(*.vdp)|*.vdp",
                                          "Save settings",
                                          _dataSource.SaveAllSettings,
                                          false,    // overwrite
                                          out string saved_filename))
            {
                Config.Setting.ProjectFile = saved_filename;
            }
        }
        internal void SaveAsProject(object sender, EventArgs e)
        {
            // Called from button and child window
            if (Utility.SaveDialogAndSave(Config.Setting.ProjectFile,
                                          "VDP File(*.vdp)|*.vdp",
                                          "Save settings",
                                          _dataSource.SaveAllSettings,
                                          true,     // save as
                                          out string saved_filename))
            {
                Config.Setting.ProjectFile = saved_filename;
            }
        }
        internal void LoadProject(object sender, EventArgs e)
        {
            // Called from button and child window
            if (Utility.LoadDialogAndLoad(Config.Setting.ProjectFile,
                                          "VDP File(*.vdp)|*.vdp",
                                          "Load settings",
                                          _dataSource.LoadAllSettings,
                                          false,       // Won't push memento
                                          out string loaded_filename))
            {
                Config.Setting.ProjectFile = loaded_filename;
                // Update UI
                _PCGWin.ChangeOccuredByHost();
                _mapWin.ChangeOccuredByHost();
                _spriteWin.ChangeOccuredByHost();
                // Clear mementos
                MementoCaretaker.Instance.Clear();
            }
        }
        internal void ExportPCG(object sender, EventArgs e)
        {
            // Called from button and child window
            if(Utility.ExportDialogAndExport(Config.Setting.ExportDirectory,
                                            "Export PCG data to",
                                            Export.PCGTypeFilter,
                                            _dataSource.ExportPCG,
                                            out string exported_file))
            {
                Config.Setting.ExportDirectory = Path.GetDirectoryName(exported_file);
            }
        }
        internal void ExportMap(object sender, EventArgs e)
        {
            // Called from button and child window
            if(Utility.ExportDialogAndExport(Config.Setting.ExportDirectory,
                                            "Export map data to",
                                            Export.MapTypeFilter,
                                            _dataSource.ExportMap,
                                            out string exported_file))
            {
                Config.Setting.ExportDirectory = Path.GetDirectoryName(exported_file);
            }
        }
        internal void ExportSprite(object sender, EventArgs e)
        {
            // Called from button and child window
            if(Utility.ExportDialogAndExport(Config.Setting.ExportDirectory,
                                            "Export sprite data to",
                                            Export.SpriteTypeFilter,
                                            _dataSource.ExportSprites,
                                            out string exported_file))
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
            if (_peekWin != null)
            {
                if (_peekWin.IsDisposed == false)
                {
                    if (_peekWin.Visible)
                    {
                        _peekWin.Hide();
                    }
                    else
                    {
                        _peekWin.Show();
                        _peekWin.BringToFront();
                    }
                    return;
                }
            }
            // Create window
            string dir = Config.Setting.PeekDirectory;
            if (!Directory.Exists(dir))
            {
                dir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            }
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = dir;
            dlg.Filter = "Rom(*.rom)|*.rom|All files(*.*)|*.*";
            dlg.FilterIndex = 1;
            dlg.Title = "Open rom or other file to peek in";
            dlg.RestoreDirectory = true;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                Config.Setting.PeekDirectory = Path.GetDirectoryName(dlg.FileName);
                _peekWin = new PeekWindow(dlg.FileName);
                _peekWin.Show();
            }
        }
        private void btnAbout_Click(object sender, EventArgs e)
        {
            if (_aboutWin != null)
            {
                if (_aboutWin.IsDisposed == false)
                {
                    _aboutWin.Show();
                    _aboutWin.BringToFront();
                    return;
                }
            }
            _aboutWin = new About();
            _aboutWin.Show();
        }
    }
}
