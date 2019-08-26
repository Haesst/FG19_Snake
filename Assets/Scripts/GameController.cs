using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] GameObject applePrefab;
    [SerializeField] int appleMinY = -9;
    [SerializeField] int appleMaxY = 9;
    [SerializeField] int appleMinX = -21;
    [SerializeField] int appleMaxX = 21;

    private int applesEaten = 0;
    public int ApplesEaten { get => applesEaten; }

    public void EatApple()
    {
        applesEaten++;
        SpawnApple();
    }
    private void SpawnApple()
    {
        // Check if there's anything in that position
        Vector3 position = GetRandomPosition();


        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.zero);

        if(hit.collider == null)
        {
            GameObject gameObject = Instantiate(applePrefab, position, Quaternion.identity);
            gameObject.name = "Apple";
            applesEaten++;
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