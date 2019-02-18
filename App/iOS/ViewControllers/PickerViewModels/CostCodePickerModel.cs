using System;
using UIKit;

namespace Timecard.iOS.ViewControllers.PickerViewModels
{
    /// <summary>
    /// Data source for cost code picker.
    /// </summary>
    class CostCodePickerModel : UIPickerViewModel
    {
        private ItemsViewModel viewModel;
        private UITextField textField;

        public CostCodePickerModel(ItemsViewModel viewModel, UITextField textField)
        {
            this.viewModel = viewModel;
            this.textField = textField;
        }

        public override void Selected(UIPickerView pickerView, nint row, nint component)
        {
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
    }
}
