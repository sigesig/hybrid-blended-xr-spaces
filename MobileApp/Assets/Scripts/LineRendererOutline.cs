using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineRendererOutline : MonoBehaviour
{
    private LineRenderer lineRenderer;
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        Vector3[] vertices = GetComponent<MeshFilter>().sharedMesh.vertices;
        lineRenderer.SetPosition(0, transform.TransformPoint(vertices[0]));
        lineRenderer.SetPosition(1, transform.TransformPoint(vertices[10]));
        lineRenderer.SetPosition(3, transform.TransformPoint(vertices[110]));
        lineRenderer.SetPosition(2, transform.TransformPoint(vertices[120]));
    }
}
