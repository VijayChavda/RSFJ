using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RSFJ.ViewModels.Utilities
{
    public class RojmelEntryTypeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value == null)
            {
                return Visibility.Collapsed;
            }

            var type = value.ToString();
            var typeToComparewith = parameter?.ToString();

            return type == typeToComparewith ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
