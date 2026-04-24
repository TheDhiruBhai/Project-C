using UnityEngine;
using Game.World;

namespace Game.Abilities
{
    [CreateAssetMenu(menuName = "Abilities/Pulling Gust")]
    public sealed class PullingGustAbilitySO : AbilitySO
    {
        public override bool CanActivate(AbilityContext ctx, out string reason)
        {
            if (!ctx.TryGetTarget(out var target) || target == null)
            {
                reason = "No target";
                return false;
            }

            var movable = target.GetComponentInParent<IMovable>();
            if (movable == null)
            {
                reason = "Not a movable object";
                return false;
            }

            if (!ctx.TryGetCaster(out var caster) || caster == null)
            {
                reason = "No caster";
                return false;
            }

            if (!movable.CanPull(caster.transform.position))
            {
                reason = "Object not in pull position";
                return false;
            }

            reason = "";
            return true;
        }

        public override void Activate(AbilityContext ctx)
        {
            PlayActivationSound(ctx.targetPoint);

            if (!ctx.TryGetTarget(out var target) || target == null) return;
            if (!ctx.TryGetCaster(out var caster) || caster == null) return;
            var movable = target.GetComponentInParent<IMovable>();
            if (movable != null) movable.Pull(caster.transform.position);
        }
    }
}
