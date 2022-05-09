using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace PhotonUtil
{
    public class PlaneAlignment : MonoBehaviour
    {
        [SerializeField] Transform spawnDestination;
        [SerializeField] Transform moveDestination;
        [SerializeField] ARSessionOrigin origin;
    

        public static void MovePlaneToCenter(Transform planeTransform, Transform playerTransform)
        {
            var planePos = new Vector3(planeTransform.position.x - 10000, planeTransform.position.y,
                planeTransform.position.z);
            planeTransform.position = new Vector3(0,0,0);
            playerTransform.parent.position = -planePos;
            /*
            playerTransform.parent = planeTransform;
            planeTransform.position = new Vector3(0, 0, 0);
            planeTransform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            playerTransform.parent = null;
            */

        }

        public static void FlipPosition(Transform planeTransform, Transform playerTransform, float degrees)
        {
            playerTransform.parent = planeTransform;
            planeTransform.rotation = Quaternion.Euler(new Vector3(planeTransform.rotation.x, planeTransform.rotation.y + degrees, planeTransform.rotation.z));

            playerTransform.parent = null;

        }
    }
}