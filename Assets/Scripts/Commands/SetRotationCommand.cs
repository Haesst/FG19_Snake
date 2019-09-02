using UnityEngine;

public class SetRotationCommand : ICommand
{
    int rotationAngleX;
    int rotationAngleY;
    int rotationAngleZ;
    Transform transform;

    public SetRotationCommand(int rotationAngleX, int rotationAngleY, int rotationAngleZ, Transform transform)
    {
        this.rotationAngleX = rotationAngleX;
        this.rotationAngleY = rotationAngleY;
        this.rotationAngleZ = rotationAngleZ;
        this.transform = transform;
    }
    public void ExecuteAction()
    {
        transform.rotation = Quaternion.Euler(rotationAngleX, rotationAngleY, rotationAngleZ);
    }
}