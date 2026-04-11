using UnityEngine;
using Game.World;

namespace Game.Abilities
{
    /// <summary>
    /// Holds a moving/disappearing/pushable object completely still for a duration.
    /// Valid when pointing at any object that implements IHoldable, including players.
    /// TargetType should be WorldObject on the CardDefinition.
    /// </summary>
    [CreateAssetMenu(menuName = "Abilities/Stable Ground")]
    public sealed class StableGroundAbilitySO : AbilitySO
    {
        [Min(0f)]
        [Tooltip("How many seconds the target is held still (design: 10 seconds).")]
        public float holdSeconds = 10f;

        public override bool CanActivate(AbilityContext ctx, out string reason)
        {
            if (!ctx.TryGetTarget(out var target) || target == null)
            {
                reason = "No target";
                return false;
            }

            if (target.GetComponentInParent<IHoldable>() == null)
            {
                reason = "Target cannot be held";
                return false;
            }

            reason = "";
            return true;
        }

        public override void Activate(AbilityContext ctx)
        {
            if (!ctx.TryGetTarget(out var target) || target == null) return;
            var holdable = target.GetComponentInParent<IHoldable>();
            if (holdable != null) holdable.HoldStill(holdSeconds);
        }
    }
}
