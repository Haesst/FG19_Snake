using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Grid))]
public class Pathfinding : MonoBehaviour
{
    private Transform seeker;
    private Transform target;
    private Grid grid;

    [SerializeField]private string foodTag = "Food";

    public Transform Seeker { set => seeker = value; }
    public Transform Target { get => target; set => target = value; }
    public List<GridNode> Path { get => grid?.Path; }

    private void Awake()
    {
        grid = GetComponent<Grid>();
        Assert.IsNotNull(grid, "Grid component could not be found");
    }

    private void Update()
    {
        if (target != null)
        {
            FindPath(seeker.position, target.position);
        }
    }

    public void UpdateGrid()
    {
        grid.UpdateGrid();
    }

    void FindPath(Vector2 startPos, Vector2 targetPos)
    {
        GridNode startNode = grid.GridNodeFromWorldPoint(startPos);
        GridNode targetNode = grid.GridNodeFromWorldPoint(targetPos);

        List<GridNode> openList = new List<GridNode>();
        HashSet<GridNode> closedList = new HashSet<GridNode>();
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            GridNode node = openList[0];
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

            if(node == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }

            foreach (GridNode neighbour in grid.GetNeighbours(node))
            {
                if(!neighbour.IsWalkable || closedList.Contains(neighbour))
                {
                    continue;
                }

                int newCostToNeighbour = node.GCost + GetDistance(node, neighbour);
                if(newCostToNeighbour < neighbour.GCost || !openList.Contains(neighbour))
                {
                    neighbour.GCost = newCostToNeighbour;
                    neighbour.HCost = GetDistance(neighbour, targetNode);
                    neighbour.Parent = node;

                    if (!openList.Contains(neighbour))
                    {
                        openList.Add(neighbour);
                    }
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

    int GetDistance(GridNode nodeA, GridNode nodeB)
    {
        int distanceX = Mathf.Abs(nodeA.GridX - nodeB.GridX);
        int distanceY = Mathf.Abs(nodeA.GridY - nodeB.GridY);

        return distanceX + distanceY;
    }
}