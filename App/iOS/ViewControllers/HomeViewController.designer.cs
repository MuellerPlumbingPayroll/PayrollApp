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
        UIKit.UIButton btnNewEntry { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel txtHoursWorkedToday { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView txtUserName { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnNewEntry != null) {
                btnNewEntry.Dispose ();
                btnNewEntry = null;
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