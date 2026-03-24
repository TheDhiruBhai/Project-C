using UnityEngine;
using Game.Player;

namespace Game.World
{
    public sealed class HealthTargetAdapter : MonoBehaviour, IHealthTarget
    {
        [SerializeField] private Health health;

        private IDamageImmune _damageImmune;

        private void Awake()
        {
            _damageImmune = GetComponentInParent<IDamageImmune>();
        }

        private void Reset()
        {
            if (health == null) health = GetComponentInParent<Health>();
        }

        public void TakeDamage(int amount)
        {
            if (health == null) return;
            // Invulnerability check — supplied by PlayerInvulnerability component
            if (_damageImmune != null && _damageImmune.IsInvulnerable) return;
            health.TakeDamage(amount);
        }

        public void Heal(int amount)
        {
            if (health != null) health.Heal(amount);
        }
    }
}