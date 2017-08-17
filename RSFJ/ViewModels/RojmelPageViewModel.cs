using RSFJ.Services;
using RSFJ.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Data;
using RSFJ.ViewModels.Utilities;
using System.Windows.Input;

namespace RSFJ.ViewModels
{
    public class RojmelPageViewModel : ViewModelBase
    {
        public ObservableCollection<RojmelEntryViewModel> Entries { get; set; }

        private RojmelEntryViewModel _SelectedEntry;
        public RojmelEntryViewModel SelectedEntry { get => _SelectedEntry; set => SetProperty(ref _SelectedEntry, value); }

        #region Aggregate columns
        private bool _ShowAggregateFineBalance;
        public bool ShowAggregateFineBalance { get => _ShowAggregateFineBalance; set => SetProperty(ref _ShowAggregateFineBalance, value); }

        private bool _ShowAggregateMoneyBalance;
        public bool ShowAggregateMoneyBalance { get => _ShowAggregateMoneyBalance; set => SetProperty(ref _ShowAggregateMoneyBalance, value); }

        private bool _ShowAggregateStockBalance;
        public bool ShowAggregateStockBalance { get => _ShowAggregateStockBalance; set => SetProperty(ref _ShowAggregateStockBalance, value); }
        #endregion

        #region Filters
        public CollectionViewSource EntriesViewSource { get; set; }

        RelayCommand _resetFiltersCommand;
        public ICommand ResetFiltersCommand
        {
            get => _resetFiltersCommand ?? (_resetFiltersCommand = new RelayCommand(param => ResetFilters(), param => true));
        }

        public List<string> UnitFilters { get; set; }

        private string _FilterUnit;
        public string FilterUnit { get => _FilterUnit; set => SetProperty(ref _FilterUnit, value); }

        private Account _FilterAccount;
        public Account FilterAccount { get => _FilterAccount; set => SetProperty(ref _FilterAccount, value); }

        private StockItem _FilterStockItem;
        public StockItem FilterStockItem { get => _FilterStockItem; set => SetProperty(ref _FilterStockItem, value); }

        private DateTime _FilterStartDate;
        public DateTime FilterStartDate { get => _FilterStartDate; set => SetProperty(ref _FilterStartDate, value); }

        private DateTime _FilterEndDate;
        public DateTime FilterEndDate { get => _FilterEndDate; set => SetProperty(ref _FilterEndDate, value); }

        private void EntriesViewSource_Filter(object sender, FilterEventArgs e)
        {
            if (e.Item is RojmelEntryViewModel entry)
            {
                if (FilterUnit == UnitFilters[0] && entry.Account == null && entry.StockItem == null)
                {
                    e.Accepted = true;
                    return;
                }

                if (FilterUnit == UnitFilters[0])
                {
                    e.Accepted = true;
                }
                else if (FilterUnit == UnitFilters[1])
                {
                    e.Accepted = entry.Type == RojmelEntryType.ItemExchangeFine || entry.Type == RojmelEntryType.UseCash;
                }
                else
                {
                    e.Accepted = entry.Type == RojmelEntryType.ItemExchangeCash || entry.Type == RojmelEntryType.SimpleCashExchange;
                }

                e.Accepted &= FilterAccount == null || entry.Account == FilterAccount;
                e.Accepted &= FilterStockItem == null || entry.StockItem == FilterStockItem;
                e.Accepted &= entry.Date.Date >= FilterStartDate && entry.Date.Date <= FilterEndDate;
            }
        }

        public void ResetFilters()
        {
            FilterUnit = UnitFilters[0];
            FilterAccount = null;
            FilterStockItem = null;
            FilterEndDate = DateTime.Now;
            FilterStartDate = FilterEndDate.Subtract(TimeSpan.FromDays(RegistoryService.Instance.RojmelPageDatesFilterSpan));
        }
        #endregion

