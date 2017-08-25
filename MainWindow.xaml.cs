/*------------------------------------------------
 * Pavel Khrapkin NIP Informatica 25.08.2017
 * 
 * IfcRepair - read input Ifc file and correct it with
 * unique set of guids in output file out.ifc by add to
 * non-unique Guids random 4 char string.
 * Non-unique Guids sometimees created in MagicCAD, when
 * ifc file make Guid with the point ('.') inside it.
 * Tekla ignore part of guid after '.', which make wrong ifc.
 */
using System;
using System.IO;
using System.Collections.Generic;
using System.Windows;

namespace IfcGuidRepair
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public string path = string.Empty;

        static StreamReader file;
        static StreamWriter outFile;

        static HashSet<string> guids = new HashSet<string>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Browse_click(object sender, RoutedEventArgs e)
        {
            string path = string.Empty;
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".ifc";

            // Display OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                FileNameTextBox.Text = filename;
            }
        }

        public void HandleIfc()
        {
            if (!File.Exists(path)) throw new Exception("No file \"" + path + "\" exists");
            string dir = Path.GetDirectoryName(path);
            string outPath = Path.Combine(dir, "out.ifc");

            ///           string outPath = @"C:\Users\khrapkin\Desktop\out.ifc";
            outFile = new StreamWriter(outPath);

            int counter = 0;
            string line;

            // Read the file and display it line by line.
            StreamReader file = new System.IO.StreamReader(path);
            while ((line = file.ReadLine()) != null)
            {
                if (!isIfcProxy(line))
                {
                    writeLine(line);
                    continue;
                }
                string ifcGuid = guid(line);
                if (guids.Contains(ifcGuid)) ifcGuid = makeUniqueId(ifcGuid);
                guids.Add(ifcGuid);
                writeLine(line, ifcGuid);
                Console.WriteLine(id(line) + " " + ifcGuid);

                counter++;
            }
            Console.WriteLine("=== Total " + counter + " guids ===");
            file.Close();

            // Suspend the screen.
            Console.ReadLine();
        }

        private static void writeLine(string line, string ifcGuid = "")
        {
            if (ifcGuid != "")
            {
                line = line.Replace(guid(line), ifcGuid);
            }
            outFile.WriteLine(line);
        }

        private static string makeUniqueId(string ifcGuid)
        {
            string uniqId = ifcGuid + Path.GetRandomFileName().Replace(".", "").Substring(0, 4);
            if (guids.Contains(uniqId)) ifcGuid = makeUniqueId(uniqId);
            return uniqId;
        }

        private static string id(string line)
        {
            int n = line.IndexOf("=");
            return line.Substring(0, n);
        }

        private static string guid(string line)
        {
            string prefix = "=IFCBUILDINGELEMENTPROXY('";
            string postfix = "',#";
            int i0 = line.IndexOf(prefix) + prefix.Length;
            int lng = line.IndexOf(postfix, i0) - i0;
            return line.Substring(i0, lng);
        }

        private static bool isIfcProxy(string line)
        {
            return line.Contains("IFCBUILDINGELEMENTPROXY");
        }
    }
}
