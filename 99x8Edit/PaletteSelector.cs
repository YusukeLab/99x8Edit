﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace _99x8Edit
{
    // Color selection window
    public partial class PaletteSelector : Form
    {
        private Action<int> colorSelected;
        private Bitmap bmpTransparent = new Bitmap(32, 32);
        public PaletteSelector(Machine source, int previous, Action<int> callback)
        {
            InitializeComponent();
            for (int i = 1; i < 16; ++i)
            {
                Color c = source.ColorOf(i);
                viewPalette.SetBackground(c, i % viewPalette.ColumnNum, i / viewPalette.ColumnNum);
            }
            Utility.DrawTransparent(bmpTransparent);
            viewPalette.SetImage(bmpTransparent, 0, 0);
            colorSelected = callback;
        }
        private void PaletteSelector_Deactivate(object sender, EventArgs e)
        {
            this.Dispose();
        }
        private void viewPalette_CellOnEdit(object sender, EventArgs e)
        {
            int color = viewPalette.Index;
            colorSelected?.Invoke(color);
            this.Dispose();
        }
        private void viewPalette_MouseDown(object sender, MouseEventArgs e)
        {
            int color = viewPalette.Index;
            colorSelected?.Invoke(color);
            this.Dispose();
        }
    }
}
