using MahApps.Metro.Controls;
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

        private bool _ShowAggregateColumns;
        public bool ShowAggregateColumns { get => _ShowAggregateColumns; set => SetProperty(ref _ShowAggregateColumns, value); }

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
    }
}
