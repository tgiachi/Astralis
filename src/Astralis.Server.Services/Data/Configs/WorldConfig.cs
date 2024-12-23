namespace Astralis.Server.Services.Data.Configs;

public class WorldConfig
{
    public long Seed { get; set; } = -1;

    public int RenderDistance { get; set; } = 5;

    public long ChunkUnloadTime { get; set; } = 60 * 5;

    public long WorldSaveInterval { get; set; } = 60 * 5;
}
