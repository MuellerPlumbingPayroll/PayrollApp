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
        private JobType _selectedJobType;
        private Job _selectedJob;
        private ItemsViewModel _viewModel;
        private UITextField _textField;

        public JobDescriptionPickerModel(ItemsViewModel viewModel, JobType selectedJobType)
        {
            _viewModel = viewModel;
            _selectedJobType = selectedJobType;

            if (_viewModel.Jobs.ContainsKey(selectedJobType) && _viewModel.Jobs[selectedJobType].Count > 0)
                _selectedJob = viewModel.Jobs[selectedJobType][0];
        }

        public override void Selected(UIPickerView pickerView, nint row, nint component)
        {
            _selectedJob = _viewModel.Jobs[_selectedJobType][(int)row];

            if (_textField != null)
                _textField.Text = _selectedJob.Address;
        }

        public override string GetTitle(UIPickerView pickerView, nint row, nint component)
        {
            if (_viewModel.Jobs.ContainsKey(_selectedJobType) && _viewModel.Jobs[_selectedJobType].Count > 0)
                return _viewModel.Jobs[_selectedJobType][(int)row].Address;

            return string.Empty;
        }

        public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
        {
            if (_viewModel.Jobs.ContainsKey(_selectedJobType))
                return _viewModel.Jobs[_selectedJobType].Count;

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
            return _selectedJob;
        }

        public void SetSelectedPickerObject(object o)
        {
            _selectedJob = (Job)o;
        }

        public void SetSelectedJobType(JobType jobType)
        {
            _selectedJobType = jobType;

            if (jobType != JobType.Service && _viewModel.Jobs.ContainsKey(jobType) 
                && _viewModel.Jobs[jobType].Count > 0)
            {
                _selectedJob = _viewModel.Jobs[jobType][0];
            }
        }

        public int[] GetPickerIndexesToSelect(object o)
        {
            var job = (Job)o;

            if (job == null || _selectedJobType == JobType.Service)
            {
                return null;
            }

            for (int i = 0; i < _viewModel.Jobs[_selectedJobType].Count; i++)
            {
                if (job.Equals(_viewModel.Jobs[_selectedJobType][i]))
                {
                    return new int[] { i };
                }
            }

            return null;
        }
    }
}
