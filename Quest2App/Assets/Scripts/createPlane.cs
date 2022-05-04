using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class createPlane : MonoBehaviour
{



    public GameObject creatingPlane;
    public GameObject planePrefab;
    public GameObject cornor;

    private GameObject s1;
    private GameObject s2;
    private GameObject s3;

    private float minSize = 0.2f;
    private OVRSkeleton bone;
    private GameObject plane;

    private float fingerDist;
    private bool hasInstantiated = false;

    // Start is called before the first frame update
    void Start()
    {
        bone = GetComponent<OVRSkeleton>();
        fingerDist = minSize;
    }

    void InstantiatePrefabs(Transform position) {

        s1 = Instantiate(cornor, position.position, Quaternion.identity) as GameObject;
        s1.transform.position = position.position;

        s3 = Instantiate(cornor, s1.transform.position, Quaternion.identity) as GameObject;
        s3.transform.position = new Vector3(s1.transform.position.x + minSize, s1.transform.position.y, s1.transform.position.z);

        s2 = Instantiate(cornor, s1.transform.position, Quaternion.identity) as GameObject;
        s2.transform.position = new Vector3(s1.transform.position.x, s1.transform.position.y, s1.transform.position.z + 0.1f);

        InstantiatePlane();

        hasInstantiated = true;
    }

    void dragSelect()
    {

        Transform thumbBone = bone.Bones[(int)OVRPlugin.BoneId.Hand_Thumb3].Transform;
        Transform indexBone = bone.Bones[(int)OVRPlugin.BoneId.Hand_Index3].Transform;

        if (s1 == null && !hasInstantiated)
        {
            InstantiatePrefabs(thumbBone);
        }

        s2.SetActive(false);//Hide third cornor

        Collider planeCollider2 = plane.transform.GetChild(0).GetComponent<Collider>(); //get collider of plane
        //if finger position is inside plane collider: set scale z
        if (planeCollider2.bounds.Contains(indexBone.position)) {
            fingerDist = Vector3.Distance(s1.transform.position, indexBone.position);
        }

        UpdatePosition();
        InstantiatePlane();
        SetPlaneRotation();
    }

    void SetPlaneRotation() {

        //Calc and set rotation of workspace
        Vector3 s1s3 = s3.transform.position - s1.transform.position;
        float angle = Vector3.Angle(s1s3, plane.transform.right); //Angle between plane x and s1s3 vector
        //adjust if rotate to negative value
        Vector3 cross = Vector3.Cross(s1s3, plane.transform.right);
        if (cross.y < 0)
            angle = -angle;

        plane.transform.Rotate(0, -angle, 0, Space.Self); //rotate plane
    }

    void UpdatePosition() {

        s1.transform.position = new Vector3(s1.transform.position.x, s1.transform.position.y, s1.transform.position.z);
        s2.transform.position = new Vector3(s1.transform.position.x, s1.transform.position.y, s2.transform.position.z);
        s3.transform.position = new Vector3(s3.transform.position.x, s1.transform.position.y, s3.transform.position.z);

        s1.transform.rotation = Quaternion.LookRotation(plane.transform.forward);
        s3.transform.rotation = Quaternion.LookRotation(plane.transform.forward);

    }

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

            dragSelect();
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


    void InstantiatePlane()
    {

        //Delete old workspace
        if (plane != null)
            Destroy(plane);

        //Calc scale for height and width based on set corner points
        float scaleX = Vector3.Distance(s1.transform.position, s3.transform.position);
        float scaleZ = fingerDist;

        //Spawn prefab of workplace
        plane = Instantiate(planePrefab, s1.transform.position, Quaternion.identity) as GameObject;

        //Create workspace of right size and put plane little lover than cornor objects
        Vector3 newScale = new Vector3(scaleX, plane.transform.localScale.y, scaleZ);
        plane.transform.localScale = newScale;
    }
}
