using System;
using Timecard.Models;
using UIKit;

namespace Timecard.iOS
{
    public partial class HistoryTableViewSectionHeader : UITableViewCell
    {
        public static readonly string REUSE_IDENTIFIER = "SECTION_CELL";

        public HistoryTableViewSectionHeader (IntPtr handle) : base (handle)
        {
            BackgroundColor = UIColor.DarkGray;
        }

        internal void UpdateHeader(DateTime date, float hoursWorked)
        {
            // Set both label text colors to white
            txtDate.TextColor = UIColor.White;
            txtHoursWorked.TextColor = UIColor.White;

            // Prevent text from accidentally overflowing either of the labels
            txtDate.AdjustsFontSizeToFitWidth = true;
            txtHoursWorked.AdjustsFontSizeToFitWidth = true;

            txtDate.Text = date.ToString(ProjectSettings.DateFormat);
            txtHoursWorked.Text = hoursWorked + " hrs";

            // If there are more than a normal number of hours entered on this day,
            // then set the warning image should be visible.
            // This doesn't necessarily mean that there's an error, 
            // but instead draws attention in case there is one.
            imgWarning.Hidden = hoursWorked <= ProjectSettings.NumberHoursInWorkDay;
        }
    }
}