using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

// ─────────────────────────────────────────────────────────────────────────────
// PhotonManager  —  updated to fit the new scene flow:
//
//   LoginScene → (Auth / Guest) → connects to Photon
//   → LobbyScene (create / join room)
//   → ReadyLobbyScene  ← NEW (4 players ready up)
//   → CharacterSelectScene  ← NEW (portrait pick)
//   → GameScene
//
// Only change from your original: OnJoinedRoom now loads "ReadyLobbyScene"
// instead of "GameScene".
// ─────────────────────────────────────────────────────────────────────────────
public class PhotonManager : MonoBehaviourPunCallbacks
{
    public static PhotonManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ─── Called right after login (Supabase OR guest) ────────────────────────
    public void ConnectToPhoton()
    {
        PhotonNetwork.NickName           = AuthManager.playerName;
        PhotonNetwork.AutomaticallySyncScene = true;

        Debug.Log("[Photon] Connecting as: " + PhotonNetwork.NickName);
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("[Photon] Connected to Master.");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        SceneManager.LoadScene("LobbyScene");
    }

    // ─── Room Creation ───────────────────────────────────────────────────────
    public void CreateRoom(string roomName, int maxPlayers = 4)
    {
        RoomOptions options = new RoomOptions
        {
            MaxPlayers = (byte)maxPlayers,
            IsVisible  = true,
            IsOpen     = true
        };
        PhotonNetwork.CreateRoom(roomName, options);
    }

    // ─── Room Join ───────────────────────────────────────────────────────────
    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    // ─── After joining a room → go to Ready Lobby first ─────────────────────
    public override void OnJoinedRoom()
    {
        Debug.Log("[Photon] Joined room: " + PhotonNetwork.CurrentRoom.Name);
        PhotonNetwork.LoadLevel("ReadyLobbyScene");   // ← changed from GameScene
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning("[Photon] Create room failed: " + message);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning("[Photon] Join room failed: " + message);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("[Photon] " + newPlayer.NickName + " entered the room.");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("[Photon] " + otherPlayer.NickName + " left the room.");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning("[Photon] Disconnected: " + cause);
        SceneManager.LoadScene("LoginScene");
    }
}
