using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

/// <summary>
/// Handles the sync of the laser pointer line
/// </summary>
public class LaserSync : MonoBehaviourPun, IPunObservable
{
    private LineRenderer _lineRenderer;
    // Start is called before the first frame update
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    
    /// <summary>
    /// Used to update the line
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="info"></param>
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if(stream.IsWriting) {
            var array = new Vector3[2];
            _lineRenderer.GetPositions(array);
            stream.SendNext(array);
        }
    }
}
