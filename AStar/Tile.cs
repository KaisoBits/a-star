using SFML.System;

namespace AStar;

public class Tile
{
    public Vector2i Position { get; }

    public bool IsWalkable { get; set; } = true;
    public int Cost { get; set; } = 1;

    public Tile(Vector2i position)
    {
        Position = position;
    }
}
