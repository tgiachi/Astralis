using System.Text.Json;
using Astralis.Core.Extensions;
using Astralis.Core.World.Blocks;
using Astralis.Game.Client.Data.Serialization.Blocks;
using Astralis.Game.Client.Interfaces.Services;
using Astralis.Game.Client.Types;
using Serilog;

namespace Astralis.Game.Client.Impl;

public class BlockService : IBlockService
{
    private readonly ILogger _logger = Log.ForContext<BlockService>();

    private readonly Dictionary<BlockType, BlockDefinitionEntry> _blockDefinitions = new();

    public BlockService()
    {
        LoadBlockDefinitions();
    }

    private void LoadBlockDefinitions()
    {
        _logger.Information("Loading block definitions");

        var blockDefinitions = AstralisGameInstances.AssetDirectories.ScanDirectory(AssetDirectoryType.Blocks, "*.json");

        _logger.Information("Found {Count} block definitions", blockDefinitions.Count);

        foreach (var blockDefinition in blockDefinitions)
        {
            var definition = LoadFromJson(blockDefinition);

            if (definition == null)
            {
                continue;
            }

            foreach (var block in definition)
            {
                if (!_blockDefinitions.TryAdd(block.BlockType, block))
                {
                    _logger.Warning("Duplicate block definition for {Type}", block.BlockType);
                    continue;
                }

                _logger.Information("Loaded block definition for {Type}", block.BlockType);
            }
        }
    }

    private List<BlockDefinitionEntry>? LoadFromJson(string path)
    {
        try
        {
            var json = File.ReadAllText(path).FromJson<BlockDefinition>();

            return json?.Blocks;
        }
        catch (Exception e)
        {
            _logger.Error(e, "Failed to load block definition from {Path}", path);
            throw;
        }
    }

    public BlockDefinitionEntry GetBlockDefinition(string name)
    {
        var blockType = Enum.Parse<BlockType>(name, true);

        if (!_blockDefinitions.TryGetValue(blockType, out var definition))
        {
            throw new Exception($"Block definition not found for {name}");
        }

        return definition;
    }
}
