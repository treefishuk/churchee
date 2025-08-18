using Bogus;
using Churchee.Data.EntityFramework.Admin;
using Churchee.Module.Dashboard.Entities;
using Churchee.Module.Dashboard.Features.Queries;
using Churchee.Module.Dashboard.Features.Queries.GetDashboardData;
using Churchee.Module.Dashboard.Tests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Diagnostics;
using Testcontainers.MsSql;

namespace Churchee.Module.Dashboard.Tests.Features.Queries
{
    public class GetDashboardDataQueryHandlerTests : IAsyncLifetime
    {
        private readonly MsSqlContainer _msSqlContainer;

        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly Mock<ILogger<GetDashboardDataQueryHandler>> _mockLogger;
        private readonly Faker _faker;

        public GetDashboardDataQueryHandlerTests()
        {
            _mockLogger = new Mock<ILogger<GetDashboardDataQueryHandler>>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            _msSqlContainer = new MsSqlBuilder()
             .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
             .WithPassword("yourStrong(!)Password")
             .Build();

            _faker = new Faker();
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
        public async Task Handle_Returns_Data_In_Under_One_Second()
        {
            // Arrange
            var query = new GetDashboardDataQuery(7);
            var cancellationToken = new CancellationToken();

            var tenantId = Ids.TenantId;

            await SetupLargeDataSet(tenantId, cancellationToken);

            var optionsBuilder = new DbContextOptionsBuilder<DashboardDataTestDbContext>();

            optionsBuilder.UseSqlServer(_msSqlContainer.GetConnectionString());

            var dbContext = new DashboardDataTestDbContext(optionsBuilder.Options);

            var efStorage = new EFStorage(dbContext, _mockHttpContextAccessor.Object);

            var handler = new GetDashboardDataQueryHandler(efStorage, _mockLogger.Object);

            // Warm up (optional, but helps with JIT and DB caching)
            await handler.Handle(query, cancellationToken);

            // Run each handler 10 times and record elapsed times
            var handlerTimes = new List<long>();

            for (int i = 0; i < 2; i++)
            {
                Console.WriteLine($"Running iteration {i + 1}...");

                var swNew = Stopwatch.StartNew();
                await handler.Handle(query, cancellationToken);
                swNew.Stop();
                handlerTimes.Add(swNew.ElapsedMilliseconds);

                Console.WriteLine($"Handler Time: {swNew.ElapsedMilliseconds} ms");

            }

            double average = handlerTimes.Average();

            Console.WriteLine($"Handler Avg: {average} ms");

            Assert.True(average < 1000, $"Expected new handler to be under 1000ms, Speed: {average}ms");
        }


        [Fact]
        public async Task Handle_Returns_Correct_Counts()
        {
            // Arrange
            var query = new GetDashboardDataQuery(0);
            var cancellationToken = new CancellationToken();

            var tenantId = Ids.TenantId;

            await SetupCountsDataSet(tenantId, cancellationToken);

            var optionsBuilder = new DbContextOptionsBuilder<DashboardDataTestDbContext>();
            optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information)
                          .EnableSensitiveDataLogging();
            optionsBuilder.UseSqlServer(_msSqlContainer.GetConnectionString());

            var dbContext = new DashboardDataTestDbContext(optionsBuilder.Options);

            var efStorage = new EFStorage(dbContext, _mockHttpContextAccessor.Object);

            var newHandler = new GetDashboardDataQueryHandler(efStorage, _mockLogger.Object);

            var response = await newHandler.Handle(query, cancellationToken);

            Assert.True(response.UniqueVisitors == 2, $"Expected 2 unique visitors, but got {response.UniqueVisitors}.");
            Assert.True(response.ReturningVisitors == 1, $"Expected 1 returning visitor, but got {response.ReturningVisitors}.");
        }


        private async Task SetupCountsDataSet(Guid appTenantId, CancellationToken cancellationToken)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DashboardDataTestDbContext>();

            optionsBuilder.UseSqlServer(_msSqlContainer.GetConnectionString());

            var dbContext = new DashboardDataTestDbContext(optionsBuilder.Options);

            var efStorage = new EFStorage(dbContext, _mockHttpContextAccessor.Object);

            var query = new GetDashboardDataQuery(0);

            var userId = Guid.NewGuid();

