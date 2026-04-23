using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game.Cards;
using Game.Player;

namespace Game.UI
{
    public sealed class GrimoirePanel : MonoBehaviour
    {
        private const int CardsPerPage = 3;

        [Header("Panel Root")]
        [SerializeField] private GameObject grimoireRoot;

        [Header("Elemental Section")]
        [SerializeField] private GameObject elementalFrame;   
        [SerializeField] private Transform elementalCardHolder;

        [Header("Utility Section")]
        [SerializeField] private GameObject utilityFrame;     
        [SerializeField] private Transform utilityCardHolder;

        [Header("Prefab")]
        [SerializeField] private GrimoireCardView cardViewPrefab;

        [Header("Player Info")]
        [SerializeField] private Text playerHpText;
        [SerializeField] private Health health;

        // Close button is wired via UIButton OnClick in the Inspector.

        [Header("Systems")]
        [SerializeField] private CardShopController shopController;
        [SerializeField] private CardsControllers.CardPlayController cardPlayController;

        private bool _isOpen = false;

        private List<CardDefinition> _elementalCards = new List<CardDefinition>();
        private List<CardDefinition> _utilityCards = new List<CardDefinition>();

        private int _elementalPage = 0;
        private int _utilityPage = 0;

        // Pooled views — one per slot in each holder (CardsPerPage each)
        private GrimoireCardView[] _elementalViews;
        private GrimoireCardView[] _utilityViews;

        private void Awake()
        {
            if (grimoireRoot != null)
                grimoireRoot.SetActive(false);

            // Pre-create pooled views so we just rebind them on each page turn
            // instead of destroying/instantiating every time.
            _elementalViews = CreateViewPool(elementalCardHolder);
            _utilityViews = CreateViewPool(utilityCardHolder);
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
            if (Input.GetKeyDown(KeyCode.G))
            {
                if (_isOpen) Close();
                else Open();
            }

            if (_isOpen && Input.GetKeyDown(KeyCode.Escape))
                Close();
        }

        public void Open()
        {
            if (_isOpen) return;
            _isOpen = true;

            if (grimoireRoot != null) grimoireRoot.SetActive(true);

            if (elementalFrame != null) elementalFrame.SetActive(true);
            if (utilityFrame != null) utilityFrame.SetActive(false);

            if (cardPlayController != null) cardPlayController.SetMenuBlocking(true);

            RefreshCardLists();

            _elementalPage = 0;
            _utilityPage = 0;

            RenderPage(_elementalViews, _elementalCards, _elementalPage);
            RenderPage(_utilityViews, _utilityCards, _utilityPage);

            UpdateNavButtons();
            UpdateHpDisplay();

            Cursor.visible = true;
        }

        public void Close()
        {
            if (!_isOpen) return;
            _isOpen = false;

            if (grimoireRoot != null) grimoireRoot.SetActive(false);
            if (cardPlayController != null) cardPlayController.SetMenuBlocking(false);

            Cursor.visible = false;
        }

        // ── Pagination (public so UIButton OnClick events can call them) ───

        public void ElementalNext()
        {
            int maxPage = MaxPage(_elementalCards);
            if (_elementalPage < maxPage)
            {
                _elementalPage++;
                RenderPage(_elementalViews, _elementalCards, _elementalPage);
                UpdateNavButtons();
            }
        }

        public void ElementalBack()
        {
            if (_elementalPage > 0)
            {
                _elementalPage--;
                RenderPage(_elementalViews, _elementalCards, _elementalPage);
                UpdateNavButtons();
            }
        }

        public void UtilityNext()
        {
            int maxPage = MaxPage(_utilityCards);
            if (_utilityPage < maxPage)
            {
                _utilityPage++;
                RenderPage(_utilityViews, _utilityCards, _utilityPage);
                UpdateNavButtons();
            }
        }

