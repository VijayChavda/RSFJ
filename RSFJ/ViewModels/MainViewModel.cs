using RSFJ.Services;
using RSFJ.ViewModels.Utilities;
using System.Windows.Input;

namespace RSFJ.ViewModels
{
    public class MainViewModel
    {
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
    }
}
