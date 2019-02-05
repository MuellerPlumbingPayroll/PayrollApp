using Foundation;
using System;
using UIKit;
using Timecard.Models;

namespace Timecard.iOS
{
    public partial class HistoryTableViewSectionHeader : UITableViewCell
    {
        public static readonly string REUSE_IDENTIFIER = "SECTION_CELL";

        public HistoryTableViewSectionHeader (IntPtr handle) : base (handle)
        {
            this.BackgroundColor = UIColor.LightGray;
        }

        public void UpdateHeader(DateTime date, float hoursWorked)
        {
            // Set both label text colors to white
            txtDate.TextColor = UIColor.White;
            txtHoursWorked.TextColor = UIColor.White;

            txtDate.Text = date.ToString(ProjectSettings.DateFormat);
            txtHoursWorked.Text = hoursWorked + " hrs";

            if (hoursWorked > ProjectSettings.NumberHoursInWorkDay)
            {
                // If there are more than a normal number of hours entered on
                // this day, then set the text color to red. This doesn't 
                // necessarily mean that there's an error (it could be overtime),
                // it just draws attention in case there is one.
                txtHoursWorked.TextColor = UIColor.Red;
            }
        }
    }
}