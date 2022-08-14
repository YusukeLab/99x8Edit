
namespace _99x8Edit
{
    partial class PaletteSelector
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
            this.viewPlt = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.viewPlt)).BeginInit();
            this.SuspendLayout();
            // 
            // viewPlt
            // 
            this.viewPlt.Location = new System.Drawing.Point(0, 0);
            this.viewPlt.Name = "viewPlt";
            this.viewPlt.Size = new System.Drawing.Size(256, 64);
            this.viewPlt.TabIndex = 0;
            this.viewPlt.TabStop = false;
            this.viewPlt.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox_Click);
            // 
            // PaletteSelector
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(256, 64);
            this.Controls.Add(this.viewPlt);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PaletteSelector";
            this.ShowInTaskbar = false;
            this.Text = "PaletteSelector";
            this.Deactivate += new System.EventHandler(this.PaletteSelector_Deactivate);
            this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.PaletteSelector_PreviewKeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.viewPlt)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox viewPlt;
    }
}