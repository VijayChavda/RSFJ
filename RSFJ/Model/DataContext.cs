using System.Collections.Generic;

namespace RSFJ.Model
{
    public class DataContext
    {
        public HashSet<RojmelEntry> RojmelEntries { get; set; }
        public HashSet<StockItem> StockItems { get; set; }
        public HashSet<Account> Accounts { get; set; }
        public HashSet<string> RojmelEntryTypes { get; set; }

        public DataContext()
        {
            RojmelEntries = new HashSet<RojmelEntry>();
            StockItems = new HashSet<StockItem>();
            Accounts = new HashSet<Account>();
            RojmelEntryTypes = new HashSet<string>()
            {
                RojmelEntryType.Initital,
                RojmelEntryType.Exchange,
                RojmelEntryType.Uplak,
                RojmelEntryType.UplakClear,
                RojmelEntryType.Bullion,
                RojmelEntryType.Customer,
            };
        }
    }
}
