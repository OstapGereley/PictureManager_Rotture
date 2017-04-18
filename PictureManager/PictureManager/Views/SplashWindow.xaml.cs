using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PictureManager.ViewModels;

namespace PictureManager.Views
{
    /// <summary>
    /// Interaction logic for SplashWindow.xaml
    /// </summary>
    public partial class SplashWindow : Window
    {
        private GreetingPage _greetingPage;

        public MainMetroWindowViewModel GetMainMetroWindowViewModel()
        {
            return _greetingPage.ViewModel;
        }

        public SplashWindow()
        {
            InitializeComponent();
        }
        private void Timeline_OnCompleted(object sender, EventArgs e)
        {
            _greetingPage = new GreetingPage();
            //var viewModel = new GreetingPageViewModel();
            //_greetingPage.DataContext = viewModel;
            _greetingPage.Show();
            Close();
        }
    }
}
