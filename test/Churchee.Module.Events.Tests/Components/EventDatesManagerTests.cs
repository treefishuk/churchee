using Bunit;
using Churchee.Module.Events.Components;
using Churchee.Module.Events.Models;
using FluentAssertions;

namespace Churchee.Module.Events.Tests.Components
{
    public class EventDatesManagerTests : TestContext
    {
        [Fact]
        public void EventDatesManager_AddDate_AddsDate()
        {
            //arrange
            JSInterop.Mode = JSRuntimeMode.Loose;

            var cut = RenderComponent<EventDatesManager>(parameters => parameters
              .Add(p => p.Dates, [])
            );

            //act
            cut.Instance.Start = DateTime.Now;
            cut.Instance.End = DateTime.Now;

            cut.Find(".btn-add").Click();

            //assert
            cut.Instance.Dates.Count().Should().Be(1);
        }

        [Fact]
        public void EventDatesManager_RemovedDate_AddsDate()
        {
            //arrange
            JSInterop.Mode = JSRuntimeMode.Loose;

            var date = new EventDateModel(Guid.NewGuid(), DateTime.Now, DateTime.Now.AddHours(1));

            var cut = RenderComponent<EventDatesManager>(parameters => parameters
              .Add(p => p.Dates, [date])
            );

            //act
            cut.Instance.Start = DateTime.Now;
            cut.Instance.End = DateTime.Now;

            cut.Find(".btn-remove").Click();

            //assert
            cut.Instance.Dates.Count().Should().Be(0);
        }
    }
}
