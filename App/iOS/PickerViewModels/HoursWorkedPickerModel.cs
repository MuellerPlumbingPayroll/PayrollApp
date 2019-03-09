using System;
using Timecard.Models;
using UIKit;

namespace Timecard.iOS.ViewControllers.PickerViewModels
{
    public class HoursWorkedPickerModel : UIPickerViewModel, ICustomPickerViewModel
    {
        private TimeWorked timeWorked;
        private UITextField textField;

        public HoursWorkedPickerModel() : this(new TimeWorked()) { }

        public HoursWorkedPickerModel(TimeWorked timeWorked)
        {
            this.timeWorked = timeWorked;
        }

        public override void Selected(UIPickerView pickerView, nint row, nint component)
        {
            if (row == 0)
                row = 1;
                
            if (component == 0)
                timeWorked.HoursPart = TimeWorked.Hours[row];
            else
                timeWorked.MinutesPart = TimeWorked.Minutes[row];

            textField.Text = timeWorked.ToColonFormat();
        }

        public override string GetTitle(UIPickerView pickerView, nint row, nint component)
        {
            if (component == 0)
                return TimeWorked.Hours[row];
            return TimeWorked.Minutes[row];
        }

        public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
        {
            if (component == 0)
                return TimeWorked.Hours.Length;
            return TimeWorked.Minutes.Length;
        }

        public override nint GetComponentCount(UIPickerView pickerView)
        {
            // One component for hours, one for minutes
            return 2;
        }

        public string GetDefaultTextFieldValue()
        {
            return timeWorked.ToColonFormat();
        }

        public void SetValueChangedView(UIView textField)
        {
            this.textField = (UITextField)textField;
        }

        public object GetSelectedPickerObject()
        {
            return timeWorked;
        }

        public void SetSelectedPickerObject(object o)
        {
            timeWorked = (TimeWorked)o;
        }

        public void SetSelectedJobType(JobType jobType)
        {
            // This method should never be called for this picker
            throw new NotImplementedException();
        }
    }
}
