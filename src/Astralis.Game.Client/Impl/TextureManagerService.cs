using Astralis.Core.Extensions;
using Astralis.Core.Interfaces.Services;
using Astralis.Core.Server.Events.Engine;
using Astralis.Game.Client.Core.Textures;
using Astralis.Game.Client.Data.Serialization.TileSet;
using Astralis.Game.Client.Interfaces.Services;
using Astralis.Game.Client.Types;
using Serilog;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Astralis.Game.Client.Impl;

public class TextureManagerService : ITextureManagerService
{
    private readonly IEventBusService _eventBusService;
    private readonly ILogger _logger = Log.ForContext<TextureManagerService>();

    private readonly Dictionary<string, Texture> _textures = new();

    public TextureManagerService(IEventBusService eventBusService)
    {
        _eventBusService = eventBusService;

        _eventBusService.Subscribe<EngineStartedEvent>(OnEngineStarted);
    }

    private void OnEngineStarted(EngineStartedEvent obj)
    {
        _logger.Information("Loading textures");
        var textures = AstralisGameInstances.AssetDirectories.ScanDirectory(AssetDirectoryType.Textures, "*.png");

        foreach (var texture in textures)
        {
            var name = Path.GetFileNameWithoutExtension(texture).ToSnakeCase();
            LoadTexture(name, texture);
        }
    }

    public void Dispose()
    {
        foreach (var texture in _textures.Values)
        {
            texture.Dispose();
        }
    }

    public void LoadTexture(string name, string path)
    {
        _logger.Information("Loading texture {Name} from {Path}", name, path);
        if (_textures.TryGetValue(name, out Texture? value))
        {
            value.Dispose();
            _textures.Remove(name);
        }

        var texture = new Texture(AstralisGameInstances.OpenGlContext.Gl, path);
        _textures.Add(name, texture);
    }

    public void LoadTexture(string name, byte[] data)
    {
        _logger.Information("Loading texture {Name} from data", name);
        if (_textures.TryGetValue(name, out Texture? value))
        {
            value.Dispose();
            _textures.Remove(name);
        }

        // load image from data
        using Image<Rgba32> image = Image.Load<Rgba32>(data);


        var texture = new Texture(AstralisGameInstances.OpenGlContext.Gl, data, (uint)image.Width, (uint)image.Height);
        _textures.Add(name, texture);
    }

    public void LoadTileSet(string fileName)
    {
        try
        {
            var tileSetData = File.ReadAllText(fileName).FromJson<TileSetData>();
            var tileDictionary = new Dictionary<string, byte[]>();

            using Image<Rgba32> image = Image.Load<Rgba32>(tileSetData.FileName);
            int rows = (image.Height - tileSetData.Margin * 2 + tileSetData.Spacing) /
                       (tileSetData.TileHeight + tileSetData.Spacing);
            int cols = (image.Width - tileSetData.Margin * 2 + tileSetData.Spacing) /
                       (tileSetData.TileWidth + tileSetData.Spacing);

            foreach (var (name, tileIndex) in tileSetData.Mapping)
            {
                int tileX = tileIndex % cols;
                int tileY = tileIndex / cols;

                int x = tileSetData.Margin + tileX * (tileSetData.TileWidth + tileSetData.Spacing);
                int y = tileSetData.Margin + tileY * (tileSetData.TileHeight + tileSetData.Spacing);

                // Crop the tile

                var tile = image.Clone();
                tile.Mutate(ctx => ctx.Crop(new Rectangle(x, y, tileSetData.TileWidth, tileSetData.TileHeight)));

                // Convert tile to byte array
                byte[] tileData = new byte[tile.Width * tile.Height * 4]; // R, G, B, A for each pixel
                tile.CopyPixelDataTo(tileData);

                tileDictionary[name] = tileData;
            }


            foreach (var (name, img) in tileDictionary)
            {
                LoadTexture(name, img);
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to load tileset {FileName}", fileName);
        }
    }
}
