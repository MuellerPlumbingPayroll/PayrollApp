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
    [Register ("HistoryTableViewSectionHeader")]
    partial class HistoryTableViewSectionHeader
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel txtDate { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel txtHoursWorked { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (txtDate != null) {
                txtDate.Dispose ();
                txtDate = null;
            }

            if (txtHoursWorked != null) {
                txtHoursWorked.Dispose ();
                txtHoursWorked = null;
            }
        }
    }
}