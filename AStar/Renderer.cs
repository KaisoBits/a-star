using SFML.Graphics;
using SFML.System;

namespace AStar;

public class Renderer : Drawable
{
    private readonly Tilemap _tilemap;
    private readonly AStarResolver _resolver;

    private readonly VertexArray _vertexArray;
    private readonly Vector2i _textureTileSize;

    private readonly Texture _tileTexture;
    private readonly Vector2i _atlasSize;

    private readonly Vector2f[] _walkableTextures = [new(1, 0), new(2, 0), new(3, 0), new(4, 0)];
    private readonly Vector2f[] _nonWalkableTextures = [new(0, 1), new(1, 1), new(2, 1), new(3, 1), new(4, 1), new(5, 1), new(3, 2), new(4, 2)];

    private AStarNode? _previousLastChecked = null;
    private readonly VertexArray _pathVertices = new(PrimitiveType.LineStrip);

    public Renderer(Tilemap tilemap, AStarResolver resolver, Texture tileTexture, Vector2i atlasSize)
    {
        _tileTexture = tileTexture;
        _tilemap = tilemap;
        _resolver = resolver;

        _vertexArray = new VertexArray(PrimitiveType.Quads, 4);
        _textureTileSize = new Vector2i((int)_tileTexture.Size.X / atlasSize.X, (int)_tileTexture.Size.Y / atlasSize.Y);
    }

    public void Draw(RenderTarget target, RenderStates states)
    {
        Vector2f fullSize = new Vector2f(_textureTileSize.X * _tilemap.GridSize.X, _textureTileSize.Y * _tilemap.GridSize.Y);

        for (int x = 0; x < _tilemap.GridSize.X; x++)
        {
            for (int y = 0; y < _tilemap.GridSize.Y; y++)
            {
                Tile tile = _tilemap.GetTileAt(new Vector2i(x, y))
                    ?? throw new Exception("This tile access should never go out of bounds");

                RenderStates rs = states;
                rs.Transform.Translate(new Vector2f(x * _textureTileSize.X, y * _textureTileSize.Y) - (fullSize / 2.0f));

                DrawTile(tile, target, rs);
            }
        }

        if (_previousLastChecked != _resolver.LastChecked)
        {
            _previousLastChecked = _resolver.LastChecked;
            _pathVertices.Clear();

            AStarNode? curr = _resolver.LastChecked;
            while (curr != null)
            {
                Vector2f position = new Vector2f(curr.Position.X * _textureTileSize.X + _textureTileSize.X / 2.0f, curr.Position.Y * _textureTileSize.Y + _textureTileSize.Y / 2.0f) - (fullSize / 2.0f);
                _pathVertices.Append(new Vertex(position, Color.Magenta));
                curr = curr.ParentPosition.HasValue ? _resolver.ClosedList[curr.ParentPosition.Value] : null;
            }
        }

        target.Draw(_pathVertices, states);
    }

    public void DrawTile(Tile tile, RenderTarget target, RenderStates states)
    {
        int posHash = Math.Abs(HashCode.Combine(tile.Position.X, tile.Position.Y));
        Vector2f textureCoord = tile.IsWalkable ?
            _walkableTextures[posHash % _walkableTextures.Length] :
            _nonWalkableTextures[posHash % _nonWalkableTextures.Length];

        textureCoord = new Vector2f(textureCoord.X * _textureTileSize.X, textureCoord.Y * _textureTileSize.Y);

        Color overlayColor = Color.White;
        if (_resolver.Origin == tile.Position)
            overlayColor = new Color(255, 150, 150);
        else if (_resolver.Destination == tile.Position)
            overlayColor = new Color(100, 100, 255);
        else if (_resolver.OpenList.ContainsKey(tile.Position))
            overlayColor = new Color(200, 200, 200);
        else if (_resolver.ClosedList.ContainsKey(tile.Position))
            overlayColor = new Color(100, 100, 100);

        _vertexArray[0] = new Vertex(new Vector2f(_textureTileSize.X, 0), overlayColor, textureCoord + new Vector2f(_textureTileSize.X, 0));
        _vertexArray[1] = new Vertex(new Vector2f(0, 0), overlayColor, textureCoord + new Vector2f(0, 0));
        _vertexArray[2] = new Vertex(new Vector2f(0, _textureTileSize.Y), overlayColor, textureCoord + new Vector2f(0, _textureTileSize.Y));
        _vertexArray[3] = new Vertex(new Vector2f(_textureTileSize.X, _textureTileSize.Y), overlayColor, textureCoord + new Vector2f(_textureTileSize.X, _textureTileSize.Y));
        states.Texture = _tileTexture;

        target.Draw(_vertexArray, states);
    }
}
