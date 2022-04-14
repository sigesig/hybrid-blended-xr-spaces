using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Chiligames.MetaAvatars;
using Oculus.Avatar2;
using Photon.Pun;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

using StreamLOD = Oculus.Avatar2.OvrAvatarEntity.StreamLOD;


/* This class is an example of how to use the Streaming functions of the avatar to send and receive data over the network
 * For this example, data isn't sent over a real network, but simply added to a queue and then "received" by a second, "remote" avatar.
 * For a real network, much of the logic of preparing snapshots and receiving based on the desired fidelity is the same
 */
namespace Chiligames.MetaAvatars
{
    public class RemoteAvatarManager : MonoBehaviourPun
    {
        private const string logScope = "SampleRemoteLoopbackManager";

        // Const & Static Variables
        private const float PLAYBACK_SMOOTH_FACTOR = 0.25f;
        private const int MAX_PACKETS_PER_FRAME = 3;

        private static readonly float[] StreamLodSnapshotIntervalSeconds = new float[OvrAvatarEntity.StreamLODCount] { 1f / 72, 2f / 72, 3f / 72, 4f / 72 };

        #region Internal Classes

        class PacketData : IDisposable
        {
            public NativeArray<byte> data;
            public StreamLOD lod;
            public float fakeLatency;
            public UInt32 dataByteCount;

            private uint refCount = 0;

            public PacketData() { }

            ~PacketData()
            {
                Dispose(false);
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            private void Dispose(bool disposing)
            {
                if (data.IsCreated)
                {
                    data.Dispose();
                }
                data = default;
            }

            public bool Unretained => refCount == 0;
            public PacketData Retain() { ++refCount; return this; }
            public bool Release()
            {
                return --refCount == 0;
            }
        };

        class LoopbackState
        {
            public List<PacketData> packetQueue = new List<PacketData>(64);
            public StreamLOD requestedLod = StreamLOD.Low;
            public float smoothedPlaybackDelay = 0f;
        };

        [System.Serializable]
        public class SimulatedLatencySettings
        {
            [Range(0.0f, 0.5f)]
            public float fakeLatencyMax = 0.25f; //250 ms max latency

            [Range(0.0f, 0.5f)]
            public float fakeLatencyMin = 0.02f; //20ms min latency

            [Range(0.0f, 1.0f)]
            public float latencyWeight = 0.25f; // How much the latest sample impacts the current latency

            [Range(0, 10)]
            public int maxSamples = 4; //How many samples in our window

            internal float averageWindow = 0f;
            internal float latencySum = 0f;
            internal List<float> latencyValues = new List<float>();

            public float NextValue()
            {
                averageWindow = latencySum / (float)latencyValues.Count;
                float randomLatency = UnityEngine.Random.Range(fakeLatencyMin, fakeLatencyMax);
                float fakeLatency = averageWindow * (1f - latencyWeight) + latencyWeight * randomLatency;

                if (latencyValues.Count >= maxSamples)
                {
                    latencySum -= latencyValues.First().Value;
                    latencyValues.RemoveFirst();
                }

                latencySum += fakeLatency;
                latencyValues.AddLast(fakeLatency);

                return fakeLatency;
            }
        };

        #endregion

        // Serialized Variables
        [SerializeField] private OvrAvatarEntity _localAvatar = null;
        [SerializeField] private Transform OVRRig;
        private List<OvrAvatarEntity> _loopbackAvatars = null;
        [Tooltip("How much time transcurs between every avatar packet sent")]
        [SerializeField] private float refreshRate = 2 / 72;
        private float refreshRateCounter = 0;
        //Network position and rotation of the OVR Rig, to be able to implement locomotion systems where we move the Rig.
        [Tooltip("Send or not the Rig position and rotation, useful when dealing with locomotion")]
        [SerializeField] private bool sendRigPositionRotation;

        // Private Variables
        private Dictionary<OvrAvatarEntity, LoopbackState> _loopbackStates =
            new Dictionary<OvrAvatarEntity, LoopbackState>();

        private Dictionary<int, OvrAvatarEntity> _avatarsById;
        private Dictionary<int, AvatarParent> _avatarParentsById;
        private float playbackDelay = 0.1f;

        private readonly List<PacketData> _packetPool = new List<PacketData>(32);
        private readonly List<PacketData> _deadList = new List<PacketData>(16);

