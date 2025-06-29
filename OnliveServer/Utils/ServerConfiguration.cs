using System.CommandLine;
using Microsoft.Extensions.Logging;

namespace OnliveServer.Utils;

public struct ServerConfiguration
{
    private const string DefaultServerIp = "127.0.0.1";
    private const int DefaultServerPort = 8001;
    private const LogLevel DefaultLogLevel = LogLevel.Debug;

    public static ServerConfiguration Current { get; private set; }

    public string ServerIp { get; init; }
    public int ServerPort { get; init; }
    public LogLevel LogLevel { get; init; }

    public static void SetFromCliArguments(string[] arguments)
    {
        var serverIpOption = new Option<string>("--ip")
            { Description = "The server's locale IP", DefaultValueFactory = _ => DefaultServerIp };

        var serverPortOption = new Option<int>("--port")
            { Description = "The server's socket port", DefaultValueFactory = _ => DefaultServerPort };

        var logLevelOption = new Option<LogLevel>("--log")
            { Description = "The minimum log level", DefaultValueFactory = _ => DefaultLogLevel };

        var rootCommand = new RootCommand
        {
            serverIpOption,
            serverPortOption,
            logLevelOption
        };

        var result = rootCommand.Parse(arguments);

        Current = new ServerConfiguration
        {
            ServerIp = result.GetRequiredValue(serverIpOption),
            ServerPort = result.GetRequiredValue(serverPortOption),
            LogLevel = result.GetRequiredValue(logLevelOption)
        };
    }
}
