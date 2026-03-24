using UnityEngine;
using Game.World;

namespace Game.Abilities
{
    [CreateAssetMenu(menuName = "Abilities/Hot Hands")]
    public sealed class HotHandsAbilitySO : AbilitySO
    {
        [Min(0f)]
        [Tooltip("How many seconds the target burns (design: 3 seconds to clear moss).")]
        public float burnSeconds = 3f;

        public override bool CanActivate(AbilityContext ctx, out string reason)
        {
            if (!ctx.TryGetTarget(out var target) || target == null)
            {
                reason = "No target";
                return false;
            }
            if (target.GetComponentInParent<IFlammable>() == null)
            {
                reason = "Not flammable";
                return false;
            }
            reason = "";
            return true;
        }

        public override void Activate(AbilityContext ctx)
        {
            if (!ctx.TryGetTarget(out var target) || target == null) return;
            var flammable = target.GetComponentInParent<IFlammable>();
            if (flammable != null) flammable.Ignite(burnSeconds);
        }
    }
}
