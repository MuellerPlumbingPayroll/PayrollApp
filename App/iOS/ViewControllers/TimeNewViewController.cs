using System;
using System.Drawing;
using System.Globalization;
using CoreLocation;
using Timecard.Exceptions;
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
        private JobDescriptionPickerModel jobDescriptionModel;

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

            jobTypeSegControl.SetTitle(JobType.Construction, 0);
            jobTypeSegControl.SetTitle(JobType.Service, 1);
            jobTypeSegControl.SetTitle(JobType.Other, 2);
 
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

            btnSaveTime.TouchUpInside += (sender, e) =>
            {
                CLLocationCoordinate2D location;
                try
                {
                    location = locationManager.GetUserLocation();
                }
                catch (LocationNotAuthorizedException ex)
                {
                    DisplayAlertMessage(string.Format("Error saving time entry: {0}", ex.Message));
                    return;
                }

                var item = new Item
                {
                    JobDate = txtDateField.Text,
                    JobDescription = txtJobDescription.Text,
                    HoursWorked = txtHoursWorked.Text,
                    CostCode = txtCostCode.Text
                };


                if (EditingItem == null) // Creating a new item
                {
                    item.JobType = jobTypeSegControl.TitleAt(jobTypeSegControl.SelectedSegment);
                    item.LatitudeCreated = location.Latitude.ToString();
                    item.LongitudeCreated = location.Longitude.ToString();
                }
                else // Editing an existing item
                {
                    item.Id = EditingItem.Id;
                    item.JobType = EditingItem.JobType;
                    item.LatitudeUpdated = location.Latitude.ToString();
                    item.LongitudeUpdated = location.Longitude.ToString();
                }

                try
                {
                    item.Clean();
                    if (EditingItem == null)
                        ViewModel.AddItemCommand.Execute(item);
                    else
                        ViewModel.UpdateItemCommand.Execute(item);
                }
                catch (InvalidItemException ex)
                {
                    DisplayAlertMessage(ex.Message);
                    return;
                }

                base.NavigationController.PopToRootViewController(true);
            };
        }

        private void DisplayAlertMessage(string message)
        {
            var alert = UIAlertController.Create("Error", message, UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create("Okay", UIAlertActionStyle.Cancel, null));

            PresentViewController(alert, animated: true, completionHandler: null);
        }

        private void ConfigureHoursWorkedPicker()
        {
            if (EditingItem != null)
            {
                var time = string.Format($"{EditingItem.GetHoursWorkedHoursPart()}:{EditingItem.GetHoursWorkedMinutesPart()}");
                txtHoursWorked.Text = time;
            }
        }

        private void ConfigureDatePicker()
        {
            datePicker = new UIDatePicker
            {
                Mode = UIDatePickerMode.Date,
                MinimumDate = (Foundation.NSDate)ProjectSettings.GetStartOfCurrentPayPeriod(),
                MaximumDate = (Foundation.NSDate)ProjectSettings.GetEndOfCurrentPayPeriod()
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
                txtDateField.Text = EditingItem.JobDate;
                datePicker.SetDate((Foundation.NSDate)ProjectSettings.LocalDateFromString(EditingItem.JobDate), false);
   
            }
        }

        private void ConfigureJobDescriptionPicker()
        {
            var selectedJobType = EditingItem != null ? EditingItem.JobType : JobType.Construction;

            jobDescriptionModel = new JobDescriptionPickerModel(ViewModel, selectedJobType);
            txtJobDescription.AddPickerToTextField(jobDescriptionModel);

            if (EditingItem != null)
            {
                if (EditingItem.JobType == JobType.Service)
                {
                    txtJobDescription.SetPickerActive(false);
                }
                txtJobDescription.Text = EditingItem.JobDescription;
            }
        }

        private void ConfigureCostCodePicker()
        {
            var model = new CostCodePickerModel(ViewModel);
            txtCostCode.AddPickerToTextField(model);

            if (EditingItem != null)
                txtCostCode.Text = EditingItem.CostCode;
        }

        /***** Event Handlers *****/

        public void HandleSwipeGesture(UISwipeGestureRecognizerDirection direction)
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

            var segmentTitle = sender.TitleAt(sender.SelectedSegment);
            jobDescriptionModel.SelectedJobType = segmentTitle;

            txtJobDescription.Text = string.Empty;
            txtJobDescription.SetPickerActive(segmentTitle != JobType.Service);

            // Cost code is not needed for "Other" job type so hide it
            txtCostCode.Hidden = segmentTitle == JobType.Other;
        }
    }
}
