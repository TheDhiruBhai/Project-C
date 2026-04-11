using UnityEngine;
using UnityEngine.UI;
using Game.CardsRuntime;

namespace Game.UI
{
    public sealed class CardView : MonoBehaviour
    {
        [Header("Display")]
        [SerializeField] private Image  icon;
        [SerializeField]
        [Tooltip("A child GameObject (e.g. a glowing border Image) shown when this card is selected.")]
        private GameObject selectedHighlight;

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

            SetSelected(false);
        }

        ///Called by HandPanel to show or hide the selection highlight.
        public void SetSelected(bool selected)
        {
            if (selectedHighlight != null)
                selectedHighlight.SetActive(selected);
        }

        private void OnMouseDown()
        {
            if (selection != null)
                selection.SelectByInstanceId(instanceId);
        }
    }
}
