using System;
using CoreLocation;
using Foundation;
using Timecard.iOS.ViewControllers.PickerViewModels;
using Timecard.Models;
using UIKit;

namespace Timecard.iOS
{
    public partial class TimeNewViewController : UIViewController
    {
        public ItemsViewModel ViewModel { get; set; }
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

            ConfigureDatePicker();
            ConfigureHoursWorkedPicker();
            ConfigureJobDescriptionPicker();
            ConfigureCostCodePicker();
            ConfigureSaveButton();

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
            // When user taps outside of a picker or keyboard, it disappears
            var tapGesture = new UITapGestureRecognizer(() => View.EndEditing(true));
            View.AddGestureRecognizer(tapGesture);

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

        private void ConfigureSaveButton()
        {
            // Round the button's corners
            btnSaveTime.Layer.CornerRadius = 10;
            btnSaveTime.ClipsToBounds = true;

            btnSaveTime.TouchUpInside += OnSaveButtonClicked;
        }

        private void DisplayAlertMessage(string message)
        {
            var alert = UIAlertController.Create("Error", message, UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create("Okay", UIAlertActionStyle.Cancel, null));

            PresentViewController(alert, animated: true, completionHandler: null);
        }

        private void ConfigureDatePicker()
        {
            datePicker = new UIDatePicker
            {
                Mode = UIDatePickerMode.Date,
                MinimumDate = (NSDate)ProjectSettings.GetStartOfCurrentPayPeriod(),
                MaximumDate = (NSDate)ProjectSettings.GetEndOfCurrentPayPeriod(),
                TimeZone = NSTimeZone.LocalTimeZone
            };
            
            // Whenever the date changes, set the date text field to the value of the picker
            datePicker.ValueChanged += (sender, e) =>
            {
                txtDateField.Text = ((DateTime)datePicker.Date).ToString(ProjectSettings.DateFormat);
            };

            txtDateField.Text = DateTime.Now.ToString(ProjectSettings.DateFormat);
            txtDateField.InputView = datePicker;

            if (EditingItem != null)
            {
                // If the user is editing this entry, set the picker to the previously selected date
                txtDateField.Text = EditingItem.JobDate.ToString(ProjectSettings.DateFormat);
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

            jobDescriptionPickerModel = new JobDescriptionPickerModel(ViewModel, selectedJobType);
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

            costCodeModel = new CostCodePickerModel(ViewModel, selectedJobType);

            if (EditingItem != null)
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
            var item = new Item
            {
                CostCode = (CostCode)txtCostCode.GetSelectedPickerObject(),
                JobDate = (DateTime)datePicker.Date,
                TimeWorked = (TimeWorked)txtHoursWorked.GetSelectedPickerObject()
            };

            if (EditingItem != null)
            {
                try
                {
                    // Attempt to get the user's location
                    CLLocationCoordinate2D location = locationManager.GetUserLocation();
                    item.Latitude = location.Latitude.ToString();
                    item.Longitude = location.Longitude.ToString();
                }
                catch (LocationNotAuthorizedException ex)
                {
                    DisplayAlertMessage(string.Format("Error saving time entry: {0}", ex.Message));
                    return;
                }
            }

            if (EditingItem == null) // Creating a new item
            {
                item.JobType = (JobType)(int)jobTypeSegControl.SelectedSegment;
            }
            else // Editing an existing item
            {
                item.Id = EditingItem.Id;
                item.JobType = EditingItem.JobType;
            }

            // Service jobs are entered via a number pad
            if (item.JobType == JobType.Service)
            {
                item.Job = new Job()
                {
                    Address = txtJobDescription.Text,
                    ClientName = txtJobDescription.Text
                };
            }
            else
            {
                item.Job = (Job)txtJobDescription.GetSelectedPickerObject();
            }

            try
            {
                CheckForEmptyTextFields(item.JobType);
                if (EditingItem == null)
                    ViewModel.AddItemCommand.Execute(item);
                else
                    ViewModel.UpdateItemCommand.Execute(item);
            }
            catch (InvalidOperationException ex)
            {
                DisplayAlertMessage(ex.Message);
                return;
            }

            base.NavigationController.PopToRootViewController(true);
        }

        private void CheckForEmptyTextFields(JobType selectedJobType)
        {
            if (string.IsNullOrWhiteSpace(txtDateField.Text))
                throw new InvalidOperationException("Job date is required.");
            if (string.IsNullOrWhiteSpace(txtHoursWorked.Text))
                throw new InvalidOperationException("Hours worked is required.");
            if (string.IsNullOrWhiteSpace(txtJobDescription.Text))
                throw new InvalidOperationException("Job description is required.");
            if (string.IsNullOrWhiteSpace(txtDateField.Text) && selectedJobType != JobType.Other)
                throw new InvalidOperationException("Cost code is required.");
        }
    }
}
