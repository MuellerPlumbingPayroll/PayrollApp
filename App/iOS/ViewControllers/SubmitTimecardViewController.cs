using System;
using Timecard.ViewModels;
using Timecard.iOS.PickerViewModels;
using UIKit;

namespace Timecard.iOS
{
    public partial class SubmitTimecardViewController : UIViewController
    {
        private SubmitTimecardViewModel _submitTimecardViewModel;

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

            btnSubmit.Layer.CornerRadius = 10;
            btnSubmit.ClipsToBounds = true;
        }

        async partial void BtnSubmit_TouchUpInsideAsync(UIButton sender)
        {
            if (!string.IsNullOrWhiteSpace(txtAnswer.Text))
            {
                uint answer = (uint)txtAnswer.PickerView.SelectedRowInComponent(0);
                bool success = await _submitTimecardViewModel.SubmitTimecardAsync(answer);
                if (success)
                    NavigationController.PopToRootViewController(true);
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
