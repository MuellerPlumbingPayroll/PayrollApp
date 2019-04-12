using System;
using Timecard.Models;
using Timecard.ViewModels;
using UIKit;

namespace Timecard.iOS.PickerViewModels
{
    public class WorkInjuryPickerModel : UIPickerViewModel, ICustomPickerViewModel
    {
        private readonly SubmitTimecardViewModel _submitTimecardViewModel;
        private UITextField _textField;

        public WorkInjuryPickerModel(SubmitTimecardViewModel submitTimecardViewModel)
        {
            _submitTimecardViewModel = submitTimecardViewModel;
        }

        public override void Selected(UIPickerView pickerView, nint row, nint component)
        {
            if (_textField != null)
                _textField.Text = _submitTimecardViewModel.WorkInjuryPickerOptions[row];
        }

        public override string GetTitle(UIPickerView pickerView, nint row, nint component)
        {
            return _submitTimecardViewModel.WorkInjuryPickerOptions[row];
        }

        public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
        {
            return _submitTimecardViewModel.WorkInjuryPickerOptions.Length;
        }

        public override nint GetComponentCount(UIPickerView pickerView)
        {
            return 1;
        }

        public string GetDefaultTextFieldValue()
        {
            return _submitTimecardViewModel.WorkInjuryPickerOptions[0];
        }

        public void SetValueChangedView(UIView view)
        {
            _textField = view as UITextField;
        }

        // These methods are not used
        public object GetSelectedPickerObject()
        {
            throw new NotImplementedException();
        }

        public void SetSelectedPickerObject(object o)
        {
            throw new NotImplementedException();
        }

        public void SetSelectedJobType(JobType jobType)
        {
            throw new NotImplementedException();
        }

        public int[] GetPickerIndexesToSelect(object o)
        {
            throw new NotImplementedException();
        }
    }
}
