using System;
using UIKit;

namespace Timecard.iOS.ViewControllers
{
    public class BaseViewController : UIViewController
    {
        public BaseViewController(IntPtr handle) : base(handle)
        {
        }

        public void DisplayAlertMessage(string message)
        {
            var alert = UIAlertController.Create("Error", message, UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create("Okay", UIAlertActionStyle.Cancel, null));

            PresentViewController(alert, animated: true, completionHandler: null);
        }
    }
}
