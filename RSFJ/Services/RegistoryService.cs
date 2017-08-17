using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSFJ.Services
{
    public class RegistoryService : INotifyPropertyChanged
    {
        public static RegistoryService Instance => _Instance ?? (_Instance = new RegistoryService());
        private static RegistoryService _Instance;

        public event PropertyChangedEventHandler PropertyChanged;

        private RegistoryService()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);

            key.CreateSubKey("Augment Software");
            key = key.OpenSubKey("Augment Software", true);

            key.CreateSubKey("RSFJ");
            key = key.OpenSubKey("RSFJ", true);

            if (key.GetValue(nameof(FailureCount)) == null)
            {
                key.SetValue(nameof(FailureCount), "0");
            }
        }

        public string MasterPassword
        {
            get
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
                key = key.OpenSubKey("Augment Software", true);
                key = key.OpenSubKey("RSFJ", true);
                return key.GetValue(nameof(MasterPassword))?.ToString();
            }
            set
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
                key = key.OpenSubKey("Augment Software", true);
                key = key.OpenSubKey("RSFJ", true);
                key.SetValue(nameof(MasterPassword), value);

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MasterPassword)));
            }
        }

        public int FailureCount
        {
            get
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
                key = key.OpenSubKey("Augment Software", true);
                key = key.OpenSubKey("RSFJ", true);
                return int.Parse(key.GetValue(nameof(FailureCount), 0).ToString());
            }
            set
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
                key = key.OpenSubKey("Augment Software", true);
                key = key.OpenSubKey("RSFJ", true);
                key.SetValue(nameof(FailureCount), value);

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FailureCount)));
            }
        }

        public int RojmelPageDatesFilterSpan
        {
            get
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
                key = key.OpenSubKey("Augment Software", true);
                key = key.OpenSubKey("RSFJ", true);
                return int.Parse(key.GetValue(nameof(RojmelPageDatesFilterSpan), 30).ToString());
            }
            set
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
                key = key.OpenSubKey("Augment Software", true);
                key = key.OpenSubKey("RSFJ", true);
                key.SetValue(nameof(RojmelPageDatesFilterSpan), value);

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RojmelPageDatesFilterSpan)));
            }
        }

        public bool ShowAggregateFineBalance
        {
            get
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
                key = key.OpenSubKey("Augment Software", true);
                key = key.OpenSubKey("RSFJ", true);
                return bool.Parse(key.GetValue(nameof(ShowAggregateFineBalance), false).ToString());
            }
            set
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
                key = key.OpenSubKey("Augment Software", true);
                key = key.OpenSubKey("RSFJ", true);
                key.SetValue(nameof(ShowAggregateFineBalance), value);

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShowAggregateFineBalance)));
            }
        }

        public bool ShowAggregateMoneyBalance
        {
            get
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
                key = key.OpenSubKey("Augment Software", true);
                key = key.OpenSubKey("RSFJ", true);
                return bool.Parse(key.GetValue(nameof(ShowAggregateMoneyBalance), false).ToString());
            }
            set
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
                key = key.OpenSubKey("Augment Software", true);
                key = key.OpenSubKey("RSFJ", true);
                key.SetValue(nameof(ShowAggregateMoneyBalance), value);

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShowAggregateMoneyBalance)));
            }
        }

        public bool ShowAggregateStockBalance
        {
            get
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
                key = key.OpenSubKey("Augment Software", true);
                key = key.OpenSubKey("RSFJ", true);
                return bool.Parse(key.GetValue(nameof(ShowAggregateStockBalance), false).ToString());
            }
            set
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
                key = key.OpenSubKey("Augment Software", true);
                key = key.OpenSubKey("RSFJ", true);
                key.SetValue(nameof(ShowAggregateStockBalance), value);

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShowAggregateStockBalance)));
            }
        }
    }
}
