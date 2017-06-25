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
    public partial class MainWindow
    {
        public MainWindow()
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

            DataContextService.Instance.Load();
            InitializeComponent();
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
    }
}