            var uniqueVisitor = new PageView(appTenantId)
            {
                Url = "/",
                UserAgent = "Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:72.0) Gecko/20100101 Firefox/72.0",
                IpAddress = "192.168.0.1",
                ViewedAt = DateTime.Now,
                Device = "desktop",
                Deleted = false
            };

            var uniqueVisitor2 = new PageView(appTenantId)
            {
                Url = "/",
                UserAgent = "Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:72.0) Gecko/20100101 Firefox/72.0",
                IpAddress = "192.168.0.2",
                ViewedAt = DateTime.Now,
                Device = "desktop",
                Deleted = false
            };

            var recurringYesterday = new PageView(appTenantId)
            {
                Url = "/",
                UserAgent = "Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:72.0) Gecko/20100101 Firefox/72.0",
                IpAddress = "192.168.0.3",
                ViewedAt = DateTime.Now.AddDays(-2),
                Device = "desktop",
                Deleted = false
            };

            var recurringVisitorToday = new PageView(appTenantId)
            {
                Url = "/",
                UserAgent = "Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:72.0) Gecko/20100101 Firefox/72.0",
                IpAddress = "192.168.0.3",
                ViewedAt = DateTime.Now,
                Device = "desktop",
                Deleted = false
            };

            var repository = efStorage.GetRepository<PageView>();

            repository.AddRange([recurringYesterday, recurringVisitorToday, uniqueVisitor, uniqueVisitor2]);

            await efStorage.SaveChangesAsync(cancellationToken);
        }


        private async Task SetupLargeDataSet(Guid appTenantId, CancellationToken cancellationToken)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DashboardDataTestDbContext>();

            optionsBuilder.UseSqlServer(_msSqlContainer.GetConnectionString());

            var dbContext = new DashboardDataTestDbContext(optionsBuilder.Options);

            var efStorage = new EFStorage(dbContext, _mockHttpContextAccessor.Object);

            var query = new GetDashboardDataQuery(7);

            var userId = Guid.NewGuid();

            string[] devices = ["mobile", "tablet", "desktop"];
            string[] systems = ["Windows", "andriod", "iOS"];
            string[] pages = ["/", "/about", "/whats-on", "/whats-on/event", "/contact"];
            string[] browsers = ["chrome", "firefox", "edge"];
            string[] referrers = ["google", "chat-gpt", "local"];
            string[] agents = [
                "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/113.0.0.0 Safari/537.3",
                "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/17.10 Safari/605.1.1"];

            var testDataBuilder = new Faker<PageView>()
                .CustomInstantiator(f => new PageView(appTenantId))
                .StrictMode(true)
                .RuleFor(o => o.ApplicationTenantId, f => appTenantId)
                .RuleFor(o => o.Id, f => Guid.NewGuid())
                .RuleFor(o => o.Device, f => f.PickRandom(devices))
                .RuleFor(o => o.UserAgent, f => f.PickRandom(agents))
                .RuleFor(o => o.Url, f => f.PickRandom(pages))
                .RuleFor(o => o.OS, f => f.PickRandom(systems))
                .RuleFor(o => o.Browser, f => f.PickRandom(browsers))
                .RuleFor(o => o.Referrer, f => f.PickRandom(referrers))
                .RuleFor(o => o.ViewedAt, f => f.Date.Between(DateTime.UtcNow.AddMonths(-1), DateTime.UtcNow))
                .RuleFor(o => o.CreatedDate, f => f.Date.Between(DateTime.UtcNow.AddMonths(-1), DateTime.UtcNow))
                .RuleFor(o => o.ModifiedDate, f => f.Date.Between(DateTime.UtcNow.AddMonths(-1), DateTime.UtcNow))
                .RuleFor(o => o.Deleted, f => false)
                .RuleFor(o => o.CreatedById, f => userId)
                .RuleFor(o => o.ModifiedById, f => userId)
                .RuleFor(o => o.CreatedByUser, f => "System")
                .RuleFor(o => o.ModifiedByName, f => "System")
                .RuleFor(o => o.IpAddress, f => f.Internet.IpAddress().ToString());


            var repository = efStorage.GetRepository<PageView>();

            const int batchSize = 1000;

            int totalCount = 100_000;

            for (int i = 0; i < totalCount; i += batchSize)
            {
                Console.WriteLine($"Adding batch {(i / batchSize) + 1} of {totalCount / batchSize}");

                var pageViews = testDataBuilder.Generate(batchSize);
                repository.AddRange(pageViews);
                await efStorage.SaveChangesAsync(cancellationToken);
            }
        }
    }
}




