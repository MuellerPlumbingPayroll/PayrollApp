using NUnit.Framework;
using Timecard.UITests.Tests;
using Xamarin.UITest;

namespace Timecard.UITests
{
    [TestFixture(Platform.iOS)]
    public class NewEntryScreenGestureTests : BaseTests
    {
        public NewEntryScreenGestureTests(Platform platform) : base(platform) { }

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            app.Tap("New Time Entry");
            app.WaitForElement(c => c.Class("UISegmentedControl"));
        }

        [Test]
        public void TestCostCodeDisappears_OnOtherSegment()
        { 
            app.Tap("Other");

            // The cost code field should not be displayed when on the "Other" job type segment
            Assert.IsFalse(IsCostCodeVisible());
        }

        [Test]
        public void TestSwipeLeft_ChangesSegment()
        {
            // Swiping left once should result in being on the "Other" job segment
            app.SwipeRightToLeft(c => c.Marked("txtHoursWorked"));
            Assert.IsFalse(IsCostCodeVisible());

            // Swiping left again should result in being on the "Service" job segment
            app.SwipeRightToLeft(c => c.Marked("txtHoursWorked"));
            Assert.IsTrue(IsCostCodeVisible());
        }

        [Test]
        public void TestSwipeRight_ChangesSegment()
        {
            // Swiping right five times should result in being on the "Other" job segment
            for (var i = 0; i < 5; i++)
                app.SwipeLeftToRight(c => c.Marked("txtHoursWorked"));
                
            Assert.IsFalse(IsCostCodeVisible());
        }

        private bool IsCostCodeVisible()
        {
            return app.Query(c => c.Marked("txtCostCode")).Length.Equals(1);
        }
    }
}
