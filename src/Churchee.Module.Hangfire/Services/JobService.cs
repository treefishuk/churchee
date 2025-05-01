using Churchee.Common.Abstractions.Queue;
using Hangfire;
using Hangfire.Storage;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Churchee.Module.Hangfire.Services
{
    public class JobService : IJobService
    {
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public JobService(IRecurringJobManager recurringJobManager, IBackgroundJobClient backgroundJobClient)
        {
            _recurringJobManager = recurringJobManager;
            _backgroundJobClient = backgroundJobClient;
        }

        public DateTime? GetLastRunDate(string recurringJobId)
        {
            DateTime? lastRun = null;

            using (var connection = JobStorage.Current.GetConnection())
            {
                var recurringJobs = connection.GetRecurringJobs();

                lastRun = recurringJobs.Where(w => w.Id == recurringJobId).Select(s => s.LastExecution).FirstOrDefault();
            }

            return lastRun;

        }

        public void QueueJob(Expression<Func<Task>> methodCall)
        {
            _backgroundJobClient.Enqueue(methodCall);
        }

        public void QueueJob<T>(Expression<Func<T, Task>> methodCall)
        {
            _backgroundJobClient.Enqueue<T>(methodCall);
        }

        public void RemoveScheduledJob(string recurringJobId)
        {
            _recurringJobManager.RemoveIfExists(recurringJobId);
        }

        public void ScheduleJob(string recurringJobId, Expression<Func<Task>> methodCall, Func<string> cronExpression)
        {
            _recurringJobManager.AddOrUpdate(recurringJobId, methodCall, cronExpression);
        }
    }
}
