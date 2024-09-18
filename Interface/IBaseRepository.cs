using System.Linq.Expressions;

namespace BookProject.Interface
{
    public interface IBaseRepository<T> where T : class
    {
        T Add(T entity);
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default(CancellationToken));
        void AddRange(IEnumerable<T> entities);
        void Edit(T entity);
        void Delete(T entity);
        void Delete(Expression<Func<T, bool>> where);
        Task<T> GetByIdAsync(Guid id);
        Task<T> GetSingleAsync(Expression<Func<T, bool>> where, CancellationToken cancellationToken = default);
        IQueryable<T> GetAll();
        IQueryable<T> GetAllIncluding(params Expression<Func<T, object>>[] where);
        IQueryable<T> GetMany(Expression<Func<T, bool>> where);
    }
}
