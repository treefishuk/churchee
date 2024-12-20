using Churchee.Common.Abstractions.Queue;
using Hangfire;
using System;
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

        public void QueueJob(Expression<Func<Task>> methodCall)
        {
            _backgroundJobClient.Enqueue(methodCall);
        }

        public void QueueJob<T>(Expression<Func<T, Task>> methodCall)
        {
            _backgroundJobClient.Enqueue<T>(methodCall);
        }

        public void SheduleJob(string recurringJobId, Expression<Func<Task>> methodCall, Func<string> cronExpression)
        {
            _recurringJobManager.AddOrUpdate(recurringJobId, methodCall, cronExpression);
        }
    }
}
