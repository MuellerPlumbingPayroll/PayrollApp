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
    [Register ("SubmitTimecardViewController")]
    partial class SubmitTimecardViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnSubmit { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        Timecard.iOS.TextFieldWithDoneButton txtAnswer { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView txtConsent { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel txtWorkInjury { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnSubmit != null) {
                btnSubmit.Dispose ();
                btnSubmit = null;
            }

            if (txtAnswer != null) {
                txtAnswer.Dispose ();
                txtAnswer = null;
            }

            if (txtConsent != null) {
                txtConsent.Dispose ();
                txtConsent = null;
            }

            if (txtWorkInjury != null) {
                txtWorkInjury.Dispose ();
                txtWorkInjury = null;
            }
        }
    }
}