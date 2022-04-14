using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class createPlaneAll : MonoBehaviour
{
    private OVRHand hand;
    private OVRSkeleton bone;

    //public GameObject canvas;
    public GameObject creatingPlane;

    private bool isMiddlePinch;
    private bool isRingPinch;
    private bool isPinkyPinch;


    public GameObject c1;
    public GameObject c2;
    public GameObject c3;

    private GameObject s1;
    private GameObject s2;
    private GameObject s3;


    private bool hasInstantiated = false;

    public GameObject pp;
    private GameObject plane;

    private float planeHeight;
    private float minSize = 0.1f;


    public enum PlaneDefiner // Dropdown in inspector
    {
        multiFingerPinchz,
        oneFingerSelectz,
        scaleSelectz

    };

    public PlaneDefiner planedefinerStrat;


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("feta createplane");
        hand = GetComponent<OVRHand>();
        bone = GetComponent<OVRSkeleton>();

    }


    //set point using one hand and multiple fingers to pinch
    void multiFingerPinch()
    {

        isMiddlePinch = hand.GetFingerIsPinching(OVRHand.HandFinger.Middle);

        isRingPinch = hand.GetFingerIsPinching(OVRHand.HandFinger.Ring);

        isPinkyPinch = hand.GetFingerIsPinching(OVRHand.HandFinger.Pinky);

        Transform thumbBone = bone.Bones[(int)OVRPlugin.BoneId.Hand_Thumb3].Transform;

        if (isMiddlePinch)
        {

            if (s1 == null)
            {
                s1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            }

            setCoordinatex(s1, thumbBone);

            if (s1 != null && s2 != null && s3 != null)
                definePlane();


        }
        else if (isRingPinch)
        {

            if (s2 == null)
            {
                s2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            }

            setCoordinatex(s2, thumbBone);

            if (s1 != null && s2 != null && s3 != null)
                definePlane();


        }
        else if (isPinkyPinch)
        {

            if (s3 == null)
            {
                s3 = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            }

            setCoordinatex(s3, thumbBone);

            if (s1 != null && s2 != null && s3 != null)
                definePlane();


        }

    }

    void oneFingerSelect()
    {

        isMiddlePinch = hand.GetFingerIsPinching(OVRHand.HandFinger.Middle);
        Transform thumbBone = bone.Bones[(int)OVRPlugin.BoneId.Hand_Thumb3].Transform;
        Transform indexBone = bone.Bones[(int)OVRSkeleton.BoneId.Hand_Index3].Transform;


        if (isMiddlePinch)
        {

            if (s1 == null)
            {
                s1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            }

            destroyPlane();
            Destroy(s2);
            Destroy(s3);

            setCoordinatex(s1, thumbBone);

            planeHeight = s1.transform.position.y;
        }

        //is first cornor set and the indexfinger at the height of the plane
        if (s1 != null && !isMiddlePinch && indexBone.position.y <= planeHeight)
        {

            float offsetZ = Mathf.Abs(s1.transform.position.z - indexBone.position.z);
            float offsetX = Mathf.Abs(s1.transform.position.x - indexBone.position.x);

            //is the finger outside the minimum size of the plane and on the width or depth?
            if (offsetZ > minSize && offsetZ > offsetX && s2 == null)
            {

                s2 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);

                setCoordinatex(s2, indexBone);

            }
            else if (offsetX > minSize && offsetX > offsetZ && s3 == null)
            {

                s3 = GameObject.CreatePrimitive(PrimitiveType.Cube);

                setCoordinatex(s3, indexBone);

            }

        }


        if (s1 != null && s2 != null && s3 != null)
        {
            definePlane();

        }
    }

    void scaleSelect()
    {

        if (s1 == null && !hasInstantiated)
        {
            Transform thumbBone = bone.Bones[(int)OVRPlugin.BoneId.Hand_Thumb3].Transform;
            s1 = Instantiate(c1, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            setCoordinatex(s1, thumbBone);
            s2 = Instantiate(c2, s1.transform.position, Quaternion.identity) as GameObject;

            s2.transform.position = new Vector3(s1.transform.position.x, s1.transform.position.y, s1.transform.position.z + minSize);

            s3 = Instantiate(c3, s1.transform.position, Quaternion.identity) as GameObject;

            s3.transform.position = new Vector3(s1.transform.position.x + minSize, s1.transform.position.y, s1.transform.position.z);

            definePlane();

            hasInstantiated = true;

        }

        s2.transform.position = new Vector3(s2.transform.position.x, s1.transform.position.y, s2.transform.position.z);
        s3.transform.position = new Vector3(s3.transform.position.x, s1.transform.position.y, s3.transform.position.z);

        definePlane();
    }




    // Update is called once per frame
    void Update()
    {

        if (creatingPlane.activeSelf)
        { // is active

            if (hasInstantiated)
            {
                s1.SetActive(true);
                s2.SetActive(true);
                s3.SetActive(true);
            }

            //multiFingerPinch();
            //oneFingerSelect();
            //scaleSelect();
            runPlaneDefiner();
        }
        else
        {
            if (hasInstantiated)
            {
                s1.SetActive(false);
                s2.SetActive(false);
                s3.SetActive(false);
            }
        }
    }

    public GameObject GetPlane()
    {

        return plane;
    }

    //Run the planedefiner strategy based on the dropdown on the inspector
    void runPlaneDefiner()
    {


        switch (planedefinerStrat)
        {
            case PlaneDefiner.multiFingerPinchz:
                multiFingerPinch();
                break;
            case PlaneDefiner.oneFingerSelectz:
                oneFingerSelect();
                break;
            case PlaneDefiner.scaleSelectz:
                scaleSelect();
                break;
            default:
                scaleSelect();
                break;
        }
    }

    void destroyPlane()
    {

        if (plane != null)
            Destroy(plane);

    }



    void definePlane()
    {

        //Delete old workspace
        destroyPlane();

        //Calc scale for height and width based on set corner points
        float scaleX = Vector3.Distance(s1.transform.position, s3.transform.position);
        float scaleZ = Vector3.Distance(s1.transform.position, s2.transform.position);

        Debug.Log("feta scaleX:" + scaleX);
        Debug.Log("feta scaleZ:" + scaleZ);

        //Spawn prefab of workplace
        plane = Instantiate(pp, s1.transform.position, Quaternion.identity) as GameObject;

        //Create workspace of right size and put plane little lover than cornor objects
        Vector3 newScale = new Vector3(scaleX, plane.transform.localScale.y, scaleZ);
        plane.transform.localScale = newScale;

        //Calc and set rotation of workspace
        Vector3 s1s2 = s2.transform.position - s1.transform.position;
        float angle = Vector3.Angle(s1s2, plane.transform.forward); //Angle between plane z and s1s2 vector

        //adjust if rotate to negative value
        Vector3 cross = Vector3.Cross(s1s2, plane.transform.forward);
        if (cross.y < 0)
            angle = -angle;

        plane.transform.Rotate(0, -angle, 0, Space.Self); //rotate plane

    }

    void setCoordinatex(GameObject s, Transform finger)
    {
        //setPoint based position on tip of finger
        var tip = finger.position;

        s.transform.position = tip;
        //s.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
    }

}
