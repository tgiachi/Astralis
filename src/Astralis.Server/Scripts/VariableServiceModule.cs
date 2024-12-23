using Astralis.Core.Server.Attributes.Scripts;
using Astralis.Core.Server.Interfaces.Services.System;

namespace Astralis.Server.Scripts;

[ScriptModule]
public class VariableServiceModule
{
    private readonly IVariablesService _variablesService;

    public VariableServiceModule(IVariablesService variablesService)
    {
        _variablesService = variablesService;
    }


    [ScriptFunction("r_text")]
    public string ReplaceText(string text)
    {
        return _variablesService.TranslateText(text);
    }
}
