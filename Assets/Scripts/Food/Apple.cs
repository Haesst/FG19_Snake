using UnityEngine;

public class Apple : Food
{
    /// <summary>
    /// An apple should always be present and instead of just despawning
    /// and creating new ones we can change the position of the apple
    /// that's in play. Then call base.Eat() to add the points and remove
    /// time and so on.
    /// </summary>
    public override void Eat()
    {
        transform.position = GameController.Instance.GetEmptyPosition();
        base.Eat();
    }
}