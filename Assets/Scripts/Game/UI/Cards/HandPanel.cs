using System.Collections.Generic;
using UnityEngine;
using Game.CardsControllers;
using Game.CardsRuntime;

namespace Game.UI
{
    public sealed class HandPanel : MonoBehaviour
    {
        [SerializeField] private CardRuntimeController    runtime;
        [SerializeField] private CardPlayController       playController;
        [SerializeField] private CardSelectionController  selectionController;
        [SerializeField] private Transform                contentRoot;
        [SerializeField] private CardView                 cardPrefab;

        private readonly List<CardView> spawned = new List<CardView>();
        private int _selectedIndex = 0;

        private void OnEnable()
        {
            if (runtime != null)
            {
                runtime.OnHandChanged         += Rebuild;
            }
            Rebuild();
        }

        private void OnDisable()
        {
            if (runtime != null)
            {
                runtime.OnHandChanged         -= Rebuild;
            }
        }

        ///Called by CardSelectionController when the selection changes.
        public void SetSelectedIndex(int index)
        {
            _selectedIndex = index;
            RefreshHighlights();
        }

        private void Rebuild()
        {
            if (runtime == null || contentRoot == null || cardPrefab == null) return;

            for (int i = 0; i < spawned.Count; i++)
                if (spawned[i] != null) Destroy(spawned[i].gameObject);
            spawned.Clear();

            var hand = runtime.HandCards;
            for (int i = 0; i < hand.Count; i++)
            {
                CardInstance inst = hand[i];
                var view = Instantiate(cardPrefab, contentRoot);
                // Pass selectionController so clicking a card syncs the index
                view.Bind(inst, playController, selectionController);
                spawned.Add(view);
            }

            RefreshHighlights();
        }

        private void RefreshHighlights()
        {
            for (int i = 0; i < spawned.Count; i++)
            {
                if (spawned[i] == null) continue;
                spawned[i].SetSelected(i == _selectedIndex);
            }
        }
    }
}
