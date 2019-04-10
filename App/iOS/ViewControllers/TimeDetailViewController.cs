using System;
using Foundation;
using Timecard.Models;
using UIKit;

namespace Timecard.iOS
{
    public partial class TimeDetailViewController : BaseViewController
    {
        public ItemDetailViewModel ViewModel { get; set; }

        public TimeDetailViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = ViewModel.Title;

            dateLabel.Text = ViewModel.Item.JobDate.ToLocalTime().ToString(ProjectSettings.DateFormat);
            hoursWorkedLabel.Text = "Time Worked:   " + ViewModel.Item.TimeWorked.ToColonFormat();
            jobTypeLabel.Text = "Type:   " + ViewModel.Item.JobType;
            jobDescriptionLabel.Text = "Job:   " + ViewModel.Item.Job.Address;
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            if (segue.Identifier == "NavigateToEditTimeSegue")
            {
                var controller = segue.DestinationViewController as TimeNewViewController;
                controller.EditingItem = ViewModel.Item;
            }
        }

        partial void DeleteButton_TouchUpInside(UIButton sender)
        {
            var alert = UIAlertController.Create(
            "Are you sure you want to delete this entry?",
            "This action cannot be undone",
             UIAlertControllerStyle.ActionSheet);

            alert.AddAction(UIAlertAction.Create("Delete", UIAlertActionStyle.Destructive, (UIAlertAction) => {
                PerformDeleteAction();
            }));
            alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));

            PresentViewController(alert, animated: true, completionHandler: null);
        }

        private async void PerformDeleteAction()
        {
            DisplayLoadingIndicator();
            bool success = await AllItemsViewModel.DeleteItem(ViewModel.Item);

            if (success)
            {
                NavigationController.PopToRootViewController(true);
            }
            else
            {
                RemoveLoadingIndicator();
                DisplayErrorMessage("Error encountered when deleting time entry.");
            }
        }
    }
}
