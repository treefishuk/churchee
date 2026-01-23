using Churchee.Common.Abstractions.Queue;
using Churchee.Module.Logging.Exceptions;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Churchee.Module.Logging.Jobs
{
    public class ErrorLoggingTestJob : ISystemJob
    {
        private readonly IHostEnvironment _environment;

        public ErrorLoggingTestJob(IHostEnvironment environment)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        public Task ExecuteAsync(CancellationToken cancellationToken)
        {
            string envName = _environment.EnvironmentName ?? string.Empty;

            return _environment.IsDevelopment() || string.Equals(envName, "Local", StringComparison.OrdinalIgnoreCase)
                ? Task.CompletedTask
                : throw new TestException("Start up Exception Logging Test");
        }
    }
}