        private PacketData GetPacketForEntityAtLOD(OvrAvatarEntity entity, StreamLOD lod)
        {
            PacketData packet;
            int poolCount = _packetPool.Count;
            if (poolCount > 0)
            {
                var lastIdx = poolCount - 1;
                packet = _packetPool[lastIdx];
                _packetPool.RemoveAt(lastIdx);
            }
            else
            {
                packet = new PacketData();
            }

            packet.lod = lod;
            return packet.Retain();
        }
        private void ReturnPacket(PacketData packet)
        {
            Debug.Assert(packet.Unretained);
            _packetPool.Add(packet);
        }

        private readonly float[] _streamLodSnapshotElapsedTime = new float[OvrAvatarEntity.StreamLODCount];

        byte[] _packetBuffer = new byte[16 * 1024];
        GCHandle _pinnedBuffer;

        public List<OvrAvatarEntity> LoopbackAvatars
        {
            get
            {
                return _loopbackAvatars;
            }

            set
            {
                _loopbackAvatars = value;
                CreateStates();
            }
        }

        #region Core Unity Functions

        public static RemoteAvatarManager instance;
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

            _loopbackAvatars = new List<OvrAvatarEntity>();
        }

        protected void Start()
        {
            _avatarsById = new Dictionary<int, OvrAvatarEntity>();
            _avatarParentsById = new Dictionary<int, AvatarParent>();

            // assume _useAdvancedLodSystem is enabled
            AvatarLODManager.Instance.firstPersonAvatarLod = _localAvatar.AvatarLOD;
            AvatarLODManager.Instance.enableDynamicStreaming = true;

            _pinnedBuffer = GCHandle.Alloc(_packetBuffer, GCHandleType.Pinned);

            CreateStates();
        }

        public void AddRemoteAvatar(int actorNumber, OvrAvatarEntity avatar)
        {
            _avatarsById.Add(actorNumber, avatar);
            _avatarParentsById.Add(actorNumber, avatar.GetComponentInParent<AvatarParent>());
            _loopbackAvatars.Add(avatar);
            _loopbackStates.Add(avatar, new LoopbackState());
        }
        public void RemoveRemoteAvatar(int actorNumber)
        {
            if (_avatarsById.ContainsKey(actorNumber))
            {
                _loopbackAvatars.Remove(_avatarsById[actorNumber]);
                _loopbackStates.Remove(_avatarsById[actorNumber]);
                var avatar = _avatarsById[actorNumber];
                _avatarsById.Remove(actorNumber);
                _avatarParentsById.Remove(actorNumber);
                Destroy(avatar.gameObject);
            }
        }

        private void CreateStates()
        {
            foreach (var item in _loopbackStates)
            {
                foreach (var packet in item.Value.packetQueue)
                {
                    if (packet.Release())
                    {
                        ReturnPacket(packet);
                    }
                }
            }
            _loopbackStates.Clear();

            foreach (var loopbackAvatar in _loopbackAvatars)
            {
                _loopbackStates.Add(loopbackAvatar, new LoopbackState());
            }
        }

        private void OnDestroy()
        {
            if (_pinnedBuffer.IsAllocated)
            {
                _pinnedBuffer.Free();
            }

            foreach (var item in _loopbackStates)
            {
                foreach (var packet in item.Value.packetQueue)
                {
                    if (packet.Release())
                    {
                        ReturnPacket(packet);
                    }
                }
            }

            foreach (var packet in _packetPool)
            {
                packet.Dispose();
            }
            _packetPool.Clear();
        }

