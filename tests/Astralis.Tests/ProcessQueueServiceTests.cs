using Astralis.Server.Data.Configs;
using Astralis.Server.Services;

namespace Astralis.Tests;

public class ProcessQueueServiceTests
{
    private readonly ProcessQueueService _sut;

    public ProcessQueueServiceTests()
    {
        _sut = new ProcessQueueService(new ProcessQueueConfig(2));
    }

    [Fact]
    public async Task Enqueue_Action_ExecutesSuccessfully()
    {
        // Arrange
        var executed = false;

        // Act
        await _sut.Enqueue("test", () => executed = true);

        // Assert
        Assert.True(executed);
    }

    [Fact]
    public async Task Enqueue_FuncT_ReturnsCorrectValue()
    {
        // Arrange
        const string expected = "test result";

        // Act
        var result = await _sut.Enqueue("test", () => expected);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task Enqueue_AsyncFunc_ExecutesSuccessfully()
    {
        // Arrange
        var executed = false;

        // Act
        await _sut.Enqueue(
            "test",
            async () =>
            {
                await Task.Delay(10);
                executed = true;
            }
        );

        // Assert
        Assert.True(executed);
    }

    [Fact]
    public async Task Enqueue_AsyncFuncT_ReturnsCorrectValue()
    {
        // Arrange
        const string expected = "async result";

        // Act
        var result = await _sut.Enqueue(
            "test",
            async () =>
            {
                await Task.Delay(10);
                return expected;
            }
        );

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task Enqueue_WhenCancelled_ThrowsTaskCanceledException()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(
            () =>
                _sut.Enqueue("test", () => Task.Delay(100), cts.Token)
        );
    }

    [Fact]
    public async Task Enqueue_WhenExceptionThrown_PropagatesException()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Test exception");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () =>
                _sut.Enqueue("test", () => throw expectedException)
        );

        Assert.Equal(expectedException.Message, exception.Message);
    }

    [Fact]
    public async Task Enqueue_ParallelExecution_RespectsMaxParallelTasks()
    {
        // Arrange
        var concurrentTasks = 0;
        var maxConcurrentTasks = 0;
        var lockObj = new object();

        // Act
        var tasks = Enumerable.Range(0, 5)
            .Select(
                async i =>
                {
                    await _sut.Enqueue(
                        "test",
                        async () =>
                        {
                            lock (lockObj)
                            {
                                concurrentTasks++;
                                maxConcurrentTasks = Math.Max(maxConcurrentTasks, concurrentTasks);
                            }

                            await Task.Delay(50);

                            lock (lockObj)
                            {
                                concurrentTasks--;
                            }
                        }
                    );
                }
            );

        await Task.WhenAll(tasks);

        // Assert
        Assert.Equal(2, maxConcurrentTasks); // maxParallelTasks è 2
    }




}
