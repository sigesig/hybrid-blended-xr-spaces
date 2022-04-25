using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManipulatePlaneDepth : MonoBehaviour
{
    private ManipulatePlane manipulatePlane;

    void Start()
    {
        manipulatePlane = GameObject.Find("OVRNetworkCameraRig").GetComponent<ManipulatePlane>();
    }


    // When this collider touches a trigger - Call Manipulateplane for reaction
    private void OnTriggerStay(Collider other)
    {
        manipulatePlane.DefinePlaneDepth(other);
    }
}
