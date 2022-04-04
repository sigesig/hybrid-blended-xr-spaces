using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectAdder : MonoBehaviour
{
    [SerializeField]
    public GameObject _prefabCube;
    private GameObject _cube;
    // Start is called before the first frame update
    void Start()
    {
        var cubePos = Camera.main.transform.position + new Vector3(0,0, 10);
        _cube = Instantiate(_prefabCube, cubePos, Quaternion.identity);
        _cube.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
