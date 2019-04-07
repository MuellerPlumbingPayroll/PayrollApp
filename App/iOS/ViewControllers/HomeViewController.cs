using System;
using Foundation;
using Timecard.Authentication;
using Timecard.iOS.ViewControllers;
using Timecard.Models;
using Timecard.Services;
using Timecard.ViewModels;
using UIKit;

namespace Timecard.iOS
{
    public partial class HomeViewController : BaseViewController
    {
        public HomeViewModel ViewModel { get; set; }
        public ItemsViewModel AllItemsViewModel { get; set; }

        public HomeViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            if (ViewModel == null)
                ViewModel = new HomeViewModel();

            AllItemsViewModel = (TabBarController as TabBarController).AllItemsViewModel;
            Title = ViewModel.Title;

            txtUserName.Editable = false;
            txtUserName.Text = "Hello, " + ViewModel.UserName;

            btnNewEntry.Layer.CornerRadius = 10;
            btnNewEntry.ClipsToBounds = true;

            ConfigureSubmitButton();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            ConfigureSubmitButton();

            txtHoursWorkedToday.Text = "Today: " + AllItemsViewModel.NumberHoursWorkedOnDay(DateTime.Now.DayOfWeek) + " hrs";
            txtHoursWorkedThisWeek.Text = "This Week: " + AllItemsViewModel.NumberHoursWorkedOnDay() + " hrs";
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            if (segue.Identifier == "NavigateFromHomeToNewTimeSegue")
            {
                var controller = segue.DestinationViewController as TimeNewViewController;
                controller.ViewModel = AllItemsViewModel;
            }
        }

        private void ConfigureSubmitButton()
        {
            btnSubmit.Layer.CornerRadius = 10;
            btnSubmit.ClipsToBounds = true;

            // Determine if today is beginning or end of the pay period
            bool shouldSubmitTimecardToday =
                DateTime.Now.DayOfWeek.Equals(ProjectSettings.PayPeriodEndDay) ||
                DateTime.Now.DayOfWeek.Equals(ProjectSettings.PayPeriodStartDay);

            if (shouldSubmitTimecardToday)
                btnSubmit.BackgroundColor = UIColor.DarkGray;
            else
                btnSubmit.BackgroundColor = UIColor.LightGray;
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
