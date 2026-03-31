using UnityEngine;
using Game.World;

namespace Game.Abilities
{
    /// <summary>
    /// Grows a permanent walkable plant bridge between two dirt patches.
    /// Valid when pointing at a GrowableSurface in range while the caster
    /// is standing on a growable surface (confirmed by ground check).
    /// TargetType should be WorldObject on the CardDefinition.
    /// </summary>
    [CreateAssetMenu(menuName = "Abilities/Plant Growth")]
    public sealed class PlantGrowthAbilitySO : AbilitySO
    {
        public override bool CanActivate(AbilityContext ctx, out string reason)
        {
            if (!ctx.TryGetTarget(out var target) || target == null)
            {
                reason = "No target";
                return false;
            }

            var growable = target.GetComponentInParent<IGrowable>();
            if (growable == null)
            {
                reason = "Not a growable surface";
                return false;
            }

            if (growable.IsGrown)
            {
                reason = "Already grown";
                return false;
            }

            reason = "";
            return true;
        }

        public override void Activate(AbilityContext ctx)
        {
            if (!ctx.TryGetTarget(out var target) || target == null) return;
            var growable = target.GetComponentInParent<IGrowable>();
            if (growable != null) growable.Grow();
        }
    }
}