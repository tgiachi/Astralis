using Silk.NET.Maths;

namespace Astralis.Game.Client.Data;

public class AstralisGameConfig
{
    public string Title { get; set; } = "Astralis Game";
    public bool EnableVSync { get; set; }
    public bool FullScreen { get; set; }
    public Vector2D<int> WindowSize { get; set; } = new Vector2D<int>(1920, 1080);
}
