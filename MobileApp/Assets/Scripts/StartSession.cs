using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Interaction.Toolkit.AR;
using Util;

public class StartSession : MonoBehaviour
{
    #region Private Serializable Fields
    [SerializeField] public Button startSessionBtn;
    [SerializeField] public Canvas sessionCanvas;
    [SerializeField] public TextMeshProUGUI roomNameInput;
    [SerializeField] public Transform contentHolderTransform;
    [SerializeField] public Button roomButtonPrefab;
    [SerializeField] public Canvas spaceCanvas;
    [SerializeField] public GameObject noConnection;
    [SerializeField] public GameObject connection;
    [SerializeField] public ARPlaneManager arPlaneManager;
    [SerializeField] public ARPlacementInteractable placementInteractable;
    #endregion

    #region MonoBehaviour CallBacks

    void Start()
    {
        Helpers.TogglePlaneDetection(arPlaneManager);
        startSessionBtn.onClick.AddListener(SwitchToSpaceCreation);
        Networking.Connect("KekW");
        if (!Networking.IsConnected())
        {
            connection.gameObject.SetActive(false);
            noConnection.gameObject.SetActive(true);
        }
        //PhotonNetwork.CreateRoom("test", new RoomOptions());
    }
    
    void Update()
    {
        InvokeRepeating(nameof(NoMasterConnection), 1.0f, 1.0f);
        bool connectionStatus = Networking.IsConnected();
        if (connectionStatus)
        {
            InvokeRepeating(nameof(CreateRoomList), 1.0f, 5.0f);
        }
    }
    
    #endregion
    
    
    #region Private Methods
    /// <summary>
    /// Used by start session button to switch between Space creation canvas and the begin session canvas
    /// </summary>
    private void SwitchToSpaceCreation()
    {
        Networking.StartSession();
        //Networking.CreateRoom(roomNameInput.text);
        placementInteractable.gameObject.SetActive(true);
        Helpers.TogglePlaneDetection(arPlaneManager);
        sessionCanvas.gameObject.SetActive(false);
        
        spaceCanvas.gameObject.SetActive(true);
    }
    /// <summary>
    /// Used to switch canvas when no access to master server is available 
    /// </summary>
    private void NoMasterConnection()
    {
        if (!Networking.IsConnected())
        {
            connection.gameObject.SetActive(false);
            noConnection.gameObject.SetActive(true);
        }
        else
        {
            connection.gameObject.SetActive(true);
            noConnection.gameObject.SetActive(false);
            //Networking.JoinLobby(); 
        }
    }
    
    /// <summary>
    /// To make sure input isn't empty
    /// </summary>
    /// <returns>true if not empty else false</returns>
    private bool IsValidRoomName()
    {
        return roomNameInput.text != " ";
    }

    private void CreateRoomList()
    {
        var rooms = Networking.GetRooms();
        foreach (RoomInfo room in rooms)
        {
            Button button = Instantiate(roomButtonPrefab, contentHolderTransform);
            button.GetComponentInChildren<TextMeshProUGUI>().text = room.Name;
        }
        
    }

    #endregion
}
