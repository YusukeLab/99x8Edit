
namespace _99x8Edit
{
    partial class FontBrowser
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
            this._fontList = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this._actualView = new _99x8Edit.MatrixControl();
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripCopy = new System.Windows.Forms.ToolStripMenuItem();
            this._textBox = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this._fontSize = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this._threshold = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this._italic = new System.Windows.Forms.CheckBox();
            this._colorMatrix = new _99x8Edit.MatrixControl();
            this.label6 = new System.Windows.Forms.Label();
            this.contextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._fontSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._threshold)).BeginInit();
            this.SuspendLayout();
            // 
            // _fontList
            // 
            this._fontList.FormattingEnabled = true;
            this._fontList.ItemHeight = 20;
            this._fontList.Location = new System.Drawing.Point(29, 39);
            this._fontList.Name = "_fontList";
            this._fontList.Size = new System.Drawing.Size(173, 444);
            this._fontList.TabIndex = 0;
            this._fontList.SelectedIndexChanged += new System.EventHandler(this._fontList_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Available Fonts";
            // 
            // _actualView
            // 
            this._actualView.AllowDrop = true;
            this._actualView.AllowMultipleSelection = true;
            this._actualView.AllowOneStrokeEditing = false;
            this._actualView.AllowSubSelection = false;
            this._actualView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._actualView.CellHeight = 16;
            this._actualView.CellWidth = 16;
            this._actualView.ColumnNum = 32;
            this._actualView.ContextMenuStrip = this.contextMenu;
            this._actualView.DrawOverlayedSelection = false;
            this._actualView.DrawTranparentColor = false;
            this._actualView.Index = 0;
            this._actualView.Location = new System.Drawing.Point(229, 144);
            this._actualView.Name = "_actualView";
            this._actualView.RowNum = 8;
            this._actualView.SelectionHeight = 1;
            this._actualView.SelectionWidth = 1;
            this._actualView.Size = new System.Drawing.Size(514, 130);
            this._actualView.TabIndex = 35;
            this._actualView.X = 0;
            this._actualView.Y = 0;
            // 
            // contextMenu
            // 
            this.contextMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripCopy});
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(164, 28);
            // 
            // toolStripCopy
            // 
            this.toolStripCopy.Name = "toolStripCopy";
            this.toolStripCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.toolStripCopy.Size = new System.Drawing.Size(163, 24);
            this.toolStripCopy.Text = "Copy";
            // 
            // _textBox
            // 
            this._textBox.AcceptsTab = true;
            this._textBox.Location = new System.Drawing.Point(229, 39);
            this._textBox.MaxLength = 256;
            this._textBox.Name = "_textBox";
            this._textBox.Size = new System.Drawing.Size(514, 65);
            this._textBox.TabIndex = 36;
            this._textBox.Text = "!\"#$%&\'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_{|}~abcdefghijklmn" +
    "opqrstuvwxyz";
            this._textBox.TextChanged += new System.EventHandler(this._textBox_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(218, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(105, 20);
            this.label2.TabIndex = 37;
            this.label2.Text = "Text to browse";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(218, 118);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 20);
            this.label3.TabIndex = 38;
            this.label3.Text = "Actual View";
            // 
            // _fontSize
            // 
            this._fontSize.Location = new System.Drawing.Point(282, 315);
            this._fontSize.Maximum = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this._fontSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._fontSize.Name = "_fontSize";
            this._fontSize.Size = new System.Drawing.Size(62, 27);
            this._fontSize.TabIndex = 39;
            this._fontSize.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this._fontSize.ValueChanged += new System.EventHandler(this._fontSize_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(277, 290);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(36, 20);
            this.label4.TabIndex = 40;
            this.label4.Text = "Size";
            // 
            // _threshold
            // 
            this._threshold.Location = new System.Drawing.Point(360, 315);
            this._threshold.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this._threshold.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._threshold.Name = "_threshold";
            this._threshold.Size = new System.Drawing.Size(62, 27);
            this._threshold.TabIndex = 41;
            this._threshold.Value = new decimal(new int[] {
            64,
            0,
            0,
            0});
            this._threshold.ValueChanged += new System.EventHandler(this._threshold_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(355, 289);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(74, 20);
            this.label5.TabIndex = 42;
            this.label5.Text = "Threshold";
            // 
            // _italic
            // 
            this._italic.AutoSize = true;
            this._italic.Location = new System.Drawing.Point(282, 360);
            this._italic.Name = "_italic";
            this._italic.Size = new System.Drawing.Size(63, 24);
            this._italic.TabIndex = 43;
            this._italic.Text = "Italic";
            this._italic.UseVisualStyleBackColor = true;
            this._italic.CheckedChanged += new System.EventHandler(this._italic_CheckedChanged);
            // 
            // _colorMatrix
            // 
            this._colorMatrix.AllowMultipleSelection = false;
            this._colorMatrix.AllowOneStrokeEditing = false;
            this._colorMatrix.AllowSubSelection = false;
            this._colorMatrix.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._colorMatrix.CellHeight = 20;
            this._colorMatrix.CellWidth = 20;
            this._colorMatrix.ColumnNum = 1;
            this._colorMatrix.DrawOverlayedSelection = false;
            this._colorMatrix.DrawTranparentColor = false;
            this._colorMatrix.Index = 0;
            this._colorMatrix.Location = new System.Drawing.Point(229, 313);
            this._colorMatrix.Name = "_colorMatrix";
            this._colorMatrix.RowNum = 8;
            this._colorMatrix.SelectionHeight = 1;
            this._colorMatrix.SelectionWidth = 1;
            this._colorMatrix.Size = new System.Drawing.Size(22, 162);
            this._colorMatrix.TabIndex = 44;
            this._colorMatrix.X = 0;
            this._colorMatrix.Y = 0;
            this._colorMatrix.CellOnEdit += new System.EventHandler<_99x8Edit.MatrixControl.EditEventArgs>(this._colorMatrix_CellOnEdit);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(216, 290);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(45, 20);
            this.label6.TabIndex = 45;
            this.label6.Text = "Color";
            // 
            // FontBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 495);
            this.Controls.Add(this.label6);
            this.Controls.Add(this._colorMatrix);
            this.Controls.Add(this._italic);
            this.Controls.Add(this.label5);
            this.Controls.Add(this._threshold);
            this.Controls.Add(this.label4);
            this.Controls.Add(this._fontSize);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this._textBox);
            this.Controls.Add(this._actualView);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._fontList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FontBrowser";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "FontBrowser";
            this.contextMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._fontSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._threshold)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox _fontList;
        private System.Windows.Forms.Label label1;
        private MatrixControl _actualView;
        private System.Windows.Forms.RichTextBox _textBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown _fontSize;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown _threshold;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox _italic;
        private System.Windows.Forms.Label label6;
        private MatrixControl _colorMatrix;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem toolStripCopy;
    }
}