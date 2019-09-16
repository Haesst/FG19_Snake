using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public const int NORTH = 0;
    public const int EAST = 1;
    public const int SOUTH = 2;
    public const int WEST = 3;

    public int Direction { get; private set; }
    public int LastDirection { get; set; }

    private void Update()
    {
        ReadInput();
    }

    /// <summary>
    /// Read the players input and set direction accordingly.
    /// </summary>
    private void ReadInput()
    {
        float movementX = Input.GetAxisRaw("Horizontal");
        float movementY = Input.GetAxisRaw("Vertical");

        if (movementX != 0)
        {
            // Make sure that we're not trying to go directly from
            // right to left and vice versa.
            if (movementX > 0 && LastDirection != WEST)
            {
                Direction = EAST;
            }
            else if (movementX < 0 && LastDirection != EAST)
            {
                Direction = WEST;
            }
        }
        else if (movementY != 0)
        {
            // Make sure that we're not trying to go directly from
            // up to down and vice versa.
            if (movementY > 0 && LastDirection != SOUTH)
            {
                Direction = NORTH;
            }
            else if (movementY < 0 && LastDirection != NORTH)
            {
                Direction = SOUTH;
            }
        }
    }

    /// <summary>
    /// Set the next direction for the snake.
    /// </summary>
    /// <param name="direction">Direction to move in.</param>
    public void SetAIMovement(int direction)
    {
        Direction = direction;
    }
}