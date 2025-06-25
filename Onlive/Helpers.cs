using Microsoft.Extensions.Logging;
using OnliveConstants;
using SFML.System;

namespace Onlive;

public static class Helpers
{
    public static Position PositionFromVector2(Vector2i vector) => new(vector.X, vector.Y);
    public static Position PositionFromVector2(Vector2f vector) => new(vector.X, vector.Y);

    public static ILogger<T> GetLogger<T>() => _loggerFactory.CreateLogger<T>();

    private static ILoggerFactory _loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
}
