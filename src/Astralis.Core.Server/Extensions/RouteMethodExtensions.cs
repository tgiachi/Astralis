using Astralis.Core.Server.Types;

namespace Astralis.Core.Server.Extensions;

public static class RouteMethodExtensions
{
    public static WatsonWebserver.Core.HttpMethod ToHttpMethod(this RouteMethodType method)
    {
        return method switch
        {
            RouteMethodType.GET     => WatsonWebserver.Core.HttpMethod.GET,
            RouteMethodType.POST    => WatsonWebserver.Core.HttpMethod.POST,
            RouteMethodType.PUT     => WatsonWebserver.Core.HttpMethod.PUT,
            RouteMethodType.DELETE  => WatsonWebserver.Core.HttpMethod.DELETE,
            RouteMethodType.HEAD    => WatsonWebserver.Core.HttpMethod.HEAD,
            RouteMethodType.OPTIONS => WatsonWebserver.Core.HttpMethod.OPTIONS,
            RouteMethodType.PATCH   => WatsonWebserver.Core.HttpMethod.PATCH,
            _                       => throw new ArgumentException($"Unsupported route method: {method}")
        };
    }

    public static RouteMethodType ToRouteMethod(this WatsonWebserver.Core.HttpMethod method)
    {
        if (method == WatsonWebserver.Core.HttpMethod.GET)
        {
            return RouteMethodType.GET;
        }

        if (method == WatsonWebserver.Core.HttpMethod.POST)
        {
            return RouteMethodType.POST;
        }

        if (method == WatsonWebserver.Core.HttpMethod.PUT)
        {
            return RouteMethodType.PUT;
        }

        if (method == WatsonWebserver.Core.HttpMethod.DELETE)
        {
            return RouteMethodType.DELETE;
        }

        if (method == WatsonWebserver.Core.HttpMethod.HEAD)
        {
            return RouteMethodType.HEAD;
        }

        if (method == WatsonWebserver.Core.HttpMethod.OPTIONS)
        {
            return RouteMethodType.OPTIONS;
        }

        if (method == WatsonWebserver.Core.HttpMethod.PATCH)
        {
            return RouteMethodType.PATCH;
        }

        throw new ArgumentException($"Unsupported HTTP method: {method}");
    }
}
