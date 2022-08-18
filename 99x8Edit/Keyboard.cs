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
                    viewSand.IncrementSelection();
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
                    viewSand.IncrementSelection();
                    this.UpdateSandbox(refresh: true);
                    break;
            }
        }
    }
    public partial class MapEditor : Form
    {
        private void viewPtn_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            // Key events in pattern list
            switch (e.KeyData)
            {
                case Keys.Enter:
                case Keys.Space:
                    // Keyboard space and enter to send pattern to map
                    dataSource.SetMapData(curMapOrg.X + viewMap.X,
                                          curMapOrg.Y + viewMap.Y,
                                          viewPtn.Index, push: true);
                    viewMap.IncrementSelection();
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