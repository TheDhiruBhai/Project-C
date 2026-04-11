using UnityEngine;

namespace Game.Player
{
    public sealed class PlayerPassThrough : MonoBehaviour, World.IWateryForm
    {
        [SerializeField]
        [Tooltip("Index of the physics layer used while Watery Form is active. " +
                 "Must be configured in the Physics Layer Collision Matrix to ignore SteelGate.")]
        private int passThroughLayer = 6; // Set to your WateryForm layer index

        private int _normalLayer;
        private float _remaining;

        public bool IsPassingThrough => _remaining > 0f;

        private void Awake()
        {
            _normalLayer = gameObject.layer;
        }

        public void SetPassThrough(float duration)
        {
            _remaining = Mathf.Max(_remaining, duration);
            gameObject.layer = passThroughLayer;
        }

        private void Update()
        {
            if (_remaining <= 0f) return;
            _remaining -= Time.deltaTime;
            if (_remaining <= 0f)
            {
                _remaining = 0f;
                gameObject.layer = _normalLayer;
            }
        }
    }
}
