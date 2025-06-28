using OnliveConstants;
using SFML.System;

namespace Onlive.Utils;

public static class VectorExtensions
{
    public static Position ToPosition(this Vector2f position)
    {
        return new Position(position.X, position.Y);
    }

    public static Position ToPosition(this Vector2i position)
    {
        return new Position(position.X, position.Y);
    }
}