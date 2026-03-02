using UnityEngine;

namespace Game.World
{
    public interface ILockable
    {
        bool IsLocked { get; }
        void Unlock();
    }

    public interface IFlammable
    {
        void Ignite(float seconds);
    }

    public interface ITransformableLiquid
    {
        bool IsFrozen { get; }
        void Freeze();
        void Melt();
    }

    public interface IHoldable
    {
        void HoldStill(float seconds);
    }

    public interface ILightReceiver
    {
        void Illuminate(float seconds);
    }

    public interface IHealthTarget
    {
        void TakeDamage(int amount);
        void Heal(int amount);
    }
}
