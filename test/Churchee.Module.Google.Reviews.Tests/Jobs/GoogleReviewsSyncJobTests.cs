using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.Storage;
using Churchee.Module.Google.Reviews.Exceptions;
using Churchee.Module.Google.Reviews.Helpers;
using Churchee.Module.Google.Reviews.Jobs;
using Churchee.Module.Reviews.Entities;
using Churchee.Module.Tokens.Entities;
using Churchee.Module.Tokens.Specifications;
using Churchee.Test.Helpers.Validation;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Churchee.Module.Google.Reviews.Tests.Jobs
{
    public class GoogleReviewsSyncJobTests
    {
        private readonly Mock<IDataStore> _mockDataStore = new();
        private readonly Mock<IHttpClientFactory> _mockClientFactory = new();
        private readonly Mock<ISettingStore> _mockSettingStore = new();
        private readonly Mock<IImageProcessor> _mockImageProcessor = new();
        private readonly Mock<IBlobStore> _mockBlobStore = new();
        private readonly Mock<ILogger<GoogleReviewsSyncJob>> _mockLogger = new();

        private GoogleReviewsSyncJob CreateJob()
        {
            return new GoogleReviewsSyncJob(
                _mockDataStore.Object,
                _mockClientFactory.Object,
                _mockSettingStore.Object,
                _mockImageProcessor.Object,
                _mockBlobStore.Object,
                _mockLogger.Object);
        }

        private static HttpClient CreateHttpClientReturning(HttpResponseMessage response)
        {
            return new HttpClient(new FakeHttpMessageHandler(response))
            {
                BaseAddress = new Uri("https://example.test/")
            };
        }

        [Fact]
        public async Task GetAccountId_ReturnsAccountId_WhenResponseIsSuccess()
        {
            // Arrange
            var accountsJson = JsonSerializer.Serialize(new
            {
                accounts = new[] { new { name = "accounts/ACCOUNT123" } }
            });

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(accountsJson, Encoding.UTF8, "application/json")
            };

            var client = CreateHttpClientReturning(response);

            var job = CreateJob();

            // Act - call private method via reflection
            var method = typeof(GoogleReviewsSyncJob).GetMethod("GetAccountId", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var task = (Task<string>)method!.Invoke(job, new object[] { client, CancellationToken.None })!;
            var accountId = await task;

            // Assert
            accountId.Should().Be("ACCOUNT123");
        }

        [Fact]
        public async Task GetAccountId_Throws_GoogleReviewSyncException_WhenNotSuccess()
        {
            // Arrange
            var response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent("error")
            };

            var client = CreateHttpClientReturning(response);

            var job = CreateJob();

            // Act
            Func<Task> act = async () => await job.GetAccountId(client, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<GoogleReviewSyncException>();
        }

        [Fact]
        public async Task GetLocationId_ReturnsLocationId_WhenMatchFound()
        {
            // Arrange
            var locationsJson = JsonSerializer.Serialize(new
            {
                locations = new[] { new { name = "locations/LOC123" } }
            });

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(locationsJson, Encoding.UTF8, "application/json")
            };

            var client = CreateHttpClientReturning(response);

            _mockSettingStore.Setup(s => s.GetSettingValue(SettingKeys.BusinessProfileId, It.IsAny<Guid>())).ReturnsAsync("LOC123");

            var cut = CreateJob();

            // Act
            var locationId = await cut.GetLocationId(client, "acct", Guid.NewGuid(), CancellationToken.None);

            // Assert
            locationId.Should().Be("LOC123");
        }

        [Fact]
        public async Task GetLocationId_Throws_GoogleReviewSyncException_WhenNotSuccess()
        {
            // Arrange
            var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("bad")
            };

            var client = CreateHttpClientReturning(response);

            _mockSettingStore.Setup(s => s.GetSettingValue(SettingKeys.BusinessProfileId, It.IsAny<Guid>())).ReturnsAsync("IGNORED");

            var cut = CreateJob();

            // Act
            Func<Task> act = async () => await cut.GetLocationId(client, "acct", Guid.NewGuid(), CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<GoogleReviewSyncException>();
        }

        [Fact]
        public async Task GetReviews_Deserializes_Response()
        {
            // Arrange
            string json = JsonSerializer.Serialize(new
            {
                reviews = new[]
                {
                    new {
                        name = "reviews/1",
                        comment = "Great",
                        starRating = 5,
                        createTime = DateTime.UtcNow,
                        reviewer = new { displayName = "Alice", profilePhotoUrl = "https://img" }
                    }
                }
            });

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var client = CreateHttpClientReturning(response);

            var job = CreateJob();

            // Act
            var result = await job.GetReviews(client, "acct", "loc", CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Reviews.Should().NotBeNull();
        }


        [Fact]
        public async Task ExecuteAsync_Calls_SaveChanges_When_Reviews_Returned()
        {
            // Arrange
            var appicationTenantId = Guid.NewGuid();

            string json = JsonSerializer.Serialize(new
            {
                reviews = new[]
                {
                    new {
                        name = "reviews/1",
                        comment = "Great",
                        starRating = 5,
                        createTime = DateTime.UtcNow,
                        reviewer = new { displayName = "Alice", profilePhotoUrl = "https://img" }
                    }
                }
            });

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var mockTokenRepo = new Mock<IRepository<Token>>();

            mockTokenRepo.Setup(s => s.FirstOrDefaultAsync(It.IsAny<GetTokenByKeySpecification>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Token(Guid.NewGuid(), "google_api_key", "FAKE_KEY"));

            var mockReviewRepo = new Mock<IRepository<Review>>();

            _mockDataStore.Setup(s => s.GetRepository<Review>()).Returns(mockReviewRepo.Object);
            _mockDataStore.Setup(s => s.GetRepository<Token>()).Returns(mockTokenRepo.Object);

            var client = CreateHttpClientReturning(response);

            _mockClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client);

            var job = CreateJob();

            // Act
            await job.ExecuteAsync(appicationTenantId, CancellationToken.None);

            // Assert
            _mockDataStore.Verify(b => b.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        public async Task ConvertImageToLocalImage_UpdatesReviewerImageUrl_OnSuccess()
        {
            // Arrange
            var imageBytes = new byte[] { 1, 2, 3, 4 };
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(imageBytes)
            };

            var client = CreateHttpClientReturning(response);

            // client factory should return the http client used to fetch the image
            _mockClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client);
            // image processor will return a new memory stream (webp content)
            _mockImageProcessor.Setup(p => p.ConvertToWebP(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new MemoryStream(new byte[] { 9, 9 }));

            _mockBlobStore.Setup(b => b.SaveAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Stream>(), true, It.IsAny<CancellationToken>()))
                .ReturnsAsync("/img/reviews/generated.webp");

            var job = CreateJob();

            var review = new Review(Guid.NewGuid())
            {
                ReviewerImageUrl = "https://remote/image.png"
            };

            var method = typeof(GoogleReviewsSyncJob).GetMethod("ConvertImageToLocalImage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Act
            var task = (Task)method!.Invoke(job, new object[] { review, CancellationToken.None })!;
            await task;

            // Assert
            review.ReviewerImageUrl.Should().Be("/img/reviews/generated.webp");
            _mockBlobStore.Verify(b => b.SaveAsync(review.ApplicationTenantId, It.Is<string>(s => s.Contains("/img/reviews/") && s.EndsWith(".webp")), It.IsAny<Stream>(), true, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ConvertImageToLocalImage_DoesNothing_WhenReviewerImageUrlIsEmpty()
        {
            // Arrange
            var job = CreateJob();

            var review = new Review(Guid.NewGuid())
            {
                ReviewerImageUrl = string.Empty
            };

            // Act
            await job.ConvertImageToLocalImage(review, CancellationToken.None);

            // Assert - ensure no blobstore save or image processor calls happened
            _mockImageProcessor.Verify(p => p.ConvertToWebP(It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Never);
            _mockBlobStore.Verify(b => b.SaveAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        // Minimal fake handler used to return a prepared HttpResponseMessage
        private class FakeHttpMessageHandler : HttpMessageHandler
        {
            private readonly HttpResponseMessage _response;

            public FakeHttpMessageHandler(HttpResponseMessage response)
            {
                _response = response;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                // Return a cloned response to avoid disposed-content issues between calls
                var clone = new HttpResponseMessage(_response.StatusCode)
                {
                    Content = _response.Content == null ? null : new StringContent(_response.Content.ReadAsStringAsync(cancellationToken).GetAwaiter().GetResult(), Encoding.UTF8, _response.Content.Headers.ContentType?.MediaType),
                    RequestMessage = request
                };

                foreach (var header in _response.Headers)
                {
                    clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }

                return Task.FromResult(clone);
            }
        }
    }
}