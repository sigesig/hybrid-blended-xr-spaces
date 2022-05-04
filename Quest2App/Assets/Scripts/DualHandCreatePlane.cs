using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DualHandCreatePlane : MonoBehaviour
{

    [SerializeField] GameObject pivotPoint;
    [SerializeField] GameObject cornerPoint;

    private OVRHand leftHand;
    private OVRSkeleton leftBone;
    private OVRHand rightHand;
    private OVRSkeleton rightBone;

    // Position of bottom left corner
    private Transform planeTransform;
    //private float planeHeight;
    //private float planeWidth;

    private GameObject plane;

    private List<GameObject> placedPoints;
    private LineRenderer lineRenderer;

    private float timer = 1.0f;
    private float waitTime = 1.0f;

    private void Start()
    {
        leftHand = transform.GetChild(0).Find("LeftHandAnchor").GetComponentInChildren<OVRHand>();
        leftBone = transform.GetChild(0).Find("LeftHandAnchor").GetComponentInChildren<OVRSkeleton>();

        rightHand = transform.GetChild(0).Find("RightHandAnchor").GetComponentInChildren<OVRHand>();
        rightBone = transform.GetChild(0).Find("RightHandAnchor").GetComponentInChildren<OVRSkeleton>();

        lineRenderer = GetComponent<LineRenderer>();
        placedPoints = new List<GameObject>();
    }
    private void Update()
    {

            DragSelect();
        

        timer += Time.deltaTime;
    }


    private void DragSelect()
    {
        if(placedPoints.Count < 2)
        {
            // Create line between fingers * Change this to translate increased length *
            lineRenderer.SetPosition(0, leftBone.Bones[(int)OVRPlugin.BoneId.Hand_IndexTip].Transform.position);
            lineRenderer.SetPosition(1, rightBone.Bones[(int)OVRPlugin.BoneId.Hand_IndexTip].Transform.position);

            // If either hand pinches -> Create points
            if (leftHand.GetFingerIsPinching(OVRHand.HandFinger.Index) || leftHand.GetFingerIsPinching(OVRHand.HandFinger.Index))
            {
                // Retrieve vector between fingers
                Vector3 widthVector = (leftBone.Bones[(int)OVRPlugin.BoneId.Hand_IndexTip].Transform.position - rightBone.Bones[(int)OVRPlugin.BoneId.Hand_IndexTip].Transform.position)*2;
            }
        }

        }
    }

