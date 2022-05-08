using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Interaction.Toolkit.AR;
using Util;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

/// <summary>
/// This is used by the create space canvas. i.e. used when the mobile user is defining the space
/// </summary>
public class CreateSpace : MonoBehaviour
{
    #region Serialized Fields
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
    [SerializeField] public GameObject corner;
    [SerializeField] public GameObject planePrefab;
    [SerializeField] public ARGestureInteractor gestureInteractable;
    #endregion
    
    #region Private variables
    private GameObject _plane;
    private GameObject _depthPointGameObject;
    private List<GameObject> _placedPoints = new List<GameObject>();
    private bool _depthPhaseRunning = false;
    private bool _positionChangeDirectionUp;

    //Used for handling the resize 
    private ARRaycastManager _arRaycastManager;
    private Vector3 _initialTouch;
    private GameObject _planeObjectPhoton;
    private bool _isScaling = false;
    private Vector3 _currentUpVector3;
    private float _initialDistanceBetween;
    
    #endregion
    
    private void Awake()
    {
        _arRaycastManager = GetComponent<ARRaycastManager>();
    }

    
    void Start()
    {
        gestureInteractable.dragGestureRecognizer.onGestureStarted += DragGestureRecognizerStarted;
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
        IsPlaneCreationPossible(numberOfPositions);
        CanDeletePreviousPoint(numberOfPositions);
        // Will handle plane create
        if (_placedPoints.Count < 2) return;
        if (!_depthPhaseRunning)
        {
            _depthPhaseRunning = StartDepthSelection();
        }
        SetRotation();
        if (_isScaling)
        {
            ResizePlane();
        }
    }

    #region Plane creation functions

    private void DrawLine(ARObjectPlacementEventArgs args)
    {
        _placedPoints.Add(args.placementObject);
        lineRenderer.positionCount++;
        var pointIndex = lineRenderer.positionCount - 1;
        lineRenderer.SetPosition(pointIndex, args.placementObject.transform.position);
    }
    
    private void DragGestureRecognizerStarted(Gesture<DragGesture> dragGesture)
    {
        const float acceleration = 0.01f;
        if (_plane == null)
        {
            return;
        }
        dragGesture.onStart += (s) =>
        {
            Debug.Log("Drag started");
        };
        
        dragGesture.onUpdated += (s) =>
        {
            if (!_isScaling)
            {
                _initialDistanceBetween = s.position.y - s.startPosition.y; //greater than 0 is up and less than zero is down
                _isScaling = !Mathf.Approximately(_initialDistanceBetween, 0);
            }
            else
            {
                var currentDistanceBetween = s.position.y - s.startPosition.y;
                if (currentDistanceBetween > 0)
                {
                    _positionChangeDirectionUp = true;
                }
                else
                {
                    _positionChangeDirectionUp = false;
                }
                var scaleFactor = currentDistanceBetween / _initialDistanceBetween;
                if (_positionChangeDirectionUp)
                {
                    _depthPointGameObject.transform.position += _depthPointGameObject.transform.forward * Time.deltaTime * scaleFactor * acceleration;
                }
                else
                {
                    _depthPointGameObject.transform.position -= _depthPointGameObject.transform.forward * Time.deltaTime * scaleFactor * acceleration;
                }
            }
        };
        
        dragGesture.onFinished += (s) =>
        {
            _isScaling = false;
        };
        
    }

    private void ResizePlane()
    {
        Vector3 startPoint = _placedPoints[0].transform.position;
        Vector3 endPoint = _placedPoints[1].transform.position;

        float planeWidth = Vector3.Distance(startPoint, endPoint);
        float planeHeight = Vector3.Distance(_placedPoints[2].transform.position, _depthPointGameObject.transform.position);
        Debug.Log("Height is: " + planeHeight);
        _plane.transform.localScale = new Vector3((planeWidth * 10), _plane.transform.localScale.y ,(planeHeight * 10) );

        _plane.transform.position =
            _placedPoints[0].transform.position
            + ((planeHeight * -_plane.transform.forward) / 2)
            + ((planeWidth * -_plane.transform.right) / 2);
    }
    
