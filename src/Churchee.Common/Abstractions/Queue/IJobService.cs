using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Churchee.Common.Abstractions.Queue
{
    public interface IJobService
    {
        void SheduleJob(string recurringJobId, Expression<Func<Task>> methodCall, Func<string> cronExpression);

        void QueueJob(Expression<Func<Task>> methodCall);

        void QueueJob<T>(Expression<Func<T, Task>> methodCall);
    }
}
