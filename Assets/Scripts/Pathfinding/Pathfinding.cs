using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Grid))]
public class Pathfinding : MonoBehaviour
{
    private Grid grid;
    public List<GridNode> Path { get => grid?.Path; }

    private void Awake()
    {
        grid = GetComponent<Grid>();
        Assert.IsNotNull(grid, "Grid component could not be found.");
    }

    public void UpdateGrid()
    {
        grid.CreateGrid();
    }

    public Vector2 GridNodeToWorldPosition(GridNode position)
    {
        int height = GameController.Instance.Height;
        int width = GameController.Instance.Width;

        return new Vector2((-width / 2 + position.GridX), (-height / 2 + position.GridY));
    }
    public GridNode GetNeighbourInDirection(int direction, GridNode gridNode)
    {
        return grid.GetNeighbourAtDirection(direction, gridNode);
    }

    public GridNode GridNodeFromWorldPoint(Vector2 position)
    {
        return grid.GridNodeFromWorldPoint(position);
    }

    public void FindPath(Vector2 startPos, Vector2 targetPos)
    {
        grid?.Path?.Clear();

        GridNode startNode = grid.GridNodeFromWorldPoint(startPos);
        GridNode targetNode = grid.GridNodeFromWorldPoint(targetPos);

        List<GridNode> openList = new List<GridNode>();
        HashSet<GridNode> closedList = new HashSet<GridNode>();
        openList.Add(startNode);

        while(openList.Count > 0)
        {
            GridNode node = openList[0];

            // Look for a node that have a lower or the same fcost with a lower hcost
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

            openList.Remove(node);
            closedList.Add(node);

            // Check if we reached the target yet
            if(node == targetNode)
            {
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
                // we want to update the tile and add it to the list if it's not in there

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

    void RetracePath(GridNode startNode, GridNode endNode)
    {
        List<GridNode> path = new List<GridNode>();
        GridNode currentNode = endNode;

        while(currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }

        path.Reverse();

        grid.Path = path;
    }

    int GetManhattanDistance(GridNode nodeA, GridNode nodeB)
    {
        int distanceX = Mathf.Abs(nodeA.GridX - nodeB.GridX);
        int distanceY = Mathf.Abs(nodeA.GridY - nodeB.GridY);

        return distanceX + distanceY;
    }
}