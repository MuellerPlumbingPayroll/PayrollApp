using System;
using UIKit;

namespace Timecard.iOS
{
    public partial class TabBarController : UITabBarController
    {
        public ItemsViewModel AllItemsViewModel { get; } = new ItemsViewModel();

        public TabBarController(IntPtr handle) : base(handle)
        {
            TabBar.Items[0].Title = "Home";
            TabBar.Items[1].Title = "History";
        }
    }
}
