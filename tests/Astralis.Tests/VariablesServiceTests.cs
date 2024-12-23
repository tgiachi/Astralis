using Astralis.Core.Interfaces.Services;
using Astralis.Server.Services;
using Moq;

namespace Astralis.Tests;

public class VariablesServiceTests
{
    private readonly VariablesService _sut;
    private readonly Mock<IEventBusService> _eventBusMock;

    public VariablesServiceTests()
    {
        _eventBusMock = new Mock<IEventBusService>();
        _sut = new VariablesService(_eventBusMock.Object);
    }

    [Fact]
    public void TranslateText_WhenVariableExists_ReplacesVariable()
    {
        // Arrange
        _sut.AddVariable("test_var", "replaced");
        var input = "This is a {test_var} string";

        // Act
        var result = _sut.TranslateText(input);

        // Assert
        Assert.Equal("This is a replaced string", result);
    }

    [Fact]
    public void TranslateText_WhenVariableDoesNotExist_LeavesTextUnchanged()
    {
        // Arrange
        var input = "This is a {nonexistent_var} string";

        // Act
        var result = _sut.TranslateText(input);

        // Assert
        Assert.Equal(input, result);
    }

    [Fact]
    public void AddVariableBuilder_WhenCalled_BuilderIsExecutedOnTranslate()
    {
        // Arrange
        var counter = 0;
        _sut.AddVariableBuilder("dynamic_var", () =>
        {
            counter++;
            return counter.ToString();
        });

        // Act
        var result1 = _sut.TranslateText("Count: {dynamic_var}");
        var result2 = _sut.TranslateText("Count: {dynamic_var}");

        // Assert
        Assert.Equal("Count: 1", result1);
        Assert.Equal("Count: 2", result2);
    }

    [Fact]
    public void RebuildVariables_WhenCalled_UpdatesAllBuilderVariables()
    {
        // Arrange
        var counter = 0;
        _sut.AddVariableBuilder("counter", () => counter++);
        _sut.RebuildVariables();
        var initialValue = _sut.TranslateText("{counter}");

        // Act
        _sut.RebuildVariables();
        var newValue = _sut.TranslateText("{counter}");

        // Assert
        Assert.NotEqual(initialValue, newValue);
    }
}
