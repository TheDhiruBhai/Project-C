using UnityEngine;
using Game.World;

namespace Game.Abilities
{
    [CreateAssetMenu(menuName = "Abilities/Freeze Liquid")]
    public sealed class FreezeLiquidAbilitySO : AbilitySO
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
                reason = "Not liquid";
                return false;
            }
            if (liquid.IsFrozen)
            {
                reason = "Already frozen";
                return false;
            }

            reason = "";
            return true;
        }

        public override void Activate(AbilityContext ctx)
        {
            if (!ctx.TryGetTarget(out var target) || target == null) return;
            var liquid = target.GetComponentInParent<ITransformableLiquid>();
            if (liquid != null) liquid.Freeze();
        }
    }
}
