using System.Reflection;
using System.Text.Json;
using Astralis.Core.Server.Attributes.Scripts;
using Astralis.Core.Server.Data.Directories;
using Astralis.Core.Server.Interfaces.Services.System;
using Astralis.Core.Server.Types;
using Humanizer;
using Serilog;

namespace Astralis.Server.Services;

public class ConfigService : IConfigService
{
    private readonly ILogger _logger = Log.ForContext<ConfigService>();

    private readonly IScriptEngineSystemService _scriptEngineSystemService;

    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly DirectoriesConfig _directoriesConfig;

    public ConfigService(
        IScriptEngineSystemService scriptEngineSystemService, JsonSerializerOptions jsonSerializerOptions,
        DirectoriesConfig directoriesConfig
    )
    {
        _scriptEngineSystemService = scriptEngineSystemService;
        _jsonSerializerOptions = jsonSerializerOptions;
        _directoriesConfig = directoriesConfig;
    }

    public Task StartAsync()
    {
        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        return Task.CompletedTask;
    }

    public void SearchForConfigAttributes(object instance)
    {
        var properties = instance.GetType().GetProperties();

        foreach (var property in properties)
        {
            var attribute = property.GetCustomAttribute<ScriptConfigVariableAttribute>();

            if (attribute == null)
            {
                continue;
            }

            _logger.Debug("Checking property {PropertyName} for script variable", property.Name);
            var value = _scriptEngineSystemService.GetContextVariable(attribute.Name, property.PropertyType, false);

            if (value == null)
            {
                var configFilePath = GetConfigPath(property.PropertyType);

                if (File.Exists(configFilePath))
                {
                    _logger.Debug("Loading config from {ConfigPath}", configFilePath);
                    var configJson = File.ReadAllText(configFilePath);
                    value = JsonSerializer.Deserialize(configJson, property.PropertyType, _jsonSerializerOptions);
                }

                if (value == null)
                {
                    _logger.Debug("Creating new instance of {Type} and saving", property.PropertyType);
                    value = Activator.CreateInstance(property.PropertyType);

                    var configJson = JsonSerializer.Serialize(value, _jsonSerializerOptions);
                    File.WriteAllText(configFilePath, configJson);
                }
            }

            if (value != null)
            {
                property.SetValue(instance, value);
            }
        }
    }

    private string GetConfigPath(Type name)
    {
        return Path.Combine(_directoriesConfig[DirectoryType.Configs], $"{name.Name.Underscore()}.json");
    }
}
