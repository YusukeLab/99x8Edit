using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace _99x8Edit
{
    public partial class About : Form
    {
        // Application information window
        public About()
        {
            // Acquire version info from assembly
            InitializeComponent();
            System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
            Version ver = asm.GetName().Version;
            // Currently it's alpha version
            lblVer.Text = "ver" + ver.ToString() + " alpha";
        }
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo ps = new ProcessStartInfo("https://twitter.com/chocolatechnica")
            {
                UseShellExecute = true,
                Verb = "open"
            };
            Process.Start(ps);
        }
    }
}
