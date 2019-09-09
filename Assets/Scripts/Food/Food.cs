using UnityEngine;

public class Food : MonoBehaviour
{
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