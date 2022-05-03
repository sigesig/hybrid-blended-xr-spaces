using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Interaction.Toolkit.AR;
using Util;

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
    #endregion
    
    #region Private variables
    private GameObject _plane;
    private List<GameObject> _placedPoints = new List<GameObject>();
    private bool _depthPhaseRunning = false;

    //Used for handling the resize 
    private ARRaycastManager _arRaycastManager;
    private Touch _initialTouch;
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
        
        // Will handle plane create
        if (_placedPoints.Count != 2) return;
        if (!_depthPhaseRunning)
        {
            _depthPhaseRunning = StartDepthSelection();
        }
        ChangeDepthGesture();
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

    private void ChangeDepthGesture()
    {
        if (Input.touchCount == 1)
        {
            var touch = Input.GetTouch(0);
            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            if (touch.phase == TouchPhase.Began)
            {
                _initialTouch = touch;
            }

            if (touch.phase == TouchPhase.Moved)
            {
                if (!_isScaling)
                {
                    _initialDistanceBetween = touch.position.y - _initialTouch.position.y; //greater than 0 is up and less than zero is down
                    _currentUpVector3 = _placedPoints[3].transform.up;
                    _isScaling = !Mathf.Approximately(_initialDistanceBetween, 0);
                }
                else
                {
                    var currentDistanceBetween = touch.position.y - _initialTouch.position.y;
                    var scaleFactor = currentDistanceBetween / _initialDistanceBetween;
                    _plane.transform.up = _currentUpVector3 * scaleFactor;
                }
            }
            else
            {
                _isScaling = false;
            }
        }
    }
    
    private void ResizePlane()
    {
        Vector3 startPoint = _placedPoints[0].transform.position;
        Vector3 endPoint = _placedPoints[1].transform.position;

        float planeWidth = Vector3.Distance(startPoint, endPoint);
        float planeHeight = Vector3.Distance(_placedPoints[2].transform.position, _placedPoints[3].transform.position);
        _plane.transform.localScale = new Vector3((planeWidth * 5) - 0.02f, 1.0f, (planeHeight * 5) - 0.02f);

        _plane.transform.position =
            _placedPoints[0].transform.position
            + ((planeHeight * -_plane.transform.forward) / 2)
            + ((planeWidth * -_plane.transform.right) / 2);
    }

    private bool StartDepthSelection()
    {
        Vector3 between = _placedPoints[0].transform.position - _placedPoints[1].transform.position;
        Vector3 newPoint = Vector3.Cross(Vector3.up, between);
        _placedPoints.Add(Instantiate(corner, newPoint, Quaternion.Euler(90, 0, 0)));
        _plane = Instantiate(planePrefab, _placedPoints[0].transform);
        //_placedPoints[0].GetComponent<MeshRenderer>().enabled = false;
        //_placedPoints[1].GetComponent<MeshRenderer>().enabled = false;
        //_placedPoints[2].GetComponent<MeshRenderer>().enabled = false;
        lineRenderer.enabled = false;
        placementInteractable.gameObject.SetActive(false);
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
