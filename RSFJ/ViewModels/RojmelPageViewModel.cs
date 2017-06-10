using RSFJ.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSFJ.ViewModels
{
    public class RojmelPageViewModel
    {
        public ObservableCollection<RojmelEntry> Entries { get; set; }

        public RojmelPageViewModel()
        {
            Entries = new ObservableCollection<RojmelEntry>();

            Entries.CollectionChanged += (sender, args) =>
            {
                if (args.NewItems != null)
                {
                    foreach (var item in args.NewItems.Cast<RojmelEntry>())
                    {
                        DataContextService.Instance.DataContext.RojmelEntries.Add(item.Model);
                    }
                }
            };

            foreach (var model in DataContextService.Instance.DataContext.RojmelEntries)
            {
                Entries.Add(new RojmelEntry(model));
            }
        }
    }

    public class RojmelEntry : INotifyPropertyChanged
    {
        public static int InstanceCount;

        public Model.RojmelEntry Model { get; set; }

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

        private bool _UplakClear;
        public bool UplakClear { get => _UplakClear; set => SetProperty(ref _UplakClear, value); }

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

        private const string Customer = "Customer";
        private const string Cash = "Cash";
        private const string Fine999 = "Fine999";

        public RojmelEntry()
        {
            Model = new Model.RojmelEntry();

            InstanceCount++;

            Id = InstanceCount;
            Date = DateTime.Now.Date;
            Account = AccountSuggestionsList.First();
            StockItem = StockItemSuggestionsList.First();

        }

        public RojmelEntry(Model.RojmelEntry Model)
        {
            InstanceCount++;

            this.Model = Model;

            _Id = Model.Id;
            _Date = Model.Date;
            _Account = Model.Account;
            _StockItem = Model.StockItem;
            _UplakClear = Model.UplakClear;
            if (Model.IsLeftSide)
            {
                _LParam1 = Model.Param1;
                _LParam2 = Model.Param2;
                _LResult = Model.Result;
            }
            else
            {
                _RParam1 = Model.Param1;
                _RParam2 = Model.Param2;
                _RResult = Model.Result;
            }

            #region Add items to Account & StockItem suggestion lists.
            if (AccountSuggestionsList.Contains(Account) == false)
            {
                AccountSuggestionsList.Add(Account);
            }

            if (StockItemSuggestionsList.Contains(StockItem) == false)
            {
                StockItemSuggestionsList.Add(StockItem);
            }
            #endregion
        }

        static RojmelEntry()
        {
            AccountSuggestionsList = new ObservableCollection<string>() { Customer };
            StockItemSuggestionsList = new ObservableCollection<string>() { Cash, Fine999 };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void SetProperty<T>(ref T Property, T Value, [System.Runtime.CompilerServices.CallerMemberName] string PropertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(Property, Value) == false)
            {
                Property = Value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));

                //Account and StockItem should be defined.
                if (string.IsNullOrWhiteSpace(Account) || string.IsNullOrWhiteSpace(StockItem)) return;

                #region Auto suggestion
                if (PropertyName == nameof(Account))
                {
                    if (AccountSuggestionsList.Contains(Account) == false)
                    {
                        AccountSuggestionsList.Add(Account);
                    }
                }

                if (PropertyName == nameof(StockItem))
                {
                    if (StockItemSuggestionsList.Contains(StockItem) == false)
                    {
                        StockItemSuggestionsList.Add(StockItem);
                    }
                }
                #endregion

                #region Calculation
                if (PropertyName == nameof(LParam1) || PropertyName == nameof(LParam2)
                            || PropertyName == nameof(Account) || PropertyName == nameof(StockItem)
                            || PropertyName == nameof(UplakClear))
                {
                    if (LParam1 == null)
                    {
                        LParam2 = null;
                        LResult = null;
                    }
                    else if (LParam1 != null)
                    {
                        LResult = LParam1;

                        if (LParam2 != null)
                        {
                            if (UplakClear)
                            {
                                LResult = LParam1 / LParam2;
                            }
                            else if (StockItem == Fine999)
                            {
                                LResult = LParam1 * LParam2;
                            }
                            else if (StockItem == Cash)
                            {
                                LResult = LParam1 / LParam2;
                            }
                            else
                            {
                                LResult = LParam1 * LParam2 / 100;
                            }
                        }

                        RParam1 = null;
                    }
                }

                if (PropertyName == nameof(RParam1) || PropertyName == nameof(RParam2)
                    || PropertyName == nameof(Account) || PropertyName == nameof(StockItem))
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
                            if (Account == Customer)
                            {
                                RResult = RParam1 * RParam2;
                            }
                            else if (StockItem == Cash)
                            {
                                RResult = RParam1 / RParam2;
                            }
                            else
                            {
                                RResult = RParam1 * RParam2 / 100;
                            }
                        }

                        LParam1 = null;
                    }
                }
                #endregion

                #region Model update
                Model.Id = Id;
                Model.Date = Date;
                Model.Account = Account;
                Model.StockItem = StockItem;
                Model.UplakClear = UplakClear;
                Model.Param1 = LParam1 ?? RParam1 ?? 0;
                Model.Param2 = LParam2 ?? RParam2 ?? 0;
                Model.Result = LResult ?? RResult ?? 0;
                Model.IsLeftSide = LResult != null && RResult == null;
                #endregion
            }
        }
    }
}
