using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Voice.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.AR;

public class SessionInProgress : MonoBehaviour
{
    #region Serialized Fields
    // Canvases
    [SerializeField] public Canvas sessionCanvas;
    [SerializeField] public Canvas currentSession;
    //Gestures
    [SerializeField] public ARGestureInteractor gestureInteractable;
    [SerializeField] public ARRaycastManager raycastManager;
    [SerializeField] public Camera ARCamera;
    [SerializeField] public ARPlacementInteractablePhotonExtension placementInteractablePhoton;
    // Buttons
    [SerializeField] public Button exitSession;
    [SerializeField] public Button laserPointer;
    [SerializeField] public TextMeshProUGUI lobbyLabel;
    
    #endregion

    #region Private variables
    private GameObject _laserLine;
    private LineRenderer _lineRenderer;
    private bool _laserPointerActive = false;
        
    #endregion
    

    void Start()
    {
        lobbyLabel.text = "Lobby: " + Networking.GetJoinedRoomName();
        
        //Laser Handling
        _laserLine = Networking.GetLaserLine();
        _lineRenderer = _laserLine.GetComponent<LineRenderer>();
        laserPointer.onClick.AddListener(LaserPointerControl);
        _laserLine.SetActive(_laserPointerActive);
        laserPointer.GetComponent<Image>().color = Color.red;
        
        //Gestures
        gestureInteractable.dragGestureRecognizer.onGestureStarted += DragGestureRecognizerStarted;
        gestureInteractable.tapGestureRecognizer.onGestureStarted += TapGestureRecognizerStarted;
        
        placementInteractablePhoton.gameObject.SetActive(true);
        
        //Exit session
        exitSession.onClick.AddListener(EndSession);
    }

    private void Update()
    {
        if (_laserPointerActive)
        {
            _lineRenderer.SetPosition(0, ARCamera.transform.position);
        }
    }

    /// <summary>
    /// End the current session. Used by the exit session button
    /// </summary>
    private void EndSession()
    {
        Networking.LeaveRoom();
        currentSession.gameObject.SetActive(false);
        sessionCanvas.gameObject.SetActive(true);
        placementInteractablePhoton.gameObject.SetActive(false);
    }
    
    /// <summary>
    /// Used by the laser pointer button
    /// </summary>
    private void LaserPointerControl()
    {
        if (_laserPointerActive)
        {
            placementInteractablePhoton.gameObject.SetActive(_laserPointerActive);
            LaserPointerSwitchButton();
            laserPointer.GetComponent<Image>().color = Color.red;
            return;
        }
        placementInteractablePhoton.gameObject.SetActive(_laserPointerActive);
        LaserPointerSwitchButton();
        laserPointer.GetComponent<Image>().color = Color.green;
        

        //recorder.IsRecording = !_voiceChatIsMuted;
    }
    
    /// <summary>
    /// Handles all the gestures involving Drag
    /// </summary>
    /// <param name="dragGesture">The drag gestures it self, provided by the onGestureStarted self</param>
    private void DragGestureRecognizerStarted(Gesture<DragGesture> dragGesture)
    {

        dragGesture.onStart += (gesture) =>
        {
             HandleLaserPointer(gesture.position);
        };

        dragGesture.onUpdated += (gesture) =>
        {
            HandleLaserPointer(gesture.position);

        };
    }
    
    /// <summary>
    /// Controls the motion of the laserPointer
    /// </summary>
    /// <param name="gesturePosition"></param>
    private void HandleLaserPointer(Vector2 gesturePosition)
    {
        if (!_laserPointerActive) return;
        
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        if (raycastManager.Raycast(gesturePosition, hits, TrackableType.PlaneWithinPolygon))
        {
            _lineRenderer.SetPosition(0, ARCamera.transform.position);
            _lineRenderer.SetPosition(1, hits[0].pose.position);
        }
    }
    
    /// <summary>
    /// Handles all the gestures involving Drag
    /// </summary>
    /// <param name="tapGesture">The tap gestures it self, provided by the onGestureStarted self</param>
    private void TapGestureRecognizerStarted(Gesture<TapGesture> tapGesture)
    {
        tapGesture.onStart += (gesture) =>
        {
            Debug.Log("PLZ JUST WORK");
        };
        
    }
    
    /// <summary>
    /// Toggles the Laser pointer on/off
    /// </summary>
    private void LaserPointerSwitchButton()
    {
        _laserPointerActive = !_laserPointerActive;
        _laserLine.SetActive(_laserPointerActive);
    }

}
