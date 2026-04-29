using UnityEngine;
using Photon.Pun;

namespace Game.World
{
    public sealed class LightReceiver : MonoBehaviour, ILightReceiver
    {
        private float _litRemaining;
        [SerializeField] private Light bonfireLight;
        private PhotonView _pv;

        private void Awake() => _pv = GetComponent<PhotonView>();

        public void Illuminate(float seconds)
        {
            _pv.RPC(nameof(RPC_Illuminate), RpcTarget.All, seconds);
        }

        [PunRPC]
        private void RPC_Illuminate(float seconds)
        {
            _litRemaining = Mathf.Max(_litRemaining, seconds);
            if (bonfireLight) bonfireLight.gameObject.SetActive(true);
        }

        private void Update()
        {
            if (_litRemaining <= 0f) return;
            _litRemaining -= Time.deltaTime;
            if (_litRemaining <= 0f)
            {
                // Only MasterClient calls the RPC to turn off — prevents double-fire
                if (PhotonNetwork.IsMasterClient)
                    _pv.RPC(nameof(RPC_LightOff), RpcTarget.All);
            }
        }

        [PunRPC]
        private void RPC_LightOff()
        {
            if (bonfireLight) bonfireLight.gameObject.SetActive(false);
        }
    }
}