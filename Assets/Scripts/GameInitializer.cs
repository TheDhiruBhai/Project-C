using System.Linq;
using System.Collections;
using UnityEngine;
using Photon.Pun;

public class GameInitializer : MonoBehaviour
{
    [Header("References")]
    public GameSpawner gameSpawner;
    public IntroManager introManager;

    [Header("Scene Camera to disable when intro ends")]
    public Camera sceneCamera;       // ← drag your scene's default camera here

    [Header("Settings")]
    public float spawnDelay = 0.5f;

    private void Start()
    {
        StartCoroutine(SpawnAfterDelay());
    }

    private IEnumerator SpawnAfterDelay()
    {
        yield return new WaitForSeconds(spawnDelay);

        // ── Read character assignment from room properties ─────────────────
        int myActor = PhotonNetwork.LocalPlayer.ActorNumber;
        int charIndex = -1;

        if (PhotonNetwork.CurrentRoom.CustomProperties
            .TryGetValue("final_" + myActor, out object val) && val is int c)
            charIndex = c;

        // ── Calculate slot ────────────────────────────────────────────────
        int mySlot = System.Array.IndexOf(
            PhotonNetwork.PlayerList
                .Select(p => p.ActorNumber)
                .OrderBy(a => a)
                .ToArray(),
            myActor
        );

        Debug.Log("[GameInitializer] Spawning CharIndex=" + charIndex + " Slot=" + mySlot);

        // ── Spawn prefab now (during intro) ───────────────────────────────
        // PlayerController starts disabled inside the prefab — player can't move yet
        GameObject playerGO = gameSpawner.RespawnAsSelected(null, charIndex, mySlot);

        // ── Wait for intro to finish ──────────────────────────────────────
        if (introManager != null)
            yield return new WaitUntil(() => !introManager.IsIntroPlaying);

        // ── Intro done: swap cameras ──────────────────────────────────────
        if (sceneCamera != null)
            sceneCamera.gameObject.SetActive(false);   // turn off scene camera

        if (playerGO != null)
        {
            PlayerController pc = playerGO.GetComponentInChildren<PlayerController>(true);
            if (pc != null)
                pc.ActivatePlayerCamera();             // turn on player camera only
                                                       // controller left as-is
        }
    }
}