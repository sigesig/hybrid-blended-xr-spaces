using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerAlignmentController : MonoBehaviourPunCallbacks
{

    private GameObject planeObject;
    private GameObject cubeObject;

    public void SignalReady()
    {

        Transform temporaryPlane = transform.GetComponent<ManipulatePlane>().EndDefinePhase();

        //  base plane
        planeObject = (GameObject)PhotonNetwork.Instantiate("DeskPlaneInteractible", temporaryPlane.position, temporaryPlane.rotation * Quaternion.Euler(0, 180, 0), 0) ;
        planeObject.transform.localScale = (temporaryPlane.transform.localScale/50);
        Destroy(temporaryPlane.gameObject);
        // Move plane
        PlaneAlignment.MovePlaneToCenter(planeObject.transform, transform);

        // Flip such that players are in front of each other
        if(!PhotonNetwork.IsMasterClient) PlaneAlignment.FlipPosition(planeObject.transform, transform, 180);

    }

}
