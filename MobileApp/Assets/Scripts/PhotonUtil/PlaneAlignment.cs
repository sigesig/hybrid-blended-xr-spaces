using UnityEngine;

namespace PhotonUtil
{
    public class PlaneAlignment : MonoBehaviour
    {
        [SerializeField] Transform spawnDestination;
        [SerializeField] Transform moveDestination;
    

        public static void MovePlaneToCenter(Transform photonPlaneTransform, Transform playerTransform, Transform planeTransform)
        {
            playerTransform.parent = planeTransform;
            photonPlaneTransform.position = new Vector3(0, 0, 0);
            photonPlaneTransform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            playerTransform.parent = null;

        }

        public static void FlipPosition(Transform planeTransform, Transform playerTransform, float degrees)
        {
            playerTransform.parent = planeTransform;
            planeTransform.rotation = Quaternion.Euler(new Vector3(planeTransform.rotation.x, planeTransform.rotation.y + degrees, planeTransform.rotation.z));

            playerTransform.parent = null;

        }
    }
}