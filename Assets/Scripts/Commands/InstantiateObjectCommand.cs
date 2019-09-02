using System.Collections.Generic;
using UnityEngine;

public class InstantiateObjectCommand : ICommand
{
    private Vector3 position;
    private GameObject prefab;
    private Quaternion rotationQuaternion;
    private Transform parent;
    private IObjectPlacer reference;

    public InstantiateObjectCommand(GameObject prefab, Vector3 position, IObjectPlacer reference = null)
    {
        this.prefab = prefab;
        this.position = position;
        rotationQuaternion = Quaternion.identity;
        parent = null;
        this.reference = reference;
    }
    public InstantiateObjectCommand(GameObject prefab, Vector3 position, Quaternion rotationQuaternion, Transform parent, IObjectPlacer reference = null)
    {
        this.prefab = prefab;
        this.position = position;
        this.rotationQuaternion = rotationQuaternion;
        this.parent = parent;
        this.reference = reference;
    }

    public void ExecuteAction()
    {
        GameObject gameObject = GameObject.Instantiate(prefab, position, rotationQuaternion, parent);
        if(reference != null)
        {
            reference.PlaceObjectCallback(gameObject);
        }
    }
}