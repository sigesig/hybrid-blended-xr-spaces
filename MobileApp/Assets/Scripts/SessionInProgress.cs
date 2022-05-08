using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Interaction.Toolkit.AR;

public class SessionInProgress : MonoBehaviour
{
    #region Serialized Fields
    // Mute/Unmute Variables
    [SerializeField] public Button muteUnmuteButton;
    [SerializeField] public Sprite muted;
    [SerializeField] public Sprite unMuted;
    // Canvases
    [SerializeField] public Canvas sessionCanvas;
    [SerializeField] public Canvas currentSession;
    [SerializeField] public Recorder recorder;
    [SerializeField] public Camera ARCamera;
    
    [SerializeField] public ARGestureInteractor gestureInteractable;
    [SerializeField] public ARRaycastManager raycastManager;
    // End Session Variables
    [SerializeField] public Button exitSession;
    #endregion

    #region Private variables
    
        private bool _voiceChatIsMuted = true;
        private GameObject _laserLine;
        private LineRenderer _lineRenderer;
        private bool _laserPointerActive = true;
        
    #endregion
    

    void Start()
    {
        _laserLine = Networking.GetLaserLine();
        _lineRenderer = _laserLine.GetComponent<LineRenderer>();
        gestureInteractable.dragGestureRecognizer.onGestureStarted += DragGestureRecognizerStarted;
        muteUnmuteButton.onClick.AddListener(VoiceChatControl);
        exitSession.onClick.AddListener(EndSession);
        //recorder.IsRecording = !_voiceChatIsMuted;
    }

    /*
     * End the current session. Used by the exit session button
     */
    private void EndSession()
    {
        Networking.LeaveRoom();
        currentSession.gameObject.SetActive(false);
        sessionCanvas.gameObject.SetActive(true);
    }
    
    /*
     * Used by the mute/unmute button. 
     */
    private void VoiceChatControl()
    {
        if (_voiceChatIsMuted)
        {
            _voiceChatIsMuted = !_voiceChatIsMuted;

            muteUnmuteButton.GetComponent<Image>().sprite = unMuted;

            return;
        }

        _voiceChatIsMuted = !_voiceChatIsMuted;
        muteUnmuteButton.GetComponent<Image>().sprite = muted;

        recorder.IsRecording = !_voiceChatIsMuted;
    }

    private void DragGestureRecognizerStarted(Gesture<DragGesture> dragGesture)
    {

        dragGesture.onStart += (s) =>
        {
            HandleLaserPointer(s);
            
        };

        dragGesture.onUpdated += (s) =>
        {
            HandleLaserPointer(s);

        };
    }

    private void HandleLaserPointer(DragGesture s)
    {
        if (!_laserPointerActive) return;
            
        Debug.Log("LASER: currently touching");
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        if (raycastManager.Raycast(s.position, hits, TrackableType.PlaneWithinPolygon))
        {
            Debug.Log("LASER: hit an object: " + hits[0]);
            _lineRenderer.SetPosition(0, ARCamera.transform.position);
            _lineRenderer.SetPosition(1, hits[0].pose.position);
        }
    }
    

    private void LaserPointerSwitchButton()
    {
        _laserPointerActive = !_laserPointerActive;
        _laserLine.gameObject.SetActive(_laserPointerActive);
    }

}
