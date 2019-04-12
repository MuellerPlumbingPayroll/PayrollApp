namespace Timecard.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        public string UserName { get; set; }
        public string NeedToSubmitTimecardWarningMessage { get; }

        public HomeViewModel(string userName = "Employee")
        {
            Title = "Home";
            UserName = userName;

            NeedToSubmitTimecardWarningMessage = "You have not submitted your timecard " +
                                         "for the previous pay period. " +
                                         "Please submit to add entries for the current period.";
        }
    }
}
