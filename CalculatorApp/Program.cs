using System;
using System.Windows.Forms;
using CalculatorApp.Data;
using CalculatorApp.Forms;

namespace CalculatorApp
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            DatabaseInitializer.Initialize();
            Application.Run(new MainMenuForm());
        }
    }
}
