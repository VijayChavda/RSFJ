using System;
using System.Collections.Generic;
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

namespace RSFJ.View
{
    /// <summary>
    /// Interaction logic for AccountPage.xaml
    /// </summary>
    public partial class AccountPage : Page
    {
        public ViewModels.AccountPageViewModel ViewModel { get; set; }

        public AccountPage()
        {
            InitializeComponent();

            Loaded += AccountPage_Loaded;
        }

        private void AccountPage_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = ViewModel = new ViewModels.AccountPageViewModel();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ViewModel.SelectedAccount = e.NewValue as Model.Account;
        }
    }
}
