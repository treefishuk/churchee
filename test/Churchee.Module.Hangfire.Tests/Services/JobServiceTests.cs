using Churchee.Module.Hangfire.Services;
using Churchee.Test.Helpers.Validation;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Hangfire.Storage;
using Moq;
using System.Linq.Expressions;

namespace Churchee.Module.Hangfire.Tests.Services
{
    public class JobServiceTests
    {
        [Fact]
        public void QueueJob_ShouldCallCreateOnBackgroundJobClient_WithCorrectJob()
        {
            // Arrange
            var bgClientMock = new Mock<IBackgroundJobClient>();
            Job? capturedJob = null;

            bgClientMock
                .Setup(b => b.Create(It.IsAny<Job>(), It.IsAny<IState>()))
                .Callback<Job, IState>((job, state) => capturedJob = job)
                .Returns("job-id");

            var sut = new JobService(Mock.Of<IRecurringJobManager>(), bgClientMock.Object);
            Expression<Func<Task>> expression = () => DummyJobHelper.DummyTaskMethod();

            // Act
            sut.QueueJob(expression);

            // Assert
            bgClientMock.Verify(b => b.Create(It.IsAny<Job>(), It.IsAny<IState>()), Times.Once);
            capturedJob.Should().NotBeNull();
            capturedJob!.Method.Should().NotBeNull();
            capturedJob.Method.Name.Should().Be(nameof(DummyJobHelper.DummyTaskMethod));
            capturedJob.Type.Should().Be(typeof(DummyJobHelper));
        }

        [Fact]
        public void QueueJob_Generic_ShouldCallCreateOnBackgroundJobClient_WithCorrectJobTypeAndMethod()
        {
            // Arrange
            var bgClientMock = new Mock<IBackgroundJobClient>();
            Job? capturedJob = null;

            bgClientMock
                .Setup(b => b.Create(It.IsAny<Job>(), It.IsAny<IState>()))
                .Callback<Job, IState>((job, state) => capturedJob = job)
                .Returns("job-id-generic");

            var sut = new JobService(Mock.Of<IRecurringJobManager>(), bgClientMock.Object);
            Expression<Func<ITestWorker, Task>> expression = (w) => w.DoWork();

            // Act
            sut.QueueJob(expression);

            // Assert
            bgClientMock.Verify(b => b.Create(It.IsAny<Job>(), It.IsAny<IState>()), Times.Once);
            capturedJob.Should().NotBeNull();
            capturedJob!.Type.Should().Be(typeof(ITestWorker));
            capturedJob.Method.Name.Should().Be(nameof(ITestWorker.DoWork));
        }

        [Fact]
        public void RemoveScheduledJob_ShouldCallRemoveIfExists()
        {
            // Arrange
            var recurringMock = new Mock<IRecurringJobManager>();
            var sut = new JobService(recurringMock.Object, Mock.Of<IBackgroundJobClient>());
            string jobId = "recurring-id";

            // Act
            sut.RemoveScheduledJob(jobId);

            // Assert
            recurringMock.Verify(r => r.RemoveIfExists(jobId), Times.Once);
        }

        [Fact]
        public void ScheduleJob_ShouldCallAddOrUpdate_WithTaskExpressionAndCron()
        {
            // Arrange
            var recurringMock = new Mock<IRecurringJobManager>();
            string? capturedCron = null;
            string? capturedId = null;

            // Mock the underlying IRecurringJobManager.AddOrUpdate(string, Job, string, RecurringJobOptions)
            recurringMock
                .Setup(r => r.AddOrUpdate(It.IsAny<string>(), It.IsAny<Job>(), It.IsAny<string>(), It.IsAny<RecurringJobOptions>()))
                .Callback<string, Job, string, RecurringJobOptions>((id, job, cron, options) =>
                {
                    capturedId = id;
                    capturedCron = cron;
                });

            var sut = new JobService(recurringMock.Object, Mock.Of<IBackgroundJobClient>());
            string jobId = "job-id";
            Expression<Func<Task>> expression = () => DummyJobHelper.DummyTaskMethod();
            static string cronExpression()
            {
                return "0 0 * * *";
            }

            // Act
            sut.ScheduleJob(jobId, expression, cronExpression);

            // Assert
            recurringMock.Verify(r => r.AddOrUpdate(It.IsAny<string>(), It.IsAny<Job>(), It.IsAny<string>(), It.IsAny<RecurringJobOptions>()), Times.Once);
            capturedCron?.Should().NotBeNull();
            capturedCron?.Should().Be(cronExpression());
            capturedId?.Should().Be(jobId);

        }

