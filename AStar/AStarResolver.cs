using SFML.System;

namespace AStar;

public sealed class AStarResolver
{
    private readonly Tilemap _map;

    public IReadOnlyDictionary<Vector2i, AStarNode> OpenList => _openList;
    private readonly Dictionary<Vector2i, AStarNode> _openList = [];
    public IReadOnlyDictionary<Vector2i, AStarNode> ClosedList => _closedList;
    private readonly Dictionary<Vector2i, AStarNode> _closedList = [];

    public IReadOnlyList<AStarNode>? SolvedPath;
    private List<AStarNode>? _solvedPath;

    public Vector2i Origin { get; }
    public Vector2i Destination { get; }

    public AStarNode LastChecked { get; private set; }

    public AStarResolver(Tilemap map, Vector2i origin, Vector2i destination)
    {
        _map = map;
        Origin = origin;
        Destination = destination;

        AStarNode startingNode = new()
        {
            Position = origin,
            GCost = 0,
            HCost = GetHeuristicCost(origin),
            ParentPosition = null
        };
        _openList.Add(origin, startingNode);

        LastChecked = startingNode;
    }

    private float GetHeuristicCost(Vector2i position) =>
        MathF.Sqrt((position.X - Destination.X) * (position.X - Destination.X)
        + (position.Y - Destination.Y) * (position.Y - Destination.Y));

    public List<AStarNode> GetNeighbors(AStarNode parent)
    {
        List<AStarNode> result = [];

        Tile? up = _map.GetTileAt(parent.Position + new Vector2i(0, -1));
        Tile? upLeft = _map.GetTileAt(parent.Position + new Vector2i(-1, -1));
        Tile? upRight = _map.GetTileAt(parent.Position + new Vector2i(1, -1));
        Tile? down = _map.GetTileAt(parent.Position + new Vector2i(0, 1));
        Tile? downLeft = _map.GetTileAt(parent.Position + new Vector2i(-1, 1));
        Tile? downRight = _map.GetTileAt(parent.Position + new Vector2i(1, 1));
        Tile? left = _map.GetTileAt(parent.Position + new Vector2i(-1, 0));
        Tile? right = _map.GetTileAt(parent.Position + new Vector2i(1, 0));

        if (up is { IsWalkable: true })
        {
            result.Add(new()
            {
                Position = up.Position,
                GCost = parent.GCost + up.Cost,
                HCost = GetHeuristicCost(up.Position),
                ParentPosition = parent.Position
            });
        }
        if (upLeft is { IsWalkable: true })
        {
            result.Add(new()
            {
                Position = upLeft.Position,
                GCost = parent.GCost + upLeft.Cost * 1.41f,
                HCost = GetHeuristicCost(upLeft.Position),
                ParentPosition = parent.Position
            });
        }
        if (upRight is { IsWalkable: true })
        {
            result.Add(new()
            {
                Position = upRight.Position,
                GCost = parent.GCost + upRight.Cost * 1.41f,
                HCost = GetHeuristicCost(upRight.Position),
                ParentPosition = parent.Position
            });
        }
        if (down is { IsWalkable: true })
        {
            result.Add(new()
            {
                Position = down.Position,
                GCost = parent.GCost + down.Cost,
                HCost = GetHeuristicCost(down.Position),
                ParentPosition = parent.Position
            });
        }
        if (downRight is { IsWalkable: true })
        {
            result.Add(new()
            {
                Position = downRight.Position,
                GCost = parent.GCost + downRight.Cost * 1.41f,
                HCost = GetHeuristicCost(downRight.Position),
                ParentPosition = parent.Position
            });
        }
        if (downLeft is { IsWalkable: true })
        {
            result.Add(new()
            {
                Position = downLeft.Position,
                GCost = parent.GCost + downLeft.Cost * 1.41f,
                HCost = GetHeuristicCost(downLeft.Position),
                ParentPosition = parent.Position
            });
        }
        if (left is { IsWalkable: true })
        {
            result.Add(new()
            {
                Position = left.Position,
                GCost = parent.GCost + left.Cost,
                HCost = GetHeuristicCost(left.Position),
                ParentPosition = parent.Position
            });
        }
        if (right is { IsWalkable: true })
        {
            result.Add(new()
            {
                Position = right.Position,
                GCost = parent.GCost + right.Cost,
                HCost = GetHeuristicCost(right.Position),
                ParentPosition = parent.Position
            });
        }

        return result;
    }

    public void Tick()
    {
        if (_solvedPath != null || _openList.Count == 0)
            return;

        AStarNode cheapestNode = _openList.MinBy(ol => ol.Value.FCost).Value;
        LastChecked = cheapestNode;

        if (cheapestNode.Position == Destination)
        {
            _solvedPath = [];
            AStarNode? curr = cheapestNode;
            while (curr != null)
            {
                _solvedPath.Insert(0, curr);
                curr = curr.ParentPosition.HasValue ? _closedList[curr.ParentPosition.Value] : null;
            }
            return;
        }

        List<AStarNode> neighbors = GetNeighbors(cheapestNode);
        foreach (AStarNode newNode in neighbors)
        {
            if (!_closedList.ContainsKey(newNode.Position) && (!_openList.TryGetValue(newNode.Position, out AStarNode? existingNode) ||
                existingNode.GCost > newNode.GCost))
            {
                _openList[newNode.Position] = newNode;
            }
        }

        _openList.Remove(cheapestNode.Position);
        _closedList.Add(cheapestNode.Position, cheapestNode);
    }
}
