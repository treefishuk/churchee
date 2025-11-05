using Azure.Storage.Blobs;
using Churchee.Blobstorage.Providers.Azure;
using Churchee.Test.Helpers.Validation;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Security.Cryptography;

namespace Churchee.Blobstorage.Providers.Tests.Azure
{
    public class AzureBlobStoreTests : IAsyncLifetime
    {
        private readonly IContainer _azuriteContainer;
        private readonly Mock<IConfiguration> _mockConfiguration;

        public AzureBlobStoreTests()
        {
            _azuriteContainer = new ContainerBuilder()
                .WithImage("mcr.microsoft.com/azure-storage/azurite:latest")
                .WithPortBinding(10000, true)
                .WithPortBinding(10001, true)
                .WithPortBinding(10002, true)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(10000))
                .Build();

            _mockConfiguration = new Mock<IConfiguration>();
        }

        public async Task InitializeAsync()
        {
            await _azuriteContainer.StartAsync();

            string connectionString = $"DefaultEndpointsProtocol=https;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:{_azuriteContainer.GetMappedPublicPort(10000)}/devstoreaccount1;";

            _mockConfiguration.Setup(s => s.GetSection("ConnectionStrings")["Storage"]).Returns(connectionString);
        }

        public async Task DisposeAsync()
        {
            await _azuriteContainer.DisposeAsync().AsTask();
        }

