using RSFJ.Services;
using RSFJ.ViewModels.Utilities;
using System.Windows.Controls;
using System.Windows.Input;

namespace RSFJ.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly View.RojmelPage RojmelPageView = new View.RojmelPage();
        private readonly View.AccountPage AccountPageView = new View.AccountPage();

        private Page _currentPage;
        public Page CurrentPage { get => _currentPage; set => SetProperty(ref _currentPage, value); }

        RelayCommand _saveCommand;
        public ICommand SaveCommand
        {
            get => _saveCommand ?? (_saveCommand = new RelayCommand(param => DataContextService.Instance.Save(), param => true));
        }

        RelayCommand _loadCommand;
        public ICommand LoadCommand
        {
            get => _loadCommand ?? (_loadCommand = new RelayCommand(param => DataContextService.Instance.Load(), param => true));
        }

        RelayCommand _backupCommand;
        public ICommand BackupCommand
        {
            get => _backupCommand ?? (_backupCommand = new RelayCommand(param => DataContextService.Instance.Backup(), param => true));
        }

        RelayCommand _restoreCommand;
        public ICommand RestoreCommand
        {
            get => _restoreCommand ?? (_restoreCommand = new RelayCommand(param => DataContextService.Instance.Restore(param as string), param => true));
        }

        RelayCommand _navigateCommand;
        public ICommand NavigateCommand
        {
            get => _navigateCommand ?? (_navigateCommand = new RelayCommand(param =>
            {
                var page = param as string;

                if (page == nameof(View.RojmelPage))
                {
                    CurrentPage = RojmelPageView;
                }
                if (page == nameof(View.AccountPage))
                {
                    CurrentPage = AccountPageView;
                }
            }, param => true));
        }

        public MainViewModel()
        {
            CurrentPage = RojmelPageView;
        }
    }
}
