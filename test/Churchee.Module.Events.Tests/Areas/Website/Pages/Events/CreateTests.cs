using Bunit;
using Bunit.TestDoubles;
using Churchee.Common.ResponseTypes;
using Churchee.Module.Events.Areas.Website.Pages.Events;
using Churchee.Module.Events.Features.Commands;
using Churchee.Module.Events.Tests.Areas.Shared.Pages;
using Churchee.Module.UI.Components;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Churchee.Module.Events.Tests.Areas.Website.Pages.Events
{
    public class CreateTests : BasePageTests
    {
        [Fact]
        public void CreateEvent_HasCorrectname()
        {
            //arrange
            MockMediator.Setup(s => s.Send(It.IsAny<ActivateEventsCommand>(), default)).ReturnsAsync(new CommandResponse());
            MockMediator.Setup(s => s.Send(It.IsAny<CreateEventCommand>(), default)).ReturnsAsync(new CommandResponse());

            SetInitialUrl<Create>();

            //act
            var cut = RenderComponent<Create>();

            //assert
            var pageName = cut.FindComponent<PageName>();

            pageName.Instance.Name.Should().Be("Create Event");
        }

        [Fact]
        public void CreateEvent_InputModelValid_RedirectsToIndex()
        {
            //arrange
            MockMediator.Setup(s => s.Send(It.IsAny<ActivateEventsCommand>(), default)).ReturnsAsync(new CommandResponse());
            MockMediator.Setup(s => s.Send(It.IsAny<CreateEventCommand>(), default)).ReturnsAsync(new CommandResponse());

            //act
            var cut = RenderComponent<Create>();

            cut.Instance.InputModel.Start = DateTime.Now;
            cut.Instance.InputModel.Title = "Test";
            cut.Instance.InputModel.Description = "Test";

            cut.Find(".sticky-formButtons .rz-success").Click();

            //assert
            var navMan = Services.GetRequiredService<FakeNavigationManager>();

            navMan.Uri.Should().Be("http://localhost/management/events");
        }

        [Fact]
        public void CreateEvent_ActivateEventsCommandReturnsError_StaysOnPage()
        {
            //arrange
            var responseFailure = new CommandResponse();
            responseFailure.AddError("Failure", "Thning");

            MockMediator.Setup(s => s.Send(It.IsAny<ActivateEventsCommand>(), default)).ReturnsAsync(new CommandResponse());
            MockMediator.Setup(s => s.Send(It.IsAny<CreateEventCommand>(), default)).ReturnsAsync(responseFailure);

            //act
            var cut = RenderComponent<Create>();

            cut.Instance.InputModel.Start = DateTime.Now;
            cut.Instance.InputModel.Title = "Test";
            cut.Instance.InputModel.Description = "Test";

            cut.Find(".sticky-formButtons .rz-success").Click();

            //assert
            var navMan = Services.GetRequiredService<FakeNavigationManager>();
            navMan.Uri.Should().NotBe("http://localhost/management/events");
        }

        [Fact]
        public void CreateEvent_CreateEventCommandReturnsError_StaysOnPage()
        {
            //arrange
            var responseFailure = new CommandResponse();
            responseFailure.AddError("Failure", "Thning");

            MockMediator.Setup(s => s.Send(It.IsAny<ActivateEventsCommand>(), default)).ReturnsAsync(responseFailure);

            //act
            var cut = RenderComponent<Create>();

            cut.Instance.InputModel.Start = DateTime.Now;
            cut.Instance.InputModel.Title = "Test";
            cut.Instance.InputModel.Description = "Test";

            cut.Find(".sticky-formButtons .rz-success").Click();

            //assert
            var navMan = Services.GetRequiredService<FakeNavigationManager>();
            navMan.Uri.Should().NotBe("http://localhost/management/events");
        }
    }
}
