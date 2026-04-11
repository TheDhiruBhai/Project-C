using UnityEngine;

namespace Game.World
{
    public sealed class Lockable : MonoBehaviour, ILockable
    {
        [SerializeField] private bool locked = true;

        public bool IsLocked => locked;

        public void Unlock()
        {
            locked = false;
            // Add door animation, collider changes, and sound here.
        }
    }
}
