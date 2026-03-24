using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game.Cards;
using Game.Player;

namespace Game.UI
{
    public sealed class GrimoirePanel : MonoBehaviour
    {
        [Header("Panel Root")]
        [SerializeField] private GameObject grimoireRoot;

        [Header("Content Parents")]
        [SerializeField] private Transform elementalContent;
        [SerializeField] private Transform utilityContent;

        [Header("Prefab")]
        [SerializeField] private GrimoireCardView cardViewPrefab;

        [Header("Player Info")]
        [SerializeField] private Text playerHpText;
        [SerializeField] private Health health;

        [Header("Buttons")]
        [SerializeField] private Button closeButton;

        [Header("Systems")]
        [SerializeField] private CardShopController shopController;
        [SerializeField] private CardsControllers.CardPlayController cardPlayController;

        private readonly List<GrimoireCardView> _spawned = new List<GrimoireCardView>();
        private bool _isOpen = false;

        // ── Unity lifecycle ────────────────────────────────────────────────

        private void Awake()
        {
            if (closeButton != null)
                closeButton.onClick.AddListener(Close);

            if (grimoireRoot != null)
                grimoireRoot.SetActive(false);
        }

        private void OnEnable()
        {
            if (health != null)
                health.OnChanged += OnHealthChanged;
        }

        private void OnDisable()
        {
            if (health != null)
                health.OnChanged -= OnHealthChanged;
        }

        private void Update()
        {
            // Toggle with G key (design doc shows a 'G' book icon in the HUD).
            if (Input.GetKeyDown(KeyCode.G))
            {
                if (_isOpen) Close();
                else Open();
            }

            // Also close with Escape.
            if (_isOpen && Input.GetKeyDown(KeyCode.Escape))
                Close();
        }

        // ── Public API ─────────────────────────────────────────────────────

        public void Open()
        {
            if (_isOpen) return;
            _isOpen = true;

            if (grimoireRoot != null) grimoireRoot.SetActive(true);
            if (cardPlayController != null) cardPlayController.SetMenuBlocking(true);

            Rebuild();
            UpdateHpDisplay(health != null ? health.CurrentHp : 0,
                            health != null ? health.MaxHp : 0);
        }

        public void Close()
        {
            if (!_isOpen) return;
            _isOpen = false;

            if (grimoireRoot != null) grimoireRoot.SetActive(false);
            if (cardPlayController != null) cardPlayController.SetMenuBlocking(false);
        }

        // ── Internal ───────────────────────────────────────────────────────

        private void Rebuild()
        {
            // Destroy old views
            foreach (var v in _spawned)
                if (v != null) Destroy(v.gameObject);
            _spawned.Clear();

            if (shopController == null || cardViewPrefab == null) return;

            var available = shopController.GetAvailableCards();

            foreach (var def in available)
            {
                if (def == null) continue;

                // Choose which column this card belongs in
                Transform parent = def.cardType == CardType.Utility
                    ? utilityContent
                    : elementalContent;

                if (parent == null) continue;

                var view = Instantiate(cardViewPrefab, parent);
                view.Bind(def, OnCardBuyRequested);
                view.SetAffordable(shopController.CanAfford(def));
                _spawned.Add(view);
            }
        }

        private void RefreshAffordability()
        {
            if (shopController == null) return;
            foreach (var view in _spawned)
            {
                if (view == null) continue;
                // Re-check affordability after every HP change
                // GrimoireCardView.Bind stores the definition; we need to reach it.
                // We store a local mapping by keeping a parallel list for simplicity.
            }
            // Simplest safe approach: full rebuild when HP changes while panel is open.
            if (_isOpen) Rebuild();
        }

        private void OnCardBuyRequested(CardDefinition def)
        {
            if (shopController != null)
                shopController.RequestPurchase(def);
        }

        private void OnHealthChanged(int current, int max)
        {
            UpdateHpDisplay(current, max);
            RefreshAffordability();
        }

        private void UpdateHpDisplay(int current, int max)
        {
            if (playerHpText != null)
                playerHpText.text = $"♥ {current}";
        }
    }
}
