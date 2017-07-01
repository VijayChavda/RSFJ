﻿using MahApps.Metro.Controls;
using RSFJ.View;
using RSFJ.ViewModels.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RSFJ.ViewModels
{
    public class SettingsFlyoutViewModel : ViewModelBase
    {
        private bool _IsShowingNewStockItemFlyout;
        public bool IsShowingNewStockItemFlyout { get => _IsShowingNewStockItemFlyout; set => SetProperty(ref _IsShowingNewStockItemFlyout, value); }

        private bool _IsShowingNewAccountFlyout;
        public bool IsShowingNewAccountFlyout { get => _IsShowingNewAccountFlyout; set => SetProperty(ref _IsShowingNewAccountFlyout, value); }

        private bool _ShowAggregateFineBalance;
        public bool ShowAggregateFineBalance { get => _ShowAggregateFineBalance; set => SetProperty(ref _ShowAggregateFineBalance, value); }

        private bool _ShowAggregateMoneyBalance;
        public bool ShowAggregateMoneyBalance { get => _ShowAggregateMoneyBalance; set => SetProperty(ref _ShowAggregateMoneyBalance, value); }

        private bool _ShowAggregateStockBalance;
        public bool ShowAggregateStockBalance { get => _ShowAggregateStockBalance; set => SetProperty(ref _ShowAggregateStockBalance, value); }

        RelayCommand _newStockItem;
        public ICommand NewStockItemCommand
        {
            get => _newStockItem ?? (_newStockItem = new RelayCommand(param => IsShowingNewStockItemFlyout = true, param => true));
        }

        RelayCommand _newAccount;
        public ICommand NewAccountCommand
        {
            get => _newAccount ?? (_newAccount = new RelayCommand(param => IsShowingNewAccountFlyout = true, param => true));
        }

        public SettingsFlyoutViewModel()
        {
            ShowAggregateFineBalance = Services.RegistoryService.Instance.ShowAggregateFineBalance;
            ShowAggregateMoneyBalance = Services.RegistoryService.Instance.ShowAggregateMoneyBalance;
            ShowAggregateStockBalance = Services.RegistoryService.Instance.ShowAggregateStockBalance;
        }

        protected override void APropertyChanged<T>(string PropertyName, T OldValue, T NewValue)
        {
            if (PropertyName == nameof(ShowAggregateFineBalance))
            {
                Services.RegistoryService.Instance.ShowAggregateFineBalance = ShowAggregateFineBalance;
            }
            if (PropertyName == nameof(ShowAggregateMoneyBalance))
            {
                Services.RegistoryService.Instance.ShowAggregateMoneyBalance = ShowAggregateMoneyBalance;
            }
            if (PropertyName == nameof(ShowAggregateStockBalance))
            {
                Services.RegistoryService.Instance.ShowAggregateStockBalance = ShowAggregateStockBalance;
            }
        }
    }
}
