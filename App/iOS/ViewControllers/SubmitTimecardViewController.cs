using System;
using Timecard.iOS.PickerViewModels;
using Timecard.iOS.ViewControllers;
using Timecard.ViewModels;
using UIKit;

namespace Timecard.iOS
{
    public partial class SubmitTimecardViewController : BaseViewController
    {
        private readonly SubmitTimecardViewModel _submitTimecardViewModel;

        public SubmitTimecardViewController (IntPtr handle) : base (handle)
        {
            _submitTimecardViewModel = new SubmitTimecardViewModel();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = _submitTimecardViewModel.Title;
            txtConsent.Text = _submitTimecardViewModel.ConsentToSubmitMessage;
            txtWorkInjury.Text = _submitTimecardViewModel.WorkInjuryQuestion;

            var pickerModel = new WorkInjuryPickerModel(_submitTimecardViewModel);
            txtAnswer.AddPickerToTextField(pickerModel);

            btnSubmit.TouchUpInside += BtnSubmit_TouchUpInside;

            AddTapToDismissGesture();
        }

        private async void BtnSubmit_TouchUpInside(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtAnswer.Text))
            {
                uint answer = (uint)txtAnswer.PickerView.SelectedRowInComponent(0);
                bool success = await _submitTimecardViewModel.SubmitTimecardAsync(answer);

                if (success)
                {
                    // Reload the time entries
                    _allItemsViewModel.LoadItemsCommand.Execute(null);
                    NavigationController.PopToRootViewController(true);
                }
                else
                {
                    DisplayErrorMessage("Error encountered while submitting timecard.");
                }
            }
            else
            {
                // The user did not answer the question. Make the border red.
                txtAnswer.Layer.BorderWidth = 4;
                txtAnswer.Layer.BorderColor = UIColor.Red.CGColor;
            }
        }
    }
}
