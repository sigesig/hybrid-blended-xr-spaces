using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AR;

[RequireComponent(typeof(ARGestureInteractor))]
public class SpaceCreationGestures : MonoBehaviour
{
    private ARGestureInteractor _arGestureInteractor;
    void Start()
    {
        _arGestureInteractor = GetComponent<ARGestureInteractor>();

        _arGestureInteractor.dragGestureRecognizer.onGestureStarted += DragGestureRecognizerStarted;
    }
    
    void Update()
    {
        
    }


    private void DragGestureRecognizerStarted(Gesture<DragGesture> dragGesture)
    {
        dragGesture.onStart += (s) =>
        {
            Debug.Log("Drag started");
        };
        
        dragGesture.onUpdated += (s) =>
        {
            Debug.Log("Drag Updated");
        };
        
        dragGesture.onFinished += (s) =>
        {
            Debug.Log("Drag finished");
        };
        
    }
}
