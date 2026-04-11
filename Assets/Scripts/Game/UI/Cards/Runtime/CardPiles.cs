using UnityEngine;
using System;
using System.Collections.Generic;

namespace Game.CardsRuntime
{
    public sealed class Deck
    {
        private readonly List<CardInstance> _cards = new List<CardInstance>();
        private readonly System.Random _rng = new System.Random();

        public int Count => _cards.Count;

        public void Add(CardInstance c) => _cards.Add(c);

        public void Shuffle()
        {
            for (int i = _cards.Count - 1; i > 0; i--)
            {
                int j = _rng.Next(i + 1);
                (_cards[i], _cards[j]) = (_cards[j], _cards[i]);
            }
        }

        public bool TryDraw(out CardInstance c)
        {
            c = null;
            if (_cards.Count == 0) return false;
            int last = _cards.Count - 1;
            c = _cards[last];
            _cards.RemoveAt(last);
            return true;
        }
    }

    public sealed class Hand
    {
        private readonly List<CardInstance> _cards = new List<CardInstance>();
        public IReadOnlyList<CardInstance> Cards => _cards;
        public int Limit { get; }

        public Hand(int limit) { Limit = limit; }

        public bool TryAdd(CardInstance c)
        {
            if (_cards.Count >= Limit) return false;
            _cards.Add(c);
            return true;
        }

        public bool Remove(CardInstance c) => _cards.Remove(c);

        public CardInstance GetByInstanceId(int id)
        {
            for (int i = 0; i < _cards.Count; i++)
                if (_cards[i].instanceId == id) return _cards[i];
            return null;
        }
    }

    public sealed class DiscardPile
    {
        private readonly List<CardInstance> _cards = new List<CardInstance>();
        public int Count => _cards.Count;
        public void Add(CardInstance c) => _cards.Add(c);
    }
}
