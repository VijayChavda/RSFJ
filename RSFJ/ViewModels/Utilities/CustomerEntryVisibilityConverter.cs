using RSFJ.Model;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RSFJ.ViewModels.Utilities
{
    public class CustomerEntryVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value.GetType() != typeof(AccountType))
            {
                return Visibility.Collapsed;
            }

            return (AccountType)value == AccountType.Customer ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
