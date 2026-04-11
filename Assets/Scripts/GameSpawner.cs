using UnityEngine;
using Photon.Pun;

public class GameSpawner : MonoBehaviour
{
    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    void Start()
    {
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        // Pick a random spawn point
        Vector3 spawnPos = Vector3.zero;
        Quaternion spawnRot = Quaternion.identity;

        if (spawnPoints.Length > 0)
        {
            int index = Random.Range(0, spawnPoints.Length);
            spawnPos = spawnPoints[index].position;
            spawnRot = spawnPoints[index].rotation;
        }

        // Spawn the player prefab across the network
        // "Player" must match your prefab name in Resources folder
        GameObject player = PhotonNetwork.Instantiate(
            "Player",
            spawnPos,
            spawnRot
        );

        Debug.Log("Spawned: " + PhotonNetwork.NickName);
    }
}