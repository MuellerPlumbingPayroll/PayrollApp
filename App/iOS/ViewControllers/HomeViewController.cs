using System;
using System.Threading.Tasks;
using Foundation;
using Timecard.Authentication;
using Timecard.Models;
using Timecard.Services;
using Timecard.ViewModels;
using UIKit;

namespace Timecard.iOS
{
    public partial class HomeViewController : BaseViewController
    {
        public HomeViewModel ViewModel { get; set; }

        public HomeViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            if (ViewModel == null)
                ViewModel = new HomeViewModel();

            Title = ViewModel.Title;

            txtUserName.Editable = false;
            txtUserName.Text = "Hello, " + ViewModel.UserName;

            btnSubmit.BackgroundColor = UIColor.LightGray;        
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            ConfigureInformationLabels();
        }

        private async void ConfigureInformationLabels()
        {
            // Sometimes the view may appear while items are still being retrieved from the server.
            // Repeatedly waiting until the items have been retrieved ensures that the hours worked text is correct.
            while (AllItemsViewModel.IsBusy)
            {
                await Task.Delay(100);
            }

            txtHoursWorkedToday.Text = "Today: " + AllItemsViewModel.NumberHoursWorkedOnDay(DateTime.Now) + " hrs";
            txtHoursWorkedThisWeek.Text = "This Period: " + AllItemsViewModel.NumberHoursWorkedOnDay() + " hrs";

            var startCurrent = ProjectSettings.GetStartOfCurrentPayPeriod().Date;
            var startEntires = AllItemsViewModel.GetStartOfPayPeriod().Date;

            if (startEntires.CompareTo(startCurrent) < 0)
            {
                txtWarningLabel.AdjustsFontSizeToFitWidth = true;
                txtWarningLabel.Text = ViewModel.NeedToSubmitTimecardWarningMessage;
            }
            else
            {
                txtWarningLabel.Text = string.Empty;
            }
        }

        public override bool ShouldPerformSegue(string segueIdentifier, NSObject sender)
        {
            if (segueIdentifier == "NavigateFromHomeToSubmit")
            {
                float numHoursWorked = AllItemsViewModel.NumberHoursWorkedOnDay();

                if (numHoursWorked < ProjectSettings.NumberHoursInWorkWeek)
                {
                    // The user hasn't worked at least 40 hours this week so display an alert message 
                    var alert = UIAlertController.Create(
                        "Are you sure you want to submit your timecard?",
                        string.Format("You have recorded less than {0} hours this week.", ProjectSettings.NumberHoursInWorkWeek),
                        UIAlertControllerStyle.ActionSheet);

                    alert.AddAction(UIAlertAction.Create("Continue", UIAlertActionStyle.Default, (UIAlertAction) =>
                    {
                        PerformSegue("NavigateFromHomeToSubmit", null);
                    }));

                    alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
                    PresentViewController(alert, animated: true, completionHandler: null);

                    return false;
                }
            }

            return true;
        }

        partial void BtnLogOut_TouchUpInside(UIButton sender)
        {
            var alert = UIAlertController.Create("Are you sure you want to log out?",
                                                 string.Empty, UIAlertControllerStyle.ActionSheet);

            alert.AddAction(UIAlertAction.Create("Log Out", UIAlertActionStyle.Destructive, (UIAlertAction) =>
            {
                LogOut();
            }));

            alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
            PresentViewController(alert, animated: true, completionHandler: null);
        }

        private void LogOut()
        {
            FirebaseManager.Auth.SignOut(out NSError error);

            if (error == null)
            {
                GoogleUserInfo.RemoveFromDevice();
                GoogleOAuthToken.RemoveFromDevice();
                FirebaseUserInfo.RemoveFromDevice();

                var rootNavController = Storyboard.InstantiateViewController("navLoginController") as UINavigationController;

                var appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;
                appDelegate.Window.RootViewController = rootNavController;
            }
            else
            {
                DisplayErrorMessage("An error occurred while logging you out.");
            }
        }
    }
}
