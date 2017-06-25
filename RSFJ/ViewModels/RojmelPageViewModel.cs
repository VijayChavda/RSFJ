using RSFJ.Services;
using RSFJ.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace RSFJ.ViewModels
{
    public class RojmelPageViewModel : ViewModelBase
    {
        public ObservableCollection<RojmelEntryViewModel> Entries { get; set; }

        private RojmelEntryViewModel _SelectedEntry;
        public RojmelEntryViewModel SelectedEntry { get => _SelectedEntry; set => SetProperty(ref _SelectedEntry, value); }

        private bool _ShowAggregateColumns;
        public bool ShowAggregateColumns { get => _ShowAggregateColumns; set => SetProperty(ref _ShowAggregateColumns, value); }

        public RojmelPageViewModel()
        {
            Entries = new ObservableCollection<RojmelEntryViewModel>();

            LoadData();

            RegistoryService.Instance.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(RegistoryService.ShowAggregateColumns))
                {
                    ShowAggregateColumns = RegistoryService.Instance.ShowAggregateColumns;
                }
            };
        }

        private void LoadData()
        {
            foreach (var model in DataContextService.Instance.DataContext.RojmelEntries)
            {
                Entries.Add(new RojmelEntryViewModel(model));
            }

            SelectedEntry = Entries.FirstOrDefault();

            ShowAggregateColumns = RegistoryService.Instance.ShowAggregateColumns;
        }

        public async Task CalculateAggregateAsync()
        {
            await Task.Run(() =>
            {
                var groupedEntries = Entries.Where(x => x.StockItem != null).GroupBy(x => x.StockItem);

                foreach (var sameStockEntries in groupedEntries)
                {
                    var stockItem = sameStockEntries.Key;
                    double inStock = 0;

                    foreach (var entry in sameStockEntries)
                    {
                        var model = entry.Model;
                        inStock += model.IsLeftSide ? model.Param1 : -model.Param1;

                        entry.AggregateInStock = inStock;
                    }

                    stockItem.InStock = inStock;
                }
            });

            await Task.Run(() =>
            {
                var groupedEntries = Entries.Where(x => x.Account != null).GroupBy(x => x.Account);

                foreach (var sameAccountEntries in groupedEntries)
                {
                    var account = sameAccountEntries.Key;
                    double fineInMoney = 0;
                    double fineInGold = 0;

                    foreach (var entry in sameAccountEntries)
                    {
                        var model = entry.Model;
                        if (entry.Type == RojmelEntryType.Exchange)
                        {
                            fineInGold += model.IsLeftSide ? -model.Result : model.Result;
                        }
                        else if (entry.Type == RojmelEntryType.UplakClear)
                        {
                            fineInGold += model.IsLeftSide ? -model.Result : model.Result;
                            fineInMoney += model.IsLeftSide ? model.Param1 : -model.Param1;
                        }
                        else if (entry.Type == RojmelEntryType.InstantCash)
                        {
                            fineInGold += model.IsLeftSide ? -model.Result : model.Result;
                        }
                        else
                        {
                            fineInMoney += model.IsLeftSide ? -model.Result : model.Result;
                        }

                        entry.AggregateFineInMoney = fineInMoney;
                        entry.AggregateFineInGold = fineInGold;
                    }

                    account.FineInGold = fineInGold;
                    account.FineInMoney = fineInMoney;
                }
            });
        }
    }

    public class RojmelEntryViewModel : ViewModelBase
    {
        public static int InstanceCount;

        public RojmelEntry Model { get; set; }

        #region Common parameters
        private int _Id;
        public int Id { get => _Id; set => SetProperty(ref _Id, value); }

        private DateTime _Date;
        public DateTime Date { get => _Date; set => SetProperty(ref _Date, value); }

        private Account _Account;
        public Account Account { get => _Account; set => SetProperty(ref _Account, value); }

        private string _Type;
        public string Type { get => _Type; set => SetProperty(ref _Type, value); }

        private StockItem _StockItem;
        public StockItem StockItem { get => _StockItem; set => SetProperty(ref _StockItem, value); }
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
        private double? _Labour;
        public double? Labour { get => _Labour; set => SetProperty(ref _Labour, value); }

        private double? _Loss;
        public double? Loss { get => _Loss; set => SetProperty(ref _Loss, value); }

        private DateTime _PaymentDue;
        public DateTime PaymentDue { get => _PaymentDue; set => SetProperty(ref _PaymentDue, value); }

        private DateTime _InstallmentPaymentDue;
        public DateTime InstallmentPaymentDue { get => _InstallmentPaymentDue; set => SetProperty(ref _InstallmentPaymentDue, value); }
        #endregion

        #region Aggregate parameters
        private double? _AggregateInStock;
        public double? AggregateInStock { get => _AggregateInStock; set => SetProperty(ref _AggregateInStock, value); }

        private double? _AggregateFineInMoney;
        public double? AggregateFineInMoney { get => _AggregateFineInMoney; set => SetProperty(ref _AggregateFineInMoney, value); }

        private double? _AggregateFineInGold;
        public double? AggregateFineInGold { get => _AggregateFineInGold; set => SetProperty(ref _AggregateFineInGold, value); }
        #endregion

        #region Column Headings
        private string _HeadingParam1;
        public string HeadingParam1 { get => _HeadingParam1; set => SetProperty(ref _HeadingParam1, value); }

        private string _HeadingParam2;
        public string HeadingParam2 { get => _HeadingParam2; set => SetProperty(ref _HeadingParam2, value); }

        private string _HeadingResult;
        public string HeadingResult { get => _HeadingResult; set => SetProperty(ref _HeadingResult, value); }
        #endregion

        public static ObservableCollection<StockItem> StockItems { get; set; }
        public static ObservableCollection<Account> Accounts { get; set; }

        static RojmelEntryViewModel()
        {
            #region Accounts
            Accounts = new ObservableCollection<Account>();

            DataContextService.Instance.DataContext.AccountAdded += (s, account) => Accounts.Add(account);
            DataContextService.Instance.DataContext.AccountRemoved += (s, account) => Accounts.Remove(account);

            foreach (var account in DataContextService.Instance.DataContext.Accounts)
            {
                Accounts.Add(account);
            }
            #endregion

            #region StockItems
            StockItems = new ObservableCollection<StockItem>();

            DataContextService.Instance.DataContext.StockItemAdded += (s, stockItem) => StockItems.Add(stockItem);
            DataContextService.Instance.DataContext.StockItemRemoved += (s, stockItem) => StockItems.Remove(stockItem);

            foreach (var stockItem in DataContextService.Instance.DataContext.StockItems)
            {
                StockItems.Add(stockItem);
            }
            #endregion
        }

        public RojmelEntryViewModel()
        {
            InstanceCount++;
            Model = new RojmelEntry() { Id = InstanceCount };

            Id = InstanceCount;
            Date = DateTime.Now.Date;
            Account = DataContextService.Instance.DataContext.Accounts.FirstOrDefault();
            PaymentDue = DateTime.Now.Add(TimeSpan.FromDays(60));
            InstallmentPaymentDue = DateTime.Now.Add(TimeSpan.FromDays(10));

            SetHeadings();

            DataContextService.Instance.DataContext.RojmelEntries.Add(Model);
        }

        public RojmelEntryViewModel(RojmelEntry Model)
        {
            InstanceCount++;
            this.Model = Model;

            _Id = Model.Id;
            _Date = Model.Date;
            _Account = Model.Account;
            _Type = Model.Type;
            _StockItem = Model.StockItem;

            _LParam1 = Model.IsLeftSide ? Model.Param1 : (double?)null;
            _LParam2 = Model.IsLeftSide ? Model.Param2 : (double?)null;
            _LResult = Model.IsLeftSide ? Model.Result : (double?)null;
            _RParam1 = Model.IsLeftSide ? (double?)null : Model.Param1;
            _RParam2 = Model.IsLeftSide ? (double?)null : Model.Param2;
            _RResult = Model.IsLeftSide ? (double?)null : Model.Result;

            if (Type == RojmelEntryType.Customer)
            {
                _Labour = Model.Param3 == null ? (double?)null : Convert.ToDouble(Model.Param3);
                _Loss = Model.Param4 == null ? (double?)null : Convert.ToDouble(Model.Param4);
            }

            _PaymentDue = Type == RojmelEntryType.Exchange ? Convert.ToDateTime(Model.Param3) : DateTime.MaxValue;
            _InstallmentPaymentDue = Type == RojmelEntryType.Exchange ? Convert.ToDateTime(Model.Param4) : DateTime.MaxValue;

            SetHeadings();

            DataContextService.Instance.DataContext.RojmelEntries.Add(Model);
        }

        protected override void APropertyChanged<T>(string PropertyName, T OldValue, T NewValue)
        {
            if (Account == null || StockItem == null)
            {
                return;
            }

            if (PropertyName == nameof(Account))
            {
                Type = Account.PreferredTransactionType;

                StockItem = null;
                LParam1 = RParam1 = null;
            }

            if (PropertyName == nameof(StockItem))
            {
                Type = StockItem == DataContext.None ? RojmelEntryType.UplakClear :
                    StockItem == DataContext.Cash ? RojmelEntryType.Uplak :
                    Account.PreferredTransactionType;

                LParam1 = RParam1 = null;
            }

            SetHeadings();

            #region Calculations
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

            if (PropertyName == nameof(LParam1) || PropertyName == nameof(LParam2) ||
                PropertyName == nameof(RParam1) || PropertyName == nameof(RParam2))
            {
                if (LParam1 == null && RParam1 == null)
                {
                    return;
                }

                bool isLeft = RParam1 == null;
                double? param1 = isLeft ? LParam1 : RParam1;
                double? param2 = isLeft ? LParam2 : RParam2;
                double? result = isLeft ? LResult : RResult;

                //Uplak or InstantCash?
                if (param2 != null)
                {
                    if (StockItem == DataContext.Cash)
                    {
                        Type = (isLeft ? LParam2 : RParam2) == null ? RojmelEntryType.Uplak : RojmelEntryType.InstantCash;
                    }
                }

                if (Type == RojmelEntryType.Exchange)
                {
                    result = param1 * param2 / 100;
                }
                else if (Type == RojmelEntryType.Customer)
                {
                    result = param1 * param2;   //TODO
                }
                else if (Type == RojmelEntryType.Bullion)
                {
                    result = StockItem == DataContext.Cash ? param1 : param1 * param2;
                }
                else if (Type == RojmelEntryType.Uplak)
                {
                    result = param1;
                }
                else if (Type == RojmelEntryType.UplakClear)
                {
                    result = param1 / param2;
                }
                else if (Type == RojmelEntryType.InstantCash)
                {
                    result = param1 / param2;
                }

                if (isLeft)
                {
                    LParam1 = param1;
                    LParam2 = param2;
                    LResult = result;
                }
                else
                {
                    RParam1 = param1;
                    RParam2 = param2;
                    RResult = result;
                }

                #region Model update
                Model.Id = Id;
                Model.Date = Date;
                Model.Account = Account;
                Model.Type = Type;
                Model.StockItem = StockItem;
                Model.IsLeftSide = isLeft;
                Model.Param1 = isLeft ? LParam1 ?? 0 : RParam1 ?? 0;
                Model.Param2 = isLeft ? LParam2 ?? 0 : RParam2 ?? 0;
                Model.Result = isLeft ? LResult ?? 0 : RResult ?? 0;

                //Following is temporary.
                Model.Param3 = Type == RojmelEntryType.Customer ? Labour : (object)InstallmentPaymentDue;
                Model.Param4 = Type == RojmelEntryType.Customer ? Loss : (object)PaymentDue;
                #endregion
            }
            #endregion
        }

        private void SetHeadings()
        {
            const string h_rupees = "Rs.";
            const string h_gram = "Grams";
            const string h_percent = "%";
            const string h_rate = "Rate";
            const string h_fine = "Fine";
            const string h_na = "-";

            if (Type == RojmelEntryType.Exchange)
            {
                HeadingParam1 = h_gram;
                HeadingParam2 = h_percent;
                HeadingResult = h_fine;
            }
            else if (Type == RojmelEntryType.Customer)
            {
                HeadingParam1 = h_gram;
                HeadingParam2 = h_rate;
                HeadingResult = h_rupees;
            }
            else if (Type == RojmelEntryType.Bullion)
            {
                HeadingParam1 = StockItem == DataContext.Cash ? h_rupees : h_gram;
                HeadingParam2 = StockItem == DataContext.Cash ? h_na : h_rate;
                HeadingResult = h_rupees;
            }
            else if (Type == RojmelEntryType.Uplak)
            {
                HeadingParam1 = h_rupees;
                HeadingParam2 = h_na;
                HeadingResult = h_rupees;
            }
            else if (Type == RojmelEntryType.UplakClear)
            {
                HeadingParam1 = h_rupees;
                HeadingParam2 = h_rate;
                HeadingResult = h_fine;
            }
        }
    }
}
