using UnityEngine;

namespace Game.World
{
    public sealed class WaterContactProbe : MonoBehaviour
    {
        [SerializeField] private string waterTag = "Water";
        public bool IsInWater { get; private set; }

        private void OnTriggerEnter(Collider other)
        {
            if (other != null && other.CompareTag(waterTag)) IsInWater = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other != null && other.CompareTag(waterTag)) IsInWater = false;
        }
    }
}
