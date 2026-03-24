using UnityEngine;

namespace Game.World
{
    [RequireComponent(typeof(NetId))]
    public sealed class WorldObjectRegistrar : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Leave null to find the WorldQueryBehaviour automatically.")]
        private WorldQueryBehaviour worldQuery;

        private NetId netId;

        private void Awake()
        {
            netId = GetComponent<NetId>();

            if (worldQuery == null)
                worldQuery = FindFirstObjectByType<WorldQueryBehaviour>();

            if (worldQuery != null && netId != null)
                worldQuery.Register(gameObject, netId.Value);
        }

        private void OnDestroy()
        {
            if (worldQuery != null && netId != null)
                worldQuery.Unregister(netId.Value);
        }
    }
}