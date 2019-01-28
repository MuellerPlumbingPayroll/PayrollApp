using Foundation;
using System;
using UIKit;
using Timecard.ViewModels;

namespace Timecard.iOS
{
    public partial class HomeViewController : UIViewController
    {
        public HomeViewModel ViewModel { get; set; }

        public HomeViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ViewModel = new HomeViewModel();
            Title = ViewModel.Title;

            txtUserName.Text = "Welcome, " + ViewModel.UserName;

            btnNewEntry.Layer.CornerRadius = 10;
            btnNewEntry.ClipsToBounds = true;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            txtHoursWorkedToday.Text = "Today's Hours: " + ViewModel.NumberHoursWorkedToday();
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            if (segue.Identifier == "NavigateFromHomeToNewTimeSegue")
            {
                var controller = segue.DestinationViewController as TimeNewViewController;
                controller.ViewModel = new ItemsViewModel();
            }
        }
    }
}