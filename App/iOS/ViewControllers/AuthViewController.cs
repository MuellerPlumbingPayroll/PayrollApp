using System;
using Firebase.Auth;
using Timecard.Authentication;
using Timecard.iOS.ViewControllers;
using Timecard.Services;
using Timecard.ViewModels;
using UIKit;

namespace Timecard.iOS
{
    public partial class AuthViewController : BaseViewController, IGoogleAuthenticationDelegate
    {
        public static GoogleAuthenticator Auth;

        private AuthViewModel _authViewModel;

        public AuthViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            _authViewModel = new AuthViewModel();
            Title = _authViewModel.Title;

            Auth = new GoogleAuthenticator(GoogleConfiguration.ClientId, 
                                           GoogleConfiguration.Scope,
                                           GoogleConfiguration.RedirectUrl,
                                           this);
            ConfigureGoogleLoginButton();
        }

        private void ConfigureGoogleLoginButton()
        {
            GoogleLoginButton.Layer.CornerRadius = 10;
            GoogleLoginButton.ClipsToBounds = true;

            GoogleLoginButton.TouchUpInside += OnGoogleLoginButtonClicked;
        }

        private void OnGoogleLoginButtonClicked(object sender, EventArgs e)
        {
            var googleToken = GoogleOAuthToken.ReadFromDevice().Result;
            var userInfo = GoogleUserInfo.ReadFromDevice().Result;

            if (googleToken == null || userInfo == null)
            {
                var authenticator = Auth.GetAuthenticator();
                var viewController = authenticator.GetUI();
                PresentViewController(viewController, true, null);
            }
            else
            {
                PerformSegueToHome(userInfo.GivenName);
            }
        }

        public async void OnAuthenticationCompleted(GoogleOAuthToken token)
        {
            // SFSafariViewController doesn't dismiss itself
            DismissViewController(true, null);

            DisplayLoadingIndicator();

            var googleUserInfo = await _authViewModel.RetrieveGoogleUserInfoAsync(token.TokenType, token.AccessToken);
            var firebaseUserInfo = await _authViewModel.AuthenticateUserWithFirebaseAsync(googleUserInfo);

            if (firebaseUserInfo != null)
            {
                AuthCredential credential = GoogleAuthProvider.GetCredential(token.IdToken, token.AccessToken);
                AuthDataResult result = await FirebaseManager.Auth.SignInAndRetrieveDataWithCredentialAsync(credential);
                
                token.SaveToDevice();
                googleUserInfo.SaveToDevice();
                firebaseUserInfo.SaveToDevice();

                PerformSegueToHome(googleUserInfo.GivenName);
            }
            else
            {
                await _authViewModel.RevokeTokenAsync(token.TokenType, token.AccessToken);

                RemoveLoadingIndicator();

                txtNotAuthorizedMessage.Text = _authViewModel.FirebaseNotAuthorizedErrorMessage;
                GoogleLoginButton.Hidden = true;
            }
        }

        public void OnAuthenticationCanceled()
        {
            // SFSafariViewController doesn't dismiss itself
            DismissViewController(true, null);
        }

        public void OnAuthenticationFailed(string message, Exception exception)
        {
            // SFSafariViewController doesn't dismiss itself
            DismissViewController(true, null);

            var alertController = new UIAlertController
            {
                Title = message,
                Message = exception?.ToString()
            };
            PresentViewController(alertController, true, null);
        }

        private void PerformSegueToHome(string userName)
        {
            var tabBarController = this.Storyboard.InstantiateViewController("tabViewController") as TabBarController;

            var navigationController = tabBarController.ViewControllers[0] as UINavigationController;
            var homeViewController = navigationController.ViewControllers[0] as HomeViewController;

            homeViewController.ViewModel = new HomeViewModel(userName);

            // Set the tab bar controller as root
            var appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;
            appDelegate.Window.RootViewController = tabBarController;
        }
    }
}