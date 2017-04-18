using System;
using System.Windows;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace PictureManager.Views
{
    /// <summary>
    /// Логика взаимодействия для MainMetroWindow.xaml
    /// </summary>
    public partial class MainMetroWindow : MetroWindow
    {
        public MainMetroWindow()
        {
            InitializeComponent();
        }

        private void ImageEx_OnSourceChanged(object sender, RoutedEventArgs e)
        {
            Border.Reset();
        }
    }
}
