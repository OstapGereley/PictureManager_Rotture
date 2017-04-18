using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;

namespace PictureManager.ViewModels
{
    public static class ExtensionMethods
    {
        public static void AddOnUI<T>(this ICollection<T> collection, T item)
        {
            Action<T> addMethod = collection.Add;
            Application.Current.Dispatcher.BeginInvoke(addMethod, item);
        }
    }
}
