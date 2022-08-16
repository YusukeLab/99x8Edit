
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
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.trackBarR = new System.Windows.Forms.TrackBar();
            this.textBoxR = new System.Windows.Forms.TextBox();
            this.textBoxG = new System.Windows.Forms.TextBox();
            this.trackBarG = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxB = new System.Windows.Forms.TextBox();
            this.trackBarB = new System.Windows.Forms.TrackBar();
            this.label3 = new System.Windows.Forms.Label();
            this.pictColor = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarR)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarG)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictColor)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Font = new System.Drawing.Font("Yu Gothic UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.btnOK.Location = new System.Drawing.Point(174, 144);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 35);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Font = new System.Drawing.Font("Yu Gothic UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.btnCancel.Location = new System.Drawing.Point(93, 144);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 35);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
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
            // trackBarR
            // 
            this.trackBarR.LargeChange = 1;
            this.trackBarR.Location = new System.Drawing.Point(36, 20);
            this.trackBarR.Maximum = 7;
            this.trackBarR.Name = "trackBarR";
            this.trackBarR.Size = new System.Drawing.Size(164, 56);
            this.trackBarR.TabIndex = 3;
            this.trackBarR.ValueChanged += new System.EventHandler(this.trackBarR_ValueChanged);
            // 
            // textBoxR
            // 
            this.textBoxR.Font = new System.Drawing.Font("Yu Gothic UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.textBoxR.Location = new System.Drawing.Point(206, 20);
            this.textBoxR.Name = "textBoxR";
            this.textBoxR.Size = new System.Drawing.Size(40, 27);
            this.textBoxR.TabIndex = 4;
            this.textBoxR.Text = "0";
            this.textBoxR.Leave += new System.EventHandler(this.textBoxR_Leave);
            // 
            // textBoxG
            // 
            this.textBoxG.Font = new System.Drawing.Font("Yu Gothic UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.textBoxG.Location = new System.Drawing.Point(206, 58);
            this.textBoxG.Name = "textBoxG";
            this.textBoxG.Size = new System.Drawing.Size(40, 27);
            this.textBoxG.TabIndex = 7;
            this.textBoxG.Text = "0";
            this.textBoxG.Leave += new System.EventHandler(this.textBoxG_Leave);
            // 
            // trackBarG
            // 
            this.trackBarG.LargeChange = 1;
            this.trackBarG.Location = new System.Drawing.Point(36, 58);
            this.trackBarG.Maximum = 7;
            this.trackBarG.Name = "trackBarG";
            this.trackBarG.Size = new System.Drawing.Size(164, 56);
            this.trackBarG.TabIndex = 6;
            this.trackBarG.ValueChanged += new System.EventHandler(this.trackBarG_ValueChanged);
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
            // textBoxB
            // 
            this.textBoxB.Font = new System.Drawing.Font("Yu Gothic UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.textBoxB.Location = new System.Drawing.Point(206, 96);
            this.textBoxB.Name = "textBoxB";
            this.textBoxB.Size = new System.Drawing.Size(40, 27);
            this.textBoxB.TabIndex = 10;
            this.textBoxB.Text = "0";
            this.textBoxB.Leave += new System.EventHandler(this.textBoxB_Leave);
            // 
            // trackBarB
            // 
            this.trackBarB.LargeChange = 1;
            this.trackBarB.Location = new System.Drawing.Point(36, 96);
            this.trackBarB.Maximum = 7;
            this.trackBarB.Name = "trackBarB";
            this.trackBarB.Size = new System.Drawing.Size(164, 56);
            this.trackBarB.TabIndex = 9;
            this.trackBarB.ValueChanged += new System.EventHandler(this.trackBarB_ValueChanged);
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
            // pictColor
            // 
            this.pictColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictColor.Location = new System.Drawing.Point(12, 140);
            this.pictColor.Name = "pictColor";
            this.pictColor.Size = new System.Drawing.Size(44, 41);
            this.pictColor.TabIndex = 11;
            this.pictColor.TabStop = false;
            // 
            // PaletteEditor
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(257, 193);
            this.Controls.Add(this.pictColor);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.textBoxB);
            this.Controls.Add(this.trackBarB);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxG);
            this.Controls.Add(this.trackBarG);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxR);
            this.Controls.Add(this.trackBarR);
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
            ((System.ComponentModel.ISupportInitialize)(this.trackBarR)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarG)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictColor)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar trackBarR;
        private System.Windows.Forms.TextBox textBoxR;
        private System.Windows.Forms.TextBox textBoxG;
        private System.Windows.Forms.TrackBar trackBarG;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxB;
        private System.Windows.Forms.TrackBar trackBarB;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.PictureBox pictColor;
        private System.Windows.Forms.Button btnOK;
    }
}