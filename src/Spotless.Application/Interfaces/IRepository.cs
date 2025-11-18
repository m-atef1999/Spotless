using Microsoft.EntityFrameworkCore.Query;
using Spotless.Domain.Entities;
using System.Linq.Expressions;

namespace Spotless.Application.Interfaces
{

    public interface IRepository<T> where T : BaseEntity
    {
        Task<T?> GetByIdAsync(Guid id);
        Task<IReadOnlyList<T>> GetByIdsAsync(IEnumerable<Guid> ids);


        Task<IReadOnlyList<T>> GetAllAsync();


        Task<IReadOnlyList<T>> GetAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate);

        Task<T?> GetSingleAsync(
            Expression<Func<T, bool>> filter,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null
        );

        Task<int> CountAsync(Expression<Func<T, bool>> filter);
        Task<IReadOnlyList<T>> GetPagedAsync(
            Expression<Func<T, bool>> filter,
            int skip,
            int take,
            Func<IQueryable<T>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<T, object>>? include = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null
        );

        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<bool> ExistsAsync(Guid id);
    }
}