using NUnit.Framework;
using Xamarin.UITest;

namespace Timecard.UITests.Tests
{
    public class BaseTests
    {
        protected IApp app;
        protected Platform platform;

        public BaseTests(Platform platform)
        {
            this.platform = platform;
        }

        [SetUp]
        public virtual void SetUp()
        {
            app = AppInitializer.StartApp(platform);

            // Always make sure we start from the app home screen
            app.WaitForElement(c => c.Marked("tabHome"));
            app.Tap(c => c.Marked("tabHome"));
            app.WaitForElement(c => c.Marked("txtHoursWorkedToday")) ;
        }
    }
}
