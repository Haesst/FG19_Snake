using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(PlayerInput))]
public class Snake : MonoBehaviour, IObjectPlacer
{
    [SerializeField] GameObject snakeHead = null;
    [SerializeField] GameObject snakeBody = null;
    [SerializeField] LayerMask snakeCollisions;

    [SerializeField] string foodTag = "Food";
    [SerializeField] bool AI = false;

    private PlayerInput playerInput;
    private LinkedList<GameObject> snakeBodyParts = new LinkedList<GameObject>();
    private Pathfinding pathfinding;

    private float currentTickTime;
    private int growAmount;
    private int lastTick;

    private List<GridNode> path = new List<GridNode>();

    public int GrowAmount { get => growAmount; set => growAmount = value; }

    #region Unity Functions
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        pathfinding = GetComponentInChildren<Pathfinding>();

        Assert.IsNotNull(playerInput, "PlayerInput component could not be found.");
        Assert.IsNotNull(pathfinding, "Pathfinding component could not  be found.");

        snakeBodyParts.AddFirst(snakeHead);
        growAmount = GameController.Instance.StartSize;
        pathfinding.Seeker = snakeHead.transform;
        // Resource loading
        // Resources.Load<GameObject>("Resources/Prefabs/Snake")
    }

    private void Update()
    {
        UpdateGrid();
        CheckForTarget();
        //CheckForFood();
        if (AI && path == null || path?.Count <= 0)
        {
            path = pathfinding.Path;
        }
        CommandInvoker.SetNextMoveAction(new MoveTransformCommand(snakeHead.transform, GetNewHeadPosition(), Quaternion.Euler(0, 0, GetRotation(playerInput.Direction))));
        
        
        if (CommandInvoker.CurrentTick > lastTick)
        {
            if (growAmount > 0)
            {
                lastTick = CommandInvoker.AddTickAction(new InstantiateObjectCommand(snakeBody, snakeHead.transform.position, Quaternion.identity, transform, this));
                growAmount--;
            }
            else
            {
                if (snakeBodyParts.Count > 1)
                {
                    GameObject lastPart = snakeBodyParts.Last.Value;
                    snakeBodyParts.RemoveLast();
                    snakeBodyParts.AddAfter(snakeBodyParts.First, lastPart);
                    lastTick = CommandInvoker.AddTickAction(new MoveTransformCommand(lastPart.transform, snakeHead.transform.position));
                }
            }
            playerInput.LastDirection = playerInput.Direction;
        }
    }
    #endregion Unity Functions

    private int GetRotation(int direction)
    {
        return direction == PlayerInput.NORTH
            ? 0
            : direction == PlayerInput.EAST
            ? -90
            : direction == PlayerInput.SOUTH
            ? 180
            : 90;
    }

    private void CheckForFood()
    {
        RaycastHit2D[] raycast = Physics2D.RaycastAll(snakeHead.transform.position, Vector2.zero);
        /*
        if (raycast.collider != null)
        {
            if(raycast.collider.tag == foodTag)
            {
                Food food = (Food)raycast.collider.GetComponent<Food>();
                Assert.IsNotNull(food, "GameObject tagged Food without food component");
                gameController.AddScore(food.Points);
                growAmount += food.GrowthAmount;
                food.Eat();
            }
        }
        */
        for(int i = 0; i < raycast.Length; i++)
        {
            if(raycast[i].collider != null && raycast[i].collider.tag == foodTag)
            {
                Food food = (Food)raycast[i].collider.GetComponent<Food>();
                Assert.IsNotNull(food, "GameObject tagged Food without food component");
                GameController.Instance.AddScore(food.Points);
                growAmount += food.GrowthAmount;
                food.Eat(transform);
            }
        }
    }

    public void PlaceObjectCallback(GameObject gameObject)
    {
        snakeBodyParts.AddAfter(snakeBodyParts.First, gameObject);
    }
    private Vector3 GetNewHeadPosition()
    {
        if (AI)
        {
            if (path != null && path.Count > 0)
            {
                if (path[0].WorldPosition.x != snakeHead.transform.position.x)
                {
                    if (path[0].WorldPosition.x > snakeHead.transform.position.x)
                    {
                        path.RemoveAt(0);
                        playerInput.Direction = PlayerInput.EAST;
                        return new Vector3(snakeHead.transform.position.x + 1, snakeHead.transform.position.y, transform.position.z);
                    }
                    else
                    {
                        path.RemoveAt(0);
                        playerInput.Direction = PlayerInput.WEST;
                        return new Vector3(snakeHead.transform.position.x - 1, snakeHead.transform.position.y, transform.position.z);
                    }
                }
                else
                {
                    if (path[0].WorldPosition.y > snakeHead.transform.position.y)
                    {
                        path.RemoveAt(0);
                        playerInput.Direction = PlayerInput.NORTH;
                        return new Vector3(snakeHead.transform.position.x, snakeHead.transform.position.y + 1, transform.position.z);
                    }
                    else
                    {
                        path.RemoveAt(0);
                        playerInput.Direction = PlayerInput.SOUTH;
                        return new Vector3(snakeHead.transform.position.x, snakeHead.transform.position.y - 1, transform.position.z);
                    }
                }
            }
            else
            {
                return new Vector3(snakeHead.transform.position.x, snakeHead.transform.position.y, transform.position.z);
            }
        }
        else
        {
            return playerInput.Direction == PlayerInput.NORTH
                ? new Vector3(snakeHead.transform.position.x, snakeHead.transform.position.y + 1, snakeHead.transform.position.z)
                : playerInput.Direction == PlayerInput.EAST
                ? new Vector3(snakeHead.transform.position.x + 1, snakeHead.transform.position.y, snakeHead.transform.position.z)
                : playerInput.Direction == PlayerInput.SOUTH
                ? new Vector3(snakeHead.transform.position.x, snakeHead.transform.position.y - 1, snakeHead.transform.position.z)
                : new Vector3(snakeHead.transform.position.x - 1, snakeHead.transform.position.y, snakeHead.transform.position.z);
        }
    }

    private void CheckForCollision()
    {
        RaycastHit2D raycast = Physics2D.Raycast(snakeHead.transform.position, Vector3.zero, 0, snakeCollisions);

        if (raycast.collider != null)
        {
            //// The raycast hits the tail before it has time to move. Make sure the distance is over 0.5 if the hitpoint is a snakepart
            if (!CheckForSnakeCollision(raycast.collider.transform) || 
                (CheckForSnakeCollision(raycast.collider.transform) && 
                Vector3.Distance(raycast.collider.transform.position, snakeHead.transform.position) < 0.5))
            {
                GameController.Instance.GameEnd();
                raycast.collider.name = "Hitpoint.";
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == foodTag)
        {
            Food food = collision.gameObject.GetComponent<Food>();
            Assert.IsNotNull(food, "Food without food component!");
            food.Eat(transform);
        }
    }

    private bool CheckForCollisionAt(Vector2 position)
    {
        Debug.Log(position);
        RaycastHit2D[] hits = Physics2D.RaycastAll(position, Vector2.zero, snakeCollisions);

        for(int i = 0; i < hits.Length; i++)
        {
            if(hits[i].collider != null && hits[i].collider.tag != foodTag)
            {
                return true;
            }
        }
        return false;
    }

    private bool CheckForSnakeCollision(Transform transform)
    {
        return transform.parent == this.transform;
    }

    private void CheckForTarget()
    {
        if(pathfinding.Target == null)
        {
            GameObject target = GameObject.FindGameObjectWithTag(foodTag);
            if(target != null)
            {
                pathfinding.Target = target.transform;
            }
        }
    }

    private void UpdateGrid()
    {
        pathfinding?.UpdateGrid();
    }
}