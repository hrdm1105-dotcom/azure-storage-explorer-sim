using System;
using System.Windows.Forms;

namespace AzureStorageExplorer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.DefaultFont = new System.Drawing.Font("Segoe UI", 9F);
            Application.Run(new MainForm());
        }
    }
}
