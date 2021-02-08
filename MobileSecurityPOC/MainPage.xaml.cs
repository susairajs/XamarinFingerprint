using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using Xamarin.Forms;

namespace MobileSecurityPOC
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private CancellationTokenSource _cancel;

        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var authType = await Plugin.Fingerprint.CrossFingerprint.Current.GetAuthenticationTypeAsync();

            lblAuthenticationType.Text = "Auth Type: " + authType;
        }

        async void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            //var result = await CrossFingerprint.Current.GetAvailabilityAsync(true);
            //var result1 = await CrossFingerprint.Current.IsAvailableAsync(true);
            //var result2 = await CrossFingerprint.Current.GetAvailabilityAsync();
            //var result3 = await CrossFingerprint.Current.IsAvailableAsync();

            // DependencyService.Get<IBiometricAuthenticateService>().Authendicate();
            await AuthenticateAsync("Authendicate with Touch ID");
            //var authType = await Plugin.Fingerprint.CrossFingerprint.Current.GetAuthenticationTypeAsync();
            //if (authType == AuthenticationType.Fingerprint)
            //{
            //    lblAuthenticationType.Text = "Auth Type: " + authType;

            //    await AuthenticateAsync("Authendicate with Touch ID");
            //}
        }

        private async Task AuthenticateAsync(string reason, string cancel = null, string fallback = null, string tooFast = null)
        {
            _cancel = false ? new CancellationTokenSource(TimeSpan.FromSeconds(10)) : new CancellationTokenSource();
            
            var dialogConfig = new AuthenticationRequestConfiguration("My App", reason)
            { 
                CancelTitle = cancel,
                FallbackTitle = fallback,
                AllowAlternativeAuthentication = true,
                ConfirmationRequired = false
            };

            dialogConfig.HelpTexts.MovedTooFast = tooFast;
            var result = await Plugin.Fingerprint.CrossFingerprint.Current.AuthenticateAsync(dialogConfig, _cancel.Token);
            await SetResultAsync(result);
        }

        private async Task SetResultAsync(FingerprintAuthenticationResult result)
        {
            if (result.Authenticated)
            {
                await DisplayAlert("Success", "Fingerprint success", "OK");
                lblStatus.Text = result.Status.ToString();
            }
            else
            {
                lblStatus.Text = $"{result.Status}: {result.ErrorMessage}";
            }
        }

        
        //async void Button_Clicked(System.Object sender, System.EventArgs e)
        //{

        //    //bool isFingerprintAvailable = await CrossFingerprint.Current.IsAvailableAsync(false);
        //    //if (!isFingerprintAvailable)
        //    //{
        //    //    await DisplayAlert("Error",
        //    //        "Biometric authentication is not available or is not configured.", "OK");
        //    //    return;
        //    //}

        //    //DependencyService.Get<IBiometricAuthenticateService>().Authendicate();
        //    var result = DependencyService.Get<IBiometricAuthenticateService>().GetAuthenticationType();
        //    if (result.Equals("TouchId"))
        //    {
        //        await GetAuthResults();
        //    }

        //    //AuthenticationRequestConfiguration conf =
        //    //    new AuthenticationRequestConfiguration("Authentication",
        //    //    "Authenticate access to your personal data");

        //    //var authResult = await CrossFingerprint.Current.AuthenticateAsync(conf);
        //    //if (authResult.Authenticated)
        //    //{
        //    //    //Success  
        //    //    await DisplayAlert("Success", "Authentication succeeded", "OK");
        //    //}
        //    //else
        //    //{
        //    //    await DisplayAlert("Error", "Authentication failed", "OK");
        //    //}
        //}

        //private async Task GetAuthResults()
        //{
        //    //todo according to Auth type change the authenticationmethod in interface if face id or touch id
        //    //string AuthType = DependencyService.Get<IFingerprintAuthService>().GetAuthenticationType();
        //    var result = await DependencyService.Get<IBiometricAuthenticateService>().AuthenticateUserIDWithTouchID();
        //    if (result)
        //    {
        //        //if (AuthType.Equals("TouchId"))
        //        //{
        //        //    contentPage.BackgroundColor = Color.Green;
        //        //    lbl.Text = "TouchID authentication success";
        //        //}
        //        //else if (AuthType.Equals("FaceId"))
        //        //{
        //        //    contentPage.BackgroundColor = Color.Green;
        //        //    lbl.Text = "FaceID authentication success";
        //        //}
        //    }
        //    else
        //    {
        //        //contentPage.BackgroundColor = Color.Red;
        //        //lbl.Text = "Authentication failed";
        //    }
        //}
    }
}
