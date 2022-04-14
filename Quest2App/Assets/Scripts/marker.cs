using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class marker : MonoBehaviour
{
    private Color[] color;
    private int penSize = 15;
    private OVRSkeleton bone;
    private whiteboardScript whiteboardPlane;
   
    public GameObject plane;

    private Vector2 lastTouchPos;

    private bool fingerOnLastFrame;

    private GameObject indexR;
    private Transform indexBone;
    public GameObject index;

    // Start is called before the first frame update
    void Start()
    {
        //Create array for pixel colors the size of our "pen"
        color = Enumerable.Repeat(Color.blue, penSize * penSize).ToArray();
        
        //Get the whiteboard object from the plane to be able to get info
        lastTouchPos = new Vector2(0,0);

        //Get coordinates of indexfinger and create a sphere at the end (to detect collison with whiteboard later)
        bone = GetComponent<OVRSkeleton>();
        indexBone = bone.Bones[(int)OVRPlugin.BoneId.Hand_Index3].Transform;
        indexR = Instantiate(index, indexBone.position, Quaternion.identity) as GameObject;
    }

    // Update is called once per frame
    void Update()
    {
        //update position of sphere to index finger
        indexBone = bone.Bones[(int)OVRPlugin.BoneId.Hand_Index3].Transform;
        indexR.transform.position = indexBone.position;

        Draw();
    }

    public void Draw()
    {
        //Raycast sphere at indexfinger to detect collision
        RaycastHit hit;
        if (Physics.Raycast(indexR.transform.position, -transform.right, out hit,0.02f))
        {
            //Only react if collison is whiteboard
            if (hit.transform.CompareTag("whiteboard"))
            {
                if (plane == null) {
                    whiteboardPlane = plane.transform.GetComponent<whiteboardScript>();
                }

                //Adjust coordinates to texture size
                var wbX = (int)(hit.textureCoord.x * whiteboardPlane.textureSize.x - (penSize / 2));
                var wbY = (int)(hit.textureCoord.y * whiteboardPlane.textureSize.y - (penSize / 2));

                //bail out if outside wb texture
                if (isOutOffBound(wbX, wbY)) return;

                //Check draw between points if finger is not raised from wb
                if (fingerOnLastFrame)
                {
                    //set pixels between last frame coord and this. Adjust 'f += x' for intensity
                    for (float f = 0.01f; f < 1.00f; f += 0.03f)
                    {
                        var lerpX = (int)Mathf.Lerp(lastTouchPos.x, wbX, f);
                        var lerpY = (int)Mathf.Lerp(lastTouchPos.y, wbY, f);

                        whiteboardPlane.texture.SetPixels(lerpX, lerpY, penSize, penSize, color);
                    }
                    whiteboardPlane.texture.Apply();
                }
                lastTouchPos = new Vector2(wbX, wbY);
                fingerOnLastFrame = true;
                return;
            }
        }
        fingerOnLastFrame = false;
    }

    public Texture GetTexture() {
        return whiteboardPlane.texture;
    }

    //Check if 'pen' is outside wb texture
    private bool isOutOffBound(float x, float y) {
        return (x < 0 || x > whiteboardPlane.textureSize.x || y < 0 || y > whiteboardPlane.textureSize.y);
    }
}
