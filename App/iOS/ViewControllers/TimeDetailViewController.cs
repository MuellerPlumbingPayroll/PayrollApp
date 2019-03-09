using System;
using Foundation;
using UIKit;
using Timecard.Models;

namespace Timecard.iOS
{
    public partial class TimeDetailViewController : UIViewController
    {
        public ItemDetailViewModel ViewModel { get; set; }
        public ItemsViewModel AllItemsViewModel { get; set; }

        public TimeDetailViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = ViewModel.Title;

            dateLabel.Text = ViewModel.Item.JobDate.ToString(ProjectSettings.DateFormat);
            hoursWorkedLabel.Text = "Time Worked:   " + ViewModel.Item.TimeWorked.ToColonFormat();
            jobTypeLabel.Text = "Type:   " + ViewModel.Item.JobType;
            jobDescriptionLabel.Text = "Job:   " + ViewModel.Item.Job.Address;

            btnEditTime.Layer.CornerRadius = 10;
            btnEditTime.ClipsToBounds = true;
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            if (segue.Identifier == "NavigateToEditTimeSegue")
            {
                var controller = segue.DestinationViewController as TimeNewViewController;
                controller.ViewModel = AllItemsViewModel;
                controller.EditingItem = ViewModel.Item;
            }
        }

        partial void DeleteButton_TouchUpInside(UIButton sender)
        {
            var alert = UIAlertController.Create(
            "Are you sure you want to delete this entry?",
            "This action cannot be undone" ,
             UIAlertControllerStyle.ActionSheet);

            alert.AddAction(UIAlertAction.Create("Delete", UIAlertActionStyle.Destructive, (UIAlertAction) => {
                AllItemsViewModel.DeleteItemCommand.Execute(ViewModel.Item);
                NavigationController.PopToRootViewController(true);
            }));
            alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));

            PresentViewController(alert, animated: true, completionHandler: null);
        }
    }
}