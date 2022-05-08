using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit.AR;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    [SerializeField] ARRaycastManager m_RaycastManager;
    [SerializeField] public Camera ARCamera;
    List<ARRaycastHit> m_Hits = new List<ARRaycastHit>();
    private GameObject avatar;
    private GameObject line;
    private LineRenderer lineRenderer;
    private bool isInRoom = false;

    void Update() {
        if(isInRoom) {
            avatar.transform.position = ARCamera.transform.position;
            avatar.transform.rotation = ARCamera.transform.rotation;
            isPointingAtSomething();
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Master: " + PhotonNetwork.IsMasterClient + " | Players In Room: " + PhotonNetwork.CurrentRoom.PlayerCount + " | RoomName: " + PhotonNetwork.CurrentRoom.Name + " Region: " + PhotonNetwork.CloudRegion);
        avatar = PhotonNetwork.Instantiate("CubeAvatar", ARCamera.transform.position, ARCamera.transform.rotation);
        line = PhotonNetwork.Instantiate("Laser", new Vector3(0,0,0), Quaternion.identity);
        lineRenderer = line.GetComponent<LineRenderer>();
        //line.SetActive(false);
        isInRoom = true;
    }

    private void isPointingAtSomething() {
        if(Input.touchCount > 0) {
            Debug.Log("LASER: currently touching");
            if(m_RaycastManager.Raycast(Input.GetTouch(0).position, m_Hits)) {
                Debug.Log("LASER: hit an object: " + m_Hits[0]);
                lineRenderer.SetPosition(0, ARCamera.transform.position);
                lineRenderer.SetPosition(1, m_Hits[0].pose.position);
            }
        }
    }
}