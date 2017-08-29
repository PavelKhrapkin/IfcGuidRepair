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
using System.Globalization;

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
        string reportPath;

        static StreamReader inFile;
        static StreamWriter outFile;
        static StreamWriter reportFile;
        Process notepad;

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
            dlg.InitialDirectory = DirPath;

            // Display OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                // Open document
                InPath = dlg.FileName;
                InFileNameTextBox.Text = Path.GetFileName(InPath);
                DirPath = Path.GetDirectoryName(InPath);
            }
        }

        private void Out_Browse_click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".ifc";
            dlg.InitialDirectory = DirPath;

            // Display OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                // Open document
                OutPath = dlg.FileName;
                OutFileNameTextBox.Text = Path.GetFileName(OutPath);
            }
        }

        public void HandleIfc()
        {
            if (!File.Exists(InPath)) throw new Exception("No file \"" + InPath + "\" exists");

            //always write outFilein the inFile directory
            string outName = Path.GetFileName(OutPath);
            OutPath = Path.Combine(DirPath, outName);

            outFile = new StreamWriter(OutPath);
            reportPath = Path.Combine(DirPath, reportName);
            reportFile = new StreamWriter(reportPath);
            reportFile.WriteLine("=== IfcGuidRepair Report File");
            reportFile.WriteLine("Input=\t" + InPath);
            reportFile.WriteLine("Output=\t" + OutPath);
            reportFile.WriteLine("Report=\t" + reportPath);

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
            outFile.Close();
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
            HandleIfc();
            notepad = Process.Start("notepad", reportPath);
        }

        private void OK_button_Click(object sender, RoutedEventArgs e)
        {
            if(notepad != null) try { notepad.Kill(); }  catch { }
            try { HandleIfc(); } catch { }
            Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("About.mht");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string help = "IfcGuidRepair_EN_Help.mht";
            CultureInfo ci = CultureInfo.InstalledUICulture;
            if(ci.CompareInfo.Name == "ru-RU") help = "IfcGuidRepair_RU_Help.mht";
            Process.Start(help);
        }
    }
}