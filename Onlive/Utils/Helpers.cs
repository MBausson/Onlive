using Microsoft.Extensions.Logging;
using OnliveConstants;
using SFML.System;

namespace Onlive.Utils;

public static class Helpers
{
    public static ILogger<T> GetLogger<T>() => LoggerFactory.CreateLogger<T>();

    private static readonly ILoggerFactory LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder => builder.AddConsole());
}
