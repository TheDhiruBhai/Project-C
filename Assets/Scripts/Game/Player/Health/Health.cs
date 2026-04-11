using UnityEngine;

namespace Game.Player
{
    public sealed class Health : MonoBehaviour
    {
        [SerializeField] private int maxHp = 100;
        [SerializeField] private int currentHp = 100;

        public int MaxHp => maxHp;
        public int CurrentHp => currentHp;

        public event System.Action<int, int> OnChanged;

        private void Awake()
        {
            currentHp = Mathf.Clamp(currentHp, 0, maxHp);
            OnChanged?.Invoke(currentHp, maxHp);
        }

        public bool CanSpend(int amount) => amount <= 0 || currentHp >= amount;

        public bool TrySpend(int amount)
        {
            if (!CanSpend(amount)) return false;
            if (amount <= 0) return true;
            currentHp -= amount;
            currentHp = Mathf.Clamp(currentHp, 0, maxHp);
            OnChanged?.Invoke(currentHp, maxHp);
            return true;
        }

        public void Heal(int amount)
        {
            if (amount <= 0) return;
            currentHp = Mathf.Clamp(currentHp + amount, 0, maxHp);
            OnChanged?.Invoke(currentHp, maxHp);
        }

        public void TakeDamage(int amount)
        {
            if (amount <= 0) return;
            currentHp = Mathf.Clamp(currentHp - amount, 0, maxHp);
            OnChanged?.Invoke(currentHp, maxHp);
        }
    }
}
