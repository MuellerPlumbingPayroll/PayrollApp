using System;
using Timecard.Models;
using UIKit;

namespace Timecard.iOS.ViewControllers.PickerViewModels
{
    /// <summary>
    /// Data source for cost code picker.
    /// </summary>
    class CostCodePickerModel : UIPickerViewModel, ICustomPickerViewModel
    {
        private static readonly string DEFAULT_COST_CODE_VALUE = "Not Listed";

        private JobType selectedJobType;
        private CostCode selectedCostCode;
        private UITextField textField;
        private ItemsViewModel viewModel;

        public CostCodePickerModel(ItemsViewModel viewModel, JobType selectedJobType)
        {
            this.viewModel = viewModel;
            this.selectedJobType = selectedJobType;

            selectedCostCode = viewModel.CostCodes[this.selectedJobType][0];
        }

        public override void Selected(UIPickerView pickerView, nint row, nint component)
        {
            selectedCostCode = viewModel.CostCodes[selectedJobType][(int)row];

            if (textField != null)
                textField.Text = selectedCostCode.Description;
        }

        public override string GetTitle(UIPickerView pickerView, nint row, nint component)
        {
            if (viewModel.CostCodes.ContainsKey(selectedJobType))
                return viewModel.CostCodes[selectedJobType][(int)row].Description;

            return DEFAULT_COST_CODE_VALUE;
        }

        public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
        {
            if (viewModel.CostCodes.ContainsKey(selectedJobType))
                return viewModel.CostCodes[selectedJobType].Count;

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
                return DEFAULT_COST_CODE_VALUE;
            }
        }

        public void SetValueChangedView(UIView textField)
        {
            this.textField = (UITextField)textField;
        }

        public object GetSelectedPickerObject()
        {
            return selectedCostCode;
        }

        public void SetSelectedPickerObject(object o)
        {
            selectedCostCode = (CostCode)o;
        }

        public void SetSelectedJobType(JobType jobType)
        {
            selectedJobType = jobType;
            if (jobType != JobType.Other)
            {
                selectedCostCode = viewModel.CostCodes[jobType][0];
            }
            else
            {
                selectedCostCode = null;
            }
        }
    }
}
