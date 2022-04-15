using System.Collections;
using System.Collections.Generic;
using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.UI;

public class SessionInProgress : MonoBehaviour
{
    // Mute/Unmute Variables
    [SerializeField] public Button muteUnmuteButton;
    [SerializeField] public Sprite muted;
    [SerializeField] public Sprite unMuted;
    // Canvases
    [SerializeField] public Canvas sessionCanvas;
    [SerializeField] public Canvas currentSession;
    [SerializeField] public Recorder recorder;
    
    private bool _voiceChatIsMuted = true;
    
    // End Session Variables
    [SerializeField] public Button exitSession;
    void Start()
    {
        muteUnmuteButton.onClick.AddListener(VoiceChatControl);
        exitSession.onClick.AddListener(EndSession);
        //recorder.IsRecording = !_voiceChatIsMuted;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    /*
     * End the current session. Used by the exit session button
     */
    private void EndSession()
    {
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
}
