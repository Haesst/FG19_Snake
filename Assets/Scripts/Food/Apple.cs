using UnityEngine;

public class Apple : Food
{
    public override void Eat()
    {
        gameController.SpawnApple();
        base.Eat();
    }
}