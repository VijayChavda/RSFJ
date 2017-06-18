using RSFJ.Services;
using RSFJ.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;

namespace RSFJ.ViewModels
{
    public class RojmelPageViewModel : ViewModelBase
    {
        public ObservableCollection<RojmelEntryViewModel> Entries { get; set; }

        public RojmelPageViewModel()
        {
            Entries = new ObservableCollection<RojmelEntryViewModel>();

            LoadData();
        }

        private void LoadData()
        {
            foreach (var model in DataContextService.Instance.DataContext.RojmelEntries)
            {
                Entries.Add(new RojmelEntryViewModel(model));
            }
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

        public List<StockItem> StockItems { get; set; }
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
        }

        public RojmelEntryViewModel()
        {
            InstanceCount++;
            Model = new RojmelEntry();

            Id = InstanceCount;
            Date = DateTime.Now.Date;
            Account = DataContextService.Instance.DataContext.Accounts.FirstOrDefault();
            PaymentDue = DateTime.Now.Add(TimeSpan.FromDays(60));
            InstallmentPaymentDue = DateTime.Now.Add(TimeSpan.FromDays(10));
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

            _Labour = Type == RojmelEntryType.Customer ? (double?)Model.Param3 : null;
            _Loss = Type == RojmelEntryType.Customer ? (double?)Model.Param4 : null;

            _PaymentDue = Type == RojmelEntryType.Exchange ? (DateTime)Model.Param3 : DateTime.MinValue;
            _InstallmentPaymentDue = Type == RojmelEntryType.Exchange ? (DateTime)Model.Param3 : DateTime.MinValue;
        }

        protected override void APropertyChanged<T>(string PropertyName, T OldValue, T NewValue)
        {
            if (PropertyName == nameof(Account))
            {
                Type = Account != null ? Account.PreferredTransactionType : RojmelEntryType.Exchange;
            }

            if (PropertyName == nameof(Type))
            {
                var dataContext = DataContextService.Instance.DataContext;
                StockItems = dataContext.StockItems.Where(x => x.AppliesToType.Contains(Type)).ToList();
                StockItem = StockItems.FirstOrDefault();
            }
        }
    }
}
