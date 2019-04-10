using System;
using CoreLocation;
using Foundation;
using Timecard.iOS.PickerViewModels;
using Timecard.Models;
using UIKit;

namespace Timecard.iOS
{
    public partial class TimeNewViewController : BaseViewController
    {
        public Item EditingItem { get; internal set; } = null;

        private LocationManager locationManager;
        private UIDatePicker datePicker;
        private JobDescriptionPickerModel jobDescriptionPickerModel;
        private CostCodePickerModel costCodeModel;

        public TimeNewViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Title = EditingItem == null ? "New Entry" : "Editing Entry";

            // Refresh the jobs and cost codes
            AllItemsViewModel.LoadJobsCommand.Execute(null);
            AllItemsViewModel.LoadCostCodesCommand.Execute(null);

            ConfigureDatePicker();
            ConfigureHoursWorkedPicker();
            ConfigureJobDescriptionPicker();
            ConfigureCostCodePicker();
           
            btnSaveTime.TouchUpInside += OnSaveButtonClicked;

            jobTypeSegControl.SetTitle(JobType.Construction.ToString(), 0);
            jobTypeSegControl.SetTitle(JobType.Service.ToString(), 1);
            jobTypeSegControl.SetTitle(JobType.Other.ToString(), 2);

            ConfigureGestures();
            ConfigureEditing();

            locationManager = new LocationManager();
            locationManager.EnableLocationServices();
        }

        private void ConfigureGestures()
        {
            AddTapToDismissGesture();

            // When the user swipes left or right, change the value of the selected job type
            var swipeLeft = new UISwipeGestureRecognizer((s) => HandleSwipeGesture(UISwipeGestureRecognizerDirection.Left))
            {
                Direction = UISwipeGestureRecognizerDirection.Left
            };
            
            var swipeRight = new UISwipeGestureRecognizer((s) => HandleSwipeGesture(UISwipeGestureRecognizerDirection.Right))
            {
                Direction = UISwipeGestureRecognizerDirection.Right
            };

            // Only add the swipe gestures if the user is editing an item
            // Users are not allowed to change the job type when editing
            if (EditingItem == null)
            {
                View.AddGestureRecognizer(swipeLeft);
                View.AddGestureRecognizer(swipeRight);
            }
        }

        private void ConfigureEditing()
        {
            if (EditingItem != null)
            {
                // Prevent user from being able to change the job type
                jobTypeSegControl.Hidden = true;
                txtCostCode.Hidden = EditingItem.JobType == JobType.Other;
            }
        }

        private void ConfigureDatePicker()
        {
            datePicker = new UIDatePicker
            {
                Mode = UIDatePickerMode.Date,
                Date = (NSDate)AllItemsViewModel.GetInitialDate(),
                MinimumDate = (NSDate)AllItemsViewModel.GetStartOfPayPeriod(),
                MaximumDate = (NSDate)AllItemsViewModel.GetEndOfPayPeriod(),
                TimeZone = NSTimeZone.LocalTimeZone
            };

            // Whenever the date changes, set the date text field to the value of the picker
            datePicker.ValueChanged += (sender, e) =>
            {
                txtDateField.Text = ((DateTime)datePicker.Date).ToLocalTime().ToString(ProjectSettings.DateFormat);
            };

            txtDateField.Text = ((DateTime)datePicker.Date).ToLocalTime().ToString(ProjectSettings.DateFormat);
            txtDateField.InputView = datePicker;

            if (EditingItem != null)
            {
                // If the user is editing this entry, set the picker to the previously selected date
                txtDateField.Text = EditingItem.JobDate.ToLocalTime().ToString(ProjectSettings.DateFormat);
                datePicker.SetDate((Foundation.NSDate)EditingItem.JobDate, false);
   
            }
        }

        private void ConfigureHoursWorkedPicker()
        {
            var model = new HoursWorkedPickerModel();

            txtHoursWorked.AddPickerToTextField(model);
            txtHoursWorked.PickerView.Select(2, 0, true);
            txtHoursWorked.PickerView.Select(1, 1, true);

            if (EditingItem != null)
            {
                txtHoursWorked.SetSelectedPickerObject(EditingItem.TimeWorked);
                txtHoursWorked.Text = EditingItem.TimeWorked.ToColonFormat();
            }
        }

        private void ConfigureJobDescriptionPicker()
        {
            var selectedJobType = EditingItem != null ? EditingItem.JobType : JobType.Construction;

            jobDescriptionPickerModel = new JobDescriptionPickerModel(AllItemsViewModel, selectedJobType);
            txtJobDescription.AddPickerToTextField(jobDescriptionPickerModel);

            if (EditingItem != null)
            {
                if (EditingItem.JobType == JobType.Service)
                {
                    txtJobDescription.SetPickerActive(false);
                }

                txtJobDescription.SetSelectedPickerObject(EditingItem.Job);
                txtJobDescription.Text = EditingItem.Job.Address;
            }
        }

        private void ConfigureCostCodePicker()
        {
            var selectedJobType = EditingItem != null ? EditingItem.JobType : JobType.Construction;

            costCodeModel = new CostCodePickerModel(AllItemsViewModel, selectedJobType);

            if (EditingItem != null && selectedJobType != JobType.Other)
            {
                costCodeModel.SetSelectedPickerObject(EditingItem.CostCode);
                txtCostCode.Text = EditingItem.CostCode.Description;
            }

            txtCostCode.AddPickerToTextField(costCodeModel);
        }

