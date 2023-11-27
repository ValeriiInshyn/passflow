using System;
using System.Windows;

namespace WPF_app.Pages
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [STAThread]
        public static void Main()
        {
            App app = new App();
            LoginView mainWindow = new LoginView(); // Replace with your main window class
            app.Run(mainWindow);
        }
    }
}
