using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace _99x8Edit
{
    // Sprite editor window
    public partial class Sprites : Form
    {
        private Machine dataSource;
        private Bitmap bmpPalette = new Bitmap(256, 64);        // Palette view
        private Bitmap bmpSprites = new Bitmap(256, 256);       // Sprites view
        private Bitmap bmpSpriteEdit = new Bitmap(256, 256);    // Sprite edit view
        private Bitmap bmpPreview = new Bitmap(32, 32);         // Edit preview
        private int currentSpriteX = 0;     // 0-7
        private int currentSpriteY = 0;     // 0-7
        private int overlaySpriteX = -1;    // -1-7
        private int overlaySpriteY = -1;    // -1-7
        private int currentLineX = 0;       // Selected line in editor(0-1)
        private int currentLineY = 0;       // Selected line in editor(0-15)
        Color colorSpriteBack = Color.Orange;
        String currentFile = "";
        public String CurrentFile
        {
            set
            {
                currentFile = value;
            }
        }
        public Sprites(Machine dataSource)
        {
            this.dataSource = dataSource;
            InitializeComponent();
            // Initialize controls
            this.viewPalette.Image = bmpPalette;
            this.viewSprites.Image = bmpSprites;
            this.viewSpriteEdit.Image = bmpSpriteEdit;
            this.viewPreview.Image = bmpPreview;
            // Refresh all views
            this.RefreshAllViews();
            // context menu
            toolStripSprCopy.Click += new EventHandler(contextSprites_copy);
            toolStripSprPaste.Click += new EventHandler(contextSprites_paste);
            toolStripSprDel.Click += new EventHandler(contextSprites_del);
            toolStripEditorCopy.Click += new EventHandler(contextEditor_copy);
            toolStripEditorPaste.Click += new EventHandler(contextEditor_paste);
            toolStripEditorDel.Click += new EventHandler(contextEditor_del);
        }
        //------------------------------------------------------------------------------
        // Overrides
        protected override bool ProcessDialogKey(Keys keyData)
        {
            switch (keyData)
            {
                // prevent focus movement by the cursor
                case Keys.Down:
                case Keys.Right:
                case Keys.Up:
                case Keys.Left:
                    break;
                default:
                    return base.ProcessDialogKey(keyData);
            }
            return true;
        }
        //------------------------------------------------------------------------------
        // Refreshing Views
        private void RefreshAllViews()
        {
            this.UpdatePaletteView();       // Palette view
            this.UpdateSpriteView();        // Sprites view
            this.UpdateSpriteEditView();    // Sprite edit view
            this.UpdateCurrentColorView();  // Current color
            this.UpdateOverlayCheck();
            if (dataSource.IsTMS9918() && !this.checkTMS.Checked)
            {
                this.checkTMS.Checked = true;
            }
            if (!dataSource.IsTMS9918() && this.checkTMS.Checked)
            {
                this.checkTMS.Checked = false;
            }
            this.btnOpenPalette.Enabled = !dataSource.IsTMS9918();
            this.btnSavePalette.Enabled = !dataSource.IsTMS9918();
        }
        private void UpdatePaletteView(bool refresh = true)
        {
            // Update palette view
            Graphics g = Graphics.FromImage(bmpPalette);
            for (int i = 0; i < 16; ++i)
            {
                Color c = dataSource.ColorCodeToWindowsColor(i);
                g.FillRectangle(new SolidBrush(c), new Rectangle((i % 8) * 32, (i / 8) * 32, 32, 32));
            }
            if(refresh) this.viewPalette.Refresh();
        }
        private void UpdateSpriteView(bool refresh = true)
        {
            Graphics g = Graphics.FromImage(bmpSprites);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            for (int i = 0; i < 8; ++i)
            {
                for(int j = 0; j < 8; ++j)
                {
                    this.DrawOneSprite(g, j, i); // For each 16x16 sprites
                }
            }
            // CRT Filter
            if (chkCRT.Checked)
            {
                FilterCRT f = new FilterCRT();
                f.Process(bmpSprites);
            }
            // Selection
            int x = currentSpriteX * 32;
            int y = currentSpriteY * 32;
            int index_of_16x16 = currentSpriteX + currentSpriteY * 8;
            bool overlayed = dataSource.GetSpriteOverlay(index_of_16x16);
            if(!overlayed)
            {
                // No overlay
                g.DrawRectangle(new Pen(Color.Red), x, y, 31, 31);
            }
            else
            {
                // Overlayed sprite, left side selection
                g.DrawLine(new Pen(Color.Red), x, y, x + 31, y);
                g.DrawLine(new Pen(Color.Red), x, y, x, y + 31);
                g.DrawLine(new Pen(Color.Red), x, y + 31, x + 31, y + 31);
                // Overlayed sprite, right side
                int index_r_16x16 = (index_of_16x16 + 1) % 64;
                int xr = (index_r_16x16 % 8) * 32;
                int yr = (index_r_16x16 / 8) * 32;
                g.DrawLine(new Pen(Color.Red), xr, yr, xr + 31, yr);
                g.DrawLine(new Pen(Color.Red), xr + 31, yr, xr + 31, yr + 31);
                g.DrawLine(new Pen(Color.Red), xr, yr + 31, xr + 31, yr + 31);
            }
            if (refresh) this.viewSprites.Refresh();
        }
        private void DrawOneSprite(Graphics g, int x, int y)
        {
            for (int dx = 0; dx < 2; ++dx)
            {
                for(int dy = 0; dy < 2; ++dy)
                {
                    // Four sprites in one 16x16 sprites
                    int target_num8x8 = (y * 8 + x) * 4 + dx * 2 + dy;
                    int target_x = x * 32 + dx * 16;
                    int target_y = y * 32 + dy * 16;
                    Bitmap b = dataSource.GetBitmapOfSprite(target_num8x8);
                    g.FillRectangle(new SolidBrush(colorSpriteBack), target_x, target_y, 17, 17);
                    g.DrawImage(b, target_x, target_y, 17, 17);
                }
            }
        }
        private void UpdateSpriteEditView(bool refresh = true)
        {
            Graphics g = Graphics.FromImage(bmpSpriteEdit);
            g.FillRectangle(new SolidBrush(colorSpriteBack), 0, 0, 256, 256);
            Graphics g_prev = Graphics.FromImage(bmpPreview);
            g_prev.FillRectangle(new SolidBrush(colorSpriteBack), 0, 0, 32, 32);
            int index_of_16x16 = currentSpriteX + currentSpriteY * 8;
            bool overlayed = dataSource.GetSpriteOverlay(index_of_16x16);
            // Four sprites are in one 16x16 sprite
            for (int x = 0; x < 2; ++x)
            {
                for(int y = 0; y < 2; ++y)
                {
                    // One sprite in 16x16 sprite
                    int target_sprite = index_of_16x16 * 4 + x * 2 + y;

                    for (int j = 0; j < 8; ++j)         // Line
                    {
                        for (int k = 0; k < 8; ++k)     // Pixel
                        {
                            int color_code = 0;     // transparent as default
                            int ptn_but = dataSource.GetSpritePixel(target_sprite, j, k);
                            if(ptn_but != 0)
                            {
                                // pixel exists, so get the color code
                                color_code = dataSource.GetSpriteColorCode(target_sprite, j);
                            }
                            if(overlayed)
                            {
                                // Overlay sprite exists
                                int over_index = (target_sprite + 4) % 256;
                                int ptn_over = dataSource.GetSpritePixel(over_index, j, k);
                                if(ptn_over != 0)
                                {
                                    // pixel of overlayed sprite exists, so get or of the color code
                                    if (dataSource.IsTMS9918())
                                    {
                                        if(color_code == 0)
                                        {
                                            // On TMS9918 \, if the pixel of primary sprite exists, don't draw
                                            color_code = dataSource.GetSpriteColorCode(over_index, j);
                                        }
                                    }
                                    else
                                    {
                                        // Take or on V9938
                                        color_code |= dataSource.GetSpriteColorCode(over_index, j);
                                    }
                                }
                            }
                            if (color_code != 0)
                            {
                                Color c = dataSource.ColorCodeToWindowsColor(color_code);
                                g.FillRectangle(new SolidBrush(c), x * 128 + k * 16, y * 128 + j * 16, 15, 15);
                                g_prev.FillRectangle(new SolidBrush(c), x * 16 + k * 2, y * 16 + j * 2, 2, 2);
                            }
                        }
                    }
                }
            }
            // Draw selection rectangle here
            // 16x16 on TMS9918, 8x1 on V9938
            if(dataSource.IsTMS9918())
            {
                // On TMS9918, select all, since only one color can be applied
                g.DrawRectangle(new Pen(Color.Red), 0, 0, 255, 255);
                panelEditor.ContextMenuStrip = null;    // disable copy/paste by line
            }
            else
            {
                // On V9938, each lines can be selected to the corresponding colors
                g.DrawRectangle(new Pen(Color.Red), currentLineX * 128, currentLineY * 16, 127, 15);
                panelEditor.ContextMenuStrip = contextEditor;
            }
            // CRT Filter
            if (chkCRT.Checked)
            {
                FilterCRT f = new FilterCRT();
                f.Process(bmpPreview);
            }
            if (refresh) this.viewSpriteEdit.Refresh();
            if(refresh) this.viewPreview.Refresh();
        }
        void UpdateCurrentColorView(bool refresh = true)
        {
            // Update current color of primary sprite
            int sprite_num_16x16 = currentSpriteY * 8 + currentSpriteX;
            int sprite_num_8x8 = sprite_num_16x16 * 4 + currentLineX * 2 + currentLineY / 8;
            int color_code_primary = dataSource.GetSpriteColorCode(sprite_num_8x8, currentLineY % 8);
            Color color_primary = dataSource.ColorCodeToWindowsColor(color_code_primary);
            viewColorL.BackColor = color_primary;
            if(refresh) viewColorL.Refresh();
            // Check overlays
            if (dataSource.GetSpriteOverlay(sprite_num_16x16))
            {
                int sprite_num_8x8_secondary = (sprite_num_8x8 + 4) % 256;
                int color_code_secondary = dataSource.GetSpriteColorCode(sprite_num_8x8_secondary, currentLineY % 8);
                Color color_secondary = dataSource.ColorCodeToWindowsColor(color_code_secondary);
                viewColorR.BackColor = color_secondary;
                viewColorR.Visible = true;
                labelColorR.Visible = true;
                if (dataSource.IsTMS9918())
                {
                    // Overlayed, but no OR color(TMS9918)
                    viewColorOR.Visible = false;
                    labelColorOR.Visible = false;
                }
                else
                {
                    // Overlayed with or color(V9938)
                    int color_code_or = color_code_primary | color_code_secondary;
                    Color color_or = dataSource.ColorCodeToWindowsColor(color_code_or);
                    viewColorOR.BackColor = color_or;
                    viewColorOR.Visible = true;
                    labelColorOR.Visible = true;
                }
            }
            else
            {
                // No overlay
                viewColorR.Visible = false;
                labelColorR.Visible = false;
                viewColorOR.Visible = false;
                labelColorOR.Visible = false;
            }
        }
        private void UpdateOverlayCheck(bool refresh = true)
        {
            int sprite_num16x16 = currentSpriteY * 8 + currentSpriteX;
            if (dataSource.GetSpriteOverlay(sprite_num16x16))
            {
                int over_sprite_16x16 = (sprite_num16x16 + 1) % 64;
                overlaySpriteX = over_sprite_16x16 % 8;
                overlaySpriteY = over_sprite_16x16 / 8;
                checkOverlay.Checked = true;
            }
            else
            {
                overlaySpriteX = -1;
                overlaySpriteY = -1;
                checkOverlay.Checked = false;
            }
            if(refresh) this.UpdateSpriteView();
        }
        //------------------------------------------------------------------------------
        // Controls
        private void checkOverlay_Click(object sender, EventArgs e)
        {
            int current_index = currentSpriteY * 8 + currentSpriteX;
            int overlay_index = (current_index + 1) % 64;
            if (checkOverlay.Checked)
            {
                dataSource.SetSpriteOverlay(current_index, true);           // Update VRAM
            }
            else
            {
                dataSource.SetSpriteOverlay(current_index, false);          // Update VRAM
            }
            this.RefreshAllViews();
        }
        private void viewSprites_MouseClick(object sender, MouseEventArgs e)
        {
            panelSprites.Focus();   // Key events are handled by parent panel
            // Sprite selected
            currentSpriteX = e.X / 32;
            currentSpriteY = e.Y / 32;
            this.RefreshAllViews();
        }
        private void panelSprites_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    if(currentSpriteY > 0)
                    {
                        currentSpriteY--;
                        this.RefreshAllViews();
                    }
                    break;
                case Keys.Left:
                    if (currentSpriteX > 0)
                    {
                        currentSpriteX--;
                        this.RefreshAllViews();
                    }
                    break;
                case Keys.Right:
                    if (currentSpriteX < 7)
                    {
                        currentSpriteX++;
                        this.RefreshAllViews();
                    }
                    break;
                case Keys.Down:
                    if (currentSpriteY < 7)
                    {
                        currentSpriteY++;
                        this.RefreshAllViews();
                    }
                    break;
            }
        }
        private void contextSprites_copy(object sender, EventArgs e)
        {
            dataSource.Copy16x16SpriteToClip(currentSpriteY * 8 + currentSpriteX);
        }
        private void contextSprites_paste(object sender, EventArgs e)
        {
            dataSource.Paste16x16SpriteFromClip(currentSpriteY * 8 + currentSpriteX);
            this.RefreshAllViews();
        }
        private void contextSprites_del(object sender, EventArgs e)
        {
            int dst16x16 = currentSpriteY * 8 + currentSpriteX;
            dataSource.Clear16x16Sprite(dst16x16);
            if (checkOverlay.Checked) dataSource.Clear16x16Sprite((dst16x16 + 1) % 64);
            this.UpdateOverlayCheck();
            this.RefreshAllViews();
        }
        private void viewSpriteEdit_MouseClick(object sender, MouseEventArgs e)
        {
            panelEditor.Focus();    // Key events are handled by parent panel
            int clicked_x = e.X / 128;
            int clicled_y = e.Y / 16;
            if(!dataSource.IsTMS9918())
            {
                // VDP is V9938 and selected line had changed
                if((currentLineX != clicked_x) || (currentLineY != clicled_y))
                {
                    currentLineX = clicked_x;
                    currentLineY = clicled_y;
                    this.UpdateSpriteEditView();
                    this.UpdateCurrentColorView();
                    return;
                }
            }
            // line clicked
            int line_x = e.X / 128;
            int line_y = e.Y / 128;
            // check pixel of first sprite
            int target_col = (e.X / 16) % 8;
            int target_row = (e.Y / 16) % 8;
            int target_sprite16x16 = currentSpriteY * 8 + currentSpriteX;
            int target_sprite8x8 = target_sprite16x16 * 4 + line_x * 2 + line_y;
            int target_prev_pixel = dataSource.GetSpritePixel(target_sprite8x8, target_row, target_col);
            // check pixel of second sprite
            int target_sprite16x16_ov = (target_sprite16x16 + 1) % 64;
            int target_sprite8x8_ov = target_sprite16x16_ov * 4 + line_x * 2 + line_y;
            int target_prev_pixel_ov = dataSource.GetSpritePixel(target_sprite8x8_ov, target_row, target_col);
            // current status 0:transparent, 1:first sprite, 2:second sprie, 3:both
            int current_stat = target_prev_pixel + (target_prev_pixel_ov << 1);
            // so the next status will be....
            int target_stat = current_stat + 1;
            if(dataSource.GetSpriteOverlay(target_sprite16x16))
            {
                if(dataSource.IsTMS9918())
                {
                    target_stat %= 3;       // Overlayed, no OR color
                }
                else
                {
                    target_stat %= 4;       // Overlayed, OR color available
                }
            }
            else
            {
                target_stat %= 2;           // No overlay
            }
            // set pixel 0:transparent, 1:first sprite, 2:second sprie, 3:both
            int pixel = ((target_stat & 0x01) > 0) ? 1 : 0;
            dataSource.SetSpritePixel(target_sprite8x8, target_row, target_col, pixel);
            if(dataSource.GetSpriteOverlay(target_sprite16x16))
            {
                pixel = ((target_stat & 0x02) > 0) ? 1 : 0;
                dataSource.SetSpritePixel(target_sprite8x8_ov, target_row, target_col, pixel);
            }
            this.UpdateSpriteEditView();
            this.UpdateSpriteView();
        }
        private void panelEditor_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            bool update = false;
            switch (e.KeyCode)
            {
                case Keys.Up:
                    if (currentLineY > 0)
                    {
                        currentLineY--;
                        update = true;
                    }
                    break;
                case Keys.Left:
                    if (currentLineX > 0)
                    {
                        currentLineX--;
                        update = true;
                    }
                    break;
                case Keys.Right:
                    if (currentLineX < 1)
                    {
                        currentLineX++;
                        update = true;
                    }
                    break;
                case Keys.Down:
                    if (currentLineY < 15)
                    {
                        currentLineY++;
                        update = true;
                    }
                    break;
            }
            if (update)
            {
                this.UpdateSpriteEditView();
                this.UpdateCurrentColorView();
            }
        }
        private void contextEditor_copy(object sender, EventArgs e)
        {
            int src16x16 = currentSpriteY * 8 + currentSpriteX;
            int src8x8 = src16x16 * 4 + currentLineX * 2 + currentLineY / 8;
            dataSource.CopySpriteLineToClip(src8x8, currentLineY % 8);
        }
        private void contextEditor_paste(object sender, EventArgs e)
        {
            int dst16x16 = currentSpriteY * 8 + currentSpriteX;
            int dst8x8 = dst16x16 * 4 + currentLineX * 2 + currentLineY / 8;
            dataSource.PasteSpriteLineFromClip(dst8x8, currentLineY % 8);
            this.UpdateSpriteEditView();
            this.UpdateSpriteView();
        }
        private void contextEditor_del(object sender, EventArgs e)
        {
            int dst16x16 = currentSpriteY * 8 + currentSpriteX;
            int dst8x8 = dst16x16 * 4 + currentLineX * 2 + currentLineY / 8;
            dataSource.ClearSpriteLine(dst8x8, currentLineY % 8);   // check overlay inside
            this.UpdateSpriteEditView();
            this.UpdateSpriteView();
        }
        private void checkTMS_Click(object sender, EventArgs e)
        {
            if (this.checkTMS.Checked && !dataSource.IsTMS9918())
            {
                // Set windows color of each color code to TMS9918
                dataSource.SetPaletteToTMS9918();
            }
            else if (!this.checkTMS.Checked && dataSource.IsTMS9918())
            {
                // Set windows color of each color code to internal palette
                dataSource.SetPaletteToV9938();
            }
            this.RefreshAllViews();     // Everything changes
        }
        private void viewColorL_Click(object sender, EventArgs e)
        {
            PaletteSelector palette_win = new PaletteSelector(bmpPalette, viewColorL_ColorSelectionCallback);
            palette_win.StartPosition = FormStartPosition.Manual;
            palette_win.Location = Cursor.Position;
            palette_win.Show();
        }
        private int viewColorL_ColorSelectionCallback(int val)
        {
            int current_target16x16 = (currentSpriteY * 8 + currentSpriteX) % 64;
            this.chgSpriteClrOfCurrent(current_target16x16, val);
            return 0;
        }
        private void viewColorR_Click(object sender, EventArgs e)
        {
            PaletteSelector palette_win = new PaletteSelector(bmpPalette, viewColorR_ColorSelectionCallback);
            palette_win.StartPosition = FormStartPosition.Manual;
            palette_win.Location = Cursor.Position;
            palette_win.Show();
        }
        private int viewColorR_ColorSelectionCallback(int val)
        {
            int current_target16x16 = (currentSpriteY * 8 + currentSpriteX + 1) % 64;
            this.chgSpriteClrOfCurrent(current_target16x16, val);
            return 0;
        }
        private void viewPalette_MouseClick(object sender, MouseEventArgs e)
        {
            // Palette view clicked
            int clicked_color_num = (e.Y / 32) * 8 + (e.X / 32);
            // Update color table of current line
            int current_target16x16 = 0;
            if (e.Button == MouseButtons.Left)          // To current sprite
            {
                current_target16x16 = (currentSpriteY * 8 + currentSpriteX) % 64;
            }
            else if (e.Button == MouseButtons.Right)    // To overlayed sprite
            {
                if (this.checkOverlay.Checked == true)
                {
                    current_target16x16 = (currentSpriteY * 8 + currentSpriteX + 1) % 64;
                }
                else return;
            }
            this.chgSpriteClrOfCurrent(current_target16x16, clicked_color_num);
        }
        private void viewPalette_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!checkTMS.Checked)
            {
                int clicked_color_num = (e.Y / 32) * 8 + (e.X / 32);
                int R = dataSource.GetPaletteR(clicked_color_num);
                int G = dataSource.GetPaletteG(clicked_color_num);
                int B = dataSource.GetPaletteB(clicked_color_num);
                PaletteEditor palette_win = new PaletteEditor(R, G, B);
                if (palette_win.ShowDialog() == DialogResult.OK)
                {
                    dataSource.SetPalette(clicked_color_num, palette_win.R, palette_win.G, palette_win.B);
                    this.RefreshAllViews();     // Everything changes
                }
            }
        }
        private void Sprites_Activated(object sender, EventArgs e)
        {
            // Refresh all since palette may have been changed
            this.RefreshAllViews();
        }
        public void ChangeOccuredByHost()
        {
            this.RefreshAllViews();
        }
        private void btnSavePalette_Click(object sender, EventArgs e)
        {
            String dir = Path.GetDirectoryName(currentFile);
            if (dir == null)
            {
                dir = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            }
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.InitialDirectory = dir;
            dlg.Filter = "PLT File(*.plt)|*.plt";
            dlg.FilterIndex = 1;
            dlg.Title = "Save palette";
            dlg.RestoreDirectory = true;
            dlg.OverwritePrompt = true;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                BinaryWriter br = new BinaryWriter(new FileStream(dlg.FileName, FileMode.Create));
                try
                {
                    dataSource.SavePaletteSettings(br);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    br.Close();
                }
            }
        }
        private void btnOpenPalette_Click(object sender, EventArgs e)
        {
            String dir = Path.GetDirectoryName(currentFile);
            if (dir == null)
            {
                dir = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            }
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = dir;
            dlg.Filter = "PLT File(*.plt)|*.plt";
            dlg.FilterIndex = 1;
            dlg.Title = "Load palette";
            dlg.RestoreDirectory = true;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                BinaryReader br = new BinaryReader(new FileStream(dlg.FileName, FileMode.Open));
                try
                {
                    dataSource.LoadPaletteSettings(br);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    br.Close();
                }
                this.RefreshAllViews();
            }
        }
        private void chkCRT_CheckedChanged(object sender, EventArgs e)
        {
            this.RefreshAllViews();
        }
        //----------------------------------------------------------------------
        // Utility
        private void chgSpriteClrOfCurrent(int target16x16, int val)
        {
            int target8x8 = target16x16 * 4;
            if (dataSource.IsTMS9918())
            {
                // Set color to four current sprites
                dataSource.SetSpriteColorCode(target8x8 + 0, 0, val);
                dataSource.SetSpriteColorCode(target8x8 + 1, 0, val);
                dataSource.SetSpriteColorCode(target8x8 + 2, 0, val);
                dataSource.SetSpriteColorCode(target8x8 + 3, 0, val);
            }
            else
            {
                // Set color to current line of current sprite
                target8x8 = target8x8 + currentLineX * 2 + currentLineY / 8;
                dataSource.SetSpriteColorCode(target8x8, currentLineY % 8, val);
            }
            this.RefreshAllViews();     // Everything changes
        }
    }
}
