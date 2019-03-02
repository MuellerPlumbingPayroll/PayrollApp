using System;
using Foundation;
using Timecard.Models;
using Timecard.ViewModels;
using UIKit;

namespace Timecard.iOS
{
    public partial class HomeViewController : UIViewController
    {
        public HomeViewModel ViewModel { get; set; }
        public ItemsViewModel AllItemsViewModel { get; set; }

        public HomeViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            if (ViewModel == null)
                ViewModel = new HomeViewModel();

            AllItemsViewModel = (this.TabBarController as TabBarController).AllItemsViewModel;
            Title = ViewModel.Title;

            txtUserName.Editable = false;
            txtUserName.Text = "Welcome, " + ViewModel.UserName;

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

            // TODO: Update with actual value
            bool hasSubmittedTimecard = false;

            // If today is the first day of a new pay period, the user should 
            // submit the previous period's entries.
            bool shouldSubmitTimecardToday = 
                new DateTime().Day.Equals(ProjectSettings.PayPeriodStartDay);

            if (!hasSubmittedTimecard && shouldSubmitTimecardToday)
                btnSubmit.BackgroundColor = UIColor.DarkGray;
            else
                btnSubmit.BackgroundColor = UIColor.LightGray;

            btnSubmit.TouchUpInside += OnSubmitButtonClicked;
        }

        private void OnSubmitButtonClicked(object sender, EventArgs e)
        {
            float numHoursWorked = AllItemsViewModel.NumberHoursWorkedOnDay();

            if (numHoursWorked < ProjectSettings.NumberHoursInWorkWeek)
            {
                var alert = UIAlertController.Create(
                    "Are you sure you want to submit your timecard?",
                    string.Format("You have recorded less than {0} hours this week.", ProjectSettings.NumberHoursInWorkWeek),
                    UIAlertControllerStyle.ActionSheet);

                alert.AddAction(UIAlertAction.Create("Continue", UIAlertActionStyle.Default, (UIAlertAction) =>
                {
                    if (numHoursWorked > 0)
                    {
                        // TODO: Submit this timecard
                    }
                }));

                alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
                PresentViewController(alert, animated: true, completionHandler: null);
            }
        }
    }
}