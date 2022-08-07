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
    public partial class MainWindow : Form
    {
        private Machine dataSource;
        private PCGEditor PCGWin;
        private Map MapWin;
        private Sprites SpriteWin;
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
            dataSource.LoadAllSettings(new BinaryReader(s));
            // Undo/Redo            
            MementoCaretaker.Instance.Initialize(this, dataSource);
            // Export menu
            foreach (KeyValuePair<String, String> p in dataSource.exportTypes)
            {
                comboExportType.Items.Add(p.Key);   // Code it since items may be expanded
            }
            comboExportType.SelectedIndex = 0;
            // Editors
            PCGWin = new PCGEditor(dataSource);
            MapWin = new Map(dataSource);
            SpriteWin = new Sprites(dataSource);
            PCGWin.Show();
        }
        private void btnPCGWin_Click(object sender, EventArgs e)
        {
            if (PCGWin.IsDisposed)
            {
                PCGWin = new PCGEditor(dataSource);
            }
            if (PCGWin.Visible)
            {
                PCGWin.Hide();
            }
            else
            {
                PCGWin.Show();
            }
        }
        private void btnMapWin_MouseClick(object sender, MouseEventArgs e)
        {
            if(MapWin.IsDisposed)
            {
                MapWin = new Map(dataSource);
            }
            if (MapWin.Visible)
            {
                MapWin.Hide();
            }
            else
            {
                MapWin.Show();
            }
        }
        private void btnSpritesWin_Click(object sender, EventArgs e)
        {
            if (SpriteWin.IsDisposed)
            {
                SpriteWin = new Sprites(dataSource);
            }
            if (SpriteWin.Visible)
            {
                SpriteWin.Hide();
            }
            else
            {
                SpriteWin.Show();
            }
        }
        private void btnSavePCG_Click(object sender, EventArgs e)
        {
            String dir = Path.GetDirectoryName(currentFile);
            String target = currentFile;
            if (dir == null)
            {
                dir = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
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
                    target = dlg.FileName;
                }
                else return;
            }
            BinaryWriter br = new BinaryWriter(new FileStream(target, FileMode.Create));
            try
            {
                dataSource.SaveAllSettings(br);
                currentFile = target;
                PCGWin.CurrentFile = currentFile;
                SpriteWin.CurrentFile = currentFile;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                currentFile = "";
                PCGWin.CurrentFile = currentFile;
                SpriteWin.CurrentFile = currentFile;
            }
            finally
            {
                br.Close();
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
                BinaryReader br = new BinaryReader(new FileStream(dlg.FileName, FileMode.Open));
                try
                {
                    dataSource.LoadAllSettings(br);
                    currentFile = dlg.FileName;
                    PCGWin.CurrentFile = currentFile;
                    SpriteWin.CurrentFile = currentFile;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    currentFile = "";
                    PCGWin.CurrentFile = currentFile;
                    SpriteWin.CurrentFile = currentFile;
                }
                finally
                {
                    br.Close();
                }
                // Update UI
                PCGWin.ChangeOccuredByHost();
                MapWin.ChangeOccuredByHost();
                SpriteWin.ChangeOccuredByHost();
                // Clear mementos
                MementoCaretaker.Instance.Clear();
            }
        }
        private void btnPCGExport_Click(object sender, EventArgs e)
        {
            String dir = Path.GetDirectoryName(currentFile);
            if (dir == null)
            {
                dir = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            }
            String ext = dataSource.exportTypes[comboExportType.Text];
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
                    dataSource.ExportPCG(comboExportType.SelectedIndex, dlg.FileName);
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
            String ext = dataSource.exportTypes[comboExportType.Text];
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
                    dataSource.ExportMap(comboExportType.SelectedIndex, dlg.FileName);
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
            String ext = dataSource.exportTypes[comboExportType.Text];
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
                    dataSource.ExportSprites(comboExportType.SelectedIndex, dlg.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void btnUndo_Click(object sender, EventArgs e)
        {
            // Undo
            MementoCaretaker.Instance.Undo();
            // Update UI
            PCGWin.ChangeOccuredByHost();
            MapWin.ChangeOccuredByHost();
            SpriteWin.ChangeOccuredByHost();
        }
        private void btnRedo_Click(object sender, EventArgs e)
        {
            // Redo
            MementoCaretaker.Instance.Redo();
            // Update UI
            PCGWin.ChangeOccuredByHost();
            MapWin.ChangeOccuredByHost();
            SpriteWin.ChangeOccuredByHost();
        }
    }
}
