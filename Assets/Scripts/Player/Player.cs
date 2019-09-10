using UnityEngine;
using UnityEngine.Assertions;

public class Player : MonoBehaviour
{
    [SerializeField] GameObject snakeHeadPrefab;
    [SerializeField] GameObject snakeBodyPrefab;

    PlayerInput playerInput;
    LinkedList snakeBodyParts;
    Vector3 lastHeadPosition;
    Camera mainCamera;
    Pathfinding pathfinder;

    private int lastTick = 0;
    public int GrowAmount { get; set; } = 0;
    public bool AI { get; set; } = false;

    private void Awake()
    {
        mainCamera = Camera.main;
        Assert.IsNotNull(mainCamera, "Main Camera not found!");

        playerInput = GetComponent<PlayerInput>();
        Assert.IsNotNull(playerInput, "Could not find PlayerInput script on GameObject!");

        pathfinder = GetComponent<Pathfinding>();
        Assert.IsNotNull(pathfinder, "Could not find Pathfinding script on GameObject!");

        GameObject go = GameObject.Instantiate(snakeHeadPrefab, transform);
        snakeBodyParts = new LinkedList();
        snakeBodyParts.InsertFirst(go);
    }

    #region UnityMethods
    private void Start()
    {
        GrowAmount = GameController.Instance.StartGrowth;
    }

    private void Update()
    {
        mainCamera.transform.position = new Vector3(transform.position.x, transform.position.y, mainCamera.transform.position.z);
        
        if(pathfinder.Path == null)
        {
            pathfinder.FindPath(snakeBodyParts.GetFirst().Value.transform.position, GameController.Instance.GetTopFood.transform.position);
        }

        if(lastTick < GameController.Instance.CurrentTick)
        {
            pathfinder.UpdateGrid();
            pathfinder.FindPath(lastHeadPosition, GameController.Instance.GetTopFood.transform.position);
            if (AI)
            {
                CalculateNextMove();
            }
            GetPlayerDirection();
            UpdateBody();

            lastHeadPosition = snakeBodyParts.GetFirst().Value.transform.position;
            lastTick++;
        }
    }
    #endregion UnityMethods

    private void GetPlayerDirection()
    {
        Transform snakeHead = snakeBodyParts.GetFirst().Value.transform;

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

        playerInput.LastDirection = playerInput.Direction;
    }

