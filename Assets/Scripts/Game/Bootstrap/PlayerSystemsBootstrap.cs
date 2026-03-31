using UnityEngine;
using Game.World;
using Game.CardsControllers;
using Game.Player;

namespace Game.Bootstrap
{
    /// <summary>
    /// Wires together all player systems at startup.
    /// Add every player component to the same prefab root and assign references
    /// in the Inspector.
    ///
    /// Required components on the player prefab:
    ///   NetId, Health, HealthTargetAdapter,
    ///   PlayerElement, PlayerInvulnerability, PlayerPassThrough,
    ///   PlayerSpeedModifier, PlayerGroundCheck,
    ///   CardRuntimeController, CardPlayController,
    ///   WaterContactProbe, LightReceiver
    /// </summary>
    public sealed class PlayerSystemsBootstrap : MonoBehaviour
    {
        [SerializeField] private int playerNetId = 1;

        [Header("World")]
        [SerializeField] private WorldQueryBehaviour worldQuery;
        [SerializeField] private NetId netIdComponent;

        [Header("Cards")]
        [SerializeField] private CardRuntimeController cardRuntime;
        [SerializeField] private CardPlayController cardPlay;

        private void Awake()
        {
            // ── Network identity ───────────────────────────────────────────
            if (netIdComponent != null) netIdComponent.Set(playerNetId);
            if (worldQuery != null) worldQuery.Register(gameObject, playerNetId);

            // ── Card systems ───────────────────────────────────────────────
            if (cardRuntime != null) cardRuntime.Initialize(playerNetId);
            if (cardPlay != null) cardPlay.Initialize(playerNetId);

            // ── Validate buff components are present ───────────────────────
            RequireComponent<PlayerInvulnerability>("PlayerInvulnerability");
            RequireComponent<PlayerPassThrough>("PlayerPassThrough");
            RequireComponent<PlayerSpeedModifier>("PlayerSpeedModifier");
            RequireComponent<PlayerGroundCheck>("PlayerGroundCheck");
            RequireComponent<PlayerElement>("PlayerElement");
        }

        private void OnDestroy()
        {
            if (worldQuery != null) worldQuery.Unregister(playerNetId);
        }

        private void RequireComponent<T>(string label) where T : Component
        {
            if (GetComponentInChildren<T>() == null)
                Debug.LogWarning(
                    $"[PlayerBootstrap] Player {playerNetId} is missing a {label} component. " +
                    $"Add it to the player prefab or some card abilities will not work.");
        }
    }
}
