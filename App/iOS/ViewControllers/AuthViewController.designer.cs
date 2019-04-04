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
    [Register ("AuthViewController")]
    partial class AuthViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton GoogleLoginButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView txtNotAuthorizedMessage { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (GoogleLoginButton != null) {
                GoogleLoginButton.Dispose ();
                GoogleLoginButton = null;
            }

            if (txtNotAuthorizedMessage != null) {
                txtNotAuthorizedMessage.Dispose ();
                txtNotAuthorizedMessage = null;
            }
        }
    }
}