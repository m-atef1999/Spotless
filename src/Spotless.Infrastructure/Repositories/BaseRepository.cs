using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Spotless.Application.Interfaces;
using Spotless.Domain.Entities;
using Spotless.Infrastructure.Context;
using System.Linq.Expressions;

namespace Spotless.Infrastructure.Repositories
{
    public class BaseRepository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly ApplicationDbContext _dbContext;

        public BaseRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public async Task<IReadOnlyList<T>> GetByIdsAsync(IEnumerable<Guid> ids)
        {
            return await _dbContext.Set<T>()
                                   .Where(e => ids.Contains(e.Id))
                                   .ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {

            return await _dbContext.Set<T>().ToListAsync();
        }



        public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate)
        {

            return await _dbContext.Set<T>().Where(predicate).ToListAsync();
        }

        public async Task<T> AddAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
            return entity;
        }

        public Task UpdateAsync(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            return Task.CompletedTask;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _dbContext.Set<T>().AnyAsync(e => e.Id == id);
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> filter)
        {
            return await _dbContext.Set<T>()
                                   .Where(filter)
                                   .CountAsync();
        }

        public async Task<IReadOnlyList<T>> GetPagedAsync(
            Expression<Func<T, bool>> filter,
            int skip,
            int take,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null)
        {
            IQueryable<T> query = _dbContext.Set<T>().Where(filter);


            if (include != null)
            {
                query = include(query);
            }


            if (orderBy != null)
            {
                query = orderBy(query);
            }
            else
            {
                query = query.OrderBy(e => e.Id);
            }


            query = query.Skip(skip)
                         .Take(take);


            return await query.ToListAsync();
        }

        public async Task<T?> GetSingleAsync(
            Expression<Func<T, bool>> filter,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null)
        {
            IQueryable<T> query = _dbContext.Set<T>().Where(filter);


            if (include != null)
            {
                query = include(query);
            }

            return await query.FirstOrDefaultAsync();
        }
    }
}