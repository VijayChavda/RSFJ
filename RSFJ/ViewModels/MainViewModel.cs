using RSFJ.Services;
using RSFJ.ViewModels.Utilities;
using System.Windows.Input;

namespace RSFJ.ViewModels
{
    /// <summary>
    /// MainWindow's ViewModel.
    /// </summary>
    public class MainViewModel
    {
        #region Commands
        /// <summary>
        /// Command to Save data.
        /// </summary>
        public ICommand SaveCommand
        {
            get => _saveCommand ?? (_saveCommand = new RelayCommand(param => DataContextService.Instance.Save(), param => true));
        }
        private RelayCommand _saveCommand;

        /// <summary>
        /// Command to Backup data.
        /// </summary>
        public ICommand BackupCommand
        {
            get => _backupCommand ?? (_backupCommand = new RelayCommand(param => DataContextService.Instance.Backup(), param => true));
        }
        private RelayCommand _backupCommand;

        /// <summary>
        /// Command to Restore previously backed-up data.
        /// </summary>
        public ICommand RestoreCommand
        {
            get => _restoreCommand ?? (_restoreCommand = new RelayCommand(param => DataContextService.Instance.Restore(param as string), param => true));
        }
        RelayCommand _restoreCommand;
        #endregion
    }
}
