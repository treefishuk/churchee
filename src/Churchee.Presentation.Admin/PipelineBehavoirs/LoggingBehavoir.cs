using MediatR;

namespace Churchee.Presentation.Admin.PipelineBehavoirs
{
    public class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger;

        public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {Name}", typeof(TResponse).Name);

            var response = await next();

            _logger.LogInformation("Returned {Name}", typeof(TResponse).Name);

            return response;
        }
    }
}