using System;

namespace Timecard
{
    public class ItemDetailViewModel : BaseViewModel
    {
        public Item Item { get; set; }
        public ItemDetailViewModel(Item item = null)
        {
            if (item != null)
            {
                Title = string.Empty;
                Item = item;
            }
        }
    }
}
