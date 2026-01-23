using Churchee.Common.Abstractions.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Helpers;
using Moq;

namespace Churchee.Module.Site.Tests.Helpers
{
    public class SuffixGenerationTests
    {
        public class TestWebContent : WebContent
        {
            // Inherit Url property from WebContent
        }

        [Fact]
        public void AddUniqueSuffixIfNeeded_UrlIsUnique_DoesNotChangeUrl()
        {
            // Arrange
            var webContent = new TestWebContent { Url = "about" };
            var repoMock = new Mock<IRepository<TestWebContent>>();
            repoMock.Setup(r => r.GetQueryable()).Returns(new List<TestWebContent>().AsQueryable());

            // Act
            SuffixGeneration.AddUniqueSuffixIfNeeded(webContent, repoMock.Object);

            // Assert
            Assert.Equal("about", webContent.Url);
        }

        [Fact]
        public void AddUniqueSuffixIfNeeded_UrlExists_AppendsSuffix()
        {
            // Arrange
            var webContent = new TestWebContent { Url = "about" };
            var existing = new List<TestWebContent>
            {
                new TestWebContent { Url = "about" }
            };
            var repoMock = new Mock<IRepository<TestWebContent>>();
            repoMock.Setup(r => r.GetQueryable()).Returns(existing.AsQueryable());

            // Act
            SuffixGeneration.AddUniqueSuffixIfNeeded(webContent, repoMock.Object);

            // Assert
            Assert.Equal("about-2", webContent.Url);
        }

        [Fact]
        public void AddUniqueSuffixIfNeeded_MultipleExistingSuffixes_AppendsNextAvailableSuffix()
        {
            // Arrange
            var webContent = new TestWebContent { Url = "about" };
            var existing = new List<TestWebContent>
            {
                new TestWebContent { Url = "about" },
                new TestWebContent { Url = "about-2" },
                new TestWebContent { Url = "about-3" }
            };
            var repoMock = new Mock<IRepository<TestWebContent>>();
            repoMock.Setup(r => r.GetQueryable()).Returns(existing.AsQueryable());

            // Act
            SuffixGeneration.AddUniqueSuffixIfNeeded(webContent, repoMock.Object);

            // Assert
            Assert.Equal("about-4", webContent.Url);
        }

        [Fact]
        public void AddUniqueSuffixIfNeeded_UrlAlreadyHasSuffix_IncrementsSuffix()
        {
            // Arrange
            var webContent = new TestWebContent { Url = "about-2" };
            var existing = new List<TestWebContent>
            {
                new TestWebContent { Url = "about" },
                new TestWebContent { Url = "about-2" }
            };
            var repoMock = new Mock<IRepository<TestWebContent>>();
            repoMock.Setup(r => r.GetQueryable()).Returns(existing.AsQueryable());

            // Act
            SuffixGeneration.AddUniqueSuffixIfNeeded(webContent, repoMock.Object);

            // Assert
            Assert.Equal("about-3", webContent.Url);
        }
    }
}