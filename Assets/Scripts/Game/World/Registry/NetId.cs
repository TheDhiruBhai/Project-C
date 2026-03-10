using UnityEngine;

namespace Game.World
{
    public sealed class NetId : MonoBehaviour
    {
        [SerializeField] private int netId;
        public int Value => netId;

        public void Set(int value) => netId = value;
    }
}
