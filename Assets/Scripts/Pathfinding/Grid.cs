using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField] LayerMask unwalkableLayers;
    private static GridNode[,] grid;
    public List<GridNode> Path { get; set; }

    private void Start()
    {
        CreateGrid();
    }

    public void CreateGrid()
    {
        int width = GameController.Instance.Width;
        int height = GameController.Instance.Height;

        grid = new GridNode[width + 1, height + 1];
        Vector2 start = new Vector2(-width / 2, -height / 2);

        for (int x = 0; x < width + 1; x++)
        {
            for(int y = 0; y < height + 1; y++)
            {
                Vector2 worldPosition = new Vector2(start.x + x, start.y + y);
                bool walkable = Physics2D.Raycast(worldPosition, Vector2.zero, 1, unwalkableLayers).collider == null;
                grid[x, y] = new GridNode(walkable, worldPosition, x, y);
            }
        }
    }

    public List<GridNode> GetNeighbours(GridNode gridNode)
    {
        List<GridNode> neigbours = new List<GridNode>();

        for(int x = gridNode.GridX - 1; x <= gridNode.GridX + 1; x++)
        {
            for(int y = gridNode.GridY - 1; y <= gridNode.GridY + 1; y++)
            {
                if(y == gridNode.GridY || x == gridNode.GridX)
                {
                    if (IsInGrid(x, y))
                    {
                        neigbours.Add(grid[x, y]);
                    }
                }
            }
        }

        return neigbours;
    }

    public GridNode GetNeighbourAtDirection(int direction, GridNode gridNode)
    {
        switch (direction)
        {
            case PlayerInput.NORTH:
                Debug.Log("North Direction");
                return grid[gridNode.GridX, gridNode.GridY + 1];
            case PlayerInput.EAST:
                Debug.Log("East Direction");
                return grid[gridNode.GridX + 1, gridNode.GridY];
            case PlayerInput.SOUTH:
                Debug.Log("South Direction");
                return grid[gridNode.GridX, gridNode.GridY - 1];
            case PlayerInput.WEST:
                Debug.Log("West Direction");
                return grid[gridNode.GridX - 1, gridNode.GridY];
            default:
                return null;
        }
    }

    public GridNode GridNodeFromWorldPoint(Vector2 position)
    {
        int width = GameController.Instance.Width;
        int height = GameController.Instance.Height;

        Vector2 bottomLeft = new Vector2(-width / 2, -height / 2);

        int xDistance = (int)position.x - (int)bottomLeft.x;
        int yDistance = (int)position.y - (int)bottomLeft.y;

        if(IsInGrid(xDistance, yDistance))
        {
            return grid[xDistance, yDistance];
        }
        else
        {
            return null;
        }
    }

    private bool IsInGrid(int x, int y)
    {
        return x > 0 && x <= GameController.Instance.Width && y > 0 && y <= GameController.Instance.Height;
    }

    private void OnDrawGizmos()
    {
        if(grid != null)
        {
            foreach (GridNode node in grid)
            {
                Gizmos.color = node.IsWalkable ? Color.white : Color.red;

                if(Path != null)
                {
                    if (Path.Contains(node))
                    {
                        Gizmos.color = Color.black;
                    }
                }

                Gizmos.DrawCube(node.WorldPosition, Vector3.one);
            }
        }
    }
}