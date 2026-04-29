using UnityEngine;
using Photon.Pun;

namespace Game.World
{
    public sealed class Holdable : MonoBehaviour, IHoldable
    {
        private float _holdRemaining;
        private PhotonView _pv;

        private void Awake() => _pv = GetComponent<PhotonView>();

        public void HoldStill(float seconds)
        {
            _pv.RPC(nameof(RPC_HoldStill), RpcTarget.All, seconds);
        }

        [PunRPC]
        private void RPC_HoldStill(float seconds)
        {
            _holdRemaining = Mathf.Max(_holdRemaining, seconds);
        }

        private void Update()
        {
            if (_holdRemaining <= 0f) return;
            _holdRemaining -= Time.deltaTime;
        }

        public bool IsHeld => _holdRemaining > 0f;
    }
}