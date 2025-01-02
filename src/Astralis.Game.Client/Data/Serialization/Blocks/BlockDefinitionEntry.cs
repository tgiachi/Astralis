using Astralis.Core.World.Blocks;
using Astralis.Game.Client.Types;

namespace Astralis.Game.Client.Data.Serialization.Blocks;

public class BlockDefinitionEntry
{
    public BlockType BlockType { get; set; }
    public string Name { get; set; }
    public bool IsTransparent { get; set; }
    public byte LightEmittance { get; set; }

    public Dictionary<BlockFace, string> Textures { get; set; }

    public BlockDefinitionEntry()
    {
    }

    public BlockDefinitionEntry(BlockType blockType, string name, bool isTransparent, byte lightEmittance, params KeyValuePair<BlockFace, string>[] textures)
    {
        BlockType = blockType;
        Name = name;
        IsTransparent = isTransparent;
        LightEmittance = lightEmittance;

        Textures = new Dictionary<BlockFace, string>();
        foreach (var texture in textures)
        {
            Textures.Add(texture.Key, texture.Value);
        }
    }
}
