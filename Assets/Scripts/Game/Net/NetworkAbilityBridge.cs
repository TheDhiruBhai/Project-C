using UnityEngine;
using Game.CardsControllers;

namespace Game.Net
{
    public sealed class NetworkAbilityBridge : MonoBehaviour
    {
        [SerializeField] private bool enabledBridge;
        [SerializeField] private CardPlayController playController;
        [SerializeField] private Abilities.AbilitySystem authorityAbilitySystem;

        public bool Enabled => enabledBridge;

        public void SendAbilityRequest(AbilityRequest req)
        {
            // Replace with networking library.

            authorityAbilitySystem.ExecuteLocal(req, out var result);

            // Replace with broadcasting to all clients.
            playController.ApplyResultToLocalState(result);
        }
    }
}
