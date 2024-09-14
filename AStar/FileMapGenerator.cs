using SFML.Graphics;
using SFML.System;

namespace AStar;

public class FileMapGenerator
{ 
    public void Generate(Tilemap tilemap, string filename)
    {
        Image image = new(filename);

        for (int y = 0; y < image.Size.Y; y++)
        {
            for (int x = 0; x < image.Size.X; x++)
            {
                if (image.GetPixel((uint)x, (uint)y) == Color.Black)
                {
                    Tile? tile = tilemap.GetTileAt(new Vector2i(x, y));
                    if (tile != null) tile.IsWalkable = false;
                }
            }
        }
    }
}
