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
            var allowSkip = Properties.Settings.Default.FailCount <= 50;

            var result = new View.Verification(allowSkip).ShowDialog();
            if (result != true)
            {
                Properties.Settings.Default.FailCount++;
                Properties.Settings.Default.Save();
            }

            if (result == true)
            {
                Properties.Settings.Default.FailCount = 0;
                Properties.Settings.Default.Save();
            }

            InitializeComponent();

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.LoadCommand.Execute(null);
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

        private void OpenWebsite(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
