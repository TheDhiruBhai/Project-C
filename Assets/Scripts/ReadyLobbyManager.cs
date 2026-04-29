using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class ReadyLobbyManager : MonoBehaviourPunCallbacks
{
    [Header("Player Slots — assign 4 of each")]
    public TMP_Text[]   playerNameTexts;   // Slot 0-3 name labels
    public GameObject[] readyIcons;        // Green checkmark per slot
    public GameObject[] emptySlotPanels;   // "Waiting for player..." per slot

    [Header("UI")]
    public Button       readyButton;
    public TMP_Text     readyButtonText;
    public TMP_Text     statusText;
    public TMP_Text     playerCountText;   // e.g. "3 / 4 players"

    [Header("Settings")]
    public int          requiredPlayers = 4;

    private const string K_READY = "lobby_ready";
    private bool _isReady = false;

    // ─────────────────────────────────────────────────────────────────────────
    private void Start()
    {
        SetMyReady(false);
        readyButton.onClick.AddListener(OnReadyClicked);
        RefreshSlots();
    }

    // ─────────────────────────────────────────────────────────────────────────
    private void OnReadyClicked()
    {
        _isReady = !_isReady;
        SetMyReady(_isReady);
        readyButtonText.text = _isReady ? "Cancel Ready" : "Ready Up";
        RefreshSlots();
        CheckAllReady();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Photon Callbacks
    // ─────────────────────────────────────────────────────────────────────────
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        RefreshSlots();
        statusText.text = newPlayer.NickName + " joined the lobby!";
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // If a ready player leaves, reset everyone's ready state
        photonView.RPC(nameof(RPC_ResetAllReady), RpcTarget.All,
                       otherPlayer.NickName);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer,
                                                   Hashtable changedProps)
    {
        RefreshSlots();
        CheckAllReady();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────────────────────────────────────
    private void SetMyReady(bool value)
    {
        Hashtable h = new Hashtable { { K_READY, value } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(h);
    }

    private void RefreshSlots()
    {
        Player[] players = PhotonNetwork.PlayerList;
        int count = players.Length;

        for (int i = 0; i < 4; i++)
        {
            bool filled = i < count;

            if (emptySlotPanels[i] != null)
                emptySlotPanels[i].SetActive(!filled);

            playerNameTexts[i].gameObject.SetActive(filled);
            readyIcons[i].SetActive(false);

            if (!filled) continue;

            Player p = players[i];
            bool isLocal = p.IsLocal;
            playerNameTexts[i].text = p.NickName + (isLocal ? " (You)" : "");

            bool ready = IsPlayerReady(p);
            readyIcons[i].SetActive(ready);
        }

        int readyCount = GetReadyCount();
        playerCountText.text = count + " / " + requiredPlayers + " players";
        statusText.text = readyCount + " / " + count + " ready";
    }

    private bool IsPlayerReady(Player p)
    {
        return p.CustomProperties.TryGetValue(K_READY, out object val)
               && val is bool b && b;
    }

    private int GetReadyCount()
    {
        int count = 0;
        foreach (Player p in PhotonNetwork.PlayerList)
            if (IsPlayerReady(p)) count++;
        return count;
    }

    private void CheckAllReady()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        if (playerCount < requiredPlayers) return;

        foreach (Player p in PhotonNetwork.PlayerList)
            if (!IsPlayerReady(p)) return;

        // All players ready — load character select
        statusText.text = "All ready! Starting character select...";
        PhotonNetwork.LoadLevel("CharacterSelectScene");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // RPC — called when a player leaves to reset ready states
    // ─────────────────────────────────────────────────────────────────────────
    [PunRPC]
    private void RPC_ResetAllReady(string leaverName)
    {
        _isReady = false;
        readyButtonText.text = "Ready Up";
        SetMyReady(false);
        RefreshSlots();
        statusText.text = leaverName + " left. Ready states reset.";
    }
}
