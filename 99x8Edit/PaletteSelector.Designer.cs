
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
            this._viewPalette = new _99x8Edit.MatrixControl();
            this.SuspendLayout();
            // 
            // _viewPalette
            // 
            this._viewPalette.AllowMultipleSelection = false;
            this._viewPalette.AllowOneStrokeEditing = false;
            this._viewPalette.AllowSelection = true;
            this._viewPalette.AllowSubSelection = false;
            this._viewPalette.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._viewPalette.CellHeight = 32;
            this._viewPalette.CellWidth = 32;
            this._viewPalette.ColumnNum = 8;
            this._viewPalette.DrawOverlayedSelection = false;
            this._viewPalette.DrawTranparentColor = true;
            this._viewPalette.Index = 0;
            this._viewPalette.Location = new System.Drawing.Point(-1, -1);
            this._viewPalette.Name = "_viewPalette";
            this._viewPalette.RowNum = 2;
            this._viewPalette.SelectionHeight = 1;
            this._viewPalette.SelectionWidth = 1;
            this._viewPalette.Size = new System.Drawing.Size(258, 66);
            this._viewPalette.TabIndex = 38;
            this._viewPalette.X = 0;
            this._viewPalette.Y = 0;
            this._viewPalette.CellOnEdit += new System.EventHandler<_99x8Edit.MatrixControl.EditEventArgs>(this.viewPalette_CellOnEdit);
            this._viewPalette.MouseDown += new System.Windows.Forms.MouseEventHandler(this.viewPalette_MouseDown);
            // 
            // PaletteSelector
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(256, 64);
            this.Controls.Add(this._viewPalette);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PaletteSelector";
            this.ShowInTaskbar = false;
            this.Text = "PaletteSelector";
            this.Deactivate += new System.EventHandler(this.PaletteSelector_Deactivate);
            this.ResumeLayout(false);

        }

        #endregion

        private MatrixControl _viewPalette;
    }
}