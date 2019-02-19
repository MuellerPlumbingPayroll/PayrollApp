using System;
using UIKit;

namespace Timecard.iOS.ViewControllers.PickerViewModels
{
    /// <summary>
    /// Data source for cost code picker.
    /// </summary>
    class CostCodePickerModel : UIPickerViewModel, ICustomPickerViewModel
    {
        private static readonly string DEFAULT_COST_CODE_VALUE = "Not Listed";

        private UITextField textField;
        private ItemsViewModel viewModel;

        public CostCodePickerModel(ItemsViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public override void Selected(UIPickerView pickerView, nint row, nint component)
        {
            if (textField != null) 
                textField.Text = viewModel.CostCodes[(int)row].Description;
        }

        public override string GetTitle(UIPickerView pickerView, nint row, nint component)
        {
            return viewModel.CostCodes[(int)row].Description;
        }

        public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
        {
            return viewModel.CostCodes.Count;
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
    }
}