    /** Sets the rotation of all objects in the forward direction created by the two placed points*/
    private void SetRotation()
    {
        if (_placedPoints.Count >= 2)
        {
            Vector3 directionalVector = Vector3.RotateTowards(_placedPoints[0].transform.forward, (_placedPoints[0].transform.position - _placedPoints[1].transform.position), 1, 1);
            _placedPoints[0].transform.rotation = Quaternion.LookRotation(directionalVector);
            _placedPoints[1].transform.rotation = Quaternion.LookRotation(directionalVector);
            _placedPoints[2].transform.rotation = Quaternion.LookRotation(directionalVector);
            _depthPointGameObject.transform.rotation = Quaternion.LookRotation(directionalVector) * Quaternion.Euler(0, -90, 0);
            _plane.transform.rotation = Quaternion.LookRotation(directionalVector) * Quaternion.Euler(0, -90, 0);
        }
    }


    private bool StartDepthSelection()
    {   
        _placedPoints[0].transform.rotation = Quaternion.Euler(0,0,0);
        _placedPoints[1].transform.rotation = Quaternion.Euler(0,0,0);
        _depthPointGameObject = Instantiate(corner, Vector3.Lerp(_placedPoints[0].transform.position, _placedPoints[1].transform.position, 0.5f), Quaternion.Euler(90, 0, 0));
        _depthPointGameObject.transform.position = new Vector3(_depthPointGameObject.transform.position.x, _depthPointGameObject.transform.position.y, _depthPointGameObject.transform.position.z + 0.1f);
        _placedPoints.Add(Instantiate(corner, Vector3.Lerp(_placedPoints[0].transform.position, _placedPoints[1].transform.position, 0.5f), Quaternion.Euler(90, 0, 0)));
        _plane = Instantiate(planePrefab, _placedPoints[0].transform);
        //_placedPoints[0].GetComponent<MeshRenderer>().enabled = false;
        //_placedPoints[1].GetComponent<MeshRenderer>().enabled = false;
        //_placedPoints[2].GetComponent<MeshRenderer>().enabled = false;
        lineRenderer.enabled = true;
        placementInteractable.gameObject.SetActive(false);
        SetRotation();
        ResizePlane();
        return true;
    }

    private void CreateNetworkConnectedPlane()
    {
        Transform temporaryPlane = EndDefinePhase();
        
        _planeObjectPhoton = (GameObject)PhotonNetwork.Instantiate("DeskPlaneInteractible", temporaryPlane.position, temporaryPlane.rotation * Quaternion.Euler(0, 180, 0), 0) ;
        _planeObjectPhoton.transform.localScale = (temporaryPlane.transform.localScale/50);
        Destroy(temporaryPlane.gameObject);
        
        // Move plane
        
        PhotonUtil.PlaneAlignment.MovePlaneToCenter(_planeObjectPhoton.transform, transform);

        // Flip such that players are in front of each other
        if(!PhotonNetwork.IsMasterClient) PhotonUtil.PlaneAlignment.FlipPosition(_planeObjectPhoton.transform, transform, 180);
    }
    
    private Transform EndDefinePhase()
    {
        Transform planeCopy = _plane.transform;
        //Destroy(this);
        //Destroy(depthDrag);
        return planeCopy;
    }
    

    /*
     * Used to toggle interactable of creation button
     */
    private void IsPlaneCreationPossible(int numberOfPoints)
    {
        if (numberOfPoints >= 2)
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
    

    #endregion

    #region Buttons control

    /*
    * Used by create plane button to create mesh from placed points
    */
    private void CreatePlane()
    {
        Helpers.TogglePlaneDetection(arPlaneManager);
        CreateNetworkConnectedPlane();
        lineRenderer.positionCount = 0;
        RemoveAllPoints();
        spaceCanvas.gameObject.SetActive(false);
        currentSession.gameObject.SetActive(true);
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
        if (_plane != null)
        {
            Destroy(_plane);
        }
        RemoveAllPoints();
    }
    
    /*
    * Used by delete last point button to removed the last placed point
    */
    private void DeleteLastPlacedPoint()
    {
        lineRenderer.positionCount = 0;
        if (_plane != null)
        {
            Destroy(_plane);
        }
        RemoveAllPoints();
        lineRenderer.enabled = true;
        placementInteractable.gameObject.SetActive(true);
        _depthPhaseRunning = false;


        /* USEFULL if we want to make deletelast point actually delete last point and not interative 
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
        */
    }

    #endregion
    
}
