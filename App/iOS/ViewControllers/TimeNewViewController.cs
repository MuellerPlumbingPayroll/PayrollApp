using System;

using UIKit;
using Timecard.Models;
using System.Drawing;

namespace Timecard.iOS
{
    public partial class TimeNewViewController : UIViewController
    {
        public ItemsViewModel ViewModel { get; set; }
        private UIDatePicker datePicker;
        private UIPickerView jobDescriptionPicker;

        public TimeNewViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            this.ConfigureDatePicker();
            this.ConfigureJobDescriptionPicker();

            jobTypeSegControl.SetTitle(JobTypes.Construction, 0);
            jobTypeSegControl.SetTitle(JobTypes.Service, 1);
            jobTypeSegControl.SetTitle(JobTypes.Other, 2);

 
            this.AddDoneButtonToTextField(txtHoursWorked);

            btnSaveTime.Layer.CornerRadius = 10;
            btnSaveTime.ClipsToBounds = true;

            btnSaveTime.TouchUpInside += (sender, e) =>
            {
                var item = new Item
                {
                    Text = "Testing",
                    JobType = jobTypeSegControl.TitleAt(jobTypeSegControl.SelectedSegment),
                    JobDescription = txtJobDescription.Text,
                    HoursWorked = txtHoursWorked.Text
                };

                var errorMessage = item.CleanAndValidate();
                if (string.IsNullOrWhiteSpace(errorMessage)) // Nothing wrong with item
                {
                    ViewModel.AddItemCommand.Execute(item);
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
        }

        private void ConfigureJobDescriptionPicker()
        {
            jobDescriptionPicker = new UIPickerView
            {
                Model = new JobDescriptionModel(this.ViewModel, txtJobDescription),
                ShowSelectionIndicator = true
            };

            AddDoneButtonToTextField(txtJobDescription);
            txtJobDescription.InputView = jobDescriptionPicker;
        }

        partial void JobTypeSegControl_ValueChanged(UISegmentedControl sender)
        {
            var segmentTitle = sender.TitleAt(sender.SelectedSegment);

            switch (segmentTitle)
            {
                case (JobTypes.Construction):
                    Console.Write("construction");
                    break;
                case (JobTypes.Service):
                    Console.Write("service");
                    break;
                case (JobTypes.Other):
                    Console.Write("other");
                    break;
            }
        }
    }

    class JobDescriptionModel : UIPickerViewModel
    {
        ItemsViewModel viewModel;
        UITextField textField;

        public JobDescriptionModel(ItemsViewModel viewModel, UITextField textField)
        {
            this.viewModel = viewModel;
            this.textField = textField;
        }

        public override void Selected(UIPickerView pickerView, nint row, nint component)
        {
            textField.Text = ProjectSettings.OtherTimeOptions[row];
        }

        public override string GetTitle(UIPickerView pickerView, nint row, nint component)
        {
            return ProjectSettings.OtherTimeOptions[row];
        }

        public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
        {
            return ProjectSettings.OtherTimeOptions.Length;
        }

        public override nint GetComponentCount(UIPickerView pickerView)
        {
            return 1;
        }
    }
}
