using Astralis.Core.Extensions;
using Astralis.Core.Interfaces.Modules;
using Astralis.Core.Server.Attributes.Scripts;
using Astralis.Core.Server.Data.Scripts;
using Astralis.Core.Server.Extensions;
using Astralis.Core.Server.Interfaces.Services.System;
using Astralis.Core.Server.Utils;
using Astralis.Server.Services;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Astralis.Server.Modules;

public class ScriptModule : IContainerModule
{
    public IServiceCollection RegisterServices(ILogger logger, IServiceCollection services)
    {
        AssemblyUtils.GetAttribute<ScriptModuleAttribute>()
            .ForEach(
                s =>
                {
                    logger.Debug("Registering script module {module}", s.Name);

                    services
                        .AddSingleton(s)
                        .AddToRegisterTypedList(new ScriptClassData(s));
                }
            );


        return services
            .AddSystemService<IScriptEngineSystemService, ScriptEngineSystemService>(-1);
    }
}
