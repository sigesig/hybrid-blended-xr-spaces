using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace PhotonUtil
{
    /// <summary>
    /// Used to align the plane correctly in the space
    /// </summary>
    public class PlaneAlignment : MonoBehaviour
    {
        /// <summary>
        /// Used to move the planes to the correct position in the shared space, so it looks correct
        /// </summary>
        /// <param name="planeTransform"></param>
        /// <param name="playerTransform"></param>
        public static void MovePlaneToCenter(Transform planeTransform, Transform playerTransform)
        {
            var planePos = planeTransform.position;
            planeTransform.position = new Vector3(0,0,0);
            playerTransform.parent.position = -planePos;
        }
        
        /// <summary>
        /// changes the position of the player in respect to the planes
        /// </summary>
        /// <param name="planeTransform"></param>
        /// <param name="playerTransform"></param>
        /// <param name="degrees"></param>
        public static void FlipPosition(Transform planeTransform, Transform playerTransform, float degrees)
        {
            playerTransform.parent = planeTransform;
            planeTransform.rotation = Quaternion.Euler(new Vector3(planeTransform.rotation.x, planeTransform.rotation.y + degrees, planeTransform.rotation.z));

            playerTransform.parent = null;

        }
    }
}