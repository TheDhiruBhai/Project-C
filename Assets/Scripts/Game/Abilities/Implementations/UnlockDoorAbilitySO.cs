using UnityEngine;
using Game.World;

namespace Game.Abilities
{
    [CreateAssetMenu(menuName = "Abilities/Unlock Door")]
    public sealed class UnlockDoorAbilitySO : AbilitySO
    {
        public override bool CanActivate(AbilityContext ctx, out string reason)
        {
            if (!ctx.TryGetTarget(out var target) || target == null)
            {
                reason = "No target";
                return false;
            }

            var lockable = target.GetComponentInParent<ILockable>();
            if (lockable == null)
            {
                reason = "Not lockable";
                return false;
            }
            if (!lockable.IsLocked)
            {
                reason = "Already unlocked";
                return false;
            }

            reason = "";
            return true;
        }

        public override void Activate(AbilityContext ctx)
        {
            if (!ctx.TryGetTarget(out var target) || target == null) return;
            var lockable = target.GetComponentInParent<ILockable>();
            if (lockable != null) lockable.Unlock();
        }
    }
}
