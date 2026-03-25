using UnityEngine;
using UnityEngine.UI;
using Game.CardsRuntime;

namespace Game.UI
{
    public sealed class CardView : MonoBehaviour
    {
        [Header("Display")]
        [SerializeField] private Image  icon;
        [SerializeField] private Text   titleText;
        [SerializeField] private Text   descText;
        [SerializeField] private Text   cooldownText;

        [Header("Selection")]
        [SerializeField]
        [Tooltip("A child GameObject (e.g. a glowing border Image) shown when this card is selected.")]
        private GameObject selectedHighlight;

        [Header("Interaction")]
        [SerializeField] private Button button;

        private int instanceId;
        private CardsControllers.CardPlayController play;
        private CardsControllers.CardSelectionController selection;

        public int InstanceId => instanceId;

        public void Bind(CardInstance instance,
                         CardsControllers.CardPlayController playController,
                         CardsControllers.CardSelectionController selectionController = null)
        {
            play      = playController;
            selection = selectionController;
            instanceId = instance.instanceId;

            if (icon != null)
                icon.sprite = instance.definition != null ? instance.definition.icon : null;

            if (titleText != null)
                titleText.text = instance.definition != null ? instance.definition.cardName : "Card";

            if (descText != null)
                descText.text = instance.definition != null ? instance.definition.description : "";

            UpdateCooldown(instance.cooldownRemaining);

            // Clicking selects the card but does not fire it.
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(OnClick);
            }

            SetSelected(false);
        }

        ///Called by HandPanel to show or hide the selection highlight.
        public void SetSelected(bool selected)
        {
            if (selectedHighlight != null)
                selectedHighlight.SetActive(selected);
        }

        public void UpdateCooldown(float seconds)
        {
            if (cooldownText == null) return;
            cooldownText.text = seconds > 0f ? Mathf.CeilToInt(seconds).ToString() : "";
        }

        private void OnClick()
        {
            if (selection != null)
            {
                // Find this card's index in the hand and select it by asking the selection controller to match instanceId.
                selection.SelectByInstanceId(instanceId);
            }
        }
    }
}
