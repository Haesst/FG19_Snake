using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Grid))]
public class Pathfinding : MonoBehaviour
{
    private Grid grid;
    public List<GridNode> Path { get => grid?.Path; }

    // Make sure we have a grid.
    private void Awake()
    {
        grid = GetComponent<Grid>();
        Assert.IsNotNull(grid, "Grid component could not be found.");
    }

    /// <summary>
    /// Update the current grid.
    /// </summary>
    public void UpdateGrid()
    {
        grid.CreateGrid();
    }

    /// <summary>
    /// Get the world position from a GridNode.
    /// </summary>
    /// <param name="position">The GridNode to convert to worldposition.</param>
    /// <returns>The worldposition in a Vector2-format.</returns>
    public Vector2 GridNodeToWorldPosition(GridNode position)
    {
        int height = GameController.Instance.Height;
        int width = GameController.Instance.Width;

        return new Vector2((-width / 2 + position.GridX), (-height / 2 + position.GridY));
    }

    /// <summary>
    /// A method that calls the grids method with the same name.
    /// It's just a way for me to have less variables.
    /// </summary>
    /// <param name="direction">Direction to check for a neighbour.</param>
    /// <param name="gridNode">The gridnode from which we want to get the neighbour.</param>
    /// <returns></returns>
    public GridNode GetNeighbourInDirection(int direction, GridNode gridNode)
    {
        return grid.GetNeighbourAtDirection(direction, gridNode);
    }

    /// <summary>
    /// A method that calls the grids method with the same name.
    /// It's just a way for me to have less variables.
    /// </summary>
    /// <param name="position">Worldpoint that we want to convert to a gridnode.</param>
    /// <returns>A GridNode or null if there's none at the given position.</returns>
    public GridNode GridNodeFromWorldPoint(Vector2 position)
    {
        return grid.GridNodeFromWorldPoint(position);
    }

    /// <summary>
    /// Find the shortest part between two Vector2 positions
    /// </summary>
    /// <param name="startPos">Startposition</param>
    /// <param name="targetPos">Targetposition</param>
    public void FindPath(Vector2 startPos, Vector2 targetPos)
    {
        // Clear the current path
        grid?.Path?.Clear();

        // Get both startpositions and targetpositions nodes
        GridNode startNode = grid.GridNodeFromWorldPoint(startPos);
        GridNode targetNode = grid.GridNodeFromWorldPoint(targetPos);

        // Create an open list of GridNodes for tiles to check.
        List<GridNode> openList = new List<GridNode>();

        // Create a HashSet of GridNodes for tiles that's locked in.
        HashSet<GridNode> closedList = new HashSet<GridNode>();

        // Add the startnode to the open list.
        openList.Add(startNode);

        // Run as long as we have tiles in the open list.
        while(openList.Count > 0)
        {
            // Get the node at the first position in the openList.
            GridNode node = openList[0];

            // Look for a node that have a lower or the same fcost with a lower hcost
            // in the open list, because if there's one then that tile is more interesting.
            for(int i = 1; i < openList.Count; i++)
            {
                if(openList[i].FCost < node.FCost || openList[i].FCost == node.FCost)
                {
                    if(openList[i].HCost < node.HCost)
                    {
                        node = openList[i];
                    }
                }
            }

            // Remove the node from the openList and add it to the closedList.
            openList.Remove(node);
            closedList.Add(node);

            // Check if we reached the target yet
            if(node == targetNode)
            {
                // Call RetracePath with the startNode and targetNode.
                RetracePath(startNode, targetNode);
                return;
            }

            // Loop through the current nodes neighbours
            foreach(GridNode neighbour in grid.GetNeighbours(node))
            {
                // Make sure that it's walkable and that we haven't already closed it
                if(!neighbour.IsWalkable || closedList.Contains(neighbour))
                {
                    continue;
                }

                // Calculate the cost to the neighbour when walking through this tile
                int newCostToNeighbour = node.GCost + GetManhattanDistance(node, neighbour);

                // If the node doesn't exist in the openlist or if we have new lower gcost
                // we want to update the tile with this node as the parent
                // and add it to the list if it's not in there

                neighbour.GCost = newCostToNeighbour;
                neighbour.HCost = GetManhattanDistance(neighbour, targetNode);
                neighbour.Parent = node;

                if (!openList.Contains(neighbour))
                {
                    openList.Add(neighbour);
                }
            }
        }
    }

    /// <summary>
    /// RetracePath adds every node to the grids path.
    /// </summary>
    /// <param name="startNode">Startnode</param>
    /// <param name="endNode">Endnode</param>
    void RetracePath(GridNode startNode, GridNode endNode)
    {
        List<GridNode> path = new List<GridNode>();
        GridNode currentNode = endNode;

        while(currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }

        // Reverse the list so it's not reversed.
        path.Reverse();

        grid.Path = path;
    }

    /// <summary>
    /// Calculate the manhattan distance between two points
    /// in the Grid.
    /// </summary>
    /// <param name="nodeA"></param>
    /// <param name="nodeB"></param>
    /// <returns>The manhattan distance.</returns>
    int GetManhattanDistance(GridNode nodeA, GridNode nodeB)
    {
        int distanceX = Mathf.Abs(nodeA.GridX - nodeB.GridX);
        int distanceY = Mathf.Abs(nodeA.GridY - nodeB.GridY);

        return distanceX + distanceY;
    }
}