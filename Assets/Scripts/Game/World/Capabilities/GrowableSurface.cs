using System.Collections;
using UnityEngine;

namespace Game.World
{
    public sealed class GrowableSurface : MonoBehaviour, IGrowable
    {
        [SerializeField]
        [Tooltip("The bridge GameObject that becomes active once growth completes.")]
        private GameObject plantBridge;

        [SerializeField]
        [Tooltip("Time in seconds for the bridge to fully form (design: 3s).")]
        private float growDuration = 3f;

        [SerializeField]
        [Tooltip("Optional particle effect played while the bridge grows.")]
        private ParticleSystem growVFX;

        private bool _grown = false;
        public bool IsGrown => _grown;

        private void Start()
        {
            if (plantBridge != null) plantBridge.SetActive(false);
        }

        public void Grow()
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
