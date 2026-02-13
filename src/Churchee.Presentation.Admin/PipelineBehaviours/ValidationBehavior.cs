using Churchee.Common.ResponseTypes;
using FluentValidation;
using MediatR;

namespace Churchee.Presentation.Admin.PipelineBehaviours
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : CommandResponse
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

                // Ensure TResponse inherits from CommandResponse
                if (!typeof(CommandResponse).IsAssignableFrom(typeof(TResponse)))
                {
                    throw new InvalidOperationException(
                        "TResponse must inherit from CommandResponse.");
                }

                // Create the correct response type
                var response = Activator.CreateInstance<TResponse>();

                foreach (var failure in failures)
                {
                    string propertyName = failure.PropertyName[(failure.PropertyName.IndexOf('.') + 1)..];

                    ((CommandResponse)(object)response).AddError(failure.ErrorMessage, propertyName);
                }

                return Task.FromResult(response);
            }

            return next(cancellationToken);
        }

    }
}
