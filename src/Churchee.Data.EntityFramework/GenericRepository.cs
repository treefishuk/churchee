using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Churchee.Common.Abstractions.Entities;
using Churchee.Common.Abstractions.Storage;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Churchee.Data.EntityFramework
{
    public class GenericRepository<T> : IRepository<T> where T : class, IEntity
    {
        private readonly DbSet<T> _dbSet;
        private readonly DbContext _dbContext;
        private readonly ISpecificationEvaluator _specificationEvaluator;

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

        public virtual T GetById(params object[] id)
        {
            return _dbSet.Find(id);
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

        public int Count()
        {
            return _dbSet.Count();
        }

        public void PermenantDelete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task SoftDelete(Guid id)
        {
            var entity = await _dbSet.FindAsync(id);

            entity.Deleted = true;
        }

        public async Task PermenantDelete(Guid id)
        {
            var entity = await _dbSet.FindAsync(id);

            _dbSet.Remove(entity);
        }

        public bool AnyWithFiltersDisabled(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.IgnoreQueryFilters().Any(predicate);
        }
    }
}
