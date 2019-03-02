namespace Timecard.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        public string UserName { get; set; }

        public HomeViewModel(string userName = "User")
        {
            Title = "Home";
            UserName = userName;
        }
    }
}
