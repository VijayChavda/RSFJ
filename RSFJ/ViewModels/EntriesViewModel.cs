using RSFJ.Model;
using RSFJ.Services;
using RSFJ.ViewModels.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Timers;
using System.Windows.Input;

namespace RSFJ.ViewModels
{
    public class EntriesViewModel : ViewModelBase
    {
        public static ObservableCollection<StockItem> StockItems { get; set; }

        public ExchangeEntryViewModel ExchangeEntryViewModel { get; set; }
        public BullionEntryViewModel BullionEntryViewModel { get; set; }
        public CustomerEntryViewModel CustomerEntryViewModel { get; set; }

        private string _Message;
        public string Message { get => _Message; set => SetProperty(ref _Message, value); }

        public EntriesViewModel()
        {
            StockItems = new ObservableCollection<StockItem>();

            ExchangeEntryViewModel = new ExchangeEntryViewModel();
            BullionEntryViewModel = new BullionEntryViewModel();
            CustomerEntryViewModel = new CustomerEntryViewModel();

            LoadData();

            DataContextService.Instance.DataContext.StockItemAdded += (s, item) =>
            {
                if (StockItems.Count(x => x.Name == item.Name) == 0)
                    StockItems.Add(item);
            };

            ExchangeEntryViewModel.EntryAdded += OnEntryAdded;
            BullionEntryViewModel.EntryAdded += OnEntryAdded;
            CustomerEntryViewModel.EntryAdded += OnEntryAdded;

            ExchangeEntryViewModel.EntryError += (s, message) => ShowMessage(message);
            BullionEntryViewModel.EntryError += (s, message) => ShowMessage(message);
            CustomerEntryViewModel.EntryError += (s, message) => ShowMessage(message);
        }

        private void OnEntryAdded(object sender, RojmelEntry entry)
        {
            ShowMessage(string.Format("Entry {0} was successfully added.", entry.ToString()));

            ExchangeEntryViewModel.Reset();
            BullionEntryViewModel.Reset();
            CustomerEntryViewModel.Reset();
        }

        private void LoadData()
        {
            StockItems.Clear();
            foreach (var item in DataContextService.Instance.DataContext.StockItems.Except(
                new StockItem[] {
                    DataContextService.Instance.DataContext.Cash,
                    DataContextService.Instance.DataContext.None
                }))
            {
                StockItems.Add(item);
            }
        }

        private void ShowMessage(string message)
        {
            Message = message;

            var timer = new Timer(2000) { AutoReset = false };
            timer.Elapsed += (s, e) => Message = string.Empty;
            timer.Start();
        }
    }

    public interface IEntryViewModel
    {
        event EventHandler<RojmelEntry> EntryAdded;

        event EventHandler<string> EntryError;
    }

    public class ExchangeEntryViewModel : ViewModelBase, IEntryViewModel
    {
        public static ObservableCollection<Account> Accounts { get; set; }

        private Account _Account;
        public Account Account { get => _Account; set => SetProperty(ref _Account, value); }

        private bool _IsExchangeWithFine;
        public bool IsExchangeWithFine { get => _IsExchangeWithFine; set => SetProperty(ref _IsExchangeWithFine, value); }

        private bool _IsCashPayment;
        public bool IsCashPayment { get => _IsCashPayment; set => SetProperty(ref _IsCashPayment, value); }

        private bool _IsFineClearWithAccountBalance;
        public bool IsFineClearWithAccountBalance { get => _IsFineClearWithAccountBalance; set => SetProperty(ref _IsFineClearWithAccountBalance, value); }

        private bool _IsFineClear;
        public bool IsFineClear { get => _IsFineClear; set => SetProperty(ref _IsFineClear, value); }

        public ExchangeWithFineViewModel ExchangeWithFineViewModel { get; set; }
        public CashPaymentViewModel CashPaymentViewModel { get; set; }
        public FineClearWithAccountBalanceViewModel FineClearWithAccountBalanceViewModel { get; set; }
        public FineClearViewModel FineClearViewModel { get; set; }

        #region Commands
        RelayCommand _CreditCommand;
        public ICommand CreditCommand
        {
            get => _CreditCommand ?? (_CreditCommand = new RelayCommand(param => AddEntry(true), param => true));
        }

        RelayCommand _DebitCommand;
        public ICommand DebitCommand
        {
            get => _DebitCommand ?? (_DebitCommand = new RelayCommand(param => AddEntry(false), param => true));
        }
        #endregion

