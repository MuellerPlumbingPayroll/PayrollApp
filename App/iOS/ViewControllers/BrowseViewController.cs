using System;
using System.Collections.Specialized;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Timecard.iOS
{
    public partial class BrowseViewController : UITableViewController
    {
        UIRefreshControl refreshControl;

        public ItemsViewModel ViewModel { get; set; }

        public BrowseViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ViewModel = BaseViewController.AllItemsViewModel;

            // Setup UITableView.
            refreshControl = new UIRefreshControl();
            refreshControl.ValueChanged += RefreshControl_ValueChanged;
            TableView.Add(refreshControl);
            TableView.Source = new ItemsDataSource(ViewModel);
            
            Title = ViewModel.Title;

            ViewModel.PropertyChanged += IsBusy_PropertyChanged;
            ViewModel.Items.CollectionChanged += Items_CollectionChanged;
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            if (ViewModel.Items.Count == 0)
                ViewModel.LoadItemsCommand.Execute(null);
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            if (segue.Identifier == "NavigateToEditItemSegue")
            {
                var controller = segue.DestinationViewController as TimeNewViewController;
                var indexPath = TableView.IndexPathForCell(sender as UITableViewCell);
                var item = ViewModel.GetItemSections()[indexPath.Section][indexPath.Row];

                controller.EditingItem = item;
            }
        }

        void RefreshControl_ValueChanged(object sender, EventArgs e)
        {
            if (!ViewModel.IsBusy && refreshControl.Refreshing)
                ViewModel.LoadItemsCommand.Execute(null);
        }

        void IsBusy_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var propertyName = e.PropertyName;
            switch (propertyName)
            {
                case nameof(ViewModel.IsBusy):
                    {
                        InvokeOnMainThread(() =>
                        {
                            if (ViewModel.IsBusy && !refreshControl.Refreshing)
                                refreshControl.BeginRefreshing();
                            else if (!ViewModel.IsBusy)
                                refreshControl.EndRefreshing();
                        });
                    }
                    break;
            }
        }

        void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            InvokeOnMainThread(TableView.ReloadData);
        }
    }

    class ItemsDataSource : UITableViewSource
    {
        private static readonly NSString CELL_IDENTIFIER = new NSString("HISTORY_CELL");
        private static readonly NSString SECTION_IDENTIFIER = new NSString("SECTION_CELL");
        private readonly int CELL_HEIGHT = 100;
        private static readonly int SECTION_HEADER_HEIGHT = 50;
        private static readonly int SECTION_FOOTER_HEIGHT = 2;

        ItemsViewModel viewModel;

        public ItemsDataSource(ItemsViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return viewModel.GetItemSections()[section].Count;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return viewModel.GetItemSections().Length;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return CELL_HEIGHT;
        }

        public override nfloat GetHeightForHeader(UITableView tableView, nint section)
        {
            return SECTION_HEADER_HEIGHT;
        }

        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            var header = tableView.DequeueReusableCell(SECTION_IDENTIFIER) as HistoryTableViewSectionHeader;

            var date = viewModel.GetStartOfPayPeriod().AddDays(section);
            var numberHoursWorked = viewModel.NumberHoursWorkedOnDay(date);

            header.UpdateHeader(date, numberHoursWorked);

            return header;
        }

        public override nfloat GetHeightForFooter(UITableView tableView, nint section)
        {
            return SECTION_FOOTER_HEIGHT;
        }

        public override UIView GetViewForFooter(UITableView tableView, nint section)
        {
            // If this is an empty section, then add a small footer to
            // differentiate the two section headers
            if (tableView.NumberOfRowsInSection(section) == 0)
            {
                UIView separator = new UIView
                {
                    Frame = new CGRect(0, 0, tableView.Bounds.Size.Width, SECTION_FOOTER_HEIGHT),
                    BackgroundColor = tableView.SeparatorColor
                };
                return separator;
            }
            return null;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(CELL_IDENTIFIER, indexPath) as HistoryTableViewCell;
            var item = viewModel.GetItemSections()[indexPath.Section][indexPath.Row];
            cell.UpdateCell(item);

            return cell;
        }
    }
}
