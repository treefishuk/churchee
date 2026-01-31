using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Churchee.Common.Abstractions;
using Churchee.Common.Abstractions.Entities;
using Churchee.Common.Abstractions.Storage;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace Churchee.Data.EntityFramework.Site
{
    public class ReadonlyGenericRepository<T> : IRepository<T> where T : class, IEntity
    {
        private readonly DbSet<T> _dbSet;
        private readonly SpecificationEvaluator _specificationEvaluator;
        private const string NotSupportedMessage = "Not supported in the readonly version of this repository";

        public ReadonlyGenericRepository(DbContext dbContext)
        {
            _dbSet = dbContext.Set<T>();
            _specificationEvaluator = SpecificationEvaluator.Default;
        }

        public virtual void Create(T entity)
        {
            throw new InvalidOperationException(NotSupportedMessage);
        }

        public virtual void Update(T entity)
        {
            throw new InvalidOperationException(NotSupportedMessage);
        }

        public virtual T GetById(params object[] keyValues)
        {
            return _dbSet.Find(keyValues);
        }

        public virtual IQueryable<T> GetQueryable()
        {
            return _dbSet.AsQueryable();
        }

        public virtual void AddRange(IEnumerable<T> entities)
        {
            throw new InvalidOperationException(NotSupportedMessage);
        }

        public IQueryable<T> ApplySpecification(ISpecification<T> specification, bool evaluateCriteriaOnly = false)
        {
            return _specificationEvaluator.GetQuery(GetQueryable(), specification, evaluateCriteriaOnly);
        }

        public bool Any()
        {
            return _dbSet.Any();
        }

        public async Task<T> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<T> GetByIdAsync(object id, CancellationToken cancellationToken)
        {
            return await _dbSet.FindAsync([id], cancellationToken);
        }

        public int Count()
        {
            return _dbSet.Count();
        }

        public async Task<int> CountAsync(CancellationToken cancellationToken)
        {
            return await _dbSet.CountAsync(cancellationToken);
        }

        public async Task<int> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken)
        {
            return await _specificationEvaluator.GetQuery(GetQueryable(), specification).CountAsync(cancellationToken);
        }

        public async Task<int> GetDistinctCountAsync<TResult>(ISpecification<T> specification, Expression<Func<T, TResult>> selector, CancellationToken cancellationToken)
        {
            return await _specificationEvaluator.GetQuery(_dbSet.AsNoTracking(), specification).Select(selector).Distinct().CountAsync(cancellationToken);
        }

        public async Task SoftDelete<TId>(TId id)
        {
            throw new InvalidOperationException(NotSupportedMessage);
        }

        public bool AnyWithFiltersDisabled(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.IgnoreQueryFilters().Any(predicate);
        }

        public async Task<T> FirstOrDefaultAsync(ISpecification<T> specification, CancellationToken cancellationToken)
        {
            return await _specificationEvaluator.GetQuery(GetQueryable(), specification).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<TResult> FirstOrDefaultAsync<TResult>(ISpecification<T> specification, Expression<Func<T, TResult>> selector, CancellationToken cancellationToken)
        {
            return await _specificationEvaluator.GetQuery(GetQueryable(), specification)
                .Select(selector)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<T>> GetListAsync(ISpecification<T> specification, CancellationToken cancellationToken)
        {
            return await _specificationEvaluator.GetQuery(GetQueryable(), specification).ToListAsync(cancellationToken);
        }

        public async Task<List<TResult>> GetListAsync<TResult>(ISpecification<T> specification, Expression<Func<T, TResult>> selector, CancellationToken cancellationToken)
        {
            return await _specificationEvaluator.GetQuery(GetQueryable(), specification).Select(selector).ToListAsync(cancellationToken);
        }

        public async Task<List<TResult>> GetListAsync<TKey, TResult>(ISpecification<T> specification, Expression<Func<T, TKey>> groupBy, Expression<Func<IGrouping<TKey, T>, TResult>> selector, CancellationToken cancellationToken)
        {
            var query = _specificationEvaluator.GetQuery(GetQueryable(), specification);

            return await query
                .GroupBy(groupBy)
                .Select(selector)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<TResult>> GetListAsync<TKey, TResult>(ISpecification<T> specification, Expression<Func<T, TKey>> groupBy, Expression<Func<IGrouping<TKey, T>, TResult>> selector, int take, CancellationToken cancellationToken)
        {
            var query = _specificationEvaluator.GetQuery(GetQueryable(), specification);

            return await query
                .GroupBy(groupBy)
                .Select(selector)
                .Take(take)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<TResult>> GetDistinctListAsync<TResult>(ISpecification<T> specification, Expression<Func<T, TResult>> selector, CancellationToken cancellationToken)
        {
            return await _specificationEvaluator.GetQuery(GetQueryable(), specification).Select(selector).Distinct().ToListAsync(cancellationToken);
        }

        public async Task<DataTableResponse<TResult>> GetDataTableResponseAsync<TResult>(ISpecification<T> specification, string orderBy, string orderByDir, int skip, int take, Expression<Func<T, TResult>> selector, CancellationToken cancellationToken)
        {
            int count = await CountAsync(cancellationToken);

            var data = await _specificationEvaluator.GetQuery(GetQueryable(), specification)
                .Select(selector)
                .OrderBy(orderBy, orderByDir)
                .Skip(skip)
                .Take(take)
                .ToListAsync(cancellationToken);

            return new DataTableResponse<TResult>
            {
                RecordsTotal = count,
                RecordsFiltered = count,
                Draw = take,
                Data = data
            };
        }

        public void PermanentDelete(T entity)
        {
            throw new InvalidOperationException(NotSupportedMessage);
        }

        public async Task PermanentDelete(Guid id)
        {
            throw new InvalidOperationException(NotSupportedMessage);
        }

        public async Task PermanentDelete(ISpecification<T> specification, CancellationToken cancellationToken)
        {
            throw new InvalidOperationException(NotSupportedMessage);
        }
    }
}
