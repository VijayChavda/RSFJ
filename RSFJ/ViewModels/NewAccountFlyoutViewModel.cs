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

        private AccountType _Type;
        public AccountType Type { get => _Type; set => SetProperty(ref _Type, value); }

        private string _Message;
        public string Message { get => _Message; set => SetProperty(ref _Message, value); }

        public List<AccountType> AccountTypes { get; set; }

        RelayCommand _addCommand;
        public ICommand AddCommand
        {
            get => _addCommand ?? (_addCommand = new RelayCommand(param => Add(), param => true));
        }

        public NewAccountFlyoutViewModel()
        {
            Group = "Others";
            Type = AccountType.Regular;
            AccountTypes = new List<AccountType>() { AccountType.Boolean, AccountType.Customer, AccountType.Regular };
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
                Group = Group,
                Note = Note,
                Type = Type
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
                    Type = AccountType.Regular;
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

            if (PropertyName != nameof(Message) && string.IsNullOrWhiteSpace(Name) == false && Phone != null && Phone.Count(c => !char.IsDigit(c)) == 0 && Phone.Length <= 10)
            {
                Message = null;
            }
        }
    }
}
