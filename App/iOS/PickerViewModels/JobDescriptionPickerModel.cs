using System;
using UIKit;

namespace Timecard.iOS.ViewControllers.PickerViewModels
{
    /// <summary>
    /// Data source for the job picker.
    /// </summary>
    class JobDescriptionPickerModel : UIPickerViewModel, ICustomPickerViewModel
    {
        private static readonly string DEFAULT_JOB_DESCRIPTION_VALUE = "Not Listed";

        public string SelectedJobType { get; set; }
        private ItemsViewModel viewModel;
        private UITextField textField;

        public JobDescriptionPickerModel(ItemsViewModel viewModel, string selectedJobType)
        {
            this.viewModel = viewModel;
            this.SelectedJobType = selectedJobType;
        }

        public override void Selected(UIPickerView pickerView, nint row, nint component)
        {
            textField.Text = viewModel.JobDescriptions[SelectedJobType][(int)row];
        }

        public override string GetTitle(UIPickerView pickerView, nint row, nint component)
        {
            return viewModel.JobDescriptions[SelectedJobType][(int)row];
        }

        public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
        {
            return viewModel.JobDescriptions[SelectedJobType].Count;
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
                return DEFAULT_JOB_DESCRIPTION_VALUE;
            }
        }

        public void SetValueChangedView(UIView textField)
        {
            this.textField = (UITextField)textField;
        }
    }
}
