using Astralis.Core.Server.Attributes.Scripts;
using Astralis.Core.Server.Data.Directories;
using Astralis.Core.Server.Interfaces.Services.System;
using NLua;


namespace Astralis.Server.Scripts;

[ScriptModule]
public class ScriptModule
{
    private readonly IScriptEngineSystemService _scriptEngineSystemService;

    private readonly DirectoriesConfig _directoriesConfig;

    public ScriptModule(
        IScriptEngineSystemService scriptEngineSystemService, DirectoriesConfig directoryConfig
    )
    {
        _scriptEngineSystemService = scriptEngineSystemService;
        _directoriesConfig = directoryConfig;
    }


    [ScriptFunction("on_bootstrap", "Called when the server is bootstrapping")]
    public void RegisterBootstrap(LuaFunction function)
    {
        _scriptEngineSystemService.AddContextVariable("bootstrap", function);
    }

    [ScriptFunction("gen_lua_def", "Generate lua definitions")]
    public string GenerateLuaDefinitions()
    {
        return _scriptEngineSystemService.GenerateDefinitionsAsync().Result;
    }
}
