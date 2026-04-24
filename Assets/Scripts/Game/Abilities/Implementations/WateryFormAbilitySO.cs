using UnityEngine;
using Game.World;

namespace Game.Abilities
{
    [CreateAssetMenu(menuName = "Abilities/Watery Form")]
    public sealed class WateryFormAbilitySO : AbilitySO
    {
        [Min(0f)]
        [Tooltip("How long the caster can pass through gates (design: 10 seconds).")]
        public float duration = 10f;

        public override bool CanActivate(AbilityContext ctx, out string reason)
        {
            if (!ctx.TryGetCaster(out var caster) || caster == null)
            {
                reason = "No caster";
                return false;
            }

            if (caster.GetComponentInChildren<IWateryForm>() == null)
            {
                reason = "Caster missing PlayerPassThrough component";
                return false;
            }

            reason = "";
            return true;
        }

        public override void Activate(AbilityContext ctx)
        {
            PlayActivationSound(ctx.targetPoint);

            if (!ctx.TryGetCaster(out var caster) || caster == null) return;
            var wateryForm = caster.GetComponentInChildren<IWateryForm>();
            if (wateryForm != null) wateryForm.SetPassThrough(duration);
        }
    }
}
