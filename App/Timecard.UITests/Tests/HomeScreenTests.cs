using System.Linq;
using NUnit.Framework;
using Timecard.UITests.Tests;
using Xamarin.UITest;

namespace Timecard.UITests
{
    [TestFixture(Platform.iOS)]
    public class HomeScreenTests : BaseTests
    {
        public HomeScreenTests(Platform platform) : base(platform) { }

        [Test]
        public void TestClickNewEntryButton_CorrectlyTransitions()
        {
            app.Tap("New Time Entry");   

            // Search for a save button on the screen
            Assert.NotNull(app.Query(c => c.Marked("Save")).First().Text);
        }

        [Test]
        public void TestAddNewEntry_UpdatesHoursToday()
        {
            int numberHoursToAdd = 8;

            // Find the hours worked today label and parse it to get the starting hours
            var todayLabelText = app.Query(c => c.Marked("txtHoursWorkedToday")).First().Text;
            var todayHoursString = todayLabelText.Replace("Today: ", string.Empty).Replace(" hrs", string.Empty);
            float startingHoursToday = float.Parse(todayHoursString);

            app.Tap("New Time Entry");

            app.Tap("Hours");
            app.EnterText(numberHoursToAdd.ToString());

            app.Tap("Job Description");
            app.Tap("Cost Code");
            app.DismissKeyboard();

            app.Tap("Save");

            app.WaitForElement(c => c.Marked("txtHoursWorkedToday"));
            todayLabelText = app.Query(c => c.Marked("txtHoursWorkedToday")).First().Text;
            var newTodayHoursString = todayLabelText.Replace("Today: ", string.Empty).Replace(" hrs", string.Empty);
            float endingHoursToday = float.Parse(newTodayHoursString);

            Assert.AreEqual(startingHoursToday + numberHoursToAdd, endingHoursToday);
        }
    }
}
