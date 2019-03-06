using System;
using System.Drawing;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace Timecard.iOS
{
    public partial class TextFieldWithDoneButton : UITextField
    {
        public UIPickerView PickerView { get; set; }

        public TextFieldWithDoneButton(IntPtr handle) : base(handle)
        {
            // Add a done button to this text field
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

        // Prevent user from copying or pasting anything into this text field
        public override bool CanPerform(Selector action, NSObject withSender)
        {
            UIMenuController.SharedMenuController.MenuVisible = false;
            return false;
        }

        public void AddPickerToTextField(ICustomPickerViewModel pickerViewModel)
        {
            pickerViewModel.SetValueChangedView(this);

            EditingDidBegin += (sender, e) =>
            {
                if (string.IsNullOrWhiteSpace(Text) && InputView != null)
                    Text = pickerViewModel.GetDefaultTextFieldValue();
            };

            PickerView = new UIPickerView
            {
                Model = (UIPickerViewModel)pickerViewModel
            };

            InputView = PickerView;
        }

        public void SetPickerActive(bool active = true, bool fieldVisible = true, bool clearText = false)
        {
            if (active)
            {
                InputView = PickerView;
                PickerView.ReloadAllComponents();
                PickerView.Select(0, 0, true); // Select the first row in the picker
            }
            else
            {
                InputView = null;
            }

            Hidden = !fieldVisible;

            if (clearText)
                Text = string.Empty;
        }
    }
}
