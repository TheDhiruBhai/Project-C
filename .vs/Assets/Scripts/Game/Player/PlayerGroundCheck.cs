using UnityEngine;

namespace Game.Player
{
    public sealed class PlayerGroundCheck : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Radius of the overlap sphere cast at the player's feet.")]
        private float groundCheckRadius = 0.3f;

        [SerializeField]
        [Tooltip("How far below the player's pivot to cast (tune to character height).")]
        private float groundCheckDistance = 1.1f;

        [SerializeField]
        [Tooltip("Layers considered as ground. Set to everything except the Player layer.")]
        private LayerMask groundMask = ~0;

        public bool IsGrounded { get; private set; }

        private void Update()
        {
            var origin = transform.position + Vector3.up * 0.05f;
            IsGrounded = Physics.SphereCast(
                origin,
                groundCheckRadius,
                Vector3.down,
                out _,
                groundCheckDistance,
                groundMask,
                QueryTriggerInteraction.Ignore
            );
        }
    }
}