        public void UtilityBack()
        {
            if (_utilityPage > 0)
            {
                _utilityPage--;
                RenderPage(_utilityViews, _utilityCards, _utilityPage);
                UpdateNavButtons();
            }
        }

        /// Fills the 3 view slots for the given page of the given card list.
        /// Slots with no card are hidden.
        private void RenderPage(GrimoireCardView[] views, List<CardDefinition> cards, int page)
        {
            int startIndex = page * CardsPerPage;

            for (int slot = 0; slot < CardsPerPage; slot++)
            {
                int cardIndex = startIndex + slot;
                bool hasCard = cardIndex < cards.Count;

                var view = views[slot];
                if (view == null) continue;

                view.gameObject.SetActive(hasCard);

                if (hasCard)
                {
                    var def = cards[cardIndex];
                    view.Bind(def, OnCardBuyRequested);
                    view.SetAffordable(shopController != null && shopController.CanAfford(def));
                }
            }
        }

        private void UpdateNavButtons()
        {
            // Nav button greying is handled here if you later add Button references.
            // For now, Next/Back are always interactable — pages that don't exist
            // simply do nothing when called (guards are inside each method above).
        }

        private void RefreshAffordability(GrimoireCardView[] views, List<CardDefinition> cards, int page)
        {
            if (shopController == null) return;

            int startIndex = page * CardsPerPage;
            for (int slot = 0; slot < CardsPerPage; slot++)
            {
                int cardIndex = startIndex + slot;
                if (cardIndex >= cards.Count) break;
                views[slot]?.SetAffordable(shopController.CanAfford(cards[cardIndex]));
            }
        }

        private void OnCardBuyRequested(CardDefinition def)
        {
            if (shopController == null || !shopController.CanAfford(def)) return;

            shopController.RequestPurchase(def);

            // Refresh affordability on both visible pages after the purchase
            RefreshAffordability(_elementalViews, _elementalCards, _elementalPage);
            RefreshAffordability(_utilityViews, _utilityCards, _utilityPage);

            UpdateHpDisplay();
        }

        private void RefreshCardLists()
        {
            _elementalCards.Clear();
            _utilityCards.Clear();

            if (shopController == null) return;

            var all = shopController.GetAvailableCards();
            foreach (var def in all)
            {
                if (def == null) continue;
                if (def.cardType == CardType.Utility)
                    _utilityCards.Add(def);
                else
                    _elementalCards.Add(def);
            }
        }

        ///Returns the index of the last valid page (0-based).
        private static int MaxPage(List<CardDefinition> cards)
        {
            if (cards.Count == 0) return 0;
            return (cards.Count - 1) / CardsPerPage;
        }

        /// Creates CardsPerPage GrimoireCardView instances as children of the
        /// given holder. Existing children are reused if they already have the
        /// component; new ones are instantiated from the prefab for any missing slots.
        private GrimoireCardView[] CreateViewPool(Transform holder)
        {
            var pool = new GrimoireCardView[CardsPerPage];
            if (holder == null || cardViewPrefab == null) return pool;

            for (int i = 0; i < CardsPerPage; i++)
            {
                // Reuse existing child if present
                if (i < holder.childCount)
                {
                    var existing = holder.GetChild(i).GetComponent<GrimoireCardView>();
                    if (existing != null)
                    {
                        pool[i] = existing;
                        continue;
                    }
                }
                // Otherwise instantiate from prefab
                pool[i] = Instantiate(cardViewPrefab, holder);
            }

            return pool;
        }

        private void OnHealthChanged(int current, int max)
        {
            UpdateHpDisplay();

            if (!_isOpen) return;

            // Refresh affordability on current visible pages
            RefreshAffordability(_elementalViews, _elementalCards, _elementalPage);
            RefreshAffordability(_utilityViews, _utilityCards, _utilityPage);
        }

        private void UpdateHpDisplay()
        {
            if (playerHpText != null && health != null)
                playerHpText.text = $"♥ {health.CurrentHp}";
        }
    }
}