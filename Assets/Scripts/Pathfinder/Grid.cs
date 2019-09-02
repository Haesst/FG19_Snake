using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Grid : MonoBehaviour
{
    [SerializeField] LayerMask unwalkableMask;

    private GridNode[,] grid;
    private List<GridNode> path;
    private List<Vector2> wallList = new List<Vector2>();

    private int currentTick = 0;

    public List<GridNode> Path { get => path; set => path = value; }

    private void Awake()
    {
        CreateGrid();
    }

    private void CreateGrid()
    {
        grid = new GridNode[GameController.Instance.GetFieldWidth + 1, GameController.Instance.GetFieldHeight + 1];
        for(int x = 0; x < GameController.Instance.GetFieldWidth + 1; x++)
        {
            for (int y = 0; y < GameController.Instance.GetFieldHeight + 1; y++)
            {
                Vector2 start = GameController.Instance.GetFieldBottomLeft;
                Vector2 worldPosition = new Vector2(start.x + x, start.y + y);
                bool walkable = Physics2D.Raycast(worldPosition, Vector2.zero, 1, unwalkableMask).collider == null;
                grid[x, y] = new GridNode(walkable, worldPosition, x, y);
                if (!walkable)
                {
                    wallList.Add(worldPosition);
                }
            }
        }
    }

    private void Update()
    {
        UpdateGrid();
    }

    public void UpdateGrid()
    {
        for (int x = 0; x < grid.GetUpperBound(0); x++)
        {
            for (int y = 0; y < grid.GetUpperBound(1); y++)
            {
                bool walkable = Physics2D.Raycast(grid[x, y].WorldPosition, Vector2.zero, 1, unwalkableMask).collider == null;
                grid[x, y].IsWalkable = walkable;
                //if (snakeList.Contains(grid[x, y].WorldPosition) || wallList.Contains(grid[x,y].WorldPosition))
                //{
                //    grid[x, y].IsWalkable = false;
                //}
            }
        }
    }

    public List<GridNode> GetNeighbours(GridNode gridNode)
    {
        //List<GridNode> neighbours = new List<GridNode>()
        return new List<GridNode>()
        {
            grid[gridNode.GridX - 1, gridNode.GridY],
            grid[gridNode.GridX + 1, gridNode.GridY],
            grid[gridNode.GridX, gridNode.GridY - 1],
            grid[gridNode.GridX, gridNode.GridY + 1]
        };

        /*
        for(int x = -1; x <= 1; x++)
        {
            for(int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }

                int checkX = gridNode.GridX + x;
                int checkY = gridNode.GridY + y;

                if (checkX >= 0 && checkX < gameController.GetFieldWidth && checkY >= 0 && checkY < gameController.GetFieldHeight)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
        */
    }

    public GridNode GridNodeFromWorldPoint(Vector2 position)
    {
        // first get x. We know that bottom left is at 0,0
        Vector2 bottomLeft = GameController.Instance.GetFieldBottomLeft;

        // Get the distance between bottom and desired position
        int xDistance = (int)position.x - (int)bottomLeft.x;

        // Get the distance between bottom left and desired position on the y axis
        int yDistance = (int)position.y - (int)bottomLeft.y;

        //Make sure that it's a position in the grid
        if(xDistance < GameController.Instance.GetFieldWidth && xDistance >= 0 && yDistance < GameController.Instance.GetFieldHeight && yDistance >= 0)
        {
            return grid[xDistance, yDistance];
        }
        else
        {
            return null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(GameController.Instance.GetFieldWidth, GameController.Instance.GetFieldHeight, 1));

        if(grid != null)
        {
            foreach(GridNode n in grid)
            {
                Gizmos.color = n.IsWalkable ? Color.white : Color.red;
                if(path != null)
                {
                    if (path.Contains(n))
                    {
                        Gizmos.color = Color.black;
                    }
                }
                Gizmos.DrawCube(n.WorldPosition, Vector3.one);
            }
        }
    }
}