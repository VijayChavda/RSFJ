using RSFJ.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace RSFJ.ViewModels
{
    public class RojmelPageViewModel : ViewModelBase
    {
        public ObservableCollection<RojmelEntryViewModel> Entries { get; set; }

        public RojmelPageViewModel()
        {
            Entries = new ObservableCollection<RojmelEntryViewModel>();

            LoadData();
        }

        private void LoadData()
        {
            foreach (var model in DataContextService.Instance.DataContext.RojmelEntries)
            {
                Entries.Add(new RojmelEntryViewModel(model));
            }
        }
    }

    public class RojmelEntryViewModel : ViewModelBase
    {
        public static int InstanceCount;

        public Model.RojmelEntry Model { get; set; }

        #region Common parameters
        private int _Id;
        public int Id { get => _Id; set => SetProperty(ref _Id, value); }

        private DateTime _Date;
        public DateTime Date { get => _Date; set => SetProperty(ref _Date, value); }

        private string _Account;
        public string Account { get => _Account; set => SetProperty(ref _Account, value); }

        private string _Type;
        public string Type { get => _Type; set => SetProperty(ref _Type, value); }

        private string _StockItem;
        public string StockItem { get => _StockItem; set => SetProperty(ref _StockItem, value); }
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

        private double? _Loss;
        public double? Loss { get => _Loss; set => SetProperty(ref _Loss, value); }

        private DateTime _InstallmentPaymentDue;
        public DateTime InstallmentPaymentDue { get => _InstallmentPaymentDue; set => SetProperty(ref _InstallmentPaymentDue, value); }

        private DateTime _PaymentDue;
        public DateTime PaymentDue { get => _PaymentDue; set => SetProperty(ref _PaymentDue, value); }
        #endregion

        public RojmelEntryViewModel()
        {
            InstanceCount++;
            Model = new Model.RojmelEntry();

            Id = InstanceCount;
            Date = DateTime.Now.Date;
        }

        public RojmelEntryViewModel(Model.RojmelEntry Model)
        {
            InstanceCount++;
            this.Model = Model;

            //TODO: Redefine
        }

        protected override void APropertyChanged<T>(string PropertyName, T OldValue, T NewValue)
        {
            //TODO: Redefine
        }
    }
}
