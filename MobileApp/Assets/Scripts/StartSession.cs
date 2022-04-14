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
    [SerializeField] public Canvas noConnectionCanvas;
    [SerializeField] public ARPlaneManager arPlaneManager;
    [SerializeField] public ARPlacementInteractable placementInteractable;

    private bool _connectionStatus;
    void Start()
    {
        Helpers.TogglePlaneDetection(arPlaneManager);
        startSessionBtn.onClick.AddListener(SwitchToSpaceCreation);
        _connectionStatus = Networking.Connect();
        Debug.Log(_connectionStatus);
        if (!_connectionStatus)
        {
            sessionCanvas.gameObject.SetActive(false);
            noConnectionCanvas.gameObject.SetActive(true);
        }
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
