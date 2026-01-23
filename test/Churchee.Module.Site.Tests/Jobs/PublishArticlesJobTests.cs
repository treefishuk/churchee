using Ardalis.Specification;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Jobs;
using Moq;

namespace Churchee.Module.Site.Tests.Jobs
{
    public class PublishArticlesJobTests
    {
        [Fact]
        public async Task PublishArticlesJob_Calls_SaveChangeAsync_OnDbContext()
        {
            // Arrange
            var mockStore = new Mock<IDataStore>();

            var mockRepo = new Mock<IRepository<Article>>();

            mockStore.Setup(s => s.GetRepository<Article>()).Returns(mockRepo.Object);

            mockRepo.Setup(Object => Object.GetListAsync(
                It.IsAny<ISpecification<Article>>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync([]);

            var cut = new PublishArticlesJob(mockStore.Object);

            // Act
            await cut.ExecuteAsync(CancellationToken.None);

            // Assert
            mockStore.Verify(v => v.SaveChangesAsync(), Times.Once);
        }

    }
}
