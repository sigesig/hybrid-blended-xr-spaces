using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OculusSampleFramework;

public class HTGrabber : OVRGrabber
{
    private OVRHand hand;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        hand = GetComponent<OVRHand>();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        CheckIndexPinch();
        
    }

    void CheckIndexPinch() {

        bool isPinch = hand.GetFingerIsPinching(OVRHand.HandFinger.Index);

        if (!m_grabbedObj && isPinch && m_grabCandidates.Count >0)
            GrabBegin();
        else if (m_grabbedObj && !isPinch)
            GrabEnd();
    }

    protected override void GrabEnd()
    {
        if (m_grabbedObj)
        {
            Vector3 lineraVelocity = (transform.position - m_lastPos) / Time.fixedDeltaTime;
            Vector3 angularVelocity = (transform.eulerAngles - m_lastRot.eulerAngles) / Time.fixedDeltaTime;

            GrabbableRelease(lineraVelocity, angularVelocity);

        }
        //if (rb != null && rb.isKinematic == true)
        //{
        //    rb.isKinematic = false;
        //}

        GrabVolumeEnable(true);
    }

}
