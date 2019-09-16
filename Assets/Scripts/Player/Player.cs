using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Pathfinding))]
[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    [SerializeField] GameObject snakeHeadPrefab;
    [SerializeField] GameObject snakeBodyPrefab;

    PlayerInput playerInput;
    LinkedList snakeBodyParts;
    Vector3 lastHeadPosition;
    Pathfinding pathfinder;

    [ShowOnly, SerializeField]private int lastTick = 0;
    public int GrowAmount { get; set; } = 0;

    #region UnityMethods
    /// <summary>
    /// Make sure that the scripts needed is assigned in the 
    /// editor. Then create a snakehead and add it to a new
    /// LinkedList.
    /// </summary>
    private void Awake()
    {

        playerInput = GetComponent<PlayerInput>();
        Assert.IsNotNull(playerInput, "Could not find PlayerInput script on GameObject!");

        pathfinder = GetComponent<Pathfinding>();
        Assert.IsNotNull(pathfinder, "Could not find Pathfinding script on GameObject!");

        GameObject go = GameObject.Instantiate(snakeHeadPrefab, transform);
        snakeBodyParts = new LinkedList();
        snakeBodyParts.InsertFirst(go);
    }

    /// <summary>
    /// Add the startgrowth specified in the gamecontroller to
    /// the snakes grow amount.
    /// </summary>
    private void Start()
    {
        GrowAmount = GameController.Instance.StartGrowth;
    }

    private void Update()
    {
        // If we don't have a path at the moment we want 
        // to recalculate the path for the snake to take.
        if (pathfinder.Path == null && gameObject != null && GameController.Instance.GetTopFood != null)
        {
            pathfinder.FindPath(snakeBodyParts.GetFirst().Value.transform.position, GameController.Instance.GetTopFood.transform.position);
        }

        // Check if the tick has been updated. 
        if (lastTick < GameController.Instance.CurrentTick)
        {
            // Update the grid in the pathfinder and then recalculate the path.
            pathfinder.UpdateGrid();
            pathfinder.FindPath(lastHeadPosition, GameController.Instance.GetTopFood.transform.position);

            // If AI is set to true then call calculate next move
            if (GameManager.Instance.AI)
            {
                CalculateNextMove();
            }
            // Update head position
            UpdateHead();
            // Update body position
            UpdateBody();

            // update lastHeadPosition to current position.
            lastHeadPosition = snakeBodyParts.GetFirst().Value.transform.position;
            lastTick++;
        }
    }
    #endregion UnityMethods

    /// <summary>
    /// Update the heads position depending on the current
    /// direction set in PlayerInput.
    /// </summary>
    private void UpdateHead()
    {
        // The head is always the first in the list
        Transform snakeHead = snakeBodyParts.GetFirst().Value.transform;

        // Get the players desired direction and
        // Update head's position and rotation
        // accordingly.
        switch (playerInput.Direction)
        {
            case (PlayerInput.NORTH):
                snakeHead.position += new Vector3(0, 1, 0);
                snakeHead.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case (PlayerInput.EAST):
                snakeHead.position += new Vector3(1, 0, 0);
                snakeHead.rotation = Quaternion.Euler(0, 0, -90);
                break;
            case (PlayerInput.SOUTH):
                snakeHead.position += new Vector3(0, -1, 0);
                snakeHead.rotation = Quaternion.Euler(0, 0, 180);
                break;
            case (PlayerInput.WEST):
                snakeHead.position += new Vector3(-1, 0, 0);
                snakeHead.rotation = Quaternion.Euler(0, 0, 90);
                break;
        }

        // Update the last direction (To make sure we can't go from right directly to left and so on)
        playerInput.LastDirection = playerInput.Direction;
    }


    private void CalculateNextMove()
    {
        // Head is always the first in the list
        Vector2 headPosition = snakeBodyParts.GetFirst().Value.transform.position;

        // Make sure we have a path
        if (pathfinder.Path.Count > 0)
        {
            // Get the next "tiles" position in the path.
            Vector2 nextMove = pathfinder.GridNodeToWorldPosition(pathfinder.Path[0]);

            // See if we want to go on the x or y axis
            if (nextMove.x == headPosition.x)
            {
                // Change y to up or down depending on the next tiles position
                playerInput.SetAIMovement(nextMove.y > headPosition.y ? PlayerInput.NORTH : PlayerInput.SOUTH);
            }
            else
            {
                // Change x to right or left depending on the next tiles position
                playerInput.SetAIMovement(nextMove.x > headPosition.x ? PlayerInput.EAST : PlayerInput.WEST);
            }
        }
        else
        {
            // We don't have a path, try not to die.
            // See if the next tile in the direction we're heading at is
            // walkable or not because if it's not then we need to see
            // where we can go in order not to die.
            if (!pathfinder.GetNeighbourInDirection(playerInput.Direction, pathfinder.GridNodeFromWorldPoint(headPosition)).IsWalkable)
            {
                int nextDirection = playerInput.Direction;

                // Check if we're moving along the x axis currently.
                if(playerInput.Direction != PlayerInput.NORTH && playerInput.Direction != PlayerInput.SOUTH)
                {
                    bool northIsEmpty = false;
                    bool southIsEmpty = false;

                    // Check if we can go north
                    if(pathfinder.GetNeighbourInDirection(PlayerInput.NORTH, pathfinder.GridNodeFromWorldPoint(headPosition)).IsWalkable)
                    {
                        northIsEmpty = true;
                    }

                    // Check if we can go south
                    if(pathfinder.GetNeighbourInDirection(PlayerInput.SOUTH, pathfinder.GridNodeFromWorldPoint(headPosition)).IsWalkable)
                    {
                        southIsEmpty = true;
                    }

                    // If both north and south is empty then just randomly pick
                    // a direction. Otherwise go in the direction that's empty.
                    if(northIsEmpty && southIsEmpty)
                    {
                        playerInput.SetAIMovement(Random.Range(1, 2) == 1 ? PlayerInput.NORTH : PlayerInput.SOUTH);
                    }
                    else if (northIsEmpty)
                    {
                        playerInput.SetAIMovement(PlayerInput.NORTH);
                    }
                    else if (southIsEmpty)
                    {
                        playerInput.SetAIMovement(PlayerInput.SOUTH);
                    }
                }
                else
                {
                    bool eastIsEmpty = false;
                    bool westIsEmpty = false;

                    // Check if we can go east.
                    if (pathfinder.GetNeighbourInDirection(PlayerInput.EAST, pathfinder.GridNodeFromWorldPoint(headPosition)).IsWalkable)
                    {
                        eastIsEmpty = true;
                    }

                    // Check if we can go west.
                    if (pathfinder.GetNeighbourInDirection(PlayerInput.WEST, pathfinder.GridNodeFromWorldPoint(headPosition)).IsWalkable)
                    {
                        westIsEmpty = true;
                    }

                    // If both east and west is empty then randomly
                    // pick a direction. Otherwise go in the direction
                    // that is empty.
                    if (eastIsEmpty && westIsEmpty)
                    {
                        playerInput.SetAIMovement(Random.Range(1, 2) == 1 ? PlayerInput.EAST : PlayerInput.WEST);
                    }
                    else if (eastIsEmpty)
                    {
                        playerInput.SetAIMovement(PlayerInput.EAST);
                    }
                    else if (westIsEmpty)
                    {
                        playerInput.SetAIMovement(PlayerInput.WEST);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Update the snakes body. If we're supposed to grow then add a
    /// new snakebody in the heads last position. If we're not supposed
    /// to grow though we remove the tail and insert it at the heads last
    /// position. We're also removing it from the list and inserting it again
    /// after the snakes head.
    /// </summary>
    private void UpdateBody()
    {
        if(GrowAmount > 0)
        {
            GameObject go = GameObject.Instantiate(snakeBodyPrefab, lastHeadPosition, Quaternion.identity, transform);
            snakeBodyParts.InsertAfter(go, snakeBodyParts.GetFirst());
            GrowAmount--;
        }
        else
        {
            GameObject tail = snakeBodyParts.GetLast().Value;
            tail.transform.position = lastHeadPosition;
            snakeBodyParts.RemoveAt(snakeBodyParts.Count - 1);
            snakeBodyParts.InsertAfter(tail, snakeBodyParts.GetFirst());
        }
    }
}