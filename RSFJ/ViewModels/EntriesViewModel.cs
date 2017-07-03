using RSFJ.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSFJ.ViewModels
{
    public class EntriesViewModel : ViewModelBase
    {
        public static List<StockItem> StockItems { get; set; }

        public ExchangeEntryViewModel ExchangeEntryViewModel { get; set; }
        public BullionEntryViewModel BullionEntryViewModel { get; set; }
        public CustomerEntryViewModel CustomerEntryViewModel { get; set; }

        public EntriesViewModel()
        {
            StockItems = new List<StockItem>();
            LoadData();

            ExchangeEntryViewModel = new ExchangeEntryViewModel();
            BullionEntryViewModel = new BullionEntryViewModel();
            CustomerEntryViewModel = new CustomerEntryViewModel();
        }

        private void LoadData()
        {
            StockItems.Clear();
            StockItems.AddRange(Services.DataContextService.Instance.DataContext.
                StockItems.Except(new StockItem[] { StockItem.Cash, StockItem.None }));
        }
    }

    public class ExchangeEntryViewModel : ViewModelBase
    {
        public static List<Account> Accounts { get; set; }

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

        public ExchangeEntryViewModel()
        {
            Accounts = new List<Account>();

            ExchangeWithFineViewModel = new ExchangeWithFineViewModel();
            CashPaymentViewModel = new CashPaymentViewModel();
            FineClearWithAccountBalanceViewModel = new FineClearWithAccountBalanceViewModel();
            FineClearViewModel = new FineClearViewModel();

            LoadData();
        }

        private void LoadData()
        {
            Accounts.Clear();
            Accounts.AddRange(Services.DataContextService.Instance.DataContext.Accounts.Where(x => x.Type == AccountType.Regular));
            Account = Accounts.FirstOrDefault();
        }
    }

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
    }

    public class CashPaymentViewModel : ViewModelBase
    {
        private double _Cash;
        public double Cash { get => _Cash; set => SetProperty(ref _Cash, value); }
    }

    public class FineClearWithAccountBalanceViewModel : ViewModelBase
    {
        private double _AccountBalance;
        public double AccountBalance { get => _AccountBalance; set => SetProperty(ref _AccountBalance, value); }

        private double _Rate;
        public double Rate { get => _Rate; set => SetProperty(ref _Rate, value); }
    }

    public class FineClearViewModel : ViewModelBase
    {
        private double _Cash;
        public double Cash { get => _Cash; set => SetProperty(ref _Cash, value); }

        private double _Rate;
        public double Rate { get => _Rate; set => SetProperty(ref _Rate, value); }
    }

    public class BullionEntryViewModel : ViewModelBase
    {
        public static List<Account> Accounts { get; set; }

        private Account _Account;
        public Account Account { get => _Account; set => SetProperty(ref _Account, value); }

        private bool _IsFine999Payment;
        public bool IsFine999Payment { get => _IsFine999Payment; set => SetProperty(ref _IsFine999Payment, value); }

        private bool _IsCashPayment;
        public bool IsCashPayment { get => _IsCashPayment; set => SetProperty(ref _IsCashPayment, value); }

        public Fine999PaymentViewModel Fine999PaymentViewModel { get; set; }
        public CashPaymentViewModel CashPaymentViewModel { get; set; }

        public BullionEntryViewModel()
        {
            Accounts = new List<Account>();

            Fine999PaymentViewModel = new Fine999PaymentViewModel();
            CashPaymentViewModel = new CashPaymentViewModel();

            LoadData();
        }

        private void LoadData()
        {
            Accounts.Clear();
            Accounts.AddRange(Services.DataContextService.Instance.DataContext.Accounts.Where(x => x.Type == AccountType.Boolean));
            Account = Accounts.FirstOrDefault();
        }
    }

    public class Fine999PaymentViewModel : ViewModelBase
    {
        private double _Weight;
        public double Weight { get => _Weight; set => SetProperty(ref _Weight, value); }

        private double _Rate;
        public double Rate { get => _Rate; set => SetProperty(ref _Rate, value); }
    }

    public class CustomerEntryViewModel : ViewModelBase
    {
        public static List<Account> Accounts { get; set; }

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

        public CustomerEntryViewModel()
        {
            Accounts = new List<Account>();

            LoadData();
        }

        private void LoadData()
        {
            Accounts.Clear();
            Accounts.AddRange(Services.DataContextService.Instance.DataContext.Accounts.Where(x => x.Type == AccountType.Customer));
            Account = Accounts.FirstOrDefault();

            StockItem = EntriesViewModel.StockItems.FirstOrDefault();
        }
    }
}
