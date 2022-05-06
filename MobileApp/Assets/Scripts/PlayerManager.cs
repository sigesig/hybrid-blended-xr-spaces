using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    ARRaycastManager m_RaycastManager;

    List<ARRaycastHit> m_Hits = new List<ARRaycastHit>();
    private GameObject avatar;
    [SerializeField]
    public Camera ARCamera;

    void Update() {
        Debug.Log("Avatar: " + avatar);
        avatar.transform.position = ARCamera.transform.position;
        avatar.transform.rotation = ARCamera.transform.rotation;
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Master: " + PhotonNetwork.IsMasterClient + " | Players In Room: " + PhotonNetwork.CurrentRoom.PlayerCount + " | RoomName: " + PhotonNetwork.CurrentRoom.Name + " Region: " + PhotonNetwork.CloudRegion);
        avatar = PhotonNetwork.Instantiate("CubeAvatar", ARCamera.transform.position, ARCamera.transform.rotation);
    }

    private bool isPointingAtSomething() {
        if(Input.touchCount == 1) {
            if(m_RaycastManager.Raycast(Input.GetTouch(0).position, m_Hits)) {
                return false;
            }
        }
        return false;
    }
}