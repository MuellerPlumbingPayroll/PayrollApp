using System;
using UIKit;

namespace Timecard.iOS
{
    public interface ICustomPickerViewModel
    {
        string GetDefaultTextFieldValue();
        void SetValueChangedView(UIView view);
    }
}
