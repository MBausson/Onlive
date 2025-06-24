using OnliveConstants;
using SFML.System;

namespace Onlive;

public static class Helpers
{
    public static Position PositionFromVector2(Vector2i vector) => new(vector.X, vector.Y);
    public static Position PositionFromVector2(Vector2f vector) => new(vector.X, vector.Y);
}
