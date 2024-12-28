using Astralis.Core.Extensions;
using Astralis.Game.Client.Types;

namespace Astralis.Game.Client.Data.Config;

public class AssetDirectories
{
    private readonly string _rootDirectory;

    public AssetDirectories(string rootDirectory)
    {
        _rootDirectory = rootDirectory;

        Init();
    }

    public string Root => _rootDirectory;

    public string this[AssetDirectoryType directoryType] => GetPath(directoryType);

    public string GetPath(AssetDirectoryType directoryType)
    {
        return Path.Combine(_rootDirectory, directoryType.ToString().ToSnakeCase());
    }

    private void Init()
    {
        if (!Directory.Exists(_rootDirectory))
        {
            Directory.CreateDirectory(_rootDirectory);
        }

        var directoryTypes = Enum.GetValues<AssetDirectoryType>().ToList();

        directoryTypes.Remove(AssetDirectoryType.Root);


        foreach (var directory in directoryTypes)
        {
            var path = GetPath(directory);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }

    public List<string> ScanDirectory(AssetDirectoryType directoryType, string filter = "*.*")
    {
        var path = GetPath(directoryType);

        return !Directory.Exists(path) ? new List<string>() : Directory.GetFiles(path, filter).ToList();
    }
}
