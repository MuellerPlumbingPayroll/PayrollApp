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
    [Register ("TimeNewViewController")]
    partial class TimeNewViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        Timecard.iOS.RoundedButton btnSaveTime { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISegmentedControl jobTypeSegControl { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        Timecard.iOS.TextFieldWithDoneButton txtCostCode { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        Timecard.iOS.TextFieldWithDoneButton txtDateField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        Timecard.iOS.TextFieldWithDoneButton txtHoursWorked { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        Timecard.iOS.TextFieldWithDoneButton txtJobDescription { get; set; }

        [Action ("JobTypeSegControl_ValueChanged:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void JobTypeSegControl_ValueChanged (UIKit.UISegmentedControl sender);

        void ReleaseDesignerOutlets ()
        {
            if (btnSaveTime != null) {
                btnSaveTime.Dispose ();
                btnSaveTime = null;
            }

            if (jobTypeSegControl != null) {
                jobTypeSegControl.Dispose ();
                jobTypeSegControl = null;
            }

            if (txtCostCode != null) {
                txtCostCode.Dispose ();
                txtCostCode = null;
            }

            if (txtDateField != null) {
                txtDateField.Dispose ();
                txtDateField = null;
            }

            if (txtHoursWorked != null) {
                txtHoursWorked.Dispose ();
                txtHoursWorked = null;
            }

            if (txtJobDescription != null) {
                txtJobDescription.Dispose ();
                txtJobDescription = null;
            }
        }
    }
}