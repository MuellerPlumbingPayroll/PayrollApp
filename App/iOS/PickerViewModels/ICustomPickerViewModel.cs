using System;
using Timecard.Models;
using UIKit;

namespace Timecard.iOS
{
    public interface ICustomPickerViewModel
    {
        void SetSelectedJobType(JobType jobType);
        string GetDefaultTextFieldValue();
        void SetValueChangedView(UIView view);
        object GetSelectedPickerObject();
        void SetSelectedPickerObject(object o);
        int[] GetPickerIndexesToSelect(object o);
    }
}
