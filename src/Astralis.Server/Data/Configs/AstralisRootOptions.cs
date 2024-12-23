using Astralis.Core.Server.Types;
using CommandLine;

namespace Astralis.Server.Data.Configs;

public class AstralisRootOptions
{
    [Option('r', "root-dir", Required = false, HelpText = "Root directory for Astralis server.")]
    public string RootPath { get; set; } = string.Empty;

    [Option('d', "database-connection-string", Required = false, HelpText = "Database connection string.")]
    public string DatabaseConnectionString { get; set; } = "Data Source=orion.db";

    [Option('t', "database-type", Required = false, HelpText = "Database type.")]
    public AstralisDatabaseType DatabaseType { get; set; } = AstralisDatabaseType.LiteDb;

    [Option('l', "log-level", Required = false, HelpText = "Log level.")]
    public AstralisLogLevel LogLevel { get; set; } = AstralisLogLevel.Debug;

    [Option('p', "server-port", Required = false, HelpText = "Server port.")]
    public int ServerPort { get; set; } = 5006;

    [Option('f', "log-on-file", Required = false, HelpText = "Log on file.")]
    public bool LogOnFile { get; set; } = true;

    [Option('c', "max-concurrent-process", Required = false, HelpText = "Max concurrent process.")]
    public int MaxConcurrentProcess { get; set; } = 4;


    [Option( "http-port", Required = false, HelpText = "Http port.")]
    public int HttpPort { get; set; } = 5005;
}
