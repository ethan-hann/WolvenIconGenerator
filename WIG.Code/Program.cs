using WolvenIconGenerator.Forms;

namespace WolvenIconGenerator
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            // Show the splash screen as a modal dialog
            using (var splashScreen = new SplashScreen())
            {
                splashScreen.ShowDialog();
            }

            Application.Run(new MainForm());
        }
    }
}