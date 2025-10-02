using System.CommandLine;
using Microsoft.Extensions.Logging;

namespace OnliveServer.Utils;

public struct ServerConfiguration
{
    private const int DefaultServerPort = 8001;
    private const LogLevel DefaultLogLevel = LogLevel.Debug;
    private const int DefaultUpdatingInterval = 300;

    public static ServerConfiguration Current { get; private set; }

    public string ServerIp { get; init; }
    public int ServerPort { get; init; }
    public LogLevel LogLevel { get; init; }
    public int UpdateInterval { get; init; }

    public static void SetFromCliArguments(string[] arguments)
    {
        var serverPortOption = new Option<int>("--port")
            { Description = "The server's socket port", DefaultValueFactory = _ => DefaultServerPort };

        var logLevelOption = new Option<LogLevel>("--log")
            { Description = "The minimum log level", DefaultValueFactory = _ => DefaultLogLevel };

        var updateInterval = new Option<int>("--interval")
        {
            Description = "Interval between each game board update", DefaultValueFactory = _ => DefaultUpdatingInterval
        };

        var rootCommand = new RootCommand
        {
            serverPortOption,
            logLevelOption,
            updateInterval
        };

        var result = rootCommand.Parse(arguments);

        Current = new ServerConfiguration
        {
            ServerPort = result.GetRequiredValue(serverPortOption),
            LogLevel = result.GetRequiredValue(logLevelOption),
            UpdateInterval = result.GetRequiredValue(updateInterval)
        };
    }
}
