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
                Debug.Log($"FreezeLiquid CanActivate: {reason}");
                return false;
            }

            var liquid = target.GetComponentInParent<ITransformableLiquid>();
            if (liquid == null)
            {
                reason = "Not liquid";
                Debug.Log($"FreezeLiquid CanActivate: {reason}");
                return false;
            }
            if (liquid.IsFrozen)
            {
                reason = "Already frozen";
                Debug.Log($"FreezeLiquid CanActivate: {reason}");
                return false;
            }

            reason = "";
            Debug.Log("FreezeLiquid CanActivate: SUCCESS");
            return true;
        }

        public override void Activate(AbilityContext ctx)
        {
            if (!ctx.TryGetTarget(out var target) || target == null)
            {
                Debug.Log("No target");
                return;
            }
            var liquid = target.GetComponentInParent<ITransformableLiquid>();
            Debug.Log($"Found Liquid: {liquid}, Type: {liquid?.GetType()}");

            if (liquid != null)
            {
                Debug.Log("Calling Freeze()");
                liquid.Freeze();
            }
        }
    }
}
