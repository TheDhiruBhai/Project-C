using UnityEngine;
using UnityEngine.UI;
using Game.Cards;

namespace Game.UI
{
    public sealed class GrimoireCardView : MonoBehaviour
    {
        [Header("Display")]
        [SerializeField] private Image icon;
        [SerializeField] private Text nameText;
        [SerializeField] private Text descText;
        [SerializeField] private Text rangeText;
        [SerializeField] private Text durationText;
        [SerializeField] private Text costText;
        [SerializeField] private Button buyButton;
        [SerializeField] private Image cardBackground;

        [Header("Element Colours")]
        [SerializeField] private Color fireColour = new Color(0.85f, 0.25f, 0.10f);
        [SerializeField] private Color waterColour = new Color(0.15f, 0.45f, 0.90f);
        [SerializeField] private Color earthColour = new Color(0.30f, 0.65f, 0.20f);
        [SerializeField] private Color airColour = new Color(0.75f, 0.90f, 0.95f);
        [SerializeField] private Color utilColour = new Color(0.75f, 0.65f, 0.45f);

        private CardDefinition _definition;
        private System.Action<CardDefinition> _onBuy;

        // ── Public API ─────────────────────────────────────────────────────

        ///Populate the view with a card and register a purchase callback.
        public void Bind(CardDefinition def, System.Action<CardDefinition> onBuy)
        {
            _definition = def;
            _onBuy = onBuy;

            if (icon != null)
                icon.sprite = def.icon;

            if (nameText != null)
                nameText.text = def.cardName;

            if (descText != null)
                descText.text = def.description;

            if (rangeText != null)
                rangeText.text = def.rangeMeters > 0f ? $"Range: {def.rangeMeters}m" : "Range: Self";

            if (durationText != null)
                durationText.text = def.durationSeconds > 0f
                    ? $"Duration: {def.durationSeconds}s"
                    : "Duration: Instant";

            if (costText != null)
                costText.text = def.hpCost > 0 ? $"♥ {def.hpCost}" : "Free";

            if (cardBackground != null)
                cardBackground.color = GetElementColour(def);

            if (buyButton != null)
            {
                buyButton.onClick.RemoveAllListeners();
                buyButton.onClick.AddListener(OnBuyClicked);
            }
        }

        ///Grey out the buy button if the player cannot afford this card.
        public void SetAffordable(bool affordable)
        {
            if (buyButton != null) buyButton.interactable = affordable;
        }

        // ── Internal ───────────────────────────────────────────────────────

        private void OnBuyClicked()
        {
            if (_definition != null) _onBuy?.Invoke(_definition);
        }

        private Color GetElementColour(CardDefinition def)
        {
            if (def.cardType == CardType.Utility) return utilColour;
            return def.elementRestriction switch
            {
                ElementType.Fire => fireColour,
                ElementType.Water => waterColour,
                ElementType.Earth => earthColour,
                ElementType.Air => airColour,
                _ => utilColour,
            };
        }
    }
}
