
namespace _99x8Edit
{
    partial class MapEditor
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
            this.label7 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtMapX = new System.Windows.Forms.TextBox();
            this.txtMapY = new System.Windows.Forms.TextBox();
            this.btnLeft = new System.Windows.Forms.Button();
            this.btnRight = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.contextPattern = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripPatternCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripPatternPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripPatternCopyDown = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripPatternCopyRight = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMap = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMapCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMapPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMapCopyDown = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMapCopyRight = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMapDel = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMapPaint = new System.Windows.Forms.ToolStripMenuItem();
            this.btnMapSize = new System.Windows.Forms.Button();
            this.chkCRT = new System.Windows.Forms.CheckBox();
            this.toolTipMap = new System.Windows.Forms.ToolTip(this.components);
            this.menuStripMain = new System.Windows.Forms.MenuStrip();
            this.toolStripBarFile = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripFileLoad = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripFileSave = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripFileSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripFileImport = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripFileExport = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripFileLoadMap = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripFileSaveMap = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripBarEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripEditUndo = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripEditRedo = new System.Windows.Forms.ToolStripMenuItem();
            this.viewPCG = new _99x8Edit.MatrixControl();
            this.contextPCG = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripPCGCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.viewMap = new _99x8Edit.MatrixControl();
            this.viewPtn = new _99x8Edit.MatrixControl();
            this.contextPattern.SuspendLayout();
            this.contextMap.SuspendLayout();
            this.menuStripMain.SuspendLayout();
            this.contextPCG.SuspendLayout();
            this.SuspendLayout();
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Yu Gothic UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label7.Location = new System.Drawing.Point(577, 39);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(90, 20);
            this.label7.TabIndex = 23;
            this.label7.Text = "Tile Patterns";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Yu Gothic UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label2.Location = new System.Drawing.Point(14, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 20);
            this.label2.TabIndex = 21;
            this.label2.Text = "PCG";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Yu Gothic UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label3.Location = new System.Drawing.Point(14, 213);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 20);
            this.label3.TabIndex = 27;
            this.label3.Text = "Map Editor";
            // 
            // txtMapX
            // 
            this.txtMapX.Enabled = false;
            this.txtMapX.Font = new System.Drawing.Font("Yu Gothic UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.txtMapX.Location = new System.Drawing.Point(261, 208);
            this.txtMapX.Name = "txtMapX";
            this.txtMapX.Size = new System.Drawing.Size(41, 27);
            this.txtMapX.TabIndex = 29;
            this.txtMapX.Text = "0";
            this.txtMapX.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtMapY
            // 
            this.txtMapY.Enabled = false;
            this.txtMapY.Font = new System.Drawing.Font("Yu Gothic UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.txtMapY.Location = new System.Drawing.Point(529, 407);
            this.txtMapY.Name = "txtMapY";
            this.txtMapY.Size = new System.Drawing.Size(37, 27);
            this.txtMapY.TabIndex = 30;
            this.txtMapY.Text = "0";
            this.txtMapY.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnLeft
            // 
            this.btnLeft.Location = new System.Drawing.Point(226, 207);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(29, 29);
            this.btnLeft.TabIndex = 31;
            this.btnLeft.Text = "←";
            this.btnLeft.UseVisualStyleBackColor = true;
            this.btnLeft.Click += new System.EventHandler(this.btnLeft_Click);
            // 
            // btnRight
            // 
            this.btnRight.Location = new System.Drawing.Point(307, 208);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(29, 29);
            this.btnRight.TabIndex = 32;
            this.btnRight.Text = "→";
            this.btnRight.UseVisualStyleBackColor = true;
            this.btnRight.Click += new System.EventHandler(this.btnRight_Click);
            // 
            // btnUp
            // 
            this.btnUp.Location = new System.Drawing.Point(533, 374);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(29, 29);
            this.btnUp.TabIndex = 33;
            this.btnUp.Text = "↑";
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnDown
            // 
            this.btnDown.Location = new System.Drawing.Point(533, 442);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(29, 29);
            this.btnDown.TabIndex = 34;
            this.btnDown.Text = "↓";
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // contextPattern
            // 
            this.contextPattern.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextPattern.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripPatternCopy,
            this.toolStripPatternPaste,
            this.toolStripPatternCopyDown,
            this.toolStripPatternCopyRight});
            this.contextPattern.Name = "contextPCGList";
            this.contextPattern.Size = new System.Drawing.Size(209, 100);
            // 
            // toolStripPatternCopy
            // 
            this.toolStripPatternCopy.Name = "toolStripPatternCopy";
            this.toolStripPatternCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.toolStripPatternCopy.Size = new System.Drawing.Size(208, 24);
            this.toolStripPatternCopy.Text = "Copy";
            // 
            // toolStripPatternPaste
            // 
            this.toolStripPatternPaste.Name = "toolStripPatternPaste";
            this.toolStripPatternPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.toolStripPatternPaste.Size = new System.Drawing.Size(208, 24);
            this.toolStripPatternPaste.Text = "Paste";
            // 
            // toolStripPatternCopyDown
            // 
            this.toolStripPatternCopyDown.Name = "toolStripPatternCopyDown";
            this.toolStripPatternCopyDown.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.toolStripPatternCopyDown.Size = new System.Drawing.Size(208, 24);
            this.toolStripPatternCopyDown.Text = "Copy Down";
            // 
            // toolStripPatternCopyRight
            // 
            this.toolStripPatternCopyRight.Name = "toolStripPatternCopyRight";
            this.toolStripPatternCopyRight.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.toolStripPatternCopyRight.Size = new System.Drawing.Size(208, 24);
            this.toolStripPatternCopyRight.Text = "Copy Right";
            // 
            // contextMap
            // 
            this.contextMap.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMap.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMapCopy,
            this.toolStripMapPaste,
            this.toolStripMapCopyDown,
            this.toolStripMapCopyRight,
            this.toolStripSeparator2,
            this.toolStripMapDel,
            this.toolStripMapPaint});
            this.contextMap.Name = "contextPCGList";
            this.contextMap.Size = new System.Drawing.Size(209, 154);
            // 
            // toolStripMapCopy
            // 
            this.toolStripMapCopy.Name = "toolStripMapCopy";
            this.toolStripMapCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.toolStripMapCopy.Size = new System.Drawing.Size(208, 24);
            this.toolStripMapCopy.Text = "Copy";
            // 
            // toolStripMapPaste
            // 
            this.toolStripMapPaste.Name = "toolStripMapPaste";
            this.toolStripMapPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.toolStripMapPaste.Size = new System.Drawing.Size(208, 24);
            this.toolStripMapPaste.Text = "Paste";
            // 
            // toolStripMapCopyDown
            // 
            this.toolStripMapCopyDown.Name = "toolStripMapCopyDown";
            this.toolStripMapCopyDown.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.toolStripMapCopyDown.Size = new System.Drawing.Size(208, 24);
            this.toolStripMapCopyDown.Text = "Copy Down";
            // 
            // toolStripMapCopyRight
            // 
            this.toolStripMapCopyRight.Name = "toolStripMapCopyRight";
            this.toolStripMapCopyRight.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.toolStripMapCopyRight.Size = new System.Drawing.Size(208, 24);
            this.toolStripMapCopyRight.Text = "Copy Right";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(205, 6);
            // 
            // toolStripMapDel
            // 
            this.toolStripMapDel.Name = "toolStripMapDel";
            this.toolStripMapDel.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.toolStripMapDel.Size = new System.Drawing.Size(208, 24);
            this.toolStripMapDel.Text = "Delete";
            // 
            // toolStripMapPaint
            // 
            this.toolStripMapPaint.Name = "toolStripMapPaint";
            this.toolStripMapPaint.Size = new System.Drawing.Size(208, 24);
            this.toolStripMapPaint.Text = "Paint";
            // 
            // btnMapSize
            // 
            this.btnMapSize.Font = new System.Drawing.Font("Yu Gothic UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.btnMapSize.Location = new System.Drawing.Point(465, 207);
            this.btnMapSize.Name = "btnMapSize";
            this.btnMapSize.Size = new System.Drawing.Size(65, 29);
            this.btnMapSize.TabIndex = 39;
            this.btnMapSize.Text = "Size";
            this.btnMapSize.UseVisualStyleBackColor = true;
            this.btnMapSize.Click += new System.EventHandler(this.btnMapSize_Click);
            // 
            // chkCRT
            // 
            this.chkCRT.AutoSize = true;
            this.chkCRT.Font = new System.Drawing.Font("Yu Gothic UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.chkCRT.Location = new System.Drawing.Point(996, 35);
            this.chkCRT.Name = "chkCRT";
            this.chkCRT.Size = new System.Drawing.Size(94, 24);
            this.chkCRT.TabIndex = 41;
            this.chkCRT.Text = "CRT Filter";
            this.chkCRT.UseVisualStyleBackColor = true;
            this.chkCRT.CheckedChanged += new System.EventHandler(this.chkCRT_CheckedChanged);
            // 
            // toolTipMap
            // 
            this.toolTipMap.AutomaticDelay = 0;
            // 
            // menuStripMain
            // 
            this.menuStripMain.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripBarFile,
            this.toolStripBarEdit});
            this.menuStripMain.Location = new System.Drawing.Point(0, 0);
            this.menuStripMain.Name = "menuStripMain";
            this.menuStripMain.Size = new System.Drawing.Size(1102, 28);
            this.menuStripMain.TabIndex = 44;
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
            this.toolStripFileLoadMap,
            this.toolStripFileSaveMap});
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
            this.toolStripFileImport.Enabled = false;
            this.toolStripFileImport.Name = "toolStripFileImport";
            this.toolStripFileImport.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.I)));
            this.toolStripFileImport.Size = new System.Drawing.Size(261, 26);
            this.toolStripFileImport.Text = "Import Map(&I)";
            // 
            // toolStripFileExport
            // 
            this.toolStripFileExport.Name = "toolStripFileExport";
            this.toolStripFileExport.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.E)));
            this.toolStripFileExport.Size = new System.Drawing.Size(261, 26);
            this.toolStripFileExport.Text = "Export Map(&E)";
            // 
            // toolStripFileLoadMap
            // 
            this.toolStripFileLoadMap.Name = "toolStripFileLoadMap";
            this.toolStripFileLoadMap.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.L)));
            this.toolStripFileLoadMap.Size = new System.Drawing.Size(261, 26);
            this.toolStripFileLoadMap.Text = "Load Map Data(&L)";
            // 
            // toolStripFileSaveMap
            // 
            this.toolStripFileSaveMap.Name = "toolStripFileSaveMap";
            this.toolStripFileSaveMap.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.S)));
            this.toolStripFileSaveMap.Size = new System.Drawing.Size(261, 26);
            this.toolStripFileSaveMap.Text = "Save Map Data(&S)";
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
            // viewPCG
            // 
            this.viewPCG.AllowDrop = true;
            this.viewPCG.AllowMultipleSelection = false;
            this.viewPCG.AllowSubSelection = false;
            this.viewPCG.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.viewPCG.CellHeight = 16;
            this.viewPCG.CellWidth = 16;
            this.viewPCG.ColumnNum = 32;
            this.viewPCG.ContextMenuStrip = this.contextPCG;
            this.viewPCG.DrawOverlayedSelection = false;
            this.viewPCG.DrawTranparentColor = false;
            this.viewPCG.Location = new System.Drawing.Point(14, 64);
            this.viewPCG.Name = "viewPCG";
            this.viewPCG.RowNum = 8;
            this.viewPCG.SelectionHeight = 1;
            this.viewPCG.SelectionWidth = 1;
            this.viewPCG.Size = new System.Drawing.Size(514, 130);
            this.viewPCG.TabIndex = 45;
            this.viewPCG.X = 0;
            this.viewPCG.Y = 0;
            this.viewPCG.CellDragStart += new System.EventHandler<System.EventArgs>(this.viewPCG_CellDragStart);
            // 
            // contextPCG
            // 
            this.contextPCG.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextPCG.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripPCGCopy});
            this.contextPCG.Name = "contextPCG";
            this.contextPCG.Size = new System.Drawing.Size(164, 28);
            // 
            // toolStripPCGCopy
            // 
            this.toolStripPCGCopy.Name = "toolStripPCGCopy";
            this.toolStripPCGCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.toolStripPCGCopy.Size = new System.Drawing.Size(163, 24);
            this.toolStripPCGCopy.Text = "Copy";
            // 
            // viewMap
            // 
            this.viewMap.AllowDrop = true;
            this.viewMap.AllowMultipleSelection = true;
            this.viewMap.AllowSubSelection = false;
            this.viewMap.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.viewMap.CellHeight = 16;
            this.viewMap.CellWidth = 16;
            this.viewMap.ColumnNum = 32;
            this.viewMap.ContextMenuStrip = this.contextMap;
            this.viewMap.DrawOverlayedSelection = false;
            this.viewMap.DrawTranparentColor = false;
            this.viewMap.Location = new System.Drawing.Point(14, 239);
            this.viewMap.Name = "viewMap";
            this.viewMap.RowNum = 24;
            this.viewMap.SelectionHeight = 2;
            this.viewMap.SelectionWidth = 2;
            this.viewMap.Size = new System.Drawing.Size(514, 386);
            this.viewMap.TabIndex = 46;
            this.viewMap.X = 0;
            this.viewMap.Y = 0;
            this.viewMap.MatrixOnScroll += new System.EventHandler<_99x8Edit.MatrixControl.ScrollEventArgs>(this.viewMap_MatrixOnScroll);
            this.viewMap.DragDrop += new System.Windows.Forms.DragEventHandler(this.panelMap_DragDrop);
            this.viewMap.DragEnter += new System.Windows.Forms.DragEventHandler(this.panelMap_DragEnter);
            // 
            // viewPtn
            // 
            this.viewPtn.AllowDrop = true;
            this.viewPtn.AllowMultipleSelection = true;
            this.viewPtn.AllowSubSelection = true;
            this.viewPtn.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.viewPtn.CellHeight = 16;
            this.viewPtn.CellWidth = 16;
            this.viewPtn.ColumnNum = 32;
            this.viewPtn.ContextMenuStrip = this.contextPattern;
            this.viewPtn.DrawOverlayedSelection = false;
            this.viewPtn.DrawTranparentColor = false;
            this.viewPtn.Location = new System.Drawing.Point(579, 65);
            this.viewPtn.Name = "viewPtn";
            this.viewPtn.RowNum = 32;
            this.viewPtn.SelectionHeight = 2;
            this.viewPtn.SelectionWidth = 2;
            this.viewPtn.Size = new System.Drawing.Size(514, 514);
            this.viewPtn.TabIndex = 47;
            this.viewPtn.X = 0;
            this.viewPtn.Y = 0;
            this.viewPtn.CellDragStart += new System.EventHandler<System.EventArgs>(this.viewPtn_CellDragStart);
            this.viewPtn.DragDrop += new System.Windows.Forms.DragEventHandler(this.panelPtn_DragDrop);
            this.viewPtn.DragEnter += new System.Windows.Forms.DragEventHandler(this.panelPtn_DragEnter);
            this.viewPtn.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.viewPtn_PreviewKeyDown);
            // 
            // MapEditor
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1102, 634);
            this.Controls.Add(this.viewPtn);
            this.Controls.Add(this.viewMap);
            this.Controls.Add(this.viewPCG);
            this.Controls.Add(this.menuStripMain);
            this.Controls.Add(this.chkCRT);
            this.Controls.Add(this.btnMapSize);
            this.Controls.Add(this.btnDown);
            this.Controls.Add(this.btnUp);
            this.Controls.Add(this.btnRight);
            this.Controls.Add(this.btnLeft);
            this.Controls.Add(this.txtMapY);
            this.Controls.Add(this.txtMapX);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label2);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MapEditor";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Map and Patterns";
            this.Activated += new System.EventHandler(this.Map_Activated);
            this.contextPattern.ResumeLayout(false);
            this.contextMap.ResumeLayout(false);
            this.menuStripMain.ResumeLayout(false);
            this.menuStripMain.PerformLayout();
            this.contextPCG.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtMapX;
        private System.Windows.Forms.TextBox txtMapY;
        private System.Windows.Forms.Button btnLeft;
        private System.Windows.Forms.Button btnRight;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.ContextMenuStrip contextPattern;
        private System.Windows.Forms.ToolStripMenuItem toolStripPatternCopy;
        private System.Windows.Forms.ToolStripMenuItem toolStripPatternPaste;
        private System.Windows.Forms.ContextMenuStrip contextMap;
        private System.Windows.Forms.ToolStripMenuItem toolStripMapCopy;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMapPaste;
        private System.Windows.Forms.ToolStripMenuItem toolStripMapDel;
        private System.Windows.Forms.ToolStripMenuItem toolStripMapPaint;
        private System.Windows.Forms.Button btnMapSize;
        private System.Windows.Forms.CheckBox chkCRT;
        private System.Windows.Forms.ToolStripMenuItem toolStripPatternCopyDown;
        private System.Windows.Forms.ToolStripMenuItem toolStripPatternCopyRight;
        private System.Windows.Forms.ToolStripMenuItem toolStripMapCopyDown;
        private System.Windows.Forms.ToolStripMenuItem toolStripMapCopyRight;
        private System.Windows.Forms.ToolTip toolTipMap;
        private System.Windows.Forms.MenuStrip menuStripMain;
        private System.Windows.Forms.ToolStripMenuItem toolStripBarFile;
        private System.Windows.Forms.ToolStripMenuItem toolStripFileLoad;
        private System.Windows.Forms.ToolStripMenuItem toolStripFileSave;
        private System.Windows.Forms.ToolStripMenuItem toolStripFileSaveAs;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem toolStripFileImport;
        private System.Windows.Forms.ToolStripMenuItem toolStripFileExport;
        private System.Windows.Forms.ToolStripMenuItem toolStripFileSaveMap;
        private System.Windows.Forms.ToolStripMenuItem toolStripFileLoadMap;
        private System.Windows.Forms.ToolStripMenuItem toolStripBarEdit;
        private System.Windows.Forms.ToolStripMenuItem toolStripEditUndo;
        private System.Windows.Forms.ToolStripMenuItem toolStripEditRedo;
        private MatrixControl viewPCG;
        private MatrixControl viewMap;
        private MatrixControl viewPtn;
        private System.Windows.Forms.ContextMenuStrip contextPCG;
        private System.Windows.Forms.ToolStripMenuItem toolStripPCGCopy;
    }
}