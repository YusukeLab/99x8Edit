
namespace _99x8Edit
{
    partial class PaletteEditor
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
            this._btnOK = new System.Windows.Forms.Button();
            this._btnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this._trackBarR = new System.Windows.Forms.TrackBar();
            this._textBoxR = new System.Windows.Forms.TextBox();
            this._textBoxG = new System.Windows.Forms.TextBox();
            this._trackBarG = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this._textBoxB = new System.Windows.Forms.TextBox();
            this._trackBarB = new System.Windows.Forms.TrackBar();
            this.label3 = new System.Windows.Forms.Label();
            this._pictColor = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this._trackBarR)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._trackBarG)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._trackBarB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._pictColor)).BeginInit();
            this.SuspendLayout();
            // 
            // _btnOK
            // 
            this._btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._btnOK.Font = new System.Drawing.Font("Yu Gothic UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this._btnOK.Location = new System.Drawing.Point(174, 144);
            this._btnOK.Name = "_btnOK";
            this._btnOK.Size = new System.Drawing.Size(75, 35);
            this._btnOK.TabIndex = 1;
            this._btnOK.Text = "OK";
            this._btnOK.UseVisualStyleBackColor = true;
            this._btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // _btnCancel
            // 
            this._btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._btnCancel.Font = new System.Drawing.Font("Yu Gothic UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this._btnCancel.Location = new System.Drawing.Point(93, 144);
            this._btnCancel.Name = "_btnCancel";
            this._btnCancel.Size = new System.Drawing.Size(75, 35);
            this._btnCancel.TabIndex = 0;
            this._btnCancel.Text = "Cancel";
            this._btnCancel.UseVisualStyleBackColor = true;
            this._btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Yu Gothic UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label1.Location = new System.Drawing.Point(12, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(18, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "R";
            // 
            // _trackBarR
            // 
            this._trackBarR.LargeChange = 1;
            this._trackBarR.Location = new System.Drawing.Point(36, 20);
            this._trackBarR.Maximum = 7;
            this._trackBarR.Name = "_trackBarR";
            this._trackBarR.Size = new System.Drawing.Size(164, 56);
            this._trackBarR.TabIndex = 3;
            this._trackBarR.ValueChanged += new System.EventHandler(this.trackBarR_ValueChanged);
            // 
            // _textBoxR
            // 
            this._textBoxR.Font = new System.Drawing.Font("Yu Gothic UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this._textBoxR.Location = new System.Drawing.Point(206, 20);
            this._textBoxR.Name = "_textBoxR";
            this._textBoxR.Size = new System.Drawing.Size(40, 27);
            this._textBoxR.TabIndex = 4;
            this._textBoxR.Text = "0";
            this._textBoxR.Leave += new System.EventHandler(this.textBoxR_Leave);
            // 
            // _textBoxG
            // 
            this._textBoxG.Font = new System.Drawing.Font("Yu Gothic UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this._textBoxG.Location = new System.Drawing.Point(206, 58);
            this._textBoxG.Name = "_textBoxG";
            this._textBoxG.Size = new System.Drawing.Size(40, 27);
            this._textBoxG.TabIndex = 7;
            this._textBoxG.Text = "0";
            this._textBoxG.Leave += new System.EventHandler(this.textBoxG_Leave);
            // 
            // _trackBarG
            // 
            this._trackBarG.LargeChange = 1;
            this._trackBarG.Location = new System.Drawing.Point(36, 58);
            this._trackBarG.Maximum = 7;
            this._trackBarG.Name = "_trackBarG";
            this._trackBarG.Size = new System.Drawing.Size(164, 56);
            this._trackBarG.TabIndex = 6;
            this._trackBarG.ValueChanged += new System.EventHandler(this.trackBarG_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Yu Gothic UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label2.Location = new System.Drawing.Point(12, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(19, 20);
            this.label2.TabIndex = 5;
            this.label2.Text = "G";
            // 
            // _textBoxB
            // 
            this._textBoxB.Font = new System.Drawing.Font("Yu Gothic UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this._textBoxB.Location = new System.Drawing.Point(206, 96);
            this._textBoxB.Name = "_textBoxB";
            this._textBoxB.Size = new System.Drawing.Size(40, 27);
            this._textBoxB.TabIndex = 10;
            this._textBoxB.Text = "0";
            this._textBoxB.Leave += new System.EventHandler(this.textBoxB_Leave);
            // 
            // _trackBarB
            // 
            this._trackBarB.LargeChange = 1;
            this._trackBarB.Location = new System.Drawing.Point(36, 96);
            this._trackBarB.Maximum = 7;
            this._trackBarB.Name = "_trackBarB";
            this._trackBarB.Size = new System.Drawing.Size(164, 56);
            this._trackBarB.TabIndex = 9;
            this._trackBarB.ValueChanged += new System.EventHandler(this.trackBarB_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Yu Gothic UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label3.Location = new System.Drawing.Point(12, 99);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(18, 20);
            this.label3.TabIndex = 8;
            this.label3.Text = "B";
            // 
            // _pictColor
            // 
            this._pictColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._pictColor.Location = new System.Drawing.Point(12, 140);
            this._pictColor.Name = "_pictColor";
            this._pictColor.Size = new System.Drawing.Size(44, 41);
            this._pictColor.TabIndex = 11;
            this._pictColor.TabStop = false;
            // 
            // PaletteEditor
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(257, 193);
            this.Controls.Add(this._pictColor);
            this.Controls.Add(this._btnOK);
            this.Controls.Add(this._btnCancel);
            this.Controls.Add(this._textBoxB);
            this.Controls.Add(this._trackBarB);
            this.Controls.Add(this.label3);
            this.Controls.Add(this._textBoxG);
            this.Controls.Add(this._trackBarG);
            this.Controls.Add(this.label2);
            this.Controls.Add(this._textBoxR);
            this.Controls.Add(this._trackBarR);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PaletteEditor";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Palette";
            this.Deactivate += new System.EventHandler(this.PaletteEditor_Deactivate);
            ((System.ComponentModel.ISupportInitialize)(this._trackBarR)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._trackBarG)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._trackBarB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._pictColor)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _btnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar _trackBarR;
        private System.Windows.Forms.TextBox _textBoxR;
        private System.Windows.Forms.TextBox _textBoxG;
        private System.Windows.Forms.TrackBar _trackBarG;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox _textBoxB;
        private System.Windows.Forms.TrackBar _trackBarB;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.PictureBox _pictColor;
        private System.Windows.Forms.Button _btnOK;
    }
}