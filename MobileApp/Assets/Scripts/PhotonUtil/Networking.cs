using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

/// <summary>
/// Handles most of the Photon networking
/// </summary>
public class Networking : MonoBehaviourPunCallbacks
{
    [SerializeField] public Camera ARCamera;
    
    private static readonly string _gameVersion = "1";
    private static bool _isConnected = false;
    private static List<RoomInfo> _rooms = new List<RoomInfo>();
    
    private static GameObject _line;
    private LineRenderer lineRenderer;
    private bool _isInRoom = false;
    private GameObject _avatar;
    
    void Update()
    {
        if(_isInRoom) {
            _avatar.transform.position = ARCamera.transform.position;
            _avatar.transform.rotation = ARCamera.transform.rotation;
        }
    }
    
    #region Public Methods
    /// <summary>
    /// Used to join a Photon room
    /// </summary>
    /// <param name="roomName">Then room to join</param>
    /// <returns>True if successful else false</returns>
    public static bool JoinRoom(string roomName)
    {
        return PhotonNetwork.JoinRoom(roomName);
    }
    
    /// <summary>
    /// Creates a photon room for the session
    /// </summary>
    /// <param name="roomName"> name of the session/room</param>
    /// <param name="roomOptions">The photon room options for this room</param>
    /// <returns>True if successful else false</returns>
    public static bool CreateRoom(string roomName, RoomOptions roomOptions = null)
    {
        return PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    /// <summary>
    /// Joins the main lobby
    /// </summary>
    /// <returns>True if successful else false</returns>
    public static bool JoinLobby()
    {
        return PhotonNetwork.JoinLobby();
    }
    
    /// <summary>
    /// Leaves the room if currently connected to one
    /// </summary>
    /// <returns>True or false depending on if it was succesfull</returns>
    public static bool LeaveRoom()
    {
        return PhotonNetwork.LeaveRoom();
    }

    /// <summary>
    /// Connect to Pun
    /// </summary>
    /// <param name="userName">The name you are joining with</param>
    public static void Connect(string userName)
    {
        try
        {
            PhotonNetwork.NickName = userName; //we can use a input to change this 
            PhotonNetwork.AutomaticallySyncScene = false; //To call PhotonNetwork.LoadLevel()
            PhotonNetwork.GameVersion = _gameVersion; // Set game version such that only people with same version can play together
            PhotonNetwork.ConnectUsingSettings();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
    
    /// <summary>
    /// Get the instantiated laser pointer line
    /// </summary>
    /// <returns>the laser line gameobject</returns>
    public static GameObject GetLaserLine()
    {
        return _line;
    }

    
    public override void OnJoinedRoom()
    {
        Debug.Log("Master: " + PhotonNetwork.IsMasterClient + " | Players In Room: " + PhotonNetwork.CurrentRoom.PlayerCount + " | RoomName: " + PhotonNetwork.CurrentRoom.Name + " Region: " + PhotonNetwork.CloudRegion);
        _avatar = PhotonNetwork.Instantiate("CubeAvatar", ARCamera.transform.position, ARCamera.transform.rotation);
        _line = PhotonNetwork.Instantiate("Laser", new Vector3(0,0,0), Quaternion.identity);
        //line.SetActive(false);
        _isInRoom = true;
    }
    
    /// <summary>
    /// Tells if the device is connected to the master server 
    /// </summary>
    /// <returns>True if connected else false</returns>
    public static bool IsConnected()
    {
        return _isConnected;
    }
    
    /// <summary>
    /// Get the list of available rooms 
    /// </summary>
    /// <returns>List of rooms</returns>
    public static List<RoomInfo> GetRooms()
    {
        return _rooms;
    }
    
    /// <summary>
    /// Tries to join a random room, if this fail then because of PUN callback(FailedtojoinRandomROom)it will create its own room
    /// </summary>
    public static void StartSession()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    /// <summary>
    /// Gets the name of the connected room if connected to a room
    /// </summary>
    /// <returns>returns the room name else returns no room</returns>
    public static string GetJoinedRoomName()
    {
        if (PhotonNetwork.InRoom)
        {
            return PhotonNetwork.CurrentRoom.Name;
        }

        return "No Room";
    }
    #endregion
    
    #region MonoBehaviourPunCallbacks Callbacks


    public override void OnConnectedToMaster()
    {
        _isConnected = true;
        Debug.Log("OnConnectedToMaster() was called by PUN.");
        //CreateRoom("Tests");
    }


    public override void OnDisconnected(DisconnectCause cause)
    {
        _isConnected = false;
        Debug.LogWarningFormat("OnDisconnected() was called by PUN with reason {0}", cause);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("Rooms was updated");
        _rooms = roomList;
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("I JUST JOINED THE LOBBY");
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("I JUST CREATED A ROOM");
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        PhotonNetwork.CreateRoom("Test", new RoomOptions());
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        PhotonNetwork.Destroy(_avatar);
        PhotonNetwork.Destroy(_line);
        _avatar = PhotonNetwork.Instantiate("CubeAvatar", ARCamera.transform.position, ARCamera.transform.rotation);
        _line = PhotonNetwork.Instantiate("Laser", new Vector3(0,0,0), Quaternion.identity);
    }
    #endregion
}
