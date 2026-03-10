using UnityEngine;
using Game.Player;

namespace Game.World
{
    public sealed class HealthTargetAdapter : MonoBehaviour, IHealthTarget
    {
        [SerializeField] private Health health;

        private void Reset()
        {
            if (health == null) health = GetComponentInParent<Health>();
        }

        public void TakeDamage(int amount)
        {
            if (health != null) health.TakeDamage(amount);
        }

        public void Heal(int amount)
        {
            if (health != null) health.Heal(amount);
        }
    }
}
