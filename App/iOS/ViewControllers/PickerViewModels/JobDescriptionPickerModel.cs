using System;
using UIKit;

namespace Timecard.iOS.ViewControllers.PickerViewModels
{
    /// <summary>
    /// Data source for the job picker.
    /// </summary>
    class JobDescriptionPickerModel : UIPickerViewModel
    {
        public string SelectedJobType { get; set; }
        private ItemsViewModel viewModel;
        private UITextField textField;

        public JobDescriptionPickerModel(ItemsViewModel viewModel, UITextField textField, string selectedJobType)
        {
            this.viewModel = viewModel;
            this.textField = textField;
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
    }
}
