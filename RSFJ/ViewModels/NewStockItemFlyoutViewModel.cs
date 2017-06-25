using RSFJ.ViewModels.Utilities;
using System.Windows.Input;
using System.Timers;
using System.Collections.Generic;
using System.Linq;
using RSFJ.Model;

namespace RSFJ.ViewModels
{
    public class NewStockItemFlyoutViewModel : ViewModelBase
    {
        private string _Name;
        public string Name { get => _Name; set => SetProperty(ref _Name, value); }

        private double? _InStock;
        public double? InStock { get => _InStock; set => SetProperty(ref _InStock, value); }

        private double? _Rate_Purity;
        public double? Rate_Purity { get => _Rate_Purity; set => SetProperty(ref _Rate_Purity, value); }

        private string _Message;
        public string Message { get => _Message; set => SetProperty(ref _Message, value); }

        RelayCommand _addCommand;
        public ICommand AddCommand
        {
            get => _addCommand ?? (_addCommand = new RelayCommand(param => Add(), param => true));
        }

        public NewStockItemFlyoutViewModel()
        {
            var types = DataContext.RojmelEntryTypes.ToList();
        }

        private void Add()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                Message = "Please provide a name to this item.";
                return;
            }

            if (Rate_Purity == null)
            {
                Message = "Please provide a valid value for purity.";
                return;
            }

            if (InStock == null)
            {
                Message = "Please provide a valid initial amount of stock.";
                return;
            }

            var newItem = new StockItem()
            {
                Name = Name,
                InStock = (double)InStock,
                Rate_Purity = (double)Rate_Purity,
                EquivalentGold = (double)(InStock * Rate_Purity),
            };

            var added = Services.DataContextService.Instance.DataContext.StockItems.Add(newItem);
            if (added)
            {
                Services.DataContextService.Instance.DataContext.FireStockItemAdded(newItem);
                Message = "Item was added successfully...";

                var timer = new Timer(800) { AutoReset = false };
                timer.Elapsed += (sender, e) =>
                {
                    Name = null;
                    InStock = null;
                    Rate_Purity = null;
                    Message = null;
                };
                timer.Start();
            }
            else
            {
                Message = "This item already exists. Please provide unique names for stock items.";
            }
        }

        protected override void APropertyChanged<T>(string PropertyName, T OldValue, T NewValue)
        {
            if (PropertyName != nameof(Message) && string.IsNullOrWhiteSpace(Name) == false && Rate_Purity != null && InStock != null)
            {
                Message = null;
            }
        }
    }
}
