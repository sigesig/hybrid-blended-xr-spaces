using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class LaserSync : MonoBehaviourPun, IPunObservable
{
    private LineRenderer _lineRenderer;
    // Start is called before the first frame update
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();  

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if(stream.IsReading) {
            _lineRenderer.gameObject.SetActive(true); 
            var array = (Vector3[]) stream.ReceiveNext();
            _lineRenderer.SetPosition(0, array[0]);
            _lineRenderer.SetPosition(1, array[1]);
        }
    }
}
