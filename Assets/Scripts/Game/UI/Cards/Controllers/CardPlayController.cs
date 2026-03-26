using UnityEngine;
using Game.CardsControllers;
using Game.Player;
using Game.Net;
using Game.Abilities;
using Game.Cards;

namespace Game.CardsControllers
{
    public sealed class CardPlayController : MonoBehaviour
    {
        [SerializeField] private CardRuntimeController cardRuntime;
        [SerializeField] private Health health;
        [SerializeField] private AbilitySystem abilitySystem;
        [SerializeField] private TargetingController targeting;
        [SerializeField] private NetworkAbilityBridge networkBridge;

        [SerializeField] private bool blockWhileInMenu = false;
        [SerializeField] private bool isStunned = false;

        private int _ownerNetId;
        private PlayerElement playerElement;

        public void Initialize(int ownerNetId)
        {
            _ownerNetId = ownerNetId;
            playerElement = GetComponent<PlayerElement>();
        }

        public void SetMenuBlocking(bool value) => blockWhileInMenu = value;
        public void SetStunned(bool value) => isStunned = value;

        public void TryPlay(int cardInstanceId)
        {
            if (blockWhileInMenu) return;
            if (isStunned) return;

            var card = cardRuntime.GetHandCardById(cardInstanceId);
            if (card == null || card.definition == null || card.definition.ability == null) return;

            if (card.IsOnCooldown) return;

            // ── Element restriction check ──────────────────────────────────
            if (card.definition.elementRestriction != ElementType.Any)
            {
                if (playerElement == null ||
                    playerElement.Element != card.definition.elementRestriction)
                {
                    Debug.Log($"[CardPlay] Cannot play {card.definition.cardName}: wrong element.");
                    return;
                }
            }

            var ability    = card.definition.ability;
            var targetType = ability.TargetType;

            // ── Self / None cards fire immediately on the activation key ───
            if (targetType == TargetType.None || targetType == TargetType.Self)
            {
                var req = AbilityRequest.Build(card.instanceId, _ownerNetId, null, Vector3.zero);
                SendOrExecute(req);
                return;
            }

            // ── Targeted cards begin the targeting phase ───────────────────
            targeting.Begin(card.instanceId, _ownerNetId, card.definition.rangeMeters, ability);
        }

        public void ConfirmTargetAndPlay(int cardInstanceId, int casterNetId,
                                         int? targetNetId, Vector3 targetPoint)
        {
            var req = AbilityRequest.Build(cardInstanceId, casterNetId, targetNetId, targetPoint);
            SendOrExecute(req);
        }

        private void SendOrExecute(AbilityRequest req)
        {
            if (networkBridge != null && networkBridge.Enabled)
            {
                networkBridge.SendAbilityRequest(req);
                return;
            }

            abilitySystem.ExecuteLocal(req, out var result);
            ApplyResultToLocalState(result);
        }

        public void ApplyResultToLocalState(AbilityResult result)
        {
            if (!result.approved) return;

            var card = cardRuntime.GetHandCardById(result.cardInstanceId);
            if (card == null) return;

            cardRuntime.StartCooldown(card);
            cardRuntime.DiscardFromHand(card);
        }
    }
}
