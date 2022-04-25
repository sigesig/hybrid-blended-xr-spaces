using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManipulatePlane : MonoBehaviour
{
    #region Private Variables

    private List<GameObject> _placedPoints;
    private LineRenderer _lineRenderer;

    #endregion
    
    
    // Start is called before the first frame update
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _placedPoints = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void DefinePlaneDepth(Collider other)
    {
        
    }
}
