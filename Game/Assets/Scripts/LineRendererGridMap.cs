using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererGridMap : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float gridSize = 1.0f;
    public int gridWidth = 7;
    public int gridHeight = 7;

    void Start()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();

        lineRenderer.positionCount = (gridWidth + 1) * 2 + (gridHeight + 1) * 2;

        int index = 0;

        // Horizontal lines
        for (int y = 0; y <= gridHeight; y++)
        {
            lineRenderer.SetPosition(index++, new Vector3(0, y * gridSize, 0));
            lineRenderer.SetPosition(index++, new Vector3(gridWidth * gridSize, y * gridSize, 0));
        }

        // Vertical lines
        /*for (int x = 0; x <= gridWidth; x++)
        {
            lineRenderer.SetPosition(index++, new Vector3(x * gridSize, 0, 0));
            lineRenderer.SetPosition(index++, new Vector3(x * gridSize, gridHeight * gridSize, 0));
        }*/
    }
}

