using System;
using Foundation;
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

            ViewModel = new HomeViewModel();
            AllItemsViewModel = (this.TabBarController as TabBarController).AllItemsViewModel;
            Title = ViewModel.Title;

            txtUserName.Text = "Welcome, " + ViewModel.UserName;

            btnNewEntry.Layer.CornerRadius = 10;
            btnNewEntry.ClipsToBounds = true;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

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
    }
}