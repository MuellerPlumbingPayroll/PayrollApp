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
    [Register ("HomeViewController")]
    partial class HomeViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnLogOut { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        Timecard.iOS.RoundedButton btnNewEntry { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        Timecard.iOS.RoundedButton btnSubmit { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel txtHoursWorkedThisWeek { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel txtHoursWorkedToday { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView txtUserName { get; set; }

        [Action ("BtnLogOut_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void BtnLogOut_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (btnLogOut != null) {
                btnLogOut.Dispose ();
                btnLogOut = null;
            }

            if (btnNewEntry != null) {
                btnNewEntry.Dispose ();
                btnNewEntry = null;
            }

            if (btnSubmit != null) {
                btnSubmit.Dispose ();
                btnSubmit = null;
            }

            if (txtHoursWorkedThisWeek != null) {
                txtHoursWorkedThisWeek.Dispose ();
                txtHoursWorkedThisWeek = null;
            }

            if (txtHoursWorkedToday != null) {
                txtHoursWorkedToday.Dispose ();
                txtHoursWorkedToday = null;
            }

            if (txtUserName != null) {
                txtUserName.Dispose ();
                txtUserName = null;
            }
        }
    }
}