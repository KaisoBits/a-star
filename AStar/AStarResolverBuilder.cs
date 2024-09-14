using SFML.System;

namespace AStar;

public sealed class AStarResolverBuilder
{
    private Vector2i? _origin;
    private Vector2i? _destination;

    private AStarResolverBuilder() { }

    public static AStarResolverBuilder Create()
    {
        return new();
    }

    public AStarResolverBuilder WithOrigin(Vector2i origin)
    {
        _origin = origin;
        return this;
    }

    public AStarResolverBuilder WithDestination(Vector2i destination)
    {
        _destination = destination;
        return this;
    }

    public AStarResolver Build(Tilemap tilemap)
    {
        if (!_origin.HasValue)
            throw new Exception("Set the origin position before building the resolver");
        if (!_destination.HasValue)
            throw new Exception("Set the destination position before building the resolver");

        return new AStarResolver(tilemap, _origin.Value, _destination.Value);
    }
}
