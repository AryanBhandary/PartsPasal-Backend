using System.Linq.Expressions;

namespace PartsPasal.Application.Interfaces;

public interface IRepositoryBase<T> where T : class
{
    Task<List<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate);

    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);

    Task SaveChangesAsync();
}