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

    /// <summary>
    /// Create a new grid and see which tiles that are walkable
    /// </summary>
    public void CreateGrid()
    {
        int width = GameController.Instance.Width;
        int height = GameController.Instance.Height;

        // Create a new GridNode two dimensional array 
        grid = new GridNode[width + 1, height + 1];
        // Get the bottom left position while keeping the middle of the
        // grid at 0,0,0 in the world position
        Vector2 start = new Vector2(-width / 2, -height / 2);

        // Loop through every position in the grid.
        for (int x = 0; x < width + 1; x++)
        {
            for(int y = 0; y < height + 1; y++)
            {
                // Get the world position
                Vector2 worldPosition = new Vector2(start.x + x, start.y + y);

                // See if we collide with anything, because if we do then it's not walkable
                bool walkable = Physics2D.Raycast(worldPosition, Vector2.zero, 1, unwalkableLayers).collider == null;

                // Create a new gridnode passing in walkable, worldposition, grid x and grid y
                grid[x, y] = new GridNode(walkable, worldPosition, x, y);
            }
        }
    }

    /// <summary>
    /// Get a list of neighbour tiles to the given tile
    /// </summary>
    /// <param name="gridNode">Tile to get neighbours to.</param>
    /// <returns>A list of GridNodes that are direct neighbours to the given tile.</returns>
    public List<GridNode> GetNeighbours(GridNode gridNode)
    {
        // Create a new list of GridNodes
        List<GridNode> neigbours = new List<GridNode>();

        //Loop through the tiles next to the given grid.
        for(int x = gridNode.GridX - 1; x <= gridNode.GridX + 1; x++)
        {
            for(int y = gridNode.GridY - 1; y <= gridNode.GridY + 1; y++)
            {
                // We only want to check the neighbours right next to the
                // current tile and not diagonally. We can do that by making
                // sure that we're at either the same x or the same y.
                if(y == gridNode.GridY || x == gridNode.GridX)
                {
                    // Make sure that the tile is inside the grid.
                    if (IsInGrid(x, y))
                    {
                        // Add it to the neighbour list
                        neigbours.Add(grid[x, y]);
                    }
                }
            }
        }

        return neigbours;
    }

    /// <summary>
    /// Get the neighbour to the direction given.
    /// </summary>
    /// <param name="direction">Direction to check.</param>
    /// <param name="gridNode">Gridnode that we check from.</param>
    /// <returns>The neighbour to the direction or null if there's none.</returns>
    public GridNode GetNeighbourAtDirection(int direction, GridNode gridNode)
    {
        switch (direction)
        {
            case PlayerInput.NORTH:
                return grid[gridNode.GridX, gridNode.GridY + 1];
            case PlayerInput.EAST:
                return grid[gridNode.GridX + 1, gridNode.GridY];
            case PlayerInput.SOUTH:
                return grid[gridNode.GridX, gridNode.GridY - 1];
            case PlayerInput.WEST:
                return grid[gridNode.GridX - 1, gridNode.GridY];
            default:
                return null;
        }
    }

    /// <summary>
    /// Get the gridnode that's at the given world point position
    /// </summary>
    /// <param name="position">Worldpoint</param>
    /// <returns>The GridNode that's at the given position or null.</returns>
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

    /// <summary>
    /// A check to make sure that a x,y position is in the grid.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns>true if it's in the grid, false if it's not.</returns>
    private bool IsInGrid(int x, int y)
    {
        return x > 0 && x <= GameController.Instance.Width && y > 0 && y <= GameController.Instance.Height;
    }

    /// <summary>
    /// Draw out the path in UnityEditor
    /// </summary>
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