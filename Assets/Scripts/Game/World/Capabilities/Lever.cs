using UnityEngine;

namespace Game.World
{
    public sealed class Lever : MonoBehaviour, ILeverable
    {
        [SerializeField] private bool toggled = false;

        [Header("Visuals")]
        [SerializeField] private Transform leverArm;
        [SerializeField] private float toggledAngle = 45f;
        [SerializeField] private float untoggledAngle = -45f;

        [Header("Events")]
        [SerializeField] private UnityEngine.Events.UnityEvent onToggleOn;
        [SerializeField] private UnityEngine.Events.UnityEvent onToggleOff;

        public bool IsToggled => toggled;

        private void Start() => ApplyVisual();

        public void Toggle()
        {
            toggled = !toggled;
            ApplyVisual();

            if (toggled) onToggleOn?.Invoke();
            else onToggleOff?.Invoke();
        }

        private void ApplyVisual()
        {
            if (leverArm == null) return;
            float angle = toggled ? toggledAngle : untoggledAngle;
            leverArm.localRotation = Quaternion.Euler(angle, 0f, 0f);
        }
    }
}