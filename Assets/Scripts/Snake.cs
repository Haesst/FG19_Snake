using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(PlayerInput))]
public class Snake : MonoBehaviour
{
    GameController gameController;
    PlayerInput playerInput;
    [SerializeField] GameObject snakeBody;
    LinkedList<GameObject> snakeBodyParts = new LinkedList<GameObject>();

    private int growAmount;
    public int GrowAmount { get => growAmount; set => growAmount = value; }

    private Vector2 headPosition;

    private float timer = 0.3f;
    private float moveTime = 0.3f;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        Assert.IsNotNull(playerInput, "PlayerInput component could not be found.");
        Assert.IsNotNull(gameController, "GameController could not be found.");
    }
    private void Update()
    {
        if(moveTime <= 0)
        {
            if (growAmount > 0)
            {
                UpdateHeadPosition();
                AddSnakeBody();
            }
            else
            {
                UpdateHeadPosition();
                UpdateBodyPosition();
            }
            headPosition = transform.position;
            moveTime = timer;
        }
        else
        {
            moveTime -= Time.deltaTime;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.name == "Apple")
        {
            Destroy(collision.gameObject);
            gameController.EatApple();
            growAmount++;
            timer -= 0.01f;
        }

    }
    private void UpdateHeadPosition()
    {
        Vector3 newPos;
        if (playerInput.Direction == PlayerInput.NORTH)
            newPos = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        else if (playerInput.Direction == PlayerInput.EAST)
            newPos = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);
        else if (playerInput.Direction == PlayerInput.SOUTH)
            newPos = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
        else
            newPos = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z);

        transform.position = newPos;
    }

    private void UpdateBodyPosition()
    {
        if(snakeBodyParts.Count > 0)
        {
            GameObject lastPart = snakeBodyParts.Last.Value;
            snakeBodyParts.RemoveLast();

            lastPart.transform.position = headPosition;
            snakeBodyParts.AddFirst(lastPart);
        }
    }

    private void AddSnakeBody()
    {
        // Todo: Figure out why it pushes the other parts
        GameObject gameObject = Instantiate(snakeBody, headPosition, Quaternion.identity);
        snakeBodyParts.AddFirst(gameObject);
        growAmount--;
    }
}