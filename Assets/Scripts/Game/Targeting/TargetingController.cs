using UnityEngine;
using Game.Abilities;

namespace Game.CardsControllers
{
    public sealed class TargetingController : MonoBehaviour
    {
        [SerializeField] private Camera cam;
        [SerializeField] private int layerMask = ~0;
        [SerializeField] private TargetingVisuals visuals;
        [SerializeField] private Abilities.AbilitySystem abilitySystem;
        [SerializeField] private CardsControllers.CardRuntimeController cardRuntime;
        [SerializeField] private CardsControllers.CardPlayController playController;
        [SerializeField] private World.WorldQueryBehaviour worldQuery;

        private bool _active;
        private int _cardInstanceId;
        private int _casterNetId;
        private float _maxRange;
        private AbilitySO _ability;

        private int? _targetNetId;
        private Vector3 _targetPoint;
        private bool _isValid;

        public void Begin(int cardInstanceId, int casterNetId, float maxRange, AbilitySO ability)
        {
            _active = true;
            _cardInstanceId = cardInstanceId;
            _casterNetId = casterNetId;
            _maxRange = Mathf.Max(0f, maxRange);
            _ability = ability;

            _targetNetId = null;
            _targetPoint = Vector3.zero;
            _isValid = false;

            if (visuals != null) visuals.Show();
        }

        public void Cancel()
        {
            _active = false;
            _ability = null;
            _targetNetId = null;
            _targetPoint = Vector3.zero;
            _isValid = false;

            if (visuals != null) visuals.HideAll();
        }

        private void Update()
        {
            if (!_active) return;
            if (cam == null || _ability == null) return;

            ResolveTarget();
            RenderFeedback();
            ValidateCurrent();
            if (Input.GetMouseButtonDown(0))
            {
                if (_isValid)
                {
                    playController.ConfirmTargetAndPlay(_cardInstanceId, _casterNetId, _targetNetId, _targetPoint);
                    Cancel();
                }
            }
            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
                Cancel();
        }

        private void ResolveTarget()
        {
            _targetNetId = null;
            _targetPoint = Vector3.zero;

            if (!worldQuery.RaycastFromCamera(cam, _maxRange, layerMask, out var hit))
                return;

            _targetPoint = hit.point;

            if (_ability.TargetType == TargetType.WorldObject || _ability.TargetType == TargetType.Player)
            {
                var go = hit.collider != null ? hit.collider.gameObject : null;
                if (go != null && worldQuery.TryGetId(go, out int id))
                    _targetNetId = id;
            }
        }

        private void RenderFeedback()
        {
            if (visuals == null) return;

            if (_ability.TargetType == TargetType.Point || _ability.TargetType == TargetType.WorldObject || _ability.TargetType == TargetType.Player)
                visuals.SetLine(cam.transform.position, _targetPoint, _isValid);
        }

        private void ValidateCurrent()
        {
            var ctx = new AbilityContext
            {
                casterNetId = _casterNetId,
                targetNetId = _targetNetId,
                targetPoint = _targetPoint,
                time = Time.timeAsDouble,
                world = worldQuery
            };

            _isValid = _ability.CanActivate(ctx, out _);
        }
    }
}
