using System.Collections.Generic;

namespace RSFJ.Model
{
    public class DataContext
    {
        public HashSet<RojmelEntry> RojmelEntries { get; set; }
        public HashSet<StockItem> StockItems { get; set; }

        public DataContext()
        {
            RojmelEntries = new HashSet<RojmelEntry>();
            StockItems = new HashSet<StockItem>();
        }
    }
}
