using Churchee.Common.ResponseTypes;
using MediatR;

namespace Churchee.Presentation.Admin.PipelineBehavoirs
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
                return await next();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception caught in pipeline: {message}", ex.Message);

                var response = new CommandResponse();

                response.AddError("Oh dear something has gone wrong... please try again. If the issue persists please contact support", string.Empty);

                return (TResponse)(object)response;
            }
        }
    }
}