        /*************************** Event Handlers ***************************/

        private void HandleSwipeGesture(UISwipeGestureRecognizerDirection direction)
        {
            switch (direction)
            {
                case UISwipeGestureRecognizerDirection.Left:
                    if (jobTypeSegControl.SelectedSegment == 0)
                        jobTypeSegControl.SelectedSegment = jobTypeSegControl.NumberOfSegments - 1;
                    else
                        jobTypeSegControl.SelectedSegment -= 1;
                    break;
                case UISwipeGestureRecognizerDirection.Right:
                    if (jobTypeSegControl.SelectedSegment == jobTypeSegControl.NumberOfSegments - 1)
                        jobTypeSegControl.SelectedSegment = 0;
                    else
                        jobTypeSegControl.SelectedSegment += 1;
                    break;
            }

            JobTypeSegControl_ValueChanged(jobTypeSegControl);
        }

        partial void JobTypeSegControl_ValueChanged(UISegmentedControl sender)
        {
            // Hide the picker or keyboard that's currently on the screen
            View.EndEditing(true);

            string segmentTitle = sender.TitleAt(sender.SelectedSegment);
            JobType segmentJobType = (JobType)Enum.Parse(typeof(JobType), segmentTitle);

            jobDescriptionPickerModel.SetSelectedJobType(segmentJobType);
            costCodeModel.SetSelectedJobType(segmentJobType);

            // Cost code is only needed for the non-other tabs
            txtCostCode.SetPickerActive(active: segmentJobType != JobType.Other,
                                        fieldVisible: segmentJobType != JobType.Other,
                                        clearText: true);

            // Job description should not have a picker if on the service tab
            txtJobDescription.SetPickerActive(active: segmentJobType != JobType.Service,
                                              fieldVisible: true,
                                              clearText: true);
        }

        private void OnSaveButtonClicked(object sender, EventArgs e)
        {
            DisplayLoadingIndicator();

            var item = new Item
            {
                CostCode = (CostCode)txtCostCode.GetSelectedPickerObject(),
                Job = (Job)txtJobDescription.GetSelectedPickerObject(),
                JobDate = ((DateTime)datePicker.Date).ToLocalTime(),
                TimeWorked = (TimeWorked)txtHoursWorked.GetSelectedPickerObject()
            };

            if (EditingItem == null)
            {
                SaveNew(item);
            }
            else
            {
                SaveExisting(item);
            }
        }

        private async void SaveNew(Item item)
        {
            item.JobType = (JobType)(int)jobTypeSegControl.SelectedSegment;
            if (item.JobType == JobType.Service)
            {
                item.Job = new Job
                {
                    Address = txtJobDescription.Text,
                    ClientName = txtJobDescription.Text
                };
            }

            try
            {
                // Attempt to get the user's location. Location services might be disabled.
                item.Latitude = locationManager.GetLatitude();
                item.Longitude = locationManager.GetLongitude();
            }
            catch (LocationNotAuthorizedException ex)
            {
                DisplayErrorMessage(string.Format("Error saving time entry: {0}", ex.Message));
                return;
            }

            try
            {
                CheckForEmptyTextFields(item.JobType);

                bool success = await AllItemsViewModel.AddItem(item);
                if (success)
                {
                    base.NavigationController.PopToRootViewController(true);
                }
                else
                {
                    RemoveLoadingIndicator();
                    DisplayErrorMessage("Failed to create new time entry.");
                }
            }
            catch (InvalidOperationException ex)
            {
                RemoveLoadingIndicator();
                DisplayErrorMessage(ex.Message);
            }
        }

        private async void SaveExisting(Item item)
        {
            item.Id = EditingItem.Id;
            item.JobType = EditingItem.JobType;
            if (item.JobType == JobType.Service)
            {
                item.Job = new Job
                {
                    Address = txtJobDescription.Text,
                    ClientName = txtJobDescription.Text
                };
            }

            try
            {
                CheckForEmptyTextFields(item.JobType);

                bool success = await AllItemsViewModel.UpdateItem(item);
                if (success)
                {
                    base.NavigationController.PopToRootViewController(true);
                }
                else
                {
                    RemoveLoadingIndicator();
                    DisplayErrorMessage("Failed to save time entry.");
                }
            }
            catch (InvalidOperationException ex)
            {
                RemoveLoadingIndicator();
                DisplayErrorMessage(ex.Message);
            }
        }

        private void CheckForEmptyTextFields(JobType selectedJobType)
        {
            if (string.IsNullOrWhiteSpace(txtDateField.Text))
                throw new InvalidOperationException("Job date is required.");
            if (string.IsNullOrWhiteSpace(txtHoursWorked.Text))
                throw new InvalidOperationException("Hours worked is required.");
            if (string.IsNullOrWhiteSpace(txtJobDescription.Text))
                throw new InvalidOperationException("Job description is required.");
            if (string.IsNullOrWhiteSpace(txtCostCode.Text) && selectedJobType != JobType.Other)
                throw new InvalidOperationException("Cost code is required.");
        }
    }
}
