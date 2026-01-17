using Churchee.Common.Storage;
using Churchee.Module.Site.Jobs;
using Moq;

namespace Churchee.Module.Site.Tests.Jobs
{
    public class PublishArticlesJobTests
    {
        [Fact]
        public void PublishArticlesJob_Calls_SaveCHangeAsync_OnDbContext()
        {
            var mockStore = new Mock<IDataStore>();

            var cut = new PublishArticlesJob(mockStore.Object);

            mockStore.Verify(v => v.SaveChangesAsync(), Times.Once);
        }

    }
}
