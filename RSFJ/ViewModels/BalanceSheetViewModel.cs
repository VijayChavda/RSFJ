using RSFJ.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSFJ.ViewModels
{
    public class BalanceSheetViewModel
    {
        public ObservableCollection<StockItem> StockItems { get; set; }

        public BalanceSheetViewModel()
        {
            StockItems = new ObservableCollection<StockItem>();

            StockItems.CollectionChanged += (sender, args) =>
            {
                if (args.NewItems != null)
                {
                    foreach (var item in args.NewItems.Cast<StockItem>())
                    {
                        DataContextService.Instance.DataContext.StockItems.Add(item.Model);
                    }
                }
            };
        }

        public void Load()
        {
            StockItems.Clear();

            foreach (var model in DataContextService.Instance.DataContext.StockItems)
            {
                StockItems.Add(new StockItem(model));
            }
        }
    }

    public class StockItem : INotifyPropertyChanged
    {
        public Model.StockItem Model { get; set; }

        private string _Name;
        public string Name { get => _Name; set => SetProperty(ref _Name, value); }

        private double _Rate;
        public double Rate { get => _Rate; set => SetProperty(ref _Rate, value); }

        private double _InStock;
        public double InStock { get => _InStock; set => SetProperty(ref _InStock, value); }

        private double _Money;
        public double Money { get => _Money; set => SetProperty(ref _Money, value); }

        public StockItem()
        {
            Model = new Model.StockItem();
        }

        public StockItem(Model.StockItem Model)
        {
            this.Model = Model;

            _Name = Model.Name;
            _Rate = Model.Rate;
            _InStock = Model.InStock;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void SetProperty<T>(ref T Property, T Value, [System.Runtime.CompilerServices.CallerMemberName] string PropertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(Property, Value) == false)
            {
                Property = Value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));

                Money = InStock * Rate;

                #region Model update
                Model.Name = Name;
                Model.InStock = InStock;
                Model.Rate = Rate;
                #endregion
            }
        }
    }
}
