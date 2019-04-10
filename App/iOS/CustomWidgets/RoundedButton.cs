using System;
using UIKit;

namespace Timecard.iOS
{
    public partial class RoundedButton : UIButton
    {
        private static readonly int CORNER_RADIUS = 10;

        public RoundedButton(IntPtr handle) : base(handle)
        {
            Layer.CornerRadius = CORNER_RADIUS;
            ClipsToBounds = true;
        }
    }
}
