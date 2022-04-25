using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class createPlaneDrag : MonoBehaviour
{
    private OVRHand hand;
    private OVRSkeleton bone;

    //public GameObject canvas;
    public GameObject creatingPlane;
    private GameObject plane;
    public GameObject workspace;

    private GameObject ws;

    private GameObject s1;
    private GameObject s2;
    private GameObject s3;


    private bool hasInstantiated = false;

    private float minSize = 0.2f;


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("feta createplane");
        hand = GetComponent<OVRHand>();
        bone = GetComponent<OVRSkeleton>();

    }

    // Update is called once per frame
    void Update()
    {
        if (creatingPlane.activeSelf)
        {
            makePlane();
        }
    }

    void makePlane() {

        if (!hasInstantiated)
        {
            Transform thumbBone = bone.Bones[(int)OVRPlugin.BoneId.Hand_Thumb3].Transform;

            ws = Instantiate(workspace, thumbBone.position, Quaternion.identity) as GameObject;

            plane = ws.transform.GetChild(0).gameObject;

            s1 = ws.transform.GetChild(1).gameObject;
            s2 = ws.transform.GetChild(2).gameObject;
            s3 = ws.transform.GetChild(3).gameObject;

            s2.transform.localPosition = new Vector3(0, 0, minSize);
            s3.transform.localPosition = new Vector3(minSize, 0, 0);

            hasInstantiated = true;
        }





        if (plane != null)
            Destroy(plane);

        
        s2.transform.localPosition = new Vector3(s2.transform.localPosition.x, s1.transform.localPosition.y, s2.transform.localPosition.z);
        s3.transform.localPosition = new Vector3(s3.transform.localPosition.x, s1.transform.localPosition.y, s3.transform.localPosition.z);

        //float scaleX = Mathf.Abs(s3.transform.position.x);
        //float scaleZ = Mathf.Abs(s2.transform.position.z);

        Debug.Log("feta tansformed cornors plane");
        float scaleX = Mathf.Abs(s1.transform.localPosition.x - s3.transform.localPosition.x);
        float scaleZ = Mathf.Abs(s1.transform.localPosition.z - s2.transform.localPosition.z);

        //Spawn prefab of workplace
        //ws = Instantiate(plane, s1.transform.position, Quaternion.identity) as GameObject;
        Debug.Log("feta instantiated plane");
        //ws.transform.parent = s1.transform;

        Vector3 newScale = new Vector3(scaleX, plane.transform.localScale.y, scaleZ);
        plane.transform.localScale = newScale;
        Debug.Log("feta scaled plane");

        Vector3 s1s2 = s2.transform.position - s1.transform.position;
        float angle = Vector3.Angle(s1s2, transform.forward); //Angle between plane z and s1s2 vector

        //adjust if rotate to negative value
        Vector3 cross = Vector3.Cross(s1s2, transform.forward);
        if (cross.y < 0)
            angle = -angle;

        ws.transform.Rotate(0, -angle, 0, Space.Self); //rotate plane
        //s1.transform.Rotate(0, -angle, 0, Space.Self); //rotate plane

    }
}
