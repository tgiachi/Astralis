using System.Text.Json;
using Astralis.Core.Server.Data.Directories;
using Astralis.Core.Server.Data.Scripts;
using Astralis.Server.Services;
using Moq;

namespace Astralis.Tests;

public class ScriptEngineSystemServiceTests
{
    private readonly ScriptEngineSystemService _sut;
    private readonly Mock<DirectoriesConfig> _dirConfigMock;
    private readonly Mock<IServiceProvider> _containerMock;
    private readonly List<ScriptClassData> _scriptModules;
    private readonly JsonSerializerOptions _jsonOptions;

    public ScriptEngineSystemServiceTests()
    {
        _dirConfigMock = new Mock<DirectoriesConfig>("test_path");
        _scriptModules = new List<ScriptClassData>();
        _containerMock = new Mock<IServiceProvider>();
        _jsonOptions = new JsonSerializerOptions();

        _sut = new ScriptEngineSystemService(
            _dirConfigMock.Object,
            _scriptModules,
            _containerMock.Object,
            _jsonOptions
        );
    }

    [Fact]
    public void AddContextVariable_WhenVariableAdded_CanBeRetrieved()
    {
        // Arrange
        var testValue = "test_value";

        // Act
        _sut.AddContextVariable("test_var", testValue);
        var result = _sut.GetContextVariable<string>("test_var");

        // Assert
        Assert.Equal(testValue, result);
    }

    [Fact]
    public async Task ExecuteCommand_WhenValidLuaCode_ReturnsResult()
    {
        // Arrange
        var luaCode = "return 'hello'";

        // Act
        var result = _sut.ExecuteCommand(luaCode);

        // Assert
        Assert.Null(result.Exception);
        Assert.NotNull(result.Result);
    }

    [Fact]
    public void GetContextVariable_WhenVariableNotFound_ThrowsException()
    {
        // Act & Assert
        Assert.Throws<KeyNotFoundException>(() => _sut.GetContextVariable<string>("nonexistent"));
    }

    [Fact]
    public async Task ExecuteFileAsync_WhenFileNotFound_ThrowsException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<FileNotFoundException>(() =>
            _sut.ExecuteFileAsync("nonexistent.lua"));
    }
}
