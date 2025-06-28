using Microsoft.Extensions.Logging;

namespace Onlive.Utils;

public static class Logging
{
    public static ILogger<T> GetLogger<T>() => CustomLoggerFactory.CreateLogger<T>();

    private static readonly ILoggerFactory CustomLoggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
}
