using UnityEngine;

namespace Game.World
{
    public interface IWorldQuery
    {
        bool TryGetObjectById(int netId, out GameObject obj);
        bool TryGetId(GameObject obj, out int netId);
        bool RaycastFromCamera(Camera cam, float maxDistance, int layerMask, out RaycastHit hit);
        bool IsTouchingWater(GameObject actor);
    }
}
