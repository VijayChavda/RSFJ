using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace RSFJ.ViewModels.Utilities
{
    public class LendingEntryVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var entry = value as RojmelEntryViewModel;

            if (entry == null)
                return Visibility.Collapsed;

            if (entry.Type == Model.RojmelEntryType.ItemExchangeFine && !entry.IsLeftSide && entry.Account.Type == Model.AccountType.Regular)
            {
                return Visibility.Visible;
            }

            if (entry.Type == Model.RojmelEntryType.ItemExchangeCash && entry.IsLeftSide && entry.Account.Type == Model.AccountType.Boolean)
            {
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
