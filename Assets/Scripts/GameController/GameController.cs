using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject apple;

    [Header("Game settings")]
    [SerializeField] private float tickTime = 0.3f;
    [SerializeField] private int startGrowth = 3;
    [Header("Map settings")]
    [SerializeField] private int width = 42;
    [SerializeField] private int height = 18;

    [Header("Tick information")]
    [ShowOnly, SerializeField] private int currentTick = 0;
    [ShowOnly, SerializeField] private float currentTickTime = 0;

    [Header("Game information")]
    [ShowOnly, SerializeField] private int score;
    [SerializeField] GameObject deathPanel;
    public ScoreEvent scoreEvent = new ScoreEvent();

    public int[,] map;
    private float tickTimer;
    private bool gameIsRunning = false;

    private List<GameObject> foodsInPlay;
    private System.Random random;
    EventTrigger trigger;


    public int StartGrowth { get => startGrowth; }
    public int CurrentTick { get => currentTick; }
    public int Width { get => width; }
    public int Height { get => height; }

    // Since we only have one kind of food right now just
    // return the first object in the list.
    public GameObject GetTopFood { get => foodsInPlay[0]; }
    public bool GameIsRunning { get => gameIsRunning; set => gameIsRunning = value; }

    public static GameController Instance { get; private set; }

    #region Unity functions
    private void Awake()
    {
        // Create a singleton instance by checking if
        // there is a current instance already and if
        // that instance's not this gameobject. Then
        // we want to delete it, but if there's none
        // we want to set the instance to be this.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        // Make sure both player and apple is specified in editor
        Assert.IsNotNull(player, "Player prefab not specified in editor");
        Assert.IsNotNull(apple, "Apple prefab not specified in editor");

        // Create a new list of foodsInPlay(In order to have more than one food currently active)
        foodsInPlay = new List<GameObject>();
        // Create a new Random
        random = new System.Random();
        tickTimer = tickTime;
        currentTickTime = tickTime;
    }

    private void Start()
    {
        gameIsRunning = true;
        SpawnSnake();
        SpawnFirstApple();
    }

    private void Update()
    {
        // In order to get a more classic snake movement we create
        // a custom game tick and every update we check if we're
        // supposed to add to the current tick and reset the timer
        // or not.
        if(tickTimer <= 0 && gameIsRunning)
        {
            currentTick++;
            tickTimer = currentTickTime;
        }
        else
        {
            tickTimer -= Time.deltaTime;
        }
    }

    #endregion Unity functions

    //public void StartGame()
    //{
    //    //UpdateScore go = GameObject.Find("ScoreText")?.GetComponent<UpdateScore>();
    //    //scoreEvent.AddListener(go.SetScoreText);
    //    //scoreEvent += test;

    //    gameIsRunning = true;
    //    SpawnSnake();
    //    SpawnFirstApple();
    //}

    /// <summary>
    /// Spawn the snakes body in an empty position.
    /// Set AI to desired value.
    /// </summary>
    private void SpawnSnake()
    {
        GameObject go = GameObject.Instantiate(player);
    }

    /// <summary>
    /// Spawn the first apple at an empty position and
    /// add it to the foodsInPlayList.
    /// </summary>
    private void SpawnFirstApple()
    {
        GameObject go = GameObject.Instantiate(apple, GetEmptyPosition(), Quaternion.identity);
        foodsInPlay.Add(go);
    }

    /// <summary>
    /// Get an empty position in the game area.
    /// It runs in a while-loop that continues until a
    /// position have been found. It takes a random position
    /// between one and the with/height and then sends out a
    /// boxcast there and if there's no hit at the position
    /// we can set positionFound to be true, otherwise we try
    /// again.
    /// </summary>
    /// <returns></returns>
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
        }

        return new Vector2(x, y);
    }

    /// <summary>
    /// Remove time frome the current tick timer.
    /// </summary>
    /// <param name="time">Time in seconds to remove.</param>
    public void RemoveTime(float time)
    {
        currentTickTime -= time;
    }

    /// <summary>
    /// Add to the players score.
    /// </summary>
    /// <param name="score">Amount to add.</param>
    public void AddScore(int score)
    {
        this.score += score;
        scoreEvent.Invoke(this.score);
    }

    /// <summary>
    /// Gets called when the snake dies.
    /// </summary>
    public void Die()
    {
        gameIsRunning = false;
        ToggleDeathPanel(true);
    }

    /// <summary>
    /// Gets called when the play again button is presssed.
    /// </summary>
    public void Restart()
    {
        // Delete the player's gameobject
        Destroy(GameObject.FindGameObjectWithTag("Player"));

        // Destroy every food item that's in play. Since we only have an apple at
        // the moment we could have done as we did with the players gameobject but
        // since we want to be able to create more food we do it this way instead.
        foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Food"))
        {
            Destroy(gameObject);
        }

        // Clear the foods in play list
        foodsInPlay.Clear();

        // Reset tick timers
        ResetTickTimers();

        // Spawn snake and apple
        SpawnSnake();
        SpawnFirstApple();

        // Start the game again.
        GameIsRunning = true;
        ToggleDeathPanel(false);
    }

    private void ResetTickTimers()
    {
        currentTickTime = tickTime;
        tickTimer = tickTime;
        currentTick = 0;
    }

    public void ToggleDeathPanel(bool show)
    {
        deathPanel.SetActive(show);
    }

    public void QuitToMainMenu()
    {
        GameManager.Instance.QuitToMainMenu();
    }
}