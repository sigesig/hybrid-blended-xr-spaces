using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the colors of user, so their lines looks different
/// </summary>
public class SetLineColor : MonoBehaviour
{
    private LineRenderer lineRenderer;

    void Start() {
        lineRenderer = GetComponent<LineRenderer>();
        SetColourScheme();
    }
    /// <summary>
    /// Sets the color of the line
    /// </summary>
    private void SetColourScheme()
    {
        if (gameObject.GetPhotonView().AmOwner)
        {

            lineRenderer.material.color = new Color(0.34f, 0.76f, 0.31f);
            GetComponentInChildren<Image>().color = new Color(0.34f, 0.76f, 0.31f, 0f); // Currently invisible surface
        } else
        {
            lineRenderer.material.color = new Color(0.31f, 0.32f, 0.73f);
            GetComponentInChildren<Image>().color = new Color(0.31f, 0.32f, 0.73f, 0f); // Currently invisible surface
        }
    }
}
