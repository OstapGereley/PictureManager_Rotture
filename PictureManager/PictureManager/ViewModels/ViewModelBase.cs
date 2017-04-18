using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using PictureManager.ViewModels.Commands;

namespace PictureManager.ViewModels
{
    [DataContract]
    public abstract class ViewModelBase : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void Dispose()
        {
            OnDispose();
        }

        protected virtual void OnDispose()
        {
            
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected void RaiseCanExecuteChanges(DelegateCommand command)
        {
            if (command != null)
                command.RaiseCanExecuteChanged();
        }
    }
}
