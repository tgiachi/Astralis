using Astralis.Game.Client.Data.Serialization.Blocks;

namespace Astralis.Game.Client.Interfaces.Services;

public interface IBlockService
{
    BlockDefinitionEntry GetBlockDefinition(string name);
}
