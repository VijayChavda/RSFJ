using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSFJ.Model
{
    public class DataContext
    {
        public List<RojmelEntry> RojmelEntries { get; set; }

        public List<StockItem> StockItems { get; set; }

        public DataContext()
        {
            RojmelEntries = new List<RojmelEntry>();
            StockItems = new List<StockItem>();
        }
    }
}
