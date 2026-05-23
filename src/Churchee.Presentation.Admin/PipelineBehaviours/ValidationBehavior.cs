using Churchee.Common.ResponseTypes;
using Churchee.CQRS.Abstractions;
using FluentValidation;

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

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {

            if (!_validators.Any())
            {
                return await next();
            }

            var failures = _validators.Select(x => x.Validate(request)).SelectMany(m => m.Errors).Where(w => w != null);

            if (failures.Any())
            {
                var response = Activator.CreateInstance<TResponse>();

                foreach (var failure in failures)
                {
                    string propertyName = failure.PropertyName[(failure.PropertyName.IndexOf('.') + 1)..];

                    ((CommandResponse)(object)response).AddError(failure.ErrorMessage, propertyName);
                }

                return await Task.FromResult(response);
            }

            return await next();
        }

    }
}
