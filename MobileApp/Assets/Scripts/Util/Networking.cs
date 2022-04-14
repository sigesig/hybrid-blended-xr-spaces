using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Networking : MonoBehaviour
{
    static string gameVersion = "1";
    
    public bool JoinRoom()
    {
        return true;
    }
    
    public bool CreateRoom()
    {
        return true;
    }
    
    /*
     * Connect to Pun2 and returns true if this was successful else false
     */
    public static bool Connect()
    {
        // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("TEST");
            return true;
        }
        else
        {
            var result = PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
            return result;
        }
    }
}
