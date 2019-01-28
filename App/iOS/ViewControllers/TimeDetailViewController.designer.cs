// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace Timecard.iOS
{
    [Register ("TimeDetailViewController")]
    partial class TimeDetailViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnEditTime { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel dateLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel hoursWorkedLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel jobDescriptionLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel jobTypeLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnEditTime != null) {
                btnEditTime.Dispose ();
                btnEditTime = null;
            }

            if (dateLabel != null) {
                dateLabel.Dispose ();
                dateLabel = null;
            }

            if (hoursWorkedLabel != null) {
                hoursWorkedLabel.Dispose ();
                hoursWorkedLabel = null;
            }

            if (jobDescriptionLabel != null) {
                jobDescriptionLabel.Dispose ();
                jobDescriptionLabel = null;
            }

            if (jobTypeLabel != null) {
                jobTypeLabel.Dispose ();
                jobTypeLabel = null;
            }
        }
    }
}