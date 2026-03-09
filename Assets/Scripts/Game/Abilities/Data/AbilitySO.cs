using UnityEngine;
using Game.World;

namespace Game.Abilities
{
    public abstract class AbilitySO : ScriptableObject, IAbility
    {
        [SerializeField] private TargetType targetType = TargetType.None;
        public TargetType TargetType => targetType;

        public abstract bool CanActivate(AbilityContext ctx, out string reason);
        public abstract void Activate(AbilityContext ctx);
    }

    public interface ITargetResolver
    {
        bool TryResolve(out TargetSelection sel);
        void RenderFeedback(bool valid);
    }

    public struct TargetSelection
    {
        public int? targetNetId;
        public Vector3 point;
    }
}
