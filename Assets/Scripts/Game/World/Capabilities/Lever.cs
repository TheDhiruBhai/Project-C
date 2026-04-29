using UnityEngine;
using Photon.Pun;

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

        private PhotonView _pv;

        public bool IsToggled => toggled;

        private void Awake() => _pv = GetComponent<PhotonView>();
        private void Start() => ApplyVisual();

        public void Toggle()
        {
            // Any client can pull a lever — broadcast the new state to all
            _pv.RPC(nameof(RPC_Toggle), RpcTarget.All, !toggled);
        }

        [PunRPC]
        private void RPC_Toggle(bool newState)
        {
            toggled = newState;
            ApplyVisual();
            if (toggled) onToggleOn?.Invoke();
            else onToggleOff?.Invoke();
        }

        private void ApplyVisual()
        {
            if (leverArm == null) return;
            leverArm.localRotation = Quaternion.Euler(
                toggled ? toggledAngle : untoggledAngle, 0f, 0f);
        }
    }
}