        public RojmelPageViewModel()
        {
            Entries = new ObservableCollection<RojmelEntryViewModel>();
            UnitFilters = new List<string>() { "All", "Fine", "Cash" };

            LoadData();

            SelectedEntry = Entries.FirstOrDefault();

            EntriesViewSource = new CollectionViewSource() { Source = Entries };
            EntriesViewSource.Filter += EntriesViewSource_Filter;

            ShowAggregateFineBalance = RegistoryService.Instance.ShowAggregateFineBalance;
            ShowAggregateMoneyBalance = RegistoryService.Instance.ShowAggregateMoneyBalance;
            ShowAggregateStockBalance = RegistoryService.Instance.ShowAggregateStockBalance;

            FilterUnit = UnitFilters[0];
            FilterAccount = null;
            FilterStockItem = null;
            FilterStartDate = DateTime.Now.Subtract(TimeSpan.FromDays(Services.RegistoryService.Instance.RojmelPageDatesFilterSpan));
            FilterEndDate = DateTime.Now;

            RegistoryService.Instance.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(RegistoryService.ShowAggregateFineBalance))
                {
                    ShowAggregateFineBalance = RegistoryService.Instance.ShowAggregateFineBalance;
                }
                if (e.PropertyName == nameof(RegistoryService.ShowAggregateMoneyBalance))
                {
                    ShowAggregateMoneyBalance = RegistoryService.Instance.ShowAggregateMoneyBalance;
                }
                if (e.PropertyName == nameof(RegistoryService.ShowAggregateStockBalance))
                {
                    ShowAggregateStockBalance = RegistoryService.Instance.ShowAggregateStockBalance;
                }
                if (e.PropertyName == nameof(RegistoryService.RojmelPageDatesFilterSpan))
                {
                    FilterStartDate = FilterEndDate.Subtract(TimeSpan.FromDays(RegistoryService.Instance.RojmelPageDatesFilterSpan));
                }
            };

