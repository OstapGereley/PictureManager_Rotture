using System.Configuration;
using System.IO;
using System.Windows;
using PictureManager.ViewModels;
using PictureManager.Views;

namespace PictureManager
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private SplashWindow _splashWindow;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var cacheDirectoryPath = ConfigurationManager.AppSettings["CacheDirectoryPath"];

            if (!Directory.Exists(cacheDirectoryPath))
            {
                Directory.CreateDirectory(cacheDirectoryPath);
            }

            _splashWindow = new SplashWindow();
            _splashWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            var viewModel = _splashWindow.GetMainMetroWindowViewModel();

            if (viewModel != null)
                viewModel.CategoriesPanel.SaveCategories();
        }
    }
}
