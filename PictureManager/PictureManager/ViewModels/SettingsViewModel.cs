using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro;
using PictureManager.ViewModels.Commands;

namespace PictureManager.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private bool _isOpen;
        public bool IsOpen
        {
            get { return _isOpen; }
            set
            {
                _isOpen = value;
                OnPropertyChanged("IsOpen");

                RaiseCanExecuteChanges(OpenSettingsCommand as DelegateCommand);
            }
        }

        public IEnumerable<string> AvailableThemes { get; private set; }
        public IEnumerable<string> AvailableAccents { get; private set; }

        public string SelectedTheme { get; set; }
        public string SelectedAccent { get; set; }

        public SettingsViewModel()
        {
            AvailableThemes = ThemeManager.AppThemes.Select(elem => elem.Name);
            AvailableAccents = ThemeManager.Accents.Select(elem => elem.Name);

            SelectedTheme = ThemeManager.DetectAppStyle(Application.Current).Item1.Name;
            SelectedAccent = ThemeManager.DetectAppStyle(Application.Current).Item2.Name;

            OpenSettingsCommand = new DelegateCommand(OpenSettings, CanOpenSettings);
            ApplySettingsCommand = new DelegateCommand(ApplySettings);
        }

        #region Commands

        public ICommand OpenSettingsCommand { get; set; }

        public void OpenSettings(object parameter)
        {
            IsOpen = true;
        }

        public bool CanOpenSettings(object parameter)
        {
            return !IsOpen;
        }

        public ICommand ApplySettingsCommand { get; set; }

        public void ApplySettings(object parameter)
        {
            ThemeManager.ChangeAppStyle(Application.Current,
                ThemeManager.GetAccent(SelectedAccent),
                ThemeManager.GetAppTheme(SelectedTheme));

            IsOpen = false;
        }

        #endregion
    }
}
