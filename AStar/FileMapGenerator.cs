using SFML.Graphics;
using SFML.System;

namespace AStar;

public sealed class FileMapGenerator
{ 
    public void Generate(Tilemap tilemap, string filename)
    {
        Image image = new(filename);

        for (int y = 0; y < image.Size.Y; y++)
        {
            for (int x = 0; x < image.Size.X; x++)
            {
                Color pixelColor = image.GetPixel((uint)x, (uint)y);
                if (pixelColor == Color.White)
                    continue;

                Tile? tile = tilemap.GetTileAt(new Vector2i(x, y));
                if (tile == null)
                    continue;

                if (pixelColor == Color.Black) tile.IsWalkable = false;
                if (pixelColor == Color.Red) tile.Cost = 7;
            }
        }
    }
}
