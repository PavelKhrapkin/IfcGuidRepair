using System.IO;
using System.Windows;

namespace IfcGuidRepair
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow wnd = new MainWindow();
            if (e.Args.Length == 1)
            {
                wnd.path = e.Args[0];
                if (!File.Exists(wnd.path)) wnd.Show();
                wnd.HandleIfc();
                Application.Current.Shutdown();
            } 
        }
    }
}
