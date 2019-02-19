using System;
using Timecard.Models;
using UIKit;

namespace Timecard.iOS.ViewControllers.PickerViewModels
{
    public class HoursWorkedPickerModel : UIPickerViewModel, ICustomPickerViewModel
    {
        private string selectedHour = "1";
        private string selectedMinutes = "00";
        private readonly string[] hours;
        private readonly string[] minutes = { "Minutes", "00", "15", "30", "45" };

        private UITextField textField;

        public HoursWorkedPickerModel()
        {
            hours = new string[ProjectSettings.MaxNumberHoursInWorkDay + 1 + 1];
            hours[0] = "Hours";
            for (var i = 0; i <= ProjectSettings.MaxNumberHoursInWorkDay; i++)
                hours[i + 1] = i.ToString();
        }

        public override void Selected(UIPickerView pickerView, nint row, nint component)
        {
            if (row == 0)
                row = 1;

            string textFormat = "{0}:{1}";

            if (component == 0)
                selectedHour = hours[row];
            else
                selectedMinutes = minutes[row];

            textField.Text = string.Format(textFormat, selectedHour, selectedMinutes);
        }

        public override string GetTitle(UIPickerView pickerView, nint row, nint component)
        {
            if (component == 0)
                return hours[row];
            return minutes[row];
        }

        public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
        {
            if (component == 0)
                return hours.Length;
            return minutes.Length;
        }

        public override nint GetComponentCount(UIPickerView pickerView)
        {
            // One component for hours, one for minutes
            return 2;
        }

        public string GetDefaultTextFieldValue()
        {
            return string.Format("{0}:{1}", selectedHour, selectedMinutes);
        }

        public void SetValueChangedView(UIView textField)
        {
            this.textField = (UITextField)textField;
        }
    }
}

