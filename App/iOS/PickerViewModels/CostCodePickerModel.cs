using System;
using Timecard.Models;
using UIKit;

namespace Timecard.iOS.PickerViewModels
{
    /// <summary>
    /// Data source for cost code picker.
    /// </summary>
    class CostCodePickerModel : UIPickerViewModel, ICustomPickerViewModel
    {
        private readonly ItemsViewModel _viewModel;
        private JobType _selectedJobType;
        private CostCode _selectedCostCode;
        private UITextField _textField;

        public CostCodePickerModel(ItemsViewModel viewModel, JobType selectedJobType)
        {
            _viewModel = viewModel;
            _selectedJobType = selectedJobType;

            if (viewModel.CostCodes.ContainsKey(selectedJobType) && viewModel.CostCodes.Count > 0)
                _selectedCostCode = viewModel.CostCodes[selectedJobType][0];
        }

        public override void Selected(UIPickerView pickerView, nint row, nint component)
        {
            _selectedCostCode = _viewModel.CostCodes[_selectedJobType][(int)row];

            if (_textField != null)
                _textField.Text = _selectedCostCode.Description;
        }

        public override string GetTitle(UIPickerView pickerView, nint row, nint component)
        {
            if (_viewModel.CostCodes.ContainsKey(_selectedJobType))
                return _viewModel.CostCodes[_selectedJobType][(int)row].Description;

            return string.Empty;
        }

        public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
        {
            if (_viewModel.CostCodes.ContainsKey(_selectedJobType))
                return _viewModel.CostCodes[_selectedJobType].Count;

            return 0;
        }

        public override nint GetComponentCount(UIPickerView pickerView)
        {
            return 1;
        }

        public string GetDefaultTextFieldValue()
        {
            try
            {
                return GetTitle(null, 0, 0);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public void SetValueChangedView(UIView textField)
        {
            _textField = (UITextField)textField;
        }

        public object GetSelectedPickerObject()
        {
            return _selectedCostCode;
        }

        public void SetSelectedPickerObject(object o)
        {
            _selectedCostCode = (CostCode)o;
        }

        public void SetSelectedJobType(JobType jobType)
        {
            _selectedJobType = jobType;
            if (_viewModel.CostCodes.ContainsKey(jobType))
            {
                _selectedCostCode = _viewModel.CostCodes[jobType][0];
            }
            else
            {
                _selectedCostCode = null;
            }
        }

        public int[] GetPickerIndexesToSelect(object o)
        {
            var costCode = (CostCode)o;

            if (costCode == null || !_viewModel.CostCodes.ContainsKey(_selectedJobType))
            {
                return null;
            }

            // Search for the cost code. If found, return its index
            for (int i = 0; i < _viewModel.CostCodes[_selectedJobType].Count; i++)
            {
                if (costCode.Equals(_viewModel.CostCodes[_selectedJobType][i]))
                {
                    return new int[] { i };
                }
            }

            return null;
        }
    }
}
