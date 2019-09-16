using UnityEngine;
using UnityEngine.Assertions;

public class SnakeHead : MonoBehaviour
{
    Player snake;

    private void Awake()
    {
        // Get snake object in parent and make sure that
        // it has one.
        snake = gameObject.GetComponentInParent<Player>();
        Assert.IsNotNull(snake, "Could not find Player Component on player");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Food":
                // Get the food component in the object tagged food.
                Food food = collision.GetComponent<Food>();
                // Make sure the food have a food script attached to it.
                Assert.IsNotNull(food, $"Food without food component. Game objects name: {collision.name}");

                // Call eat, add growth to the snake and remove time.
                food.Eat();
                snake.GrowAmount += food.GrowAmount;
                GameController.Instance.RemoveTime(food.TimeRemoval);
                break;
            case "Snake":
                // The snake ate itself. Die.
                GameController.Instance.Die();
                Debug.Log("I ate myself :(");
                break;
            case "Wall":
                // The snake tried to eat a wall. Die.
                GameController.Instance.Die();
                Debug.Log("I broke my teeth on a wall :(");
                break;
            default:
                Debug.Log($"I hit a: {collision.tag}");
                break;
        }
    }
}