using System;

using UIKit;
using Timecard.Models;
using System.Drawing;

namespace Timecard.iOS
{
    public partial class TimeNewViewController : UIViewController
    {
        public ItemsViewModel ViewModel { get; set; }
        public Item EditingItem { get; internal set; } = null;

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

            // When user taps outside of a picker or keyboard, it disappears
            var gestureRecognizer = new UITapGestureRecognizer(() => View.EndEditing(true));
            View.AddGestureRecognizer(gestureRecognizer);

            ConfigureEditing();
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
                Item item;

                if (EditingItem == null) // Creating a new item
                {
                    item = new Item
                    {
                        JobDate = txtDateField.Text,
                        JobType = jobTypeSegControl.TitleAt(jobTypeSegControl.SelectedSegment),
                        JobDescription = txtJobDescription.Text,
                        HoursWorked = txtHoursWorked.Text,
                        CostCode = txtCostCode.Text
                    };
                }
                else // Editing an existing item
                {
                    item = new Item
                    {
                        Id = EditingItem.Id,
                        JobDate = txtDateField.Text,
                        JobType = EditingItem.JobType,
                        JobDescription = txtJobDescription.Text,
                        HoursWorked = txtHoursWorked.Text,
                        CostCode = txtCostCode.Text
                    };
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
                    var alert = UIAlertController.Create("Error", errorMessage, UIAlertControllerStyle.Alert);
                    alert.AddAction(UIAlertAction.Create("Okay", UIAlertActionStyle.Cancel, null));

                    PresentViewController(alert, animated: true, completionHandler: null);
                }
            };
        }

        private void ConfigureDatePicker()
        {
            datePicker = new UIDatePicker
            {
                Mode = UIDatePickerMode.Date,
                MinimumDate = (Foundation.NSDate)DateTime.UtcNow.Date,
                MaximumDate = (Foundation.NSDate)DateTime.UtcNow.Date.AddDays(7),
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
                txtDateField.Text = EditingItem.JobDate;
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

            };

            AddDoneButtonToTextField(txtCostCode);
            txtCostCode.InputView = costCodePicker;

            if (EditingItem != null)
            {
                txtCostCode.Text = EditingItem.CostCode;
            }
        }

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
}
