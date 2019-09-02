using UnityEngine;
using UnityEngine.Assertions;

public class Apple : Food
{
    public override void Eat(Transform eater)
    {
        Snake snake = eater.GetComponent<Snake>();
        Assert.IsNotNull(snake, "Snake without snake component");
        snake.GrowAmount += growthAmount;
        GameController.Instance.AddScore(points);
        GameController.Instance.TimeRemoval += timeRemoval;

        Vector3 newPosition = GameController.Instance.GetRandomEmptyPosition();
        transform.position = newPosition;
    }
}