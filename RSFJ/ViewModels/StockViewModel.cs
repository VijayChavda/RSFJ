using RSFJ.Model;
using RSFJ.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSFJ.ViewModels
{
    public class StockViewModel : ViewModelBase
    {
        public ObservableCollection<StockItem> StockItems { get; set; }

        public StockViewModel()
        {
            StockItems = new ObservableCollection<StockItem>();

            FillData();

            DataContextService.Instance.DataContext.StockItemAdded += (s, item) =>
            {
                if (StockItems.Count(x => x.Name == item.Name) == 0)
                    StockItems.Add(item);
            };
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
