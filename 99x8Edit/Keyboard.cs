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
    public partial class SpriteEditor : Form
    {
        private void panelEditor_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            // Key events in sprite editor
            switch (e.KeyData)
            {
                case Keys.Oemplus:
                case Keys.Add:
                case Keys.OemMinus:
                case Keys.Subtract:
                    {
                        int index16 = viewSprite.Index;
                        int color_code = dataSource.GetSpriteColorCode(index16, viewEdit.Y);
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
                        dataSource.SetSpriteColorCode(index16, viewEdit.Y, color_code, push: true);
                        this.RefreshAllViews();
                    }
                    break;
                case Keys.OemCloseBrackets:
                case Keys.OemOpenBrackets:
                    // Check overlays
                    if (dataSource.GetSpriteOverlay(viewSprite.Index))
                    {
                        int index16 = (viewSprite.Index + 1) % 64; // For overlayed
                        int color_code = dataSource.GetSpriteColorCode(index16, viewEdit.Y);
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
                        dataSource.SetSpriteColorCode(index16, viewEdit.Y, color_code, push: true);
                        this.RefreshAllViews();
                    }
                    break;
            }
        }
    }
}