        public event EventHandler<RojmelEntry> EntryAdded;
        public event EventHandler<string> EntryError;

        public ExchangeEntryViewModel()
        {
            Accounts = new ObservableCollection<Account>();

            ExchangeWithFineViewModel = new ExchangeWithFineViewModel();
            CashPaymentViewModel = new CashPaymentViewModel();
            FineClearWithAccountBalanceViewModel = new FineClearWithAccountBalanceViewModel();
            FineClearViewModel = new FineClearViewModel();

            LoadData();

            DataContextService.Instance.DataContext.AccountAdded += (s, account) =>
            {
                if (Accounts.Count(x => x.Name == account.Name) == 0 && account.Type == AccountType.Regular)
                    Accounts.Add(account);
            };
        }

        private void LoadData()
        {
            Accounts.Clear();
            foreach (var item in DataContextService.Instance.DataContext.Accounts.Where(x => x.Type == AccountType.Regular))
            {
                Accounts.Add(item);
            }
        }

        private void AddEntry(bool OnLeftSide)
        {
            if (Account == null)
            {
                EntryError?.Invoke(this, "You need to select an Account for Rojmel entry.");
                return;
            }

            if (IsExchangeWithFine && ExchangeWithFineViewModel.StockItem == null)
            {
                EntryError?.Invoke(this, "You need to select an item for Rojmel entry.");
                return;
            }

            var entry = new RojmelEntry()
            {
                Account = Account,
                Date = DateTime.Now,
                IsLeftSide = OnLeftSide
            };

            if (IsExchangeWithFine)
            {
                entry.Type = RojmelEntryType.ItemExchangeFine;
                entry.StockItem = ExchangeWithFineViewModel.StockItem;
                entry.Param1 = ExchangeWithFineViewModel.Weight;
                entry.Param2 = ExchangeWithFineViewModel.Purity;
                entry.FullPaymentDueDays = DateTime.Now.Subtract(ExchangeWithFineViewModel.PaymentBefore).Days;
                entry.PartialPaymentInterval = DateTime.Now.Subtract(ExchangeWithFineViewModel.PaymentInterval).Days;
            }
            else if (IsCashPayment)
            {
                entry.Type = RojmelEntryType.SimpleCashExchange;
                entry.StockItem = DataContextService.Instance.DataContext.Cash;
                entry.Param1 = CashPaymentViewModel.Cash;
            }
            else if (IsFineClearWithAccountBalance)
            {
                entry.Type = RojmelEntryType.UseCash;
                entry.StockItem = DataContextService.Instance.DataContext.None;
                entry.Param1 = FineClearWithAccountBalanceViewModel.AccountBalance;
                entry.Param2 = FineClearWithAccountBalanceViewModel.Rate;
            }
            else if (IsFineClear)
            {
                entry.Type = RojmelEntryType.ItemExchangeCash;
                entry.StockItem = DataContextService.Instance.DataContext.Cash;
                entry.Param1 = FineClearViewModel.Cash;
                entry.Param2 = FineClearViewModel.Rate;
            }
            else
            {
                EntryError?.Invoke(this, "Please select an option.");
                return;
            }

            DataContextService.Instance.DataContext.AddRojmelEntry(entry);

            EntryAdded?.Invoke(this, entry);
        }

        internal void Reset()
        {
            Account = null;
            IsExchangeWithFine = false;
            IsCashPayment = false;
            IsFineClearWithAccountBalance = false;
            IsFineClear = false;

            ExchangeWithFineViewModel.Reset();
            CashPaymentViewModel.Reset();
            FineClearWithAccountBalanceViewModel.Reset();
            FineClearViewModel.Reset();
        }
    }

    public class BullionEntryViewModel : ViewModelBase, IEntryViewModel
    {
        public static ObservableCollection<Account> Accounts { get; set; }

        private Account _Account;
        public Account Account { get => _Account; set => SetProperty(ref _Account, value); }

        private bool _IsFine999Payment;
        public bool IsFine999Payment { get => _IsFine999Payment; set => SetProperty(ref _IsFine999Payment, value); }

        private bool _IsCashPayment;
        public bool IsCashPayment { get => _IsCashPayment; set => SetProperty(ref _IsCashPayment, value); }

        public Fine999PaymentViewModel Fine999PaymentViewModel { get; set; }
        public CashPaymentViewModel CashPaymentViewModel { get; set; }

        #region Commands
        RelayCommand _CreditCommand;
        public ICommand CreditCommand
        {
            get => _CreditCommand ?? (_CreditCommand = new RelayCommand(param => AddEntry(true), param => true));
        }

