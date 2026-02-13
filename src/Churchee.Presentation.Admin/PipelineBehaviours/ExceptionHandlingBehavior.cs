using Churchee.Common.ResponseTypes;
using MediatR;

namespace Churchee.Presentation.Admin.PipelineBehaviours
{
    public class ExceptionHandlingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : CommandResponse
    {

        private readonly ILogger<ExceptionHandlingBehavior<TRequest, TResponse>> _logger;

        public ExceptionHandlingBehavior(ILogger<ExceptionHandlingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            try
            {
                return await next(cancellationToken);
            }
            catch (Exception ex)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError(ex, "Exception caught in pipeline: {Message}", ex.Message);
                }

                // Try to create the specific TResponse type
                try
                {
                    if (Activator.CreateInstance<TResponse>() is CommandResponse specific)
                    {
                        specific.AddError("Oh dear something has gone wrong... please try again. If the issue persists please contact support", string.Empty);
                        return (TResponse)(object)specific;
                    }
                }
                catch (Exception createEx)
                {
                    _logger.LogError(createEx, "Failed to construct TResponse in exception handler.");
                }

                // If we get here we can't produce a TResponse instance reliably - rethrow to avoid returning an invalid object
                throw;
            }
        }
    }
}
