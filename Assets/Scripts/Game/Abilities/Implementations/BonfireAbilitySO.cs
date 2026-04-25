using UnityEngine;
using Game.World;

namespace Game.Abilities
{
    [CreateAssetMenu(menuName = "Abilities/Bonfire")]
    public sealed class BonfireAbilitySO : AbilitySO
    {
        [Min(0f)] public float lightSeconds = 10f;

        public override bool CanActivate(AbilityContext ctx, out string reason)
        {
            if (!ctx.TryGetCaster(out var caster) || caster == null)
            {
                reason = "No caster";
                return false;
            }
            if (ctx.world != null && ctx.world.IsTouchingWater(caster))
            {
                reason = "Touching water";
                return false;
            }
            reason = "";
            return true;
        }

        public override void Activate(AbilityContext ctx)
        {
            if (!ctx.TryGetCaster(out var caster) || caster == null) return;
            PlayActivationSound(caster.transform.position);
            var receiver = caster.GetComponentInChildren<ILightReceiver>();
            if (receiver != null) receiver.Illuminate(lightSeconds);
        }
    }
}
