﻿using RSFJ.Services;
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

                        entry.StockItemBalance = inStock;
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

                        switch (entry.Type)
                        {
                            case RojmelEntryType.ItemExchangeFine:
                                fineInGold += model.IsLeftSide ? -model.Result : model.Result;
                                break;
                            case RojmelEntryType.ItemExchangeCash:
                                fineInMoney += model.IsLeftSide ? -model.Result : model.Result;
                                fineInGold += model.IsLeftSide ? model.Param1 : -model.Param1;
                                break;
                            case RojmelEntryType.SimpleCashExchange:
                                fineInMoney += model.IsLeftSide ? -model.Result : model.Result;
                                break;
                            case RojmelEntryType.UseCash:
                                fineInGold += model.IsLeftSide ? -model.Result : model.Result;
                                fineInMoney += model.IsLeftSide ? model.Param1 : -model.Param1;
                                break;
                        }

                        entry.AccountMoneyBalance = fineInMoney;
                        entry.AccountFineBalance = fineInGold;
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
        private double? _Labour;
        public double? Labour { get => _Labour; set => SetProperty(ref _Labour, value); }

        private double? _Waste;
        public double? Waste { get => _Waste; set => SetProperty(ref _Waste, value); }

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

        #region UI parameters
        private bool _IsParam2Disabled;
        public bool IsParam2Disabled { get => _IsParam2Disabled; set => SetProperty(ref _IsParam2Disabled, value); }
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
            TotalPaymentDueDate = DateTime.Now.Add(TimeSpan.FromDays(60));    //TODO: Take the last value
            PartialPaymentInterval = 10;    //TODO: Take the last value

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
            _Labour = Model.Labour;
            _Waste = Model.Waste;
            _PartialPaymentInterval = Model.PartialPaymentInterval;
            _TotalPaymentDueDate = Model.Date.AddDays(Model.FullPaymentDueDays);
            _IsLabourAsAmount = Model.IsLabourAsAmount;

            DataContextService.Instance.DataContext.RojmelEntries.Add(Model);
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

            #region Determine the Type of entry
            if (PropertyName == nameof(LParam2) || PropertyName == nameof(RParam2))
            {
                if (StockItem == StockItem.Cash)
                {
                    Type = Param2 == null ? RojmelEntryType.SimpleCashExchange :
                        (Account.Type == AccountType.Regular ? RojmelEntryType.UseCash : RojmelEntryType.Invalid);
                }
            }

            if (PropertyName == nameof(Account) || PropertyName == nameof(StockItem))
            {
                if (StockItem == StockItem.Cash)
                {
                    Type = RojmelEntryType.SimpleCashExchange;    //May change due to change in Param2.
                }
                else if (StockItem == StockItem.None)
                {
                    Type = Account.Type == AccountType.Regular ? RojmelEntryType.UseCash : RojmelEntryType.Invalid;
                }
                else
                {
                    Type = Account.Type == AccountType.Regular ? RojmelEntryType.ItemExchangeFine : RojmelEntryType.ItemExchangeCash;
                }

                //Reset dependent properties
                if (PropertyName == nameof(Account))
                {
                    StockItem = null;
                }

                if (PropertyName == nameof(StockItem))
                {
                    LParam1 = RParam1 = null;
                }
            }
            #endregion

            #region Calculations
            if (PropertyName == nameof(LParam1) || PropertyName == nameof(LParam2) ||
                PropertyName == nameof(RParam1) || PropertyName == nameof(RParam2))
            {
                if (Param1 == null)
                    return;

                //Round off the parameters first.
                Param1 = Math.Round(Param1 ?? 0, 2);
                Param2 = Math.Round(Param2 ?? 0, 2);
                Result = Math.Round(Result ?? 0, 2);

                switch (Type)
                {
                    case RojmelEntryType.ItemExchangeFine:
                        Result = Param1 * Param2 / 100;
                        break;
                    case RojmelEntryType.ItemExchangeCash:
                        Result = Param1 * Param2;
                        break;
                    case RojmelEntryType.SimpleCashExchange:
                        Result = Param1;
                        break;
                    case RojmelEntryType.UseCash:
                        Result = Param1 / Param2;
                        break;
                    case RojmelEntryType.Invalid:
                        Result = null;
                        break;
                }

                if (Result == null)
                    return;

            }
            #endregion

            #region Model update
            Model.Id = Id;
            Model.Date = Date;
            Model.Account = Account;
            Model.Type = Type;
            Model.StockItem = StockItem;
            Model.Param1 = Param1 ?? 0;
            Model.Param2 = Param2;
            Model.Result = Result ?? 0;
            Model.Labour = Labour;
            Model.Waste = Waste;
            Model.PartialPaymentInterval = PartialPaymentInterval;
            Model.FullPaymentDueDays = (TotalPaymentDueDate - Date).Days;
            Model.IsLabourAsAmount = IsLabourAsAmount;
            Model.IsLeftSide = IsLeftSide;
            #endregion
        }
    }
}
