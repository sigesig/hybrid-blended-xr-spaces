using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Text;

public class ConsoleText : MonoBehaviourPunCallbacks
{

    [SerializeField] GameObject networkText;
    [SerializeField] GameObject debugLogText;

    private List<string> logOutput;
    void Start()
    {
        Application.logMessageReceived += LogCallback;
    }

    void Update()
    {
        networkText.GetComponent<Text>().text = GetNetworkStatus();
        debugLogText.GetComponent<Text>().text = GetLogOutput();

    }

    private string GetNetworkStatus()
    {
        bool isConnected = PhotonNetwork.IsConnectedAndReady;
        bool isMaster = PhotonNetwork.IsMasterClient;
        int connectedToServer = PhotonNetwork.CountOfPlayers;

        StringBuilder s = new StringBuilder();
        s.AppendLine("Connected to server: " + isConnected);
        s.AppendLine("Is Master client: " + isMaster);
        s.AppendLine("Players connected: " + connectedToServer);

        return s.ToString();
    }

    private string GetLogOutput()
    {
        StringBuilder s = new StringBuilder();
        s.AppendLine(Application.identifier);
        s.AppendLine(Application.version);

        //foreach (string log in logOutput)
       // {
       //     s.AppendLine(log);
       // }

        return s.ToString();
    }
    public void LogCallback(string logString, string stackTrace, LogType type) {
        logOutput.Add(logString);
        if (logOutput.Count > 8)
        {
            logOutput.RemoveAt(0);
        }
    }

}
