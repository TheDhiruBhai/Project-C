using System.Collections.Generic;
using UnityEngine;
using Game.CardsControllers;
using Game.CardsRuntime;

namespace Game.UI
{
    public sealed class HandPanel : MonoBehaviour
    {
        [SerializeField] private CardRuntimeController runtime;
        [SerializeField] private CardPlayController playController;
        [SerializeField] private Transform contentRoot;
        [SerializeField] private CardView cardPrefab;

        private readonly List<CardView> _spawned = new List<CardView>();

        private void OnEnable()
        {
            if (runtime != null)
            {
                runtime.OnHandChanged += Rebuild;
                runtime.OnCardCooldownChanged += OnCooldownChanged;
            }
            Rebuild();
        }

        private void OnDisable()
        {
            if (runtime != null)
            {
                runtime.OnHandChanged -= Rebuild;
                runtime.OnCardCooldownChanged -= OnCooldownChanged;
            }
        }

        private void Rebuild()
        {
            if (runtime == null || contentRoot == null || cardPrefab == null) return;

            for (int i = 0; i < _spawned.Count; i++)
                if (_spawned[i] != null) Destroy(_spawned[i].gameObject);
            _spawned.Clear();

            var hand = runtime.HandCards;
            for (int i = 0; i < hand.Count; i++)
            {
                CardInstance inst = hand[i];
                var view = Instantiate(cardPrefab, contentRoot);
                view.Bind(inst, playController);
                _spawned.Add(view);
            }
        }

        private void OnCooldownChanged(int cardInstanceId, float cooldownRemaining)
        {
            for (int i = 0; i < _spawned.Count; i++)
            {
                var view = _spawned[i];
                if (view == null) continue;

                if (view.InstanceId == cardInstanceId)
                {
                    view.UpdateCooldown(cooldownRemaining);
                    return;
                }
            }
        }
    }
}
