using System;
using System.Windows.Forms;
using Telerik.WinControls;

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
            Application.DefaultFont = new System.Drawing.Font("Segoe UI", 10F);
            
            // Apply Telerik theme
            ThemeResolutionService.ApplicationTheme = "TelerikMetro";
            
            Application.Run(new MainForm());
        }
    }
}
