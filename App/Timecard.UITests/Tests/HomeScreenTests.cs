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
            app.Tap(NEW_ENTRY_BUTTON_TITLE);   

            // Search for a save button on the screen
            Assert.NotNull(app.Query(c => c.Marked("Save")).First().Text);
        }

        [Test]
        public void TestAddNewEntry_UpdatesHoursLabels()
        {
            int numberHoursToAdd = 8;
            string todayLabelName = "txtHoursWorkedToday";
            string weekLabelName = "txtHoursWorkedThisWeek";

            float startingHoursToday = GetHoursFromLabel(todayLabelName);
            float startingHoursWeek = GetHoursFromLabel(weekLabelName);

            app.Tap(NEW_ENTRY_BUTTON_TITLE);
            FillOutEntry(CONSTRUCTION_SEGMENT_TITLE, numberHoursToAdd);
            app.WaitForElement(NEW_ENTRY_BUTTON_TITLE);

            float endingHoursToday = GetHoursFromLabel(todayLabelName);
            float endingHoursWeek = GetHoursFromLabel(weekLabelName);

            Assert.AreEqual(startingHoursToday + numberHoursToAdd, endingHoursToday);
            Assert.AreEqual(startingHoursWeek + numberHoursToAdd, endingHoursWeek);
        }

        private float GetHoursFromLabel(string labelName) 
        {
            var labelText = app.Query(c => c.Marked(labelName)).First().Text;
            var hoursString = labelText.Replace(" hrs", string.Empty).Split(' ').Last();
            return float.Parse(hoursString);
        }
    }
}