            Entries.CollectionChanged += (s, e) =>
            {
                if (e.OldItems != null && e.OldItems.Count > 0)
                {
                    foreach (var item in e.OldItems.Cast<RojmelEntryViewModel>())
                    {
                        DataContextService.Instance.DataContext.RojmelEntries.RemoveAll(x => x.Id == item.Id);
                    }
                }
            };
        }

        private void LoadData()
        {
            foreach (var model in DataContextService.Instance.DataContext.RojmelEntries.OrderBy(x => x.Id))
            {
                Entries.Add(new RojmelEntryViewModel(model));
            }
        }

        protected override async void APropertyChanged<T>(string PropertyName, T OldValue, T NewValue)
        {
            if (PropertyName == nameof(FilterUnit) ||
                PropertyName == nameof(FilterAccount) || PropertyName == nameof(FilterStockItem) ||
                PropertyName == nameof(FilterStartDate) || PropertyName == nameof(FilterEndDate))
            {
                try
                {
                    EntriesViewSource.View.Refresh();
                }
                catch { }
            }

            if (PropertyName == nameof(ShowAggregateFineBalance) || PropertyName == nameof(ShowAggregateMoneyBalance) ||
                PropertyName == nameof(ShowAggregateStockBalance))
            {
                await CalculateAggregateAsync();
            }
        }

        public async Task CalculateAggregateAsync()
        {
            await Task.Run(() =>
            {
                if (ShowAggregateStockBalance)
                {
                    var groupedEntries = Entries.Where(x => x.StockItem != null).GroupBy(x => x.StockItem);

                    foreach (var sameStockEntries in groupedEntries)
                    {
                        var stockItem = sameStockEntries.Key;
                        double? inStock = 0;

                        foreach (var entry in sameStockEntries)
                        {
                            if (entry.StockItem == DataContextService.Instance.DataContext.None)
                                continue;

                            inStock += entry.IsLeftSide ? entry.Param1 : -entry.Param1;

                            entry.StockItemBalance = inStock;
                        }
                    }
                }

                if (ShowAggregateFineBalance || ShowAggregateMoneyBalance)
                {
                    var groupedEntries = Entries.Where(x => x.Account != null).GroupBy(x => x.Account);

                    foreach (var sameAccountEntries in groupedEntries)
                    {
                        var account = sameAccountEntries.Key;
                        double? fineInMoney = 0;
                        double? fineInGold = 0;

                        foreach (var entry in sameAccountEntries)
                        {
                            switch (entry.Type)
                            {
                                case RojmelEntryType.ItemExchangeFine:
                                    fineInGold += entry.IsLeftSide ? -entry.Result : entry.Result;
                                    break;
                                case RojmelEntryType.ItemExchangeCash:
                                    fineInMoney += entry.IsLeftSide ? -entry.Result : entry.Result;
                                    break;
                                case RojmelEntryType.SimpleCashExchange:
                                    fineInMoney += entry.IsLeftSide ? -entry.Result : entry.Result;
                                    break;
                                case RojmelEntryType.UseCash:
                                    fineInGold += entry.IsLeftSide ? -entry.Result : entry.Result;
                                    //Use account money balance if Cash is not provided.
                                    if (entry.StockItem == DataContextService.Instance.DataContext.None)
                                    {
                                        fineInMoney += entry.IsLeftSide ? entry.Param1 : -entry.Param1;
                                    }
                                    break;
                            }

                            entry.AccountMoneyBalance = fineInMoney;
                            entry.AccountFineBalance = fineInGold;
                        }
                    }
                }
            });
        }
    }

    public class RojmelEntryViewModel : ViewModelBase
    {
        public RojmelEntry Model { get; private set; }

        #region Common parameters
        private int? _Id;
        public int? Id { get => _Id; set => SetProperty(ref _Id, value); }

        private DateTime _Date;
        public DateTime Date { get => _Date; set => SetProperty(ref _Date, value); }

        private Account _Account;
        public Account Account { get => _Account; set => SetProperty(ref _Account, value); }

        private StockItem _StockItem;
        public StockItem StockItem { get => _StockItem; set => SetProperty(ref _StockItem, value); }

        private RojmelEntryType _Type;
        public RojmelEntryType Type { get => _Type; set => SetProperty(ref _Type, value); }

        private DateTime _TotalPaymentDueDate;
        public DateTime TotalPaymentDueDate { get => _TotalPaymentDueDate; set => SetProperty(ref _TotalPaymentDueDate, value); }

        private int _PartialPaymentInterval;
        public int PartialPaymentInterval { get => _PartialPaymentInterval; set => SetProperty(ref _PartialPaymentInterval, value); }
        #endregion

        #region General parameters
        private double? _LParam1;
        public double? LParam1 { get => _LParam1; set => SetProperty(ref _LParam1, value); }

        private double? _LParam2;
        public double? LParam2 { get => _LParam2; set => SetProperty(ref _LParam2, value); }

        private double? _LResult;
        public double? LResult { get => _LResult; set => SetProperty(ref _LResult, value); }

        private double? _RParam1;
        public double? RParam1 { get => _RParam1; set => SetProperty(ref _RParam1, value); }

        private double? _RParam2;
        public double? RParam2 { get => _RParam2; set => SetProperty(ref _RParam2, value); }

        private double? _RResult;
        public double? RResult { get => _RResult; set => SetProperty(ref _RResult, value); }
        #endregion

        #region Special parameters
        private double _Labour;
        public double Labour { get => _Labour; set => SetProperty(ref _Labour, value); }

        private double _Waste;
        public double Waste { get => _Waste; set => SetProperty(ref _Waste, value); }

        private bool _IsLabourAsAmount;
        public bool IsLabourAsAmount { get => _IsLabourAsAmount; set => SetProperty(ref _IsLabourAsAmount, value); }
        #endregion

        #region Aggregate parameters
        private double? _StockItemBalance;
        public double? StockItemBalance { get => _StockItemBalance; set => SetProperty(ref _StockItemBalance, value); }

        private double? _AccountMoneyBalance;
        public double? AccountMoneyBalance { get => _AccountMoneyBalance; set => SetProperty(ref _AccountMoneyBalance, value); }

        private double? _AccountFineBalance;
        public double? AccountFineBalance { get => _AccountFineBalance; set => SetProperty(ref _AccountFineBalance, value); }
        #endregion

        #region Getters & Setters for one-side entries
        public bool IsLeftSide
        {
            get
            {
                return (LParam1 != null && RParam1 == null);
            }
        }

        public double? Param1
        {
            get => IsLeftSide ? LParam1 : RParam1;
            set
            {
                if (IsLeftSide) LParam1 = value;
                else RParam1 = value;
            }
        }

        public double? Param2
        {
            get => IsLeftSide ? LParam2 : RParam2;
            set
            {
                if (IsLeftSide) LParam2 = value;
                else RParam2 = value;
            }
        }

        public double? Result
        {
            get => IsLeftSide ? LResult : RResult;
            set
            {
                if (IsLeftSide) LResult = value;
                else RResult = value;
            }
        }
        #endregion

        public static ObservableCollection<StockItem> StockItems { get; set; }
        public static ObservableCollection<Account> Accounts { get; set; }

        static RojmelEntryViewModel()
        {
            #region Accounts
            Accounts = new ObservableCollection<Account>();
            foreach (var account in DataContextService.Instance.DataContext.Accounts)
            {
                Accounts.Add(account);
            }
            DataContextService.Instance.DataContext.AccountAdded += (s, account) =>
            {
                if (Accounts.Count(x => x.Name == account.Name) == 0)
                    Accounts.Add(account);
            };
            #endregion

            #region StockItems
            StockItems = new ObservableCollection<StockItem>();
            foreach (var stockItem in DataContextService.Instance.DataContext.StockItems)
            {
                StockItems.Add(stockItem);
            }
            DataContextService.Instance.DataContext.StockItemAdded += (s, item) =>
            {
                if (StockItems.Count(x => x.Name == item.Name) == 0)
                    StockItems.Add(item);
            };
            #endregion
        }

        public RojmelEntryViewModel()
        {
            Model = new RojmelEntry();

            Id = null;
            Date = DateTime.Now.Date;
            Account = DataContextService.Instance.DataContext.Accounts.FirstOrDefault();
            TotalPaymentDueDate = DateTime.Now.Add(TimeSpan.FromDays(60));    //TODO: Take the last value
            PartialPaymentInterval = 10;    //TODO: Take the last value
        }

        public RojmelEntryViewModel(RojmelEntry Model) : this()
        {
            this.Model = Model;

            _Id = Model.Id;
            _Date = Model.Date;
            _Account = Accounts.Single(x => x.Name == Model.AccountName);
            _Type = Model.Type;
            _StockItem = StockItems.Single(x => x.Name == Model.StockItemName);

            _LParam1 = Model.IsLeftSide ? Model.Param1 : (double?)null;
            _LParam2 = Model.IsLeftSide ? Model.Param2 : (double?)null;
            _LResult = Model.IsLeftSide ? Model.Result : (double?)null;
            _RParam1 = Model.IsLeftSide ? (double?)null : Model.Param1;
            _RParam2 = Model.IsLeftSide ? (double?)null : Model.Param2;
            _RResult = Model.IsLeftSide ? (double?)null : Model.Result;
            _Labour = Model.Labour ?? 0;
            _Waste = Model.Waste ?? 0;
            _PartialPaymentInterval = Model.PartialPaymentInterval;
            _TotalPaymentDueDate = Model.Date.AddDays(Model.FullPaymentDueDays);
            _IsLabourAsAmount = Model.IsLabourAsAmount;
        }

        protected override void APropertyChanged<T>(string PropertyName, T OldValue, T NewValue)
        {
            #region Validate simultaneous two side inputs.
            if (PropertyName == nameof(LParam1) || PropertyName == nameof(LParam2))
            {
                if (LParam1 == null)
                {
                    LParam2 = null;
                    LResult = null;
                }
                else
                {
                    RParam1 = null;
                }
            }

            if (PropertyName == nameof(RParam1) || PropertyName == nameof(RParam2))
            {
                if (RParam1 == null)
                {
                    RParam2 = null;
                    RResult = null;
                }
                else
                {
                    LParam1 = null;
                }
            }

            //Do not proceed if Account and StockItem are unset.
            if (Account == null || StockItem == null)
            {
                LParam1 = RParam1 = null;
                return;
            }
            #endregion

            #region Reset dependent properties
            if (PropertyName == nameof(Account))
            {
                StockItem = null;
            }

            if (PropertyName == nameof(StockItem))
            {
                LParam1 = RParam1 = null;
            }
            #endregion

            #region Determine the Type of entry
            if (Account == DataContextService.Instance.DataContext.Self)
            {
                Type = RojmelEntryType.Initial;
            }
            else if (StockItem == DataContextService.Instance.DataContext.Cash)
            {
                Type = Param2 == null ? RojmelEntryType.SimpleCashExchange : RojmelEntryType.UseCash;
            }
            else if (StockItem == DataContextService.Instance.DataContext.None)
            {
                Type = RojmelEntryType.UseCash;
            }
            else
            {
                Type = Account.Type == AccountType.Regular ? RojmelEntryType.ItemExchangeFine : RojmelEntryType.ItemExchangeCash;
            }
            #endregion

            if (PropertyName == nameof(Date) ||
                PropertyName == nameof(PartialPaymentInterval) || PropertyName == nameof(TotalPaymentDueDate) ||
                PropertyName == nameof(Labour) ||
                PropertyName == nameof(Waste) || PropertyName == nameof(IsLabourAsAmount))
            {
                if (Validate() == null)
                {
                    CommitEntry();
                }
            }
        }

        public string Validate()
        {
            var dataContext = DataContextService.Instance.DataContext;

            if (Account == null)
            {
                return "Account is required";
            }
            if (StockItem == null)
            {
                return "Stock item is required";
            }
            if (Account == dataContext.Self && StockItem == dataContext.None)
            {
                return "You cannot make a Self transaction with no item!";
            }
            if (Param1 == null)
            {
                return "Param 1 is required";
            }
            if (Account != dataContext.Self && StockItem != dataContext.Cash && Param2 == null)
            {
                return "Param 2 is required";
            }
            if (Account == dataContext.Self && Param2 != null)
            {
                return "Self transactions are made for initial stock entry. Please remove entry from column 2.";
            }

            return null;
        }

        public void CommitEntry()
        {
            DataContextService.Instance.DataContext.RemoveRojmelEntry(Model);

            Model.Date = Date;
            Model.AccountName = Account.Name;
            Model.Type = Type;
            Model.StockItemName = StockItem.Name;
            Model.Param1 = (double)Param1;
            Model.Param2 = Param2;
            Model.Labour = Labour;
            Model.Waste = Waste;
            Model.PartialPaymentInterval = PartialPaymentInterval;
            Model.FullPaymentDueDays = (TotalPaymentDueDate - Date).Days;
            Model.IsLabourAsAmount = IsLabourAsAmount;
            Model.IsLeftSide = IsLeftSide;

            DataContextService.Instance.DataContext.AddRojmelEntry(Model, Id == null);

            Id = Model.Id;
            Result = Model.Result;
        }
    }
}
