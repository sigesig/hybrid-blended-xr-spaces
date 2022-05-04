using System.Collections;
using UnityEngine;

/// <summary>
/// This class is in charge of setting the rotation and position of the avatar's parent transform, which is useful when we want to implement locomotion.
/// </summary>
public class AvatarParent : MonoBehaviour
{
    private Vector3 targetPos = Vector3.zero;
    private float targetRot = 0;

    public void SetTargetPosRot(float[] pos, float rot)
    {
        targetPos = new Vector3(pos[0], pos[1], pos[2]);
        targetRot = rot;
    }

    private void Update()
    {
        if(targetPos != Vector3.zero)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, 0.1f);
        }
        if(targetRot != 0)
        {
            transform.rotation = Quaternion.Euler(Vector3.Lerp(transform.rotation.eulerAngles, new Vector3(0, targetRot, 0), 0.1f));
        }
    }
}
