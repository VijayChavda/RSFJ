using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSFJ.Services
{
    public class RegistoryService
    {
        public static RegistoryService Instance => _Instance ?? (_Instance = new RegistoryService());
        private static RegistoryService _Instance;

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
            }
        }
    }
}
