
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
            this._btnLinear = new System.Windows.Forms.RadioButton();
            this._btnSprites = new System.Windows.Forms.RadioButton();
            this._contextPeek = new System.Windows.Forms.ContextMenuStrip(this.components);
            this._toolStripCopy = new System.Windows.Forms.ToolStripMenuItem();
            this._txtAddr = new System.Windows.Forms.TextBox();
            this._btnUp = new System.Windows.Forms.Button();
            this._btnDown = new System.Windows.Forms.Button();
            this._toolTipPeek = new System.Windows.Forms.ToolTip(this.components);
            this._viewPeek = new _99x8Edit.MatrixControl();
            this._contextPeek.SuspendLayout();
            this.SuspendLayout();
            // 
            // _btnLinear
            // 
            this._btnLinear.AutoSize = true;
            this._btnLinear.Font = new System.Drawing.Font("Yu Gothic UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this._btnLinear.Location = new System.Drawing.Point(439, 11);
            this._btnLinear.Name = "_btnLinear";
            this._btnLinear.Size = new System.Drawing.Size(70, 24);
            this._btnLinear.TabIndex = 0;
            this._btnLinear.TabStop = true;
            this._btnLinear.Text = "Linear";
            this._btnLinear.UseVisualStyleBackColor = true;
            this._btnLinear.Click += new System.EventHandler(this.btnLinear_Click);
            // 
            // _btnSprites
            // 
            this._btnSprites.AutoSize = true;
            this._btnSprites.Font = new System.Drawing.Font("Yu Gothic UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this._btnSprites.Location = new System.Drawing.Point(439, 36);
            this._btnSprites.Name = "_btnSprites";
            this._btnSprites.Size = new System.Drawing.Size(69, 24);
            this._btnSprites.TabIndex = 1;
            this._btnSprites.TabStop = true;
            this._btnSprites.Text = "16x16";
            this._btnSprites.UseVisualStyleBackColor = true;
            this._btnSprites.Click += new System.EventHandler(this.btnSprites_Click);
            // 
            // _contextPeek
            // 
            this._contextPeek.ImageScalingSize = new System.Drawing.Size(20, 20);
            this._contextPeek.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._toolStripCopy});
            this._contextPeek.Name = "contextPeek";
            this._contextPeek.Size = new System.Drawing.Size(164, 28);
            // 
            // _toolStripCopy
            // 
            this._toolStripCopy.Name = "_toolStripCopy";
            this._toolStripCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this._toolStripCopy.Size = new System.Drawing.Size(163, 24);
            this._toolStripCopy.Text = "Copy";
            // 
            // _txtAddr
            // 
            this._txtAddr.Font = new System.Drawing.Font("Yu Gothic UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this._txtAddr.Location = new System.Drawing.Point(223, 30);
            this._txtAddr.Name = "_txtAddr";
            this._txtAddr.Size = new System.Drawing.Size(77, 27);
            this._txtAddr.TabIndex = 3;
            this._txtAddr.Text = "4000";
            this._txtAddr.Leave += new System.EventHandler(this.txtAddr_Leave);
            // 
            // _btnUp
            // 
            this._btnUp.Location = new System.Drawing.Point(180, 30);
            this._btnUp.Name = "_btnUp";
            this._btnUp.Size = new System.Drawing.Size(33, 29);
            this._btnUp.TabIndex = 4;
            this._btnUp.Text = "<";
            this._btnUp.UseVisualStyleBackColor = true;
            this._btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // _btnDown
            // 
            this._btnDown.Location = new System.Drawing.Point(311, 29);
            this._btnDown.Name = "_btnDown";
            this._btnDown.Size = new System.Drawing.Size(33, 29);
            this._btnDown.TabIndex = 5;
            this._btnDown.Text = ">";
            this._btnDown.UseVisualStyleBackColor = true;
            this._btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // _toolTipPeek
            // 
            this._toolTipPeek.AutomaticDelay = 0;
            // 
            // _viewPeek
            // 
            this._viewPeek.AllowDrop = true;
            this._viewPeek.AllowMultipleSelection = true;
            this._viewPeek.AllowOneStrokeEditing = false;
            this._viewPeek.AllowSelection = true;
            this._viewPeek.AllowSubSelection = false;
            this._viewPeek.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._viewPeek.CellHeight = 16;
            this._viewPeek.CellWidth = 16;
            this._viewPeek.ColumnNum = 32;
            this._viewPeek.ContextMenuStrip = this._contextPeek;
            this._viewPeek.DrawOverlayedSelection = false;
            this._viewPeek.DrawTransparentColor = false;
            this._viewPeek.Index = 0;
            this._viewPeek.Location = new System.Drawing.Point(8, 65);
            this._viewPeek.Name = "_viewPeek";
            this._viewPeek.RowNum = 32;
            this._viewPeek.SelectionHeight = 2;
            this._viewPeek.SelectionWidth = 2;
            this._viewPeek.Size = new System.Drawing.Size(514, 514);
            this._viewPeek.TabIndex = 48;
            this._viewPeek.X = 0;
            this._viewPeek.Y = 0;
            this._viewPeek.MatrixOnScroll += new System.EventHandler<_99x8Edit.MatrixControl.ScrollEventArgs>(this.viewPtn_MatrixOnScroll);
            // 
            // PeekWindow
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(532, 586);
            this.Controls.Add(this._viewPeek);
            this.Controls.Add(this._btnDown);
            this.Controls.Add(this._btnUp);
            this.Controls.Add(this._txtAddr);
            this.Controls.Add(this._btnSprites);
            this.Controls.Add(this._btnLinear);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PeekWindow";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Peek binaries";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Peek_FormClosing);
            this._contextPeek.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton _btnLinear;
        private System.Windows.Forms.RadioButton _btnSprites;
        private System.Windows.Forms.TextBox _txtAddr;
        private System.Windows.Forms.Button _btnUp;
        private System.Windows.Forms.Button _btnDown;
        private System.Windows.Forms.ContextMenuStrip _contextPeek;
        private System.Windows.Forms.ToolStripMenuItem _toolStripCopy;
        private System.Windows.Forms.ToolTip _toolTipPeek;
        private MatrixControl _viewPeek;
    }
}