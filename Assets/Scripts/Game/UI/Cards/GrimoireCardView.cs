using UnityEngine;
using UnityEngine.UI;
using Game.Cards;

namespace Game.UI
{
    public sealed class GrimoireCardView : MonoBehaviour
    {
        [Header("Display")]
        [SerializeField] private Image icon;
        [SerializeField] private Text costText;

        private CardDefinition definition;
        private System.Action<CardDefinition> _onClicked;
        private Button button;

        ///Populate the view with a card and register a purchase callback.
        public void Bind(CardDefinition def, System.Action<CardDefinition> onClicked)
        {
            definition = def;
            _onClicked = onClicked;

            if (icon != null)icon.sprite = def.icon;

            if (costText != null)
                costText.text = def.hpCost > 0 ? $"♥ {def.hpCost}" : "Free";

            // Get or add a Button so the whole card is clickable
            button = GetComponent<Button>();
            if (button == null) button = gameObject.AddComponent<Button>();

            button.onClick.RemoveAllListeners();

            var defCopy = def;
            button.onClick.AddListener(() => onClicked?.Invoke(defCopy));
        }

        ///Grey out the buy button if the player cannot afford this card.
        public void SetAffordable(bool affordable)
        {
            if (button != null) button.interactable = affordable;
        }

        private void OnBuyClicked()
        {
            if (definition != null) _onClicked?.Invoke(definition);
        }
    }
}
