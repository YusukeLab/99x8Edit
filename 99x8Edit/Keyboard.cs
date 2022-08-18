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
        private void viewEdit_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            // Key events in character editor
            switch (e.KeyData)
            {
                case Keys.Oemplus:
                case Keys.Add:
                case Keys.OemMinus:
                case Keys.Subtract:
                case Keys.OemCloseBrackets:
                case Keys.OemOpenBrackets:
                    int target = viewPCG.Index
                               + (viewEdit.Y / 8) * viewPCG.ColumnNum
                               + viewEdit.X;        // Target character
                    int line = viewEdit.Y % 8;      // Target line
                    int fore = dataSource.GetPCGColor(target, line, foreground: true);
                    int back = dataSource.GetPCGColor(target, line, foreground: false);
                    if ((e.KeyData == Keys.Oemplus) || (e.KeyData == Keys.Add))
                    {
                        // Increment foreground color
                        fore = (fore + 1) % 16;
                        dataSource.SetPCGColor(target, line, fore, isForeGround: true, push: true);
                    }
                    if ((e.KeyData == Keys.OemMinus) || (e.KeyData == Keys.Subtract))
                    {
                        // Decrement foreground color
                        fore = (fore + 15) % 16;
                        dataSource.SetPCGColor(target, line, fore, isForeGround: true, push: true);
                    }
                    if (e.KeyData == Keys.OemCloseBrackets)
                    {
                        // Increment backgroundcolor
                        back = (back + 1) % 16;
                        dataSource.SetPCGColor(target, line, back, isForeGround: false, push: true);
                    }
                    if (e.KeyData == Keys.OemOpenBrackets)
                    {
                        // Decrement background color
                        back = (back + 15) % 16;
                        dataSource.SetPCGColor(target, line, back, isForeGround: false, push: true);
                    }
                    this.RefreshAllViews();
                    break;
            }
        }
        private void viewPCG_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            // Key events in character list
            switch (e.KeyData)
            {
                case Keys.Enter:
                    dataSource.SetNameTable(viewSand.Index, viewPCG.Index, push: true);
                    viewSand.Index++;
                    this.UpdateSandbox(refresh: true);
                    break;
            }
        }
        private void viewSand_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            // Key events in sandbox
            switch (e.KeyData)
            {
                case Keys.Enter:
                    dataSource.SetNameTable(viewSand.Index, viewPCG.Index, push: true);
                    viewSand.Index++;
                    this.UpdateSandbox(refresh: true);
                    break;
            }
        }
    }
    public partial class Map : Form
    {
        private void panelPCG_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            // Key events in character list
            switch (e.KeyData)
            {
                case Keys.Up:
                    if (curPCG.Y > 0)
                    {
                        curPCG.Y--;
                        this.UpdatePCGList(refresh: true);
                    }
                    break;
                case Keys.Left:
                    if (curPCG.X > 0)
                    {
                        curPCG.X--;
                        this.UpdatePCGList(refresh: true);
                    }
                    break;
                case Keys.Right:
                    if (curPCG.X < 31)
                    {
                        curPCG.X++;
                        this.UpdatePCGList(refresh: true);
                    }
                    break;
                case Keys.Down:
                    if (curPCG.Y < 7)
                    {
                        curPCG.Y++;
                        this.UpdatePCGList(refresh: true);
                    }
                    break;
                case Keys.Enter:
                    dataSource.SetPCGInPattern(curPtn.Y * 16 + curPtn.X,
                                             curCellInPtn.X + curCellInPtn.Y * 2,
                                             curPCG.Y * 32 + curPCG.X, push: true);
                    this.UpdateMapPatterns(refresh: true);
                    this.UpdateMap(refresh: true);
                    break;
            }
        }
        private void panelPatterns_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            // Key events in pattern list
            switch (e.KeyData)
            {
                case Keys.Up | Keys.Shift:
                    if (curPtn.ToY > 0)
                    {
                        curPtn.ToY--;
                        if (curCellInPtn.Y > 0) curCellInPtn.Y = 0;
                        this.UpdateMapPatterns(refresh: true);
                    }
                    break;
                case Keys.Down | Keys.Shift:
                    if (curPtn.ToY < 15)
                    {
                        curPtn.ToY++;
                        if (curCellInPtn.Y < 1) curCellInPtn.Y = 1;
                        this.UpdateMapPatterns(refresh: true);
                    }
                    break;
                case Keys.Left | Keys.Shift:
                    if (curPtn.ToX > 0)
                    {
                        curPtn.ToX--;
                        if (curCellInPtn.X > 0) curCellInPtn.X = 0;
                        this.UpdateMapPatterns(refresh: true);
                    }
                    break;
                case Keys.Right | Keys.Shift:
                    if (curPtn.ToX < 15)
                    {
                        curPtn.ToX++;
                        if (curCellInPtn.X < 1) curCellInPtn.X = 1;
                        this.UpdateMapPatterns(refresh: true);
                    }
                    break;
                case Keys.Up:
                    if (curCellInPtn.Y > 0)
                    {
                        curCellInPtn.Y--;
                        this.UpdateMapPatterns(refresh: true);
                    }
                    else if (curPtn.Y > 0)
                    {
                        curPtn.Y--;
                        this.UpdateMapPatterns(refresh: true);
                    }
                    break;
                case Keys.Down:
                    if (curCellInPtn.Y == 0)
                    {
                        curCellInPtn.Y++;
                        this.UpdateMapPatterns(refresh: true);
                    }
                    else if (curPtn.Y < 15)
                    {
                        curPtn.Y++;
                        this.UpdateMapPatterns(refresh: true);
                    }
                    break;
                case Keys.Left:
                    if (curCellInPtn.X > 0)
                    {
                        curCellInPtn.X--;
                        this.UpdateMapPatterns(refresh: true);
                    }
                    else if (curPtn.X > 0)
                    {
                        curPtn.X--;
                        this.UpdateMapPatterns(refresh: true);
                    }
                    break;
                case Keys.Right:
                    if (curCellInPtn.X == 0)
                    {
                        curCellInPtn.X++;
                        this.UpdateMapPatterns(refresh: true);
                    }
                    else if (curPtn.X < 15)
                    {
                        curPtn.X++;
                        this.UpdateMapPatterns(refresh: true);
                    }
                    break;
                case Keys.Enter:
                    int current_ptn = curPtn.X + curPtn.Y * 16;
                    dataSource.SetMapData(curMapOrg.X + curMap.X,
                                          curMapOrg.Y + curMap.Y,
                                          current_ptn, push: true);
                    int prev_x = curMap.X;
                    int prev_y = curMap.Y;
                    curMap.X = (curMap.X + 1) % 16;
                    if (curMap.X == 0) curMap.Y = (curMap.Y + 1) % 12;
                    this.UpdateMap(refresh: true);
                    break;
            }
        }
        private void panelMap_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            // Key events in map editor
            switch (e.KeyData)
            {
                case Keys.Up | Keys.Shift:
                    if (curMap.ToY > 0)
                    {
                        curMap.ToY--;
                        this.UpdateMap(refresh: true);
                    }
                    break;
                case Keys.Down | Keys.Shift:
                    if (curMap.ToY < 11)
                    {
                        curMap.ToY++;
                        this.UpdateMap(refresh: true);
                    }
                    break;
                case Keys.Left | Keys.Shift:
                    if (curMap.ToX > 0)
                    {
                        curMap.ToX--;
                        this.UpdateMap(refresh: true);
                    }
                    break;
                case Keys.Right | Keys.Shift:
                    if (curMap.ToX < 15)
                    {
                        curMap.ToX++;
                        this.UpdateMap(refresh: true);
                    }
                    break;
                case Keys.Up:
                    if (curMap.Y == 0)
                    {
                        if (curMapOrg.Y > 0) curMapOrg.Y--;
                    }
                    else curMap.Y--;
                    this.UpdateMap(refresh: true);
                    break;
                case Keys.Down:
                    if (curMap.Y == 11)
                    {
                        if (curMapOrg.Y < dataSource.MapHeight - 12) curMapOrg.Y++;
                    }
                    else curMap.Y++;
                    this.UpdateMap(refresh: true);
                    break;
                case Keys.Left:
                    if (curMap.X == 0)
                    {
                        if (curMapOrg.X > 0) curMapOrg.X--;
                    }
                    else curMap.X--;
                    this.UpdateMap(refresh: true);
                    break;
                case Keys.Right:
                    if (curMap.X == 15)
                    {
                        if (curMapOrg.X < dataSource.MapWidth - 16) curMapOrg.X++;
                    }
                    else curMap.X++;
                    this.UpdateMap(refresh: true);
                    break;
            }
        }
    }
    public partial class Sprites : Form
    {
        private void panelSprites_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            // Key events in sprite list
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
                        this.RefreshAllViews();
                    }
                    break;
                case Keys.Down:
                    if (curSpr.Y < 7)
                    {
                        curSpr.Y++;
                        this.RefreshAllViews();
                    }
                    break;
                case Keys.Left:
                    if (curSpr.X > 0)
                    {
                        curSpr.X--;
                        this.RefreshAllViews();
                    }
                    break;
                case Keys.Right:
                    if (curSpr.X < 7)
                    {
                        curSpr.X++;
                        this.RefreshAllViews();
                    }
                    break;
            }
        }
        private void panelEditor_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            // Key events in sprite editor
            Action refresh = () =>
            {
                this.UpdateSpriteEditView(refresh: true);
                this.UpdateCurrentColorView(refresh: true);
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
                        refresh();
                    }
                    break;
                case Keys.Down:
                    if (curLine.Y < 15)
                    {
                        curLine.Y++;
                        refresh();
                    }
                    break;
                case Keys.Left:
                    if ((currentDot == 0) && (curLine.X > 0))
                    {
                        curLine.X--;
                        currentDot = 7;
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
                    this.EditCurrentSprite(curLine.X * 8 + currentDot, curLine.Y);
                    break;
                // Changing colors
                case Keys.D1:
                case Keys.NumPad1:
                    this.EditCurrentSprite(curLine.X * 8 + 0, curLine.Y);
                    break;
                case Keys.D2:
                case Keys.NumPad2:
                    this.EditCurrentSprite(curLine.X * 8 + 1, curLine.Y);
                    break;
                case Keys.D3:
                case Keys.NumPad3:
                    this.EditCurrentSprite(curLine.X * 8 + 2, curLine.Y);
                    break;
                case Keys.D4:
                case Keys.NumPad4:
                    this.EditCurrentSprite(curLine.X * 8 + 3, curLine.Y);
                    break;
                case Keys.D5:
                case Keys.NumPad5:
                    this.EditCurrentSprite(curLine.X * 8 + 4, curLine.Y);
                    break;
                case Keys.D6:
                case Keys.NumPad6:
                    this.EditCurrentSprite(curLine.X * 8 + 5, curLine.Y);
                    break;
                case Keys.D7:
                case Keys.NumPad7:
                    this.EditCurrentSprite(curLine.X * 8 + 6, curLine.Y);
                    break;
                case Keys.D8:
                case Keys.NumPad8:
                    this.EditCurrentSprite(curLine.X * 8 + 7, curLine.Y);
                    break;
                case Keys.Oemplus:
                case Keys.Add:
                case Keys.OemMinus:
                case Keys.Subtract:
                    {
                        int index16 = curSpr.Y * 8 + curSpr.X;
                        int color_code = dataSource.GetSpriteColorCode(index16, curLine.Y);
                        if ((e.KeyData == Keys.Oemplus) || (e.KeyData == Keys.Add))
                        {
                            // Increment color of the primary sprite
                            if (color_code < 15) color_code++;
                        }
                        else
                        {
                            // Decrement color of the primary sprite
                            if (color_code > 1) color_code--;
                        }
                        dataSource.SetSpriteColorCode(index16, curLine.Y, color_code, push: true);
                        this.RefreshAllViews();
                    }
                    break;
                case Keys.OemCloseBrackets:
                case Keys.OemOpenBrackets:
                    // Check overlays
                    if (dataSource.GetSpriteOverlay(curSpr.Y * 8 + curSpr.X))
                    {
                        int index16 = (curSpr.Y * 8 + curSpr.X + 1) % 64; // For overlayed
                        int color_code = dataSource.GetSpriteColorCode(index16, curLine.Y);
                        if (e.KeyData == Keys.OemCloseBrackets)
                        {
                            // Increment color of the secondary sprite
                            if (color_code < 15) color_code++;
                        }
                        else
                        {
                            // Decrement color of the secondary sprite
                            if (color_code > 1) color_code--;
                        }
                        dataSource.SetSpriteColorCode(index16, curLine.Y, color_code, push: true);
                        this.RefreshAllViews();
                    }
                    break;
            }
        }
        private void panelColor_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            // Key events in current color control
            switch (e.KeyData)
            {
                case Keys.Left:
                    if (currentColor.X > 0)
                    {
                        currentColor.X--;
                        this.UpdateCurrentColorView(refresh: true);
                    }
                    break;
                case Keys.Right:
                    if (currentColor.X < 1 && viewColorR.Visible)
                    {
                        currentColor.X++;
                        this.UpdateCurrentColorView(refresh: true);
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
            // Key events in palette control
            switch (e.KeyCode)
            {
                case Keys.Up:
                    if (curPal.Y > 0)
                    {
                        curPal.Y--;
                    }
                    this.UpdatePaletteView(refresh: true);
                    break;
                case Keys.Down:
                    if (curPal.Y < 1)
                    {
                        curPal.Y++;
                    }
                    this.UpdatePaletteView(refresh: true);
                    break;
                case Keys.Left:
                    if (curPal.X > 0)
                    {
                        curPal.X--;
                    }
                    this.UpdatePaletteView(refresh: true);
                    break;
                case Keys.Right:
                    if (curPal.X < 7)
                    {
                        curPal.X++;
                    }
                    this.UpdatePaletteView(refresh: true);
                    break;
                case Keys.Space:
                case Keys.Enter:
                    this.EditPalette(curPal.Y * 8 + curPal.X);
                    break;
            }
        }
    }
}