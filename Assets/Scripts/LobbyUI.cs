using UnityEngine;
using TMPro;
using Photon.Pun;

public class LobbyUI : MonoBehaviour
{
    public TMP_InputField roomNameInput;
    public TextMeshProUGUI statusText;

    void Start()
    {
        statusText.text = "Welcome " + AuthManager.playerName + "!";
    }

    public void OnCreateRoomClick()
    {
        string roomName = roomNameInput.text;
        if (string.IsNullOrEmpty(roomName))
            roomName = "Room_" + Random.Range(1000, 9999);

        PhotonManager.Instance.CreateRoom(roomName);
        statusText.text = "Creating room: " + roomName;
    }

    public void OnJoinRoomClick()
    {
        string roomName = roomNameInput.text;
        if (string.IsNullOrEmpty(roomName))
        {
            statusText.text = "Enter a room name!";
            return;
        }

        PhotonManager.Instance.JoinRoom(roomName);
        statusText.text = "Joining room: " + roomName;
    }
}
