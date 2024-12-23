namespace Astralis.Core.Server.Utils;

public static class EnvUtils
{
    public static bool IsOnDocker => Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
}
