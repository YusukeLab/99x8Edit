
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
            this.viewColor = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.viewPalette = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.viewColor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewPalette)).BeginInit();
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
            // viewColor
            // 
            this.viewColor.Location = new System.Drawing.Point(20, 50);
            this.viewColor.Name = "viewColor";
            this.viewColor.Size = new System.Drawing.Size(640, 418);
            this.viewColor.TabIndex = 1;
            this.viewColor.TabStop = false;
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
            // viewPalette
            // 
            this.viewPalette.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.viewPalette.Location = new System.Drawing.Point(20, 526);
            this.viewPalette.Name = "viewPalette";
            this.viewPalette.Size = new System.Drawing.Size(258, 66);
            this.viewPalette.TabIndex = 31;
            this.viewPalette.TabStop = false;
            this.viewPalette.MouseMove += new System.Windows.Forms.MouseEventHandler(this.viewPalette_MouseMove);
            // 
            // PaletteOrColors
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(678, 628);
            this.Controls.Add(this.viewPalette);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.viewColor);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PaletteOrColors";
            this.Text = "PaletteOrColors";
            this.Deactivate += new System.EventHandler(this.PaletteOrColors_Deactivate);
            ((System.ComponentModel.ISupportInitialize)(this.viewColor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewPalette)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox viewColor;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox viewPalette;
    }
}