using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class SetAvatarColor : MonoBehaviour
{
    
    private MeshRenderer meshRenderer;

    void Start() {
        meshRenderer = GetComponent<MeshRenderer>();
        SetColourScheme();
    }
    private void SetColourScheme()
    {
        if (gameObject.GetPhotonView().AmOwner)
        {
            meshRenderer.material.color = new Color(0.34f, 0.76f, 0.31f);
            //GetComponentInChildren<Image>().color = new Color(0.34f, 0.76f, 0.31f, 0f); // Currently invisible surface
        } else
        {
            meshRenderer.material.color = new Color(0.31f, 0.32f, 0.73f);
            //GetComponentInChildren<Image>().color = new Color(0.31f, 0.32f, 0.73f, 0f); // Currently invisible surface
        }
    }
}
