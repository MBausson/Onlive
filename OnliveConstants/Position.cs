namespace OnliveConstants;

public struct Position(int x, int y)
{
    public int X { get; set; } = x;
    public int Y { get; set; } = y;

    public Position(float x, float y) : this(Convert.ToInt32(x), Convert.ToInt32(y))
    {
    }

    public string ToRequestString() => $"{X};{Y}";

    public override string ToString() => $"[{X};{Y}]";

    public static Position Zero => new(0, 0);

    public static Position operator +(Position left, Position right)
    {
        return new(left.X + right.X, left.Y + right.Y);
    }

    public static Position operator -(Position left, Position right)
    {
        return new(left.X - right.X, left.Y - right.Y);
    }

    public static Position operator *(Position left, Position right)
    {
        return new(left.X * right.X, left.Y * right.Y);
    }

    public static Position operator /(Position left, Position right)
    {
        return new(left.X / right.X, left.Y / right.Y);
    }

    public static Position operator +(Position left, int right)
    {
        return new(left.X + right, left.Y + right);
    }

    public static Position operator +(int left, Position right)
    {
        return new(left + right.X, left + right.Y);
    }
}
