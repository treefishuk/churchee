using Churchee.Module.Events.Entities;
using Churchee.Module.Events.Specifications;
using FluentAssertions;

namespace Churchee.Module.Events.Tests.Specifications
{
    public class EventDatesForEventSpecificationTests
    {
        [Fact]
        public void EventDatesForEventSpecification_DoesntReturnPastDates()
        {
            //arrange
            var eventId = Guid.NewGuid();

            var dates = new List<EventDate>
            {
                new EventDate{  Start = DateTime.Now.AddDays(-2), End = DateTime.Now.AddDays(-1), EventId = eventId },
                new EventDate{  Start = DateTime.Now.AddDays(2), End = DateTime.Now.AddDays(2), EventId = eventId  },
            };

            //act
            var specification = new EventDatesForEventSpecification(eventId);

            var results = specification.Evaluate(dates).ToList();

            //assert
            results.Count.Should().Be(1);
        }

        [Fact]
        public void EventDatesForEventSpecification_ReturnsDatesForSpecifiedEventOnly()
        {
            //arrange
            var eventId = Guid.NewGuid();

            var dates = new List<EventDate>
            {
                new EventDate{  Start = DateTime.Now.AddDays(3), End = DateTime.Now.AddDays(3), EventId = eventId  },
                new EventDate{  Start = DateTime.Now.AddDays(3), End = DateTime.Now.AddDays(3), EventId = Guid.NewGuid()  }
            };

            //act
            var specification = new EventDatesForEventSpecification(eventId);

            var results = specification.Evaluate(dates).ToList();

            //assert
            results.Count.Should().Be(1);
        }
    }
}
