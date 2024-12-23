namespace Astralis.Server.Data.Configs;

public class NetworkServerConfig
{
    public int Port { get; set; } = 5006;


    public NetworkServerConfig()
    {
    }

    public NetworkServerConfig(int port)
    {
        Port = port;
    }
}
