using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace QuizzQuest.Utilitaires
{
    internal class Converters : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is int itemIndex))
            {
                return Color.FromArgb("#000000"); // Noir si la conversion échoue
            }

            return itemIndex % 2 == 0 ? Color.FromArgb("#D3D3D3") : Color.FromArgb("#A9A9A9");
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
