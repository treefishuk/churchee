using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Churchee.Common.Abstractions;
using Churchee.Common.Abstractions.Entities;
using Churchee.Common.Abstractions.Storage;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Churchee.Data.EntityFramework.Admin
{
    public class GenericRepository<T> : IRepository<T> where T : class, IEntity
    {
        private readonly DbSet<T> _dbSet;
        private readonly DbContext _dbContext;
        private readonly SpecificationEvaluator _specificationEvaluator;

        public GenericRepository(DbContext dbContext)
        {
            _dbSet = dbContext.Set<T>();
            _dbContext = dbContext;
            _specificationEvaluator = SpecificationEvaluator.Default;
        }

        public virtual void Create(T entity)
        {
            _dbSet.Add(entity);
        }

        public virtual void Update(T entity)
        {
            _dbContext.Set<T>().Attach(entity);
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
            _dbSet.AddRange(entities);
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

        public void PermanentDelete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task PermanentDelete(Guid id)
        {
            var entity = await _dbSet.FindAsync(id);

            _dbSet.Remove(entity);
        }

        public async Task SoftDelete(Guid id)
        {
            var entity = await _dbSet.FindAsync(id);

            entity.Deleted = true;
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


    }
}
