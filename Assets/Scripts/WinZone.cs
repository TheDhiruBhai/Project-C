using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// ─────────────────────────────────────────────────────────────────────────────
// WinZone  —  attach to any GameObject with a Collider (Is Trigger = ON)
//
// When ALL players in the room are standing inside this zone at the same time,
// TriggerWin() is called on GameTimerManager.
//
// SETUP:
//   • Add a Collider to this GameObject → tick  Is Trigger  ON
//   • Add this script
//   • Assign gameTimerManager in Inspector
//   • Your player prefabs must have a tag set to  "Player"
//     (Select prefab → Tag dropdown → Player)
// ─────────────────────────────────────────────────────────────────────────────
public class WinZone : MonoBehaviour
{
    [Header("References")]
    public GameTimerManager gameTimerManager;

    [Header("Settings")]
    public string playerTag = "Player";   // must match tag on your player prefabs

    // Tracks which PhotonView IDs are currently inside the zone
    private HashSet<int> _playersInside = new HashSet<int>();
    private bool _winTriggered = false;

    // ─────────────────────────────────────────────────────────────────────────
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        PhotonView pv = other.GetComponentInParent<PhotonView>();
        if (pv == null) return;

        _playersInside.Add(pv.ViewID);
        Debug.Log("[WinZone] Player entered: " + pv.Owner?.NickName
                  + "  Inside: " + _playersInside.Count
                  + " / " + PhotonNetwork.CurrentRoom.PlayerCount);

        CheckWinCondition();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        PhotonView pv = other.GetComponentInParent<PhotonView>();
        if (pv == null) return;

        _playersInside.Remove(pv.ViewID);
        Debug.Log("[WinZone] Player left: " + pv.Owner?.NickName
                  + "  Inside: " + _playersInside.Count
                  + " / " + PhotonNetwork.CurrentRoom.PlayerCount);
    }

    // ─────────────────────────────────────────────────────────────────────────
    private void CheckWinCondition()
    {
        if (_winTriggered) return;
        if (!PhotonNetwork.IsMasterClient) return;   // only MasterClient calls this

        int totalPlayers = PhotonNetwork.CurrentRoom.PlayerCount;

        if (_playersInside.Count >= totalPlayers)
        {
            _winTriggered = true;
            Debug.Log("[WinZone] All " + totalPlayers + " players inside — triggering win!");
            gameTimerManager.TriggerWin();
        }
    }
}