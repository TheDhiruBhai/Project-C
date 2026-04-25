using UnityEngine;
using Game.World;
using Game.Player;

namespace Game.Abilities
{
    [CreateAssetMenu(menuName = "Abilities/Wind Walk")]
    public sealed class WindWalkAbilitySO : AbilitySO
    {
        [Min(1f)]
        [Tooltip("Speed multiplier applied to the caster (design: 2x).")]
        public float speedMultiplier = 2f;

        [Min(0f)]
        [Tooltip("How long the speed boost lasts (design: 10 seconds).")]
        public float duration = 10f;

        public override bool CanActivate(AbilityContext ctx, out string reason)
        {
            if (!ctx.TryGetCaster(out var caster) || caster == null)
            {
                reason = "No caster";
                return false;
            }

            var modifier = caster.GetComponentInChildren<ISpeedModifiable>();
            if (modifier == null)
            {
                reason = "Caster missing PlayerSpeedModifier component";
                return false;
            }

            // We check for a grounded flag via the PlayerGroundCheck helper if present.
            var groundCheck = caster.GetComponentInChildren<PlayerGroundCheck>();
            if (groundCheck != null && !groundCheck.IsGrounded)
            {
                reason = "Cannot use while airborne";
                return false;
            }

            reason = "";
            return true;
        }

        public override void Activate(AbilityContext ctx)
        {
            if (!ctx.TryGetCaster(out var caster) || caster == null) return;
            PlayActivationSound(caster.transform.position);
            var modifier = caster.GetComponentInChildren<ISpeedModifiable>();
            if (modifier != null) modifier.ApplySpeedMultiplier(speedMultiplier, duration);
        }
    }
}
