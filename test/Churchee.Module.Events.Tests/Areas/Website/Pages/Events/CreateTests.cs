using Churchee.Module.Events.Areas.Website.Pages.Events;
using Bunit;
using FluentAssertions;
using Churchee.Common.Abstractions.Auth;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Radzen;
using MediatR;

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

            JSInterop.SetupVoid("Radzen.preventArrows", _ => true);
            JSInterop.SetupVoid("Radzen.createEditor", _ => true);
            JSInterop.SetupVoid("Radzen.createDatePicker", _ => true);
            JSInterop.SetupVoid("Radzen.uploads", _ => true);

            //act
            var cut = RenderComponent<Create>();

            //assert
            cut.Instance.InputModel.Should().NotBeNull();

        }

    }
}
