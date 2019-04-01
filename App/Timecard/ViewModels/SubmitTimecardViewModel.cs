using System.Threading.Tasks;
using Timecard.Models;

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

        public async Task<bool> SubmitTimecardAsync(uint selectedPickerOptionIndex)
        {
            bool injured = selectedPickerOptionIndex == 1;
            var timecardSubmission = new TimecardSubmission
            {
                Injured = injured
            };

            bool success = await DataStore.SubmitTimecardAsync(timecardSubmission);

            if (success)
                System.Diagnostics.Debug.WriteLine("Successfully submitted timecard.");
            else
                System.Diagnostics.Debug.WriteLine("Failed to submit timecard.");

            return success;
        }
    }
}
