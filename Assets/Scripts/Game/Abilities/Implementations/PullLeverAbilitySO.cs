using UnityEngine;
using Game.World;

namespace Game.Abilities
{
    [CreateAssetMenu(menuName = "Abilities/Pull Lever")]
    public sealed class PullLeverAbilitySO : AbilitySO
    {
        public override bool CanActivate(AbilityContext ctx, out string reason)
        {
            if (!ctx.TryGetTarget(out var target) || target == null)
            {
                reason = "No target";
                return false;
            }
            if (target.GetComponentInParent<ILeverable>() == null)
            {
                reason = "Not a lever";
                return false;
            }
            reason = "";
            return true;
        }

        public override void Activate(AbilityContext ctx)
        {
            PlayActivationSound(ctx.targetPoint);

            if (!ctx.TryGetTarget(out var target) || target == null) return;
            var lever = target.GetComponentInParent<ILeverable>();
            if (lever != null) lever.Toggle();
        }
    }
}
