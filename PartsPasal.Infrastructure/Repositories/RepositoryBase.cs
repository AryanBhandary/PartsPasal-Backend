using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PartsPasal.Application.Interfaces;
using PartsPasal.Infrastructure.Data;

namespace PartsPasal.Infrastructure.Repositories;

public class RepositoryBase<T>(AppDbContext context) : IRepositoryBase<T> where T : class
{
    protected AppDbContext Context { get; } = context;

    public async Task<List<T>> GetAllAsync() =>
        await Context.Set<T>().ToListAsync();

    public async Task<T?> GetByIdAsync(int id) =>
        await Context.Set<T>().FindAsync(id);

    public async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate) =>
        await Context.Set<T>().Where(predicate).ToListAsync();

    public async Task AddAsync(T entity) =>
        await Context.Set<T>().AddAsync(entity);

    public void Update(T entity) =>
        Context.Set<T>().Update(entity);

    public void Delete(T entity) =>
        Context.Set<T>().Remove(entity);

    public async Task SaveChangesAsync() =>
        await Context.SaveChangesAsync();
}