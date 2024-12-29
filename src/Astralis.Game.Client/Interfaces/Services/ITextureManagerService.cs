namespace Astralis.Game.Client.Interfaces.Services;

public interface ITextureManagerService : IDisposable
{
    void LoadTexture(string name, string path);

    void LoadTexture(string name, byte[] data);

    void LoadTileSet(string fileName);
}
