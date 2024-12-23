using System.Linq.Expressions;
using Astralis.Core.Server.Interfaces.Entities;

namespace Astralis.Core.Server.Interfaces.Services.System;

public interface IDatabaseService : IDisposable
{
    Task<TEntity> InsertAsync<TEntity>(TEntity entity) where TEntity : class, IBaseDbEntity;

    Task<List<TEntity>> InsertAsync<TEntity>(List<TEntity> entities) where TEntity : class, IBaseDbEntity;

    Task<int> CountAsync<TEntity>() where TEntity : class, IBaseDbEntity;

    Task<TEntity> FindByIdAsync<TEntity>(Guid id) where TEntity : class, IBaseDbEntity;

    Task<IEnumerable<TEntity>> FindAllAsync<TEntity>() where TEntity : class, IBaseDbEntity;

    Task<IEnumerable<TEntity>> QueryAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
        where TEntity : class, IBaseDbEntity;

    Task<TEntity?> FirstOrDefaultAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
        where TEntity : class, IBaseDbEntity;

    Task UpdateAsync<TEntity>(TEntity entity) where TEntity : class, IBaseDbEntity;

    Task DeleteAsync<TEntity>(TEntity entity) where TEntity : class, IBaseDbEntity;

    Task DeleteAsync<TEntity>(Guid id) where TEntity : class, IBaseDbEntity;

    Task DeleteAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity :class,  IBaseDbEntity;

    Task DeleteAllAsync<TEntity>() where TEntity : class, IBaseDbEntity;

    Task<bool> ExistsAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class, IBaseDbEntity;
}
