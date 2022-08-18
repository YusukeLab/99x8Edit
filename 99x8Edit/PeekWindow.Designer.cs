
namespace _99x8Edit
{
    partial class PeekWindow
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
            this.btnLinear = new System.Windows.Forms.RadioButton();
            this.btnSprites = new System.Windows.Forms.RadioButton();
            this.contextPeek = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.txtAddr = new System.Windows.Forms.TextBox();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.toolTipPeek = new System.Windows.Forms.ToolTip(this.components);
            this.viewPeek = new _99x8Edit.MatrixControl();
            this.contextPeek.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnLinear
            // 
            this.btnLinear.AutoSize = true;
            this.btnLinear.Font = new System.Drawing.Font("Yu Gothic UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.btnLinear.Location = new System.Drawing.Point(439, 11);
            this.btnLinear.Name = "btnLinear";
            this.btnLinear.Size = new System.Drawing.Size(70, 24);
            this.btnLinear.TabIndex = 0;
            this.btnLinear.TabStop = true;
            this.btnLinear.Text = "Linear";
            this.btnLinear.UseVisualStyleBackColor = true;
            this.btnLinear.Click += new System.EventHandler(this.btnLinear_Click);
            // 
            // btnSprites
            // 
            this.btnSprites.AutoSize = true;
            this.btnSprites.Font = new System.Drawing.Font("Yu Gothic UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.btnSprites.Location = new System.Drawing.Point(439, 36);
            this.btnSprites.Name = "btnSprites";
            this.btnSprites.Size = new System.Drawing.Size(69, 24);
            this.btnSprites.TabIndex = 1;
            this.btnSprites.TabStop = true;
            this.btnSprites.Text = "16x16";
            this.btnSprites.UseVisualStyleBackColor = true;
            this.btnSprites.Click += new System.EventHandler(this.btnSprites_Click);
            // 
            // contextPeek
            // 
            this.contextPeek.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextPeek.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripCopy});
            this.contextPeek.Name = "contextPeek";
            this.contextPeek.Size = new System.Drawing.Size(164, 28);
            // 
            // toolStripCopy
            // 
            this.toolStripCopy.Name = "toolStripCopy";
            this.toolStripCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.toolStripCopy.Size = new System.Drawing.Size(163, 24);
            this.toolStripCopy.Text = "Copy";
            // 
            // txtAddr
            // 
            this.txtAddr.Font = new System.Drawing.Font("Yu Gothic UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.txtAddr.Location = new System.Drawing.Point(223, 30);
            this.txtAddr.Name = "txtAddr";
            this.txtAddr.Size = new System.Drawing.Size(77, 27);
            this.txtAddr.TabIndex = 3;
            this.txtAddr.Text = "4000";
            this.txtAddr.Leave += new System.EventHandler(this.txtAddr_Leave);
            // 
            // btnUp
            // 
            this.btnUp.Location = new System.Drawing.Point(180, 30);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(33, 29);
            this.btnUp.TabIndex = 4;
            this.btnUp.Text = "<";
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnDown
            // 
            this.btnDown.Location = new System.Drawing.Point(311, 29);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(33, 29);
            this.btnDown.TabIndex = 5;
            this.btnDown.Text = ">";
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // toolTipPeek
            // 
            this.toolTipPeek.AutomaticDelay = 0;
            // 
            // viewPeek
            // 
            this.viewPeek.AllowDrop = true;
            this.viewPeek.AllowMultipleSelection = true;
            this.viewPeek.AllowSubSelection = false;
            this.viewPeek.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.viewPeek.CellHeight = 16;
            this.viewPeek.CellWidth = 16;
            this.viewPeek.ColumnNum = 32;
            this.viewPeek.ContextMenuStrip = this.contextPeek;
            this.viewPeek.DrawOverlayedSelection = false;
            this.viewPeek.DrawTranparentColor = false;
            this.viewPeek.Location = new System.Drawing.Point(8, 65);
            this.viewPeek.Name = "viewPeek";
            this.viewPeek.RowNum = 32;
            this.viewPeek.SelectionHeight = 2;
            this.viewPeek.SelectionWidth = 2;
            this.viewPeek.Size = new System.Drawing.Size(514, 514);
            this.viewPeek.TabIndex = 48;
            this.viewPeek.X = 0;
            this.viewPeek.Y = 0;
            this.viewPeek.MatrixOnScroll += new System.EventHandler<_99x8Edit.MatrixControl.ScrollEventArgs>(this.viewPtn_MatrixOnScroll);
            // 
            // PeekWindow
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(532, 586);
            this.Controls.Add(this.viewPeek);
            this.Controls.Add(this.btnDown);
            this.Controls.Add(this.btnUp);
            this.Controls.Add(this.txtAddr);
            this.Controls.Add(this.btnSprites);
            this.Controls.Add(this.btnLinear);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PeekWindow";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Peek binaries";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Peek_FormClosing);
            this.contextPeek.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton btnLinear;
        private System.Windows.Forms.RadioButton btnSprites;
        private System.Windows.Forms.TextBox txtAddr;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.ContextMenuStrip contextPeek;
        private System.Windows.Forms.ToolStripMenuItem toolStripCopy;
        private System.Windows.Forms.ToolTip toolTipPeek;
        private MatrixControl viewPeek;
    }
}