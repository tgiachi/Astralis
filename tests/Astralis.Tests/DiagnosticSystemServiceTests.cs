using Astralis.Core.Interfaces.EventBus;
using Astralis.Core.Interfaces.Services;
using Astralis.Core.Server.Data.Directories;
using Astralis.Core.Server.Events.Engine;
using Astralis.Server.Services;
using Moq;

namespace Astralis.Tests;

public class DiagnosticSystemServiceTests
{
    private readonly DiagnosticSystemService _sut;
    private readonly Mock<IEventBusService> _eventBusMock;
    private readonly Mock<DirectoriesConfig> _dirConfigMock;

    public DiagnosticSystemServiceTests()
    {
        _eventBusMock = new Mock<IEventBusService>();
        _dirConfigMock = new Mock<DirectoriesConfig>("test_path");
        _sut = new DiagnosticSystemService(_eventBusMock.Object, _dirConfigMock.Object);
    }

    [Fact]
    public async Task StartAsync_WhenCalled_SubscribesToEvents()
    {
        // Act
        await _sut.StartAsync();

        // Assert
        _eventBusMock.Verify(x => x.Subscribe(It.IsAny<IEventBusListener<EngineStartedEvent>>()), Times.Once);
        _eventBusMock.Verify(x => x.Subscribe(It.IsAny<IEventBusListener<EngineShuttingDownEvent>>()), Times.Once);
    }

    [Fact]
    public async Task OnEngineStarted_WhenCalled_CreatesPidFile()
    {
        // Arrange
        var evt = new EngineStartedEvent();

        // Act
        await _sut.OnEventAsync(evt);

        // Assert
        Assert.True(File.Exists(_sut.PidFileName));

        // Cleanup
        if (File.Exists(_sut.PidFileName))
            File.Delete(_sut.PidFileName);
    }

    [Fact]
    public async Task OnEngineShuttingDown_WhenCalled_DeletesPidFile()
    {
        // Arrange
        var evt = new EngineShuttingDownEvent();
        File.WriteAllText(_sut.PidFileName, "test");

        // Act
        await _sut.OnEventAsync(evt);

        // Assert
        Assert.False(File.Exists(_sut.PidFileName));
    }

    [Fact]
    public void PidFileName_WhenAccessed_ReturnsCorrectPath()
    {
        // Act
        string pidFilePath = _sut.PidFileName;

        // Assert
        Assert.EndsWith("orionserver.pid", pidFilePath);
    }
}
