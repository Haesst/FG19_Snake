using UnityEngine;

public class Apple : Food
{
    public override void Eat()
    {
        transform.position = GameController.Instance.GetEmptyPosition();
        base.Eat();
    }
}