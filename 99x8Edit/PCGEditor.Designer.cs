
namespace _99x8Edit
{
    partial class PCGEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.viewPCGEdit = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.contextPCGList = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripPCGCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripPCGPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripPCGDel = new System.Windows.Forms.ToolStripMenuItem();
            this.label3 = new System.Windows.Forms.Label();
            this.viewColorL = new System.Windows.Forms.PictureBox();
            this.viewColorR = new System.Windows.Forms.PictureBox();
            this.label4 = new System.Windows.Forms.Label();
            this.viewPalette = new System.Windows.Forms.PictureBox();
            this.btnOpenPalette = new System.Windows.Forms.Button();
            this.checkTMS = new System.Windows.Forms.CheckBox();
            this.btnSavePalette = new System.Windows.Forms.Button();
            this.toolTipPCG = new System.Windows.Forms.ToolTip(this.components);
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.viewSandbox = new System.Windows.Forms.PictureBox();
            this.panelPCG = new System.Windows.Forms.FlowLayoutPanel();
            this.viewPCG = new System.Windows.Forms.PictureBox();
            this.contextSandbox = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripSandboxCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSandboxPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSandboxDel = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSandboxPaint = new System.Windows.Forms.ToolStripMenuItem();
            this.panelSandbox = new System.Windows.Forms.FlowLayoutPanel();
            this.contextEditor = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripEditorCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripEditorPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripEditorDel = new System.Windows.Forms.ToolStripMenuItem();
            this.panelEditor = new System.Windows.Forms.FlowLayoutPanel();
            this.chkCRT = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.viewPCGEdit)).BeginInit();
            this.contextPCGList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.viewColorL)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewColorR)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewPalette)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewSandbox)).BeginInit();
            this.panelPCG.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.viewPCG)).BeginInit();
            this.contextSandbox.SuspendLayout();
            this.panelSandbox.SuspendLayout();
            this.contextEditor.SuspendLayout();
            this.panelEditor.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Yu Gothic UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label1.Location = new System.Drawing.Point(13, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "PCG Editor";
            // 
            // viewPCGEdit
            // 
            this.viewPCGEdit.Location = new System.Drawing.Point(0, 0);
            this.viewPCGEdit.Margin = new System.Windows.Forms.Padding(0);
            this.viewPCGEdit.Name = "viewPCGEdit";
            this.viewPCGEdit.Size = new System.Drawing.Size(258, 258);
            this.viewPCGEdit.TabIndex = 1;
            this.viewPCGEdit.TabStop = false;
            this.viewPCGEdit.MouseClick += new System.Windows.Forms.MouseEventHandler(this.viewPCGEdit_MouseClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ContextMenuStrip = this.contextPCGList;
            this.label2.Font = new System.Drawing.Font("Yu Gothic UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label2.Location = new System.Drawing.Point(301, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "PCG";
            // 
            // contextPCGList
            // 
            this.contextPCGList.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextPCGList.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripPCGCopy,
            this.toolStripPCGPaste,
            this.toolStripSeparator1,
            this.toolStripPCGDel});
            this.contextPCGList.Name = "contextPCGList";
            this.contextPCGList.Size = new System.Drawing.Size(176, 82);
            // 
            // toolStripPCGCopy
            // 
            this.toolStripPCGCopy.Name = "toolStripPCGCopy";
            this.toolStripPCGCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.toolStripPCGCopy.Size = new System.Drawing.Size(175, 24);
            this.toolStripPCGCopy.Text = "Copy";
            // 
            // toolStripPCGPaste
            // 
            this.toolStripPCGPaste.Name = "toolStripPCGPaste";
            this.toolStripPCGPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.toolStripPCGPaste.Size = new System.Drawing.Size(175, 24);
            this.toolStripPCGPaste.Text = "Paste";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(172, 6);
            // 
            // toolStripPCGDel
            // 
            this.toolStripPCGDel.Name = "toolStripPCGDel";
            this.toolStripPCGDel.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.toolStripPCGDel.Size = new System.Drawing.Size(175, 24);
            this.toolStripPCGDel.Text = "Delete";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Yu Gothic UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label3.Location = new System.Drawing.Point(13, 301);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(97, 20);
            this.label3.TabIndex = 4;
            this.label3.Text = "Current Color";
            // 
            // viewColorL
            // 
            this.viewColorL.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.viewColorL.Location = new System.Drawing.Point(14, 328);
            this.viewColorL.Name = "viewColorL";
            this.viewColorL.Size = new System.Drawing.Size(32, 32);
            this.viewColorL.TabIndex = 5;
            this.viewColorL.TabStop = false;
            this.viewColorL.Click += new System.EventHandler(this.viewColorL_Click);
            // 
            // viewColorR
            // 
            this.viewColorR.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.viewColorR.Location = new System.Drawing.Point(45, 328);
            this.viewColorR.Name = "viewColorR";
            this.viewColorR.Size = new System.Drawing.Size(32, 32);
            this.viewColorR.TabIndex = 6;
            this.viewColorR.TabStop = false;
            this.viewColorR.Click += new System.EventHandler(this.viewColorR_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Yu Gothic UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label4.Location = new System.Drawing.Point(13, 489);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 20);
            this.label4.TabIndex = 7;
            this.label4.Text = "Palette";
            // 
            // viewPalette
            // 
            this.viewPalette.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.viewPalette.Location = new System.Drawing.Point(13, 519);
            this.viewPalette.Name = "viewPalette";
            this.viewPalette.Size = new System.Drawing.Size(258, 66);
            this.viewPalette.TabIndex = 8;
            this.viewPalette.TabStop = false;
            this.viewPalette.MouseClick += new System.Windows.Forms.MouseEventHandler(this.viewPalette_MouseClick);
            this.viewPalette.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.viewPalette_MouseDoubleClick);
            // 
            // btnOpenPalette
            // 
            this.btnOpenPalette.BackColor = System.Drawing.Color.White;
            this.btnOpenPalette.BackgroundImage = global::_99x8Edit.Properties.Resources.open;
            this.btnOpenPalette.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnOpenPalette.Enabled = false;
            this.btnOpenPalette.Location = new System.Drawing.Point(208, 484);
            this.btnOpenPalette.Name = "btnOpenPalette";
            this.btnOpenPalette.Size = new System.Drawing.Size(32, 32);
            this.btnOpenPalette.TabIndex = 14;
            this.toolTipPCG.SetToolTip(this.btnOpenPalette, "Load Palettes");
            this.btnOpenPalette.UseVisualStyleBackColor = false;
            this.btnOpenPalette.Click += new System.EventHandler(this.btnOpenPalette_Click);
            // 
            // checkTMS
            // 
            this.checkTMS.AutoSize = true;
            this.checkTMS.Checked = true;
            this.checkTMS.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkTMS.Font = new System.Drawing.Font("Yu Gothic UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.checkTMS.Location = new System.Drawing.Point(109, 489);
            this.checkTMS.Name = "checkTMS";
            this.checkTMS.Size = new System.Drawing.Size(92, 24);
            this.checkTMS.TabIndex = 15;
            this.checkTMS.Text = "TMS9918";
            this.checkTMS.UseVisualStyleBackColor = true;
            this.checkTMS.Click += new System.EventHandler(this.checkTMS_Click);
            // 
            // btnSavePalette
            // 
            this.btnSavePalette.BackColor = System.Drawing.Color.White;
            this.btnSavePalette.BackgroundImage = global::_99x8Edit.Properties.Resources.save;
            this.btnSavePalette.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnSavePalette.Enabled = false;
            this.btnSavePalette.Location = new System.Drawing.Point(239, 484);
            this.btnSavePalette.Name = "btnSavePalette";
            this.btnSavePalette.Size = new System.Drawing.Size(32, 32);
            this.btnSavePalette.TabIndex = 16;
            this.toolTipPCG.SetToolTip(this.btnSavePalette, "Save Palettes");
            this.btnSavePalette.UseVisualStyleBackColor = false;
            this.btnSavePalette.Click += new System.EventHandler(this.btnSavePalette_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Yu Gothic UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label5.Location = new System.Drawing.Point(24, 361);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(13, 15);
            this.label5.TabIndex = 17;
            this.label5.Text = "1";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Yu Gothic UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label6.Location = new System.Drawing.Point(52, 361);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(13, 15);
            this.label6.TabIndex = 18;
            this.label6.Text = "0";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Yu Gothic UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label7.Location = new System.Drawing.Point(301, 173);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(67, 20);
            this.label7.TabIndex = 19;
            this.label7.Text = "Sandbox";
            // 
            // viewSandbox
            // 
            this.viewSandbox.Location = new System.Drawing.Point(0, 0);
            this.viewSandbox.Margin = new System.Windows.Forms.Padding(0);
            this.viewSandbox.Name = "viewSandbox";
            this.viewSandbox.Size = new System.Drawing.Size(514, 386);
            this.viewSandbox.TabIndex = 20;
            this.viewSandbox.TabStop = false;
            this.viewSandbox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.viewSandbox_MouseClick);
            // 
            // panelPCG
            // 
            this.panelPCG.AllowDrop = true;
            this.panelPCG.ContextMenuStrip = this.contextPCGList;
            this.panelPCG.Controls.Add(this.viewPCG);
            this.panelPCG.Location = new System.Drawing.Point(301, 30);
            this.panelPCG.Name = "panelPCG";
            this.panelPCG.Size = new System.Drawing.Size(534, 130);
            this.panelPCG.TabIndex = 22;
            this.panelPCG.TabStop = true;
            this.panelPCG.DragDrop += new System.Windows.Forms.DragEventHandler(this.panelPCG_DragDrop);
            this.panelPCG.DragEnter += new System.Windows.Forms.DragEventHandler(this.panelPCG_DragEnter);
            this.panelPCG.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.panelPCG_PreviewKeyDown);
            // 
            // viewPCG
            // 
            this.viewPCG.Location = new System.Drawing.Point(3, 3);
            this.viewPCG.Name = "viewPCG";
            this.viewPCG.Size = new System.Drawing.Size(514, 130);
            this.viewPCG.TabIndex = 0;
            this.viewPCG.TabStop = false;
            this.viewPCG.MouseDown += new System.Windows.Forms.MouseEventHandler(this.viewPCG_MouseDown);
            // 
            // contextSandbox
            // 
            this.contextSandbox.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextSandbox.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSandboxCopy,
            this.toolStripSandboxPaste,
            this.toolStripSeparator2,
            this.toolStripSandboxDel,
            this.toolStripSandboxPaint});
            this.contextSandbox.Name = "contextPCGList";
            this.contextSandbox.Size = new System.Drawing.Size(176, 106);
            // 
            // toolStripSandboxCopy
            // 
            this.toolStripSandboxCopy.Name = "toolStripSandboxCopy";
            this.toolStripSandboxCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.toolStripSandboxCopy.Size = new System.Drawing.Size(175, 24);
            this.toolStripSandboxCopy.Text = "Copy";
            // 
            // toolStripSandboxPaste
            // 
            this.toolStripSandboxPaste.Name = "toolStripSandboxPaste";
            this.toolStripSandboxPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.toolStripSandboxPaste.Size = new System.Drawing.Size(175, 24);
            this.toolStripSandboxPaste.Text = "Paste";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(172, 6);
            // 
            // toolStripSandboxDel
            // 
            this.toolStripSandboxDel.Name = "toolStripSandboxDel";
            this.toolStripSandboxDel.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.toolStripSandboxDel.Size = new System.Drawing.Size(175, 24);
            this.toolStripSandboxDel.Text = "Delete";
            // 
            // toolStripSandboxPaint
            // 
            this.toolStripSandboxPaint.Name = "toolStripSandboxPaint";
            this.toolStripSandboxPaint.Size = new System.Drawing.Size(175, 24);
            this.toolStripSandboxPaint.Text = "Paint";
            // 
            // panelSandbox
            // 
            this.panelSandbox.AllowDrop = true;
            this.panelSandbox.ContextMenuStrip = this.contextSandbox;
            this.panelSandbox.Controls.Add(this.viewSandbox);
            this.panelSandbox.Location = new System.Drawing.Point(301, 199);
            this.panelSandbox.Name = "panelSandbox";
            this.panelSandbox.Size = new System.Drawing.Size(534, 386);
            this.panelSandbox.TabIndex = 24;
            this.panelSandbox.DragDrop += new System.Windows.Forms.DragEventHandler(this.panelSandbox_DragDrop);
            this.panelSandbox.DragEnter += new System.Windows.Forms.DragEventHandler(this.panelSandbox_DragEnter);
            this.panelSandbox.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.panelSandbox_PreviewKeyDown);
            // 
            // contextEditor
            // 
            this.contextEditor.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextEditor.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripEditorCopy,
            this.toolStripEditorPaste,
            this.toolStripSeparator3,
            this.toolStripEditorDel});
            this.contextEditor.Name = "contextPCGList";
            this.contextEditor.Size = new System.Drawing.Size(176, 82);
            // 
            // toolStripEditorCopy
            // 
            this.toolStripEditorCopy.Name = "toolStripEditorCopy";
            this.toolStripEditorCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.toolStripEditorCopy.Size = new System.Drawing.Size(175, 24);
            this.toolStripEditorCopy.Text = "Copy";
            // 
            // toolStripEditorPaste
            // 
            this.toolStripEditorPaste.Name = "toolStripEditorPaste";
            this.toolStripEditorPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.toolStripEditorPaste.Size = new System.Drawing.Size(175, 24);
            this.toolStripEditorPaste.Text = "Paste";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(172, 6);
            // 
            // toolStripEditorDel
            // 
            this.toolStripEditorDel.Name = "toolStripEditorDel";
            this.toolStripEditorDel.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.toolStripEditorDel.Size = new System.Drawing.Size(175, 24);
            this.toolStripEditorDel.Text = "Delete";
            // 
            // panelEditor
            // 
            this.panelEditor.ContextMenuStrip = this.contextEditor;
            this.panelEditor.Controls.Add(this.viewPCGEdit);
            this.panelEditor.Location = new System.Drawing.Point(14, 34);
            this.panelEditor.Name = "panelEditor";
            this.panelEditor.Size = new System.Drawing.Size(267, 264);
            this.panelEditor.TabIndex = 26;
            this.panelEditor.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.panelEditor_PreviewKeyDown);
            // 
            // chkCRT
            // 
            this.chkCRT.AutoSize = true;
            this.chkCRT.Font = new System.Drawing.Font("Yu Gothic UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.chkCRT.Location = new System.Drawing.Point(723, 5);
            this.chkCRT.Name = "chkCRT";
            this.chkCRT.Size = new System.Drawing.Size(94, 24);
            this.chkCRT.TabIndex = 27;
            this.chkCRT.Text = "CRT Filter";
            this.chkCRT.UseVisualStyleBackColor = true;
            this.chkCRT.CheckedChanged += new System.EventHandler(this.chkCRT_CheckedChanged);
            // 
            // PCGEditor
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(839, 598);
            this.Controls.Add(this.chkCRT);
            this.Controls.Add(this.panelEditor);
            this.Controls.Add(this.panelSandbox);
            this.Controls.Add(this.panelPCG);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnSavePalette);
            this.Controls.Add(this.checkTMS);
            this.Controls.Add(this.btnOpenPalette);
            this.Controls.Add(this.viewPalette);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.viewColorR);
            this.Controls.Add(this.viewColorL);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PCGEditor";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "PCG Editor";
            this.Activated += new System.EventHandler(this.FormPCG_Activated);
            ((System.ComponentModel.ISupportInitialize)(this.viewPCGEdit)).EndInit();
            this.contextPCGList.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.viewColorL)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewColorR)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewPalette)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewSandbox)).EndInit();
            this.panelPCG.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.viewPCG)).EndInit();
            this.contextSandbox.ResumeLayout(false);
            this.panelSandbox.ResumeLayout(false);
            this.contextEditor.ResumeLayout(false);
            this.panelEditor.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox viewPCGEdit;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.PictureBox viewColorL;
        private System.Windows.Forms.PictureBox viewColorR;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox viewPalette;
        private System.Windows.Forms.Button btnOpenPalette;
        private System.Windows.Forms.CheckBox checkTMS;
        private System.Windows.Forms.Button btnSavePalette;
        private System.Windows.Forms.ToolTip toolTipPCG;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.PictureBox viewSandbox;
        private System.Windows.Forms.ContextMenuStrip contextPCGList;
        private System.Windows.Forms.ToolStripMenuItem toolStripPCGCopy;
        private System.Windows.Forms.ToolStripMenuItem toolStripPCGPaste;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem toolStripPCGDel;
        private System.Windows.Forms.FlowLayoutPanel panelPCG;
        private System.Windows.Forms.PictureBox viewPCG;
        private System.Windows.Forms.ContextMenuStrip contextSandbox;
        private System.Windows.Forms.ToolStripMenuItem toolStripSandboxCopy;
        private System.Windows.Forms.ToolStripMenuItem toolStripSandboxPaste;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem toolStripSandboxDel;
        private System.Windows.Forms.FlowLayoutPanel panelSandbox;
        private System.Windows.Forms.ToolStripMenuItem toolStripSandboxPaint;
        private System.Windows.Forms.ContextMenuStrip contextEditor;
        private System.Windows.Forms.ToolStripMenuItem toolStripEditorCopy;
        private System.Windows.Forms.ToolStripMenuItem toolStripEditorPaste;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem toolStripEditorDel;
        private System.Windows.Forms.FlowLayoutPanel panelEditor;
        private System.Windows.Forms.CheckBox chkCRT;
    }
}