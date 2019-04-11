using System;
using UIKit;

namespace Timecard.iOS.Utilities
{
    public class TimeCrudManager
    {
        private readonly BaseViewController _viewController;

        public TimeCrudManager(BaseViewController viewController)
        {
            _viewController = viewController;
        }

        public void AddDeleteButtonToNavBarForItem(Item itemToDelete)
        {
            if (itemToDelete != null)
            {
                var button = new UIButton(UIButtonType.System);
                button.SetTitle("Delete", UIControlState.Normal);

                button.AddTarget((sender, e) =>
                {
                    OnDeleteButtonClicked(itemToDelete);
                },
                UIControlEvent.TouchUpInside);

                var barButton = new UIBarButtonItem(button);
                _viewController.NavigationItem.SetRightBarButtonItem(barButton, false);
            }
        }

        private void OnDeleteButtonClicked(Item itemToDelete)
        {
            var alert = UIAlertController.Create(
            "Are you sure you want to delete this entry?",
            "This action cannot be undone",
             UIAlertControllerStyle.ActionSheet);

            alert.AddAction(UIAlertAction.Create("Delete", UIAlertActionStyle.Destructive, (UIAlertAction) => {
                PerformDeleteAction(itemToDelete);
            }));
            alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
            
            _viewController.PresentViewController(alert, animated: true, completionHandler: null);
        }

        private async void PerformDeleteAction(Item itemToDelete)
        {
            _viewController.DisplayLoadingIndicator();
            bool success = await BaseViewController.AllItemsViewModel.DeleteItem(itemToDelete);

            if (success)
            {
                _viewController.NavigationController.PopToRootViewController(true);
            }
            else
            {
                _viewController.RemoveLoadingIndicator();
                _viewController.DisplayErrorMessage("Error encountered when deleting time entry.");
            }
        }

        public async void SaveItem(Item item)
        {
            _viewController.DisplayLoadingIndicator();

            bool success;
            if (item.Id == null) 
            {
                success = await BaseViewController.AllItemsViewModel.AddItem(item);
            }
            else
            {
                success = await BaseViewController.AllItemsViewModel.UpdateItem(item);
            }

            if (success)
            {
                _viewController.NavigationController.PopToRootViewController(true);
            }
            else
            {
                _viewController.RemoveLoadingIndicator();
                _viewController.DisplayErrorMessage("Failed to save time entry.");
            }
        }
    }
}
