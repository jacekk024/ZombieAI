using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBoundary : MonoBehaviour
{
    public testGenerator mapGenerator;
    public GameObject heatmap;
    private float sizeOfGrid = 21.0f;

    void Start()
    {
        Vector2 gridSize = mapGenerator.gridSize;
        Vector2 gridWorldSize = new Vector2(gridSize.x * sizeOfGrid * 2, gridSize.y * sizeOfGrid * 2);

        GameObject boundary = new GameObject("Boundary");
        BoxCollider boundaryCollider = boundary.AddComponent<BoxCollider>();

        boundary.transform.position = new Vector3(sizeOfGrid * -1, 0.0f, sizeOfGrid * -1);
        boundaryCollider.size = new Vector3(gridWorldSize.x, 1, gridWorldSize.y);

        heatmap.transform.localScale = new Vector3(gridSize.x * 4.2f, 1.0f, gridSize.y * 4.2f);
        heatmap.transform.position = new Vector3(sizeOfGrid * -1, 0, sizeOfGrid * -1);
        heatmap.GetComponent<RaycastGradient>().minX = (sizeOfGrid * gridSize.x * -1) - sizeOfGrid;
        heatmap.GetComponent<RaycastGradient>().minZ = (sizeOfGrid * gridSize.y * -1) - sizeOfGrid;
        heatmap.GetComponent<RaycastGradient>().maxX = (sizeOfGrid * gridSize.x) - sizeOfGrid;
        heatmap.GetComponent<RaycastGradient>().maxZ = (sizeOfGrid * gridSize.y) - sizeOfGrid;
    }
}
