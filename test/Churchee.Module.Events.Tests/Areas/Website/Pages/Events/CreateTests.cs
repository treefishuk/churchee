using Bunit;
using Bunit.TestDoubles;
using Churchee.Common.Abstractions.Auth;
using Churchee.Common.ResponseTypes;
using Churchee.Module.Events.Areas.Website.Pages.Events;
using Churchee.Module.Events.Features.Commands;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Radzen;

namespace Churchee.Module.Events.Tests.Areas.Website.Pages.Events
{
    public class CreateTests : TestContext
    {
        [Fact]
        public void CreateEvent_InputModelValid_RedirectsToIndex()
        {
            //arrange
            var mockCurrentUser = new Mock<ICurrentUser>();
            Services.AddSingleton(mockCurrentUser.Object);

            var mockMediatorService = new Mock<IMediator>();

            mockMediatorService.Setup(s => s.Send(It.IsAny<ActivateEventsCommand>(), default)).ReturnsAsync(new CommandResponse());
            mockMediatorService.Setup(s => s.Send(It.IsAny<CreateEventCommand>(), default)).ReturnsAsync(new CommandResponse());
            Services.AddSingleton(mockMediatorService.Object);

            Services.AddRadzenComponents();

            JSInterop.Mode = JSRuntimeMode.Loose;

            var navMan = Services.GetRequiredService<FakeNavigationManager>();

            //act
            var cut = RenderComponent<Create>();

            cut.Instance.InputModel.Start = DateTime.Now;
            cut.Instance.InputModel.Title = "Test";
            cut.Instance.InputModel.Description = "Test";

            cut.Find(".sticky-formButtons .rz-success").Click();

            //assert
            navMan.Uri.Should().Be("http://localhost/management/events");
        }

        [Fact]
        public void CreateEvent_ActivateEventsCommandReturnsError_StaysOnPage()
        {
            //arrange
            var mockCurrentUser = new Mock<ICurrentUser>();
            Services.AddSingleton(mockCurrentUser.Object);

            var mockMediatorService = new Mock<IMediator>();

            var responseFailure = new CommandResponse();

            responseFailure.AddError("Failure", "Thning");

            mockMediatorService.Setup(s => s.Send(It.IsAny<ActivateEventsCommand>(), default)).ReturnsAsync(new CommandResponse());
            mockMediatorService.Setup(s => s.Send(It.IsAny<CreateEventCommand>(), default)).ReturnsAsync(responseFailure);

            Services.AddSingleton(mockMediatorService.Object);

            Services.AddRadzenComponents();

            JSInterop.Mode = JSRuntimeMode.Loose;

            var navMan = Services.GetRequiredService<FakeNavigationManager>();

            //act
            var cut = RenderComponent<Create>();

            cut.Instance.InputModel.Start = DateTime.Now;
            cut.Instance.InputModel.Title = "Test";
            cut.Instance.InputModel.Description = "Test";

            cut.Find(".sticky-formButtons .rz-success").Click();

            //assert
            navMan.Uri.Should().NotBe("http://localhost/management/events");
        }

        [Fact]
        public void CreateEvent_CreateEventCommandReturnsError_StaysOnPage()
        {
            //arrange
            var mockCurrentUser = new Mock<ICurrentUser>();
            Services.AddSingleton(mockCurrentUser.Object);

            var mockMediatorService = new Mock<IMediator>();

            var responseFailure = new CommandResponse();

            responseFailure.AddError("Failure", "Thning");

            mockMediatorService.Setup(s => s.Send(It.IsAny<ActivateEventsCommand>(), default)).ReturnsAsync(responseFailure);

            Services.AddSingleton(mockMediatorService.Object);

            Services.AddRadzenComponents();

            JSInterop.Mode = JSRuntimeMode.Loose;

            var navMan = Services.GetRequiredService<FakeNavigationManager>();

            //act
            var cut = RenderComponent<Create>();

            cut.Instance.InputModel.Start = DateTime.Now;
            cut.Instance.InputModel.Title = "Test";
            cut.Instance.InputModel.Description = "Test";

            cut.Find(".sticky-formButtons .rz-success").Click();

            //assert
            navMan.Uri.Should().NotBe("http://localhost/management/events");
        }
    }
}
