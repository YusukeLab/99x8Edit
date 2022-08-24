
namespace _99x8Edit
{
    partial class PaletteOrColors
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
            this.label1 = new System.Windows.Forms.Label();
            this._viewColor = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this._viewPalette = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this._viewColor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._viewPalette)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(150, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Effective OR color list";
            // 
            // _viewColor
            // 
            this._viewColor.Location = new System.Drawing.Point(20, 50);
            this._viewColor.Name = "_viewColor";
            this._viewColor.Size = new System.Drawing.Size(640, 418);
            this._viewColor.TabIndex = 1;
            this._viewColor.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 495);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "Filtered with:";
            // 
            // _viewPalette
            // 
            this._viewPalette.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._viewPalette.Location = new System.Drawing.Point(20, 526);
            this._viewPalette.Name = "_viewPalette";
            this._viewPalette.Size = new System.Drawing.Size(258, 66);
            this._viewPalette.TabIndex = 31;
            this._viewPalette.TabStop = false;
            this._viewPalette.MouseMove += new System.Windows.Forms.MouseEventHandler(this.viewPalette_MouseMove);
            // 
            // PaletteOrColors
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(678, 628);
            this.Controls.Add(this._viewPalette);
            this.Controls.Add(this.label2);
            this.Controls.Add(this._viewColor);
            this.Controls.Add(this.label1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PaletteOrColors";
            this.Text = "PaletteOrColors";
            this.Deactivate += new System.EventHandler(this.PaletteOrColors_Deactivate);
            ((System.ComponentModel.ISupportInitialize)(this._viewColor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._viewPalette)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox _viewColor;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox _viewPalette;
    }
}