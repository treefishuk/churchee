using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Churchee.Common.Abstractions.Queue
{
    public interface IJobService
    {
        void ScheduleJob(string recurringJobId, Expression<Func<Task>> methodCall, Func<string> cronExpression);

        void ScheduleJob<T>(string recurringJobId, Expression<Func<T, Task>> methodCall, Func<string> cronExpression);

        void RemoveScheduledJob(string recurringJobId);

        void QueueJob(Expression<Func<Task>> methodCall);

        void QueueJob<T>(Expression<Func<T, Task>> methodCall);

        DateTime? GetLastRunDate(string recurringJobId);
    }
}
