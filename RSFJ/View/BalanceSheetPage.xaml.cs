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
    /// Interaction logic for BalanceSheetPage.xaml
    /// </summary>
    public partial class BalanceSheetPage : Page
    {
        public bool IsCurrentRateValid
        {
            get { return (bool)GetValue(IsCurrentRateValidProperty); }
            set { SetValue(IsCurrentRateValidProperty, value); }
        }

        public static readonly DependencyProperty IsCurrentRateValidProperty =
            DependencyProperty.Register("IsCurrentRateValid", typeof(bool), typeof(BalanceSheetPage), new PropertyMetadata(false));

        public BalanceSheetPage()
        {
            InitializeComponent();

            Loaded += BalanceSheetPage_Loaded;
        }

        private void BalanceSheetPage_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.Load();
        }

        private void ChangeRate_Click(object sender, RoutedEventArgs e)
        {
            var rate = double.Parse(V_Rate.Text);
            ViewModel.CurrentEditingStockItem.Rate = IsCurrentRateValid ? Math.Round(rate, 2) : 0;
        }

        private void V_Rate_TextChanged(object sender, TextChangedEventArgs e)
        {
            IsCurrentRateValid = double.TryParse(V_Rate.Text, out double temp);
        }
    }
}
