using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateSpace : MonoBehaviour
{
    // Buttons
    [SerializeField] public Button exitSpaceCreationBtn;
    [SerializeField] public Button deleteLastPointBtn;
    [SerializeField] public Button createPointBtn;
    [SerializeField] public Button createPlaneBtn;
    // Canvases
    [SerializeField] public Canvas sessionCanvas;
    [SerializeField] public Canvas spaceCanvas;
    void Start()
    {
        exitSpaceCreationBtn.onClick.AddListener(StopSpaceCreation);
        deleteLastPointBtn.onClick.AddListener(DeleteLastPlacedPoint);
        createPointBtn.onClick.AddListener(PlacePoint);
        createPlaneBtn.onClick.AddListener(CreatePlane);
    }
    

    void Update()
    {
        
    }
    
    /*
    * Used by exit button for switching between Space creation canvas and the begin session canvas
    */
    private void StopSpaceCreation()
    {
        spaceCanvas.gameObject.SetActive(false);
        sessionCanvas.gameObject.SetActive(true);
    }
    

    private void CreatePlane()
    {
        throw new System.NotImplementedException();
    }
    
    private void DeleteLastPlacedPoint()
    {
        throw new System.NotImplementedException();
    }

    private void PlacePoint()
    {
        throw new System.NotImplementedException();
    }
}
