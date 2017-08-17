using MahApps.Metro.Controls;
using RSFJ.Services;
using RSFJ.View;
using RSFJ.ViewModels.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RSFJ.ViewModels
{
    public class SettingsFlyoutViewModel : ViewModelBase
    {
        private bool _ShowAggregateFineBalance;
        public bool ShowAggregateFineBalance { get => _ShowAggregateFineBalance; set => SetProperty(ref _ShowAggregateFineBalance, value); }

        private bool _ShowAggregateMoneyBalance;
        public bool ShowAggregateMoneyBalance { get => _ShowAggregateMoneyBalance; set => SetProperty(ref _ShowAggregateMoneyBalance, value); }

        private bool _ShowAggregateStockBalance;
        public bool ShowAggregateStockBalance { get => _ShowAggregateStockBalance; set => SetProperty(ref _ShowAggregateStockBalance, value); }

        private int _RojmelPageDatesFilterSpan;
        public int RojmelPageDatesFilterSpan { get => _RojmelPageDatesFilterSpan; set => SetProperty(ref _RojmelPageDatesFilterSpan, value); }

        RelayCommand _startOverCommand;
        public ICommand StartOverCommand
        {
            get => _startOverCommand ?? (_startOverCommand = new RelayCommand(param => StartOver(), param => true));
        }

        private void StartOver()
        {
            var result1 = MessageBox.Show("Are you sure you want to start over? You will loose all your data and the app will start fresh.",
                "RSFJ", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

            if (result1 != MessageBoxResult.Yes) return;

            var result2 = MessageBox.Show("ARE YOU ABSOLUTELY SURE YOU WANT TO DO THIS!!? Consider taking a backup before you delete all your data!",
                "RSFJ", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result2 != MessageBoxResult.Yes) return;

            MessageBox.Show("The application will now restart", "RSFJ", MessageBoxButton.OK, MessageBoxImage.Information);

            DataContextService.Instance.StartOver();

            Process.Start(Application.ResourceAssembly.Location);
            Environment.Exit(0);
        }

        public SettingsFlyoutViewModel()
        {
            ShowAggregateFineBalance = Services.RegistoryService.Instance.ShowAggregateFineBalance;
            ShowAggregateMoneyBalance = Services.RegistoryService.Instance.ShowAggregateMoneyBalance;
            ShowAggregateStockBalance = Services.RegistoryService.Instance.ShowAggregateStockBalance;
            RojmelPageDatesFilterSpan = Services.RegistoryService.Instance.RojmelPageDatesFilterSpan;
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
            if (PropertyName == nameof(RojmelPageDatesFilterSpan))
            {
                Services.RegistoryService.Instance.RojmelPageDatesFilterSpan = RojmelPageDatesFilterSpan;
            }
        }
    }
}
