using Astralis.Core.Interfaces.Events;
using Astralis.Core.Services;

namespace Astralis.Tests;

public class EventBusServiceTests
{
    private readonly EventBusService _sut;

    public EventBusServiceTests()
    {
        _sut = new EventBusService();
    }

    [Fact]
    public async Task PublishAsync_WhenEventPublished_SubscriberReceivesEvent()
    {
        // Arrange
        var testEvent = new TestEvent { Message = "Test" };
        var receivedEvent = false;

        _sut.Subscribe<TestEvent>(e =>
        {
            Assert.Equal("Test", e.Message);
            receivedEvent = true;
        });

        // Act
        await _sut.PublishAsync(testEvent);

        // Assert
        Assert.True(receivedEvent);
    }

    [Fact]
    public void Subscribe_WhenMultipleSubscribers_AllReceiveEvents()
    {
        // Arrange
        var testEvent = new TestEvent { Message = "Test" };
        var subscriber1Called = false;
        var subscriber2Called = false;

        _sut.Subscribe<TestEvent>(_ => subscriber1Called = true);
        _sut.Subscribe<TestEvent>(_ => subscriber2Called = true);

        // Act
        _sut.Publish(testEvent);

        // Assert
        Assert.True(subscriber1Called);
        Assert.True(subscriber2Called);
    }



    private class TestEvent : IAstralisEvent
    {
        public string Message { get; set; }
    }
}
