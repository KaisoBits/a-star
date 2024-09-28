using SFML.Graphics;
using SFML.System;

namespace AStar;

public class Renderer : Drawable
{
    private readonly Tilemap _tilemap;
    public AStarResolver Resolver { get; set; }

    private readonly VertexArray _vertexArray;
    private readonly Vector2i _textureTileSize;

    private readonly Texture _tileTexture;
    private readonly Vector2i _atlasSize;

    private readonly Vector2f[] _walkableTextures = [new(1, 0), new(2, 0), new(3, 0), new(4, 0)];
    private readonly Vector2f[] _nonWalkableTextures = [new(0, 1), new(1, 1), new(2, 1), new(3, 1), new(4, 1), new(5, 1), new(3, 2), new(4, 2)];
    private readonly Vector2f[] _obstacleTextures = [new(2, 15)];

    private AStarNode? _previousLastChecked = null;
    private readonly VertexArray _pathVertices = new(PrimitiveType.LineStrip);

    private readonly Clock _clock = new();

    public Renderer(Tilemap tilemap, AStarResolver resolver, Texture tileTexture, Vector2i atlasSize)
    {
        _tileTexture = tileTexture;
        _tilemap = tilemap;
        Resolver = resolver;

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

        if (!_previousLastChecked.HasValue || _previousLastChecked.Value.Position != Resolver.LastChecked.Position)
        {
            _previousLastChecked = Resolver.LastChecked;
            _pathVertices.Clear();

            AStarNode? curr = Resolver.LastChecked;
            while (curr != null)
            {
                Vector2f position = new Vector2f(
                    curr.Value.Position.X * _textureTileSize.X + _textureTileSize.X / 2.0f, 
                    curr.Value.Position.Y * _textureTileSize.Y + _textureTileSize.Y / 2.0f) - (fullSize / 2.0f);
                _pathVertices.Append(new Vertex(position, Color.Magenta));
                curr = curr.Value.ParentPosition.HasValue ? Resolver.ClosedList[curr.Value.ParentPosition.Value] : null;
            }
        }

        target.Draw(_pathVertices, states);
    }

    public void DrawTile(Tile tile, RenderTarget target, RenderStates states)
    {
        int posHash = Math.Abs(HashCode.Combine(tile.Position.X, tile.Position.Y));

        Vector2f textureCoord = tile switch
        {
            { IsWalkable: true, Cost: <= 1 } => _walkableTextures[posHash % _walkableTextures.Length],
            { IsWalkable: true, Cost: > 1 } => _obstacleTextures[posHash % _obstacleTextures.Length],
            { IsWalkable: false } => _nonWalkableTextures[posHash % _nonWalkableTextures.Length],
        };

        if (tile.Position == Resolver.Origin)
            textureCoord = new(27, 0);
        else if (tile.Position == Resolver.Destination)
            textureCoord = new(41, 4);

        textureCoord = new Vector2f(textureCoord.X * _textureTileSize.X, textureCoord.Y * _textureTileSize.Y);

        Color overlayColor = Color.White;
        if (Resolver.OpenList.ContainsKey(tile.Position))
            overlayColor = new Color(180, 180, 180);
        else if (Resolver.ClosedList.ContainsKey(tile.Position))
            overlayColor = new Color(100, 100, 100);

        _vertexArray[0] = new Vertex(new Vector2f(_textureTileSize.X, 0), overlayColor, textureCoord + new Vector2f(_textureTileSize.X, 0));
        _vertexArray[1] = new Vertex(new Vector2f(0, 0), overlayColor, textureCoord + new Vector2f(0, 0));
        _vertexArray[2] = new Vertex(new Vector2f(0, _textureTileSize.Y), overlayColor, textureCoord + new Vector2f(0, _textureTileSize.Y));
        _vertexArray[3] = new Vertex(new Vector2f(_textureTileSize.X, _textureTileSize.Y), overlayColor, textureCoord + new Vector2f(_textureTileSize.X, _textureTileSize.Y));
        states.Texture = _tileTexture;

        target.Draw(_vertexArray, states);
    }

    public Tile? GetTileOnCoords(Vector2f coords)
    {
        Vector2f fullSize = new Vector2f(_textureTileSize.X * _tilemap.GridSize.X, _textureTileSize.Y * _tilemap.GridSize.Y);

        Vector2f transformedCoords = coords + (fullSize / 2.0f);
        Vector2i tileCoord = new Vector2i((int)transformedCoords.X / _textureTileSize.X, (int)transformedCoords.Y / _textureTileSize.Y);

        return _tilemap.GetTileAt(tileCoord);
    }
}
