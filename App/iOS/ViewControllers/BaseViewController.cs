﻿using System;
using CoreGraphics;
using UIKit;

namespace Timecard.iOS
{
    public class BaseViewController : UIViewController
    {
        public static readonly ItemsViewModel AllItemsViewModel;

        static BaseViewController()
        {
            AllItemsViewModel = new ItemsViewModel();
        }
        
        private UIView _spinnerView;

        public BaseViewController(IntPtr handle) : base(handle)
        {
        }

        public void DisplayErrorMessage(string message)
        {
            var alert = UIAlertController.Create("Error", message, UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create("Okay", UIAlertActionStyle.Cancel, null));

            PresentViewController(alert, animated: true, completionHandler: null);
        }

        public void AddTapToDismissGesture()
        {
            // When user taps outside of a picker or keyboard, it disappears
            var tapGesture = new UITapGestureRecognizer(() => View.EndEditing(true));
            View.AddGestureRecognizer(tapGesture);
        }

        public void DisplayLoadingIndicator()
        {
            if (_spinnerView == null)
            {
                _spinnerView = new UIView(View.Bounds)
                {
                    BackgroundColor = new UIColor(0.5f, 0.5f, 0.5f, 0.5f)
                };

                var activityIndicator = new UIActivityIndicatorView(new CGRect(0, 0, 100, 100))
                {
                    ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.WhiteLarge,
                    Center = _spinnerView.Center,
                    HidesWhenStopped = true
                };
                activityIndicator.Layer.CornerRadius = 5;
                activityIndicator.StartAnimating();

                _spinnerView.AddSubview(activityIndicator);
            }

            View.AddSubview(_spinnerView);
        }

        public void RemoveLoadingIndicator()
        {
            if (_spinnerView != null)
                _spinnerView.RemoveFromSuperview();
        }
    }
}
