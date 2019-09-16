using UnityEngine;

public class Food : MonoBehaviour
{
    /// <summary>
    /// This is a base class that every food item
    /// is created from. I wanted it to be easy to add
    /// other food items that could be spawned like power-ups
    /// or extra points.
    /// </summary>
    [SerializeField] private int score = 10;
    [SerializeField] private int growAmount = 1;
    [SerializeField] private float timeRemoval = 0.01f;

    public int GrowAmount { get => growAmount; }
    public float TimeRemoval { get => timeRemoval; }

    virtual public void Eat()
    {
        GameController.Instance.AddScore(score);
    }
}