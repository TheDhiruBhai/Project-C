using UnityEngine;
using Photon.Pun;
using Game.Player;

namespace Game.World
{
    public sealed class HealthTargetAdapter : MonoBehaviour, IHealthTarget
    {
        [SerializeField] private Health health;
        private IDamageImmune _damageImmune;
        private PhotonView _pv;

        private void Awake()
        {
            _pv = GetComponent<PhotonView>();
            _damageImmune = GetComponentInParent<IDamageImmune>();
        }

        private void Reset()
        {
            if (health == null) health = GetComponentInParent<Health>();
        }

        public void TakeDamage(int amount)
        {
            if (health == null) return;
            if (_damageImmune != null && _damageImmune.IsInvulnerable) return;
            _pv.RPC(nameof(RPC_TakeDamage), RpcTarget.All, amount);
        }

        public void Heal(int amount)
        {
            if (health == null) return;
            _pv.RPC(nameof(RPC_Heal), RpcTarget.All, amount);
        }

        [PunRPC]
        private void RPC_TakeDamage(int amount)
        {
            if (health == null) return;
            if (_damageImmune != null && _damageImmune.IsInvulnerable) return;
            health.TakeDamage(amount);
        }

        [PunRPC]
        private void RPC_Heal(int amount)
        {
            if (health != null) health.Heal(amount);
        }
    }
}