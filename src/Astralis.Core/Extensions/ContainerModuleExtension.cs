using System;
using Astralis.Core.Interfaces.Modules;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Astralis.Core.Extensions;

public static class ContainerModuleExtension
{
    public static IServiceCollection AddContainerModule(this IServiceCollection services, Type moduleType)
    {
        var module = Activator.CreateInstance(moduleType) as IContainerModule;

        if (module == null)
        {
            throw new Exception($"Type {moduleType.FullName} is not a valid module.");
        }

        Log.Debug("Registering services for module {ModuleType}", moduleType.FullName);
        return module.RegisterServices(Log.ForContext(moduleType), services);
    }


    public static IServiceCollection AddContainerModule<TModule>(this IServiceCollection services)
        where TModule : IContainerModule
    {
        return services.AddContainerModule(typeof(TModule));
    }
}
