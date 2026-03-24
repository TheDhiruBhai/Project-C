using UnityEngine;
using Game.World;

namespace Game.Abilities
{
    [CreateAssetMenu(menuName = "Abilities/Heal")]
    public sealed class HealAbilitySO : AbilitySO
    {
        [Min(0)] public int healAmount = 15;

        public override bool CanActivate(AbilityContext ctx, out string reason)
        {
            if (!ctx.TryGetTarget(out var target) || target == null)
            {
                reason = "No target";
                return false;
            }
            if (target.GetComponentInParent<IHealthTarget>() == null)
            {
                reason = "Target cannot be healed";
                return false;
            }
            reason = "";
            return true;
        }

        public override void Activate(AbilityContext ctx)
        {
            if (!ctx.TryGetTarget(out var target) || target == null) return;
            var hp = target.GetComponentInParent<IHealthTarget>();
            if (hp != null) hp.Heal(healAmount);
        }
    }
}
