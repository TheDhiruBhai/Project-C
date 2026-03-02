using UnityEngine;

namespace Game.World
{
    public sealed class Flammable : MonoBehaviour, IFlammable
    {
        [SerializeField] private float burnTimeSeconds = 3f;
        private float _remaining;
        private bool _burning;

        public void Ignite(float seconds)
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
                // Replace with destroyed state or disable visuals.
                gameObject.SetActive(false);
            }
        }
    }
}
