using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GameController : MonoBehaviour
{
    // Todo: Pathfinding
    [Header("Prefabs")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject apple;

    [Header("Game settings")]
    [SerializeField] private float tickTime = 0.3f;
    [SerializeField] private int startGrowth = 3;
    [Header("Map settings")]
    [SerializeField] private int width = 42;
    [SerializeField] private int height = 18;
    [SerializeField] private string mapSeed;

    [Header("Tick information")]
    [ShowOnly, SerializeField] private int currentTick = 0;
    [Header("Game information")]
    [ShowOnly, SerializeField] private int score;
    [SerializeField] private ScoreEvent scoreEvent;

    public int[,] map;
    private float tickTimer;
    private bool gameIsRunning = true;

    private List<GameObject> foodsInPlay;

    private static System.Random random;

    public bool GameLoading { get; private set; } = true;
    public bool GameRunning { get; set; }
    public int Score { get => score; private set => score = value; }
    public int StartGrowth { get => startGrowth; }
    public static System.Random GetRandom { get => random; }
    public int CurrentTick { get => currentTick; }
    public int Width { get => width; }
    public int Height { get => height; }
    public GameObject GetTopFood { get => foodsInPlay[0]; }
    public bool GameIsRunning { get => gameIsRunning; set => gameIsRunning = value; }

    public static GameController Instance { get; private set; }

    private void Awake()
    {
        Assert.IsNotNull(player, "Player not specified in editor");
        Assert.IsNotNull(apple, "Apple not specified in editor");
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
        if(mapSeed == null || mapSeed == "")
        {
            mapSeed = System.DateTime.Now.ToString();
        }

        foodsInPlay = new List<GameObject>();
        random = new System.Random(mapSeed.GetHashCode());
        tickTimer = tickTime;
    }

    private void Start()
    {
        SpawnSnake();
        SpawnFirstApple();
    }

    private void Update()
    {
        if(tickTimer <= 0 && gameIsRunning)
        {
            currentTick++;
            tickTimer = tickTime;
        }
        else
        {
            tickTimer -= Time.deltaTime;
        }
    }

    private void SpawnSnake()
    {
        GameObject go = GameObject.Instantiate(player);
        go.GetComponent<Player>().AI = true;
    }

    private void SpawnFirstApple()
    {
        GameObject go = GameObject.Instantiate(apple, GetEmptyPosition(), Quaternion.identity);
        foodsInPlay.Add(go);
    }

    public Vector2 GetEmptyPosition()
    {
        int x = 0;
        int y = 0;

        bool positionFound = false;

        while (!positionFound)
        {
            x = random.Next(1, width);
            y = random.Next(1, height);

            x = -width / 2 + x;
            y = -height / 2 + y;

            RaycastHit2D[] raycastHits = Physics2D.BoxCastAll(new Vector2(x, y), Vector2.one * 0.5f, 0f, Vector2.zero);

            if(raycastHits.Length == 0)
            {
                positionFound = true;
            }
            else
            {
                Debug.Log($"Tried position [{x},{y}] but couldn't return it because there's {raycastHits.Length} objects there");
            }
        }

        return new Vector2(x, y);
    }

    public void RemoveTime(float time)
    {
        tickTimer -= time;
    }

    public void AddScore(int score)
    {
        this.score += score;
        scoreEvent.Invoke(this.score);
    }
}