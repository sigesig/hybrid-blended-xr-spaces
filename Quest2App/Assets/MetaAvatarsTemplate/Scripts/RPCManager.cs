using Photon.Pun;

namespace Chiligames.MetaAvatars
{
    public class RPCManager : MonoBehaviourPunCallbacks
    {
        public static RPCManager instance;

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

        [PunRPC]
        public void RPC_CreateRemoteAvatarEntity(int actorNumber, long userID, float x, float y, float z, float rotationY)
        {
            PlayerManager.instance.CreateRemoteAvatarEntity(actorNumber, userID, x, y, z, rotationY);
        }
    }
}

