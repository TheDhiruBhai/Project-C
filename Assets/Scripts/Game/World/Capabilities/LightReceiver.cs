using UnityEngine;

namespace Game.World
{
    public sealed class LightReceiver : MonoBehaviour, ILightReceiver
    {
        private float _litRemaining;
        [SerializeField] private Light bonfireLight;

        public void Illuminate(float seconds)
        {
            _litRemaining = Mathf.Max(_litRemaining, seconds);
            if (bonfireLight) bonfireLight.gameObject.SetActive(true);
            Debug.Log($"[LightReceiver] Light enabled for {seconds}s");
        }

        private void Update()
        {
            if (_litRemaining <= 0f) return;
            _litRemaining -= Time.deltaTime;
            if (_litRemaining <= 0f)
            {
                if (bonfireLight) bonfireLight.gameObject.SetActive(false);
            }
        }
    }
}
