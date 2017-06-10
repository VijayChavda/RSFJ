using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
