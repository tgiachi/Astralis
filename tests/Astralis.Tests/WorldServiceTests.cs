using System.Numerics;
using Astralis.Core.Interfaces.Services;
using Astralis.Core.Server.Data.Directories;
using Astralis.Core.Server.Extensions;
using Astralis.Core.Server.Interfaces.Services.System;
using Astralis.Core.Server.Types;
using Astralis.Core.World.Chunks;
using Astralis.Server.Services.Data.Configs;
using Astralis.Server.Services.Impl;
using Moq;

namespace Astralis.Tests;

public class WorldServiceTests : IDisposable
{
    private readonly Mock<IProcessQueueService> _processQueueMock;
    private readonly Mock<ISchedulerSystemService> _schedulerMock;
    private readonly WorldConfig _worldConfig;
    private readonly DirectoriesConfig _dirConfig;
    private readonly WorldService _worldService;
    private readonly string _testPath;

    public WorldServiceTests()
    {
        Mock<IEventBusService> eventBusMock = new();
        _processQueueMock = new Mock<IProcessQueueService>();
        _schedulerMock = new Mock<ISchedulerSystemService>();

        // Setup test directory
        _testPath = Path.Combine(Path.GetTempPath(), "OrionWorldTests");
        Directory.CreateDirectory(_testPath);
        _dirConfig = new DirectoriesConfig(_testPath);

        _worldConfig = new WorldConfig
        {
            Seed = 12345,
            RenderDistance = 2,
            ChunkUnloadTime = 300,
            WorldSaveInterval = 300
        };

        _worldService = new WorldService(
            eventBusMock.Object,
            _processQueueMock.Object,
            _dirConfig,
            _schedulerMock.Object
        )
        {
            WorldConfig = _worldConfig
        };
    }

    [Fact]
    public async Task GetChunkByWorldPosition_ShouldGenerateNewChunk_WhenChunkDoesNotExist()
    {
        // Arrange
        var position = new Vector3(0, 0, 0);
        _processQueueMock
            .Setup(
                x => x.Enqueue(
                    It.IsAny<string>(),
                    It.IsAny<Func<Task<ChunkEntity>>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns((string ctx, Func<Task<ChunkEntity>> func, CancellationToken token) => func());

        // Act
        var chunk = await _worldService.GetChunkByWorldPositionAsync(position);

        // Assert
        Assert.NotNull(chunk);
        Assert.Equal(0, chunk.Position.X);
        Assert.Equal(0, chunk.Position.Y);
        Assert.Equal(0, chunk.Position.Z);
        Assert.Equal(ChunkEntity.TOTAL_SIZE, chunk.Blocks.Length);
    }

    [Fact]
    public async Task GetChunkByWorldPosition_ShouldReturnExistingChunk_WhenChunkExists()
    {
        // Arrange
        var position = new Vector3(0, 0, 0);
        _processQueueMock
            .Setup(
                x => x.Enqueue(
                    It.IsAny<string>(),
                    It.IsAny<Func<Task<ChunkEntity>>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns((string ctx, Func<Task<ChunkEntity>> func, CancellationToken token) => func());

        var firstChunk = await _worldService.GetChunkByWorldPositionAsync(position);

        // Act
        var secondChunk = await _worldService.GetChunkByWorldPositionAsync(position);

        // Assert
        Assert.Same(firstChunk, secondChunk);
    }

    [Fact]
    public async Task SaveWorld_ShouldCreateWorldFile()
    {
        // Arrange
        var position = new Vector3(0, 0, 0);
        _processQueueMock
            .Setup(
                x => x.Enqueue(
                    It.IsAny<string>(),
                    It.IsAny<Func<Task<ChunkEntity>>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns((string ctx, Func<Task<ChunkEntity>> func, CancellationToken token) => func());

        await _worldService.GetChunkByWorldPositionAsync(position);

        _processQueueMock
            .Setup(
                x => x.Enqueue(
                    ProcessQueueServiceExtension.DefaultContext,
                    It.IsAny<Func<Task>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns((string ctx, Func<Task> func, CancellationToken token) => func());

        // Act
        await _worldService.SaveWorldAsync();

        // Assert
        var worldFile = Path.Combine(_dirConfig[DirectoryType.Worlds], $"{_worldConfig.Seed}.world");
        Assert.True(File.Exists(worldFile));
    }

    [Fact]
    public async Task OnReady_ShouldInitializeWorldSeed_WhenSeedIsNegative()
    {
        // Arrange
        _worldConfig.Seed = -1;
        _processQueueMock
            .Setup(
                x => x.Enqueue(
                    It.IsAny<string>(),
                    It.IsAny<Func<Task<ChunkEntity>>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns((string ctx, Func<Task<ChunkEntity>> func, CancellationToken token) => func());

        // Act
        await _worldService.OnReadyAsync();

        // Assert
        Assert.NotEqual(-1, _worldConfig.Seed);
        _schedulerMock.Verify(
            x => x.AddSchedulerJob(
                "SaveWorld",
                TimeSpan.FromSeconds(_worldConfig.WorldSaveInterval),
                It.IsAny<Func<Task>>()
            ),
            Times.Once
        );
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(16, 0, 0)]
    [InlineData(-16, 0, 0)]
    [InlineData(0, 0, 16)]
    public async Task GetChunkByWorldPosition_ShouldCalculateCorrectChunkPosition(int x, int y, int z)
    {
        // Arrange
        var worldPosition = new Vector3(x, y, z);
        var expectedChunkPos = new Vector3(
            x / ChunkEntity.CHUNK_SIZE,
            y / ChunkEntity.CHUNK_HEIGHT,
            z / ChunkEntity.CHUNK_SIZE
        );

        _processQueueMock
            .Setup(
                x => x.Enqueue(
                    It.IsAny<string>(),
                    It.IsAny<Func<Task<ChunkEntity>>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns((string ctx, Func<Task<ChunkEntity>> func, CancellationToken token) => func());

        // Act
        var chunk = await _worldService.GetChunkByWorldPositionAsync(worldPosition);

        // Assert
        Assert.Equal(expectedChunkPos, new Vector3(chunk.Position.X, chunk.Position.Y, chunk.Position.Z));
    }


    [Fact]
    public void Dispose()
    {
        // Cleanup test directory after tests
        if (Directory.Exists(_testPath))
        {
            Directory.Delete(_testPath, true);
        }
    }
}
