using System;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace ElBilliard
{
    public partial class LoadLogo : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindowEx(IntPtr parent, IntPtr next, string sClassName, string sWindowTitle);
        [DllImport("User32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        Image image;
        System.Windows.Forms.Timer gifTimer = new System.Windows.Forms.Timer();

        public LoadLogo()
        {
            gifTimer.Interval = 3000;
            gifTimer.Tick +=new EventHandler(gifTimer_Tick);
            gifTimer.Start();
            Shown += new EventHandler(LoadLogo_Shown);
            if (File.Exists("logo.gif"))
                image = Image.FromFile("logo.gif"); 
            InitializeComponent();
        }

        private void LoadLogo_Shown(object sender, EventArgs e)
        {
            try
            {
                Program.rEvent.WaitOne();
            }
            catch (ThreadAbortException err)
            {
                MessageBox.Show(err.Data.ToString());
                Close();
            }
            if (image != null)
                pictureBox1.Image = image;
            this.Size = new Size(pictureBox1.Width, pictureBox1.Height);
            this.Activate();
        }

        private void gifTimer_Tick(object sender, EventArgs e)
        {
            IntPtr hWnd = IntPtr.Zero;
            hWnd = FindWindowEx(IntPtr.Zero, IntPtr.Zero, null, "BILLIARD");
            if (hWnd != IntPtr.Zero)
            {
                ShowWindow(hWnd, 2);
                ShowWindow(hWnd, 1);
                Close();
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Dispose();
        }
    }
}
