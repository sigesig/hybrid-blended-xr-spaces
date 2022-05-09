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
    [SerializeField] public ARPlacementInteractable placementInteractable;
    [SerializeField] public GameObject networkCube;
    [SerializeField] public ARSessionOrigin ARSessionOrigin;
    [SerializeField] public XRInteractionManager XRInteractionManager;
    // Buttons
    [SerializeField] public Button exitSession;
    [SerializeField] public Button laserPointer;
    [SerializeField] public TextMeshProUGUI lobbyLabel;
    
    #endregion

    #region Private variables
    private GameObject _laserLine;
    private LineRenderer _lineRenderer;
    private bool _laserPointerActive = false;
    private GameObject _originalPlacementPrefab;
        
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
        
        placementInteractable.gameObject.SetActive(true);
        _originalPlacementPrefab = placementInteractable.placementPrefab;
        placementInteractable.placementPrefab = networkCube;
        placementInteractable.objectPlaced.AddListener(SpawnObject);
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

    /*
     * End the current session. Used by the exit session button
     */
    private void EndSession()
    {
        Networking.LeaveRoom();
        currentSession.gameObject.SetActive(false);
        sessionCanvas.gameObject.SetActive(true);
        placementInteractable.placementPrefab = _originalPlacementPrefab;
        placementInteractable.gameObject.SetActive(false);
    }
    
    /*
     * Used by the laser pointer button
     */
    private void LaserPointerControl()
    {
        if (_laserPointerActive)
        {
            placementInteractable.gameObject.SetActive(_laserPointerActive);
            LaserPointerSwitchButton();
            laserPointer.GetComponent<Image>().color = Color.red;
            return;
        }
        placementInteractable.gameObject.SetActive(_laserPointerActive);
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

    private void TapGestureRecognizerStarted(Gesture<TapGesture> tapGesture)
    {
        tapGesture.onStart += (gesture) =>
        {
            Debug.Log("PLZ JUST WORK");
        };
        
    }

    private void SpawnObject(ARObjectPlacementEventArgs args)
    {
        var placedCubeTransform = args.placementObject.transform;
        var cube = PhotonNetwork.Instantiate("NetworkCube", placedCubeTransform.position, placedCubeTransform.rotation);
        Destroy(args.placementObject);
        cube.GetComponent<ARRotationInteractable>().gameObject.SetActive(true);
        cube.GetComponent<ARScaleInteractable>().gameObject.SetActive(true);
        cube.GetComponent<ARTranslationInteractable>().gameObject.SetActive(true);
        cube.GetComponent<ARRotationInteractable>().arSessionOrigin = ARSessionOrigin;
        cube.GetComponent<ARScaleInteractable>().arSessionOrigin = ARSessionOrigin;
        cube.GetComponent<ARTranslationInteractable>().arSessionOrigin = ARSessionOrigin;
        cube.GetComponent<ARRotationInteractable>().interactionManager = XRInteractionManager;
        cube.GetComponent<ARScaleInteractable>().interactionManager = XRInteractionManager;
        cube.GetComponent<ARTranslationInteractable>().interactionManager = XRInteractionManager;
    }

    private void LaserPointerSwitchButton()
    {
        _laserPointerActive = !_laserPointerActive;
        _laserLine.SetActive(_laserPointerActive);
    }

}
