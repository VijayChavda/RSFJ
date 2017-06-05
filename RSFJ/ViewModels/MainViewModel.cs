using RSFJ.ViewModels.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RSFJ.ViewModels
{
    public class MainViewModel
    {
        public ObservableCollection<RojmelEntryViewModel> Entries { get; set; }

        RelayCommand _saveCommand;
        public ICommand SaveCommand
        {
            get => _saveCommand ?? (_saveCommand = new RelayCommand(param => this.Save(), param => true));
        }

        RelayCommand _loadCommand;
        public ICommand LoadCommand
        {
            get => _loadCommand ?? (_loadCommand = new RelayCommand(param => this.Load(), param => true));
        }

        RelayCommand _backupCommand;
        public ICommand BackupCommand
        {
            get => _backupCommand ?? (_backupCommand = new RelayCommand(param => this.Backup(), param => true));
        }

        RelayCommand _restoreCommand;
        public ICommand RestoreCommand
        {
            get => _restoreCommand ?? (_restoreCommand = new RelayCommand(param => this.Restore(param.ToString()), param => true));
        }

        public MainViewModel()
        {
            Entries = new ObservableCollection<RojmelEntryViewModel>()
            {
                new RojmelEntryViewModel(){ Id=1, Date = DateTime.Now.Date.AddDays(3), Account = "Person 1", StockItem = "Item 1" },
                new RojmelEntryViewModel(){ Id=2, Date = DateTime.Now.Date.AddDays(1), Account = "Person 2", StockItem = "Item 2" },
                new RojmelEntryViewModel(){ Id=3, Date = DateTime.Now.Date.AddDays(2), Account = "Person 3", StockItem = "Item 3" },
            };
        }

        private void Save()
        {

        }

        private void Load()
        {

        }

        private void Backup()
        {

        }

        private void Restore(string BackupFile)
        {

        }
    }

    public class RojmelEntryViewModel : INotifyPropertyChanged
    {
        public static int InstanceCount;

        public static ObservableCollection<string> AccountSuggestionsList { get; set; }

        public static ObservableCollection<string> StockItemSuggestionsList { get; set; }

        private int _Id;
        public int Id { get => _Id; set => SetProperty(ref _Id, value); }

        private DateTime _Date;
        public DateTime Date { get => _Date; set => SetProperty(ref _Date, value); }

        private string _Account;
        public string Account { get => _Account; set => SetProperty(ref _Account, value); }

        private string _StockItem;
        public string StockItem { get => _StockItem; set => SetProperty(ref _StockItem, value); }

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

        private double? _Param3;
        public double? Param3 { get => _Param3; set => SetProperty(ref _Param3, value); }

        private double? _Param4;
        public double? Param4 { get => _Param4; set => SetProperty(ref _Param4, value); }

        private string _LParam1Name;
        public string LParam1Name { get => _LParam1Name; set => SetProperty(ref _LParam1Name, value); }

        private string _LParam2Name;
        public string LParam2Name { get => _LParam2Name; set => SetProperty(ref _LParam2Name, value); }

        private string _RParam1Name;
        public string RParam1Name { get => _RParam1Name; set => SetProperty(ref _RParam1Name, value); }

        private string _RParam2Name;
        public string RParam2Name { get => _RParam2Name; set => SetProperty(ref _RParam2Name, value); }

        public RojmelEntryViewModel()
        {
            InstanceCount++;

            Id = InstanceCount;
            Date = DateTime.Now.Date;
            Account = AccountSuggestionsList.First();
            StockItem = StockItemSuggestionsList.First();
        }

        static RojmelEntryViewModel()
        {
            AccountSuggestionsList = new ObservableCollection<string>() { "SELF" };
            StockItemSuggestionsList = new ObservableCollection<string>() { "CASH" };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void SetProperty<T>(ref T Property, T Value, [System.Runtime.CompilerServices.CallerMemberName] string PropertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(Property, Value) == false)
            {
                Property = Value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));

                if (PropertyName == nameof(Account))
                {
                    if (string.IsNullOrWhiteSpace(Account) == false && AccountSuggestionsList.Contains(Account) == false)
                    {
                        AccountSuggestionsList.Add(Account);
                    }
                }

                if (PropertyName == nameof(StockItem))
                {
                    if (string.IsNullOrWhiteSpace(StockItem) == false && StockItemSuggestionsList.Contains(StockItem) == false)
                    {
                        StockItemSuggestionsList.Add(StockItem);
                    }
                }

                if (PropertyName == nameof(LParam1) || PropertyName == nameof(LParam2))
                {
                    if (LParam1 == null)
                    {
                        LParam2 = null;
                        LResult = null;
                    }

                    if (LParam1 != null)
                    {
                        LResult = LParam1;

                        if (LParam2 != null)
                        {
                            LResult = LParam1 + LParam2;
                        }

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

                    if (RParam1 != null)
                    {
                        RResult = RParam1;

                        if (RParam2 != null)
                        {
                            RResult = RParam1 + RParam2;
                        }

                        LParam1 = null;
                    }
                }
            }
        }
    }
}
