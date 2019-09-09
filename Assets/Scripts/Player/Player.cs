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

    private int lastTick = 0;
    public int GrowAmount { get; set; } = 0;

    private void Awake()
    {
        mainCamera = Camera.main;
        Assert.IsNotNull(mainCamera, "Main Camera not found!");

        playerInput = GetComponent<PlayerInput>();
        Assert.IsNotNull(playerInput, "Could not find PlayerInput script on GameObject!");

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
        
        if(lastTick < GameController.Instance.CurrentTick)
        {
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