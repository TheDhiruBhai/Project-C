using UnityEngine;

namespace Game.World
{
    public sealed class LightReceiver : MonoBehaviour, ILightReceiver
    {
        private float _litRemaining;

        public void Illuminate(float seconds)
        {
            _litRemaining = Mathf.Max(_litRemaining, seconds);
            // Disable darkness volume or increase local light here.
        }

        private void Update()
        {
            if (_litRemaining <= 0f) return;
            _litRemaining -= Time.deltaTime;
            if (_litRemaining <= 0f)
            {
                // Re enable darkness effect here.
            }
        }
    }
}
