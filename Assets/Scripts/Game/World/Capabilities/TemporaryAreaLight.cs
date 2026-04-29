using System.Collections;
using UnityEngine;
using Photon.Pun;

namespace Game.World
{
    public sealed class TemporaryAreaLight : MonoBehaviour, IAreaLightSpawner
    {
        [SerializeField] private Light pointLight;
        private PhotonView _pv;

        private void Awake() => _pv = GetComponent<PhotonView>();

        private void Reset()
        {
            if (pointLight == null) pointLight = GetComponent<Light>();
        }

        public void Initialize(float radius, float duration)
        {
            if (pointLight != null) pointLight.range = radius;
            StartCoroutine(DestroyAfter(duration));
        }

        // Called by whoever triggers the light — MasterClient broadcasts spawn
        public void SpawnLight(Vector3 position, float radius, float duration)
        {
            if (!PhotonNetwork.IsMasterClient) return;
            _pv.RPC(nameof(RPC_SpawnLight), RpcTarget.All,
                    position.x, position.y, position.z, radius, duration);
        }

        [PunRPC]
        private void RPC_SpawnLight(float x, float y, float z, float radius, float duration)
        {
            var go = new GameObject("TempAreaLight");
            go.transform.position = new Vector3(x, y, z);

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