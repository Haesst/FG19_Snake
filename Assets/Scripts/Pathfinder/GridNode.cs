using UnityEngine;

public class GridNode
{
    private bool isWalkable;
    private Vector2 worldPosition;
    private int gridX;
    private int gridY;

    private int gCost;
    private int hCost;
    private GridNode parent;

    public bool IsWalkable { get => isWalkable; set => isWalkable = value; }
    public Vector2 WorldPosition { get => worldPosition; }
    public int GridX { get => gridX; }
    public int GridY { get => gridY; }

    public int GCost { get => gCost; set => gCost = value; }
    public int HCost { get => hCost; set => hCost = value; }
    public int FCost { get => gCost + hCost; }
    public GridNode Parent { get => parent; set => parent = value; }

    public GridNode(bool isWalkable, Vector2 worldPosition, int gridX, int gridY)
    {
        this.isWalkable = isWalkable;
        this.worldPosition = worldPosition;
        this.gridX = gridX;
        this.gridY = gridY;
    }
}