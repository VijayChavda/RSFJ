using RSFJ.Model;
using RSFJ.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSFJ.ViewModels
{
    public class AccountPageViewModel : ViewModelBase
    {
        public ObservableCollection<GroupedAccounts> GroupedAccounts { get; set; }
        public ObservableCollection<HistoryItemViewModel> HistoryEntries { get; set; }

        private Account _SelectedAccount;
        public Account SelectedAccount { get => _SelectedAccount; set => SetProperty(ref _SelectedAccount, value); }

        private HistoryItemViewModel _SelectedEntry;
        public HistoryItemViewModel SelectedEntry { get => _SelectedEntry; set => SetProperty(ref _SelectedEntry, value); }

        public AccountPageViewModel()
        {
            GroupedAccounts = new ObservableCollection<GroupedAccounts>();
            HistoryEntries = new ObservableCollection<HistoryItemViewModel>();

            LoadData();
        }

        private void LoadData()
        {
            foreach (var entry in DataContextService.Instance.DataContext.Accounts.GroupBy(x => x.Group))
            {
                GroupedAccounts.Add(new GroupedAccounts()
                {
                    Group = entry.Key,
                    Accounts = new ObservableCollection<Account>(entry)
                });

                SelectedAccount = GroupedAccounts.FirstOrDefault()?.Accounts?.FirstOrDefault();
            }
        }

        protected override void APropertyChanged<T>(string PropertyName, T OldValue, T NewValue)
        {
            if (PropertyName == nameof(SelectedAccount))
            {
                HistoryEntries.Clear();

                var accountEntries = DataContextService.Instance.DataContext.RojmelEntries.Where(x => x.Account == SelectedAccount);

                var lendingEntries = accountEntries.Where(x => !x.IsLeftSide && x.Type == RojmelEntryType.ItemExchangeFine).ToArray();
                var paybackEntries = accountEntries.Where(x => x.IsLeftSide && x.Type == RojmelEntryType.ItemExchangeFine || x.Type == RojmelEntryType.UseCash).ToArray();
                AnalyzeHistory(lendingEntries, paybackEntries);

                lendingEntries = accountEntries.Where(x => x.IsLeftSide && x.Type == RojmelEntryType.ItemExchangeCash).ToArray();
                paybackEntries = accountEntries.Where(x => !x.IsLeftSide && x.Type == RojmelEntryType.SimpleCashExchange).ToArray();
                AnalyzeHistory(lendingEntries, paybackEntries);
            }
        }

        private void AnalyzeHistory(RojmelEntry[] lendingEntries, RojmelEntry[] paybackEntries)
        {
            int j = -1;
            double? paybackStillRemaining = null;
            for (int i = 0; i < lendingEntries.Length; i++)
            {
                var lendingEntryModel = lendingEntries[i];
                var lendingEntry = new HistoryItemViewModel(lendingEntryModel);
                double lendingRemaining = lendingEntryModel.Result;

                for (; j + 1 < paybackEntries.Length;)
                {
                    j++;

                    var paybackEntryModel = paybackEntries[j];
                    var paybackEntry = new HistoryItemViewModel(paybackEntryModel);
                    double paybackRemaining = paybackStillRemaining ?? paybackEntryModel.Result;

                    #region Calculate delay in partial and full payments
                    var daysPassedSinceBought = (paybackEntry.Date - lendingEntry.Date).Days;
                    if (j == 0)
                    {
                        paybackEntry.PartialPaymentLate = (daysPassedSinceBought > lendingEntry.PartialPaymentInterval ? daysPassedSinceBought - lendingEntry.PartialPaymentInterval : (int?)null);
                    }
                    else
                    {
                        var daysPassedSinceLastPayback = (paybackEntry.Date - paybackEntries[j - 1].Date).Days;
                        paybackEntry.PartialPaymentLate = (daysPassedSinceLastPayback > lendingEntry.PartialPaymentInterval ? daysPassedSinceLastPayback - lendingEntry.PartialPaymentInterval : (int?)null);
                    }
                    paybackEntry.FullPaymentLate = daysPassedSinceBought > lendingEntry.TotalPaymentDueDays ? daysPassedSinceBought - lendingEntry.TotalPaymentDueDays : (int?)null;
                    #endregion

                    lendingEntry.PaybackEntries.Add(paybackEntry);

                    if (lendingRemaining > paybackRemaining)
                    {
                        lendingRemaining -= paybackRemaining;
                        paybackStillRemaining = null;
                        continue;
                    }
                    else if (lendingRemaining < paybackRemaining)
                    {
                        paybackStillRemaining = paybackStillRemaining ?? paybackRemaining;    //If it is null, make it current payback value.
                        paybackStillRemaining -= lendingRemaining;
                        break;
                    }
                    else break;
                }

                HistoryEntries.Add(lendingEntry);
            }
        }
    }

    public class GroupedAccounts
    {
        public string Group { get; set; }

        public ObservableCollection<Account> Accounts { get; set; }

        public GroupedAccounts()
        {
            Accounts = new ObservableCollection<Account>();
        }

        public override string ToString()
        {
            return Group;
        }
    }

    public class HistoryItemViewModel
    {
        public RojmelEntryType Type { get; set; }

        public string Id { get; set; }

        public DateTime Date { get; set; }

        public string StockItem { get; set; }

        public int? PartialPaymentLate { get; set; }

        public int? FullPaymentLate { get; set; }

        public int TotalPaymentDueDays { get; set; }

        public int PartialPaymentInterval { get; set; }

        public string LParam1 { get; set; }

        public string LParam2 { get; set; }

        public string LResult { get; set; }

        public string RParam1 { get; set; }

        public string RParam2 { get; set; }

        public string RResult { get; set; }

        public string AccountMoneyBalance { get; set; }

        public string AccountFineBalance { get; set; }

        public ObservableCollection<HistoryItemViewModel> PaybackEntries { get; set; }

        public HistoryItemViewModel(RojmelEntry Model)
        {
            Type = Model.Type;
            Id = Model.Id.ToString();
            Date = Model.Date;
            StockItem = Model.StockItem.ToString();
            PartialPaymentInterval = Model.PartialPaymentInterval;
            TotalPaymentDueDays = Model.FullPaymentDueDays;
            LParam1 = Model.IsLeftSide ? Model.Param1.ToString() : string.Empty;
            LParam2 = Model.IsLeftSide ? Model.Param2.ToString() : string.Empty;
            LResult = Model.IsLeftSide ? Model.Result.ToString() : string.Empty;
            RParam1 = !Model.IsLeftSide ? Model.Param1.ToString() : string.Empty;
            RParam2 = !Model.IsLeftSide ? Model.Param2.ToString() : string.Empty;
            RResult = !Model.IsLeftSide ? Model.Result.ToString() : string.Empty;

            PaybackEntries = new ObservableCollection<HistoryItemViewModel>();
        }
    }
}
