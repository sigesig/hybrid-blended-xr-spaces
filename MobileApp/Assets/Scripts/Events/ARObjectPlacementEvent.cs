using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class ARObjectPlacementEvent : UnityEvent<ARPlacementInteractablePhotonExtension, GameObject> { }
