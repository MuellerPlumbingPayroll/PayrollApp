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

        public string SelectedJobType { get; set; }
        private UITextField textField;
        private ItemsViewModel viewModel;

        public CostCodePickerModel(ItemsViewModel viewModel, string selectedJobType)
        {
            this.viewModel = viewModel;
            this.SelectedJobType = selectedJobType;
        }

        public override void Selected(UIPickerView pickerView, nint row, nint component)
        {
            if (textField != null) 
                textField.Text = viewModel.CostCodes[SelectedJobTypeToCodeGroup()][(int)row].Description;
        }

        public override string GetTitle(UIPickerView pickerView, nint row, nint component)
        {
            var codeGroup = SelectedJobTypeToCodeGroup();

            if (viewModel.CostCodes.ContainsKey(codeGroup))
                return viewModel.CostCodes[codeGroup][(int)row].Description;
            return DEFAULT_COST_CODE_VALUE;
        }

        public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
        {
            var codeGroup = SelectedJobTypeToCodeGroup();

            if (viewModel.CostCodes.ContainsKey(codeGroup))
                return viewModel.CostCodes[SelectedJobTypeToCodeGroup()].Count;
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

        private string SelectedJobTypeToCodeGroup()
        {
            switch(SelectedJobType)
            {
                case JobType.Construction:
                    return CostCode.PlumbingCodeGroup;
                case JobType.Service:
                    return CostCode.ServiceCodeGroup;
                default:
                    return null;
            }
        }
    }
}
