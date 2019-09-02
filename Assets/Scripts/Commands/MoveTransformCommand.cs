using UnityEngine;

public class MoveTransformCommand : ICommand
{
    private Vector3 toPosition;
    private Transform transform;
    private Quaternion rotation;

    public MoveTransformCommand(Transform transform, Vector3 toPosition)
    {
        this.toPosition = toPosition;
        this.transform = transform;
    }

    public MoveTransformCommand(Transform transform, Vector3 toPosition, Quaternion rotation)
    {
        this.toPosition = toPosition;
        this.transform = transform;
        this.rotation = rotation;
    }

    public void ExecuteAction()
    {
        transform.position = toPosition;
        if (rotation != null)
        {
            transform.rotation = rotation;
        }
    }
}