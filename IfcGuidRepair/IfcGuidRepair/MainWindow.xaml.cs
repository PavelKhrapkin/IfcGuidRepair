/*------------------------------------------------
 * Pavel Khrapkin NIP Informatica 28.08.2017
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
using System.Diagnostics;

namespace IfcGuidRepair
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public string InPath;
        public string OutPath;
        public string DirPath;
        string reportName = "Report.log";

        static StreamReader inFile;
        static StreamWriter outFile;
        static StreamWriter reportFile;

        public bool HandlingComplete = false;

        static HashSet<string> guids = new HashSet<string>();

        public MainWindow()
        {
            InitializeComponent();
            if(string.IsNullOrWhiteSpace(DirPath) || !Directory.Exists(DirPath))
                DirPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string[] fileEntries = Directory.GetFiles(DirPath, "*.ifc");
            InPath = fileEntries[0];
            InFileNameTextBox.Text = Path.GetFileName(InPath);
            if (string.IsNullOrWhiteSpace(OutPath))
                OutPath = Path.Combine(DirPath, "out.ifc");
            OutFileNameTextBox.Text = Path.GetFileName(OutPath);
        }

        private void In_Browse_click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".ifc";

            // Display OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                dlg.InitialDirectory = DirPath;
                InFileNameTextBox.Text = Path.GetFileName(filename);
                DirPath = Path.GetDirectoryName(filename);
            }
        }

        private void Out_Browse_click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".ifc";

            // Display OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                dlg.InitialDirectory = DirPath;
                OutFileNameTextBox.Text = filename;
            }
        }

        public void HandleIfc()
        {
            if (!File.Exists(InPath)) throw new Exception("No file \"" + InPath + "\" exists");

            outFile = new StreamWriter(OutPath);
            reportFile = new StreamWriter(Path.Combine(DirPath, reportName));
            reportFile.WriteLine("=== Report File: Input=" + InPath + ", Output=" + OutPath);

            int totalGuidsCounter = 0, nonUniqueGuidsCounter = 0;
            string line;

            // Read the file and display it line by line.
            inFile = new System.IO.StreamReader(InPath);
            while ((line = inFile.ReadLine()) != null)
            {
                if (!isIfcProxy(line))
                {
                    writeLine(line);
                    continue;
                }
                string ifcGuid = guid(line);
                if (guids.Contains(ifcGuid))
                {
                    ifcGuid = makeUniqueId(ifcGuid);
                    nonUniqueGuidsCounter++;
                }
                guids.Add(ifcGuid);
                writeLine(line, ifcGuid);
                reportFile.WriteLine(id(line) + " " + ifcGuid);

                totalGuidsCounter++;
            }
            reportFile.WriteLine("=== Total Guids=" + totalGuidsCounter + 
                " guids, total repaired=" + nonUniqueGuidsCounter +" ===");
            inFile.Close();
            reportFile.Close();
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

        private void Report_click(object sender, RoutedEventArgs e)
        {
            string repPath = Path.Combine(DirPath, reportName);
            HandleIfc();
            Process.Start("notepad", repPath);
        }

        private void OK_button_Click(object sender, RoutedEventArgs e)
        {
            HandleIfc();
            Close();
        }
    }
}