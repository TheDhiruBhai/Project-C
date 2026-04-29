using UnityEngine;
using Photon.Pun;

public class GameSpawner : MonoBehaviour
{
    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Character Prefabs")]
    // 0=Fire, 1=Water, 2=Earth, 3=Wind
    public string[] characterPrefabNames = { "FirePlayer", "WaterPlayer", "EarthPlayer", "WindPlayer" };

    [Header("Lobby Fallback Prefab")]
    public string fallbackPrefabName = "Player";

    public GameObject SpawnLobbyPlayer(int slotIndex)
    {
        return SpawnPrefab(fallbackPrefabName, slotIndex);
    }

    public GameObject RespawnAsSelected(GameObject currentObject, int elementIndex, int slotIndex)
    {
        if (currentObject != null)
            PhotonNetwork.Destroy(currentObject);

        string prefabName = fallbackPrefabName;

        if (elementIndex >= 0 && elementIndex < characterPrefabNames.Length)
            prefabName = characterPrefabNames[elementIndex];

        return SpawnPrefab(prefabName, slotIndex);
    }

    private GameObject SpawnPrefab(string prefabName, int slotIndex)
    {
        Vector3 pos = Vector3.zero;
        Quaternion rot = Quaternion.identity;

        if (spawnPoints != null && slotIndex >= 0 && slotIndex < spawnPoints.Length)
        {
            pos = spawnPoints[slotIndex].position;
            rot = spawnPoints[slotIndex].rotation;
        }

        GameObject go = PhotonNetwork.Instantiate(prefabName, pos, rot);
        Debug.Log("[GameSpawner] Spawned " + prefabName + " at slot " + slotIndex);
        return go;
    }
}