using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.Interaction.Toolkit.AR;
using Util;

/// <summary>
/// This is used by the create space canvas. i.e. used when the mobile user is defining the space
/// </summary>
public class CreateSpace : MonoBehaviour
{
    // Buttons variables
    [SerializeField] public Button exitSpaceCreationBtn;
    [SerializeField] public Button deleteLastPointBtn;
    [SerializeField] public Button createPlaneBtn;
    // Canvases variables
    [SerializeField] public Canvas sessionCanvas;
    [SerializeField] public Canvas spaceCanvas;
    [SerializeField] public Canvas currentSession;
    // Space Variables
    [SerializeField] public LineRenderer lineRenderer;
    [SerializeField] public ARPlacementInteractable placementInteractable;
    [SerializeField] public ARPlaneManager arPlaneManager;

    private List<GameObject> _placedPoints = new List<GameObject>();
    void Start()
    {
        //Space creation setup
        placementInteractable.gameObject.SetActive(true);
        placementInteractable.objectPlaced.AddListener(DrawLine);
        //Buttons setup
        exitSpaceCreationBtn.onClick.AddListener(StopSpaceCreation);
        deleteLastPointBtn.onClick.AddListener(DeleteLastPlacedPoint);
        createPlaneBtn.onClick.AddListener(CreatePlane);
    }
    

    void Update()
    {
        var numberOfPositions = lineRenderer.positionCount;
        IsMeshCreationPossible(numberOfPositions);
        CanDeletePreviousPoint(numberOfPositions);
        
    }


    private void DrawLine(ARObjectPlacementEventArgs args)
    {
        _placedPoints.Add(args.placementObject);
        lineRenderer.positionCount++;
        var pointIndex = lineRenderer.positionCount - 1;
        lineRenderer.SetPosition(pointIndex, args.placementObject.transform.position);
    }
    
    /*
    * Used by exit button for switching between Space creation canvas and the begin session canvas
    */
    private void StopSpaceCreation()
    {
        Helpers.TogglePlaneDetection(arPlaneManager);
        lineRenderer.positionCount = 0;
        spaceCanvas.gameObject.SetActive(false);
        placementInteractable.gameObject.SetActive(false);
        sessionCanvas.gameObject.SetActive(true);
        RemoveAllPoints();
    }
    
    /*
    * Used by create plane button to create mesh from placed points
    */
    private void CreatePlane()
    {
        Helpers.TogglePlaneDetection(arPlaneManager);
        var createdMesh = CreateMesh();
        GameObject go = new GameObject();
        go.AddComponent<MeshFilter>();
        go.AddComponent<MeshRenderer>();
        go.GetComponent<MeshFilter>().mesh = createdMesh;
        go.GetComponent<Renderer>().material.color = Color.red;
        var instantiatedMesh = Instantiate(go);
        RemoveAllPoints();
        spaceCanvas.gameObject.SetActive(false);
        currentSession.gameObject.SetActive(true);
    }
    
    /*
    * Used by delete last point button to removed the last placed point
    */
    private void DeleteLastPlacedPoint()
    {
        GameObject pointObj = null;
        if (_placedPoints.Any())
        {
            int lastIndex = _placedPoints.Count - 1;
            pointObj = _placedPoints[lastIndex];
            Destroy(pointObj);
            _placedPoints.RemoveAt(lastIndex);
        }
        
        var newPositionCount = lineRenderer.positionCount - 1;
        Vector3[] newPositions = new Vector3[newPositionCount];
        for (int i = newPositionCount; i > 0; i--)
        {
            var newIndex = i - 1;
            newPositions[newIndex] = lineRenderer.GetPosition(newIndex);
        }
        lineRenderer.positionCount = newPositions.Length;
        lineRenderer.SetPositions(newPositions);
    }

    private Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        //GetComponent<MeshFilter>().mesh = mesh;
        Debug.Log("test1");
        List<Vector3> meshVertices = new List<Vector3>();
        List<int> meshIndices = new List<int>();
        Debug.Log("test2");
        // Generate get vertices
        List<Vector3> vertices = new List<Vector3>();
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            vertices.Add(lineRenderer.GetPosition(i));
        }

        Debug.Log("test3");
        //Convert Vector3[] to Vector2[]
        List<Vector2> vector2S = new List<Vector2>();
        foreach (var vertex in vertices)
        {
            vector2S.Add(vertex);
        }
        Debug.Log("test4");
        //Triangulate

        List<Triangulator.Triangle> triangles;
        try
        {
            triangles = Triangulator.Triangulate(vector2S);
            Triangulator.AddTrianglesToMesh(ref meshVertices,ref meshIndices, triangles, 0, true);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        //Add to mesh
        //mesh.vertices = meshVertices.ToArray();
        //mesh.triangles = meshIndices.ToArray();
        
        return mesh;
    }

    /*
     * Used to toggle interactable of creation button
     */
    private void IsMeshCreationPossible(int numberOfPoints)
    {
        if (numberOfPoints >= 3)
        {
            createPlaneBtn.interactable = true;
            return;
        }
        createPlaneBtn.interactable = false;
    }
    
    /*
     * Used to toggle interactable of delete button
     */
    private void CanDeletePreviousPoint(int numberOfPoints)
    {
        if (numberOfPoints >= 1)
        {
            deleteLastPointBtn.interactable = true;
            return;
        }
        deleteLastPointBtn.interactable = false;
    }

    private void RemoveAllPoints()
    {
        lineRenderer.positionCount = 0;
        foreach (GameObject pointObj in _placedPoints)
        {
            Destroy(pointObj);
        }
    }
}
