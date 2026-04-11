using System.Collections.Generic;
using UnityEngine;
using Game.Cards;
using Game.Player;

namespace Game.UI
{
    public sealed class CardShopController : MonoBehaviour
    {
        [Header("Data")]
        [Tooltip("All CardDefinition assets in the game — assign every card here.")]
        [SerializeField] private List<CardDefinition> cardDatabase = new List<CardDefinition>();

        [Header("Player References")]
        [SerializeField] private Health health;
        [SerializeField] private PlayerElement playerElement;
        [SerializeField] private CardsControllers.CardRuntimeController cardRuntime;

        [Header("UI References")]
        [SerializeField] private ConfirmPurchaseDialog confirmDialog;

        // ── Public API ─────────────────────────────────────────────────────

        ///Returns every card this player is allowed to see in the shop.
      
        
        
        public List<CardDefinition> GetAvailableCards()
        {
            var result = new List<CardDefinition>();
            var element = playerElement != null ? playerElement.Element : ElementType.Any;

            foreach (var def in cardDatabase)
            {
                if (def == null) continue;

                bool isUtility = def.cardType == CardType.Utility;
                bool matchesElem = def.elementRestriction == ElementType.Any ||
                                   def.elementRestriction == element;

                if (isUtility || matchesElem)
                    result.Add(def);
            }
            return result;
        }

        ///Returns true if the player currently has enough HP to buy this card.
        public bool CanAfford(CardDefinition def)
        {
            if (def == null) return false;
            if (health == null) return true; // No health component — allow purchase.
            return health.CanSpend(def.hpCost);
        }

        public void RequestPurchase(CardDefinition def)
        {
            if (!CanAfford(def))
            {
                Debug.Log($"[Shop] Cannot afford {def.cardName} — need {def.hpCost} HP.");
                return;
            }

            if (confirmDialog != null)
                confirmDialog.Show(def, OnPurchaseConfirmed);
            else
                OnPurchaseConfirmed(def); // No dialog — purchase immediately.
        }

        // ── Internal ───────────────────────────────────────────────────────

        private void OnPurchaseConfirmed(CardDefinition def)
        {
            if (!CanAfford(def)) return; // Re-check in case HP changed while dialog was open.

            if (health != null && def.hpCost > 0)
                health.TrySpend(def.hpCost);

            if (cardRuntime != null)
                cardRuntime.AddCardToCollection(def);

            Debug.Log($"[Shop] Purchased {def.cardName} for {def.hpCost} HP.");
        }
    }
}
