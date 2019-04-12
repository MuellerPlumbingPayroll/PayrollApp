using System;
using UIKit;
using Timecard.Models;

namespace Timecard.iOS.PickerViewModels
{
    /// <summary>
    /// Data source for the job picker.
    /// </summary>
    internal class JobDescriptionPickerModel : UIPickerViewModel, ICustomPickerViewModel
    {
        private static readonly string DEFAULT_JOB_DESCRIPTION_VALUE = "Not Listed";

        private JobType selectedJobType;
        private Job selectedJob;
        private ItemsViewModel viewModel;
        private UITextField textField;

        public JobDescriptionPickerModel(ItemsViewModel viewModel, JobType selectedJobType)
        {
            this.viewModel = viewModel;
            this.selectedJobType = selectedJobType;

            if (viewModel.Jobs[selectedJobType].Count > 0)
                this.selectedJob = viewModel.Jobs[selectedJobType][0];
        }

        public override void Selected(UIPickerView pickerView, nint row, nint component)
        {
            selectedJob = viewModel.Jobs[selectedJobType][(int)row];

            if (textField != null)
                textField.Text = selectedJob.Address;
        }

        public override string GetTitle(UIPickerView pickerView, nint row, nint component)
        {
            return viewModel.Jobs[selectedJobType][(int)row].Address;
        }

        public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
        {
            return viewModel.Jobs[selectedJobType].Count;
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

        public object GetSelectedPickerObject()
        {
            return selectedJob;
        }

        public void SetSelectedPickerObject(object o)
        {
            selectedJob = (Job)o;
        }

        public void SetSelectedJobType(JobType jobType)
        {
            selectedJobType = jobType;

            if (jobType != JobType.Service)
            {
                selectedJob = viewModel.Jobs[jobType][0];
            }
        }

        public int[] GetPickerIndexesToSelect(object o)
        {
            var job = (Job)o;

            if (job == null || selectedJobType == JobType.Service)
            {
                return null;
            }

            for (int i = 0; i < viewModel.Jobs[selectedJobType].Count; i++)
            {
                if (job.Equals(viewModel.Jobs[selectedJobType][i]))
                {
                    return new int[] { i };
                }
            }

            return null;
        }
    }
}
