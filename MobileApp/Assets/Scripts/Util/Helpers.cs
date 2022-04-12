using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

namespace Util
{
    public static class Helpers
    {
        /*
         * Used to toggle the plane detection on and off
         */
        public static void TogglePlaneDetection(ARPlaneManager aRPlaneManager)
        {
            aRPlaneManager.enabled = !aRPlaneManager.enabled;

            foreach (ARPlane plane in aRPlaneManager.trackables)
            {
                plane.gameObject.SetActive(aRPlaneManager.enabled);
            }
        }
    }
}