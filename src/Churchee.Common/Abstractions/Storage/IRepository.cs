using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Ardalis.Specification;
using Churchee.Common.Abstractions.Entities;

namespace Churchee.Common.Abstractions.Storage
{
    public interface IRepository
    {
    }

    public interface IRepository<T> : IRepository where T : class, IEntity
    {
        IQueryable<T> GetQueryable();

        IQueryable<T> ApplySpecification(ISpecification<T> specification, bool evaluateCriteriaOnly = false);

        T GetById(params object[] keyValues);

        Task<T> GetByIdAsync(object id);

        void Create(T entity);

        void PermenantDelete(T entity);

        Task PermenantDelete(Guid id);

        Task SoftDelete(Guid id);

        void Update(T entity);

        void AddRange(IEnumerable<T> entities);

        bool Any();

        bool AnyWithFiltersDisabled(Expression<Func<T, bool>> predicate);

        int Count();
    }
}
