using System;
using UIKit;

namespace Timecard.iOS
{
    public partial class HistoryTableViewCell : UITableViewCell
    {
        public static readonly string REUSE_IDENTIFIER = "HISTORY_CELL";

        public HistoryTableViewCell (IntPtr handle) : base (handle)
        {
        }

        internal void UpdateCell(Item item)
        {
            txtHoursWorked.AdjustsFontSizeToFitWidth = true;
            txtJobDescription.AdjustsFontSizeToFitWidth = true;

            txtJobDescription.Text = item.JobType.ToString() + " - " + item.Job.Address;
            txtHoursWorked.Text = item.TimeWorked.ToDecimalFormat();
        }
    }
}