    private void CalculateNextMove()
    {
        Vector2 headPosition = snakeBodyParts.GetFirst().Value.transform.position;

        if (pathfinder.Path.Count > 0)
        {
            Vector2 nextMove = pathfinder.GridNodeToWorldPosition(pathfinder.Path[0]);

            if (nextMove.x == headPosition.x)
            {
                // Change y
                playerInput.SetAIMovement(nextMove.y > headPosition.y ? PlayerInput.NORTH : PlayerInput.SOUTH);
            }
            else
            {
                playerInput.SetAIMovement(nextMove.x > headPosition.x ? PlayerInput.EAST : PlayerInput.WEST);
            }
        }
        else
        {
            Debug.Log("no path :(");
            if (!pathfinder.GetNeighbourInDirection(playerInput.Direction, pathfinder.GridNodeFromWorldPoint(headPosition)).IsWalkable)
            {
                int nextDirection = playerInput.Direction;

                if(playerInput.Direction != PlayerInput.NORTH && playerInput.Direction != PlayerInput.SOUTH)
                {
                    bool northIsEmpty = false;
                    bool southIsEmpty = false;

                    Debug.Log("East, or west is occupied, I NEED TO STEER AWAY!!!!");
                    if(pathfinder.GetNeighbourInDirection(PlayerInput.NORTH, pathfinder.GridNodeFromWorldPoint(headPosition)).IsWalkable)
                    {
                        Debug.Log("I can go north, remember that!");
                        northIsEmpty = true;
                    }

                    if(pathfinder.GetNeighbourInDirection(PlayerInput.SOUTH, pathfinder.GridNodeFromWorldPoint(headPosition)).IsWalkable)
                    {
                        Debug.Log("I can go south, remember that!");
                        southIsEmpty = true;
                    }

                    if(northIsEmpty && southIsEmpty)
                    {
                        Debug.Log("Both south and north is valid, lets make a guess.");
                        playerInput.SetAIMovement(Random.Range(1, 2) == 1 ? PlayerInput.NORTH : PlayerInput.SOUTH);
                    }
                    else if (northIsEmpty)
                    {
                        Debug.Log("Just north is empty, there's were we're heading");
                        playerInput.SetAIMovement(PlayerInput.NORTH);
                    }
                    else if (southIsEmpty)
                    {
                        Debug.Log("Just south is empty, there's were we're heading");
                        playerInput.SetAIMovement(PlayerInput.SOUTH);
                    }
                }
                else
                {
                    bool eastIsEmpty = false;
                    bool westIsEmpty = false;

                    Debug.Log("North or south is occupied, I NEED TO STEER AWAY!!!!");
                    if (pathfinder.GetNeighbourInDirection(PlayerInput.EAST, pathfinder.GridNodeFromWorldPoint(headPosition)).IsWalkable)
                    {
                        Debug.Log("I can go east, remember that!");
                        eastIsEmpty = true;
                    }

                    if (pathfinder.GetNeighbourInDirection(PlayerInput.WEST, pathfinder.GridNodeFromWorldPoint(headPosition)).IsWalkable)
                    {
                        Debug.Log("I can go west, remember that!");
                        westIsEmpty = true;
                    }

                    if (eastIsEmpty && westIsEmpty)
                    {
                        Debug.Log("Both east and west is valid, lets make a guess.");
                        playerInput.SetAIMovement(Random.Range(1, 2) == 1 ? PlayerInput.EAST : PlayerInput.WEST);
                    }
                    else if (eastIsEmpty)
                    {
                        Debug.Log("Just east is empty, there's were we're heading");
                        playerInput.SetAIMovement(PlayerInput.EAST);
                    }
                    else if (westIsEmpty)
                    {
                        Debug.Log("Just west is empty, there's were we're heading");
                        playerInput.SetAIMovement(PlayerInput.WEST);
                    }
                }
            }
            //Debug.Log("No path for you!");
            //if (!pathfinder.GetNeighbourInDirection(playerInput.Direction, pathfinder.GridNodeFromWorldPoint(headPosition)).IsWalkable)
            //{
            //    string direction = playerInput.Direction == PlayerInput.NORTH ? "North" :
            //                        playerInput.Direction == PlayerInput.EAST ? "East" :
            //                        playerInput.Direction == PlayerInput.SOUTH ? "South" :
            //                        playerInput.Direction == PlayerInput.WEST ? "West" : "";
            //    Debug.Log($"Trying my neighbour to the {direction}. It's not walkable");


                //    if(playerInput.Direction == PlayerInput.NORTH || playerInput.Direction == PlayerInput.SOUTH)
                //    {
                //        int possibleDirections = 0;
                //        bool eastIsEmpty = false;

                //        if (pathfinder.GetNeighbourInDirection(PlayerInput.EAST, pathfinder.GridNodeFromWorldPoint(headPosition)).IsWalkable)
                //        {
                //            possibleDirections++;
                //            eastIsEmpty = true;
                //        }

                //        if (pathfinder.GetNeighbourInDirection(PlayerInput.WEST, pathfinder.GridNodeFromWorldPoint(headPosition)).IsWalkable)
                //        {
                //            possibleDirections++;
                //        }

                //        if(possibleDirections == 2)
                //        {
                //            playerInput.SetAIMovement(Random.Range(0, 2) == 1 ? PlayerInput.EAST : PlayerInput.WEST);
                //        }
                //        else
                //        {
                //            if(possibleDirections == 1)
                //            {
                //                if (eastIsEmpty)
                //                {
                //                    playerInput.SetAIMovement(PlayerInput.EAST);
                //                }
                //                else
                //                {
                //                    playerInput.SetAIMovement(PlayerInput.WEST);
                //                }
                //            }
                //        }
                //    }
                //    else
                //    {
                //        int possibleDirections = 0;
                //        bool northIsEmpty = false;

                //        if (pathfinder.GetNeighbourInDirection(PlayerInput.NORTH, pathfinder.GridNodeFromWorldPoint(headPosition)).IsWalkable)
                //        {
                //            possibleDirections++;
                //        }

                //        if (pathfinder.GetNeighbourInDirection(PlayerInput.SOUTH, pathfinder.GridNodeFromWorldPoint(headPosition)).IsWalkable)
                //        {
                //            possibleDirections++;
                //        }

                //        if (possibleDirections == 2)
                //        {
                //            playerInput.SetAIMovement(Random.Range(0, 2) == 1 ? PlayerInput.NORTH : PlayerInput.SOUTH);
                //        }
                //        else
                //        {
                //            if (possibleDirections == 1)
                //            {
                //                if (northIsEmpty)
                //                {
                //                    playerInput.SetAIMovement(PlayerInput.NORTH);
                //                }
                //                else
                //                {
                //                    playerInput.SetAIMovement(PlayerInput.SOUTH);
                //                }
                //            }
                //        }
                //    }
                //}
                //else
                //{
                //    Debug.Log("Going forward!");
                //    playerInput.SetAIMovement(playerInput.Direction);
                //}
                //// 1. if node to the direction is not walkable
                //// 2. check the other axis for an empty space
                //// 3. if there's an empty space then choose a
                //// direction at random
        }
    }

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

    private void SearchForFood()
    {

    }
}