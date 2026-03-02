using UnityEngine;
using UnityEngine.UI;
using Game.CardsRuntime;

namespace Game.UI
{
    public sealed class CardView : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private Text titleText;
        [SerializeField] private Text descText;
        [SerializeField] private Text cooldownText;
        [SerializeField] private Button button;

        private int _instanceId;
        private CardsControllers.CardPlayController _play;

        public int InstanceId => _instanceId;

        public void Bind(CardInstance instance, CardsControllers.CardPlayController playController)
        {
            _play = playController;
            _instanceId = instance.instanceId;

            if (icon != null) icon.sprite = instance.definition != null ? instance.definition.icon : null;
            if (titleText != null) titleText.text = instance.definition != null ? instance.definition.cardName : "Card";
            if (descText != null) descText.text = instance.definition != null ? instance.definition.description : "";
            UpdateCooldown(instance.cooldownRemaining);

            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(OnClick);
            }
        }

        public void UpdateCooldown(float seconds)
        {
            if (cooldownText == null) return;
            cooldownText.text = seconds > 0f ? Mathf.CeilToInt(seconds).ToString() : "";
        }

        private void OnClick()
        {
            if (_play == null) return;
            _play.TryPlay(_instanceId);
        }
    }
}
