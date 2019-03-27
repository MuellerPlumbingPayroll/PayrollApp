namespace Timecard.ViewModels
{
    public class SubmitTimecardViewModel : BaseViewModel
    {
        public string ConsentToSubmitMessage { get; }
        public string WorkInjuryQuestion { get; }
        public string[] WorkInjuryPickerOptions { get; }

        public SubmitTimecardViewModel()
        {
            Title = "Submit Timecard";

            ConsentToSubmitMessage = "By clicking submit, you confirm that the information " +
                "you have entered is truthful.";
            WorkInjuryQuestion = "Did you have a work-related injury this week?";
            WorkInjuryPickerOptions = new string[] { "No", "Yes" };
        }

        public void SubmitTimecard(uint selectedPickerOptionIndex)
        {

        }
    }
}
