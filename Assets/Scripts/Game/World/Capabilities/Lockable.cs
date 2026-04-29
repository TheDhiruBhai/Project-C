using UnityEngine;
using Photon.Pun;

namespace Game.World
{
    public sealed class Lockable : MonoBehaviour, ILockable
    {
        [SerializeField] private bool locked = true;
        private PhotonView _pv;

        public bool IsLocked => locked;

        private void Awake() => _pv = GetComponent<PhotonView>();

        public void Unlock()
        {
            if (!locked) return;
            _pv.RPC(nameof(RPC_Unlock), RpcTarget.All);
        }

        [PunRPC]
        private void RPC_Unlock()
        {
            locked = false;
            // Add door animation, collider changes, and sound here.
        }
    }
}