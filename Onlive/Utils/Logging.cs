using Microsoft.Extensions.Logging;

namespace Onlive.Utils;

public static class Logging
{
    private static readonly ILoggerFactory CustomLoggerFactory = LoggerFactory.Create(builder => builder
        .AddConsole()
    );

    public static ILogger<T> GetLogger<T>()
    {
        return CustomLoggerFactory.CreateLogger<T>();
    }
}
