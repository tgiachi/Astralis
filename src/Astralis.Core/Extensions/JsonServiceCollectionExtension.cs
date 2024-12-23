using Astralis.Core.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace Astralis.Core.Extensions;

public static class JsonServiceCollectionExtension
{
    public static IServiceCollection RegisterDefaultJsonOptions(this IServiceCollection services)
    {
        return services.AddSingleton(JsonUtils.GetDefaultJsonSettings());
    }
}
