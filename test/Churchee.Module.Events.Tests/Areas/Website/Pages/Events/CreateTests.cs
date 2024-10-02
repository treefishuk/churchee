using Bunit;
using Churchee.Common.Abstractions.Auth;
using Churchee.Module.Events.Areas.Website.Pages.Events;
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
        public void CreateEvent_InputModelNotNull()
        {
            //arrange
            var mockCurrentUser = new Mock<ICurrentUser>();
            Services.AddSingleton(mockCurrentUser.Object);

            var mockMediatorService = new Mock<IMediator>();
            Services.AddSingleton(mockMediatorService.Object);

            Services.AddRadzenComponents();

            JSInterop.Mode = JSRuntimeMode.Loose;

            //act
            var cut = RenderComponent<Create>();

            cut.Instance.InputModel.Start = DateTime.Now;
            cut.Instance.InputModel.Title = "Test";
            cut.Instance.InputModel.Description = "Test";

            //assert
            cut.Instance.InputModel.Should().NotBeNull();

        }
    }
}
