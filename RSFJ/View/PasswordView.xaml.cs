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
using System.Windows.Shapes;

namespace RSFJ.View
{
    /// <summary>
    /// Interaction logic for PasswordView.xaml
    /// </summary>
    public partial class PasswordView : Window
    {
        public PasswordView()
        {
            InitializeComponent();

            V_TextBlock.Text = Services.RegistoryService.Instance.MasterPassword == null ?
                "Set a master password to protect your data." :
                "You need to enter the password before you continue...";
        }

        private void SignIn_Click(object sender, RoutedEventArgs e)
        {
            var hash = Services.SecurityService.Instance.Hash(V_PasswordBox.Password);

            if (Services.RegistoryService.Instance.MasterPassword == null)
            {
                Services.RegistoryService.Instance.MasterPassword = hash;
                DialogResult = true;
            }
            else if (hash == Services.RegistoryService.Instance.MasterPassword)
            {
                DialogResult = true;
            }
            else
            {
                DialogResult = false;
            }

            Close();
        }
    }
}
