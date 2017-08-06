using Microsoft.Win32;
using RSFJ.Services;
using RSFJ.ViewModels.Utilities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RSFJ.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private bool _noAccounts;
        public bool AccountsPresent { get => _noAccounts; set => SetProperty(ref _noAccounts, value); }

        private Page _currentPage;
        public Page CurrentPage { get => _currentPage; set => SetProperty(ref _currentPage, value); }

        private readonly View.EntriesPage EntriesPage = new View.EntriesPage();
        private readonly View.RojmelPage RojmelPage = new View.RojmelPage();
        private readonly View.AccountPage AccountPage = new View.AccountPage();
        private readonly View.StockPage StockPage = new View.StockPage();

        RelayCommand _saveCommand;
        public ICommand SaveCommand
        {
            get => _saveCommand ?? (_saveCommand = new RelayCommand(param => Save(), param => true));
        }

        RelayCommand _loadCommand;
        public ICommand LoadCommand
        {
            get => _loadCommand ?? (_loadCommand = new RelayCommand(param => DataContextService.Instance.Load(), param => true));
        }

        RelayCommand _backupCommand;
        public ICommand BackupCommand
        {
            get => _backupCommand ?? (_backupCommand = new RelayCommand(param => Backup(), param => true));
        }

        RelayCommand _restoreCommand;
        public ICommand RestoreCommand
        {
            get => _restoreCommand ?? (_restoreCommand = new RelayCommand(param => Restore(), param => true));
        }

        RelayCommand _navigateCommand;
        public ICommand NavigateCommand
        {
            get => _navigateCommand ?? (_navigateCommand = new RelayCommand(param =>
            {
                AccountsPresent = DataContextService.Instance.DataContext.Accounts.Count != 0;

                if (AccountsPresent == false)
                {
                    MessageBox.Show("No accounts found. You need to add Accounts first..", "RSFJ", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var page = param as string;

                if (page == nameof(View.RojmelPage))
                {
                    CurrentPage = new View.RojmelPage();
                }
                if (page == nameof(View.AccountPage))
                {
                    CurrentPage = new View.AccountPage();
                }
                if (page == nameof(View.EntriesPage))
                {
                    CurrentPage = new View.EntriesPage();
                }
                if (page == nameof(View.StockPage))
                {
                    CurrentPage = new View.StockPage();
                }
            }, param => true));
        }

        public MainViewModel()
        {
            AccountsPresent = DataContextService.Instance.DataContext.Accounts.Count != 0;
            CurrentPage = AccountsPresent ? EntriesPage : null;
        }

        private void Save()
        {
            DataContextService.Instance.Save();
            MessageBox.Show("Saved.", "RSFJ", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Backup()
        {
            DataContextService.Instance.Backup();
            MessageBox.Show("Backup was completed.", "RSFJ", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Restore()
        {
            OpenFileDialog dlg = new OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".png";
            dlg.Filter = "RSFJ Backup File (*.rsfj)|*.rsfj";


            // Display OpenFileDialog by calling ShowDialog method 
            bool? result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result.HasValue && result.Value)
            {
                // Open document 
                DataContextService.Instance.Restore(dlg.FileName);
            }

            MessageBox.Show("The application was restored to the selected backup.", "RSFJ", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
