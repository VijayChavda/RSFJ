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

        public bool AllowSkip { get; set; }

        public Verification(bool allowSkip = true)
        {
            InitializeComponent();

            AllowSkip = allowSkip;

            Loaded += async (s, e) =>
            {
                DataContext = this;
                await VerifyApplicationAsync();
            };
        }

        private async Task VerifyApplicationAsync()
        {
            B_Skip.IsEnabled = AllowSkip;

            Message = "Please wait, we are verifying this product...";
            MessageBrush = InfoMessageBrush;

            V_Commands.Visibility = Visibility.Collapsed;
            IsVerifying = true;
            var response = await AppVerificationService.Instance.RequestVerificationAsync();
            IsVerifying = false;
            V_Commands.Visibility = Visibility.Visible;

            ActOnResponse(response);
        }

        private async Task ActivateApplicationAsync()
        {
            B_Skip.IsEnabled = AllowSkip;

            Message = "Please wait while we activate your product...";
            MessageBrush = InfoMessageBrush;

            V_Commands.Visibility = Visibility.Collapsed;
            IsVerifying = true;
            var response = await AppVerificationService.Instance.RequestActivationAsync(V_KeyBox.Text);
            IsVerifying = false;
            V_Commands.Visibility = Visibility.Visible;

            ActOnResponse(response);
        }

        private void ActOnResponse(string response)
        {
            B_Retry.Visibility = B_Activate.Visibility = B_Navigate.Visibility = Visibility.Collapsed;

            if (response.Length > 20)
            {
                Message = "There was some problem while verifying the software. Here's the error code: UNKNOWN_HTTP";
                MessageBrush = ErrorMessageBrush;
                B_Retry.Visibility = Visibility.Visible;
            }
            else if (response == nameof(AppVerificationServerResponses.OK_VERIFIED))
            {
                Message = "Verification was successful. Loading app...";
                MessageBrush = InfoMessageBrush;
                MoveToMainPage();
            }
            else if (response == nameof(AppVerificationServerResponses.OK_ACTIVATED))
            {
                Message = "Your product has been activated. Thankyou for being a genuine customer :)";
                MessageBrush = InfoMessageBrush;
                MoveToMainPage();
            }
            else if (response == nameof(AppVerificationServerResponses.ERR_NO_KEY))
            {
                Message = "Welcome to RSFJ. Please activate this product with a key. " +
                    "If you skip this now, you will be asked for key later on.";
                MessageBrush = InfoMessageBrush;
                B_Navigate.Visibility = Visibility.Visible;
            }
            else if (response == nameof(AppVerificationServerResponses.ERR_USED))
            {
                Message = "The key you provided is already in use. If you think this is a mistake, contact support.";
                MessageBrush = ErrorMessageBrush;
                B_Retry.Visibility = Visibility.Visible;
            }
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

                if (AllowSkip)
                {
                    MoveToMainPage(false);
                }
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
        }

        private void MoveToMainPage(bool result = true)
        {
            DialogResult = result;
            Close();
        }

        private async void Retry_Button_Click(object sender, RoutedEventArgs e)
        {
            if (V_Activation.Visibility == Visibility.Visible)
            {
                await ActivateApplicationAsync();
            }
            else
            {
                await VerifyApplicationAsync();
            }
        }

        private async void Activate_Button_Click(object sender, RoutedEventArgs e)
        {
            await ActivateApplicationAsync();
        }

        private void Skip_Button_Click(object sender, RoutedEventArgs e)
        {
            MoveToMainPage(false);
        }

        private void Navigate_Button_Click(object sender, RoutedEventArgs e)
        {
            Message = "Please enter your product key";
            MessageBrush = InfoMessageBrush;
            B_Activate.Visibility = Visibility.Visible;
            B_Navigate.Visibility = Visibility.Collapsed;
            V_Activation.Visibility = Visibility.Visible;
        }
    }
}
