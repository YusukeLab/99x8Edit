
namespace _99x8Edit
{
    partial class CursorAnimation
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
            this._pict = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this._pict)).BeginInit();
            this.SuspendLayout();
            // 
            // _pict
            // 
            this._pict.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._pict.Location = new System.Drawing.Point(0, 0);
            this._pict.Name = "_pict";
            this._pict.Size = new System.Drawing.Size(64, 64);
            this._pict.TabIndex = 0;
            this._pict.TabStop = false;
            // 
            // CursorAnimation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Yellow;
            this.ClientSize = new System.Drawing.Size(70, 70);
            this.Controls.Add(this._pict);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "CursorAnimation";
            this.Text = "CursorAnim";
            this.TransparencyKey = System.Drawing.Color.Yellow;
            ((System.ComponentModel.ISupportInitialize)(this._pict)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox _pict;
    }
}