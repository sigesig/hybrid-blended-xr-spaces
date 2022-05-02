using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerAlignmentController : MonoBehaviourPunCallbacks
{

    private GameObject planeObject;

    public void SignalReady()
    {

        //Transform temporaryPlane = transform.GetComponent<ManipulatePlane>().EndDefinePhase();

        //  base plane
        //planeObject = (GameObject)PhotonNetwork.Instantiate("DeskPlaneInteractible", temporaryPlane.position, temporaryPlane.rotation * Quaternion.Euler(0, 180, 0), 0) ;
        //planeObject.transform.localScale = (temporaryPlane.transform.localScale/50);
        planeObject = (GameObject)PhotonNetwork.Instantiate("DeskPlaneInteractible", new Vector3(0,-5,0), Quaternion.identity * Quaternion.Euler(0, 180, 0), 0) ;
        Debug.Log("Created");
        //planeObject.transform.localScale = (temporaryPlane.transform.localScale/50);
        //Destroy(temporaryPlane.gameObject);
        // Move plane
        PlaneAlignment.MovePlaneToCenter(planeObject.transform, transform);
        Debug.Log("Created plane and moved to center");
        

        // Flip such that players are in front of each other
        if(!PhotonNetwork.IsMasterClient) PlaneAlignment.FlipPosition(planeObject.transform, transform, 180);

    }

}
