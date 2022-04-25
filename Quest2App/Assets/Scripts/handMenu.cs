using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class handMenu : MonoBehaviour
{
    //public GameObject cube1;

    public GameObject canvas;
    public GameObject text;

    private bool definePlane;
    private bool openMenu;
    //private GameObject cube2;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("feta handmenu");

        openMenu = false;
        definePlane = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        

    }

    public void createObject() {

        //cube2 = Instantiate(cube1, transform.position, Quaternion.identity) as GameObject;
        //cube2.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.5f)
        canvas.SetActive(true);


    }

    public void destroyObject() {

        //Destroy(cube2);
        canvas.SetActive(false);
    }

    public void toggleActive() {

        openMenu = !openMenu;
        canvas.SetActive(openMenu);

    }

    public void toggleDefinePlane() {

        definePlane = !definePlane;
        text.SetActive(definePlane);
    }

    public void setActiveMenu() {

        canvas.SetActive(true);
    }

    public void setNotActiveMenu()
    {
        canvas.SetActive(false);

    }
}
