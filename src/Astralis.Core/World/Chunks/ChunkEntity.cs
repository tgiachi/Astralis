using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Astralis.Core.Numerics;
using Astralis.Core.World.Blocks;

namespace Astralis.Core.World.Chunks;

public class ChunkEntity
{
    public const int CHUNK_SIZE = 16;
    public const int CHUNK_HEIGHT = 16;
    public const int TOTAL_SIZE = CHUNK_SIZE * CHUNK_HEIGHT * CHUNK_SIZE;

    public BlockType[] Blocks { get; } = new BlockType[TOTAL_SIZE];
    public Vector3Int Position { get; }
    public bool IsModified { get; set; }

    public ChunkEntity(Vector3Int position)
    {
        Position = position;
    }


    public ChunkEntity(Vector3Int position, IEnumerable<BlockType> blocks)
    {
        Blocks = blocks.ToArray();
        Position = position;
    }

    public static int GetIndexFromPosition(Vector3Int position) =>
        position.X + CHUNK_SIZE * position.Y + CHUNK_SIZE * CHUNK_HEIGHT * position.Z;

    public static Vector3Int GetPositionFromIndex(int index) =>
        new(index % CHUNK_SIZE, (index / CHUNK_SIZE) % CHUNK_HEIGHT, index / (CHUNK_SIZE * CHUNK_HEIGHT));

    public BlockType this[int index]
    {
        get => Blocks[index];
        set => Blocks[index] = value;
    }

    public BlockType this[int x, int y, int z]
    {
        get => GetBlock(x, y, z);
        set => SetBlock(x, y, z, value);
    }

    public BlockType this[Vector3Int position]
    {
        get => GetBlock(position);
        set => SetBlock(position, value);
    }

    public static List<Vector3Int> GetChunkPositionsAroundPosition(Vector3 position, int radius)
    {
        var chunkPositions = new List<Vector3Int>();

        for (var x = -radius; x <= radius; x++)
        {
            for (var y = -radius; y <= radius; y++)
            {
                for (var z = -radius; z <= radius; z++)
                {
                    var chunkPosition = new Vector3Int(
                        (int)position.X / CHUNK_SIZE + x,
                        (int)position.Y / CHUNK_HEIGHT + y,
                        (int)position.Z / CHUNK_SIZE + z
                    );

                    chunkPositions.Add(chunkPosition);
                }
            }
        }

        return chunkPositions;
    }

    public void SetBlock(int x, int y, int z, BlockType type) =>
        SetBlock(new Vector3Int(x, y, z), type);

    public void SetBlock(Vector3Int position, BlockType type) =>
        Blocks[GetIndexFromPosition(position)] = type;

    public BlockType GetBlock(int x, int y, int z) =>
        (x >= 0 && x < CHUNK_SIZE && y >= 0 && y < CHUNK_HEIGHT && z >= 0 && z < CHUNK_SIZE)
            ? Blocks[GetIndexFromPosition(new Vector3Int(x, y, z))]
            : BlockType.Air;

    public BlockType GetBlock(Vector3Int localPosition) =>
        GetBlock(localPosition.X, localPosition.Y, localPosition.Z);

    public string GetBlockStats(double minimumPercentage = 0.1)
    {
        var blockCounts = new Dictionary<BlockType, int>();

        foreach (var block in Blocks)
        {
            blockCounts.TryAdd(block, 0);
            blockCounts[block]++;
        }

        var composition = new List<string>();
        foreach (var kvp in blockCounts.OrderByDescending(x => x.Value))
        {
            double percentage = (kvp.Value * 100.0) / TOTAL_SIZE;
            if (percentage >= minimumPercentage)
            {
                composition.Add($"{kvp.Key}: {percentage:F1}%");
            }
        }

        return composition.Count > 0
            ? string.Join(", ", composition)
            : "Empty chunk";
    }
}
