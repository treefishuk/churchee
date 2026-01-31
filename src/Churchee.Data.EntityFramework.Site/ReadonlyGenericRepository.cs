using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Churchee.Common.Abstractions.Entities;
using Churchee.Data.EntityFramework.Shared;
using Microsoft.EntityFrameworkCore;

namespace Churchee.Data.EntityFramework.Site
{
    public class ReadonlyGenericRepository<T> : GenericRepository<T> where T : class, IEntity
    {
        private readonly SpecificationEvaluator _specificationEvaluator;
        private const string NotSupportedMessage = "Not supported in the readonly version of this repository";

        public ReadonlyGenericRepository(DbContext dbContext) : base(dbContext)
        {
            _specificationEvaluator = SpecificationEvaluator.Default;
        }

        public override void Create(T entity)
        {
            throw new InvalidOperationException(NotSupportedMessage);
        }

        public override void Update(T entity)
        {
            throw new InvalidOperationException(NotSupportedMessage);
        }

        public override void AddRange(IEnumerable<T> entities)
        {
            throw new InvalidOperationException(NotSupportedMessage);
        }

        public override async Task SoftDelete<TId>(TId id)
        {
            throw new InvalidOperationException(NotSupportedMessage);
        }

        public override void PermanentDelete(T entity)
        {
            throw new InvalidOperationException(NotSupportedMessage);
        }

        public override async Task PermanentDelete(Guid id)
        {
            throw new InvalidOperationException(NotSupportedMessage);
        }

        public override async Task PermanentDelete(ISpecification<T> specification, CancellationToken cancellationToken)
        {
            throw new InvalidOperationException(NotSupportedMessage);
        }
    }
}
