using UnityEngine;
using Game.World;

namespace Game.Abilities
{
    [CreateAssetMenu(menuName = "Abilities/Damage Target")]
    public sealed class DamageTargetAbilitySO : AbilitySO
    {
        [Min(0)] public int damage = 20;

        public override bool CanActivate(AbilityContext ctx, out string reason)
        {
            if (!ctx.TryGetTarget(out var target) || target == null)
            {
                reason = "No target";
                return false;
            }
            if (target.GetComponentInParent<IHealthTarget>() == null)
            {
                reason = "Invalid target";
                return false;
            }
            reason = "";
            return true;
        }

        public override void Activate(AbilityContext ctx)
        {
            if (!ctx.TryGetTarget(out var target) || target == null) return;
            var hp = target.GetComponentInParent<IHealthTarget>();
            if (hp != null) hp.TakeDamage(damage);
        }
    }
}
