using UnityEngine;
using Game.CardsControllers;

namespace Game.CardsControllers
{
    public sealed class CardSelectionController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CardRuntimeController cardRuntime;
        [SerializeField] private CardPlayController cardPlay;
        [SerializeField] private TargetingController targeting;
        [SerializeField] private Game.UI.HandPanel handPanel;

        [Header("Input")]
        [SerializeField] private float scrollThreshold = 0.01f;

        private int selectedIndex = 0;

        public int SelectedIndex => selectedIndex;

        private void Update()
        {
            if (cardRuntime == null || cardPlay == null) return;

            int handCount = cardRuntime.HandCards.Count;
            if (handCount == 0) return;

            bool selectionChanged = false;

            // Scroll wheel cycling
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll > scrollThreshold)
            {
                selectedIndex = (selectedIndex - 1 + handCount) % handCount;
                selectionChanged = true;
            }
            else if (scroll < -scrollThreshold)
            {
                selectedIndex = (selectedIndex + 1) % handCount;
                selectionChanged = true;
            }
            else
            {
                // Number keys 1–7
                for (int i = 0; i < Mathf.Min(handCount, 7); i++)
                {
                    if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                    {
                        selectedIndex = i;
                        selectionChanged = true;
                        break;
                    }
                }
            }

            // Clamp in case the hand shrank after a card was played
            if (selectedIndex >= handCount)
            {
                selectedIndex = handCount - 1;
                selectionChanged = true;
            }

            if (selectionChanged)
            {
                NotifyHandPanel();
                ActivateSelectedCard();
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
                    ActivateSelectedCard();
                    return;
                }
            }
        }

        private void ActivateSelectedCard()
        {
            if (targeting != null)
                targeting.Cancel();

            var hand = cardRuntime.HandCards;
            if (selectedIndex < hand.Count)
                cardPlay.TryPlay(hand[selectedIndex].instanceId);
        }

        private void NotifyHandPanel()
        {
            if (handPanel != null)
                handPanel.SetSelectedIndex(selectedIndex);
        }
    }
}