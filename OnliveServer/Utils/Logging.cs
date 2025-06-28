using Microsoft.Extensions.Logging;

namespace OnliveServer.Utils;

public static class Logging
{
    private static readonly ILoggerFactory CustomLoggerFactory = LoggerFactory.Create(builder =>
        builder.AddSimpleConsole(options =>
            {
                options.UseUtcTimestamp = true;
                options.TimestampFormat = "[HH:mm dd/MM/yyyy] ";
            })
            .SetMinimumLevel(ServerConfiguration.Current.LogLevel));

    public static ILogger<T> GetLogger<T>()
    {
        return CustomLoggerFactory.CreateLogger<T>();
    }
}