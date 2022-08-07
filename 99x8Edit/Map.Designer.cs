
namespace _99x8Edit
{
    partial class Map
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
            this.viewPatterns = new System.Windows.Forms.PictureBox();
            this.label7 = new System.Windows.Forms.Label();
            this.viewPCG = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.viewMap = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtMapX = new System.Windows.Forms.TextBox();
            this.txtMapY = new System.Windows.Forms.TextBox();
            this.btnLeft = new System.Windows.Forms.Button();
            this.btnRight = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.panelPatterns = new System.Windows.Forms.FlowLayoutPanel();
            this.contextPattern = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripPatternCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripPatternPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMap = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMapCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMapPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMapDel = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMapPaint = new System.Windows.Forms.ToolStripMenuItem();
            this.panelMap = new System.Windows.Forms.FlowLayoutPanel();
            this.btnMapSize = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.viewPatterns)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewPCG)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewMap)).BeginInit();
            this.panelPatterns.SuspendLayout();
            this.contextPattern.SuspendLayout();
            this.contextMap.SuspendLayout();
            this.panelMap.SuspendLayout();
            this.SuspendLayout();
            // 
            // viewPatterns
            // 
            this.viewPatterns.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.viewPatterns.Location = new System.Drawing.Point(0, 0);
            this.viewPatterns.Margin = new System.Windows.Forms.Padding(0);
            this.viewPatterns.Name = "viewPatterns";
            this.viewPatterns.Size = new System.Drawing.Size(514, 514);
            this.viewPatterns.TabIndex = 24;
            this.viewPatterns.TabStop = false;
            this.viewPatterns.MouseDown += new System.Windows.Forms.MouseEventHandler(this.viewPatterns_MouseDown);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(577, 6);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(90, 20);
            this.label7.TabIndex = 23;
            this.label7.Text = "Tile Patterns";
            // 
            // viewPCG
            // 
            this.viewPCG.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.viewPCG.Location = new System.Drawing.Point(14, 32);
            this.viewPCG.Margin = new System.Windows.Forms.Padding(0);
            this.viewPCG.Name = "viewPCG";
            this.viewPCG.Size = new System.Drawing.Size(514, 130);
            this.viewPCG.TabIndex = 22;
            this.viewPCG.TabStop = false;
            this.viewPCG.MouseDown += new System.Windows.Forms.MouseEventHandler(this.viewPCG_MouseDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 20);
            this.label2.TabIndex = 21;
            this.label2.Text = "PCG";
            // 
            // viewMap
            // 
            this.viewMap.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.viewMap.Location = new System.Drawing.Point(0, 0);
            this.viewMap.Margin = new System.Windows.Forms.Padding(0);
            this.viewMap.Name = "viewMap";
            this.viewMap.Size = new System.Drawing.Size(514, 386);
            this.viewMap.TabIndex = 28;
            this.viewMap.TabStop = false;
            this.viewMap.MouseClick += new System.Windows.Forms.MouseEventHandler(this.viewMap_MouseClick);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 180);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 20);
            this.label3.TabIndex = 27;
            this.label3.Text = "Map Editor";
            // 
            // txtMapX
            // 
            this.txtMapX.Enabled = false;
            this.txtMapX.Location = new System.Drawing.Point(261, 175);
            this.txtMapX.Name = "txtMapX";
            this.txtMapX.Size = new System.Drawing.Size(41, 27);
            this.txtMapX.TabIndex = 29;
            this.txtMapX.Text = "0";
            this.txtMapX.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtMapY
            // 
            this.txtMapY.Enabled = false;
            this.txtMapY.Location = new System.Drawing.Point(529, 374);
            this.txtMapY.Name = "txtMapY";
            this.txtMapY.Size = new System.Drawing.Size(37, 27);
            this.txtMapY.TabIndex = 30;
            this.txtMapY.Text = "0";
            this.txtMapY.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnLeft
            // 
            this.btnLeft.Location = new System.Drawing.Point(226, 174);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(29, 29);
            this.btnLeft.TabIndex = 31;
            this.btnLeft.Text = "←";
            this.btnLeft.UseVisualStyleBackColor = true;
            this.btnLeft.Click += new System.EventHandler(this.btnLeft_Click);
            // 
            // btnRight
            // 
            this.btnRight.Location = new System.Drawing.Point(307, 175);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(29, 29);
            this.btnRight.TabIndex = 32;
            this.btnRight.Text = "→";
            this.btnRight.UseVisualStyleBackColor = true;
            this.btnRight.Click += new System.EventHandler(this.btnRight_Click);
            // 
            // btnUp
            // 
            this.btnUp.Location = new System.Drawing.Point(533, 341);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(29, 29);
            this.btnUp.TabIndex = 33;
            this.btnUp.Text = "↑";
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnDown
            // 
            this.btnDown.Location = new System.Drawing.Point(533, 409);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(29, 29);
            this.btnDown.TabIndex = 34;
            this.btnDown.Text = "↓";
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // panelPatterns
            // 
            this.panelPatterns.AllowDrop = true;
            this.panelPatterns.ContextMenuStrip = this.contextPattern;
            this.panelPatterns.Controls.Add(this.viewPatterns);
            this.panelPatterns.Location = new System.Drawing.Point(579, 32);
            this.panelPatterns.Name = "panelPatterns";
            this.panelPatterns.Size = new System.Drawing.Size(524, 560);
            this.panelPatterns.TabIndex = 36;
            this.panelPatterns.DragDrop += new System.Windows.Forms.DragEventHandler(this.panelPatterns_DragDrop);
            this.panelPatterns.DragEnter += new System.Windows.Forms.DragEventHandler(this.panelPatterns_DragEnter);
            // 
            // contextPattern
            // 
            this.contextPattern.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextPattern.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripPatternCopy,
            this.toolStripPatternPaste});
            this.contextPattern.Name = "contextPCGList";
            this.contextPattern.Size = new System.Drawing.Size(165, 52);
            // 
            // toolStripPatternCopy
            // 
            this.toolStripPatternCopy.Name = "toolStripPatternCopy";
            this.toolStripPatternCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.toolStripPatternCopy.Size = new System.Drawing.Size(164, 24);
            this.toolStripPatternCopy.Text = "Copy";
            // 
            // toolStripPatternPaste
            // 
            this.toolStripPatternPaste.Name = "toolStripPatternPaste";
            this.toolStripPatternPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.toolStripPatternPaste.Size = new System.Drawing.Size(164, 24);
            this.toolStripPatternPaste.Text = "Paste";
            // 
            // contextMap
            // 
            this.contextMap.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMap.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMapCopy,
            this.toolStripMapPaste,
            this.toolStripSeparator2,
            this.toolStripMapDel,
            this.toolStripMapPaint});
            this.contextMap.Name = "contextPCGList";
            this.contextMap.Size = new System.Drawing.Size(176, 106);
            // 
            // toolStripMapCopy
            // 
            this.toolStripMapCopy.Name = "toolStripMapCopy";
            this.toolStripMapCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.toolStripMapCopy.Size = new System.Drawing.Size(175, 24);
            this.toolStripMapCopy.Text = "Copy";
            // 
            // toolStripMapPaste
            // 
            this.toolStripMapPaste.Name = "toolStripMapPaste";
            this.toolStripMapPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.toolStripMapPaste.Size = new System.Drawing.Size(175, 24);
            this.toolStripMapPaste.Text = "Paste";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(172, 6);
            // 
            // toolStripMapDel
            // 
            this.toolStripMapDel.Name = "toolStripMapDel";
            this.toolStripMapDel.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.toolStripMapDel.Size = new System.Drawing.Size(175, 24);
            this.toolStripMapDel.Text = "Delete";
            // 
            // toolStripMapPaint
            // 
            this.toolStripMapPaint.Name = "toolStripMapPaint";
            this.toolStripMapPaint.Size = new System.Drawing.Size(175, 24);
            this.toolStripMapPaint.Text = "Paint";
            // 
            // panelMap
            // 
            this.panelMap.AllowDrop = true;
            this.panelMap.ContextMenuStrip = this.contextMap;
            this.panelMap.Controls.Add(this.viewMap);
            this.panelMap.Location = new System.Drawing.Point(14, 206);
            this.panelMap.Name = "panelMap";
            this.panelMap.Size = new System.Drawing.Size(519, 386);
            this.panelMap.TabIndex = 38;
            this.panelMap.DragDrop += new System.Windows.Forms.DragEventHandler(this.panelMap_DragDrop);
            this.panelMap.DragEnter += new System.Windows.Forms.DragEventHandler(this.panelMap_DragEnter);
            // 
            // btnMapSize
            // 
            this.btnMapSize.Location = new System.Drawing.Point(465, 174);
            this.btnMapSize.Name = "btnMapSize";
            this.btnMapSize.Size = new System.Drawing.Size(65, 29);
            this.btnMapSize.TabIndex = 39;
            this.btnMapSize.Text = "Size";
            this.btnMapSize.UseVisualStyleBackColor = true;
            this.btnMapSize.Click += new System.EventHandler(this.btnMapSize_Click);
            // 
            // Map
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1102, 598);
            this.Controls.Add(this.btnMapSize);
            this.Controls.Add(this.panelMap);
            this.Controls.Add(this.panelPatterns);
            this.Controls.Add(this.btnDown);
            this.Controls.Add(this.btnUp);
            this.Controls.Add(this.btnRight);
            this.Controls.Add(this.btnLeft);
            this.Controls.Add(this.txtMapY);
            this.Controls.Add(this.txtMapX);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.viewPCG);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Map";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Map and Patterns";
            this.Activated += new System.EventHandler(this.Map_Activated);
            ((System.ComponentModel.ISupportInitialize)(this.viewPatterns)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewPCG)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewMap)).EndInit();
            this.panelPatterns.ResumeLayout(false);
            this.contextPattern.ResumeLayout(false);
            this.contextMap.ResumeLayout(false);
            this.panelMap.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox viewPatterns;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.PictureBox viewPCG;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox viewMap;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtMapX;
        private System.Windows.Forms.TextBox txtMapY;
        private System.Windows.Forms.Button btnLeft;
        private System.Windows.Forms.Button btnRight;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.FlowLayoutPanel panelPatterns;
        private System.Windows.Forms.ContextMenuStrip contextPattern;
        private System.Windows.Forms.ToolStripMenuItem toolStripPatternCopy;
        private System.Windows.Forms.ToolStripMenuItem toolStripPatternPaste;
        private System.Windows.Forms.ContextMenuStrip contextMap;
        private System.Windows.Forms.ToolStripMenuItem toolStripMapCopy;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.FlowLayoutPanel panelMap;
        private System.Windows.Forms.ToolStripMenuItem toolStripMapPaste;
        private System.Windows.Forms.ToolStripMenuItem toolStripMapDel;
        private System.Windows.Forms.ToolStripMenuItem toolStripMapPaint;
        private System.Windows.Forms.Button btnMapSize;
    }
}