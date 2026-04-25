using UnityEngine;
using Game.World;

namespace Game.Abilities
{
    public abstract class AbilitySO : ScriptableObject, IAbility
    {
        [SerializeField] private TargetType targetType = TargetType.None;
        public TargetType TargetType => targetType;

        [Header("Audio")]
        [SerializeField] private AudioClip activationClip;
        [SerializeField][Range(0f, 1f)] private float volume = 1f;

        public abstract bool CanActivate(AbilityContext ctx, out string reason);
        public abstract void Activate(AbilityContext ctx);

        protected void PlayActivationSound(Vector3 position)
        {
            if (activationClip != null)
                AudioSource.PlayClipAtPoint(activationClip, position, volume);
        }
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