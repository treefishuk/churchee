using Churchee.Common.Storage;
using Churchee.Data.EntityFramework.Admin.Registrations;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Testcontainers.MsSql;

namespace Churchee.Data.EntityFramework.Admin.Tests.Registrations
{
    public class ServiceRegistrationsTests : IAsyncLifetime
    {
        private readonly MsSqlContainer _msSqlContainer;

        public ServiceRegistrationsTests()
        {
            _msSqlContainer = new MsSqlBuilder()
             .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
             .WithPassword("yourStrong(!)Password")
             .Build();
        }

        public async Task InitializeAsync()
        {
            await _msSqlContainer.StartAsync();
        }

        public async Task DisposeAsync()
        {
            await _msSqlContainer.DisposeAsync().AsTask();
        }

        [Fact]
        public void ServiceRegistrations_ShouldReturnExpectedServices()
        {
            // Arrange
            var services = new ServiceCollection();

            // Mock Mediator
            var mediatorMock = new Mock<IMediator>();
            services.AddSingleton(mediatorMock.Object);

            // Mock Logger
            var logger = new Mock<ILogger<ApplicationDbContext>>();
            services.AddSingleton(logger.Object);

            // Mock Configuration
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(s => s.GetSection("ConnectionStrings")["DefaultConnection"]).Returns(_msSqlContainer.GetConnectionString());

            var mockConfigurationSection = new Mock<IConfigurationSection>();
            mockConfigurationSection.Setup(s => s["DPK"]).Returns("TestDPKValue");
            mockConfigurationSection.Setup(s => s.Path).Returns("Security");
            mockConfigurationSection.Setup(s => s.Key).Returns("Security");
            mockConfigurationSection.Setup(s => s.Value).Returns(string.Empty);

            mockConfiguration.Setup(s => s.GetSection("Security")).Returns(mockConfigurationSection.Object);

            services.AddSingleton(mockConfiguration.Object);

            // Mock IHttpContextAccessor
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var mockHttpContext = new DefaultHttpContext();
            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(mockHttpContext);

            services.AddSingleton(mockHttpContextAccessor.Object);

            try
            {
                // Create a test .pfx file
                var certificate = CreateSelfSignedCertificate();
                File.WriteAllBytes("dp.pfx", certificate.Export(X509ContentType.Pfx));

                var serviceRegistrations = new ServiceRegistrations();

                var serviceProvider = services.BuildServiceProvider();

                // Act
                serviceRegistrations.Execute(services, serviceProvider);

                serviceProvider = services.BuildServiceProvider();

                // Assert
                serviceProvider.GetService<ApplicationDbContext>().Should().NotBeNull();
                serviceProvider.GetService<DbContext>().Should().NotBeNull();
                serviceProvider.GetService<IDataStore>().Should().NotBeNull();
            }
            finally
            {
                // Cleanup: Delete the test .pfx file
                if (File.Exists("dp.pfx"))
                {
                    File.Delete("dp.pfx");
                }
            }

        }

        private static X509Certificate2 CreateSelfSignedCertificate()
        {
            using var rsa = RSA.Create(2048);
            var request = new CertificateRequest(
                "CN=TestCertificate",
                rsa,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1);

            var certificate = request.CreateSelfSigned(
                DateTimeOffset.Now.AddDays(-1),
                DateTimeOffset.Now.AddYears(1));

            return certificate;
        }
    }
}
