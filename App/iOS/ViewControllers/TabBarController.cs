using System;
using UIKit;

namespace Timecard.iOS
{
    public partial class TabBarController : UITabBarController
    {
        public TabBarController(IntPtr handle) : base(handle)
        {
            TabBar.Items[0].Title = "Home";
            TabBar.Items[0].AccessibilityLabel = "tabHome";

            TabBar.Items[1].Title = "History";
            TabBar.Items[1].AccessibilityLabel = "tabHistory";
        }
    }
}
