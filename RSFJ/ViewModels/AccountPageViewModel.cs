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
                var rightEntries = accountEntries.Where(x => !x.IsLeftSide && x.Type == RojmelEntryType.ItemExchangeFine).ToArray();
                var leftEntries = accountEntries.Where(x => x.IsLeftSide && x.Type == RojmelEntryType.ItemExchangeFine || x.Type == RojmelEntryType.UseCash).ToArray();

                int jStart = 0;
                double? leftStillRemaining = null;
                for (int i = 0; i < rightEntries.Length; i++)
                {
                    var rightEntryModel = rightEntries[i];
                    var rightEntry = new HistoryItemViewModel(rightEntryModel);
                    double rightRemaining = rightEntryModel.Result;

                    for (int j = jStart; j < leftEntries.Length; j++)
                    {
                        var leftEntryModel = leftEntries[j];
                        var leftEntry = new HistoryItemViewModel(leftEntryModel);
                        double leftRemaining = leftStillRemaining ?? leftEntryModel.Result;

                        rightEntry.PaybackEntries.Add(leftEntry);

                        if (rightRemaining > leftRemaining)
                        {
                            rightRemaining -= leftRemaining;
                            leftStillRemaining = null;
                            continue;
                        }
                        else if (rightRemaining < leftRemaining)
                        {
                            leftStillRemaining = leftStillRemaining ?? leftRemaining;    //If it is null, make it current left value.
                            leftStillRemaining -= rightRemaining;
                            jStart = j;
                            break;
                        }
                        else break;
                    }

                    HistoryEntries.Add(rightEntry);
                }
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

    public class HistoryItemViewModel : ViewModelBase
    {
        public string Id { get; set; }

        public string Date { get; set; }

        public string StockItem { get; set; }

        public string PaymentAfter { get; set; }

        public string PaymentLate { get; set; }

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
            Id = Model.Id.ToString();
            Date = Model.Date.ToShortDateString();
            StockItem = Model.StockItem.ToString();
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
