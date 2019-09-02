using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ScoreIntEvent : UnityEvent<int>{ }

public class GameController : MonoBehaviour
{
    private static GameController instance;
    public static GameController Instance { get => instance; }

    [SerializeField] GameObject applePrefab = null;
    [SerializeField] int foodMinY = -9;
    [SerializeField] int foodMaxY = 9;
    [SerializeField] int foodMaxX = 21;
    [SerializeField] int foodMinX = -21;
    [SerializeField] float startingTimer = 0.6f;
    [SerializeField] float lowestTimer = 0.1f;
    [SerializeField] int startSize = 1;

    private int score = 0;
    private bool gameInPlay = true;
    private float timeRemoval = 0;
    
    public ScoreIntEvent scoreIntEvent;
    public float StartingTimer { get => startingTimer; }
    public int StartSize { get => startSize; }
    public bool GameInPlay { get => gameInPlay; }

    public float TimeRemoval { get => timeRemoval; set => timeRemoval = value; }

    public int GetFieldWidth { get => Mathf.Abs(foodMaxX - foodMinX); }
    public int GetFieldHeight { get => Mathf.Abs(foodMaxY - foodMinY); }
    public Vector2 GetFieldBottomLeft { get => new Vector2(foodMinX, foodMinY); }

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public void AddScore(int score)
    {
        this.score += score;
        scoreIntEvent.Invoke(this.score);
    }

    public void EatApple()
    {
        SpawnApple();
    }
    public void SpawnApple()
    {
        // Check if there's anything in that position
        Vector3 position = GetRandomPosition();


        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.zero);

        if(hit.collider == null)
        {
            CommandInvoker.AddAction(new InstantiateObjectCommand(applePrefab, position));
        }
        else
        {
            SpawnApple();
        }
    }

    public Vector3 GetRandomEmptyPosition()
    {
        while(true)
        {
            Vector3 attempt = GetRandomPosition();

            RaycastHit2D hit = Physics2D.Raycast(attempt, Vector2.zero);

            if(hit.collider == null)
            {
                return attempt;
            }
        }
    }

    private Vector3 GetRandomPosition()
    {
        return new Vector3(Random.Range(foodMaxX, foodMinX), Random.Range(foodMinY, foodMaxY));
    }

    private void Start()
    {
        SpawnApple();
    }

    public void GameEnd()
    {
        gameInPlay = false;
    }
}