using UnityEngine;
using Game.World;

namespace Game.Abilities
{
    /// <summary>
    /// Spawns a temporary area light at the targeted world point.
    /// TargetType should be Point on the CardDefinition.
    ///
    /// Setup: Assign lightPrefab to a prefab containing a TemporaryAreaLight component.
    /// </summary>
    [CreateAssetMenu(menuName = "Abilities/Hot Flash")]
    public sealed class HotFlashAbilitySO : AbilitySO
    {
        [Min(0f)] public float lightRadius = 10f;
        [Min(0f)] public float lightDuration = 5f;

        [Tooltip("Prefab with a TemporaryAreaLight component and a Point Light.")]
        public GameObject lightPrefab;

        public override bool CanActivate(AbilityContext ctx, out string reason)
        {
            // Valid anywhere the player chooses to aim within range.
            reason = "";
            return true;
        }

        public override void Activate(AbilityContext ctx)
        {
            if (lightPrefab == null)
            {
                Debug.LogWarning("[HotFlash] No light prefab assigned on HotFlashAbilitySO.");
                return;
            }

            var go = Object.Instantiate(lightPrefab, ctx.targetPoint, Quaternion.identity);
            var light = go.GetComponent<TemporaryAreaLight>();
            if (light != null) light.Initialize(lightRadius, lightDuration);
        }
    }
}
