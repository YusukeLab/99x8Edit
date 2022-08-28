
namespace _99x8Edit
{
    partial class MapSelector
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
            this._view = new _99x8Edit.MatrixControl();
            this._txtW = new System.Windows.Forms.TextBox();
            this._txtH = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this._context = new System.Windows.Forms.ContextMenuStrip(this.components);
            this._menuCopy = new System.Windows.Forms.ToolStripMenuItem();
            this._menuPaste = new System.Windows.Forms.ToolStripMenuItem();
            this._context.SuspendLayout();
            this.SuspendLayout();
            // 
            // _view
            // 
            this._view.AllowDrop = true;
            this._view.AllowMultipleSelection = false;
            this._view.AllowOneStrokeEditing = false;
            this._view.AllowSelection = true;
            this._view.AllowSubSelection = true;
            this._view.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._view.CellHeight = 16;
            this._view.CellWidth = 16;
            this._view.ColumnNum = 16;
            this._view.ContextMenuStrip = this._context;
            this._view.DrawOverlayedSelection = false;
            this._view.DrawTransparentColor = true;
            this._view.Index = 0;
            this._view.Location = new System.Drawing.Point(12, 48);
            this._view.Name = "_view";
            this._view.RowNum = 16;
            this._view.SelectionHeight = 1;
            this._view.SelectionWidth = 1;
            this._view.Size = new System.Drawing.Size(258, 258);
            this._view.TabIndex = 34;
            this._view.X = 0;
            this._view.Y = 0;
            this._view.SelectionChanged += new System.EventHandler<System.EventArgs>(this._view_SelectionChanged);
            // 
            // _txtW
            // 
            this._txtW.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._txtW.Location = new System.Drawing.Point(189, 11);
            this._txtW.Name = "_txtW";
            this._txtW.Size = new System.Drawing.Size(29, 27);
            this._txtW.TabIndex = 35;
            this._txtW.Leave += new System.EventHandler(this._txtW_Leave);
            // 
            // _txtH
            // 
            this._txtH.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._txtH.Location = new System.Drawing.Point(242, 11);
            this._txtH.Name = "_txtH";
            this._txtH.Size = new System.Drawing.Size(29, 27);
            this._txtH.TabIndex = 36;
            this._txtH.Leave += new System.EventHandler(this._txtH_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Yu Gothic UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label1.Location = new System.Drawing.Point(222, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(16, 20);
            this.label1.TabIndex = 37;
            this.label1.Text = "x";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Yu Gothic UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label2.Location = new System.Drawing.Point(147, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 20);
            this.label2.TabIndex = 38;
            this.label2.Text = "Size:";
            // 
            // _context
            // 
            this._context.ImageScalingSize = new System.Drawing.Size(20, 20);
            this._context.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._menuCopy,
            this._menuPaste});
            this._context.Name = "_context";
            this._context.Size = new System.Drawing.Size(165, 52);
            // 
            // _menuCopy
            // 
            this._menuCopy.Name = "_menuCopy";
            this._menuCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this._menuCopy.Size = new System.Drawing.Size(164, 24);
            this._menuCopy.Text = "Copy";
            // 
            // _menuPaste
            // 
            this._menuPaste.Name = "_menuPaste";
            this._menuPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this._menuPaste.Size = new System.Drawing.Size(164, 24);
            this._menuPaste.Text = "Paste";
            // 
            // MapSelector
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(283, 322);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._txtH);
            this.Controls.Add(this._txtW);
            this.Controls.Add(this._view);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MapSelector";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Map Selector";
            this._context.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MatrixControl _view;
        private System.Windows.Forms.TextBox _txtW;
        private System.Windows.Forms.TextBox _txtH;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ContextMenuStrip _context;
        private System.Windows.Forms.ToolStripMenuItem _menuCopy;
        private System.Windows.Forms.ToolStripMenuItem _menuPaste;
    }
}