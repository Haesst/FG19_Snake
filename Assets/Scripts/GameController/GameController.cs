using UnityEngine;
using UnityEngine.Assertions;

public class GameController : MonoBehaviour
{
    // Todo: Apples can spawn under snake :S
    // Todo: Pathfinding
    [Header("Prefabs")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject apple;

    [Header("Game settings")]
    [SerializeField] private float tickTime = 0.3f;
    [SerializeField] private int startGrowth = 3;
    [Header("Map settings")]
    [SerializeField] private int width = 40;
    [SerializeField] private int height = 18;
    [SerializeField] private string mapSeed;

    [Header("Tick information")]
    [ShowOnly, SerializeField] private int currentTick = 0;
    [Header("Game information")]
    [ShowOnly, SerializeField] private int score;
    [SerializeField] private ScoreEvent scoreEvent;

    public int[,] map;
    private float tickTimer;

    private static System.Random random;

    public bool GameLoading { get; private set; } = true;
    public bool GameRunning { get; set; }
    public int Score { get => score; private set => score = value; }
    public int StartGrowth { get => startGrowth; }
    public static System.Random GetRandom { get => random; }
    public int CurrentTick { get => currentTick; }

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
        if(tickTimer <= 0)
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
    }

    private void SpawnFirstApple()
    {
        GameObject go = GameObject.Instantiate(apple, GetEmptyPosition(), Quaternion.identity);
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

            RaycastHit2D raycastHit = Physics2D.Raycast(new Vector2(x, y), Vector2.zero);

            if(raycastHit.collider == null)
            {
                positionFound = true;
            }
            else
            {
                Debug.Log($"Tried position [{x},{y}] but couldn't return it because there's a {raycastHit.collider.name} there");
            }
        }

        return new Vector2((-width / 2 + 1 + x), (-height / 2) + 1 + y);
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