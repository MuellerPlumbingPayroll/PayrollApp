using System;
using System.Drawing;
using System.Globalization;
using CoreLocation;
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
        private UIPickerView costCodePicker;
        private UIPickerView jobDescriptionPicker;
        private JobDescriptionModel jobDescriptionModel;

        public TimeNewViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Title = EditingItem == null ? "New Entry" : "Editing Entry";

            ConfigureDatePicker();
            ConfigureJobDescriptionPicker();
            ConfigureCostCodePicker();
            ConfigureSaveButton();

            jobTypeSegControl.SetTitle(JobType.Construction, 0);
            jobTypeSegControl.SetTitle(JobType.Service, 1);
            jobTypeSegControl.SetTitle(JobType.Other, 2);
 
            AddDoneButtonToTextField(txtHoursWorked);

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
            var swipeLeft = new UISwipeGestureRecognizer((s) =>
            {
                if (jobTypeSegControl.SelectedSegment == 0)
                    jobTypeSegControl.SelectedSegment = jobTypeSegControl.NumberOfSegments - 1;
                else
                    jobTypeSegControl.SelectedSegment -= 1;

                JobTypeSegControl_ValueChanged(jobTypeSegControl);
            })
            {
                Direction = UISwipeGestureRecognizerDirection.Left
            };

            var swipeRight = new UISwipeGestureRecognizer((s) =>
            {
                if (jobTypeSegControl.SelectedSegment == jobTypeSegControl.NumberOfSegments - 1)
                    jobTypeSegControl.SelectedSegment = 0;
                else
                    jobTypeSegControl.SelectedSegment += 1;

                JobTypeSegControl_ValueChanged(jobTypeSegControl);
            })
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
                txtHoursWorked.Text = EditingItem.HoursWorked;
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
                    JobDate = DateTime.ParseExact(txtDateField.Text, ProjectSettings.DateFormat,
                        CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal),
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

                var errorMessage = item.CleanAndValidate();
                if (string.IsNullOrWhiteSpace(errorMessage)) // Nothing wrong with the item
                {
                    if (EditingItem == null)
                    {
                        ViewModel.AddItemCommand.Execute(item);
                    }
                    else
                    {
                        ViewModel.UpdateItemCommand.Execute(item);
                    }

                    NavigationController.PopToRootViewController(true);
                }
                else
                {
                    DisplayAlertMessage(errorMessage);  
                }
            };
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
                MinimumDate = (Foundation.NSDate)ProjectSettings.GetStartOfCurrentPayPeriod(),
                MaximumDate = (Foundation.NSDate)ProjectSettings.GetEndOfCurrentPayPeriod()
            };
            
            // Whenever the date changes, set the date text field to the value of the picker
            datePicker.ValueChanged += (sender, e) =>
            {
                txtDateField.Text = ((DateTime)datePicker.Date).ToString(ProjectSettings.DateFormat);
            };

            AddDoneButtonToTextField(txtDateField);
            txtDateField.Text = DateTime.Now.ToString(ProjectSettings.DateFormat);
            txtDateField.InputView = datePicker;

            if (EditingItem != null)
            {
                // If the user is editing this entry, set the picker to the previously selected date
                txtDateField.Text = EditingItem.JobDate.ToString(ProjectSettings.DateFormat);
                datePicker.SetDate((Foundation.NSDate)EditingItem.JobDate, false);
            }
        }

        private void ConfigureJobDescriptionPicker()
        {
            var selectedJobType = EditingItem != null ? EditingItem.JobType : JobType.Construction;

            jobDescriptionModel = new JobDescriptionModel(ViewModel, txtJobDescription, selectedJobType);
            jobDescriptionPicker = new UIPickerView
            {
                Model = jobDescriptionModel
            };

            AddDoneButtonToTextField(txtJobDescription);
            txtJobDescription.InputView = jobDescriptionPicker;

            txtJobDescription.EditingDidBegin += (sender, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtJobDescription.Text))
                {
                    // Try because getting the title may fail
                    try
                    {
                        var model = jobDescriptionPicker.Model;
                        txtJobDescription.Text = model.GetTitle(jobDescriptionPicker, 0, 0);
                    } catch { }
                }
            };

            if (EditingItem != null)
            {
                if (EditingItem.JobType == JobType.Service)
                {
                    txtJobDescription.InputView = null;
                }

                txtJobDescription.Text = EditingItem.JobDescription;
            }
        }

        private void ConfigureCostCodePicker()
        {
            costCodePicker = new UIPickerView
            {
                Model = new CostCodeModel(ViewModel, txtCostCode)
            };

            AddDoneButtonToTextField(txtCostCode);
            txtCostCode.InputView = costCodePicker;

            txtCostCode.EditingDidBegin += (sender, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtCostCode.Text))
                {
                    // Try because getting the title may fail
                    try
                    {
                        var model = costCodePicker.Model;
                        txtCostCode.Text = model.GetTitle(costCodePicker, 0, 0);
                    } catch { }
                }
            };

            if (EditingItem != null)
            {
                txtCostCode.Text = EditingItem.CostCode;
            }
        }

        /// <summary>
        /// By default, the keyboard or view that pops up when selecting a 
        // text field does not have a done button or way to dismiss the view.
        /// This method adds a bar with a done button to the specified text field.
        /// </summary>
        /// <param name="textField">Text field.</param>
        private void AddDoneButtonToTextField(UITextField textField)
        {
            var toolbar = new UIToolbar(new RectangleF(0.0f, 0.0f, 50.0f, 44.0f));
            var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate
            {
                textField.ResignFirstResponder();
            });

            toolbar.Items = new UIBarButtonItem[] {
                new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
                doneButton
            };

            textField.InputAccessoryView = toolbar;
        }

        /* Event Handlers */

        partial void JobTypeSegControl_ValueChanged(UISegmentedControl sender)
        {
            // Hide the picker or keyboard that's currently on the screen
            View.EndEditing(true);

            var segmentTitle = sender.TitleAt(sender.SelectedSegment);

            txtJobDescription.InputView = segmentTitle == JobType.Service ? null : jobDescriptionPicker;
            txtJobDescription.Text = string.Empty;

            txtCostCode.Hidden = segmentTitle == JobType.Other;

            jobDescriptionModel.SelectedJobType = segmentTitle;
            jobDescriptionPicker.ReloadAllComponents();
        }
    }


    /// <summary>
    /// Data source for the job picker.
    /// </summary>
    class JobDescriptionModel : UIPickerViewModel
    {
        public string SelectedJobType { get; set; }
        private ItemsViewModel viewModel;
        private UITextField textField;

        public JobDescriptionModel(ItemsViewModel viewModel, UITextField textField, string selectedJobType)
        {
            this.viewModel = viewModel;
            this.textField = textField;
            this.SelectedJobType = selectedJobType;
        }

        public override void Selected(UIPickerView pickerView, nint row, nint component)
        {
            textField.Text = viewModel.JobDescriptions[SelectedJobType][(int) row];
        }

        public override string GetTitle(UIPickerView pickerView, nint row, nint component)
        {
            return viewModel.JobDescriptions[SelectedJobType][(int) row];
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


    /// <summary>
    /// Data source for cost code picker.
    /// </summary>
    class CostCodeModel : UIPickerViewModel
    {
        private ItemsViewModel viewModel;
        private UITextField textField;

        public CostCodeModel(ItemsViewModel viewModel, UITextField textField)
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
