
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
            this.btnExportSprites = new System.Windows.Forms.Button();
            this.btnSpritesWin = new System.Windows.Forms.Button();
            this.toolTipMain = new System.Windows.Forms.ToolTip(this.components);
            this.btnMapWin = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnPCGExport = new System.Windows.Forms.Button();
            this.btnExportMap = new System.Windows.Forms.Button();
            this.btnPCGWin = new System.Windows.Forms.Button();
            this.btnUndo = new System.Windows.Forms.Button();
            this.btnRedo = new System.Windows.Forms.Button();
            this.btnPeek = new System.Windows.Forms.Button();
            this.btnAbout = new System.Windows.Forms.Button();
            this.btnSaveAs = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnExportSprites
            // 
            this.btnExportSprites.Font = new System.Drawing.Font("Yu Gothic UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.btnExportSprites.Location = new System.Drawing.Point(68, 127);
            this.btnExportSprites.Name = "btnExportSprites";
            this.btnExportSprites.Size = new System.Drawing.Size(94, 29);
            this.btnExportSprites.TabIndex = 18;
            this.btnExportSprites.Text = "Export";
            this.toolTipMain.SetToolTip(this.btnExportSprites, "Export Sprites");
            this.btnExportSprites.UseVisualStyleBackColor = true;
            this.btnExportSprites.Click += new System.EventHandler(this.ExportSprite);
            // 
            // btnSpritesWin
            // 
            this.btnSpritesWin.BackColor = System.Drawing.Color.White;
            this.btnSpritesWin.BackgroundImage = global::_99x8Edit.Properties.Resources.sprite;
            this.btnSpritesWin.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnSpritesWin.Location = new System.Drawing.Point(12, 118);
            this.btnSpritesWin.Name = "btnSpritesWin";
            this.btnSpritesWin.Size = new System.Drawing.Size(48, 48);
            this.btnSpritesWin.TabIndex = 13;
            this.toolTipMain.SetToolTip(this.btnSpritesWin, "Edit Sprites");
            this.btnSpritesWin.UseVisualStyleBackColor = false;
            this.btnSpritesWin.Click += new System.EventHandler(this.btnSpritesWin_Click);
            // 
            // toolTipMain
            // 
            this.toolTipMain.AutomaticDelay = 0;
            // 
            // btnMapWin
            // 
            this.btnMapWin.BackColor = System.Drawing.Color.White;
            this.btnMapWin.BackgroundImage = global::_99x8Edit.Properties.Resources.maps;
            this.btnMapWin.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnMapWin.Location = new System.Drawing.Point(12, 64);
            this.btnMapWin.Name = "btnMapWin";
            this.btnMapWin.Size = new System.Drawing.Size(48, 48);
            this.btnMapWin.TabIndex = 5;
            this.toolTipMain.SetToolTip(this.btnMapWin, "Edit Map");
            this.btnMapWin.UseVisualStyleBackColor = false;
            this.btnMapWin.MouseClick += new System.Windows.Forms.MouseEventHandler(this.btnMapWin_MouseClick);
            // 
            // btnLoad
            // 
            this.btnLoad.BackColor = System.Drawing.Color.White;
            this.btnLoad.BackgroundImage = global::_99x8Edit.Properties.Resources.open;
            this.btnLoad.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnLoad.Location = new System.Drawing.Point(10, 188);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(48, 48);
            this.btnLoad.TabIndex = 8;
            this.toolTipMain.SetToolTip(this.btnLoad, "Open");
            this.btnLoad.UseVisualStyleBackColor = false;
            this.btnLoad.Click += new System.EventHandler(this.LoadProject);
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.White;
            this.btnSave.BackgroundImage = global::_99x8Edit.Properties.Resources.save;
            this.btnSave.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnSave.Location = new System.Drawing.Point(64, 188);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(48, 48);
            this.btnSave.TabIndex = 9;
            this.toolTipMain.SetToolTip(this.btnSave, "Save");
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.SaveProject);
            // 
            // btnPCGExport
            // 
            this.btnPCGExport.Font = new System.Drawing.Font("Yu Gothic UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.btnPCGExport.Location = new System.Drawing.Point(68, 22);
            this.btnPCGExport.Name = "btnPCGExport";
            this.btnPCGExport.Size = new System.Drawing.Size(94, 29);
            this.btnPCGExport.TabIndex = 10;
            this.btnPCGExport.Text = "Export";
            this.toolTipMain.SetToolTip(this.btnPCGExport, "Export PCG and Sandbox");
            this.btnPCGExport.UseVisualStyleBackColor = true;
            this.btnPCGExport.Click += new System.EventHandler(this.ExportPCG);
            // 
            // btnExportMap
            // 
            this.btnExportMap.Font = new System.Drawing.Font("Yu Gothic UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.btnExportMap.Location = new System.Drawing.Point(68, 74);
            this.btnExportMap.Name = "btnExportMap";
            this.btnExportMap.Size = new System.Drawing.Size(94, 29);
            this.btnExportMap.TabIndex = 12;
            this.btnExportMap.Text = "Export";
            this.toolTipMain.SetToolTip(this.btnExportMap, "Export Map");
            this.btnExportMap.UseVisualStyleBackColor = true;
            this.btnExportMap.Click += new System.EventHandler(this.ExportMap);
            // 
            // btnPCGWin
            // 
            this.btnPCGWin.BackColor = System.Drawing.Color.White;
            this.btnPCGWin.BackgroundImage = global::_99x8Edit.Properties.Resources.pcg;
            this.btnPCGWin.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnPCGWin.Location = new System.Drawing.Point(12, 10);
            this.btnPCGWin.Name = "btnPCGWin";
            this.btnPCGWin.Size = new System.Drawing.Size(48, 48);
            this.btnPCGWin.TabIndex = 13;
            this.toolTipMain.SetToolTip(this.btnPCGWin, "Edit PCG");
            this.btnPCGWin.UseVisualStyleBackColor = false;
            this.btnPCGWin.Click += new System.EventHandler(this.btnPCGWin_Click);
            // 
            // btnUndo
            // 
            this.btnUndo.BackColor = System.Drawing.Color.White;
            this.btnUndo.BackgroundImage = global::_99x8Edit.Properties.Resources.undo;
            this.btnUndo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnUndo.Enabled = false;
            this.btnUndo.Location = new System.Drawing.Point(35, 242);
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.Size = new System.Drawing.Size(48, 48);
            this.btnUndo.TabIndex = 19;
            this.toolTipMain.SetToolTip(this.btnUndo, "Undo");
            this.btnUndo.UseVisualStyleBackColor = false;
            this.btnUndo.Click += new System.EventHandler(this.btnUndo_Click);
            // 
            // btnRedo
            // 
            this.btnRedo.BackColor = System.Drawing.Color.White;
            this.btnRedo.BackgroundImage = global::_99x8Edit.Properties.Resources.redo;
            this.btnRedo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnRedo.Enabled = false;
            this.btnRedo.Location = new System.Drawing.Point(90, 242);
            this.btnRedo.Name = "btnRedo";
            this.btnRedo.Size = new System.Drawing.Size(48, 48);
            this.btnRedo.TabIndex = 20;
            this.toolTipMain.SetToolTip(this.btnRedo, "Redo");
            this.btnRedo.UseVisualStyleBackColor = false;
            this.btnRedo.Click += new System.EventHandler(this.btnRedo_Click);
            // 
            // btnPeek
            // 
            this.btnPeek.BackColor = System.Drawing.Color.White;
            this.btnPeek.BackgroundImage = global::_99x8Edit.Properties.Resources.peek;
            this.btnPeek.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnPeek.Location = new System.Drawing.Point(35, 296);
            this.btnPeek.Name = "btnPeek";
            this.btnPeek.Size = new System.Drawing.Size(48, 48);
            this.btnPeek.TabIndex = 21;
            this.toolTipMain.SetToolTip(this.btnPeek, "Peek binaries");
            this.btnPeek.UseVisualStyleBackColor = false;
            this.btnPeek.Click += new System.EventHandler(this.btnPeek_Click);
            // 
            // btnAbout
            // 
            this.btnAbout.BackColor = System.Drawing.Color.White;
            this.btnAbout.BackgroundImage = global::_99x8Edit.Properties.Resources.info;
            this.btnAbout.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnAbout.Location = new System.Drawing.Point(90, 296);
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.Size = new System.Drawing.Size(48, 48);
            this.btnAbout.TabIndex = 22;
            this.toolTipMain.SetToolTip(this.btnAbout, "About");
            this.btnAbout.UseVisualStyleBackColor = false;
            this.btnAbout.Click += new System.EventHandler(this.btnAbout_Click);
            // 
            // btnSaveAs
            // 
            this.btnSaveAs.BackColor = System.Drawing.Color.White;
            this.btnSaveAs.BackgroundImage = global::_99x8Edit.Properties.Resources.saveas;
            this.btnSaveAs.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnSaveAs.Location = new System.Drawing.Point(118, 188);
            this.btnSaveAs.Name = "btnSaveAs";
            this.btnSaveAs.Size = new System.Drawing.Size(48, 48);
            this.btnSaveAs.TabIndex = 23;
            this.toolTipMain.SetToolTip(this.btnSaveAs, "Save As");
            this.btnSaveAs.UseVisualStyleBackColor = false;
            this.btnSaveAs.Click += new System.EventHandler(this.SaveAsProject);
            // 
            // MainWindow
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(175, 352);
            this.Controls.Add(this.btnSaveAs);
            this.Controls.Add(this.btnAbout);
            this.Controls.Add(this.btnPeek);
            this.Controls.Add(this.btnRedo);
            this.Controls.Add(this.btnUndo);
            this.Controls.Add(this.btnExportSprites);
            this.Controls.Add(this.btnPCGWin);
            this.Controls.Add(this.btnExportMap);
            this.Controls.Add(this.btnSpritesWin);
            this.Controls.Add(this.btnPCGExport);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnMapWin);
            this.Controls.Add(this.btnLoad);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainWindow";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "99x8Edit";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnExportSprites;
        private System.Windows.Forms.Button btnSpritesWin;
        private System.Windows.Forms.ToolTip toolTipMain;
        private System.Windows.Forms.Button btnMapWin;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnPCGExport;
        private System.Windows.Forms.Button btnExportMap;
        private System.Windows.Forms.Button btnPCGWin;
        private System.Windows.Forms.Button btnUndo;
        private System.Windows.Forms.Button btnRedo;
        private System.Windows.Forms.Button btnPeek;
        private System.Windows.Forms.Button btnAbout;
        private System.Windows.Forms.Button btnSaveAs;
        private System.Windows.Forms.Button btnSave;
    }
}

