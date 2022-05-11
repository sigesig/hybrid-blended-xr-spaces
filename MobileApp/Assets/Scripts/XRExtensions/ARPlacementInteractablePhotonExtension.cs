using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Interaction.Toolkit.AR;

/// <summary>
/// Own implementation of ARPlacementInteractable such that it can spawn photon objects instead of normal objects
/// </summary>
public class ARPlacementInteractablePhotonExtension : ARBaseGestureInteractable
{
    [SerializeField] private GameObject placementPrefab;
    
    private List<GameObject> placedObjects = new List<GameObject>();

    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private static GameObject trackablesObject;

    protected override bool CanStartManipulationForGesture(TapGesture gesture)
    {
        if (gesture.targetObject == null)
        {
            return true;
        }
        return false;
    }

    protected override void OnEndManipulation(TapGesture gesture)
    {
        if (gesture.isCanceled)
        {
            return;
        }

        if (gesture.targetObject != null)
        {
            return;
        }

        if (GestureTransformationUtility.Raycast(gesture.startPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            var hit = hits[0];

            if (Vector3.Dot(Camera.main.transform.position - hit.pose.position, hit.pose.rotation * Vector3.up) < 0)
            {
                return;
            }

            var placedObject = PhotonNetwork.Instantiate("NetworkCube", hit.pose.position, hit.pose.rotation);
            placedObjects.Add(placedObject);
            
            var anchorObject = new GameObject("PlacementAnchor");
            anchorObject.transform.position = hit.pose.position;
            anchorObject.transform.rotation = hit.pose.rotation;

            if (trackablesObject == null)
            {
                trackablesObject = GameObject.Find("Trackables");
            }

            if (trackablesObject != null)
            {
                anchorObject.transform.parent = trackablesObject.transform;
            }
            
        }
    }
}
