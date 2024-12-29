namespace Astralis.Game.Client.Data.Serialization.TileSet;

public class TileSetData
{
    public string FileName { get; set; }
    public int TileWidth { get; set; }
    public int TileHeight { get; set; }
    public int Spacing { get; set; } = 0;
    public int Margin { get; set; } = 0;

    public string TilePrefix { get; set; } = string.Empty;

    public Dictionary<string, int> Mapping { get; set; } = new();
}
