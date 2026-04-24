using UnityEngine;
using Game.World;

namespace Game.Abilities
{
    [CreateAssetMenu(menuName = "Abilities/Melt Ice")]
    public sealed class MeltIceAbilitySO : AbilitySO
    {
        public override bool CanActivate(AbilityContext ctx, out string reason)
        {
            if (!ctx.TryGetTarget(out var target) || target == null)
            {
                reason = "No target";
                return false;
            }

            var liquid = target.GetComponentInParent<ITransformableLiquid>();
            if (liquid == null)
            {
                reason = "Not a transformable liquid";
                return false;
            }
            if (!liquid.IsFrozen)
            {
                reason = "Not frozen";
                return false;
            }

            reason = "";
            return true;
        }

        public override void Activate(AbilityContext ctx)
        {
            PlayActivationSound(ctx.targetPoint);

            if (!ctx.TryGetTarget(out var target) || target == null) return;
            var liquid = target.GetComponentInParent<ITransformableLiquid>();
            if (liquid != null) liquid.Melt();
        }
    }
}
