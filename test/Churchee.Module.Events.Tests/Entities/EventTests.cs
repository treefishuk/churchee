using Churchee.Module.Events.Entities;
using Churchee.Test.Helpers.Validation;

namespace Churchee.Module.Events.Tests.Entities
{
    public class EventTests
    {
        [Fact]
        public void Converts_Time_To_Daylight_Savings()
        {
            // Arrange
            var eventDateStart = new DateTime(2026, 4, 15, 14, 0, 0, DateTimeKind.Utc);
            var eventDateEnd = new DateTime(2026, 4, 15, 16, 0, 0, DateTimeKind.Utc);

            var builder = new Event.Builder();

            builder.SetTitle("New Event");

            // Act
            if (OperatingSystem.IsWindows())
            {
                builder.SetDatesFromUtc(eventDateStart, eventDateEnd, TimeZoneInfo.FindSystemTimeZoneById("Europe/London"));
            }
            else
            {
                builder.SetDatesFromUtc(eventDateStart, eventDateEnd, TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time"));
            }

            var eventEntity = builder.Build();

            // Assert
            eventEntity.EventDates.FirstOrDefault()?.Start?.Hour.Should().Be(15);
        }

        [Fact]
        public void SetDatesFromUtc_Does_Nothing_When_Null()
        {
            // Arrange
            var builder = new Event.Builder();

            builder.SetTitle("New Event");

            // Act
            if (OperatingSystem.IsWindows())
            {
                builder.SetDatesFromUtc(null, null, TimeZoneInfo.FindSystemTimeZoneById("Europe/London"));
            }
            else
            {
                builder.SetDatesFromUtc(null, null, TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time"));
            }

            var eventEntity = builder.Build();

            // Assert
            eventEntity.EventDates.Count.Should().Be(0);
        }

    }
}
