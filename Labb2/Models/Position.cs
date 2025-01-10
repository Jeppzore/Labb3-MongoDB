namespace Labb3_MongoDB.Models;

public struct Position(int x, int y)
{
    public int X { get; set; } = x;

    public int Y { get; set; } = y;

    public Position(Position position) : this(position.X, position.Y) { }
}