using UnityEngine;
using UnityEngine.Assertions;

public class SnakeHead : MonoBehaviour
{
    Player snake;

    private void Awake()
    {
        snake = gameObject.GetComponentInParent<Player>();
        Assert.IsNotNull(snake, "Could not find Player Component on player");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Food":
                Food food = collision.GetComponent<Food>();
                Assert.IsNotNull(food, $"Food without food component. Game objects name: {collision.name}");

                food.Eat();
                snake.GrowAmount += food.GrowAmount;
                GameController.Instance.RemoveTime(food.TimeRemoval);
                break;
            default:
                Debug.Log($"I hit a: {collision.tag}");
                break;
        }
    }
}