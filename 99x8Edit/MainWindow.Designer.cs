
namespace _99x8Edit
{
    partial class MainWindow
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this._btnExportSprites = new System.Windows.Forms.Button();
            this._btnSpritesWin = new System.Windows.Forms.Button();
            this.toolTipMain = new System.Windows.Forms.ToolTip(this.components);
            this._btnMapWin = new System.Windows.Forms.Button();
            this._btnLoad = new System.Windows.Forms.Button();
            this._btnSave = new System.Windows.Forms.Button();
            this._btnPCGExport = new System.Windows.Forms.Button();
            this._btnExportMap = new System.Windows.Forms.Button();
            this._btnPCGWin = new System.Windows.Forms.Button();
            this._btnUndo = new System.Windows.Forms.Button();
            this._btnRedo = new System.Windows.Forms.Button();
            this._btnPeek = new System.Windows.Forms.Button();
            this._btnAbout = new System.Windows.Forms.Button();
            this._btnSaveAs = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _btnExportSprites
            // 
            this._btnExportSprites.Font = new System.Drawing.Font("Yu Gothic UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this._btnExportSprites.Location = new System.Drawing.Point(68, 127);
            this._btnExportSprites.Name = "_btnExportSprites";
            this._btnExportSprites.Size = new System.Drawing.Size(94, 29);
            this._btnExportSprites.TabIndex = 18;
            this._btnExportSprites.Text = "Export";
            this.toolTipMain.SetToolTip(this._btnExportSprites, "Export Sprites");
            this._btnExportSprites.UseVisualStyleBackColor = true;
            this._btnExportSprites.Click += new System.EventHandler(this.ExportSprite);
            // 
            // _btnSpritesWin
            // 
            this._btnSpritesWin.BackColor = System.Drawing.Color.White;
            this._btnSpritesWin.BackgroundImage = global::_99x8Edit.Properties.Resources.sprite;
            this._btnSpritesWin.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this._btnSpritesWin.Location = new System.Drawing.Point(12, 118);
            this._btnSpritesWin.Name = "_btnSpritesWin";
            this._btnSpritesWin.Size = new System.Drawing.Size(48, 48);
            this._btnSpritesWin.TabIndex = 13;
            this.toolTipMain.SetToolTip(this._btnSpritesWin, "Edit Sprites");
            this._btnSpritesWin.UseVisualStyleBackColor = false;
            this._btnSpritesWin.Click += new System.EventHandler(this.btnSpritesWin_Click);
            // 
            // toolTipMain
            // 
            this.toolTipMain.AutomaticDelay = 0;
            // 
            // _btnMapWin
            // 
            this._btnMapWin.BackColor = System.Drawing.Color.White;
            this._btnMapWin.BackgroundImage = global::_99x8Edit.Properties.Resources.maps;
            this._btnMapWin.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this._btnMapWin.Location = new System.Drawing.Point(12, 64);
            this._btnMapWin.Name = "_btnMapWin";
            this._btnMapWin.Size = new System.Drawing.Size(48, 48);
            this._btnMapWin.TabIndex = 5;
            this.toolTipMain.SetToolTip(this._btnMapWin, "Edit Map");
            this._btnMapWin.UseVisualStyleBackColor = false;
            this._btnMapWin.MouseClick += new System.Windows.Forms.MouseEventHandler(this.btnMapWin_MouseClick);
            // 
            // _btnLoad
            // 
            this._btnLoad.BackColor = System.Drawing.Color.White;
            this._btnLoad.BackgroundImage = global::_99x8Edit.Properties.Resources.open;
            this._btnLoad.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this._btnLoad.Location = new System.Drawing.Point(10, 188);
            this._btnLoad.Name = "_btnLoad";
            this._btnLoad.Size = new System.Drawing.Size(48, 48);
            this._btnLoad.TabIndex = 8;
            this.toolTipMain.SetToolTip(this._btnLoad, "Open");
            this._btnLoad.UseVisualStyleBackColor = false;
            this._btnLoad.Click += new System.EventHandler(this.LoadProject);
            // 
            // _btnSave
            // 
            this._btnSave.BackColor = System.Drawing.Color.White;
            this._btnSave.BackgroundImage = global::_99x8Edit.Properties.Resources.save;
            this._btnSave.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this._btnSave.Location = new System.Drawing.Point(64, 188);
            this._btnSave.Name = "_btnSave";
            this._btnSave.Size = new System.Drawing.Size(48, 48);
            this._btnSave.TabIndex = 9;
            this.toolTipMain.SetToolTip(this._btnSave, "Save");
            this._btnSave.UseVisualStyleBackColor = false;
            this._btnSave.Click += new System.EventHandler(this.SaveProject);
            // 
            // _btnPCGExport
            // 
            this._btnPCGExport.Font = new System.Drawing.Font("Yu Gothic UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this._btnPCGExport.Location = new System.Drawing.Point(68, 22);
            this._btnPCGExport.Name = "_btnPCGExport";
            this._btnPCGExport.Size = new System.Drawing.Size(94, 29);
            this._btnPCGExport.TabIndex = 10;
            this._btnPCGExport.Text = "Export";
            this.toolTipMain.SetToolTip(this._btnPCGExport, "Export PCG and Sandbox");
            this._btnPCGExport.UseVisualStyleBackColor = true;
            this._btnPCGExport.Click += new System.EventHandler(this.ExportPCG);
            // 
            // _btnExportMap
            // 
            this._btnExportMap.Font = new System.Drawing.Font("Yu Gothic UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this._btnExportMap.Location = new System.Drawing.Point(68, 74);
            this._btnExportMap.Name = "_btnExportMap";
            this._btnExportMap.Size = new System.Drawing.Size(94, 29);
            this._btnExportMap.TabIndex = 12;
            this._btnExportMap.Text = "Export";
            this.toolTipMain.SetToolTip(this._btnExportMap, "Export Map");
            this._btnExportMap.UseVisualStyleBackColor = true;
            this._btnExportMap.Click += new System.EventHandler(this.ExportMap);
            // 
            // _btnPCGWin
            // 
            this._btnPCGWin.BackColor = System.Drawing.Color.White;
            this._btnPCGWin.BackgroundImage = global::_99x8Edit.Properties.Resources.pcg;
            this._btnPCGWin.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this._btnPCGWin.Location = new System.Drawing.Point(12, 10);
            this._btnPCGWin.Name = "_btnPCGWin";
            this._btnPCGWin.Size = new System.Drawing.Size(48, 48);
            this._btnPCGWin.TabIndex = 13;
            this.toolTipMain.SetToolTip(this._btnPCGWin, "Edit PCG");
            this._btnPCGWin.UseVisualStyleBackColor = false;
            this._btnPCGWin.Click += new System.EventHandler(this.btnPCGWin_Click);
            // 
            // _btnUndo
            // 
            this._btnUndo.BackColor = System.Drawing.Color.White;
            this._btnUndo.BackgroundImage = global::_99x8Edit.Properties.Resources.undo;
            this._btnUndo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this._btnUndo.Enabled = false;
            this._btnUndo.Location = new System.Drawing.Point(35, 242);
            this._btnUndo.Name = "_btnUndo";
            this._btnUndo.Size = new System.Drawing.Size(48, 48);
            this._btnUndo.TabIndex = 19;
            this.toolTipMain.SetToolTip(this._btnUndo, "Undo");
            this._btnUndo.UseVisualStyleBackColor = false;
            this._btnUndo.Click += new System.EventHandler(this.btnUndo_Click);
            // 
            // _btnRedo
            // 
            this._btnRedo.BackColor = System.Drawing.Color.White;
            this._btnRedo.BackgroundImage = global::_99x8Edit.Properties.Resources.redo;
            this._btnRedo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this._btnRedo.Enabled = false;
            this._btnRedo.Location = new System.Drawing.Point(90, 242);
            this._btnRedo.Name = "_btnRedo";
            this._btnRedo.Size = new System.Drawing.Size(48, 48);
            this._btnRedo.TabIndex = 20;
            this.toolTipMain.SetToolTip(this._btnRedo, "Redo");
            this._btnRedo.UseVisualStyleBackColor = false;
            this._btnRedo.Click += new System.EventHandler(this.btnRedo_Click);
            // 
            // _btnPeek
            // 
            this._btnPeek.BackColor = System.Drawing.Color.White;
            this._btnPeek.BackgroundImage = global::_99x8Edit.Properties.Resources.peek;
            this._btnPeek.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this._btnPeek.Location = new System.Drawing.Point(35, 296);
            this._btnPeek.Name = "_btnPeek";
            this._btnPeek.Size = new System.Drawing.Size(48, 48);
            this._btnPeek.TabIndex = 21;
            this.toolTipMain.SetToolTip(this._btnPeek, "Peek binaries");
            this._btnPeek.UseVisualStyleBackColor = false;
            this._btnPeek.Click += new System.EventHandler(this.btnPeek_Click);
            // 
            // _btnAbout
            // 
            this._btnAbout.BackColor = System.Drawing.Color.White;
            this._btnAbout.BackgroundImage = global::_99x8Edit.Properties.Resources.info;
            this._btnAbout.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this._btnAbout.Location = new System.Drawing.Point(90, 296);
            this._btnAbout.Name = "_btnAbout";
            this._btnAbout.Size = new System.Drawing.Size(48, 48);
            this._btnAbout.TabIndex = 22;
            this.toolTipMain.SetToolTip(this._btnAbout, "About");
            this._btnAbout.UseVisualStyleBackColor = false;
            this._btnAbout.Click += new System.EventHandler(this.btnAbout_Click);
            // 
            // _btnSaveAs
            // 
            this._btnSaveAs.BackColor = System.Drawing.Color.White;
            this._btnSaveAs.BackgroundImage = global::_99x8Edit.Properties.Resources.saveas;
            this._btnSaveAs.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this._btnSaveAs.Location = new System.Drawing.Point(118, 188);
            this._btnSaveAs.Name = "_btnSaveAs";
            this._btnSaveAs.Size = new System.Drawing.Size(48, 48);
            this._btnSaveAs.TabIndex = 23;
            this.toolTipMain.SetToolTip(this._btnSaveAs, "Save As");
            this._btnSaveAs.UseVisualStyleBackColor = false;
            this._btnSaveAs.Click += new System.EventHandler(this.SaveAsProject);
            // 
            // MainWindow
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(175, 352);
            this.Controls.Add(this._btnSaveAs);
            this.Controls.Add(this._btnAbout);
            this.Controls.Add(this._btnPeek);
            this.Controls.Add(this._btnRedo);
            this.Controls.Add(this._btnUndo);
            this.Controls.Add(this._btnExportSprites);
            this.Controls.Add(this._btnPCGWin);
            this.Controls.Add(this._btnExportMap);
            this.Controls.Add(this._btnSpritesWin);
            this.Controls.Add(this._btnPCGExport);
            this.Controls.Add(this._btnSave);
            this.Controls.Add(this._btnMapWin);
            this.Controls.Add(this._btnLoad);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainWindow";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "99x8Edit";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing);
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button _btnExportSprites;
        private System.Windows.Forms.Button _btnSpritesWin;
        private System.Windows.Forms.ToolTip toolTipMain;
        private System.Windows.Forms.Button _btnMapWin;
        private System.Windows.Forms.Button _btnLoad;
        private System.Windows.Forms.Button _btnPCGExport;
        private System.Windows.Forms.Button _btnExportMap;
        private System.Windows.Forms.Button _btnPCGWin;
        private System.Windows.Forms.Button _btnUndo;
        private System.Windows.Forms.Button _btnRedo;
        private System.Windows.Forms.Button _btnPeek;
        private System.Windows.Forms.Button _btnAbout;
        private System.Windows.Forms.Button _btnSaveAs;
        private System.Windows.Forms.Button _btnSave;
    }
}

