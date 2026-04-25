using System.Collections;
using UnityEngine;

namespace Game.World
{
    public sealed class TemporaryAreaLight : MonoBehaviour, IAreaLightSpawner
    {
        [SerializeField] private Light pointLight;

        private void Reset()
        {
            if (pointLight == null) pointLight = GetComponent<Light>();
        }

        public void Initialize(float radius, float duration)
        {
            if (pointLight != null) pointLight.range = radius;
            StartCoroutine(DestroyAfter(duration));
        }

        public void SpawnLight(Vector3 position, float radius, float duration)
        {
            var go = new GameObject("TempAreaLight");
            go.transform.position = position;
            var l = go.AddComponent<Light>();
            l.type = LightType.Point;
            l.range = radius;
            var comp = go.AddComponent<TemporaryAreaLight>();
            comp.pointLight = l;
            comp.Initialize(radius, duration);
        }

        private IEnumerator DestroyAfter(float duration)
        {
            yield return new WaitForSeconds(duration);
            Destroy(gameObject);
        }
    }
}
