using MahApps.Metro.Controls.Dialogs;
using RSFJ.Services;
using RSFJ.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RSFJ
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MahApps.Metro.Controls.MetroWindow
    {
        public MainWindow()
        {
            bool bypass = true;
#if DEBUG
            bypass = false;
#endif
            if (bypass)
            {
                VerifySoftware();

                VerifyPasswordAsync();
            }

            DataContextService.Instance.Load();
            InitializeComponent();

            if (DataContextService.Instance.DataContext.Accounts.Count == 0)
            {
                addAccount.IsOpen = true;
            }
        }

        private void VerifySoftware()
        {
            var allowSkip = RegistoryService.Instance.FailureCount <= 50;

            var result = new View.Verification(allowSkip).ShowDialog();
            if (result != true)
            {
                RegistoryService.Instance.FailureCount++;
            }

            if (result == true)
            {
                RegistoryService.Instance.FailureCount = 0;
            }
        }

        private void VerifyPasswordAsync()
        {
            var result = new View.PasswordView().ShowDialog();
            if (result != true)
            {
                MessageBox.Show("Sorry, the password you entered was incorrect. " +
                    "The application will now exit.", "Incorrect password.", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }

        private void MetroWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
            {
                ViewModel.SaveCommand.Execute(null);
            }

            if (e.Key == Key.B && Keyboard.Modifiers == ModifierKeys.Control)
            {
                ViewModel.BackupCommand.Execute(null);
            }

            if (e.Key == Key.R && Keyboard.Modifiers == ModifierKeys.Control)
            {
                ViewModel.RestoreCommand.Execute(null);
            }
        }

        private void AFlyout_IsOpenChanged(object sender, RoutedEventArgs e)
        {
            if (addstockItem.IsOpen || addAccount.IsOpen || aboutFlyout.IsOpen || settingsFlyout.IsOpen)
            {
                MainFrame.IsHitTestVisible = false;
                MainFrame.Opacity = 0.3;
            }
            else
            {
                MainFrame.IsHitTestVisible = true;
                MainFrame.Opacity = 1;
            }
        }
    }
}