        [Fact]
        public async Task GetAsync_ShouldReturnStream_WhenBlobExists()
        {
            // Arrange
            var blobStore = new AzureBlobStore(_mockConfiguration.Object);

            var applicationTenantId = Guid.NewGuid();
            string fullPath = "test/blob.txt";
            var cancellationToken = CancellationToken.None;

            var blobServiceClient = new BlobServiceClient(_mockConfiguration.Object.GetConnectionString("Storage"));
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(applicationTenantId.ToString());
            await blobContainerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
            var blobClient = blobContainerClient.GetBlobClient(fullPath);
            await blobClient.UploadAsync(new MemoryStream(new byte[] { 1, 2, 3 }), cancellationToken: cancellationToken);

            // Act
            var result = await blobStore.GetReadStreamAsync(applicationTenantId, fullPath, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<Stream>();
        }

        [Fact]
        public async Task SaveAsync_ShouldUploadBlob_WhenCalled_AndDosentAlreadyExist()
        {
            // Arrange
            var blobStore = new AzureBlobStore(_mockConfiguration.Object);

            var applicationTenantId = Guid.NewGuid();
            string fullPath = "test/blob.txt";
            var stream = new MemoryStream(new byte[] { 1, 2, 3 });
            var cancellationToken = CancellationToken.None;

            // Act
            string result = await blobStore.SaveAsync(applicationTenantId, fullPath, stream, false, cancellationToken);

            // Assert
            result.Should().Be(fullPath);

            var blobServiceClient = new BlobServiceClient(_mockConfiguration.Object.GetConnectionString("Storage"));
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(applicationTenantId.ToString());
            var blobClient = blobContainerClient.GetBlobClient(fullPath);
            var exists = await blobClient.ExistsAsync(cancellationToken);
            exists.Value.Should().BeTrue();
        }

        [Fact]
        public async Task SaveAsync_ShouldUploadBlob_WhenCalled_AndAlreadyExist_AndOverrideTrue()
        {
            // Arrange
            var blobStore = new AzureBlobStore(_mockConfiguration.Object);

            var applicationTenantId = Guid.NewGuid();
            string fullPath = "test/blob.txt";
            var stream = new MemoryStream(new byte[] { 1, 2, 3 });
            var cancellationToken = CancellationToken.None;
            var blobServiceClient = new BlobServiceClient(_mockConfiguration.Object.GetConnectionString("Storage"));
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(applicationTenantId.ToString());
            await blobContainerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
            var blobClient = blobContainerClient.GetBlobClient(fullPath);
            await blobClient.UploadAsync(new MemoryStream(new byte[] { 1, 2, 3 }), cancellationToken: cancellationToken);

            // Act
            string result = await blobStore.SaveAsync(applicationTenantId, fullPath, stream, true, cancellationToken);

            // Assert
            result.Should().Be(fullPath);
            var exists = await blobClient.ExistsAsync(cancellationToken);
            exists.Value.Should().BeTrue();
        }

        [Fact]
        public async Task SaveAsync_ShouldUploadINcrementedBlob_WhenCalled_AndAlreadyExist_AndOverrideFalse()
        {
            // Arrange
            var blobStore = new AzureBlobStore(_mockConfiguration.Object);

            var applicationTenantId = Guid.NewGuid();
            string fullPath = "test/blob.txt";
            var stream = new MemoryStream(new byte[] { 1, 2, 3 });
            var cancellationToken = CancellationToken.None;
            var blobServiceClient = new BlobServiceClient(_mockConfiguration.Object.GetConnectionString("Storage"));
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(applicationTenantId.ToString());
            await blobContainerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
            var blobClient = blobContainerClient.GetBlobClient(fullPath);
            await blobClient.UploadAsync(new MemoryStream(new byte[] { 1, 2, 3 }), cancellationToken: cancellationToken);

            // Act
            string result = await blobStore.SaveAsync(applicationTenantId, fullPath, stream, false, cancellationToken);

            // Assert
            result.Should().Be("test/blob(1).txt");
            var exists = await blobClient.ExistsAsync(cancellationToken);
            exists.Value.Should().BeTrue();
        }

        [Fact]
        public async Task SaveAsync_ShouldCreateContainer_WhenItDoesNotExist()
        {
            // Arrange
            var blobStore = new AzureBlobStore(_mockConfiguration.Object);

            var applicationTenantId = Guid.NewGuid();
            string fullPath = "test/blob.txt";
            var stream = new MemoryStream(new byte[] { 1, 2, 3 });
            var cancellationToken = CancellationToken.None;

            // Act
            string result = await blobStore.SaveAsync(applicationTenantId, fullPath, stream, false, cancellationToken);

            // Assert
            result.Should().Be(fullPath);

            var blobServiceClient = new BlobServiceClient(_mockConfiguration.Object.GetConnectionString("Storage"));
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(applicationTenantId.ToString());
            var exists = await blobContainerClient.ExistsAsync(cancellationToken);
            exists.Value.Should().BeTrue();
        }

        [Fact]
        public async Task WriteChunksAsync_ShouldUploadBlobInChunks_AndReportProgress()
        {
            // Arrange
            var blobStore = new AzureBlobStore(_mockConfiguration.Object);

            var applicationTenantId = Guid.NewGuid();
            string fullPath = "test/chunkedblob.txt";
            var cancellationToken = CancellationToken.None;

            // Create a test file with size > 1MB to ensure chunking
            byte[] fileBytes = new byte[2 * 1024 * 1024 + 500]; // ~2MB
            RandomNumberGenerator.Fill(fileBytes);

            // Mock IBrowserFile
            var browserFileMock = new Mock<IBrowserFile>();
            browserFileMock.Setup(f => f.OpenReadStream(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .Returns(new MemoryStream(fileBytes));
            browserFileMock.Setup(f => f.Size).Returns(fileBytes.Length);

            // Track progress
            var progressUpdates = new List<double>();
            var progressMock = new Mock<IProgress<double>>();
            progressMock.Setup(p => p.Report(It.IsAny<double>()))
                .Callback<double>(v => progressUpdates.Add(v));

            // Act
            await blobStore.WriteChunksAsync(applicationTenantId, fullPath, browserFileMock.Object, progressMock.Object, cancellationToken);

            // Assert
            var blobServiceClient = new BlobServiceClient(_mockConfiguration.Object.GetConnectionString("Storage"));
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(applicationTenantId.ToString());
            var blobClient = blobContainerClient.GetBlobClient(fullPath);
            var exists = await blobClient.ExistsAsync(cancellationToken);
            exists.Value.Should().BeTrue();

            // Download and verify content
            var downloaded = await blobClient.DownloadContentAsync(cancellationToken);
            downloaded.Value.Content.ToArray().Should().BeEquivalentTo(fileBytes);

            // Progress should be reported multiple times and end at 100
            Assert.NotNull(progressUpdates);
            Assert.True(Math.Abs(progressUpdates.Last() - 100.0) < 0.01);
        }
    }
}
