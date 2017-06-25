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

        public bool ShowAggregateColumns
        {
            get
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
                key = key.OpenSubKey("Augment Software", true);
                key = key.OpenSubKey("RSFJ", true);
                return bool.Parse(key.GetValue(nameof(ShowAggregateColumns), false).ToString());
            }
            set
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
                key = key.OpenSubKey("Augment Software", true);
                key = key.OpenSubKey("RSFJ", true);
                key.SetValue(nameof(ShowAggregateColumns), value);

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShowAggregateColumns)));
            }
        }
    }
}
