using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using Astralis.Core.Server.Data.Directories;
using Astralis.Core.Server.Extensions;
using Astralis.Core.Server.Interfaces.Services.System;
using Astralis.Core.Server.Types;
using Astralis.Server.Data.Configs;
using Serilog;
using WatsonWebserver;
using WatsonWebserver.Core;
using HttpMethod = WatsonWebserver.Core.HttpMethod;


namespace Astralis.Server.Services;

public partial class HttpServerService : IHttpServerService
{
    private readonly Webserver _server;
    private readonly DirectoriesConfig _directoriesConfig;
    private readonly HttpServerConfig _config;
    private readonly ILogger _logger = Log.ForContext<HttpServerService>();

    private readonly IVariablesService _variablesService;
    private bool _isRunning;

    public HttpServerService(
        HttpServerConfig config, DirectoriesConfig directoriesConfig, IVariablesService variablesService
    )
    {
        _config = config;
        _directoriesConfig = directoriesConfig;
        _variablesService = variablesService;
        _server = new Webserver(new WebserverSettings("127.0.0.1", config.Port), DefaultRoute);
        InitializeHttpDirectory();
    }

    private void InitializeHttpDirectory()
    {
        var httpPath = _directoriesConfig[DirectoryType.Http];
        _logger.Debug("Initializing HTTP directory at {Path}", httpPath);


        _server.Routes.PreAuthentication.Dynamic.Add(
            HttpMethod.GET,
            RootRegex(),
            async (ctx) =>
            {
                var requestPath = ctx.Request.Url.RawWithoutQuery.TrimStart('/');
                var fullPath = string.IsNullOrEmpty(requestPath)
                    ? Path.Combine(httpPath, _config.DefaultIndex)
                    : Path.Combine(httpPath, requestPath);


                if (Directory.Exists(fullPath))
                {
                    fullPath = Path.Combine(fullPath, _config.DefaultIndex);
                }

                var (content, contentType, statusCode) = await ReadFromFileSystem(fullPath);

                ctx.Response.StatusCode = statusCode;
                ctx.Response.ContentType = contentType;
                await ctx.Response.Send(content);
            }
        );
    }

    public void AddContentRoute(string path, bool listFiles)
    {
        _logger.Debug("Adding content route for path {Path} (listFiles: {ListFiles})", path, listFiles);
        _server.Routes.PreAuthentication.Content.Add(path, listFiles);
    }

    public void AddStaticRoute(RouteMethodType method, string path, Func<HttpContextBase, Task> handler)
    {
        _logger.Debug("Adding static route {Method} {Path}", method, path);
        _server.Routes.PreAuthentication.Static.Add(
            method.ToHttpMethod(),
            path,
            async (ctx) =>
            {
                try
                {
                    await handler(ctx);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error in static route {Method} {Path}", method, path);
                    ctx.Response.StatusCode = 500;
                    await ctx.Response.Send("Internal Server Error");
                }
            }
        );
    }

    public void AddParameterRoute(RouteMethodType method, string path, Func<HttpContextBase, Task> handler)
    {
        _logger.Debug("Adding parameter route {Method} {Path}", method, path);
        _server.Routes.PreAuthentication.Parameter.Add(
            method.ToHttpMethod(),
            path,
            async (ctx) =>
            {
                try
                {
                    await handler(ctx);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error in parameter route {Method} {Path}", method, path);
                    ctx.Response.StatusCode = 500;
                    await ctx.Response.Send("Internal Server Error");
                }
            }
        );
    }

    private async Task<(byte[] Content, string ContentType, int StatusCode)> ReadFromFileSystem(string fileName)
    {
        try
        {
            if (!File.Exists(fileName))
            {
                _logger.Warning("File not found: {FileName}", fileName);
                return ("404 - File Not Found"u8.ToArray(), "text/plain", 404);
            }

            string contentType = GetContentType(fileName);
            byte[] content;

            if (contentType == "text/html")
            {
                string htmlContent = await File.ReadAllTextAsync(fileName);
                string processedContent = _variablesService.TranslateText(htmlContent);
                content = Encoding.UTF8.GetBytes(processedContent);
            }
            else
            {
                content = await File.ReadAllBytesAsync(fileName);
            }

            _logger.Debug("Successfully read file: {FileName}", fileName);
            return (content, contentType, 200);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error reading file: {FileName}", fileName);
            return (Encoding.UTF8.GetBytes("500 - Internal Server Error"), "text/plain", 500);
        }
    }


    public static string GetLocalIPViaConnection()
    {
        using Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0);
        socket.Connect("8.8.8.8", 65530);
        IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
        return endPoint.Address.ToString();
    }


    public void AddDynamicRoute(RouteMethodType method, Regex pattern, Func<HttpContextBase, Task> handler)
    {
        _logger.Debug("Adding dynamic route {Method} {Pattern}", method, pattern);
        _server.Routes.PreAuthentication.Dynamic.Add(
            method.ToHttpMethod(),
            pattern,
            async (ctx) =>
            {
                try
                {
                    await handler(ctx);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error in dynamic route {Method} {Pattern}", method, pattern);
                    ctx.Response.StatusCode = 500;
                    await ctx.Response.Send("Internal Server Error");
                }
            }
        );
    }


    public async Task StartAsync()
    {
        if (!_isRunning)
        {
            _logger.Information(
                "Starting HTTP server on http://{Hostname}:{Port}",
                _server.Settings.Hostname,
                _server.Settings.Port
            );
            _server.StartAsync();
            _isRunning = true;
        }
    }

    public async Task StopAsync()
    {
        if (_isRunning)
        {
            _logger.Information("Stopping HTTP server");
            _server.Stop();
            _isRunning = false;
        }
    }


    static async Task DefaultRoute(HttpContextBase ctx) =>
        await ctx.Response.Send("Hello from the default route!");


    private string GetContentType(string path)
    {
        return Path.GetExtension(path).ToLower() switch
        {
            ".html"           => "text/html",
            ".css"            => "text/css",
            ".js"             => "application/javascript",
            ".json"           => "application/json",
            ".png"            => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".gif"            => "image/gif",
            ".svg"            => "image/svg+xml",
            ".ico"            => "image/x-icon",
            _                 => "application/octet-stream"
        };
    }


    [GeneratedRegex("/*")]
    private static partial Regex RootRegex();
}
