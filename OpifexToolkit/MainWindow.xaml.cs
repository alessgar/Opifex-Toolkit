using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace OpifexToolkit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {

        Importer importerOBJ;
        Shrinker shrinkerOBJ;

        public MainWindow()
        {
            InitializeComponent();
            importerOBJ = new Importer();
            shrinkerOBJ = new Shrinker();
        }



        /*
        * 
        * Custom Functions
        * 
        */

        public void showMSGBox(string text, string title)
        {
            MessageBox.Show(text, title);
        }



        /*
        * 
        * WPF FORM ACTIONS (Last function ALWAYS "Start" button) - IMPORTER
        * 
        */

        private void Button_Click_ChooseVMF(object sender, RoutedEventArgs e)
        {
            //VMF Select Button
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".vmf";
            dlg.Filter = "Valve Map Files (*.vmf)|*.vmf|All Files (*)|*";
            dlg.AddExtension = true;
            dlg.CheckFileExists = true;
            dlg.DereferenceLinks = true;
            dlg.Multiselect = false;
            Nullable<bool> result = dlg.ShowDialog();
            if (result.HasValue && result.Value)
            {
                string filename = dlg.FileName;
                ImportMapPathBox.Text = filename;
            }
        }

        private void Button_Click_ChooseLibrary(object sender, RoutedEventArgs e)
        {
            //Steam Library Select Button
            CommonOpenFileDialog cofd = new CommonOpenFileDialog();
            cofd.IsFolderPicker = true;
            if (cofd.ShowDialog() == CommonFileDialogResult.Ok)
            {
                ImportLibraryPathBox.Text = cofd.FileName;
            }
        }

        private void Button_Click_ChooseOutputDir(object sender, RoutedEventArgs e)
        {
            //Content Pack Select Button
            CommonOpenFileDialog cofd = new CommonOpenFileDialog();
            cofd.IsFolderPicker = true;
            if (cofd.ShowDialog() == CommonFileDialogResult.Ok)
            {
                ImportContentPackPathBox.Text = cofd.FileName;
            }
        }

        private void Button_Click_ChooseIncludeDir(object sender, RoutedEventArgs e)
        {
            //Content Pack Select Button
            CommonOpenFileDialog cofd = new CommonOpenFileDialog();
            cofd.IsFolderPicker = true;
            if (cofd.ShowDialog() == CommonFileDialogResult.Ok)
            {
                IncludeContentPackPathBox.Text = cofd.FileName;
            }
        }
        private void ModelStripLOD_Checked(object sender, RoutedEventArgs e)
        {
            RLODFilterBox.IsEnabled = true;
            RLODFilterLabel.IsEnabled = true;
            RLODFilterDesc.IsEnabled = true;
        }

        private void ModelStripLOD_UnChecked(object sender, RoutedEventArgs e)
        {
            RLODFilterBox.IsEnabled = false;
            RLODFilterLabel.IsEnabled = false;
            RLODFilterDesc.IsEnabled = false;
        }

        private void ImportStart_Click(object sender, RoutedEventArgs e)
        {
            //Start Import Button
            //Get all settings and store them in variables, we dont need things changing on us partway through this long process
            string VMFPath = "";
            string SteamLibPath = "";
            string OutputPath = "";
            string IncludePath = "";
            string ContentID = "";

            VMFPath = ImportMapPathBox.Text;
            SteamLibPath = ImportLibraryPathBox.Text;
            OutputPath = ImportContentPackPathBox.Text;
            IncludePath = IncludeContentPackPathBox.Text;
            ContentID = ImportContentIdentifierBox.Text;

            //Check all given values from the user to confirm they will work (or should work)
            ImportStatusText.Foreground = Brushes.Black;
            ImportStatusText.Content = "Checking Parameters";

            ImportStart.IsEnabled = false;

            bool sgi_validated = false;
            if (VMFPath != "")
            {
                if (File.Exists(VMFPath))
                {
                    //apply some steam library fixing incase they dont do the common folder
                    if (SteamLibPath.EndsWith("common") != true)
                    {
                        if (SteamLibPath.EndsWith("steamapps"))
                        {
                            SteamLibPath = SteamLibPath + "\\common";
                        }
                        else
                        {
                            SteamLibPath = SteamLibPath + "\\steamapps\\common";
                        }
                    }
                    if (SteamLibPath != "")
                    {
                        if (Directory.Exists(SteamLibPath))
                        {
                            if (OutputPath != "")
                            {
                                if (Directory.Exists(OutputPath))
                                {
                                    if (IncludePath != "")
                                    {
                                        if (Directory.Exists(IncludePath) == false)
                                        {
                                            IncludePath = "";
                                            ImportStatusText.Content = "Warning: Include Directory is not valid! Skipping include step.";
                                        }
                                    }
                                    if (ContentID != "")
                                    {
                                        Regex CIDReg = new Regex("^[a-zA-Z0-9_-]*$");
                                        MatchCollection CIDMatches = CIDReg.Matches(ContentID);
                                        if (CIDMatches.Count > 0)
                                        {
                                            if (CIDMatches[0].Length == ContentID.Length)
                                            {
                                                sgi_validated = true;
                                            }
                                            else
                                            {
                                                ImportStatusText.Foreground = Brushes.Red;
                                                ImportStatusText.Content = "Content Identifier is not valid!";
                                                ImportStart.IsEnabled = true;
                                            }
                                        }
                                        else
                                        {
                                            ImportStatusText.Foreground = Brushes.Red;
                                            ImportStatusText.Content = "Content Identifier is not valid!";
                                            ImportStart.IsEnabled = true;
                                        }
                                    }
                                    else if(IgnoreCID.IsChecked.Value)
                                    {
                                        sgi_validated = true;
                                    }
                                    else
                                    {
                                        ImportStatusText.Foreground = Brushes.Red;
                                        ImportStatusText.Content = "Content Identifier is not valid!";
                                        ImportStart.IsEnabled = true;
                                    }
                                }
                                else
                                {
                                    ImportStatusText.Foreground = Brushes.Red;
                                    ImportStatusText.Content = "Output Folder provided does not exist!";
                                    ImportStart.IsEnabled = true;
                                }
                            }
                            else
                            {
                                ImportStatusText.Foreground = Brushes.Red;
                                ImportStatusText.Content = "No Output Folder provided!";
                                ImportStart.IsEnabled = true;
                            }
                        }
                        else
                        {
                            ImportStatusText.Foreground = Brushes.Red;
                            ImportStatusText.Content = "Steam Library provided does not exist!";
                            ImportStart.IsEnabled = true;
                        }
                    }
                    else
                    {
                        ImportStatusText.Foreground = Brushes.Red;
                        ImportStatusText.Content = "No Steam Library provided!";
                        ImportStart.IsEnabled = true;
                    }
                }
                else
                {
                    ImportStatusText.Foreground = Brushes.Red;
                    ImportStatusText.Content = "VMF provided does not exist!";
                    ImportStart.IsEnabled = true;
                }
            }
            else
            {
                ImportStatusText.Foreground = Brushes.Red;
                ImportStatusText.Content = "No VMF provided!";
                ImportStart.IsEnabled = true;
            }

            List<string> MSLODF = new List<string>();
            if (RLODFilterBox.Text != "")
            {
                string[] lines = RLODFilterBox.Text.Split(
                    new[] { "\r\n", "\r", "\n" },
                    StringSplitOptions.None
                );
                foreach (string line in lines)
                {
                    if (line != "")
                    {
                        MSLODF.Add(line);
                    }
                }
            }

            if (sgi_validated)
            {
                bool MSLOD = ModelStripLOD.IsChecked.Value;
                bool MapSkipStepThree = MapSkipCreate.IsChecked.Value;
                Thread myNewThread = new Thread(() => importerOBJ.SourceImportThreaded(VMFPath, SteamLibPath, OutputPath, IncludePath, ContentID, MSLOD, MSLODF, MapSkipStepThree, this));
                myNewThread.Start();
            }
        }



        /*
        * 
        * WPF FORM ACTIONS (Last function ALWAYS "Start" button) - SHRINKER
        * 
        */

        private void Button_Click_ShrinkChooseVMF(object sender, RoutedEventArgs e)
        {
            //VMF Select Button
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".vmf";
            dlg.Filter = "Valve Map Files (*.vmf)|*.vmf|All Files (*)|*";
            dlg.AddExtension = true;
            dlg.CheckFileExists = true;
            dlg.DereferenceLinks = true;
            dlg.Multiselect = false;
            Nullable<bool> result = dlg.ShowDialog();
            if (result.HasValue && result.Value)
            {
                string filename = dlg.FileName;
                ShrinkMapPathBox.Text = filename;
            }
        }

        private void Button_Click_ShrinkChoosePack(object sender, RoutedEventArgs e)
        {
            //Steam Library Select Button
            CommonOpenFileDialog cofd = new CommonOpenFileDialog();
            cofd.IsFolderPicker = true;
            if (cofd.ShowDialog() == CommonFileDialogResult.Ok)
            {
                ShrinkCPDirBox.Text = cofd.FileName;
            }
        }

        private void Button_Click_ShrinkChooseOutputDir(object sender, RoutedEventArgs e)
        {
            //Content Pack Select Button
            CommonOpenFileDialog cofd = new CommonOpenFileDialog();
            cofd.IsFolderPicker = true;
            if (cofd.ShowDialog() == CommonFileDialogResult.Ok)
            {
                ShrinkOutputPathBox.Text = cofd.FileName;
            }
        }

        private void Button_Click_ShrinkChooseIncludeDir(object sender, RoutedEventArgs e)
        {
            //Content Pack Select Button
            CommonOpenFileDialog cofd = new CommonOpenFileDialog();
            cofd.IsFolderPicker = true;
            if (cofd.ShowDialog() == CommonFileDialogResult.Ok)
            {
                ShrinkIncludePathBox.Text = cofd.FileName;
            }
        }

        private void ShrinkStart_Click(object sender, RoutedEventArgs e)
        {
            //Start Import Button
            //Get all settings and store them in variables, we dont need things changing on us partway through this long process
            string VMFPath = "";
            string CPPath = "";
            string OutputPath = "";
            string IncludePath = "";

            VMFPath = ShrinkMapPathBox.Text;
            CPPath = ShrinkCPDirBox.Text;
            OutputPath = ShrinkOutputPathBox.Text;
            IncludePath = ShrinkIncludePathBox.Text;

            //Check all given values from the user to confirm they will work (or should work)
            ImportStatusText.Foreground = Brushes.Black;
            ImportStatusText.Content = "Checking Parameters";

            ShrinkStart.IsEnabled = false;

            bool sgi_validated = false;
            if (VMFPath != "")
            {
                if (File.Exists(VMFPath))
                {

                    if (Directory.Exists(CPPath))
                    {
                        if (OutputPath != "")
                        {
                            if (Directory.Exists(OutputPath))
                            {
                                if (IncludePath != "")
                                {
                                    if (Directory.Exists(IncludePath) == false)
                                    {
                                        IncludePath = "";
                                        ImportStatusText.Content = "Warning: Include Directory is not valid! Skipping include step.";
                                    }
                                }
                                sgi_validated = true;
                            }
                            else
                            {
                                ImportStatusText.Foreground = Brushes.Red;
                                ImportStatusText.Content = "Output Folder provided does not exist!";
                                ImportStart.IsEnabled = true;
                            }
                        }
                        else
                        {
                            ImportStatusText.Foreground = Brushes.Red;
                            ImportStatusText.Content = "No Output Folder provided!";
                            ImportStart.IsEnabled = true;
                        }
                    }
                    else
                    {
                        ImportStatusText.Foreground = Brushes.Red;
                        ImportStatusText.Content = "Content Pack Directory provided does not exist!";
                        ImportStart.IsEnabled = true;
                    }
                }
                else
                {
                    ImportStatusText.Foreground = Brushes.Red;
                    ImportStatusText.Content = "VMF provided does not exist!";
                    ImportStart.IsEnabled = true;
                }
            }
            else
            {
                ImportStatusText.Foreground = Brushes.Red;
                ImportStatusText.Content = "No VMF provided!";
                ImportStart.IsEnabled = true;
            }

            if (sgi_validated)
            {
                Thread myNewThread = new Thread(() => shrinkerOBJ.ShrinkerThreaded(VMFPath, CPPath, OutputPath, IncludePath, this));
                myNewThread.Start();
            }
        }
    }
}
