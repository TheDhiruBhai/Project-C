using UnityEngine;
using Game.World;
using Game.CardsControllers;

namespace Game.Bootstrap
{
    public sealed class PlayerSystemsBootstrap : MonoBehaviour
    {
        [SerializeField] private int playerNetId = 1;

        [Header("References")]
        [SerializeField] private WorldQueryBehaviour worldQuery;
        [SerializeField] private NetId netIdComponent;

        [SerializeField] private CardRuntimeController cardRuntime;
        [SerializeField] private CardPlayController cardPlay;

        private void Awake()
        {
            if (netIdComponent != null) netIdComponent.Set(playerNetId);
            if (worldQuery != null) worldQuery.Register(gameObject, playerNetId);

            if (cardRuntime != null) cardRuntime.Initialize(playerNetId);
            if (cardPlay != null) cardPlay.Initialize(playerNetId);
        }

        private void OnDestroy()
        {
            if (worldQuery != null) worldQuery.Unregister(playerNetId);
        }
    }
}
