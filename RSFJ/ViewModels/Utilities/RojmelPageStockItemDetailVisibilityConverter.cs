using RSFJ.Services;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RSFJ.ViewModels.Utilities
{
    public class RojmelPageStockItemDetailVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is RojmelEntryViewModel entry)
            {
                if (entry.StockItem == null || entry.StockItem.Name == DataContextService.Instance.DataContext.None.Name)
                {
                    return Visibility.Collapsed;
                }

                return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
