using Astralis.Game.Client.Core.Textures;

namespace Astralis.Game.Client.Interfaces.Services;

public interface ITextureManagerService : IDisposable
{
    void LoadTexture(string name, string path);

    void LoadTexture(string name, byte[] data, int width, int height);

    void LoadTileSet(string fileName);

    Texture2d GetTexture(string name);

}
