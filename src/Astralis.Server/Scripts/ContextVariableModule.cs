using Astralis.Core.Server.Attributes.Scripts;
using Astralis.Core.Server.Interfaces.Services.System;

namespace Astralis.Server.Scripts;

[ScriptModule]
public class ContextVariableModule
{
    private readonly IScriptEngineSystemService _scriptEngineSystemService;

    public ContextVariableModule(IScriptEngineSystemService scriptEngineSystemService)
    {
        _scriptEngineSystemService = scriptEngineSystemService;
    }

    [ScriptFunction("add_var")]
    public void AddContextVariable(string variableName, object value)
    {
        _scriptEngineSystemService.AddContextVariable(variableName, value);
    }
}