        RelayCommand _DebitCommand;
        public ICommand DebitCommand
        {
            get => _DebitCommand ?? (_DebitCommand = new RelayCommand(param => AddEntry(false), param => true));
        }
        #endregion

        public event EventHandler<RojmelEntry> EntryAdded;
        public event EventHandler<string> EntryError;

        public BullionEntryViewModel()
        {
            Accounts = new ObservableCollection<Account>();

            Fine999PaymentViewModel = new Fine999PaymentViewModel();
            CashPaymentViewModel = new CashPaymentViewModel();

            LoadData();

            DataContextService.Instance.DataContext.AccountAdded += (s, account) =>
            {
                if (Accounts.Count(x => x.Name == account.Name) == 0 && account.Type == AccountType.Boolean)
                    Accounts.Add(account);
            };
        }

        private void LoadData()
        {
            Accounts.Clear();
            foreach (var item in DataContextService.Instance.DataContext.Accounts.Where(x => x.Type == AccountType.Boolean))
            {
                Accounts.Add(item);
            }
        }

        private void AddEntry(bool OnLeftSide)
        {
            if (Account == null)
            {
                EntryError?.Invoke(this, "You need to select an Account for Rojmel entry.");
                return;
            }

            var entry = new RojmelEntry()
            {
                Account = Account,
                Date = DateTime.Now,
                IsLeftSide = OnLeftSide
            };

            if (IsFine999Payment)
            {
                entry.Type = RojmelEntryType.ItemExchangeCash;
                entry.StockItem = DataContextService.Instance.DataContext.Fine999;
                entry.Param1 = Fine999PaymentViewModel.Weight;
                entry.Param2 = Fine999PaymentViewModel.Rate;
                entry.FullPaymentDueDays = DateTime.Now.Subtract(Fine999PaymentViewModel.PaymentBefore).Days;
                entry.PartialPaymentInterval = DateTime.Now.Subtract(Fine999PaymentViewModel.PaymentInterval).Days;
            }
            else if (IsCashPayment)
            {
                entry.Type = RojmelEntryType.SimpleCashExchange;
                entry.StockItem = DataContextService.Instance.DataContext.Cash;
                entry.Param1 = CashPaymentViewModel.Cash;
            }
            else
            {
                EntryError?.Invoke(this, "Please select an option.");
                return;
            }

            DataContextService.Instance.DataContext.AddRojmelEntry(entry);

            EntryAdded?.Invoke(this, entry);
        }

        internal void Reset()
        {
            Account = null;
            IsFine999Payment = false;
            IsCashPayment = false;

            Fine999PaymentViewModel.Reset();
            CashPaymentViewModel.Reset();
        }
    }

    public class CustomerEntryViewModel : ViewModelBase, IEntryViewModel
    {
        public static ObservableCollection<Account> Accounts { get; set; }

        private Account _Account;
        public Account Account { get => _Account; set => SetProperty(ref _Account, value); }

        private StockItem _StockItem;
        public StockItem StockItem { get => _StockItem; set => SetProperty(ref _StockItem, value); }

        private double _Weight;
        public double Weight { get => _Weight; set => SetProperty(ref _Weight, value); }

        private double _Rate;
        public double Rate { get => _Rate; set => SetProperty(ref _Rate, value); }

        private double _Labour;
        public double Labour { get => _Labour; set => SetProperty(ref _Labour, value); }

        private double _Waste;
        public double Waste { get => _Waste; set => SetProperty(ref _Waste, value); }

        private bool _IsLabourAsAmount;
        public bool IsLabourAsAmount { get => _IsLabourAsAmount; set => SetProperty(ref _IsLabourAsAmount, value); }

        #region Commands
        RelayCommand _CreditCommand;
        public ICommand CreditCommand
        {
            get => _CreditCommand ?? (_CreditCommand = new RelayCommand(param => AddEntry(true), param => true));
        }

        RelayCommand _DebitCommand;
        public ICommand DebitCommand
        {
            get => _DebitCommand ?? (_DebitCommand = new RelayCommand(param => AddEntry(false), param => true));
        }
        #endregion

        public event EventHandler<RojmelEntry> EntryAdded;
        public event EventHandler<string> EntryError;

        public CustomerEntryViewModel()
        {
            Accounts = new ObservableCollection<Account>();

            LoadData();

            DataContextService.Instance.DataContext.AccountAdded += (s, account) =>
            {
                if (Accounts.Count(x => x.Name == account.Name) == 0 && account.Type == AccountType.Customer)
                    Accounts.Add(account);
            };
        }

