using System.Collections.Generic;
using UnityEngine;

namespace Game.World
{
    public sealed class WorldQueryBehaviour : MonoBehaviour, IWorldQuery
    {
        [Header("Raycast")]
        [SerializeField] private LayerMask defaultMask = ~0;

        private readonly Dictionary<int, GameObject> _byId = new Dictionary<int, GameObject>();

        public void Register(GameObject obj, int netId)
        {
            if (obj == null) return;
            _byId[netId] = obj;
        }

        public void Unregister(int netId)
        {
            if (_byId.ContainsKey(netId))
                _byId.Remove(netId);
        }

        public bool TryGetObjectById(int netId, out GameObject obj) => _byId.TryGetValue(netId, out obj);

        public bool TryGetId(GameObject obj, out int netId)
        {
            netId = 0;
            if (obj == null) return false;
            var nid = obj.GetComponentInParent<NetId>();
            if (nid == null) return false;
            netId = nid.Value;
            return true;
        }

        public bool RaycastFromCamera(Camera cam, float maxDistance, int layerMask, out RaycastHit hit)
        {
            hit = default;
            if (cam == null) return false;

            int mask = layerMask == 0 ? defaultMask.value : layerMask;
            var ray = new Ray(cam.transform.position, cam.transform.forward);
            return Physics.Raycast(ray, out hit, maxDistance, mask);
        }

        public bool IsTouchingWater(GameObject actor)
        {
            if (actor == null) return false;
            var probe = actor.GetComponentInChildren<WaterContactProbe>();
            if (probe == null) return false;
            return probe.IsInWater;
        }
    }
}
