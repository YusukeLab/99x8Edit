
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
            this._txtMapX = new System.Windows.Forms.TextBox();
            this._txtMapY = new System.Windows.Forms.TextBox();
            this._btnLeft = new System.Windows.Forms.Button();
            this._btnRight = new System.Windows.Forms.Button();
            this._btnUp = new System.Windows.Forms.Button();
            this._btnDown = new System.Windows.Forms.Button();
            this._contextPattern = new System.Windows.Forms.ContextMenuStrip(this.components);
            this._toolStripPatternCopy = new System.Windows.Forms.ToolStripMenuItem();
            this._toolStripPatternPaste = new System.Windows.Forms.ToolStripMenuItem();
            this._toolStripPatternCopyDown = new System.Windows.Forms.ToolStripMenuItem();
            this._toolStripPatternCopyRight = new System.Windows.Forms.ToolStripMenuItem();
            this._contextMap = new System.Windows.Forms.ContextMenuStrip(this.components);
            this._toolStripMapCopy = new System.Windows.Forms.ToolStripMenuItem();
            this._toolStripMapPaste = new System.Windows.Forms.ToolStripMenuItem();
            this._toolStripMapCopyDown = new System.Windows.Forms.ToolStripMenuItem();
            this._toolStripMapCopyRight = new System.Windows.Forms.ToolStripMenuItem();
            this._toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this._toolStripMapDel = new System.Windows.Forms.ToolStripMenuItem();
            this._toolStripMapPaint = new System.Windows.Forms.ToolStripMenuItem();
            this._btnMapSize = new System.Windows.Forms.Button();
            this._chkCRT = new System.Windows.Forms.CheckBox();
            this._toolTipMap = new System.Windows.Forms.ToolTip(this.components);
            this._menuStripMain = new System.Windows.Forms.MenuStrip();
            this._toolStripBarFile = new System.Windows.Forms.ToolStripMenuItem();
            this._toolStripFileLoad = new System.Windows.Forms.ToolStripMenuItem();
            this._toolStripFileSave = new System.Windows.Forms.ToolStripMenuItem();
            this._toolStripFileSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this._toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this._toolStripFileImport = new System.Windows.Forms.ToolStripMenuItem();
            this._toolStripFileExport = new System.Windows.Forms.ToolStripMenuItem();
            this._toolStripFileLoadMap = new System.Windows.Forms.ToolStripMenuItem();
            this._toolStripFileSaveMap = new System.Windows.Forms.ToolStripMenuItem();
            this._toolStripBarEdit = new System.Windows.Forms.ToolStripMenuItem();
            this._toolStripEditUndo = new System.Windows.Forms.ToolStripMenuItem();
            this._toolStripEditRedo = new System.Windows.Forms.ToolStripMenuItem();
            this._viewPCG = new _99x8Edit.MatrixControl();
            this._contextPCG = new System.Windows.Forms.ContextMenuStrip(this.components);
            this._toolStripPCGCopy = new System.Windows.Forms.ToolStripMenuItem();
            this._viewMap = new _99x8Edit.MatrixControl();
            this._viewPtn = new _99x8Edit.MatrixControl();
            this._contextPattern.SuspendLayout();
            this._contextMap.SuspendLayout();
            this._menuStripMain.SuspendLayout();
            this._contextPCG.SuspendLayout();
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
            // _txtMapX
            // 
            this._txtMapX.Enabled = false;
            this._txtMapX.Font = new System.Drawing.Font("Yu Gothic UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this._txtMapX.Location = new System.Drawing.Point(261, 208);
            this._txtMapX.Name = "_txtMapX";
            this._txtMapX.Size = new System.Drawing.Size(41, 27);
            this._txtMapX.TabIndex = 29;
            this._txtMapX.Text = "0";
            this._txtMapX.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // _txtMapY
            // 
            this._txtMapY.Enabled = false;
            this._txtMapY.Font = new System.Drawing.Font("Yu Gothic UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this._txtMapY.Location = new System.Drawing.Point(529, 407);
            this._txtMapY.Name = "_txtMapY";
            this._txtMapY.Size = new System.Drawing.Size(37, 27);
            this._txtMapY.TabIndex = 30;
            this._txtMapY.Text = "0";
            this._txtMapY.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // _btnLeft
            // 
            this._btnLeft.Location = new System.Drawing.Point(226, 207);
            this._btnLeft.Name = "_btnLeft";
            this._btnLeft.Size = new System.Drawing.Size(29, 29);
            this._btnLeft.TabIndex = 31;
            this._btnLeft.Text = "←";
            this._btnLeft.UseVisualStyleBackColor = true;
            this._btnLeft.Click += new System.EventHandler(this.btnLeft_Click);
            // 
            // _btnRight
            // 
            this._btnRight.Location = new System.Drawing.Point(307, 208);
            this._btnRight.Name = "_btnRight";
            this._btnRight.Size = new System.Drawing.Size(29, 29);
            this._btnRight.TabIndex = 32;
            this._btnRight.Text = "→";
            this._btnRight.UseVisualStyleBackColor = true;
            this._btnRight.Click += new System.EventHandler(this.btnRight_Click);
            // 
            // _btnUp
            // 
            this._btnUp.Location = new System.Drawing.Point(533, 374);
            this._btnUp.Name = "_btnUp";
            this._btnUp.Size = new System.Drawing.Size(29, 29);
            this._btnUp.TabIndex = 33;
            this._btnUp.Text = "↑";
            this._btnUp.UseVisualStyleBackColor = true;
            this._btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // _btnDown
            // 
            this._btnDown.Location = new System.Drawing.Point(533, 442);
            this._btnDown.Name = "_btnDown";
            this._btnDown.Size = new System.Drawing.Size(29, 29);
            this._btnDown.TabIndex = 34;
            this._btnDown.Text = "↓";
            this._btnDown.UseVisualStyleBackColor = true;
            this._btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // _contextPattern
            // 
            this._contextPattern.ImageScalingSize = new System.Drawing.Size(20, 20);
            this._contextPattern.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._toolStripPatternCopy,
            this._toolStripPatternPaste,
            this._toolStripPatternCopyDown,
            this._toolStripPatternCopyRight});
            this._contextPattern.Name = "contextPCGList";
            this._contextPattern.Size = new System.Drawing.Size(209, 100);
            // 
            // _toolStripPatternCopy
            // 
            this._toolStripPatternCopy.Name = "_toolStripPatternCopy";
            this._toolStripPatternCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this._toolStripPatternCopy.Size = new System.Drawing.Size(208, 24);
            this._toolStripPatternCopy.Text = "Copy";
            // 
            // _toolStripPatternPaste
            // 
            this._toolStripPatternPaste.Name = "_toolStripPatternPaste";
            this._toolStripPatternPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this._toolStripPatternPaste.Size = new System.Drawing.Size(208, 24);
            this._toolStripPatternPaste.Text = "Paste";
            // 
            // _toolStripPatternCopyDown
            // 
            this._toolStripPatternCopyDown.Name = "_toolStripPatternCopyDown";
            this._toolStripPatternCopyDown.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this._toolStripPatternCopyDown.Size = new System.Drawing.Size(208, 24);
            this._toolStripPatternCopyDown.Text = "Copy Down";
            // 
            // _toolStripPatternCopyRight
            // 
            this._toolStripPatternCopyRight.Name = "_toolStripPatternCopyRight";
            this._toolStripPatternCopyRight.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this._toolStripPatternCopyRight.Size = new System.Drawing.Size(208, 24);
            this._toolStripPatternCopyRight.Text = "Copy Right";
            // 
            // _contextMap
            // 
            this._contextMap.ImageScalingSize = new System.Drawing.Size(20, 20);
            this._contextMap.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._toolStripMapCopy,
            this._toolStripMapPaste,
            this._toolStripMapCopyDown,
            this._toolStripMapCopyRight,
            this._toolStripSeparator2,
            this._toolStripMapDel,
            this._toolStripMapPaint});
            this._contextMap.Name = "contextPCGList";
            this._contextMap.Size = new System.Drawing.Size(209, 154);
            // 
            // _toolStripMapCopy
            // 
            this._toolStripMapCopy.Name = "_toolStripMapCopy";
            this._toolStripMapCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this._toolStripMapCopy.Size = new System.Drawing.Size(208, 24);
            this._toolStripMapCopy.Text = "Copy";
            // 
            // _toolStripMapPaste
            // 
            this._toolStripMapPaste.Name = "_toolStripMapPaste";
            this._toolStripMapPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this._toolStripMapPaste.Size = new System.Drawing.Size(208, 24);
            this._toolStripMapPaste.Text = "Paste";
            // 
            // _toolStripMapCopyDown
            // 
            this._toolStripMapCopyDown.Name = "_toolStripMapCopyDown";
            this._toolStripMapCopyDown.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this._toolStripMapCopyDown.Size = new System.Drawing.Size(208, 24);
            this._toolStripMapCopyDown.Text = "Copy Down";
            // 
            // _toolStripMapCopyRight
            // 
            this._toolStripMapCopyRight.Name = "_toolStripMapCopyRight";
            this._toolStripMapCopyRight.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this._toolStripMapCopyRight.Size = new System.Drawing.Size(208, 24);
            this._toolStripMapCopyRight.Text = "Copy Right";
            // 
            // _toolStripSeparator2
            // 
            this._toolStripSeparator2.Name = "_toolStripSeparator2";
            this._toolStripSeparator2.Size = new System.Drawing.Size(205, 6);
            // 
            // _toolStripMapDel
            // 
            this._toolStripMapDel.Name = "_toolStripMapDel";
            this._toolStripMapDel.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this._toolStripMapDel.Size = new System.Drawing.Size(208, 24);
            this._toolStripMapDel.Text = "Delete";
            // 
            // _toolStripMapPaint
            // 
            this._toolStripMapPaint.Name = "_toolStripMapPaint";
            this._toolStripMapPaint.Size = new System.Drawing.Size(208, 24);
            this._toolStripMapPaint.Text = "Paint";
            // 
            // _btnMapSize
            // 
            this._btnMapSize.Font = new System.Drawing.Font("Yu Gothic UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this._btnMapSize.Location = new System.Drawing.Point(465, 207);
            this._btnMapSize.Name = "_btnMapSize";
            this._btnMapSize.Size = new System.Drawing.Size(65, 29);
            this._btnMapSize.TabIndex = 39;
            this._btnMapSize.Text = "Size";
            this._btnMapSize.UseVisualStyleBackColor = true;
            this._btnMapSize.Click += new System.EventHandler(this.btnMapSize_Click);
            // 
            // _chkCRT
            // 
            this._chkCRT.AutoSize = true;
            this._chkCRT.Font = new System.Drawing.Font("Yu Gothic UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this._chkCRT.Location = new System.Drawing.Point(996, 35);
            this._chkCRT.Name = "_chkCRT";
            this._chkCRT.Size = new System.Drawing.Size(94, 24);
            this._chkCRT.TabIndex = 41;
            this._chkCRT.Text = "CRT Filter";
            this._chkCRT.UseVisualStyleBackColor = true;
            this._chkCRT.CheckedChanged += new System.EventHandler(this.chkCRT_CheckedChanged);
            // 
            // _toolTipMap
            // 
            this._toolTipMap.AutomaticDelay = 0;
            // 
            // _menuStripMain
            // 
            this._menuStripMain.ImageScalingSize = new System.Drawing.Size(20, 20);
            this._menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._toolStripBarFile,
            this._toolStripBarEdit});
            this._menuStripMain.Location = new System.Drawing.Point(0, 0);
            this._menuStripMain.Name = "_menuStripMain";
            this._menuStripMain.Size = new System.Drawing.Size(1102, 28);
            this._menuStripMain.TabIndex = 44;
            this._menuStripMain.Text = "menuStrip1";
            // 
            // _toolStripBarFile
            // 
            this._toolStripBarFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._toolStripFileLoad,
            this._toolStripFileSave,
            this._toolStripFileSaveAs,
            this._toolStripSeparator4,
            this._toolStripFileImport,
            this._toolStripFileExport,
            this._toolStripFileLoadMap,
            this._toolStripFileSaveMap});
            this._toolStripBarFile.Name = "_toolStripBarFile";
            this._toolStripBarFile.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F)));
            this._toolStripBarFile.Size = new System.Drawing.Size(63, 24);
            this._toolStripBarFile.Text = "File(&F)";
            // 
            // _toolStripFileLoad
            // 
            this._toolStripFileLoad.Name = "_toolStripFileLoad";
            this._toolStripFileLoad.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.O)));
            this._toolStripFileLoad.Size = new System.Drawing.Size(261, 26);
            this._toolStripFileLoad.Text = "Open Project(&O)";
            // 
            // _toolStripFileSave
            // 
            this._toolStripFileSave.Name = "_toolStripFileSave";
            this._toolStripFileSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this._toolStripFileSave.Size = new System.Drawing.Size(261, 26);
            this._toolStripFileSave.Text = "Save Project";
            // 
            // _toolStripFileSaveAs
            // 
            this._toolStripFileSaveAs.Name = "_toolStripFileSaveAs";
            this._toolStripFileSaveAs.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.A)));
            this._toolStripFileSaveAs.Size = new System.Drawing.Size(261, 26);
            this._toolStripFileSaveAs.Text = "Save Project As(&A)";
            // 
            // _toolStripSeparator4
            // 
            this._toolStripSeparator4.Name = "_toolStripSeparator4";
            this._toolStripSeparator4.Size = new System.Drawing.Size(258, 6);
            // 
            // _toolStripFileImport
            // 
            this._toolStripFileImport.Enabled = false;
            this._toolStripFileImport.Name = "_toolStripFileImport";
            this._toolStripFileImport.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.I)));
            this._toolStripFileImport.Size = new System.Drawing.Size(261, 26);
            this._toolStripFileImport.Text = "Import Map(&I)";
            // 
            // _toolStripFileExport
            // 
            this._toolStripFileExport.Name = "_toolStripFileExport";
            this._toolStripFileExport.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.E)));
            this._toolStripFileExport.Size = new System.Drawing.Size(261, 26);
            this._toolStripFileExport.Text = "Export Map(&E)";
            // 
            // _toolStripFileLoadMap
            // 
            this._toolStripFileLoadMap.Name = "_toolStripFileLoadMap";
            this._toolStripFileLoadMap.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.L)));
            this._toolStripFileLoadMap.Size = new System.Drawing.Size(261, 26);
            this._toolStripFileLoadMap.Text = "Load Map Data(&L)";
            // 
            // _toolStripFileSaveMap
            // 
            this._toolStripFileSaveMap.Name = "_toolStripFileSaveMap";
            this._toolStripFileSaveMap.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.S)));
            this._toolStripFileSaveMap.Size = new System.Drawing.Size(261, 26);
            this._toolStripFileSaveMap.Text = "Save Map Data(&S)";
            // 
            // _toolStripBarEdit
            // 
            this._toolStripBarEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._toolStripEditUndo,
            this._toolStripEditRedo});
            this._toolStripBarEdit.Name = "_toolStripBarEdit";
            this._toolStripBarEdit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.E)));
            this._toolStripBarEdit.Size = new System.Drawing.Size(67, 24);
            this._toolStripBarEdit.Text = "Edit(&E)";
            // 
            // _toolStripEditUndo
            // 
            this._toolStripEditUndo.Name = "_toolStripEditUndo";
            this._toolStripEditUndo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this._toolStripEditUndo.Size = new System.Drawing.Size(179, 26);
            this._toolStripEditUndo.Text = "Undo";
            // 
            // _toolStripEditRedo
            // 
            this._toolStripEditRedo.Name = "_toolStripEditRedo";
            this._toolStripEditRedo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this._toolStripEditRedo.Size = new System.Drawing.Size(179, 26);
            this._toolStripEditRedo.Text = "Redo";
            // 
            // _viewPCG
            // 
            this._viewPCG.AllowDrop = true;
            this._viewPCG.AllowMultipleSelection = false;
            this._viewPCG.AllowOneStrokeEditing = false;
            this._viewPCG.AllowSelection = true;
            this._viewPCG.AllowSubSelection = false;
            this._viewPCG.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._viewPCG.CellHeight = 16;
            this._viewPCG.CellWidth = 16;
            this._viewPCG.ColumnNum = 32;
            this._viewPCG.ContextMenuStrip = this._contextPCG;
            this._viewPCG.DrawOverlayedSelection = false;
            this._viewPCG.DrawTranparentColor = false;
            this._viewPCG.Index = 0;
            this._viewPCG.Location = new System.Drawing.Point(14, 64);
            this._viewPCG.Name = "_viewPCG";
            this._viewPCG.RowNum = 8;
            this._viewPCG.SelectionHeight = 1;
            this._viewPCG.SelectionWidth = 1;
            this._viewPCG.Size = new System.Drawing.Size(514, 130);
            this._viewPCG.TabIndex = 45;
            this._viewPCG.X = 0;
            this._viewPCG.Y = 0;
            this._viewPCG.CellDragStart += new System.EventHandler<System.EventArgs>(this.viewPCG_CellDragStart);
            // 
            // _contextPCG
            // 
            this._contextPCG.ImageScalingSize = new System.Drawing.Size(20, 20);
            this._contextPCG.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._toolStripPCGCopy});
            this._contextPCG.Name = "contextPCG";
            this._contextPCG.Size = new System.Drawing.Size(164, 28);
            // 
            // _toolStripPCGCopy
            // 
            this._toolStripPCGCopy.Name = "_toolStripPCGCopy";
            this._toolStripPCGCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this._toolStripPCGCopy.Size = new System.Drawing.Size(163, 24);
            this._toolStripPCGCopy.Text = "Copy";
            // 
            // _viewMap
            // 
            this._viewMap.AllowDrop = true;
            this._viewMap.AllowMultipleSelection = true;
            this._viewMap.AllowOneStrokeEditing = false;
            this._viewMap.AllowSelection = true;
            this._viewMap.AllowSubSelection = false;
            this._viewMap.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._viewMap.CellHeight = 16;
            this._viewMap.CellWidth = 16;
            this._viewMap.ColumnNum = 32;
            this._viewMap.ContextMenuStrip = this._contextMap;
            this._viewMap.DrawOverlayedSelection = false;
            this._viewMap.DrawTranparentColor = false;
            this._viewMap.Index = 0;
            this._viewMap.Location = new System.Drawing.Point(14, 239);
            this._viewMap.Name = "_viewMap";
            this._viewMap.RowNum = 24;
            this._viewMap.SelectionHeight = 2;
            this._viewMap.SelectionWidth = 2;
            this._viewMap.Size = new System.Drawing.Size(514, 386);
            this._viewMap.TabIndex = 46;
            this._viewMap.X = 0;
            this._viewMap.Y = 0;
            this._viewMap.MatrixOnScroll += new System.EventHandler<_99x8Edit.MatrixControl.ScrollEventArgs>(this.viewMap_MatrixOnScroll);
            this._viewMap.DragDrop += new System.Windows.Forms.DragEventHandler(this.panelMap_DragDrop);
            this._viewMap.DragEnter += new System.Windows.Forms.DragEventHandler(this.panelMap_DragEnter);
            // 
            // _viewPtn
            // 
            this._viewPtn.AllowDrop = true;
            this._viewPtn.AllowMultipleSelection = true;
            this._viewPtn.AllowOneStrokeEditing = false;
            this._viewPtn.AllowSelection = true;
            this._viewPtn.AllowSubSelection = true;
            this._viewPtn.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._viewPtn.CellHeight = 16;
            this._viewPtn.CellWidth = 16;
            this._viewPtn.ColumnNum = 32;
            this._viewPtn.ContextMenuStrip = this._contextPattern;
            this._viewPtn.DrawOverlayedSelection = false;
            this._viewPtn.DrawTranparentColor = false;
            this._viewPtn.Index = 0;
            this._viewPtn.Location = new System.Drawing.Point(579, 65);
            this._viewPtn.Name = "_viewPtn";
            this._viewPtn.RowNum = 32;
            this._viewPtn.SelectionHeight = 2;
            this._viewPtn.SelectionWidth = 2;
            this._viewPtn.Size = new System.Drawing.Size(514, 514);
            this._viewPtn.TabIndex = 47;
            this._viewPtn.X = 0;
            this._viewPtn.Y = 0;
            this._viewPtn.CellDragStart += new System.EventHandler<System.EventArgs>(this.viewPtn_CellDragStart);
            this._viewPtn.DragDrop += new System.Windows.Forms.DragEventHandler(this.panelPtn_DragDrop);
            this._viewPtn.DragEnter += new System.Windows.Forms.DragEventHandler(this.panelPtn_DragEnter);
            this._viewPtn.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.viewPtn_PreviewKeyDown);
            // 
            // MapEditor
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1102, 634);
            this.Controls.Add(this._viewPtn);
            this.Controls.Add(this._viewMap);
            this.Controls.Add(this._viewPCG);
            this.Controls.Add(this._menuStripMain);
            this.Controls.Add(this._chkCRT);
            this.Controls.Add(this._btnMapSize);
            this.Controls.Add(this._btnDown);
            this.Controls.Add(this._btnUp);
            this.Controls.Add(this._btnRight);
            this.Controls.Add(this._btnLeft);
            this.Controls.Add(this._txtMapY);
            this.Controls.Add(this._txtMapX);
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
            this._contextPattern.ResumeLayout(false);
            this._contextMap.ResumeLayout(false);
            this._menuStripMain.ResumeLayout(false);
            this._menuStripMain.PerformLayout();
            this._contextPCG.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox _txtMapX;
        private System.Windows.Forms.TextBox _txtMapY;
        private System.Windows.Forms.Button _btnLeft;
        private System.Windows.Forms.Button _btnRight;
        private System.Windows.Forms.Button _btnUp;
        private System.Windows.Forms.Button _btnDown;
        private System.Windows.Forms.ContextMenuStrip _contextPattern;
        private System.Windows.Forms.ToolStripMenuItem _toolStripPatternCopy;
        private System.Windows.Forms.ContextMenuStrip _contextMap;
        private System.Windows.Forms.ToolStripMenuItem _toolStripMapCopy;
        private System.Windows.Forms.Button _btnMapSize;
        private System.Windows.Forms.CheckBox _chkCRT;
        private System.Windows.Forms.ToolTip _toolTipMap;
        private System.Windows.Forms.MenuStrip _menuStripMain;
        private System.Windows.Forms.ToolStripMenuItem _toolStripBarFile;
        private MatrixControl _viewPCG;
        private MatrixControl _viewMap;
        private MatrixControl _viewPtn;
        private System.Windows.Forms.ContextMenuStrip _contextPCG;
        private System.Windows.Forms.ToolStripMenuItem _toolStripPCGCopy;
        private System.Windows.Forms.ToolStripMenuItem _toolStripFileLoad;
        private System.Windows.Forms.ToolStripMenuItem _toolStripFileSave;
        private System.Windows.Forms.ToolStripMenuItem _toolStripFileSaveAs;
        private System.Windows.Forms.ToolStripSeparator _toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem _toolStripFileImport;
        private System.Windows.Forms.ToolStripMenuItem _toolStripFileExport;
        private System.Windows.Forms.ToolStripMenuItem _toolStripFileLoadMap;
        private System.Windows.Forms.ToolStripMenuItem _toolStripFileSaveMap;
        private System.Windows.Forms.ToolStripMenuItem _toolStripBarEdit;
        private System.Windows.Forms.ToolStripMenuItem _toolStripEditUndo;
        private System.Windows.Forms.ToolStripMenuItem _toolStripEditRedo;
        private System.Windows.Forms.ToolStripMenuItem _toolStripMapPaste;
        private System.Windows.Forms.ToolStripMenuItem _toolStripMapCopyDown;
        private System.Windows.Forms.ToolStripMenuItem _toolStripMapCopyRight;
        private System.Windows.Forms.ToolStripSeparator _toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem _toolStripMapDel;
        private System.Windows.Forms.ToolStripMenuItem _toolStripMapPaint;
        private System.Windows.Forms.ToolStripMenuItem _toolStripPatternPaste;
        private System.Windows.Forms.ToolStripMenuItem _toolStripPatternCopyDown;
        private System.Windows.Forms.ToolStripMenuItem _toolStripPatternCopyRight;
    }
}