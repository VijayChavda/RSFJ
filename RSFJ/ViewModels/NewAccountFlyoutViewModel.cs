using RSFJ.ViewModels.Utilities;
using System.Windows.Input;
using System.Linq;
using System.Timers;
using RSFJ.Model;
using System.Collections.Generic;

namespace RSFJ.ViewModels
{
    public class NewAccountFlyoutViewModel : ViewModelBase
    {
        private string _Name;
        public string Name { get => _Name; set => SetProperty(ref _Name, value); }

        private string _Phone;
        public string Phone { get => _Phone; set => SetProperty(ref _Phone, value); }

        private string _Group;
        public string Group { get => _Group; set => SetProperty(ref _Group, value); }

        private string _Note;
        public string Note { get => _Note; set => SetProperty(ref _Note, value); }

        private string _PreferredTransactionType;
        public string PreferredTransactionType { get => _PreferredTransactionType; set => SetProperty(ref _PreferredTransactionType, value); }

        public List<string> PreferredTransactionTypes { get; set; }

        private string _Message;
        public string Message { get => _Message; set => SetProperty(ref _Message, value); }

        RelayCommand _addCommand;
        public ICommand AddCommand
        {
            get => _addCommand ?? (_addCommand = new RelayCommand(param => Add(), param => true));
        }

        public NewAccountFlyoutViewModel()
        {
            var types = Services.DataContextService.Instance.DataContext.RojmelEntryTypes.ToList();
            PreferredTransactionTypes = new List<string>() { types[0], types[1], types[2] };

            Group = "Others";
            PreferredTransactionType = PreferredTransactionTypes[0];
        }

        private void Add()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                Message = "Please provide a name to this item.";
                return;
            }

            if (Phone != null)
            {
                if (Phone.Count(c => !char.IsDigit(c)) > 0)
                {
                    Message = "Phone number must contain only digits.";
                    return;
                }
                if (Phone.Length > 10)
                {
                    Message = "Phone number cannot be more than 10 digits.";
                    return;
                }
            }

            var newItem = new Account()
            {
                Name = Name,
                Phone = Phone,
                FineInGold = 0,
                FineInMoney = 0,
                Group = Group,
                Note = Note,
                PreferredTransactionType = PreferredTransactionType
            };

            var added = Services.DataContextService.Instance.DataContext.Accounts.Add(newItem);
            if (added)
            {
                Message = "A new Account was added successfully...";
                Services.DataContextService.Instance.DataContext.FireAccountAdded(newItem);

                var timer = new Timer(800) { AutoReset = false };
                timer.Elapsed += (sender, e) =>
                {
                    Name = null;
                    Phone = null;
                    Group = null;
                    Note = null;
                    PreferredTransactionType = null;
                    Message = null;
                };
                timer.Start();
            }
            else
            {
                Message = "This account already exists. Please provide unique names for accounts.";
            }
        }

        protected override void APropertyChanged<T>(string PropertyName, T OldValue, T NewValue)
        {
            if (PropertyName == nameof(Group))
            {
                Group = string.IsNullOrWhiteSpace(Group) ? "Others" : Group;
            }

            if (PropertyName == nameof(PreferredTransactionType))
            {
                PreferredTransactionType = string.IsNullOrWhiteSpace(PreferredTransactionType) ? PreferredTransactionTypes[0] : PreferredTransactionType;
            }

            if (PropertyName != nameof(Message) && string.IsNullOrWhiteSpace(Name) == false && Phone != null && Phone.Count(c => !char.IsDigit(c)) == 0 && Phone.Length <= 10)
            {
                Message = null;
            }
        }
    }
}
