﻿using RSFJ.Model;
using RSFJ.ViewModels.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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

        private void AddEntry(bool OnLeftSide)
        {
            var entry = new RojmelEntry()
            {
                Id = Services.DataContextService.Instance.DataContext.RojmelEntries.Count + 1,
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
                entry.StockItem = StockItem.Cash;
                entry.Param1 = CashPaymentViewModel.Cash;
            }
            else if (IsFineClearWithAccountBalance)
            {
                entry.Type = RojmelEntryType.UseCash;
                entry.StockItem = StockItem.None;
                entry.Param1 = FineClearWithAccountBalanceViewModel.AccountBalance;
                entry.Param2 = FineClearWithAccountBalanceViewModel.Rate;
            }
            else if (IsFineClear)
            {
                entry.Type = RojmelEntryType.ItemExchangeCash;
                entry.StockItem = StockItem.Cash;
                entry.Param1 = FineClearViewModel.Cash;
                entry.Param2 = FineClearViewModel.Rate;
            }

            Services.DataContextService.Instance.DataContext.RojmelEntries.Add(entry);
        }
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

        private void AddEntry(bool OnLeftSide)
        {
            var entry = new RojmelEntry()
            {
                Id = Services.DataContextService.Instance.DataContext.RojmelEntries.Count + 1,
                Account = Account,
                Date = DateTime.Now,
                IsLeftSide = OnLeftSide
            };

            if (IsFine999Payment)
            {
                entry.Type = RojmelEntryType.ItemExchangeCash;
                entry.StockItem = StockItem.Fine999;
                entry.Param1 = Fine999PaymentViewModel.Weight;
                entry.Param2 = Fine999PaymentViewModel.Rate;
                entry.FullPaymentDueDays = DateTime.Now.Subtract(Fine999PaymentViewModel.PaymentBefore).Days;
                entry.PartialPaymentInterval = DateTime.Now.Subtract(Fine999PaymentViewModel.PaymentInterval).Days;
            }
            else if (IsCashPayment)
            {
                entry.Type = RojmelEntryType.SimpleCashExchange;
                entry.StockItem = StockItem.Cash;
                entry.Param1 = CashPaymentViewModel.Cash;
            }

            Services.DataContextService.Instance.DataContext.RojmelEntries.Add(entry);
        }
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

        private void AddEntry(bool OnLeftSide)
        {
            var entry = new RojmelEntry()
            {
                Id = Services.DataContextService.Instance.DataContext.RojmelEntries.Count + 1,
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

            Services.DataContextService.Instance.DataContext.RojmelEntries.Add(entry);
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
    }
    #endregion
}
