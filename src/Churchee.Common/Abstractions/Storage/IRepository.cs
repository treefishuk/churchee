using Ardalis.Specification;
using Churchee.Common.Abstractions.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Churchee.Common.Abstractions.Storage
{
    public interface IRepository;

    public interface IRepository<T> : IRepository where T : class, IEntity
    {
        IQueryable<T> GetQueryable();

        IQueryable<T> ApplySpecification(ISpecification<T> specification, bool evaluateCriteriaOnly = false);

        Task<T> FirstOrDefaultAsync(ISpecification<T> specification, CancellationToken cancellationToken);

        Task<TResult> FirstOrDefaultAsync<TResult>(ISpecification<T> specification, Expression<Func<T, TResult>> selector, CancellationToken cancellationToken);

        T GetById(params object[] keyValues);

        Task<T> GetByIdAsync(object id);

        Task<T> GetByIdAsync(object id, CancellationToken cancellationToken);

        void Create(T entity);

        void PermanentDelete(T entity);

        Task PermanentDelete(Guid id);

        Task SoftDelete(Guid id);

        void Update(T entity);

        void AddRange(IEnumerable<T> entities);

        bool Any();

        bool AnyWithFiltersDisabled(Expression<Func<T, bool>> predicate);

        int Count();

        Task<int> CountAsync(CancellationToken cancellationToken);

        Task<List<T>> GetListAsync(ISpecification<T> specification, CancellationToken cancellationToken);

        Task<List<TResult>> GetListAsync<TResult>(ISpecification<T> specification, Expression<Func<T, TResult>> selector, CancellationToken cancellationToken);

        Task<DataTableResponse<TResult>> GetDataTableResponseAsync<TResult>(ISpecification<T> specification, string orderBy, string orderByDir, int skip, int take, Expression<Func<T, TResult>> selector, CancellationToken cancellationToken);
    }
}
