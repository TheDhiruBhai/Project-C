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

            // Check via IHealthTarget (adapter) — not Health directly
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
            PlayActivationSound(ctx.targetPoint);

            if (!ctx.TryGetTarget(out var target) || target == null) return;

            // ── FIX: go through IHealthTarget (HealthTargetAdapter) ───────
            // This hits the RPC path → heal is broadcast to all clients
            // OLD (wrong): var hp = target.GetComponentInParent<Health>();
            //              if (hp != null) hp.Heal(healAmount);
            var healthTarget = target.GetComponentInParent<IHealthTarget>();
            if (healthTarget != null)
                healthTarget.Heal(healAmount);
        }
    }
}