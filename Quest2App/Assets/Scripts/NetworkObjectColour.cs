using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkObjectColour : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int colorCode = GetComponent<PhotonView>().ViewID % 6;
        GetComponent<Renderer>().material.SetColor("_Color", DefineObjectColor(colorCode));
    }

    private Color DefineObjectColor(int colorCode)
    {
        switch (colorCode)
        {
            case 0:
                return Color.white;
            case 1:
                return Color.blue;
            case 2:
                return Color.red;
            case 3:
                return Color.green;
            case 4:
                return Color.yellow;
            case 5:
                return Color.magenta;
            default:
                return Color.black;
        }
    }
}
