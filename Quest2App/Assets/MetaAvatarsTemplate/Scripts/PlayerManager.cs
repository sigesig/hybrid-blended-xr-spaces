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

        [SerializeField] GameObject remoteAvatarPrefab;
        [SerializeField] GameObject photonVoiceSetupPrefab;

        [HideInInspector] public ulong userID = 0;

        public static PlayerManager instance;

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

        public override void OnJoinedRoom()
        {
            //If we are master, instantiate the RPCManager when joining room
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.InstantiateRoomObject("RPCManager", Vector3.zero, Quaternion.identity);
            }

            //Set the user to different spawning locations
            if (PhotonNetwork.LocalPlayer.ActorNumber <= spawnPoints.Length)
            {
                OVRCameraRig.transform.position = spawnPoints[PhotonNetwork.LocalPlayer.ActorNumber - 1].transform.position;
                OVRCameraRig.transform.rotation = spawnPoints[PhotonNetwork.LocalPlayer.ActorNumber - 1].transform.rotation;
            }

            var voiceSetup = PhotonNetwork.Instantiate(photonVoiceSetupPrefab.name, centerEyeAnchor.transform.position, centerEyeAnchor.transform.rotation);
            voiceSetup.transform.SetParent(centerEyeAnchor);

            GetUserID();
        }

        //Get Oculus/Meta user ID and send it to other users to spawn our avatars in their instances of the app
        private void GetUserID()
        {
            Users.GetLoggedInUser().OnComplete(message =>
            {
                if (!message.IsError)
                {
                    userID = message.Data.ID;
                    Debug.Log("User id is: " + userID.ToString());
                    RPCManager.instance.photonView.RPC("RPC_CreateRemoteAvatarEntity", RpcTarget.OthersBuffered, PhotonNetwork.LocalPlayer.ActorNumber, (long)userID, OVRCameraRig.transform.position.x, 
                        OVRCameraRig.transform.position.y, OVRCameraRig.transform.position.z, OVRCameraRig.transform.rotation.eulerAngles.y);
                }
                else
                {
                    var e = message.GetError();
                }
            });
        }

        //Create the avatar from a remote user in our instance of the app
        public void CreateRemoteAvatarEntity(int actorNumber, long userID, float x, float y, float z, float rotationY)
        {
            GameObject obj = Instantiate(remoteAvatarPrefab, new Vector3(x ,y ,z), Quaternion.Euler(0, rotationY, 0));
            PunAvatarEntity avatarEntity = obj.GetComponentInChildren<PunAvatarEntity>();
            avatarEntity.LoadRemoteUserCdnAvatar((ulong)userID);
            RemoteAvatarManager.instance.AddRemoteAvatar(actorNumber, avatarEntity);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);
            DestroyRemoteAvatarEntity(otherPlayer.ActorNumber);
        }

        public void DestroyRemoteAvatarEntity(int actorNumber)
        {
            RemoteAvatarManager.instance.RemoveRemoteAvatar(actorNumber);
        }
    }
}
