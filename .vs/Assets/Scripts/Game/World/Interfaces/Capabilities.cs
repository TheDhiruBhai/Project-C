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
        bool IsHeld { get; }
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

    public interface ILeverable
    {
        bool IsToggled { get; }
        void Toggle();
    }

    public interface IGrowable
    {
        bool IsGrown { get; }
        void Grow();
    }

    public interface IMovable
    {
        bool CanPush(Vector3 casterPosition);
        bool CanPull(Vector3 casterPosition);
        void Push(Vector3 casterPosition);
        void Pull(Vector3 casterPosition);
    }

    public interface ISpeedModifiable
    {
        void ApplySpeedMultiplier(float multiplier, float duration);
    }

    public interface IDamageImmune
    {
        bool IsInvulnerable { get; }
        void SetInvulnerable(float duration);
    }

    public interface IWateryForm
    {
        bool IsPassingThrough { get; }
        void SetPassThrough(float duration);
    }

    public interface IAreaLightSpawner
    {
        void SpawnLight(Vector3 position, float radius, float duration);
    }
}