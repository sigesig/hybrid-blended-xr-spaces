using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.AR;

public class CreateSpace : MonoBehaviour
{
    // Buttons variables
    [SerializeField] public Button exitSpaceCreationBtn;
    [SerializeField] public Button deleteLastPointBtn;
    [SerializeField] public Button createPointBtn;
    [SerializeField] public Button createPlaneBtn;
    // Canvases variables
    [SerializeField] public Canvas sessionCanvas;
    [SerializeField] public Canvas spaceCanvas;
    [SerializeField] public Canvas currentSession;
    // Space Variables
    [SerializeField] public LineRenderer lineRenderer;
    [SerializeField] public ARPlacementInteractable placementInteractable;

    void Start()
    {
        //Space creation setup
        placementInteractable.gameObject.SetActive(true);
        placementInteractable.objectPlaced.AddListener(DrawLine);
        //Buttons setup
        exitSpaceCreationBtn.onClick.AddListener(StopSpaceCreation);
        deleteLastPointBtn.onClick.AddListener(DeleteLastPlacedPoint);
        createPointBtn.onClick.AddListener(PlacePoint);
        createPlaneBtn.onClick.AddListener(CreatePlane);
    }
    

    void Update()
    {
        
    }


    private void DrawLine(ARObjectPlacementEventArgs args)
    {
        lineRenderer.positionCount++;
        var pointIndex = lineRenderer.positionCount - 1;
        lineRenderer.SetPosition(pointIndex, args.placementObject.transform.position);
    }
    
    /*
    * Used by exit button for switching between Space creation canvas and the begin session canvas
    */
    private void StopSpaceCreation()
    {
        spaceCanvas.gameObject.SetActive(false);
        sessionCanvas.gameObject.SetActive(true);
        placementInteractable.gameObject.SetActive(false);
    }
    
    /*
    * Used by create plane button to create mesh from placed points
    */
    private void CreatePlane()
    {
        spaceCanvas.gameObject.SetActive(false);
        currentSession.gameObject.SetActive(true);
        if (lineRenderer.positionCount >= 3)
        {
            //CreateMesh();
        }
    }
    
    /*
    * Used by delete last point button to removed the last placed point
    */
    private void DeleteLastPlacedPoint()
    {
        var newPositionCount = lineRenderer.positionCount - 1;
        Vector3[] newPositions = new Vector3[newPositionCount];
        for (int i = 0; i < newPositionCount; i++)
        {
            var newIndex = i + 1;
            newPositions[i] = lineRenderer.GetPosition(newIndex);
        }
        lineRenderer.SetPositions(newPositions);
    }
    
    /*
    * Used by place point button to place a point
    */
    private void PlacePoint()
    {
        throw new System.NotImplementedException();
    }
}
