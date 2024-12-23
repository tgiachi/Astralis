using System.Diagnostics;
using System.Linq.Expressions;
using Astralis.Core.Server.Data.Directories;
using Astralis.Core.Server.Data.Internal;
using Astralis.Core.Server.Interfaces.Entities;
using Astralis.Core.Server.Interfaces.Services.System;
using Astralis.Core.Server.Types;
using Astralis.Server.Data.Configs;
using Serilog;

namespace Astralis.Server.Services;

public class SqlDatabaseService : IDatabaseService
{
    private readonly ILogger _logger = Log.Logger.ForContext<SqlDatabaseService>();

    private readonly IFreeSql _connectionFactory = null!;


    private readonly List<DbEntityTypeData> _entityTypeData;

    public SqlDatabaseService(
        AstralisRootOptions astralisServerOptions, DirectoriesConfig directoriesConfig, List<DbEntityTypeData> entityTypeData
    )
    {
        _entityTypeData = entityTypeData;

        // TODO: Check for mysql and postgresql
        _connectionFactory = new FreeSql.FreeSqlBuilder()
            .UseConnectionString(
                GetDataType(astralisServerOptions),
                Path.Combine(directoriesConfig[DirectoryType.Database], astralisServerOptions.DatabaseConnectionString)
            )
            .UseAutoSyncStructure(true)
            .Build();

        _logger.Information("Sql database service initialized");
    }


    private async Task MigrateAsync()
    {
        _logger.Information("Migrating database structure");
        var startTime = Stopwatch.GetTimestamp();

        _connectionFactory.CodeFirst.SyncStructure(_entityTypeData.Select(entityType => entityType.EntityType).ToArray());

        var endTime = Stopwatch.GetTimestamp();

        _logger.Information("Database structure migrated in {Time} ms", Stopwatch.GetElapsedTime(startTime, endTime));
    }

    private static FreeSql.DataType GetDataType(AstralisRootOptions astralisServerOptions)
    {
        return astralisServerOptions.DatabaseType switch
        {
            AstralisDatabaseType.Sqlite => FreeSql.DataType.Sqlite,
            AstralisDatabaseType.MySql => FreeSql.DataType.MySql,
            AstralisDatabaseType.PostgreSql => FreeSql.DataType.PostgreSQL,
            _ => throw new ArgumentOutOfRangeException(nameof(astralisServerOptions.DatabaseType))
        };
    }


    public async Task<TEntity> InsertAsync<TEntity>(TEntity entity) where TEntity : class, IBaseDbEntity
    {
        using var dbConnection = _connectionFactory.GetRepository<TEntity>();
        return await dbConnection.InsertAsync(entity);
    }

    public async Task<List<TEntity>> InsertAsync<TEntity>(List<TEntity> entities) where TEntity : class, IBaseDbEntity
    {
        using var dbConnection = _connectionFactory.GetRepository<TEntity>();
        return await dbConnection.InsertAsync(entities);
    }

    public async Task<int> CountAsync<TEntity>() where TEntity : class, IBaseDbEntity
    {
        using var dbConnection = _connectionFactory.GetRepository<TEntity>();
        return (int)(await dbConnection.Where(entity => true).CountAsync());
    }

    public async Task<TEntity> FindByIdAsync<TEntity>(Guid id) where TEntity : class, IBaseDbEntity
    {
        using var dbConnection = _connectionFactory.GetRepository<TEntity>();
        return await dbConnection.Where(entity => entity.Id == id).FirstAsync();
    }

    public async Task<IEnumerable<TEntity>> FindAllAsync<TEntity>() where TEntity : class, IBaseDbEntity
    {
        using var dbConnection = _connectionFactory.GetRepository<TEntity>();
        return await dbConnection.Select.ToListAsync();
    }

    public async Task<IEnumerable<TEntity>> QueryAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
        where TEntity : class, IBaseDbEntity
    {
        using var dbConnection = _connectionFactory.GetRepository<TEntity>();
        return await dbConnection.Where(predicate).ToListAsync();
    }

    public Task<TEntity> FirstOrDefaultAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
        where TEntity : class, IBaseDbEntity
    {
        using var dbConnection = _connectionFactory.GetRepository<TEntity>();
        return dbConnection.Where(predicate).FirstAsync();
    }

    public async Task UpdateAsync<TEntity>(TEntity entity) where TEntity : class, IBaseDbEntity
    {
        using var dbConnection = _connectionFactory.GetRepository<TEntity>();
        await dbConnection.UpdateAsync(entity);
    }

    public async Task DeleteAsync<TEntity>(TEntity entity) where TEntity : class, IBaseDbEntity
    {
        using var dbConnection = _connectionFactory.GetRepository<TEntity>();
        await dbConnection.DeleteAsync(entity);
    }

    public async Task DeleteAsync<TEntity>(Guid id) where TEntity : class, IBaseDbEntity
    {
        using var dbConnection = _connectionFactory.GetRepository<TEntity>();
        await dbConnection.DeleteAsync(entity => entity.Id == id);
    }

    public async Task DeleteAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class, IBaseDbEntity
    {
        using var dbConnection = _connectionFactory.GetRepository<TEntity>();
        await dbConnection.DeleteAsync(predicate);
    }

    public async Task DeleteAllAsync<TEntity>() where TEntity : class, IBaseDbEntity
    {
        using var dbConnection = _connectionFactory.GetRepository<TEntity>();
        await dbConnection.DeleteAsync(entity => true);
    }

    public async Task<bool> ExistsAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
        where TEntity : class, IBaseDbEntity
    {
        using var dbConnection = _connectionFactory.GetRepository<TEntity>();
        return await dbConnection.Where(predicate).AnyAsync();
    }


    public async Task StartAsync()
    {
        MigrateAsync();
    }

    public Task StopAsync()
    {
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _connectionFactory.Dispose();
    }
}
