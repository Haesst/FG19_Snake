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
        float movementX = Input.GetAxisRaw("Horizontal");
        float movementY = Input.GetAxisRaw("Vertical");

        if(movementX != 0)
        {
            if(movementX > 0 && LastDirection != WEST)
            {
                Direction = EAST;
            }
            else if(movementX < 0 && LastDirection != EAST)
            {
                Direction = WEST;
            }
        }
        else if (movementY != 0)
        {
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
    
    public void SetAIMovement(int direction)
    {
        Direction = direction;
    }
}