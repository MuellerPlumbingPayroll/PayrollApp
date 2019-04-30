using System;
using UIKit;
using Timecard.Models;

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

            if (item.JobType == JobType.Other)
            {
                // For other jobs, display the cost code instead of the job as it is more informative
                txtJobDescription.Text = item.JobType.ToString() + " - " + item.CostCode.Description;
            }
            else
            {
                txtJobDescription.Text = item.JobType.ToString() + " - " + item.Job.Address;
            }

            txtHoursWorked.Text = item.TimeWorked.ToDecimalFormat();
        }
    }
}