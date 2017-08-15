using RSFJ.Model;
using System;
using System.Globalization;
using System.Windows.Data;

namespace RSFJ.ViewModels.Utilities
{
    public class RojmelEntryTypeToHeadingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            const string h_rupees = "Rs";
            const string h___fine = "Fine";
            const string h__grams = "Grams";
            const string h___rate = "Rate";
            const string h_purity = "%";
            const string h___none = "-";
            const string h___stock = "Stock";

            if (value.GetType() != typeof(RojmelEntryType))
                return null;

            if (parameter == null || string.IsNullOrEmpty(parameter.ToString()))
                return null;

            var type = (RojmelEntryType)value;
            var param = int.Parse(parameter.ToString());
            switch (type)
            {
                case RojmelEntryType.ItemExchangeFine:
                    return param == 0 ? h__grams : param == 1 ? h_purity : h___fine;

                case RojmelEntryType.ItemExchangeCash:
                    return param == 0 ? h__grams : param == 1 ? h___rate : h_rupees;

                case RojmelEntryType.SimpleCashExchange:
                    return param == 0 ? h_rupees : param == 1 ? h___none : h_rupees;

                case RojmelEntryType.UseCash:
                    return param == 0 ? h_rupees : param == 1 ? h___rate : h___fine;

                case RojmelEntryType.Invalid:
                    return param == 0 ? h___none : param == 1 ? h___none : h___none;
                case RojmelEntryType.Initial:
                    return param == 0 ? h___stock : param == 1 ? h___none : h___stock;
                default:
                    return param == 0 ? h___none : param == 1 ? h___none : h___none;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
