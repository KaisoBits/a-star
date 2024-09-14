using SFML.System;

namespace AStar;

public class AStarNode
{
    public required Vector2i Position { get; init; }
    public required float GCost { get; init; }
    public required float HCost { get; init; }
    public required Vector2i? ParentPosition { get; init; }

    public float FCost => GCost + HCost;
};
