using UnityEngine;
using Game.World;

namespace Game.Abilities
{
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
            PlayActivationSound(ctx.targetPoint);

            if (!ctx.TryGetTarget(out var target) || target == null) return;
            var growable = target.GetComponentInParent<IGrowable>();
            if (growable != null) growable.Grow();
        }
    }
}