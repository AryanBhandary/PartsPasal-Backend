using PartsPasal.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using PartsPasal.Infrastructure.Data;

namespace PartsPasal.Infrastructure.Repositories;

public class RepositoryBase<T> : IRepositoryBase<T> where T : class
{
    // Add generic repository implementation here
}
