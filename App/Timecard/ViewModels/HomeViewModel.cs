using System;
namespace Timecard.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        public string UserName { get; set; }
        public ItemsViewModel ViewModel { get; set; }

        public HomeViewModel()
        {
            Title = "Home";
            UserName = "Hardcoded User";

            ViewModel = new ItemsViewModel();
        }
        
        public string NumberHoursWorkedToday()
        {
            return "Nah g";  //ViewModel.NumberHoursWorkedToday();
        }
    }
}
