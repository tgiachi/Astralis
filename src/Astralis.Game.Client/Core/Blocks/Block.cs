using System.Numerics;
using Astralis.Core.Numerics;
using Astralis.Core.World.Blocks;
using Astralis.Game.Client.Data.Serialization.Blocks;

namespace Astralis.Game.Client.Core.Blocks;

public class Block : ICloneable
{
    public Vector3Int Position { get; set; }
    public string Name { get; set; }
    public bool IsAir { get; set; }
    public bool IsTransparent { get; set; }
    public byte LightEmittance { get; set; }
    public BlockType BlockType { get; set; }
    public BlockData Data { get; set; }
    public BlockDefinitionEntry Definition { get; set; }

    // todo add texture

    public Block(BlockDefinitionEntry definition)
    {
        IsAir = definition.BlockType == BlockType.Air;
        Position = Vector3Int.Zero;
        Name = definition.Name;
        IsTransparent = definition.IsTransparent;
        LightEmittance = definition.LightEmittance;
        BlockType = definition.BlockType;
        Definition = definition;
        Data = new BlockData(definition.BlockType);
        Data.SetLightLevel(definition.LightEmittance);
    }

    public Block(
        Vector3Int position, string name = "", bool transparent = true, byte lightEmitting = 0,
        BlockType blockType = BlockType.Air, BlockDefinitionEntry? blockJson = null
    )
    {
        Position = position;
        Name = name;
        IsAir = blockType == BlockType.Air;
        IsTransparent = transparent;
        LightEmittance = lightEmitting;
        BlockType = blockType;
        Definition = blockJson ?? new BlockDefinitionEntry(blockType, name, transparent, lightEmitting);
        Data = new BlockData(blockType);
        Data.SetLightLevel(lightEmitting);
    }

    public override string ToString()
    {
        return $"{BlockType} at {Position}";
    }

    public object Clone()
    {
        return new Block(Definition);
    }
}
