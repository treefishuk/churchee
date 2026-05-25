using Churchee.Common.ResponseTypes;
using Churchee.CQRS.Abstractions;
using FluentValidation;
using FluentValidation.Results;

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

            var failures = new List<ValidationFailure>();

            var context = new ValidationContext<TRequest>(request);

            foreach (var validator in _validators)
            {
                var result = await validator.ValidateAsync(context, cancellationToken);
                if (!result.IsValid)
                {
                    failures.AddRange(result.Errors);
                }
            }

            if (failures.Count != 0)
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
