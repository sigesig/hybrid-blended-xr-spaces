using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Interaction.Toolkit.AR;
using Util;

public class StartSession : MonoBehaviour
{
    [SerializeField] public Button startSessionBtn;
    [SerializeField] public Canvas sessionCanvas;
    [SerializeField] public Canvas spaceCanvas;
    [SerializeField] public ARPlaneManager arPlaneManager;
    [SerializeField] public ARPlacementInteractable placementInteractable;

    void Start()
    {
        Helpers.TogglePlaneDetection(arPlaneManager);
        startSessionBtn.onClick.AddListener(SwitchToSpaceCreation);
        
    }
    
    void Update()
    {
        
    }
    
    /*
     * Used by start session button to switch between Space creation canvas and the begin session canvas
     */
    private void SwitchToSpaceCreation()
    {
        placementInteractable.gameObject.SetActive(true);
        Helpers.TogglePlaneDetection(arPlaneManager);
        sessionCanvas.gameObject.SetActive(false);
        spaceCanvas.gameObject.SetActive(true);
    }
}
