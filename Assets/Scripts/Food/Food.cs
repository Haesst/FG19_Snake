using UnityEngine;
using UnityEngine.Assertions;

public class Food : MonoBehaviour
{
    [SerializeField] protected int points = 10;
    [SerializeField] protected int growthAmount = 1;
    [SerializeField] protected float timeRemoval = 0f;

    public int Points { get => points; }
    public int GrowthAmount { get => growthAmount; }

    public virtual void Eat(Transform eater)
    {
        Snake snake = eater.GetComponent<Snake>();
        Assert.IsNotNull(snake, "Snake without snake component");
        snake.GrowAmount += growthAmount;
        GameController.Instance.AddScore(points);
        GameController.Instance.TimeRemoval += timeRemoval;
        Destroy(gameObject);
    }
}