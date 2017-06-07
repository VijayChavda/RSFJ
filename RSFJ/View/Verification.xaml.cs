using RSFJ.Services;
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
using System.Windows.Threading;

namespace RSFJ.View
{
    /// <summary>
    /// Interaction logic for Verification.xaml
    /// </summary>
    public partial class Verification : Window
    {
        public bool IsVerifying
        {
            get { return (bool)GetValue(IsVerifyingProperty); }
            set { SetValue(IsVerifyingProperty, value); }
        }

        public static readonly DependencyProperty IsVerifyingProperty =
            DependencyProperty.Register("IsVerifying", typeof(bool), typeof(Verification), new PropertyMetadata(true));

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(Verification), new PropertyMetadata(string.Empty));

        public SolidColorBrush MessageBrush
        {
            get { return (SolidColorBrush)GetValue(MessageBrushProperty); }
            set { SetValue(MessageBrushProperty, value); }
        }

        public static readonly DependencyProperty MessageBrushProperty =
            DependencyProperty.Register("MessageBrush", typeof(SolidColorBrush), typeof(Verification), new PropertyMetadata(InfoMessageBrush));

        private static readonly SolidColorBrush ErrorMessageBrush = new SolidColorBrush(Colors.Yellow);
        private static readonly SolidColorBrush InfoMessageBrush = new SolidColorBrush(Colors.White);

        public Verification()
        {
            InitializeComponent();

            Loaded += async (s, e) =>
            {
                DataContext = this;
                await VerifyApplicationAsync();
            };
        }

        private async Task VerifyApplicationAsync()
        {
            Message = "Please wait, we are verifying this product...";
            MessageBrush = InfoMessageBrush;
            IsVerifying = true;

            V_Commands.Visibility = Visibility.Collapsed;
            B_Retry.Visibility = Visibility.Collapsed;
            B_Activate.Visibility = Visibility.Collapsed;
            B_Cancel.Visibility = Visibility.Collapsed;

            var response = await AppVerificationService.Instance.RequestVerificationAsync();

            if (response.Length > 20)
            {
                Message = "There was some problem while verifying the software. Here's the error code: UNKNOWN_HTTP";
                MessageBrush = ErrorMessageBrush;
                B_Retry.Visibility = Visibility.Visible;
            }
            else if (response == nameof(AppVerificationServerResponses.ERR_NO_KEY))
            {
                Message = "Welcome to RSFJ. You will need to activate this product with a key before you can use it.";
                MessageBrush = InfoMessageBrush;
                B_Activate.Visibility = Visibility.Visible;
                B_Cancel.Visibility = Visibility.Visible;
            }
            //else if (response == nameof(AppVerificationServerResponses.ERR_USED))
            //{
            //    Message = "The key you provided is already in use. If you think this is a mistake, contact support.";
            //    MessageBrush = ErrorMessageBrush;
            //    B_Okay.Visibility = Visibility.Visible;
            //}
            else if (response == nameof(AppVerificationServerResponses.ERR_WRONG_KEY))
            {
                Message = "The key you provided is not valid. Please try again.";
                MessageBrush = ErrorMessageBrush;
                B_Retry.Visibility = Visibility.Visible;
            }
            else if (response == AppVerificationService.ERR_NO_INTERNET)
            {
                Message = "No internet connectivity was detected. Please try again later.";
                MessageBrush = InfoMessageBrush;
                B_Retry.Visibility = Visibility.Visible;
            }
            else if (response == AppVerificationService.ERR_NO_SERVER)
            {
                Message = "There was some problem trying to contact the server. Please try again later.";
                MessageBrush = ErrorMessageBrush;
                B_Retry.Visibility = Visibility.Visible;
            }
            else if (response == AppVerificationService.ERR_REQUEST)
            {
                Message = "An unknown local error is preventing from contacting the server. Please try again later.";
                MessageBrush = ErrorMessageBrush;
                B_Retry.Visibility = Visibility.Visible;
            }
            else
            {
                Message = "There was some problem while verifying the software. Here's the error code: " + response;
                MessageBrush = ErrorMessageBrush;
                B_Retry.Visibility = Visibility.Visible;
            }

            IsVerifying = false;
            V_Commands.Visibility = Visibility.Visible;
        }

        private async void Retry_Button_Click(object sender, RoutedEventArgs e)
        {
            await VerifyApplicationAsync();
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Activate_Button_Click(object sender, RoutedEventArgs e)
        {
            //TODO
        }
    }
}
