using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class whiteboardScript : MonoBehaviour
{
    public Texture2D texture;
    public Vector2 textureSize = new Vector2(2048, 2048);

    private marker marker;
    // Start is called before the first frame update
    void Start()
    {
        var render = GetComponent<Renderer>();
        texture = new Texture2D((int)textureSize.x, (int)textureSize.y, TextureFormat.RGBA32, true, true);
        //Texture2D(int width, int height, TextureFormat format, bool mipmap, bool linear);
        render.material.mainTexture = texture;
        //marker = hand.GetComponent<marker>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerStay(Collider col)

    {
        if (col.gameObject.tag == "RightIndex")
        {
           
        }
    }
}
