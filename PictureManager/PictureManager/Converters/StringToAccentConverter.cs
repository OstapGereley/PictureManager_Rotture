using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;
using MahApps.Metro;

namespace PictureManager.Converters
{
    class StringToAccentConverter : IValueConverter
    {
        private readonly Dictionary<string, SolidColorBrush> _cache = new Dictionary<string, SolidColorBrush>();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var accentName = value.ToString();

            if (!_cache.ContainsKey(accentName))
            {
                var accent = ThemeManager.Accents.FirstOrDefault(elem => elem.Name == accentName);

                if (accent != null)
                {
                    var accentColor = (Color) accent.Resources["AccentColor"];

                    _cache.Add(accentName, new SolidColorBrush(accentColor));

                }
            }

            return _cache[accentName];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
