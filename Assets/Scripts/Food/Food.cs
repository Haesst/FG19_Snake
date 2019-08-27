using UnityEngine;
using UnityEngine.Assertions;

public class Food : MonoBehaviour
{
    [SerializeField] int points = 10;
    [SerializeField] int growthAmount = 1;
    [SerializeField] string gameControllerTag = "GameController";

    protected static GameController gameController;

    public int Points { get => points; }
    public int GrowthAmount { get => growthAmount; }

    private void Awake()
    {
        if (gameController == null)
        {
            gameController = GameObject.FindGameObjectWithTag(gameControllerTag).GetComponent<GameController>();
        }
        Assert.IsNotNull(gameController, "Could not find GameController");
    }

    public virtual void Eat()
    {
        Destroy(gameObject);
    }
}