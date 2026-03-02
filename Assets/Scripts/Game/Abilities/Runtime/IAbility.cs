using UnityEngine;

namespace Game.Abilities
{
    public interface IAbility
    {
        TargetType TargetType { get; }
        bool CanActivate(AbilityContext ctx, out string reason);
        void Activate(AbilityContext ctx);
    }
}
