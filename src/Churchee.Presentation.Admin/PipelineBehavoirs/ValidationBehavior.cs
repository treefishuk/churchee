using Churchee.Common.ResponseTypes;
using FluentValidation;
using MediatR;

namespace Churchee.Presentation.Admin.PipelineBehavoirs
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : class
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var failures = _validators.Select(x => x.Validate(request)).SelectMany(m => m.Errors).Where(w => w != null);

            if (failures.Any())
            {
                var response = new CommandResponse();

                foreach (var failure in failures)
                {
                    string propertyName = failure.PropertyName.Substring(failure.PropertyName.IndexOf('.') + 1);

                    response.AddError(failure.ErrorMessage, propertyName);
                }

                if (response is TResponse castResponse)
                {
                    return Task.FromResult(castResponse);
                }

                throw new InvalidOperationException("TResponse must be of type CommandResponse.");
            }

            return next();
        }

    }
}
