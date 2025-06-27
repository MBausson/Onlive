using OnliveConstants;
using SFML.System;

namespace Onlive.Utils;

public static class VectorExtensions
{
    public static Position ToPosition(this Vector2f position)
    {
        return new(position.X, position.Y);
    }

    public static Position ToPosition(this Vector2i position)
    {
        return new(position.X, position.Y);
    }
}
