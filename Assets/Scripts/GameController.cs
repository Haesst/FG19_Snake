using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ScoreIntEvent : UnityEvent<int>{ }

public class GameController : MonoBehaviour
{
    [SerializeField] GameObject applePrefab = null;
    [SerializeField] int appleMinY = -9;
    [SerializeField] int appleMaxY = 9;
    [SerializeField] int appleMinX = -21;
    [SerializeField] int appleMaxX = 21;
    [SerializeField] float startingTimer = 0.6f;
    [SerializeField] float lowestTimer = 0.1f;
    [SerializeField] float appleTimeRemoval = 0.01f;
    [SerializeField] int startSize = 1;

    private int applesEaten = 0;
    private int score = 0;

    public ScoreIntEvent scoreIntEvent;
    public int ApplesEaten { get => applesEaten; }
    public float StartingTimer { get => startingTimer; }
    public float LowestTimer { get => lowestTimer; }
    public float AppleTimeRemoval { get => appleTimeRemoval; }
    public int StartSize { get => startSize; }

    public void AddScore(int score)
    {
        this.score += score;
        scoreIntEvent.Invoke(this.score);
    }

    public void EatApple()
    {
        applesEaten++;
        SpawnApple();
        scoreIntEvent.Invoke(applesEaten);
    }
    public void SpawnApple()
    {
        // Check if there's anything in that position
        Vector3 position = GetRandomPosition();


        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.zero);

        if(hit.collider == null)
        {
            GameObject gameObject = Instantiate(applePrefab, position, Quaternion.identity);
            gameObject.name = "Apple";
        }
        else
        {
            SpawnApple();
        }
    }

    private Vector3 GetRandomPosition()
    {
        return new Vector3(Random.Range(appleMinX, appleMaxX), Random.Range(appleMinY, appleMaxY));
    }

    private void Start()
    {
        SpawnApple();
    }
}