using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class ChatManager : MonoBehaviourPunCallbacks
{
    [Header("UI")]
    public GameObject chatPanel;
    public TMP_InputField messageInput;
    public Transform messageContainer;
    public GameObject messagePrefab;
    public Button sendButton;
    public ScrollRect scrollRect;

    [Header("Disable these GameObjects while chat is open")]
    public GameObject[] disableOnChat;

    [Header("Settings")]
    public KeyCode toggleKey = KeyCode.T;
    public int maxMessages = 50;
    public Color myColor = new Color(0.45f, 1f, 0.55f);
    public Color otherColor = new Color(1f, 1f, 1f);

    private bool _open = false;
    private List<GameObject> _messages = new List<GameObject>();

    // Cached reference — found once after spawn
    private PlayerController _myController;

    // ─────────────────────────────────────────────────────────────────────────
    private void Start()
    {
        chatPanel.SetActive(false);
        sendButton.onClick.AddListener(SendMessage);
        messageInput.onSubmit.AddListener(_ => SendMessage());
    }

    private void Update()
    {
        if (!messageInput.isFocused && Input.GetKeyDown(toggleKey))
            ToggleChat();

        if (_open && Input.GetKeyDown(KeyCode.Escape))
            CloseChat();

        if (_open && messageInput.isFocused
            && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
            SendMessage();
    }

    // ─────────────────────────────────────────────────────────────────────────
    public void ToggleChat() { if (_open) CloseChat(); else OpenChat(); }

    public void OpenChat()
    {
        _open = true;
        chatPanel.SetActive(true);
        messageInput.ActivateInputField();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SetDisableTargets(false);
        SetMyPlayerController(false);   // ← disable component only
    }

    public void CloseChat()
    {
        _open = false;
        chatPanel.SetActive(false);
        messageInput.DeactivateInputField();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SetDisableTargets(true);
        SetMyPlayerController(true);    // ← re-enable component only
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Finds the local player's PlayerController via photonView.IsMine
    // Caches it so we only search once
    // ─────────────────────────────────────────────────────────────────────────
    private void SetMyPlayerController(bool enabled)
    {
        // Use cache if already found
        if (_myController == null)
        {
            foreach (PlayerController pc in FindObjectsByType<PlayerController>(FindObjectsSortMode.None))
            {
                if (pc.photonView.IsMine)
                {
                    _myController = pc;
                    break;
                }
            }
        }

        if (_myController != null)
            _myController.enabled = enabled;
    }

    // ─────────────────────────────────────────────────────────────────────────
    private void SetDisableTargets(bool active)
    {
        if (disableOnChat == null) return;
        foreach (GameObject go in disableOnChat)
            if (go != null) go.SetActive(active);
    }

    // ─────────────────────────────────────────────────────────────────────────
    private void SendMessage()
    {
        string text = messageInput.text.Trim();
        if (string.IsNullOrEmpty(text)) return;

        messageInput.text = string.Empty;
        messageInput.ActivateInputField();

        photonView.RPC(nameof(RPC_Receive), RpcTarget.All,
                       PhotonNetwork.LocalPlayer.NickName, text);
    }

    [PunRPC]
    private void RPC_Receive(string sender, string message)
    {
        bool mine = sender == PhotonNetwork.LocalPlayer.NickName;

        GameObject msgObj = Instantiate(messagePrefab, messageContainer);
        TMP_Text tmp = msgObj.GetComponentInChildren<TMP_Text>();

        if (tmp != null)
        {
            tmp.text = "<b>" + sender + "</b> : " + message;
            tmp.color = mine ? myColor : otherColor;
        }

        _messages.Add(msgObj);

        if (_messages.Count > maxMessages)
        {
            Destroy(_messages[0]);
            _messages.RemoveAt(0);
        }

        Canvas.ForceUpdateCanvases();
        if (scrollRect != null) scrollRect.verticalNormalizedPosition = 0f;
    }
}