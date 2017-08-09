using RSFJ.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace RSFJ
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            DispatcherUnhandledException += App_DispatcherUnhandledException;

            Exit += App_Exit;
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            var result = MessageBox.Show("Do you wish to create a backup your Rojmel?", "RSFJ", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);

            if (result == MessageBoxResult.Yes)
            {
                DataContextService.Instance.Backup();
            }
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            ReportingService.Instance.CreateReport("TODO: User's message will be placed here", e.Exception);

            Environment.Exit(0);
        }
    }
}
