using UnityEngine;
using UnityEngine.UI;
using Game.Cards;

namespace Game.UI
{
    public sealed class ConfirmPurchaseDialog : MonoBehaviour
    {
        [SerializeField] private GrimoireCardView previewCard;
        [SerializeField] private Text promptText;
        [SerializeField] private Button acceptButton;
        [SerializeField] private Button cancelButton;

        private CardDefinition _pending;
        private System.Action<CardDefinition> _onConfirm;

        private void Awake()
        {
            if (acceptButton != null) acceptButton.onClick.AddListener(OnAccept);
            if (cancelButton != null) cancelButton.onClick.AddListener(OnCancel);
            gameObject.SetActive(false);
        }

        // ── Public API ─────────────────────────────────────────────────────

        ///Open the dialog for a specific card purchase.
        public void Show(CardDefinition def, System.Action<CardDefinition> onConfirm)
        {
            _pending = def;
            _onConfirm = onConfirm;

            if (previewCard != null)
                previewCard.Bind(def, _ => { }); // preview only — no nested buy callback

            if (promptText != null)
                promptText.text = def.hpCost > 0
                    ? $"Buy {def.cardName} for {def.hpCost} HP?"
                    : $"Take {def.cardName}?";

            gameObject.SetActive(true);
        }

        public void Hide()
        {
            _pending = null;
            _onConfirm = null;
            gameObject.SetActive(false);
        }

        // ── Internal ───────────────────────────────────────────────────────

        private void OnAccept()
        {
            if (_pending != null) _onConfirm?.Invoke(_pending);
            Hide();
        }

        private void OnCancel() => Hide();
    }
}
