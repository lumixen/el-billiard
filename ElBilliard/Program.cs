using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;

namespace ElBilliard
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static AutoResetEvent _rEvent = new AutoResetEvent(false);
        public static AutoResetEvent rEvent
        {
            get { return _rEvent; }
            set { _rEvent = value; }
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Thread thread = new Thread(new ThreadStart(StartLogo));
            thread.Start();
            Application.Run(new Main());
        }
        static void StartLogo()
        {
            Application.Run(new LoadLogo());
        }
    }
}
