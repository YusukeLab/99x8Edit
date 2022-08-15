using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace _99x8Edit
{
    class dummy { }
    // Partial classes for the keyboard inputs of main editors
    public partial class PCGEditor : Form
    {
        protected override bool ProcessDialogKey(Keys keyData)
        {
            switch (keyData)
            {
                // prevent focus movement by the cursor
                case Keys.Down:
                case Keys.Right:
                case Keys.Up:
                case Keys.Left:
                case Keys.Down | Keys.Shift:
                case Keys.Right | Keys.Shift:
                case Keys.Up | Keys.Shift:
                case Keys.Left | Keys.Shift:
                    break;
                default:
                    return base.ProcessDialogKey(keyData);
            }
            return true;
        }
        private void panelPalette_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    if (curPal.Y > 0)
                    {
                        curPal.Y--;
                    }
                    this.UpdatePaletteView();
                    break;
                case Keys.Down:
                    if (curPal.Y < 1)
                    {
                        curPal.Y++;
                    }
                    this.UpdatePaletteView();
                    break;
                case Keys.Left:
                    if (curPal.X > 0)
                    {
                        curPal.X--;
                    }
                    this.UpdatePaletteView();
                    break;
                case Keys.Right:
                    if (curPal.X < 7)
                    {
                        curPal.X++;
                    }
                    this.UpdatePaletteView();
                    break;
                case Keys.Space:
                case Keys.Enter:
                    this.EditPalette(curPal.Y * 8 + curPal.X);
                    break;
            }
        }
        private void panelColor_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Left:
                    if (curColor.X > 0)
                    {
                        curColor.X--;
                        this.UpdateCurrentColorView();
                    }
                    break;
                case Keys.Right:
                    if (curColor.X < 1)
                    {
                        curColor.X++;
                        this.UpdateCurrentColorView();
                    }
                    break;
                case Keys.Space:
                case Keys.Enter:
                    if (curColor.X == 0)
                    {
                        this.viewColorL_Click(null, null);
                    }
                    else if (curColor.X == 1)
                    {
                        this.viewColorR_Click(null, null);
                    }
                    break;
            }
        }
        private void panelEditor_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            Action refresh = () =>
            {
                this.UpdatePCGEditView();               // Update editor view
                this.UpdateCurrentColorView();          // Update view of current color
            };
            switch (e.KeyData)
            {
                case Keys.Up | Keys.Shift:
                    if (curLine.ToY > 0)
                    {
                        curLine.ToY--;
                        refresh();
                    }
                    break;
                case Keys.Down | Keys.Shift:
                    if (curLine.ToY < 15)
                    {
                        curLine.ToY++;
                        refresh();
                    }
                    break;
                case Keys.Left | Keys.Shift:
                    if (curLine.ToX > 0)
                    {
                        curLine.ToX--;
                        refresh();
                    }
                    break;
                case Keys.Right | Keys.Shift:
                    if (curLine.ToX < 1)
                    {
                        curLine.ToX++;
                        refresh();
                    }
                    break;
                case Keys.Up:
                    if (curLine.Y > 0)
                    {
                        curLine.Y--;
                        curLine.ResetSelection();
                        refresh();
                    }
                    break;
                case Keys.Down:
                    if (curLine.Y < 15)
                    {
                        curLine.Y++;
                        curLine.ResetSelection();
                        refresh();
                    }
                    break;
                case Keys.Left:
                    if ((currentDot == 0) && (curLine.X > 0))
                    {
                        curLine.X--;
                        currentDot = 7;
                        curLine.ResetSelection();
                        refresh();
                    }
                    else if (currentDot > 0)
                    {
                        currentDot--;
                        refresh();
                    }
                    break;
                case Keys.Right:
                    if ((currentDot == 7) && (curLine.X < 1))
                    {
                        curLine.X++;
                        currentDot = 0;
                        curLine.ResetSelection();
                        refresh();
                    }
                    else if (currentDot < 7)
                    {
                        currentDot++;
                        refresh();
                    }
                    break;
                case Keys.Space:
                    // toggle the color of selected pixel
                    this.EditCurrentPCG(currentDot, curLine.Y % 8);
                    break;
                case Keys.D1:
                case Keys.NumPad1:
                    this.EditCurrentPCG(0, curLine.Y % 8);
                    break;
                case Keys.D2:
                case Keys.NumPad2:
                    this.EditCurrentPCG(1, curLine.Y % 8);
                    break;
                case Keys.D3:
                case Keys.NumPad3:
                    this.EditCurrentPCG(2, curLine.Y % 8);
                    break;
                case Keys.D4:
                case Keys.NumPad4:
                    this.EditCurrentPCG(3, curLine.Y % 8);
                    break;
                case Keys.D5:
                case Keys.NumPad5:
                    this.EditCurrentPCG(4, curLine.Y % 8);
                    break;
                case Keys.D6:
                case Keys.NumPad6:
                    this.EditCurrentPCG(5, curLine.Y % 8);
                    break;
                case Keys.D7:
                case Keys.NumPad7:
                    this.EditCurrentPCG(6, curLine.Y % 8);
                    break;
                case Keys.D8:
                case Keys.NumPad8:
                    this.EditCurrentPCG(7, curLine.Y % 8);
                    break;
                case Keys.Oemplus:
                case Keys.Add:
                case Keys.OemMinus:
                case Keys.Subtract:
                case Keys.OemCloseBrackets:
                case Keys.OemOpenBrackets:
                    int current_pcg = curPCG.Y * 32 + curPCG.X;
                    int current_target_pcg = (current_pcg + curLine.X + (curLine.Y / 8) * 32) % 256;
                    if ((e.KeyData == Keys.Oemplus) || (e.KeyData == Keys.Add))
                    {
                        // Increment foreground color
                        int color = dataSource.GetColorTable(current_target_pcg, curLine.Y % 8, true);
                        color = (color + 1) % 16;
                        dataSource.SetColorTable(current_target_pcg, curLine.Y % 8, color, true, true);
                    }
                    if ((e.KeyData == Keys.OemMinus) || (e.KeyData == Keys.Subtract))
                    {
                        // Decrement foreground color
                        int color = dataSource.GetColorTable(current_target_pcg, curLine.Y % 8, true);
                        color = (color + 15) % 16;
                        dataSource.SetColorTable(current_target_pcg, curLine.Y % 8, color, true, true);
                    }
                    if (e.KeyData == Keys.OemCloseBrackets)
                    {
                        // Increment backgroundcolor
                        int color = dataSource.GetColorTable(current_target_pcg, curLine.Y % 8, false);
                        color = (color + 1) % 16;
                        dataSource.SetColorTable(current_target_pcg, curLine.Y % 8, color, false, true);
                    }
                    if (e.KeyData == Keys.OemOpenBrackets)
                    {
                        // Decrement background color
                        int color = dataSource.GetColorTable(current_target_pcg, curLine.Y % 8, false);
                        color = (color + 15) % 16;
                        dataSource.SetColorTable(current_target_pcg, curLine.Y % 8, color, false, true);
                    }
                    this.RefreshAllViews();
                    break;
            }
        }
        private void panelPCG_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            Action refresh = () =>
            {
                this.UpdatePCGList();
                this.UpdatePCGEditView();
                this.UpdateCurrentColorView();
            };
            switch (e.KeyData)
            {
                case Keys.Up | Keys.Shift:
                    if (curPCG.ToY > 0)
                    {
                        curPCG.ToY--;
                        refresh();
                    }
                    break;
                case Keys.Down | Keys.Shift:
                    if (curPCG.ToY < 7)
                    {
                        curPCG.ToY++;
                        refresh();
                    }
                    break;
                case Keys.Left | Keys.Shift:
                    if (curPCG.ToX > 0)
                    {
                        curPCG.ToX--;
                        refresh();
                    }
                    break;
                case Keys.Right | Keys.Shift:
                    if (curPCG.ToX < 31)
                    {
                        curPCG.ToX++;
                        refresh();
                    }
                    break;
                case Keys.Up:
                    if (curPCG.Y > 0)
                    {
                        curPCG.Y--;
                        curPCG.ResetSelection();
                        refresh();
                    }
                    break;
                case Keys.Down:
                    if (curPCG.Y < 7)
                    {
                        curPCG.Y++;
                        curPCG.ResetSelection();
                        refresh();
                    }
                    break;
                case Keys.Left:
                    if (curPCG.X > 0)
                    {
                        curPCG.X--;
                        curPCG.ResetSelection();
                        refresh();
                    }
                    break;
                case Keys.Right:
                    if (curPCG.X < 31)
                    {
                        curPCG.X++;
                        curPCG.ResetSelection();
                        refresh();
                    }
                    break;
                case Keys.Enter:
                    dataSource.SetNameTable(curSand.Y * 32 + curSand.X,
                                            curPCG.Y * 32 + curPCG.X, true);
                    if (curSand.X < 31) curSand.X++;
                    this.UpdateSandbox();
                    break;
            }
        }
        private void panelSandbox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Up | Keys.Shift:
                    if (curSand.ToY > 0)
                    {
                        curSand.ToY--;
                        this.UpdateSandbox();
                    }
                    break;
                case Keys.Down | Keys.Shift:
                    if (curSand.ToY < 23)
                    {
                        curSand.ToY++;
                        this.UpdateSandbox();
                    }
                    break;
                case Keys.Left | Keys.Shift:
                    if (curSand.ToX > 0)
                    {
                        curSand.ToX--;
                        this.UpdateSandbox();
                    }
                    break;
                case Keys.Right | Keys.Shift:
                    if (curSand.ToX < 31)
                    {
                        curSand.ToX++;
                        this.UpdateSandbox();
                    }
                    break;
                case Keys.Up:
                    if (curSand.Y > 0)
                    {
                        curSand.Y--;
                        curSand.ResetSelection();
                        this.UpdateSandbox();
                    }
                    break;
                case Keys.Down:
                    if (curSand.Y < 23)
                    {
                        curSand.Y++;
                        curSand.ResetSelection();
                        this.UpdateSandbox();
                    }
                    break;
                case Keys.Left:
                    if (curSand.X > 0)
                    {
                        curSand.X--;
                        curSand.ResetSelection();
                        this.UpdateSandbox();
                    }
                    break;
                case Keys.Right:
                    if (curSand.X < 31)
                    {
                        curSand.X++;
                        curSand.ResetSelection();
                        this.UpdateSandbox();
                    }
                    break;
                case Keys.Enter:
                    dataSource.SetNameTable(curSand.Y * 32 + curSand.X,
                                            curPCG.Y * 32 + curPCG.X, true);
                    if (curSand.X < 31) ++curSand.X;
                    this.UpdateSandbox();
                    break;
            }
        }
    }
    public partial class Map : Form
    {
        protected override bool ProcessDialogKey(Keys keyData)
        {
            switch (keyData)
            {
                // prevent focus movement by the cursor
                case Keys.Down:
                case Keys.Right:
                case Keys.Up:
                case Keys.Left:
                case Keys.Down | Keys.Shift:
                case Keys.Right | Keys.Shift:
                case Keys.Up | Keys.Shift:
                case Keys.Left | Keys.Shift:
                    break;
                default:
                    return base.ProcessDialogKey(keyData);
            }
            return true;
        }
        private void panelPCG_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Up:
                    if (curPCG.Y > 0)
                    {
                        curPCG.Y--;
                        this.UpdatePCGList();
                    }
                    break;
                case Keys.Left:
                    if (curPCG.X > 0)
                    {
                        curPCG.X--;
                        this.UpdatePCGList();
                    }
                    break;
                case Keys.Right:
                    if (curPCG.X < 31)
                    {
                        curPCG.X++;
                        this.UpdatePCGList();
                    }
                    break;
                case Keys.Down:
                    if (curPCG.Y < 7)
                    {
                        curPCG.Y++;
                        this.UpdatePCGList();
                    }
                    break;
                case Keys.Enter:
                    dataSource.SetPCGInPattern(curPtn.Y * 16 + curPtn.X,
                                             curCellInPtn.X + curCellInPtn.Y * 2,
                                             curPCG.Y * 32 + curPCG.X, true);
                    this.UpdateMapPatterns();
                    this.UpdateMap();
                    break;
            }
        }
        private void panelPatterns_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Up | Keys.Shift:
                    if (curPtn.ToY > 0)
                    {
                        curPtn.ToY--;
                        if (curCellInPtn.Y > 0) curCellInPtn.Y = 0;
                        this.UpdateMapPatterns();
                    }
                    break;
                case Keys.Down | Keys.Shift:
                    if (curPtn.ToY < 15)
                    {
                        curPtn.ToY++;
                        if (curCellInPtn.Y < 1) curCellInPtn.Y = 1;
                        this.UpdateMapPatterns();
                    }
                    break;
                case Keys.Left | Keys.Shift:
                    if (curPtn.ToX > 0)
                    {
                        curPtn.ToX--;
                        if (curCellInPtn.X > 0) curCellInPtn.X = 0;
                        this.UpdateMapPatterns();
                    }
                    break;
                case Keys.Right | Keys.Shift:
                    if (curPtn.ToX < 15)
                    {
                        curPtn.ToX++;
                        if (curCellInPtn.X < 1) curCellInPtn.X = 1;
                        this.UpdateMapPatterns();
                    }
                    break;
                case Keys.Up:
                    if (curCellInPtn.Y > 0)
                    {
                        curCellInPtn.Y--;
                        this.UpdateMapPatterns();
                    }
                    else if (curPtn.Y > 0)
                    {
                        curPtn.Y--;
                        curPtn.ResetSelection();
                        this.UpdateMapPatterns();
                    }
                    break;
                case Keys.Down:
                    if (curCellInPtn.Y == 0)
                    {
                        curCellInPtn.Y++;
                        this.UpdateMapPatterns();
                    }
                    else if (curPtn.Y < 15)
                    {
                        curPtn.Y++;
                        curPtn.ResetSelection();
                        this.UpdateMapPatterns();
                    }
                    break;
                case Keys.Left:
                    if (curCellInPtn.X > 0)
                    {
                        curCellInPtn.X--;
                        this.UpdateMapPatterns();
                    }
                    else if (curPtn.X > 0)
                    {
                        curPtn.X--;
                        curPtn.ResetSelection();
                        this.UpdateMapPatterns();
                    }
                    break;
                case Keys.Right:
                    if (curCellInPtn.X == 0)
                    {
                        curCellInPtn.X++;
                        this.UpdateMapPatterns();
                    }
                    else if (curPtn.X < 15)
                    {
                        curPtn.X++;
                        curPtn.ResetSelection();
                        this.UpdateMapPatterns();
                    }
                    break;
                case Keys.Enter:
                    int current_ptn = curPtn.X + curPtn.Y * 16;
                    dataSource.SetMapData(curMapOrg.X + curMap.X,
                                          curMapOrg.Y + curMap.Y,
                                          current_ptn, true);
                    int prev_x = curMap.X;
                    int prev_y = curMap.Y;
                    curMap.X = (curMap.X + 1) % 16;
                    if (curMap.X == 0) curMap.Y = (curMap.Y + 1) % 12;
                    curPtn.ResetSelection();
                    this.UpdateMap();
                    break;
            }
        }
        private void panelMap_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Up | Keys.Shift:
                    if (curMap.ToY > 0)
                    {
                        curMap.ToY--;
                        this.UpdateMap();
                    }
                    break;
                case Keys.Down | Keys.Shift:
                    if (curMap.ToY < 11)
                    {
                        curMap.ToY++;
                        this.UpdateMap();
                    }
                    break;
                case Keys.Left | Keys.Shift:
                    if (curMap.ToX > 0)
                    {
                        curMap.ToX--;
                        this.UpdateMap();
                    }
                    break;
                case Keys.Right | Keys.Shift:
                    if (curMap.ToX < 15)
                    {
                        curMap.ToX++;
                        this.UpdateMap();
                    }
                    break;
                case Keys.Up:
                    if (curMap.Y == 0)
                    {
                        if (curMapOrg.Y > 0) curMapOrg.Y--;
                    }
                    else curMap.Y--;
                    curMap.ResetSelection();
                    this.UpdateMap();
                    break;
                case Keys.Down:
                    if (curMap.Y == 11)
                    {
                        if (curMapOrg.Y < dataSource.MapHeight - 12) curMapOrg.Y++;
                    }
                    else curMap.Y++;
                    curMap.ResetSelection();
                    this.UpdateMap();
                    break;
                case Keys.Left:
                    if (curMap.X == 0)
                    {
                        if (curMapOrg.X > 0) curMapOrg.X--;
                    }
                    else curMap.X--;
                    curMap.ResetSelection();
                    this.UpdateMap();
                    break;
                case Keys.Right:
                    if (curMap.X == 15)
                    {
                        if (curMapOrg.X < dataSource.MapWidth - 16) curMapOrg.X++;
                    }
                    else curMap.X++;
                    curMap.ResetSelection();
                    this.UpdateMap();
                    break;
            }
        }
    }
    public partial class Sprites : Form
    {
        protected override bool ProcessDialogKey(Keys keyData)
        {
            switch (keyData)
            {
                // prevent focus movement by the cursor
                case Keys.Down:
                case Keys.Right:
                case Keys.Up:
                case Keys.Left:
                case Keys.Down | Keys.Shift:
                case Keys.Right | Keys.Shift:
                case Keys.Up | Keys.Shift:
                case Keys.Left | Keys.Shift:
                    break;
                default:
                    return base.ProcessDialogKey(keyData);
            }
            return true;
        }
        private void panelSprites_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Up | Keys.Shift:
                    if (curSpr.ToY > 0)
                    {
                        curSpr.ToY--;
                        this.RefreshAllViews();
                    }
                    break;
                case Keys.Down | Keys.Shift:
                    if (curSpr.ToY < 7)
                    {
                        curSpr.ToY++;
                        this.RefreshAllViews();
                    }
                    break;
                case Keys.Left | Keys.Shift:
                    if (curSpr.ToX > 0)
                    {
                        curSpr.ToX--;
                        this.RefreshAllViews();
                    }
                    break;
                case Keys.Right | Keys.Shift:
                    if (curSpr.ToX < 7)
                    {
                        curSpr.ToX++;
                        this.RefreshAllViews();
                    }
                    break;
                case Keys.Up:
                    if (curSpr.Y > 0)
                    {
                        curSpr.Y--;
                        curSpr.ResetSelection();
                        this.RefreshAllViews();
                    }
                    break;
                case Keys.Down:
                    if (curSpr.Y < 7)
                    {
                        curSpr.Y++;
                        curSpr.ResetSelection();
                        this.RefreshAllViews();
                    }
                    break;
                case Keys.Left:
                    if (curSpr.X > 0)
                    {
                        curSpr.X--;
                        curSpr.ResetSelection();
                        this.RefreshAllViews();
                    }
                    break;
                case Keys.Right:
                    if (curSpr.X < 7)
                    {
                        curSpr.X++;
                        curSpr.ResetSelection();
                        this.RefreshAllViews();
                    }
                    break;
            }
        }
        private void panelEditor_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            Action refresh = () =>
            {
                this.UpdateSpriteEditView();
                this.UpdateCurrentColorView();
            };
            switch (e.KeyData)
            {
                // Multiple selection
                case Keys.Up | Keys.Shift:
                    if (curLine.ToY > 0)
                    {
                        curLine.ToY--;
                        refresh();
                    }
                    break;
                case Keys.Down | Keys.Shift:
                    if (curLine.ToY < 15)
                    {
                        curLine.ToY++;
                        refresh();
                    }
                    break;
                case Keys.Left | Keys.Shift:
                    if (curLine.ToX > 0)
                    {
                        curLine.ToX--;
                        refresh();
                    }
                    break;
                case Keys.Right | Keys.Shift:
                    if (curLine.ToX < 1)
                    {
                        curLine.ToX++;
                        refresh();
                    }
                    break;
                // Moving current selection
                case Keys.Up:
                    if (curLine.Y > 0)
                    {
                        curLine.Y--;
                        curLine.ResetSelection();
                        refresh();
                    }
                    break;
                case Keys.Down:
                    if (curLine.Y < 15)
                    {
                        curLine.Y++;
                        curLine.ResetSelection();
                        refresh();
                    }
                    break;
                case Keys.Left:
                    if ((currentDot == 0) && (curLine.X > 0))
                    {
                        curLine.X--;
                        currentDot = 7;
                        curLine.ResetSelection();
                        refresh();
                    }
                    else if (currentDot > 0)
                    {
                        currentDot--;
                        refresh();
                    }
                    break;
                case Keys.Right:
                    if ((currentDot == 7) && (curLine.X < 1))
                    {
                        curLine.X++;
                        currentDot = 0;
                        curLine.ResetSelection();
                        refresh();
                    }
                    else if (currentDot < 7)
                    {
                        currentDot++;
                        refresh();
                    }
                    break;
                // Editing
                case Keys.Space:
                    // toggle the color of selected pixel
                    this.EditCurrentSprite(currentDot, curLine.Y % 8);
                    break;
                // Changing colors
                case Keys.D1:
                case Keys.NumPad1:
                    this.EditCurrentSprite(0, curLine.Y % 8);
                    break;
                case Keys.D2:
                case Keys.NumPad2:
                    this.EditCurrentSprite(1, curLine.Y % 8);
                    break;
                case Keys.D3:
                case Keys.NumPad3:
                    this.EditCurrentSprite(2, curLine.Y % 8);
                    break;
                case Keys.D4:
                case Keys.NumPad4:
                    this.EditCurrentSprite(3, curLine.Y % 8);
                    break;
                case Keys.D5:
                case Keys.NumPad5:
                    this.EditCurrentSprite(4, curLine.Y % 8);
                    break;
                case Keys.D6:
                case Keys.NumPad6:
                    this.EditCurrentSprite(5, curLine.Y % 8);
                    break;
                case Keys.D7:
                case Keys.NumPad7:
                    this.EditCurrentSprite(6, curLine.Y % 8);
                    break;
                case Keys.D8:
                case Keys.NumPad8:
                    this.EditCurrentSprite(7, curLine.Y % 8);
                    break;
                case Keys.Oemplus:
                case Keys.Add:
                case Keys.OemMinus:
                case Keys.Subtract:
                    {
                        int sprite_num_16x16 = curSpr.Y * 8 + curSpr.X;
                        int sprite_num_8x8 = sprite_num_16x16 * 4 + curLine.X * 2 + curLine.Y / 8;
                        int color_code_primary = dataSource.GetSpriteColorCode(sprite_num_8x8, curLine.Y % 8);
                        if ((e.KeyData == Keys.Oemplus) || (e.KeyData == Keys.Add))
                        {
                            // Increment color of the primary sprite
                            if (color_code_primary < 15) color_code_primary++;
                        }
                        else
                        {
                            // Decrement color of the primary sprite
                            if (color_code_primary > 1) color_code_primary--;
                        }
                        this.SetSpriteColor(sprite_num_16x16, color_code_primary);
                        this.RefreshAllViews();
                    }
                    break;
                case Keys.OemCloseBrackets:
                case Keys.OemOpenBrackets:
                    // Check overlays
                    if (dataSource.GetSpriteOverlay(curSpr.Y * 8 + curSpr.X))
                    {
                        int sprite_num_16x16 = curSpr.Y * 8 + curSpr.X;
                        int sprite_num_8x8 = sprite_num_16x16 * 4 + curLine.X * 2 + curLine.Y / 8;
                        int sprite_num_8x8_secondary = (sprite_num_8x8 + 4) % 256;
                        int color_code_secondary = dataSource.GetSpriteColorCode(sprite_num_8x8_secondary, curLine.Y % 8);
                        if (e.KeyData == Keys.OemCloseBrackets)
                        {
                            // Increment color of the secondary sprite
                            if (color_code_secondary < 15) color_code_secondary++;
                        }
                        else
                        {
                            // Decrement color of the secondary sprite
                            if (color_code_secondary > 1) color_code_secondary--;
                        }
                        this.SetSpriteColor(sprite_num_8x8_secondary / 4, color_code_secondary);
                        this.RefreshAllViews();
                    }
                    break;
            }
        }
        private void panelColor_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Left:
                    if (currentColor.X > 0)
                    {
                        currentColor.X--;
                        this.UpdateCurrentColorView();
                    }
                    break;
                case Keys.Right:
                    if (currentColor.X < 1 && viewColorR.Visible)
                    {
                        currentColor.X++;
                        this.UpdateCurrentColorView();
                    }
                    break;
                case Keys.Space:
                case Keys.Enter:
                    if (currentColor.X == 0)
                    {
                        this.viewColorL_Click(null, null);
                    }
                    else if (currentColor.X == 1)
                    {
                        this.viewColorR_Click(null, null);
                    }
                    break;
            }
        }
        private void panelPalette_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    if (curPal.Y > 0)
                    {
                        curPal.Y--;
                    }
                    this.UpdatePaletteView();
                    break;
                case Keys.Down:
                    if (curPal.Y < 1)
                    {
                        curPal.Y++;
                    }
                    this.UpdatePaletteView();
                    break;
                case Keys.Left:
                    if (curPal.X > 0)
                    {
                        curPal.X--;
                    }
                    this.UpdatePaletteView();
                    break;
                case Keys.Right:
                    if (curPal.X < 7)
                    {
                        curPal.X++;
                    }
                    this.UpdatePaletteView();
                    break;
                case Keys.Space:
                case Keys.Enter:
                    this.EditPalette(curPal.Y * 8 + curPal.X);
                    break;
            }
        }
    }
}