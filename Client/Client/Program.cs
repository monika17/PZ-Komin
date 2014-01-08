using System;
using System.Windows.Forms;

namespace Komin
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [MTAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new KominClientForm());
            }
            catch (ObjectDisposedException) { }
        }
    }
}
