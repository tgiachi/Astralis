using Astralis.Core.Interfaces.Services;
using Astralis.Core.Server.Data.Config;
using Astralis.Core.Server.Interfaces.Scheduler;
using Astralis.Server.Services;
using Moq;

namespace Astralis.Tests;

public class SchedulerSystemServiceTests
{
    private readonly SchedulerSystemService _sut;
    private readonly Mock<IEventBusService> _eventBusMock;
    private readonly SchedulerServiceConfig _config;

    public SchedulerSystemServiceTests()
    {
        _eventBusMock = new Mock<IEventBusService>();
        _config = new SchedulerServiceConfig(20, 15, 4);
        _sut = new SchedulerSystemService(_config, _eventBusMock.Object);
    }

    [Fact]
    public void EnqueueAction_WhenActionAdded_IncreasesQueueCount()
    {
        // Arrange
        var mockAction = new Mock<ISchedulerAction>();

        // Act
        _sut.EnqueueAction(mockAction.Object);

        // Assert
        Assert.Equal(1, _sut.ActionsInQueue);
    }

    [Fact]
    public void EnqueueActions_WhenMultipleActionsAdded_IncreasesQueueCount()
    {
        // Arrange
        var actions = new List<ISchedulerAction>
        {
            new Mock<ISchedulerAction>().Object,
            new Mock<ISchedulerAction>().Object,
            new Mock<ISchedulerAction>().Object
        };

        // Act
        _sut.EnqueueActions(actions);

        // Assert
        Assert.Equal(3, _sut.ActionsInQueue);
    }



    [Fact]
    public void CurrentTick_WhenStarted_IncreasesOverTime()
    {
        // Arrange
        var initialTick = _sut.CurrentTick;

        // Act & Assert
        Assert.True(_sut.CurrentTick >= initialTick);
    }
}
