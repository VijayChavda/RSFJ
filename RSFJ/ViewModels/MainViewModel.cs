using RSFJ.Services;
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
        RelayCommand _saveCommand;
        public ICommand SaveCommand
        {
            get => _saveCommand ?? (_saveCommand = new RelayCommand(param => this.Save(), param => true));
        }

        RelayCommand _backupCommand;
        public ICommand BackupCommand
        {
            get => _backupCommand ?? (_backupCommand = new RelayCommand(param => this.Backup(), param => true));
        }

        RelayCommand _restoreCommand;
        public ICommand RestoreCommand
        {
            get => _restoreCommand ?? (_restoreCommand = new RelayCommand(param => this.Restore(param as string), param => true));
        }

        public MainViewModel()
        {
        }

        private void Save()
        {
            DataContextService.Instance.Save();
        }

        private void Backup()
        {

        }

        private void Restore(string BackupFile)
        {

        }
    }
}
