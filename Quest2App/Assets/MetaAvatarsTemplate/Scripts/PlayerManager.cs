using Oculus.Avatar2;
using Oculus.Platform;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
//
//For handling local objects and sending data over the network
//
namespace Chiligames.MetaAvatars
{
    public class PlayerManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] GameObject OVRCameraRig;
        [SerializeField] Transform centerEyeAnchor;
        [SerializeField] Transform[] spawnPoints;

        [SerializeField] GameObject photonVoiceSetupPrefab;

        [HideInInspector] public ulong userID = 0;

        public static PlayerManager instance;
        private GameObject avatar;
        private GameObject centerEye;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                instance = this;
            }
        }

        public void Update() {
            //apparently it does not work if you guard for avatar != null??
            avatar.transform.position = centerEyeAnchor.transform.position;
            avatar.transform.rotation = centerEyeAnchor.transform.rotation;
        }

        public override void OnJoinedRoom()
        {
            //Set the user to different spawning locations
            if (PhotonNetwork.LocalPlayer.ActorNumber <= spawnPoints.Length)
            {
                OVRCameraRig.transform.position = spawnPoints[PhotonNetwork.LocalPlayer.ActorNumber - 1].transform.position;
                OVRCameraRig.transform.rotation = spawnPoints[PhotonNetwork.LocalPlayer.ActorNumber - 1].transform.rotation;
            }
            var voiceSetup = PhotonNetwork.Instantiate(photonVoiceSetupPrefab.name, centerEyeAnchor.transform.position, centerEyeAnchor.transform.rotation);
            voiceSetup.transform.SetParent(centerEyeAnchor);

            //Instantiate avatar/representation of HMD user on Photonnetwork
            avatar = PhotonNetwork.Instantiate("CubeAvatar", centerEyeAnchor.transform.position, centerEyeAnchor.transform.rotation);
            //avatar.SetActive(false);
        }
    }
}
