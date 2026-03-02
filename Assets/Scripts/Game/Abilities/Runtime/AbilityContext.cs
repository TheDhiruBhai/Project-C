using UnityEngine;
using Game.World;

namespace Game.Abilities
{
    public struct AbilityContext
    {
        public int casterNetId;
        public int? targetNetId;
        public Vector3 targetPoint;
        public double time;
        public IWorldQuery world;

        public bool TryGetCaster(out GameObject caster)
        {
            caster = null;
            if (world == null) return false;
            return world.TryGetObjectById(casterNetId, out caster);
        }

        public bool TryGetTarget(out GameObject target)
        {
            target = null;
            if (world == null) return false;
            if (!targetNetId.HasValue) return false;
            return world.TryGetObjectById(targetNetId.Value, out target);
        }
    }
}
