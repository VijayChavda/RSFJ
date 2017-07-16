using RSFJ.Model;
using RSFJ.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSFJ.ViewModels
{
    public class StockViewModel : ViewModelBase
    {
        public List<StockItem> StockItems { get; set; }

        public StockViewModel()
        {
            StockItems = new List<StockItem>();

            FillData();
        }

        private void FillData()
        {
            foreach (var item in DataContextService.Instance.DataContext.StockItems)
            {
                StockItems.Add(item);
            }
        }
    }
}
