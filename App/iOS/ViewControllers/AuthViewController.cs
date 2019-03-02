using System;
using Timecard.Authentication;
using Timecard.Services;
using UIKit;

namespace Timecard.iOS
{
    public partial class AuthViewController : UIViewController, IGoogleAuthenticationDelegate
    {
        public static GoogleAuthenticator Auth;

        public AuthViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = "Login";

            Auth = new GoogleAuthenticator(GoogleConfiguration.ClientId, 
                                           GoogleConfiguration.Scope,
                                           GoogleConfiguration.RedirectUrl,
                                           this);
            
            GoogleLoginButton.TouchUpInside += OnGoogleLoginButtonClicked;
        }

        private void OnGoogleLoginButtonClicked(object sender, EventArgs e)
        {
            var authenticator = Auth.GetAuthenticator();
            var viewController = authenticator.GetUI();
            PresentViewController(viewController, true, null);
        }

        public async void OnAuthenticationCompleted(GoogleOAuthToken token)
        {
            // SFSafariViewController doesn't dismiss itself
            DismissViewController(true, null);

            var googleService = new GoogleService();
            var userInfo = await googleService.GetUserInfoAsync(token.TokenType, token.AccessToken);

            // TODO: Save the user's info to the device

            GoogleLoginButton.SetTitle($"Connected with {userInfo.Email}", UIControlState.Normal);

            // TODO: Check if user is authorized to access the app
            var isUserAuthorized = true;

            if (isUserAuthorized)
            {
                userInfo.SaveToDevice();
                PerformSegueToHome(userInfo.GivenName);
            }
            else
            {
                // TODO: Display appropriate message on UI
            }
        }

        public void OnAuthenticationCanceled()
        {
            // SFSafariViewController doesn't dismiss itself
            DismissViewController(true, null);

            var alertController = new UIAlertController
            {
                Title = "Authentication canceled",
                Message = "You didn't complete the authentication process"
            };
            PresentViewController(alertController, true, null);
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

            homeViewController.ViewModel = new ViewModels.HomeViewModel(userName);

            // Set the tab bar controller as root
            var appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;
            appDelegate.Window.RootViewController = tabBarController;
        }
    }
}