        [Fact]
        public void ScheduleJob_Generic_ShouldCallAddOrUpdate_WithGenericExpressionAndCron()
        {
            // Arrange
            var recurringMock = new Mock<IRecurringJobManager>();
            Job? capturedJob = null;
            string? capturedCron = null;
            string? capturedId = null;
            RecurringJobOptions? capturedOptions = null;

            // Mock the underlying IRecurringJobManager.AddOrUpdate(string, Job, string, RecurringJobOptions)
            recurringMock
                .Setup(r => r.AddOrUpdate(It.IsAny<string>(), It.IsAny<Job>(), It.IsAny<string>(), It.IsAny<RecurringJobOptions>()))
                .Callback<string, Job, string, RecurringJobOptions>((id, job, cron, options) =>
                {
                    capturedId = id;
                    capturedJob = job;
                    capturedCron = cron;
                    capturedOptions = options;
                });

            var sut = new JobService(recurringMock.Object, Mock.Of<IBackgroundJobClient>());
            string jobId = "job-id-generic";
            Expression<Func<ITestWorker, Task>> expression = (w) => w.DoWork();
            static string cronExpression()
            {
                return "*/5 * * * *";
            }

            // Act
            sut.ScheduleJob(jobId, expression, cronExpression);

            // Assert
            // Verify the low-level AddOrUpdate was invoked (extension method translates to this)
            recurringMock.Verify(r => r.AddOrUpdate(It.IsAny<string>(), It.IsAny<Job>(), It.IsAny<string>(), It.IsAny<RecurringJobOptions>()), Times.Once);

            capturedJob.Should().NotBeNull();
            capturedJob!.Type.Should().Be(typeof(ITestWorker));
            capturedJob.Method.Name.Should().Be(nameof(ITestWorker.DoWork));

            capturedId?.Should().Be(jobId);
            capturedCron?.Should().Be(cronExpression());
        }


        [Fact]
        public void GetLastRunDate_ShouldReturnLastExecutionForMatchingRecurringJob()
        {
            // Arrange

            string recurringJobId = "my-recurring-job";
            var expectedLastExecution = DateTime.UtcNow.AddHours(-1);

            // Mock IStorageConnection so the StorageConnectionExtensions.GetRecurringJobs extension
            // will call into our mocked methods (GetAllItemsFromSet/GetAllEntriesFromHash).
            var connectionMock = new Mock<IStorageConnection>();

            connectionMock
                .Setup(c => c.GetAllItemsFromSet("recurring-jobs"))
                .Returns(["other-job", recurringJobId]);

            // Provide hash entries for each recurring job id. Return LastExecution as ISO 8601 string.
            connectionMock
                .Setup(c => c.GetAllEntriesFromHash(It.IsAny<string>()))
                .Returns((string key) =>
                {
                    if (key == $"recurring-job:{recurringJobId}")
                    {
                        return new Dictionary<string, string>
                        {
                            ["LastExecution"] = expectedLastExecution.ToString("o")
                        };
                    }

                    // other job has an earlier last execution
                    return new Dictionary<string, string>
                    {
                        ["LastExecution"] = DateTime.UtcNow.AddDays(-1).ToString("o")
                    };
                });

            // Create a test JobStorage that returns our mocked connection
            var testStorage = new TestJobStorage(connectionMock.Object, Mock.Of<IMonitoringApi>());
            JobStorage.Current = testStorage;

            var sut = new JobService(Mock.Of<IRecurringJobManager>(), Mock.Of<IBackgroundJobClient>());

            // Act
            var actual = sut.GetLastRunDate(recurringJobId);

            // Assert
            actual?.Should().BeCloseTo(expectedLastExecution, TimeSpan.FromSeconds(1));

        }

        // Minimal test JobStorage used to swap JobStorage.Current during tests
        private class TestJobStorage : JobStorage
        {
            private readonly IStorageConnection _connection;
            private readonly IMonitoringApi _monitoringApi;

            public TestJobStorage(IStorageConnection connection, IMonitoringApi monitoringApi)
            {
                _connection = connection;
                _monitoringApi = monitoringApi;
            }

            public override IStorageConnection GetConnection()
            {
                return _connection;
            }

            public override IMonitoringApi GetMonitoringApi()
            {
                return _monitoringApi;
            }
        }

        public interface ITestWorker
        {
            Task DoWork();
        }
    }
}