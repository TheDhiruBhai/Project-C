using System.Collections;
using UnityEngine;
using Photon.Pun;

namespace Game.World
{
    public sealed class GrowableSurface : MonoBehaviour, IGrowable
    {
        [SerializeField] private GameObject plantBridge;
        [SerializeField] private float growDuration = 3f;
        [SerializeField] private ParticleSystem growVFX;

        private bool _grown = false;
        private PhotonView _pv;

        public bool IsGrown => _grown;

        private void Awake() => _pv = GetComponent<PhotonView>();

        private void Start()
        {
            if (plantBridge != null) plantBridge.SetActive(false);
        }

        public void Grow()
        {
            if (_grown) return;
                _pv.RPC(nameof(RPC_Grow), RpcTarget.All);
        }

        [PunRPC]
        private void RPC_Grow()
        {
            if (_grown) return;
            _grown = true;
            StartCoroutine(GrowCoroutine());
        }

        private IEnumerator GrowCoroutine()
        {
            if (growVFX != null) growVFX.Play();
            yield return new WaitForSeconds(growDuration);
            if (growVFX != null) growVFX.Stop();
            if (plantBridge != null) plantBridge.SetActive(true);
        }
    }
}