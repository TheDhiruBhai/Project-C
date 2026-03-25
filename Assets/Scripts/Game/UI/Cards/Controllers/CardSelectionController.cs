using UnityEngine;
using Game.CardsControllers;

namespace Game.CardsControllers
{
    public sealed class CardSelectionController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CardRuntimeController cardRuntime;
        [SerializeField] private CardPlayController    cardPlay;
        [SerializeField] private Game.UI.HandPanel     handPanel;

        [Header("Input")]
        [SerializeField] private KeyCode cycleLeftKey  = KeyCode.Q;
        [SerializeField] private KeyCode cycleRightKey = KeyCode.E;
        [SerializeField] private KeyCode activateKey   = KeyCode.F;

        private int selectedIndex = 0;

        public int SelectedIndex => selectedIndex;

        private void Update()
        {
            if (cardRuntime == null || cardPlay == null) return;

            int handCount = cardRuntime.HandCards.Count;
            if (handCount == 0) return;

            //Selection input
            bool selectionChanged = false;

            if (Input.GetKeyDown(cycleLeftKey))
            {
                selectedIndex = (selectedIndex - 1 + handCount) % handCount;
                selectionChanged = true;
            }
            else if (Input.GetKeyDown(cycleRightKey))
            {
                selectedIndex = (selectedIndex + 1) % handCount;
                selectionChanged = true;
            }
            else
            {
                // Number keys 1–5
                for (int i = 0; i < Mathf.Min(handCount, 5); i++)
                {
                    if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                    {
                        selectedIndex   = i;
                        selectionChanged = true;
                        break;
                    }
                }
            }

            // Clamp in case the hand shrank after a card was played
            if (selectedIndex >= handCount)
            {
                selectedIndex   = handCount - 1;
                selectionChanged = true;
            }

            if (selectionChanged)
                NotifyHandPanel();

            if (Input.GetKeyDown(activateKey))
            {
                var hand = cardRuntime.HandCards;
                if (selectedIndex < hand.Count)
                    cardPlay.TryPlay(hand[selectedIndex].instanceId);
            }
        }

        public void SelectByInstanceId(int instanceId)
        {
            var hand = cardRuntime.HandCards;
            for (int i = 0; i < hand.Count; i++)
            {
                if (hand[i].instanceId == instanceId)
                {
                    selectedIndex = i;
                    NotifyHandPanel();
                    return;
                }
            }
        }

        private void NotifyHandPanel()
        {
            if (handPanel != null)
                handPanel.SetSelectedIndex(selectedIndex);
        }
    }
}
