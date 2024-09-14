using SFML.System;

namespace AStar;

public class Tilemap
{
    public Vector2i GridSize { get; }

    private readonly Tile[,] _map;

    public Tilemap(Vector2i gridSize)
    {
        GridSize = gridSize;

        _map = new Tile[gridSize.X, gridSize.Y];
        for (int y = 0; y < gridSize.Y; y++)
            for (int x = 0; x < gridSize.X; x++)
            {
                Tile t = new(new Vector2i(x, y));
                _map[x, y] = t;
            }
    }

    public Tile? GetTileAt(Vector2i position)
    {
        return IsInBounds(position) ? _map[position.X, position.Y] : null;
    }

    private bool IsInBounds(Vector2i position)
    {
        return position.X >= 0 && position.X < GridSize.X &&
            position.Y >= 0 && position.Y < GridSize.Y;
    }
}
