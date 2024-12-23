using System.Text;
using Astralis.Core.Server.Data.Scripts;
using Astralis.Server.Utils;
using NLua;

namespace Astralis.Server.ScriptGeneration;

public class LuaTypeDefinitionsGenerator
{
    private readonly StringBuilder _builder = new();

    public async Task<string> GenerateTypeDefinitionsAsync(
        IEnumerable<ScriptFunctionDescriptor> functions,
        Dictionary<string, object> contextVariables
    )
    {
        _builder.AppendLine("---@meta");
        _builder.AppendLine("");
        _builder.AppendLine("-- This file is auto-generated. Do not edit.");
        _builder.AppendLine("");



        // Generate types for complex objects
        await GenerateTypesAsync(contextVariables);

        // Generate function definitions
        foreach (var function in functions)
        {
            await GenerateFunctionDefinitionAsync(function);
        }

        return _builder.ToString();
    }

    private async Task GenerateTypesAsync(Dictionary<string, object> contextVariables)
    {
        foreach (var (name, value) in contextVariables)
        {
            if (value is LuaTable table)
            {
                _builder.AppendLine($"---@class {name}");
                foreach (var field in ScriptUtils.LuaTableToDictionary(table))
                {
                    _builder.AppendLine($"---@field {field.Key} {LuaTypeConverter.GetLuaType(field.Value)}");
                }

                _builder.AppendLine();
            }
        }
    }

    private async Task GenerateFunctionDefinitionAsync(ScriptFunctionDescriptor function)
    {
        // Function documentation
        if (!string.IsNullOrEmpty(function.Help))
        {
            _builder.AppendLine($"---@description {function.Help}");
        }

        // Parameters
        foreach (var param in function.Parameters)
        {
            _builder.AppendLine($"---@param {param.ParameterName} {LuaTypeConverter.GetLuaType(param.ParameterType)}");
        }

        // Return type
        _builder.AppendLine($"---@return {LuaTypeConverter.GetLuaType(function.RawReturnType)}");

        // Function declaration
        _builder.AppendLine(
            $"function {function.FunctionName}({string.Join(", ", function.Parameters.Select(p => p.ParameterName))}) end"
        );
        _builder.AppendLine();
    }
}