        private void Update()
        {
            for (int i = 0; i < OvrAvatarEntity.StreamLODCount; ++i)
            {
                // Assume remote Avatar StreamLOD sizes are the same
                float streamBytesPerSecond = _localAvatar.GetLastByteSizeForLodIndex(i) / StreamLodSnapshotIntervalSeconds[i];
                AvatarLODManager.Instance.dynamicStreamLodBitsPerSecond[i] = (long)(streamBytesPerSecond * 8);
            }

            foreach (var item in _loopbackStates)
            {
                var loopbackAvatar = item.Key;
                var loopbackState = item.Value;

                if (!loopbackAvatar.IsCreated)
                {
                    continue;
                }

                //UpdatePlaybackTimeDelay(loopbackAvatar, loopbackState);

                // "Remote" avatar receives incoming data and applies if it is the correct lod
                if (loopbackState.packetQueue.Count > 0)
                {
                    foreach (var packet in loopbackState.packetQueue)
                    {
                        packet.fakeLatency -= Time.deltaTime;

                        if (packet.fakeLatency <= 0f)
                        {
                            var dataSlice = packet.data.Slice(0, (int)packet.dataByteCount);
                            byte[] dataArray = dataSlice.ToArray();
                            photonView.RPC("RPC_SendPacketData", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, dataArray);
                            if (sendRigPositionRotation)
                            {
                                //Send transform and rotation from rig (avatar parent)
                                float[] pos = new float[3] { OVRRig.position.x, OVRRig.position.y, OVRRig.position.z};
                                float rot = OVRRig.rotation.eulerAngles.y;
                                photonView.RPC("RPC_SendPosRot", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, pos, rot);
                            }
                            _deadList.Add(packet);
                        }
                    }

                    foreach (var packet in _deadList)
                    {
                        loopbackState.packetQueue.Remove(packet);
                        if (packet.Release())
                        {
                            ReturnPacket(packet);
                        }
                    }
                    _deadList.Clear();
                }

                // "Send" the lod that "remote" avatar wants to use back over the network
                // TODO delay this reception for an accurate test
                loopbackState.requestedLod = loopbackAvatar.activeStreamLod;
            }
        }

        private void LateUpdate()
        {
            refreshRateCounter += Time.deltaTime;
            if(refreshRateCounter >= refreshRate)
            {
                // Local avatar has fully updated this frame and can send data to the network
                SendSnapshot();
                refreshRateCounter = 0;
            }
        }

        #endregion

        #region Local Avatar

        private void SendSnapshot()
        {
            if (!_localAvatar.HasJoints) { return; }

            for (int streamLod = (int)StreamLOD.High; streamLod <= (int)StreamLOD.Low; ++streamLod)
            {
                int packetsSentThisFrame = 0;
                _streamLodSnapshotElapsedTime[streamLod] += Time.unscaledDeltaTime;
                while (_streamLodSnapshotElapsedTime[streamLod] > StreamLodSnapshotIntervalSeconds[streamLod])
                {
                    SendPacket((StreamLOD)streamLod);
                    _streamLodSnapshotElapsedTime[streamLod] -= StreamLodSnapshotIntervalSeconds[streamLod];
                    if (++packetsSentThisFrame >= MAX_PACKETS_PER_FRAME)
                    {
                        _streamLodSnapshotElapsedTime[streamLod] = 0;
                        break;
                    }
                }
            }
        }

        private void SendPacket(StreamLOD lod)
        {
            var packet = GetPacketForEntityAtLOD(_localAvatar, lod);

            packet.dataByteCount = _localAvatar.RecordStreamData_AutoBuffer(lod, ref packet.data);
            Debug.Assert(packet.dataByteCount > 0);

            foreach (var loopbackState in _loopbackStates.Values)
            {
                if (loopbackState.requestedLod == lod)
                {
                    loopbackState.packetQueue.Add(packet.Retain());
                }
            }

            if (packet.Release())
            {
                ReturnPacket(packet);
            }
        }

        #endregion

        [PunRPC]
        private void RPC_SendPacketData(int actorNumber, byte[] data)
        {
            var _nativeArray = Converters.MoveFromByteArray<byte>(ref data);
            NativeSlice<byte> nativeSlice = new NativeSlice<byte>(_nativeArray);
            ReceivePacketData(actorNumber, nativeSlice);
        }

        [PunRPC]
        private void RPC_SendPosRot(int actorNumber, float[] pos, float rot)
        {
            if (_avatarsById.ContainsKey(actorNumber))
            {
                _avatarParentsById[actorNumber].SetTargetPosRot(pos, rot);
            }
        }

        private void ReceivePacketData(int actorNumber, in NativeSlice<byte> data)
        {

            if (_avatarsById.ContainsKey(actorNumber))
            {
                //Hard coded playback delay, we should calculate from latency
                _avatarsById[actorNumber].SetPlaybackTimeDelay(playbackDelay);
                _avatarsById[actorNumber].ApplyStreamData(in data);
            }
        }
    }
}
