namespace Timecard.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        public string UserName { get; set; }

        public HomeViewModel()
        {
            Title = "Home";
            UserName = "Hardcoded User";
        }
    }
}
