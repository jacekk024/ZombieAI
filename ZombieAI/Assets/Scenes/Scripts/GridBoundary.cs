using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBoundary : MonoBehaviour
{
    public testGenerator mapGenerator;

    void Start()
    {
        Vector2 gridSize = mapGenerator.gridSize;
        Vector2 gridWorldSize = new Vector2(gridSize.x * 42, gridSize.y * 42);

        GameObject boundary = new GameObject("Boundary");
        BoxCollider boundaryCollider = boundary.AddComponent<BoxCollider>();

        boundary.transform.position = new Vector3(-21, 0, -21);
        boundaryCollider.size = new Vector3(gridWorldSize.x, 1, gridWorldSize.y);
    }
}
