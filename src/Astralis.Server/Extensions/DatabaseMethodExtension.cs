using System.ComponentModel.DataAnnotations.Schema;
using Astralis.Core.Extensions;
using Astralis.Core.Server.Data.Internal;
using Astralis.Core.Server.Extensions;
using Astralis.Core.Server.Interfaces.Entities;
using Astralis.Core.Server.Interfaces.Services.System;
using Astralis.Core.Server.Types;
using Astralis.Core.Server.Utils;
using Astralis.Server.Services;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Astralis.Server.Extensions;

public static class DatabaseMethodExtension
{
    public static IServiceCollection RegisterDatabaseAndScanEntities(
        this IServiceCollection services, AstralisDatabaseType databaseType
    )
    {
        Log.Logger.Information("Registering database service for {databaseType}", databaseType);
        switch (databaseType)
        {
            case AstralisDatabaseType.Sqlite:
            case AstralisDatabaseType.MySql:
            case AstralisDatabaseType.PostgreSql:
                services.AddSystemService<IDatabaseService, SqlDatabaseService>();
                break;
            case AstralisDatabaseType.LiteDb:
                services.AddSystemService<IDatabaseService, LiteDbDatabaseService>();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(databaseType), databaseType, null);
        }

        AssemblyUtils.GetAttribute<TableAttribute>()
            .ForEach(
                s => { services.AddDbEntity(s); }
            );

        return services;
    }

    public static IServiceCollection AddDbEntity(this IServiceCollection services, Type type)
    {
        Log.Logger.Information("Registering database entity {entityType}", type.Name);
        return services.AddToRegisterTypedList(new DbEntityTypeData(type));
    }

    public static IServiceCollection AddDbEntity<T>(this IServiceCollection services) where T : class, IBaseDbEntity
    {
        return services.AddToRegisterTypedList(new DbEntityTypeData(typeof(T)));
    }
}
