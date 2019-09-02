using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public const int NORTH = 0;
    public const int EAST = 1;
    public const int SOUTH = 2;
    public const int WEST = 3;

    private int direction;
    private int desiredDirection;
    private int lastDirection;

    public int Direction { get => direction; set => direction = value; }
    public int DesiredDirection { get => desiredDirection; set => desiredDirection = value; }
    public int LastDirection { get => lastDirection; set => lastDirection = value; }

    private void Awake()
    {
        direction = Random.Range(0, 4);
    }

    private void GetPlayerInput()
    {
        if (Input.GetAxisRaw("Vertical") > 0 && lastDirection != SOUTH)
            direction = NORTH;
        else if (Input.GetAxisRaw("Vertical") < 0 && lastDirection != NORTH)
            direction = SOUTH;
        else if (Input.GetAxisRaw("Horizontal") > 0 && lastDirection != WEST)
            direction = EAST;
        else if (Input.GetAxisRaw("Horizontal") < 0 && lastDirection != EAST)
            direction = WEST;
    }

    private void Update()
    {
        GetPlayerInput();
    }
}