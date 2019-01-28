using Foundation;
using System;
using UIKit;

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

            dateLabel.Text = ViewModel.Item.JobDate;
            hoursWorkedLabel.Text = "Hours Worked:   " + ViewModel.Item.HoursWorked;
            jobTypeLabel.Text = "Type:   " + ViewModel.Item.JobType;
            jobDescriptionLabel.Text = "Job:   " + ViewModel.Item.JobDescription;

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
    }
}