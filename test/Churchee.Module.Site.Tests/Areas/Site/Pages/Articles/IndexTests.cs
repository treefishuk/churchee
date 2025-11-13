using Bunit;
using Churchee.Common.Abstractions;
using Churchee.Common.ResponseTypes;
using Churchee.Module.Site.Features.Blog.Commands;
using Churchee.Module.Site.Features.Blog.Queries.GetListBlogItems;
using Churchee.Module.Site.Features.Blog.Responses;
using Churchee.Module.UI.Components;
using Churchee.Test.Helpers.Blazor;
using Churchee.Test.Helpers.Validation;
using Moq;
using Radzen;
using Index = Churchee.Module.Site.Areas.Site.Pages.Articles.Index;

namespace Churchee.Module.Site.Tests.Areas.Site.Pages.Articles
{
    public class IndexTests : BasePageTests
    {

        [Fact]
        public void ArticlesIndex_HasCorrectName()
        {
            //arrange
            var data = new DataTableResponse<GetListBlogItemsResponseItem>();

            MockMediator.Setup(s => s.Send(It.IsAny<GetListBlogItemsRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(data);

            SetInitialUrl<Index>();

            //act
            var cut = Render<Index>();

            //assert
            var pageName = cut.FindComponent<PageName>();

            pageName.Instance.Name.Should().Be("Articles");
        }

        [Fact]
        public void ArticlesIndex_DeleteSucceeds_ShowsSuccessMessage()
        {
            // Arrange
            SetupGetListingQueryResponse();

            MockMediator.Setup(s => s.Send(It.IsAny<DeleteArticleCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(new CommandResponse());

            // Act
            var cut = Render<Index>();

            cut.FindAll(".delete-row")[0].Click();

            // Assert
            NotificationService.Notifications.Count.Should().Be(1);
            NotificationService.Notifications.First().Summary.Should().Be("Article Deleted");
            NotificationService.Notifications.First().Severity.Should().Be(NotificationSeverity.Success);
        }

        [Fact]
        public void ArticlesIndex_DeleteFails_ShowsErrorMessage()
        {
            // Arrange
            SetupGetListingQueryResponse();

            var commandResponse = new CommandResponse();
            commandResponse.AddError("Error", "Thing");

            MockMediator.Setup(s => s.Send(It.IsAny<DeleteArticleCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(commandResponse);

            // Act
            var cut = Render<Index>();

            cut.FindAll(".delete-row")[0].Click();

            // Assert
            NotificationService.Notifications.Count.Should().Be(1);
            NotificationService.Notifications.First().Summary.Should().Be("Error: Could not remove Article");
            NotificationService.Notifications.First().Severity.Should().Be(NotificationSeverity.Error);
        }


        private void SetupGetListingQueryResponse()
        {
            var data = new DataTableResponse<GetListBlogItemsResponseItem>
            {
                Data =
                [
                    new() { Id = Guid.NewGuid(), Title = "Test" }
                ]
            };

            MockMediator.Setup(s => s.Send(It.IsAny<GetListBlogItemsRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(data);
        }
    }
}
