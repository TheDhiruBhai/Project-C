using UnityEngine;
using Game.World;

namespace Game.Abilities
{
    /// <summary>
    /// Makes the caster completely invulnerable to damage for a duration.
    /// TargetType should be Self on the CardDefinition.
    /// </summary>
    [CreateAssetMenu(menuName = "Abilities/Thick Skin")]
    public sealed class ThickSkinAbilitySO : AbilitySO
    {
        [Min(0f)]
        [Tooltip("How long the player is invulnerable (design: 30 seconds).")]
        public float duration = 30f;

        public override bool CanActivate(AbilityContext ctx, out string reason)
        {
            if (!ctx.TryGetCaster(out var caster) || caster == null)
            {
                reason = "No caster";
                return false;
            }

            if (caster.GetComponentInChildren<IDamageImmune>() == null)
            {
                reason = "Caster missing PlayerInvulnerability component";
                return false;
            }

            reason = "";
            return true;
        }

        public override void Activate(AbilityContext ctx)
        {
            if (!ctx.TryGetCaster(out var caster) || caster == null) return;
            var immune = caster.GetComponentInChildren<IDamageImmune>();
            if (immune != null) immune.SetInvulnerable(duration);
        }
    }
}
