/*------------------------------------------------
 * Pavel Khrapkin NIP Informatica 28.08.2017
 * 
 * App.xaml.cs - Strar of the Application IfcGuidRepair
 *               Parse of the comand-line arguments, and
 *               start IfsGuidHandling, or if arguments
 *               not present in comand line - start WPF
 */
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
            else
            {
                wnd.HandleIfc();
                Application.Current.Shutdown();
            }
        }
    }
}
