using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(PlayerInput))]
public class Snake : MonoBehaviour
{
    [SerializeField] GameObject snakeHead = null;
    [SerializeField] GameObject snakeBody = null;
    [SerializeField] LayerMask snakeCollisions;

    [SerializeField] string foodTag = "Food";


    GameController gameController;
    PlayerInput playerInput;
    private LinkedList<GameObject> snakeBodyParts = new LinkedList<GameObject>();
    private Vector2 headPosition;

    private float timer;
    private float currentTickTime;
    private int growAmount;

    public int GrowAmount { get => growAmount; set => growAmount = value; }

    // Todo: Move gameRunning to GameController
    private bool gameRunning = true;

    #region Unity Functions
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        Assert.IsNotNull(playerInput, "PlayerInput component could not be found.");
        Assert.IsNotNull(gameController, "GameController could not be found.");
        snakeBodyParts.AddFirst(snakeHead);

        timer = gameController.StartingTimer;
        currentTickTime = gameController.StartingTimer;
        growAmount = gameController.StartSize;
    }

    private void Update()
    {
        if(timer <= 0 && gameRunning)
        {
            if (growAmount > 0)
            {
                AddSnakeBody();
                UpdateHeadPosition();
                CheckForCollision();
            }
            else
            {
                UpdateBodyPosition();
                UpdateHeadPosition();
                CheckForCollision();
            }
            headPosition = snakeHead.transform.position;
            timer = currentTickTime;
        }
        else
        {
            timer -= Time.deltaTime;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == foodTag)
        {
            Food food = (Food)collision.gameObject.GetComponent<Food>();
            gameController.AddScore(food.Points);
            growAmount += food.GrowthAmount;
            food.Eat();
        }
        //else if(collision.name == "Apple")
        //{
        //    Destroy(collision.gameObject);
        //    gameController.EatApple();
        //    growAmount++;
        //    if(timer - gameController.AppleTimeRemoval <= gameController.LowestTimer)
        //    {
        //        currentTickTime = gameController.LowestTimer;
        //    }
        //    else
        //    {
        //        currentTickTime -= gameController.AppleTimeRemoval;
        //    }
        //}
    }
    #endregion Unity Functions
    private void UpdateHeadPosition()
    {
        Vector3 newPos;
        int rotation;
        if (playerInput.Direction == PlayerInput.NORTH)
        {
            newPos = new Vector3(snakeHead.transform.position.x, snakeHead.transform.position.y + 1, snakeHead.transform.position.z);
            rotation = 0;
        }
        else if (playerInput.Direction == PlayerInput.EAST)
        {
            newPos = new Vector3(snakeHead.transform.position.x + 1, snakeHead.transform.position.y, snakeHead.transform.position.z);
            rotation = -90;
        }
        else if (playerInput.Direction == PlayerInput.SOUTH)
        {
            newPos = new Vector3(snakeHead.transform.position.x, snakeHead.transform.position.y - 1, snakeHead.transform.position.z);
            rotation = 180;
        }
        else
        {
            newPos = new Vector3(snakeHead.transform.position.x - 1, snakeHead.transform.position.y, snakeHead.transform.position.z);
            rotation = 90;
        }

        snakeHead.transform.position = newPos;
        snakeHead.transform.rotation = Quaternion.Euler(0, 0, rotation);
    }

    private void UpdateBodyPosition()
    {
        if(snakeBodyParts.Count > 1)
        {
            GameObject lastPart = snakeBodyParts.Last.Value;
            snakeBodyParts.RemoveLast();

            lastPart.transform.position = headPosition;
            snakeBodyParts.AddAfter(snakeBodyParts.First, lastPart);
        }
    }

    private void AddSnakeBody()
    {
        GameObject gameObject = Instantiate(snakeBody, transform);
        gameObject.transform.position = headPosition;
        snakeBodyParts.AddAfter(snakeBodyParts.First, gameObject);
        growAmount--;
    }

    private void CheckForCollision()
    {
        RaycastHit2D raycast = Physics2D.Raycast(snakeHead.transform.position, Vector3.zero, 0, snakeCollisions);

        if (raycast.collider != null)
        {
            //// The raycast hit's the tail before it has time to move. Make sure the distance is over 0.5 if the hitpoint is a snakepart
            if (!CheckForSnakeCollision(raycast.collider.transform) || 
                (CheckForSnakeCollision(raycast.collider.transform) && 
                Vector3.Distance(raycast.collider.transform.position, snakeHead.transform.position) < 0.5))
            {
                gameRunning = false;
                raycast.collider.name = "Hitpoint.";
            }
        }
    }

    private bool CheckForSnakeCollision(Transform transform)
    {
        return transform.parent == this.transform;
    }
}