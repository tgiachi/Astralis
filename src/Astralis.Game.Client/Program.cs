using Astralis.Game.Client.Data;
using Astralis.Game.Client.Data.Config;
using Astralis.Game.Client.Impl;
using Serilog;
using Silk.NET.Windowing;


namespace Astralis.Game.Client;

class Program
{
    public static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .MinimumLevel.Debug()
            .CreateLogger();

        Log.Logger.Information("View thread id: {ThreadId}", Environment.CurrentManagedThreadId);


        AstralisGameInstances.AssetDirectories = new AssetDirectories(Path.Combine(Directory.GetCurrentDirectory(), "Assets"));

        AstralisGameInstances.ServiceProvider = new AstralisServiceProvider();

        AstralisGameInstances.VariablesService();
        AstralisGameInstances.VersionService();
        AstralisGameInstances.FontManagerService();
        AstralisGameInstances.EcsService();

        AstralisGameInstances.OpenGlContext = new OpenGlContext(
            new AstralisGameConfig(),
            AstralisGameInstances.EventBusService()
        );

        try
        {
            var window = AstralisGameInstances.OpenGlContext.Window;
            window.Run();

        }
        catch (Exception e)
        {
            Log.Logger.Error(e, "An error occurred while running the game.");
        }
        finally
        {
            AstralisGameInstances.OpenGlContext.Stop();
        }


    }
}
