using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public const int NORTH = 0;
    public const int EAST = 1;
    public const int SOUTH = 2;
    public const int WEST = 3;

    private int direction;
    public int Direction { get => direction; }
    private float baseMoveSpeed = 0.6f;
    public float BaseMoveSpeed { get => baseMoveSpeed; }
    public float timer;

    private void Awake()
    {
        direction = Random.Range(0, 4);
    }
    private void Update()
    {
        PlayerMovement();
    }

    private void PlayerMovement()
    {
        if (Input.GetAxisRaw("Vertical") > 0 && direction != SOUTH)
            direction = NORTH;
        else if (Input.GetAxisRaw("Vertical") < 0 && direction != NORTH)
            direction = SOUTH;
        else if (Input.GetAxisRaw("Horizontal") > 0 && direction != WEST)
            direction = EAST;
        else if (Input.GetAxisRaw("Horizontal") < 0 && direction != EAST)
            direction = WEST;
    }
}