        private void LoadData()
        {
            Accounts.Clear();
            foreach (var item in DataContextService.Instance.DataContext.Accounts.Where(x => x.Type == AccountType.Customer))
            {
                Accounts.Add(item);
            }
        }

        private void AddEntry(bool OnLeftSide)
        {
            if (Account == null)
            {
                EntryError?.Invoke(this, "You need to select an Account for Rojmel entry.");
                return;
            }

            if (StockItem == null)
            {
                EntryError?.Invoke(this, "You need to select an item for Rojmel entry.");
                return;
            }

            var entry = new RojmelEntry()
            {
                Type = RojmelEntryType.ItemExchangeCash,
                Account = Account,
                StockItem = StockItem,
                Date = DateTime.Now,
                IsLeftSide = OnLeftSide,
                IsLabourAsAmount = IsLabourAsAmount,
                Labour = Labour,
                Param1 = Weight,
                Param2 = Rate,
                Waste = Waste
            };

            DataContextService.Instance.DataContext.AddRojmelEntry(entry);

            EntryAdded?.Invoke(this, entry);
        }

        internal void Reset()
        {
            Account = null;
            StockItem = null;

            Weight = 0;
            Rate = 0;
            Labour = 0;
            Waste = 0;
            IsLabourAsAmount = false;
        }
    }

    #region Sub types
    public class ExchangeWithFineViewModel : ViewModelBase
    {
        private StockItem _StockItem;
        public StockItem StockItem { get => _StockItem; set => SetProperty(ref _StockItem, value); }

        private double _Weight;
        public double Weight { get => _Weight; set => SetProperty(ref _Weight, value); }

        private double _Purity;
        public double Purity { get => _Purity; set => SetProperty(ref _Purity, value); }

        private DateTime _PaymentBefore;
        public DateTime PaymentBefore { get => _PaymentBefore; set => SetProperty(ref _PaymentBefore, value); }

        private DateTime _PaymentInterval;
        public DateTime PaymentInterval { get => _PaymentInterval; set => SetProperty(ref _PaymentInterval, value); }

        public ExchangeWithFineViewModel()
        {
            StockItem = EntriesViewModel.StockItems.FirstOrDefault();
            PaymentBefore = DateTime.Now.AddDays(30);
            PaymentInterval = DateTime.Now.AddDays(10);
        }

        internal void Reset()
        {
            StockItem = null;
            Weight = 0;
            Purity = 0;
            PaymentBefore = DateTime.Today.AddMonths(1);
            PaymentInterval = DateTime.Today.AddDays(10);
        }
    }

    public class CashPaymentViewModel : ViewModelBase
    {
        private double _Cash;
        public double Cash { get => _Cash; set => SetProperty(ref _Cash, value); }

        internal void Reset()
        {
            Cash = 0;
        }
    }

    public class FineClearWithAccountBalanceViewModel : ViewModelBase
    {
        private double _AccountBalance;
        public double AccountBalance { get => _AccountBalance; set => SetProperty(ref _AccountBalance, value); }

        private double _Rate;
        public double Rate { get => _Rate; set => SetProperty(ref _Rate, value); }

        internal void Reset()
        {
            AccountBalance = 0;
            Rate = 0;
        }
    }

    public class FineClearViewModel : ViewModelBase
    {
        private double _Cash;
        public double Cash { get => _Cash; set => SetProperty(ref _Cash, value); }

        private double _Rate;
        public double Rate { get => _Rate; set => SetProperty(ref _Rate, value); }

        internal void Reset()
        {
            Cash = 0;
            Rate = 0;
        }
    }

    public class Fine999PaymentViewModel : ViewModelBase
    {
        private double _Weight;
        public double Weight { get => _Weight; set => SetProperty(ref _Weight, value); }

        private double _Rate;
        public double Rate { get => _Rate; set => SetProperty(ref _Rate, value); }

        private DateTime _PaymentBefore;
        public DateTime PaymentBefore { get => _PaymentBefore; set => SetProperty(ref _PaymentBefore, value); }

        private DateTime _PaymentInterval;
        public DateTime PaymentInterval { get => _PaymentInterval; set => SetProperty(ref _PaymentInterval, value); }

        internal void Reset()
        {
            Weight = 0;
            Rate = 0;
            PaymentBefore = DateTime.Today.AddMonths(1);
            PaymentInterval = DateTime.Today.AddDays(10);
        }
    }
    #endregion
}
