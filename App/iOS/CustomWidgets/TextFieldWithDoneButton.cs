using Foundation;
using System;
using System.Drawing;
using UIKit;

namespace Timecard.iOS
{
    public partial class TextFieldWithDoneButton : UITextField
    {
        public TextFieldWithDoneButton (IntPtr handle) : base (handle)
        {
            var toolbar = new UIToolbar(new RectangleF(0.0f, 0.0f, 50.0f, 44.0f));
            var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate
            {
                ResignFirstResponder();
            });

            toolbar.Items = new UIBarButtonItem[] 
            {
                new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
                doneButton
            };

            InputAccessoryView = toolbar;
        }
    }
}