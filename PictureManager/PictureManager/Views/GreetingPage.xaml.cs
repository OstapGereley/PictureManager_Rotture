using System.Windows;
using MahApps.Metro.Controls;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using PictureManager.Models;
using PictureManager.ViewModels;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace PictureManager.Views
{
    /// <summary>
    /// Логика взаимодействия для GreatingPage.xaml
    /// </summary>
    public partial class GreetingPage : MetroWindow
    {
        public Stack<PathInfo> PathNames = new Stack<PathInfo>();
        string path_from_dialog = "";

        public MainMetroWindowViewModel ViewModel { get; set; }
        

        public GreetingPage()
        {

            InitializeComponent();
            if (File.Exists("PathFile.s"))
            {
                Deserialize();
            }
            else
            { 
                PathNames.Push(new PathInfo(""));
                
            }
            PathBox.ItemsSource = PathNames;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new CommonOpenFileDialog();
                dlg.Title = "My Title";
                dlg.IsFolderPicker = true;
                dlg.AddToMostRecentlyUsedList = false;
                dlg.AllowNonFileSystemItems = false;
                dlg.EnsureFileExists = true;
                dlg.EnsurePathExists = true;
                dlg.EnsureReadOnly = false;
                dlg.EnsureValidNames = true;
                dlg.Multiselect = false;
                dlg.ShowPlacesList = true;
            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
                path_from_dialog = dlg.FileName;
            var is_repeated = false;
            foreach (var item in PathNames)
            {
                if (item.Name == path_from_dialog)
                {
                    is_repeated = true;
                    break;
                }
                    
            }

            if (path_from_dialog != "" && !is_repeated)
            {
                PathNames.Push(new PathInfo(path_from_dialog));
            }
            is_repeated = false;
            Serialize();
            Deserialize();
            PathBox.ItemsSource = PathNames;
            Text.Text = path_from_dialog;
            try
            {
                var mainMetroWindow = new MainMetroWindow();
                var viewModel = new MainMetroWindowViewModel(path_from_dialog);
                mainMetroWindow.DataContext = viewModel;
                ViewModel = viewModel;
                mainMetroWindow.Show();
                Close();
            }
            catch
            {
            }
        }
        public void Serialize()
        {
           
                FileStream fs = new FileStream("PathFile.s", FileMode.Create, FileAccess.Write, FileShare.ReadWrite);

                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, PathNames);
                fs.Close();
            
        }

        public void Deserialize()
        {

            FileStream fs = new FileStream("PathFile.s", FileMode.Open, FileAccess.Read, FileShare.Read);

                BinaryFormatter bf = new BinaryFormatter();
                var words = (Stack<PathInfo>)bf.Deserialize(fs);
                PathNames = words;
                fs.Close();
        }

        private void PathBox_Selected(object sender, RoutedEventArgs e)
        {
            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var path = PathBox.SelectedItem as PathInfo;
            if (path.Name != "")
            {
                var p = path.Name;
                var mainMetroWindow = new MainMetroWindow();
                var viewModel = new MainMetroWindowViewModel(p);
                ViewModel = viewModel;
                mainMetroWindow.DataContext = viewModel;
                mainMetroWindow.Show();
                Close();
            }
        }


    }
}
