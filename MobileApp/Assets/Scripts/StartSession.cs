using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartSession : MonoBehaviour
{
    [SerializeField] public Button startSessionBtn;
    [SerializeField] public Canvas sessionCanvas;
    [SerializeField] public Canvas spaceCanvas;

    void Start()
    {
        startSessionBtn.onClick.AddListener(SwitchToSpaceCreation);
    }

    
    void Update()
    {
        
    }
    
    /*
     * Used by start session button to switch between Space creation canvas and the begin session canvas
     */
    private void SwitchToSpaceCreation()
    {
        Debug.Log("Pressed");
        sessionCanvas.gameObject.SetActive(false);
        spaceCanvas.gameObject.SetActive(true);
    }
}
