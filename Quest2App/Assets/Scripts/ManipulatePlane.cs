using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManipulatePlane : MonoBehaviour
{

    [SerializeField] GameObject pivotPoint;
    [SerializeField] GameObject cornerPoint;
    [SerializeField] GameObject depthTexture;
    [SerializeField] GameObject indexPoint;
    
    private OVRHand leftHand;
    private OVRSkeleton leftBone;    
    private OVRHand rightHand;
    private OVRSkeleton rightBone;

    private GameObject plane;
    private GameObject depthDrag;

    private List<GameObject> placedPoints;
    private LineRenderer lineRenderer;

    private float timer = 1.0f;
    private float waitTime = 1.0f; // Minimum wait time before multiple pinch gestures

    private GameObject indexLeft;
    private GameObject indexRight;

    private void Start()
    {
        leftHand = GameObject.FindWithTag("LeftHand").GetComponentInChildren<OVRHand>();
        leftBone = GameObject.FindWithTag("LeftHand").GetComponentInChildren<OVRSkeleton>();
        rightBone = GameObject.FindWithTag("RightHand").GetComponentInChildren<OVRSkeleton>();
        rightHand = GameObject.FindWithTag("RightHand").GetComponentInChildren<OVRHand>();

        lineRenderer = GetComponent<LineRenderer>();
        placedPoints = new List<GameObject>();

        indexLeft = Instantiate(indexPoint, leftBone.Bones[(int)OVRPlugin.BoneId.Hand_IndexTip].Transform);
        indexRight = Instantiate(indexPoint, rightBone.Bones[(int)OVRPlugin.BoneId.Hand_IndexTip].Transform);
        indexRight.name = "indexRight";
        indexLeft.name = "indexLeft";

    }
    private void Update()
    {
        if (timer > waitTime)
        {
            DefineEdgePoints();
        }
      
        SetRotation();
        if (plane != null) ResizePlane();
        timer += Time.deltaTime;

        indexLeft.transform.position = leftBone.Bones[(int)OVRPlugin.BoneId.Hand_IndexTip].Transform.position;
        indexRight.transform.position = rightBone.Bones[(int)OVRPlugin.BoneId.Hand_IndexTip].Transform.position;
    }

    /** Listens for pinch gesture and creates a point */
    private void DefineEdgePoints()
    {
        // When pinching - Create new point
        if ((leftHand.GetFingerIsPinching(OVRHand.HandFinger.Index) || 
            rightHand.GetFingerIsPinching(OVRHand.HandFinger.Index)) && 
            placedPoints.Count < 2)
        {
            // Detect which finger is pinching
            Transform fingerTransform;
            if (leftHand.GetFingerIsPinching(OVRHand.HandFinger.Index)) { 
                fingerTransform = leftBone.Bones[(int)OVRPlugin.BoneId.Hand_IndexTip].Transform; 
            }
            else {
                fingerTransform = rightBone.Bones[(int)OVRPlugin.BoneId.Hand_IndexTip].Transform;
            }

            // Create point at index finger end
            GameObject point = Instantiate(cornerPoint, fingerTransform.position, Quaternion.Euler(0, 0, 0));
            placedPoints.Add(point);

            // If second point has been created - Go into depth and height phase
            if(placedPoints.Count == 2) InitiateDepthSelection();
           
            timer = 0;
        }
    }

    /** When two points have been placed, change some things to go to next phase of creation */
    private void InitiateDepthSelection()
    {
        placedPoints.Add(Instantiate(cornerPoint, Vector3.Lerp(placedPoints[0].transform.position, placedPoints[1].transform.position, 0.5f), Quaternion.Euler(90, 0, 0)));
        depthDrag = Instantiate(depthTexture, Vector3.Lerp(placedPoints[0].transform.position, placedPoints[1].transform.position, 0.5f), Quaternion.Euler(90, 0, 0));
        plane = Instantiate(pivotPoint, placedPoints[0].transform);
        depthDrag.transform.position = new Vector3(depthDrag.transform.position.x, depthDrag.transform.position.y, depthDrag.transform.position.z + 0.5f);
        placedPoints[0].GetComponent<MeshRenderer>().enabled = false;
        placedPoints[1].GetComponent<MeshRenderer>().enabled = false;
        placedPoints[2].GetComponent<MeshRenderer>().enabled = false;
        lineRenderer.enabled = false;
    }

    /** This function is called when index finger is colliding with table while defining depth*/
    public void DefinePlaneDepth(Collider other)
    {
        Transform collidingPoint = null;
        if (other.gameObject.name == indexLeft.name) collidingPoint = indexLeft.transform;
        else if (other.gameObject.name == indexRight.name) collidingPoint = indexRight.transform;

        Vector3 localPointVector = placedPoints[2].transform.InverseTransformVector(placedPoints[2].transform.position);
        Vector3 touchPointLocalVector = placedPoints[2].transform.InverseTransformVector(collidingPoint.position);
        float distance = Mathf.Abs((localPointVector - touchPointLocalVector).x) / 15;

        // Will lower plane on y-axis if to fit to table height (If finger is lower than plane)
        if (localPointVector.y > touchPointLocalVector.y)
        {
            depthDrag.transform.position = new Vector3(depthDrag.transform.position.x, collidingPoint.position.y + 0.2f, depthDrag.transform.position.z);
            placedPoints[0].transform.position = new Vector3(placedPoints[0].transform.position.x, collidingPoint.position.y, placedPoints[0].transform.position.z);
            placedPoints[1].transform.position = new Vector3(placedPoints[1].transform.position.x, collidingPoint.position.y, placedPoints[1].transform.position.z);
            placedPoints[2].transform.position = new Vector3(placedPoints[2].transform.position.x, collidingPoint.position.y, placedPoints[2].transform.position.z);
        }
        depthDrag.transform.position = placedPoints[2].transform.position + (placedPoints[2].transform.right * distance);
    }

    /** Defines the height, width and position of the plane from current information */
    private void ResizePlane()
    {
        Vector3 startPoint = placedPoints[0].transform.position;
        Vector3 endPoint = placedPoints[1].transform.position;

        float planeWidth = Vector3.Distance(startPoint, endPoint);
        float planeHeight = Vector3.Distance(placedPoints[2].transform.position, depthDrag.transform.position);
        plane.transform.localScale = new Vector3((planeWidth * 5) - 0.02f, 1.0f, (planeHeight * 5) - 0.02f);

        plane.transform.position =
            placedPoints[0].transform.position
            + ((planeHeight * -plane.transform.forward) / 2)
            + ((planeWidth * -plane.transform.right) / 2);
    }

    /** Sets the rotation of all objects in the forward direction created by the two placed points*/
    private void SetRotation()
    {
        if (placedPoints.Count >= 2)
        {
            Vector3 directionalVector = Vector3.RotateTowards(placedPoints[0].transform.forward, (placedPoints[0].transform.position - placedPoints[1].transform.position), 1, 1);
            placedPoints[0].transform.rotation = Quaternion.LookRotation(directionalVector);
            placedPoints[1].transform.rotation = Quaternion.LookRotation(directionalVector);
            placedPoints[2].transform.rotation = Quaternion.LookRotation(directionalVector);
            depthDrag.transform.rotation = Quaternion.LookRotation(directionalVector) * Quaternion.Euler(0, -90, 0);
            plane.transform.rotation = Quaternion.LookRotation(directionalVector) * Quaternion.Euler(0, -90, 0);
        }
    }

    public Transform EndDefinePhase()
    {
        Transform planeCopy = plane.transform;
        indexLeft.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
        indexRight.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
        //Destroy(this);
        //Destroy(depthDrag);
        return planeCopy;
    }
}
