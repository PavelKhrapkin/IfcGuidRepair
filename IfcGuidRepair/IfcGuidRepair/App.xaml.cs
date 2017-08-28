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
            if(e.Args.Length > 1)
            {
                wnd.InPath = e.Args[0];
                wnd.DirPath = Path.GetDirectoryName(wnd.InPath);
                wnd.OutPath = e.Args[1];
            }
            if(e.Args.Length == 1)
            {
                wnd.InPath = e.Args[0];
                wnd.DirPath = Path.GetDirectoryName(wnd.InPath);
                wnd.OutPath = Path.Combine(wnd.DirPath, "out.ifc");
            }
            if(e.Args.Length < 1 || !File.Exists(wnd.InPath))
            {
                wnd.Show();
            }
            wnd.HandleIfc();
            Application.Current.Shutdown();
        }
    }
}
