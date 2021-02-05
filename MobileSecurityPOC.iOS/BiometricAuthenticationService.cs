using System;
using System.Threading.Tasks;
using Foundation;
using LocalAuthentication;
using MobileSecurityPOC.iOS;
using UIKit;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(BiometricAuthenticationService))]
namespace MobileSecurityPOC.iOS
{
    public class BiometricAuthenticationService : IBiometricAuthenticateService
    {
        public BiometricAuthenticationService()
        {
        }
        string BiometryType = "";

        public Task<bool> AuthenticateUserIDWithTouchID()
        {
            bool outcome = false;
            var tcs = new TaskCompletionSource<bool>();

            var context = new LAContext();
            if (context.CanEvaluatePolicy(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, out NSError AuthError))
            {


                var replyHandler = new LAContextReplyHandler((success, error) => {

                    Device.BeginInvokeOnMainThread(() => {
                        if (success)
                        {
                            outcome = true;
                        }
                        else
                        {
                            outcome = false;
                        }
                        tcs.SetResult(outcome);
                    });

                });
                //This will call both TouchID and FaceId 
                context.EvaluatePolicy(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, "Login with touch ID", replyHandler);
            };
            return tcs.Task;
        }

        public bool fingerprintEnabled()
        {
            throw new NotImplementedException();
        }

        private static int GetOsMajorVersion()
        {
            return int.Parse(UIDevice.CurrentDevice.SystemVersion.Split('.')[0]);
        }

        public string GetAuthenticationType()
        {
            var localAuthContext = new LAContext();
            NSError AuthError;

            if (localAuthContext.CanEvaluatePolicy(LAPolicy.DeviceOwnerAuthentication, out AuthError))
            {
                if (localAuthContext.CanEvaluatePolicy(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, out AuthError))
                {
                    if (GetOsMajorVersion() >= 11 && localAuthContext.BiometryType == LABiometryType.FaceId)
                    {
                        return "FaceId";
                    }

                    return "TouchId";
                }

                return "PassCode";
            }

            return "None";
        }

        public void Authendicate()
        {
            //base.ViewWillAppear(animated);
            //unAuthenticatedLabel.Text = "";
            var context = new LAContext();
            var buttonText = "";

            // Is login with biometrics possible?
            if (context.CanEvaluatePolicy(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, out var authError1))
            {
                // has Touch ID or Face ID
                if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
                {
                    context.LocalizedReason = "Authorize for access to secrets"; // iOS 11
                    BiometryType = context.BiometryType == LABiometryType.TouchId ? "Touch ID" : "Face ID";
                    buttonText = $"Login with {BiometryType}";
                    
                }
                // No FaceID before iOS 11
                else
                {
                    buttonText = $"Login with Touch ID";
                }
            }

            // Is pin login possible?
            else if (context.CanEvaluatePolicy(LAPolicy.DeviceOwnerAuthentication, out var authError2))
            {
                buttonText = $"Login"; // with device PIN
                BiometryType = "Device PIN";
            }

            // Local authentication not possible
            else
            {
                // Application might choose to implement a custom username/password
                buttonText = "Use unsecured";
                BiometryType = "none";
            }
            
            //AuthenticateButton.SetTitle(buttonText, UIControlState.Normal);
        }
    }
}
