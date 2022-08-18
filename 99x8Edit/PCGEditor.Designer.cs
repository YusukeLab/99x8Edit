
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
            this.label2 = new System.Windows.Forms.Label();
            this.contextPCG = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripPCGCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripPCGPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripPCGCopyDown = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripPCGCopyRight = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripPCGDel = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripPCGInverse = new System.Windows.Forms.ToolStripMenuItem();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.chkTMS = new System.Windows.Forms.CheckBox();
            this.toolTipPCG = new System.Windows.Forms.ToolTip(this.components);
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.contextSandbox = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripSandboxCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSandboxPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSandboxCopyDown = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSandboxCopyRight = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSandboxDel = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSandboxPaint = new System.Windows.Forms.ToolStripMenuItem();
            this.contextEditor = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripEditorCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripEditorPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripEditorCopyDown = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripEditorCopyRight = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripEditorDel = new System.Windows.Forms.ToolStripMenuItem();
            this.chkCRT = new System.Windows.Forms.CheckBox();
            this.menuStripMain = new System.Windows.Forms.MenuStrip();
            this.toolStripBarFile = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripFileLoad = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripFileSave = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripFileSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripFileImport = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripFileExport = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripFileLoadPCG = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripFileSavePCG = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripFileLoadPal = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripFileSavePal = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripBarEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripEditUndo = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripEditRedo = new System.Windows.Forms.ToolStripMenuItem();
            this.viewSand = new _99x8Edit.MatrixControl();
            this.viewPCG = new _99x8Edit.MatrixControl();
            this.viewEdit = new _99x8Edit.EditorControl();
            this.viewColor = new _99x8Edit.MatrixControl();
            this.viewPalette = new _99x8Edit.MatrixControl();
            this.contextPCG.SuspendLayout();
            this.contextSandbox.SuspendLayout();
            this.contextEditor.SuspendLayout();
            this.menuStripMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Yu Gothic UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label1.Location = new System.Drawing.Point(12, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "PCG Editor";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ContextMenuStrip = this.contextPCG;
            this.label2.Font = new System.Drawing.Font("Yu Gothic UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label2.Location = new System.Drawing.Point(301, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "PCG";
            // 
            // contextPCG
            // 
            this.contextPCG.AllowDrop = true;
            this.contextPCG.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextPCG.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripPCGCopy,
            this.toolStripPCGPaste,
            this.toolStripPCGCopyDown,
            this.toolStripPCGCopyRight,
            this.toolStripSeparator1,
            this.toolStripPCGDel,
            this.toolStripPCGInverse});
            this.contextPCG.Name = "contextPCGList";
            this.contextPCG.Size = new System.Drawing.Size(209, 154);
            // 
            // toolStripPCGCopy
            // 
            this.toolStripPCGCopy.Name = "toolStripPCGCopy";
            this.toolStripPCGCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.toolStripPCGCopy.Size = new System.Drawing.Size(208, 24);
            this.toolStripPCGCopy.Text = "Copy";
            // 
            // toolStripPCGPaste
            // 
            this.toolStripPCGPaste.Name = "toolStripPCGPaste";
            this.toolStripPCGPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.toolStripPCGPaste.Size = new System.Drawing.Size(208, 24);
            this.toolStripPCGPaste.Text = "Paste";
            // 
            // toolStripPCGCopyDown
            // 
            this.toolStripPCGCopyDown.Name = "toolStripPCGCopyDown";
            this.toolStripPCGCopyDown.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.toolStripPCGCopyDown.Size = new System.Drawing.Size(208, 24);
            this.toolStripPCGCopyDown.Text = "Copy Down";
            // 
            // toolStripPCGCopyRight
            // 
            this.toolStripPCGCopyRight.Name = "toolStripPCGCopyRight";
            this.toolStripPCGCopyRight.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.toolStripPCGCopyRight.Size = new System.Drawing.Size(208, 24);
            this.toolStripPCGCopyRight.Text = "Copy Right";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(205, 6);
            // 
            // toolStripPCGDel
            // 
            this.toolStripPCGDel.Name = "toolStripPCGDel";
            this.toolStripPCGDel.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.toolStripPCGDel.Size = new System.Drawing.Size(208, 24);
            this.toolStripPCGDel.Text = "Delete";
            // 
            // toolStripPCGInverse
            // 
            this.toolStripPCGInverse.Name = "toolStripPCGInverse";
            this.toolStripPCGInverse.Size = new System.Drawing.Size(208, 24);
            this.toolStripPCGInverse.Text = "Swap Color";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Yu Gothic UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label3.Location = new System.Drawing.Point(13, 332);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(97, 20);
            this.label3.TabIndex = 4;
            this.label3.Text = "Current Color";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Yu Gothic UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label4.Location = new System.Drawing.Point(13, 524);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 20);
            this.label4.TabIndex = 7;
            this.label4.Text = "Palette";
            // 
            // chkTMS
            // 
            this.chkTMS.AutoSize = true;
            this.chkTMS.Checked = true;
            this.chkTMS.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTMS.Font = new System.Drawing.Font("Yu Gothic UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.chkTMS.Location = new System.Drawing.Point(179, 524);
            this.chkTMS.Name = "chkTMS";
            this.chkTMS.Size = new System.Drawing.Size(92, 24);
            this.chkTMS.TabIndex = 15;
            this.chkTMS.Text = "TMS9918";
            this.chkTMS.UseVisualStyleBackColor = true;
            this.chkTMS.Click += new System.EventHandler(this.checkTMS_Click);
            // 
            // toolTipPCG
            // 
            this.toolTipPCG.AutomaticDelay = 0;
            this.toolTipPCG.AutoPopDelay = 0;
            this.toolTipPCG.InitialDelay = 0;
            this.toolTipPCG.ReshowDelay = 0;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Yu Gothic UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label5.Location = new System.Drawing.Point(25, 395);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(13, 15);
            this.label5.TabIndex = 17;
            this.label5.Text = "1";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Yu Gothic UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label6.Location = new System.Drawing.Point(53, 395);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(13, 15);
            this.label6.TabIndex = 18;
            this.label6.Text = "0";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Yu Gothic UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label7.Location = new System.Drawing.Point(301, 204);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(67, 20);
            this.label7.TabIndex = 19;
            this.label7.Text = "Sandbox";
            // 
            // contextSandbox
            // 
            this.contextSandbox.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextSandbox.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSandboxCopy,
            this.toolStripSandboxPaste,
            this.toolStripSandboxCopyDown,
            this.toolStripSandboxCopyRight,
            this.toolStripSeparator2,
            this.toolStripSandboxDel,
            this.toolStripSandboxPaint});
            this.contextSandbox.Name = "contextPCGList";
            this.contextSandbox.Size = new System.Drawing.Size(209, 154);
            // 
            // toolStripSandboxCopy
            // 
            this.toolStripSandboxCopy.Name = "toolStripSandboxCopy";
            this.toolStripSandboxCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.toolStripSandboxCopy.Size = new System.Drawing.Size(208, 24);
            this.toolStripSandboxCopy.Text = "Copy";
            // 
            // toolStripSandboxPaste
            // 
            this.toolStripSandboxPaste.Name = "toolStripSandboxPaste";
            this.toolStripSandboxPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.toolStripSandboxPaste.Size = new System.Drawing.Size(208, 24);
            this.toolStripSandboxPaste.Text = "Paste";
            // 
            // toolStripSandboxCopyDown
            // 
            this.toolStripSandboxCopyDown.Name = "toolStripSandboxCopyDown";
            this.toolStripSandboxCopyDown.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.toolStripSandboxCopyDown.Size = new System.Drawing.Size(208, 24);
            this.toolStripSandboxCopyDown.Text = "Copy Down";
            // 
            // toolStripSandboxCopyRight
            // 
            this.toolStripSandboxCopyRight.Name = "toolStripSandboxCopyRight";
            this.toolStripSandboxCopyRight.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.toolStripSandboxCopyRight.Size = new System.Drawing.Size(208, 24);
            this.toolStripSandboxCopyRight.Text = "Copy Right";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(205, 6);
            // 
            // toolStripSandboxDel
            // 
            this.toolStripSandboxDel.Name = "toolStripSandboxDel";
            this.toolStripSandboxDel.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.toolStripSandboxDel.Size = new System.Drawing.Size(208, 24);
            this.toolStripSandboxDel.Text = "Delete";
            // 
            // toolStripSandboxPaint
            // 
            this.toolStripSandboxPaint.Name = "toolStripSandboxPaint";
            this.toolStripSandboxPaint.Size = new System.Drawing.Size(208, 24);
            this.toolStripSandboxPaint.Text = "Paint";
            // 
            // contextEditor
            // 
            this.contextEditor.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextEditor.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripEditorCopy,
            this.toolStripEditorPaste,
            this.toolStripEditorCopyDown,
            this.toolStripEditorCopyRight,
            this.toolStripSeparator3,
            this.toolStripEditorDel});
            this.contextEditor.Name = "contextPCGList";
            this.contextEditor.Size = new System.Drawing.Size(209, 130);
            // 
            // toolStripEditorCopy
            // 
            this.toolStripEditorCopy.Name = "toolStripEditorCopy";
            this.toolStripEditorCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.toolStripEditorCopy.Size = new System.Drawing.Size(208, 24);
            this.toolStripEditorCopy.Text = "Copy Line";
            // 
            // toolStripEditorPaste
            // 
            this.toolStripEditorPaste.Name = "toolStripEditorPaste";
            this.toolStripEditorPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.toolStripEditorPaste.Size = new System.Drawing.Size(208, 24);
            this.toolStripEditorPaste.Text = "Paste Line";
            // 
            // toolStripEditorCopyDown
            // 
            this.toolStripEditorCopyDown.Name = "toolStripEditorCopyDown";
            this.toolStripEditorCopyDown.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.toolStripEditorCopyDown.Size = new System.Drawing.Size(208, 24);
            this.toolStripEditorCopyDown.Text = "Copy Down";
            // 
            // toolStripEditorCopyRight
            // 
            this.toolStripEditorCopyRight.Name = "toolStripEditorCopyRight";
            this.toolStripEditorCopyRight.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.toolStripEditorCopyRight.Size = new System.Drawing.Size(208, 24);
            this.toolStripEditorCopyRight.Text = "Copy Right";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(205, 6);
            // 
            // toolStripEditorDel
            // 
            this.toolStripEditorDel.Name = "toolStripEditorDel";
            this.toolStripEditorDel.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.toolStripEditorDel.Size = new System.Drawing.Size(208, 24);
            this.toolStripEditorDel.Text = "Delete";
            // 
            // chkCRT
            // 
            this.chkCRT.AutoSize = true;
            this.chkCRT.Font = new System.Drawing.Font("Yu Gothic UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.chkCRT.Location = new System.Drawing.Point(721, 37);
            this.chkCRT.Name = "chkCRT";
            this.chkCRT.Size = new System.Drawing.Size(94, 24);
            this.chkCRT.TabIndex = 27;
            this.chkCRT.Text = "CRT Filter";
            this.chkCRT.UseVisualStyleBackColor = true;
            this.chkCRT.CheckedChanged += new System.EventHandler(this.chkCRT_CheckedChanged);
            // 
            // menuStripMain
            // 
            this.menuStripMain.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripBarFile,
            this.toolStripBarEdit});
            this.menuStripMain.Location = new System.Drawing.Point(0, 0);
            this.menuStripMain.Name = "menuStripMain";
            this.menuStripMain.Size = new System.Drawing.Size(839, 28);
            this.menuStripMain.TabIndex = 31;
            this.menuStripMain.Text = "menuStrip1";
            // 
            // toolStripBarFile
            // 
            this.toolStripBarFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripFileLoad,
            this.toolStripFileSave,
            this.toolStripFileSaveAs,
            this.toolStripSeparator4,
            this.toolStripFileImport,
            this.toolStripFileExport,
            this.toolStripFileLoadPCG,
            this.toolStripFileSavePCG,
            this.toolStripSeparator5,
            this.toolStripFileLoadPal,
            this.toolStripFileSavePal});
            this.toolStripBarFile.Name = "toolStripBarFile";
            this.toolStripBarFile.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F)));
            this.toolStripBarFile.Size = new System.Drawing.Size(63, 24);
            this.toolStripBarFile.Text = "File(&F)";
            // 
            // toolStripFileLoad
            // 
            this.toolStripFileLoad.Name = "toolStripFileLoad";
            this.toolStripFileLoad.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.O)));
            this.toolStripFileLoad.Size = new System.Drawing.Size(261, 26);
            this.toolStripFileLoad.Text = "Open Project(&O)";
            // 
            // toolStripFileSave
            // 
            this.toolStripFileSave.Name = "toolStripFileSave";
            this.toolStripFileSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.toolStripFileSave.Size = new System.Drawing.Size(261, 26);
            this.toolStripFileSave.Text = "Save Project";
            // 
            // toolStripFileSaveAs
            // 
            this.toolStripFileSaveAs.Name = "toolStripFileSaveAs";
            this.toolStripFileSaveAs.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.A)));
            this.toolStripFileSaveAs.Size = new System.Drawing.Size(261, 26);
            this.toolStripFileSaveAs.Text = "Save Project As(&A)";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(258, 6);
            // 
            // toolStripFileImport
            // 
            this.toolStripFileImport.Name = "toolStripFileImport";
            this.toolStripFileImport.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.I)));
            this.toolStripFileImport.Size = new System.Drawing.Size(261, 26);
            this.toolStripFileImport.Text = "Import PCG(&I)";
            // 
            // toolStripFileExport
            // 
            this.toolStripFileExport.Name = "toolStripFileExport";
            this.toolStripFileExport.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.E)));
            this.toolStripFileExport.Size = new System.Drawing.Size(261, 26);
            this.toolStripFileExport.Text = "Export PCG(&E)";
            // 
            // toolStripFileLoadPCG
            // 
            this.toolStripFileLoadPCG.Name = "toolStripFileLoadPCG";
            this.toolStripFileLoadPCG.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.L)));
            this.toolStripFileLoadPCG.Size = new System.Drawing.Size(261, 26);
            this.toolStripFileLoadPCG.Text = "Load PCG Data(&L)";
            // 
            // toolStripFileSavePCG
            // 
            this.toolStripFileSavePCG.Name = "toolStripFileSavePCG";
            this.toolStripFileSavePCG.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.S)));
            this.toolStripFileSavePCG.Size = new System.Drawing.Size(261, 26);
            this.toolStripFileSavePCG.Text = "Save PCG Data(&S)";
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(258, 6);
            // 
            // toolStripFileLoadPal
            // 
            this.toolStripFileLoadPal.Name = "toolStripFileLoadPal";
            this.toolStripFileLoadPal.Size = new System.Drawing.Size(261, 26);
            this.toolStripFileLoadPal.Text = "Load Palette";
            // 
            // toolStripFileSavePal
            // 
            this.toolStripFileSavePal.Name = "toolStripFileSavePal";
            this.toolStripFileSavePal.Size = new System.Drawing.Size(261, 26);
            this.toolStripFileSavePal.Text = "Save Palette";
            // 
            // toolStripBarEdit
            // 
            this.toolStripBarEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripEditUndo,
            this.toolStripEditRedo});
            this.toolStripBarEdit.Name = "toolStripBarEdit";
            this.toolStripBarEdit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.E)));
            this.toolStripBarEdit.Size = new System.Drawing.Size(67, 24);
            this.toolStripBarEdit.Text = "Edit(&E)";
            // 
            // toolStripEditUndo
            // 
            this.toolStripEditUndo.Name = "toolStripEditUndo";
            this.toolStripEditUndo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.toolStripEditUndo.Size = new System.Drawing.Size(179, 26);
            this.toolStripEditUndo.Text = "Undo";
            // 
            // toolStripEditRedo
            // 
            this.toolStripEditRedo.Name = "toolStripEditRedo";
            this.toolStripEditRedo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.toolStripEditRedo.Size = new System.Drawing.Size(179, 26);
            this.toolStripEditRedo.Text = "Redo";
            // 
            // viewSand
            // 
            this.viewSand.AllowDrop = true;
            this.viewSand.AllowMultipleSelection = true;
            this.viewSand.AllowSubSelection = false;
            this.viewSand.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.viewSand.CellHeight = 16;
            this.viewSand.CellWidth = 16;
            this.viewSand.ColumnNum = 32;
            this.viewSand.ContextMenuStrip = this.contextSandbox;
            this.viewSand.Location = new System.Drawing.Point(301, 227);
            this.viewSand.Name = "viewSand";
            this.viewSand.RowNum = 24;
            this.viewSand.SelectionHeight = 1;
            this.viewSand.SelectionWidth = 1;
            this.viewSand.Size = new System.Drawing.Size(514, 386);
            this.viewSand.TabIndex = 33;
            this.viewSand.X = 0;
            this.viewSand.Y = 0;
            this.viewSand.DragDrop += new System.Windows.Forms.DragEventHandler(this.viewSand_DragDrop);
            this.viewSand.DragEnter += new System.Windows.Forms.DragEventHandler(this.viewSand_DragEnter);
            this.viewSand.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.viewSand_PreviewKeyDown);
            // 
            // viewPCG
            // 
            this.viewPCG.AllowDrop = true;
            this.viewPCG.AllowMultipleSelection = true;
            this.viewPCG.AllowSubSelection = false;
            this.viewPCG.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.viewPCG.CellHeight = 16;
            this.viewPCG.CellWidth = 16;
            this.viewPCG.ColumnNum = 32;
            this.viewPCG.ContextMenuStrip = this.contextPCG;
            this.viewPCG.Location = new System.Drawing.Point(301, 61);
            this.viewPCG.Name = "viewPCG";
            this.viewPCG.RowNum = 8;
            this.viewPCG.SelectionHeight = 1;
            this.viewPCG.SelectionWidth = 1;
            this.viewPCG.Size = new System.Drawing.Size(514, 130);
            this.viewPCG.TabIndex = 34;
            this.viewPCG.X = 0;
            this.viewPCG.Y = 0;
            this.viewPCG.SelectionChanged += new System.EventHandler<System.EventArgs>(this.viewPCG_SelectionChanged);
            this.viewPCG.CellDragStart += new System.EventHandler<System.EventArgs>(this.viewPCG_CellDragStart);
            this.viewPCG.DragDrop += new System.Windows.Forms.DragEventHandler(this.viewPCG_DragDrop);
            this.viewPCG.DragEnter += new System.Windows.Forms.DragEventHandler(this.viewPCG_DragEnter);
            this.viewPCG.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.viewPCG_PreviewKeyDown);
            // 
            // viewEdit
            // 
            this.viewEdit.AllowDrop = true;
            this.viewEdit.AllowMultipleSelection = true;
            this.viewEdit.AllowSubSelection = true;
            this.viewEdit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.viewEdit.CellHeight = 16;
            this.viewEdit.CellWidth = 16;
            this.viewEdit.ColumnNum = 16;
            this.viewEdit.ContextMenuStrip = this.contextEditor;
            this.viewEdit.Location = new System.Drawing.Point(14, 61);
            this.viewEdit.Name = "viewEdit";
            this.viewEdit.RowNum = 16;
            this.viewEdit.SelectionHeight = 1;
            this.viewEdit.SelectionWidth = 8;
            this.viewEdit.Size = new System.Drawing.Size(258, 258);
            this.viewEdit.TabIndex = 35;
            this.viewEdit.X = 0;
            this.viewEdit.Y = 0;
            this.viewEdit.SelectionChanged += new System.EventHandler<System.EventArgs>(this.viewEdit_SelectionChanged);
            this.viewEdit.CellOnEdit += new System.EventHandler<System.EventArgs>(this.viewEdit_Edited);
            this.viewEdit.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.viewEdit_PreviewKeyDown);
            // 
            // viewColor
            // 
            this.viewColor.AllowMultipleSelection = false;
            this.viewColor.AllowSubSelection = false;
            this.viewColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.viewColor.CellHeight = 32;
            this.viewColor.CellWidth = 32;
            this.viewColor.ColumnNum = 2;
            this.viewColor.Location = new System.Drawing.Point(14, 358);
            this.viewColor.Name = "viewColor";
            this.viewColor.RowNum = 1;
            this.viewColor.SelectionHeight = 1;
            this.viewColor.SelectionWidth = 1;
            this.viewColor.Size = new System.Drawing.Size(66, 34);
            this.viewColor.TabIndex = 36;
            this.viewColor.X = 0;
            this.viewColor.Y = 0;
            this.viewColor.CellOnEdit += new System.EventHandler<System.EventArgs>(this.viewColor_Click);
            this.viewColor.Click += new System.EventHandler(this.viewColor_Click);
            // 
            // viewPalette
            // 
            this.viewPalette.AllowMultipleSelection = false;
            this.viewPalette.AllowSubSelection = false;
            this.viewPalette.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.viewPalette.CellHeight = 32;
            this.viewPalette.CellWidth = 32;
            this.viewPalette.ColumnNum = 8;
            this.viewPalette.Location = new System.Drawing.Point(13, 550);
            this.viewPalette.Name = "viewPalette";
            this.viewPalette.RowNum = 2;
            this.viewPalette.SelectionHeight = 1;
            this.viewPalette.SelectionWidth = 1;
            this.viewPalette.Size = new System.Drawing.Size(258, 66);
            this.viewPalette.TabIndex = 37;
            this.viewPalette.X = 0;
            this.viewPalette.Y = 0;
            this.viewPalette.CellOnEdit += new System.EventHandler<System.EventArgs>(this.viewPalette_CellOnEdit);
            this.viewPalette.MouseClick += new System.Windows.Forms.MouseEventHandler(this.viewPalette_MouseClick);
            this.viewPalette.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.viewPalette_MouseDoubleClick);
            // 
            // PCGEditor
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(839, 642);
            this.Controls.Add(this.viewPalette);
            this.Controls.Add(this.viewColor);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.viewEdit);
            this.Controls.Add(this.viewPCG);
            this.Controls.Add(this.viewSand);
            this.Controls.Add(this.menuStripMain);
            this.Controls.Add(this.chkCRT);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.chkTMS);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStripMain;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PCGEditor";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "PCG Editor";
            this.Activated += new System.EventHandler(this.FormPCG_Activated);
            this.contextPCG.ResumeLayout(false);
            this.contextSandbox.ResumeLayout(false);
            this.contextEditor.ResumeLayout(false);
            this.menuStripMain.ResumeLayout(false);
            this.menuStripMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkTMS;
        private System.Windows.Forms.ToolTip toolTipPCG;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ContextMenuStrip contextPCG;
        private System.Windows.Forms.ToolStripMenuItem toolStripPCGCopy;
        private System.Windows.Forms.ToolStripMenuItem toolStripPCGPaste;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem toolStripPCGDel;
        private System.Windows.Forms.ContextMenuStrip contextSandbox;
        private System.Windows.Forms.ToolStripMenuItem toolStripSandboxCopy;
        private System.Windows.Forms.ToolStripMenuItem toolStripSandboxPaste;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem toolStripSandboxDel;
        private System.Windows.Forms.ToolStripMenuItem toolStripSandboxPaint;
        private System.Windows.Forms.ContextMenuStrip contextEditor;
        private System.Windows.Forms.ToolStripMenuItem toolStripEditorCopy;
        private System.Windows.Forms.ToolStripMenuItem toolStripEditorPaste;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem toolStripEditorDel;
        private System.Windows.Forms.CheckBox chkCRT;
        private System.Windows.Forms.ToolStripMenuItem toolStripPCGInverse;
        private System.Windows.Forms.ToolStripMenuItem toolStripSandboxCopyDown;
        private System.Windows.Forms.ToolStripMenuItem toolStripSandboxCopyRight;
        private System.Windows.Forms.ToolStripMenuItem toolStripPCGCopyDown;
        private System.Windows.Forms.ToolStripMenuItem toolStripPCGCopyRight;
        private System.Windows.Forms.ToolStripMenuItem toolStripEditorCopyDown;
        private System.Windows.Forms.ToolStripMenuItem toolStripEditorCopyRight;
        private System.Windows.Forms.MenuStrip menuStripMain;
        private System.Windows.Forms.ToolStripMenuItem toolStripBarFile;
        private System.Windows.Forms.ToolStripMenuItem toolStripFileLoad;
        private System.Windows.Forms.ToolStripMenuItem toolStripFileSave;
        private System.Windows.Forms.ToolStripMenuItem toolStripFileSaveAs;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem toolStripFileImport;
        private System.Windows.Forms.ToolStripMenuItem toolStripFileExport;
        private System.Windows.Forms.ToolStripMenuItem toolStripFileSavePCG;
        private System.Windows.Forms.ToolStripMenuItem toolStripFileLoadPCG;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem toolStripFileLoadPal;
        private System.Windows.Forms.ToolStripMenuItem toolStripFileSavePal;
        private System.Windows.Forms.ToolStripMenuItem toolStripBarEdit;
        private System.Windows.Forms.ToolStripMenuItem toolStripEditUndo;
        private System.Windows.Forms.ToolStripMenuItem toolStripEditRedo;
        private MatrixControl viewSand;
        private MatrixControl viewPCG;
        private EditorControl viewEdit;
        private MatrixControl viewColor;
        private MatrixControl viewPalette;
    }
}