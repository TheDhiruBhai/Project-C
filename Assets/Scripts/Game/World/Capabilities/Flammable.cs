using UnityEngine;
using Photon.Pun;

namespace Game.World
{
    public sealed class Flammable : MonoBehaviour, IFlammable
    {
        [SerializeField] private float burnTimeSeconds = 3f;
        private float _remaining;
        private bool _burning;
        private PhotonView _pv;

        private void Awake() => _pv = GetComponent<PhotonView>();

        public void Ignite(float seconds)
        {
            if (_burning) return;
            // Only MasterClient broadcasts ignition to avoid duplicate calls
            if (PhotonNetwork.IsMasterClient)
                _pv.RPC(nameof(RPC_Ignite), RpcTarget.All, seconds);
        }

        [PunRPC]
        private void RPC_Ignite(float seconds)
        {
            _burning = true;
            _remaining = Mathf.Max(_remaining, seconds > 0f ? seconds : burnTimeSeconds);
        }

        private void Update()
        {
            if (!_burning) return;
            _remaining -= Time.deltaTime;
            if (_remaining <= 0f)
            {
                _burning = false;
                // MasterClient disables so it fires once across all clients
                if (PhotonNetwork.IsMasterClient)
                    _pv.RPC(nameof(RPC_BurnOut), RpcTarget.All);
            }
        }

        [PunRPC]
        private void RPC_BurnOut()
        {
            _burning = false;
            gameObject.SetActive(false);
        }
    }
}