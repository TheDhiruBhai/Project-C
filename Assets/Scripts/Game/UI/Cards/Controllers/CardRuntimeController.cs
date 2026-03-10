using System;
using System.Collections.Generic;
using UnityEngine;
using Game.Cards;
using Game.CardsRuntime;

namespace Game.CardsControllers
{
    public sealed class CardRuntimeController : MonoBehaviour
    {
        [Header("Initial Deck")]
        [SerializeField] private List<CardDefinition> startingDeck = new List<CardDefinition>();
        [SerializeField] private int handLimit = 5;

        private int _nextInstanceId = 1;
        private Deck _deck;
        private Hand _hand;
        private DiscardPile _discard;

        public event Action OnHandChanged;
        public event Action<int, float> OnCardCooldownChanged;

        public IReadOnlyList<CardInstance> HandCards => _hand.Cards;

        public void Initialize(int ownerPlayerNetId)
        {
            _deck = new Deck();
            _hand = new Hand(handLimit);
            _discard = new DiscardPile();

            for (int i = 0; i < startingDeck.Count; i++)
            {
                var def = startingDeck[i];
                if (def == null) continue;
                var inst = new CardInstance(_nextInstanceId++, ownerPlayerNetId, def);
                _deck.Add(inst);
            }
            _deck.Shuffle();

            DrawUpToLimit();
        }

        private void Update()
        {
            if (_hand == null) return;

            bool anyCooldownTicked = false;

            for (int i = 0; i < _hand.Cards.Count; i++)
            {
                var c = _hand.Cards[i];
                if (c.cooldownRemaining > 0f)
                {
                    c.cooldownRemaining = Mathf.Max(0f, c.cooldownRemaining - Time.deltaTime);
                    anyCooldownTicked = true;
                    OnCardCooldownChanged?.Invoke(c.instanceId, c.cooldownRemaining);
                }
            }

            if (anyCooldownTicked)
                OnHandChanged?.Invoke();
        }

        public CardInstance GetHandCardById(int instanceId) => _hand.GetByInstanceId(instanceId);

        public void StartCooldown(CardInstance card)
        {
            if (card == null || card.definition == null) return;
            card.cooldownRemaining = Mathf.Max(0f, card.definition.cooldownSeconds);
            OnCardCooldownChanged?.Invoke(card.instanceId, card.cooldownRemaining);
        }

        public void DiscardFromHand(CardInstance card)
        {
            if (card == null) return;
            if (_hand.Remove(card))
            {
                _discard.Add(card);
                OnHandChanged?.Invoke();
                DrawUpToLimit();
            }
        }

        public void DrawUpToLimit()
        {
            if (_deck == null || _hand == null) return;
            while (_hand.Cards.Count < _hand.Limit)
            {
                if (!_deck.TryDraw(out var next)) break;
                _hand.TryAdd(next);
            }
            OnHandChanged?.Invoke();
        }
    }
}