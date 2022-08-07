﻿
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
            this.comboExportType = new System.Windows.Forms.ComboBox();
            this.toolTipMain = new System.Windows.Forms.ToolTip(this.components);
            this.btnMapWin = new System.Windows.Forms.Button();
            this.btnLoadPCG = new System.Windows.Forms.Button();
            this.btnSavePCG = new System.Windows.Forms.Button();
            this.contextSave = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripSave = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.btnPCGExport = new System.Windows.Forms.Button();
            this.btnExportMap = new System.Windows.Forms.Button();
            this.btnPCGWin = new System.Windows.Forms.Button();
            this.btnUndo = new System.Windows.Forms.Button();
            this.btnRedo = new System.Windows.Forms.Button();
            this.btnPeek = new System.Windows.Forms.Button();
            this.btnAbout = new System.Windows.Forms.Button();
            this.contextSave.SuspendLayout();
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
            this.btnExportSprites.Click += new System.EventHandler(this.btnExportSprites_Click);
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
            // comboExportType
            // 
            this.comboExportType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboExportType.Font = new System.Drawing.Font("Yu Gothic UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.comboExportType.FormattingEnabled = true;
            this.comboExportType.Location = new System.Drawing.Point(21, 177);
            this.comboExportType.Name = "comboExportType";
            this.comboExportType.Size = new System.Drawing.Size(133, 28);
            this.comboExportType.TabIndex = 7;
            this.toolTipMain.SetToolTip(this.comboExportType, "Export Type");
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
            // btnLoadPCG
            // 
            this.btnLoadPCG.BackColor = System.Drawing.Color.White;
            this.btnLoadPCG.BackgroundImage = global::_99x8Edit.Properties.Resources.open;
            this.btnLoadPCG.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnLoadPCG.Location = new System.Drawing.Point(8, 236);
            this.btnLoadPCG.Name = "btnLoadPCG";
            this.btnLoadPCG.Size = new System.Drawing.Size(48, 48);
            this.btnLoadPCG.TabIndex = 8;
            this.toolTipMain.SetToolTip(this.btnLoadPCG, "Open All");
            this.btnLoadPCG.UseVisualStyleBackColor = false;
            this.btnLoadPCG.Click += new System.EventHandler(this.btnLoadPCG_Click);
            // 
            // btnSavePCG
            // 
            this.btnSavePCG.BackColor = System.Drawing.Color.White;
            this.btnSavePCG.BackgroundImage = global::_99x8Edit.Properties.Resources.save;
            this.btnSavePCG.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnSavePCG.ContextMenuStrip = this.contextSave;
            this.btnSavePCG.Location = new System.Drawing.Point(62, 236);
            this.btnSavePCG.Name = "btnSavePCG";
            this.btnSavePCG.Size = new System.Drawing.Size(48, 48);
            this.btnSavePCG.TabIndex = 9;
            this.toolTipMain.SetToolTip(this.btnSavePCG, "Save All");
            this.btnSavePCG.UseVisualStyleBackColor = false;
            this.btnSavePCG.Click += new System.EventHandler(this.btnSavePCG_Click);
            // 
            // contextSave
            // 
            this.contextSave.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextSave.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSave,
            this.toolStripSaveAs});
            this.contextSave.Name = "contextSave";
            this.contextSave.Size = new System.Drawing.Size(220, 52);
            // 
            // toolStripSave
            // 
            this.toolStripSave.Name = "toolStripSave";
            this.toolStripSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.toolStripSave.Size = new System.Drawing.Size(219, 24);
            this.toolStripSave.Text = "Save";
            // 
            // toolStripSaveAs
            // 
            this.toolStripSaveAs.Name = "toolStripSaveAs";
            this.toolStripSaveAs.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.toolStripSaveAs.Size = new System.Drawing.Size(219, 24);
            this.toolStripSaveAs.Text = "Save As";
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
            this.btnPCGExport.Click += new System.EventHandler(this.btnPCGExport_Click);
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
            this.btnExportMap.Click += new System.EventHandler(this.btnExportMap_Click);
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
            this.btnUndo.Location = new System.Drawing.Point(7, 290);
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
            this.btnRedo.Location = new System.Drawing.Point(62, 290);
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
            this.btnPeek.Location = new System.Drawing.Point(116, 236);
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
            this.btnAbout.Location = new System.Drawing.Point(116, 290);
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.Size = new System.Drawing.Size(48, 48);
            this.btnAbout.TabIndex = 22;
            this.toolTipMain.SetToolTip(this.btnAbout, "About");
            this.btnAbout.UseVisualStyleBackColor = false;
            this.btnAbout.Click += new System.EventHandler(this.btnAbout_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(175, 353);
            this.Controls.Add(this.btnAbout);
            this.Controls.Add(this.btnPeek);
            this.Controls.Add(this.btnRedo);
            this.Controls.Add(this.btnUndo);
            this.Controls.Add(this.btnExportSprites);
            this.Controls.Add(this.btnPCGWin);
            this.Controls.Add(this.btnExportMap);
            this.Controls.Add(this.btnSpritesWin);
            this.Controls.Add(this.comboExportType);
            this.Controls.Add(this.btnPCGExport);
            this.Controls.Add(this.btnSavePCG);
            this.Controls.Add(this.btnMapWin);
            this.Controls.Add(this.btnLoadPCG);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainWindow";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "99x8Edit";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.contextSave.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ComboBox comboExportType;
        private System.Windows.Forms.Button btnExportSprites;
        private System.Windows.Forms.Button btnSpritesWin;
        private System.Windows.Forms.ToolTip toolTipMain;
        private System.Windows.Forms.Button btnMapWin;
        private System.Windows.Forms.Button btnLoadPCG;
        private System.Windows.Forms.Button btnSavePCG;
        private System.Windows.Forms.Button btnPCGExport;
        private System.Windows.Forms.Button btnExportMap;
        private System.Windows.Forms.Button btnPCGWin;
        private System.Windows.Forms.Button btnUndo;
        private System.Windows.Forms.Button btnRedo;
        private System.Windows.Forms.Button btnPeek;
        private System.Windows.Forms.Button btnAbout;
        private System.Windows.Forms.ContextMenuStrip contextSave;
        private System.Windows.Forms.ToolStripMenuItem toolStripSave;
        private System.Windows.Forms.ToolStripMenuItem toolStripSaveAs;
    }
}
