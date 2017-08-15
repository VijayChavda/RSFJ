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
    public class RojmelPageAccountDetailVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is RojmelEntryViewModel entry)
            {
                if (entry.Account == null || entry.Account.Type == Model.AccountType.Self)
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
