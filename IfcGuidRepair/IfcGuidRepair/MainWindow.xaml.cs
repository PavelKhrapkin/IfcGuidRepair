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
        public MainWindow()
        {
            InitializeComponent();
            string path = GetFilePath();
            HandleIfc(path);
        }

        private string GetFilePath()
        {
            string path = string.Empty;
            return path;
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

        static StreamReader file;
        static StreamWriter outFile;

        static HashSet<string> guids = new HashSet<string>();
        private void HandleIfc(string path)
        {

        }
    }
}
