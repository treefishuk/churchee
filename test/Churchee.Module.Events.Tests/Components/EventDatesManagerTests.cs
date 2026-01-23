using Bunit;
using Churchee.Module.Events.Components;
using Churchee.Module.Events.Models;
using Churchee.Test.Helpers.Blazor;
using Churchee.Test.Helpers.Validation;

namespace Churchee.Module.Events.Tests.Components
{
    public class EventDatesManagerTests : BasePageTests
    {
        [Fact]
        public void EventDatesManager_AddDate_AddsDate()
        {
            //arrange
            JSInterop.Mode = JSRuntimeMode.Loose;

            var cut = Render<EventDatesManager>(parameters => parameters
              .Add(p => p.Dates, [])
            );

            //act
            cut.Instance.Start = DateTime.Now;
            cut.Instance.End = DateTime.Now;

            cut.Find(".btn-add").Click();

            //assert
            cut.Instance.Dates.Count.Should().Be(1);
        }

        [Fact]
        public void EventDatesManager_RemovedDate_AddsDate()
        {
            //arrange
            JSInterop.Mode = JSRuntimeMode.Loose;

            var date = new EventDateModel(Guid.NewGuid(), DateTime.Now, DateTime.Now.AddHours(1));

            var cut = Render<EventDatesManager>(parameters => parameters
              .Add(p => p.Dates, [date])
            );

            //act
            cut.Instance.Start = DateTime.Now;
            cut.Instance.End = DateTime.Now;

            cut.Find(".delete-row").Click();

            //assert
            cut.Instance.Dates.Count.Should().Be(0);
        }

        [Fact]
        public void AddRecurringWeekly_AddsWeeklyOccurrencesUntilFinal()
        {
            // arrange
            JSInterop.Mode = JSRuntimeMode.Loose;

            var start = new DateTime(2026, 1, 01, 10, 0, 0); // 1 Jan 2026 10:00
            var end = start.AddHours(1);
            var final = start.AddDays(21); // inclusive -> occurrences: 0,7,14,21 => 4

            var dates = new List<EventDateModel>();

            var cut = Render<EventDatesManager>(parameters => parameters
                .Add(p => p.Dates, dates)
            );

            // set up recurring weekly
            cut.Instance.Start = start;
            cut.Instance.End = end;
            cut.Instance.Final = final;
            cut.Instance.Recouring = true;
            cut.Instance.SelectedRecourance = "Week";

            // act
            cut.Find(".btn-add").Click();

            // assert
            cut.Instance.Dates.Count.Should().Be(4);

            for (var i = 0; i < 4; i++)
            {
                var expected = start.AddDays(7 * i);
                cut.Instance.Dates?[i].Start.Should().Be(expected);
                // duration preserved
                var endValue = cut.Instance.Dates?[i].End;
                if (endValue.HasValue)
                {
                    endValue.Value.Should().Be(expected.AddHours(1));
                }
            }
        }

        [Fact]
        public void AddRecurringMonthly_AddsOrdinalWeekdayOccurrencesUntilFinal()
        {
            // arrange
            JSInterop.Mode = JSRuntimeMode.Loose;

            // choose a date that is the 1st Monday of Jan 2026
            var start = new DateTime(2026, 1, 05, 09, 30, 0); // 5 Jan 2026 09:30 (first Monday)
            var end = start.AddHours(2);
            var final = new DateTime(2026, 3, 10, 0, 0, 0); // include March first Monday (Mar 2)

            var dates = new List<EventDateModel>();

            var cut = Render<EventDatesManager>(parameters => parameters
                .Add(p => p.Dates, dates)
            );

            // set up recurring monthly on the same ordinal weekday
            cut.Instance.Start = start;
            cut.Instance.End = end;
            cut.Instance.Final = final;
            cut.Instance.Recouring = true;
            cut.Instance.SelectedRecourance = "Month";

            // act
            cut.Find(".btn-add").Click();

            // assert
            // Expect occurrences in Jan (5th), Feb (2nd), Mar (2nd) => 3 occurrences
            cut.Instance.Dates.Count.Should().Be(3);

            cut.Instance.Dates[0].Start.Should().Be(start);
            cut.Instance.Dates[1].Start.Should().Be(new DateTime(2026, 2, 02, 09, 30, 0));
            cut.Instance.Dates[2].Start.Should().Be(new DateTime(2026, 3, 02, 09, 30, 0));

            // duration preserved
            for (int i = 0; i < cut.Instance.Dates.Count; i++)
            {
                var s = cut.Instance.Dates[i].Start;
                var e = cut.Instance.Dates[i].End;
                if (s != null && e.HasValue)
                {
                    e.Value.Should().Be(s.Value.AddHours(2));
                }
            }
        }
    }
}
