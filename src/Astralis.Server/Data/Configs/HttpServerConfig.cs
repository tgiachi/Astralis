namespace Astralis.Server.Data.Configs;

public class HttpServerConfig
{
    public int Port { get; set; }

    public string DefaultIndex { get; set; } = "index.html";

    public HttpServerConfig()
    {
        Port = 8080;
    }

    public HttpServerConfig(int port)
    {
        Port = port;
    }
}
