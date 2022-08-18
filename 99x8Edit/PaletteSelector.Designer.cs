
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
            this.viewPalette = new _99x8Edit.MatrixControl();
            this.SuspendLayout();
            // 
            // viewPalette
            // 
            this.viewPalette.AllowMultipleSelection = false;
            this.viewPalette.AllowSubSelection = false;
            this.viewPalette.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.viewPalette.CellHeight = 32;
            this.viewPalette.CellWidth = 32;
            this.viewPalette.ColumnNum = 8;
            this.viewPalette.Location = new System.Drawing.Point(-1, -1);
            this.viewPalette.Name = "viewPalette";
            this.viewPalette.RowNum = 2;
            this.viewPalette.SelectionHeight = 1;
            this.viewPalette.SelectionWidth = 1;
            this.viewPalette.Size = new System.Drawing.Size(258, 66);
            this.viewPalette.TabIndex = 38;
            this.viewPalette.X = 0;
            this.viewPalette.Y = 0;
            this.viewPalette.CellOnEdit += new System.EventHandler<System.EventArgs>(this.viewPalette_CellOnEdit);
            this.viewPalette.MouseDown += new System.Windows.Forms.MouseEventHandler(this.viewPalette_MouseDown);
            // 
            // PaletteSelector
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(256, 64);
            this.Controls.Add(this.viewPalette);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PaletteSelector";
            this.ShowInTaskbar = false;
            this.Text = "PaletteSelector";
            this.Deactivate += new System.EventHandler(this.PaletteSelector_Deactivate);
            this.ResumeLayout(false);

        }

        #endregion

        private MatrixControl viewPalette;
    }
}