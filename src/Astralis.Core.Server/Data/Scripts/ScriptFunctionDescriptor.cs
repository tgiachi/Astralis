namespace Astralis.Core.Server.Data.Scripts;

public class ScriptFunctionDescriptor
{
    public string FunctionName { get; set; } = null!;
    public string? Help { get; set; }

    public List<ScriptFunctionParameterDescriptor> Parameters { get; set; } = new();
    public string ReturnType { get; set; }

    public Type RawReturnType { get; set; } = null!;
}
