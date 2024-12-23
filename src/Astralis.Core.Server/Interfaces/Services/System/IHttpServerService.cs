using System.Text.RegularExpressions;
using Astralis.Core.Interfaces.Services.Base;
using Astralis.Core.Server.Types;
using WatsonWebserver.Core;

namespace Astralis.Core.Server.Interfaces.Services.System;

public interface IHttpServerService : IAstralisSystemService
{
    // Content routes
    void AddContentRoute(string path, bool listFiles);

    // Static routes
    void AddStaticRoute(RouteMethodType method, string path, Func<HttpContextBase, Task> handler);

    // Parameter routes
    void AddParameterRoute(RouteMethodType method, string path, Func<HttpContextBase, Task> handler);

    // Dynamic routes
    void AddDynamicRoute(RouteMethodType method, Regex pattern, Func<HttpContextBase, Task> handler);
}
