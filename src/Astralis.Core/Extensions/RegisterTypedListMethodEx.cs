using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Astralis.Core.Extensions;

public static class RegisterTypedListMethodEx
{
    public static IServiceCollection AddToRegisterTypedList<TListEntity>(
        this IServiceCollection services, TListEntity entity
    )
    {
        var typedList = new List<TListEntity>();

        if (services.Any(x => x.ServiceType == typeof(List<TListEntity>)))
        {
            // get list of service definitions
            var serviceDefinitions = services.First(x => x.ServiceType == typeof(List<TListEntity>));
            typedList = (List<TListEntity>)serviceDefinitions.ImplementationInstance;
            typedList.Add(entity);
        }
        else
        {
            // add list of service definitions
            typedList.Add(entity);
            services.AddSingleton(typeof(List<TListEntity>), typedList);
            return services;
        }

        return services;
    }
}
