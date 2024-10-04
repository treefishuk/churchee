using Bunit;
using Bunit.TestDoubles;
using Churchee.Common.ResponseTypes;
using Churchee.Module.Events.Areas.Website.Pages.Events;
using Churchee.Module.Events.Features.Commands;
using Churchee.Module.Events.Features.Queries;
using Churchee.Module.Events.Models;
using Churchee.Module.Events.Tests.Areas.Shared.Pages;
using Churchee.Module.UI.Components;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Churchee.Module.Events.Tests.Areas.Website.Pages.Events
{
    public class EditTests : BasePageTests
    {
        [Fact]
        public void EditEvent_HasCorrectname()
        {
            //arrange
            SetupGetDetailByIdResponse();

            SetInitialUrl<Edit>();

            //act
            var cut = RenderComponent<Edit>();

            //assert
            var pageName = cut.FindComponent<PageName>();

            pageName.Instance.Name.Should().Be("Edit Event");
        }

        [Fact]
        public void EditEvent_ClickCancel_ReturnsToIndex()
        {
            //arrange
            SetupGetDetailByIdResponse();

            var cut = RenderComponent<Edit>();

            //act
            cut.Find(".sticky-formButtons .rz-danger").Click();

            //assert
            var navMan = Services.GetRequiredService<FakeNavigationManager>();
            navMan.Uri.Should().Be("http://localhost/management/events");

        }

        [Fact]
        public void EditEvent_FormInvalid_ClickSave_StaysOnPage()
        {
            //arrange
            SetupGetDetailByIdResponse();

            var cut = RenderComponent<Edit>();

            //act
            cut.Instance.InputModel.Title = string.Empty;

            cut.Find(".sticky-formButtons .rz-success").Click();

            //assert
            var navMan = Services.GetRequiredService<FakeNavigationManager>();
            navMan.Uri.Should().NotBe("http://localhost/management/events");

        }

        [Fact]
        public void EditEvent_FormValid_ClickSave_ReturnsToIndex()
        {
            //arrange
            SetupGetDetailByIdResponse();
            SetupUpdateEventCommandResponse();

            var cut = RenderComponent<Edit>();

            //act
            cut.Find(".sticky-formButtons .rz-success").Click();

            //assert
            var navMan = Services.GetRequiredService<FakeNavigationManager>();
            navMan.Uri.Should().Be("http://localhost/management/events");

        }

        private void SetupGetDetailByIdResponse()
        {
            var mediatorResponse = new GetDetailByIdResponse(
                title: "Title",
                description: "Description",
                content: "",
                imageUrl: "/img/test.jpg",
                locationName: "Somewhere",
                city: "Emerald",
                street: "Yellow Brick Road",
                postCode: "OZ1 TWW",
                country: "Kansas",
                latitude: 39.105092049099866m,
                longitude: -94.62311716634233m,
                dates: [
                    new EventDateModel(DateTime.Now, DateTime.Now.AddMinutes(90))
                ]);

            MockMediator.Setup(s => s.Send(It.IsAny<GetDetailByIdQuery>(), default)).ReturnsAsync(mediatorResponse);
        }

        private void SetupUpdateEventCommandResponse()
        {
            MockMediator.Setup(s => s.Send(It.IsAny<UpdateEventCommand>(), default)).ReturnsAsync(new CommandResponse());
        }
    }
}
