using Astralis.Core.Extensions;
using Astralis.Core.Server.Data.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Astralis.Core.Server.Extensions;

public static class ServicesExtension
{
    public static IServiceCollection AddSystemService(
        this IServiceCollection services, Type serviceType, Type implementationType, int priority = 0
    )
    {
        services.AddSingleton(serviceType, implementationType);

        services.AddToRegisterTypedList(new SystemServiceData(serviceType, implementationType, priority));

        return services;
    }

    public static IServiceCollection AddSystemService<TInterface, TService>(
        this IServiceCollection services, int priority = 0
    )
        where TInterface : class
        where TService : class, TInterface
    {
        services.AddSingleton<TInterface, TService>();

        services.AddToRegisterTypedList(new SystemServiceData(typeof(TInterface), typeof(TService), priority));

        return services;
    }

    public static IServiceCollection AddGameService(
        this IServiceCollection services, Type serviceType, Type implementationType, int priority = 0
    )
    {
        services.AddSingleton(serviceType, implementationType);

        services.AddToRegisterTypedList(new GameServiceData(serviceType, implementationType, priority));

        return services;
    }

    public static IServiceCollection AddGameService<TInterface, TService>(
        this IServiceCollection services, int priority = 0
    )
        where TInterface : class
        where TService : class, TInterface
    {
        services.AddSingleton<TInterface, TService>();

        services.AddToRegisterTypedList(new GameServiceData(typeof(TInterface), typeof(TService), priority));

        return services;
    }

    public static IServiceCollection AddHandlerService<TService>(this IServiceCollection services, int priority = 10)
        where TService : class
    {
        return services.AddSingleton<TService>()
            .AddToRegisterTypedList(new GameServiceData(typeof(TService), typeof(TService), priority));
    }
}
