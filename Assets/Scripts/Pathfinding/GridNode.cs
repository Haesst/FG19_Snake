using UnityEngine;

public class GridNode
{
    public bool IsWalkable { get; set; }
    public Vector2 WorldPosition { get; private set; }
    public int GridX { get; private set; }
    public int GridY { get; private set; }
    
    public int GCost { get; set; }
    public int HCost { get; set; }
    public int FCost { get => GCost + HCost; }
    public GridNode Parent { get; set; }

    public GridNode(bool isWalkable, Vector2 worldPosition, int gridX, int gridY)
    {
        IsWalkable = isWalkable;
        WorldPosition = worldPosition;
        GridX = gridX;
        GridY = gridY;
    }
}