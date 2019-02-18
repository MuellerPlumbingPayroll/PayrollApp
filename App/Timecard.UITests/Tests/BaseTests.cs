using NUnit.Framework;
using Xamarin.UITest;

namespace Timecard.UITests.Tests
{
    public class BaseTests
    {
        protected readonly string NEW_ENTRY_BUTTON_TITLE = "New Time Entry";
        protected readonly string HOME_TAB_LABEL = "tabHome";
        protected readonly string HISTORY_TAB_LABEL = "tabHistory";

        protected readonly string CONSTRUCTION_SEGMENT_TITLE = "Construction";
        protected readonly string SERVICE_SEGMENT_TITLE = "Service";
        protected readonly string OTHER_SEGMENT_TITLE = "Other";

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
            app.WaitForElement(c => c.Marked(HOME_TAB_LABEL));
            app.Tap(c => c.Marked(HOME_TAB_LABEL));
            app.WaitForElement(c => c.Marked("txtHoursWorkedToday")) ;
        }

        protected void FillOutEntry(string tabName, int numHours, 
                                    string jobDescription = null, string costCode = null)
        {
            app.Tap(c => c.Marked(tabName));

            app.Tap("Hours");
            app.EnterText(numHours.ToString());

            app.Tap("Job Description");
            if (tabName == SERVICE_SEGMENT_TITLE)
                app.EnterText(jobDescription);

            if (tabName != OTHER_SEGMENT_TITLE) 
                app.Tap("Cost Code");
            app.DismissKeyboard();

            app.Tap("Save");
        }
    }
}
