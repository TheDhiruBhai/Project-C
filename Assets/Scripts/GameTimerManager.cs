using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class GameTimerManager : MonoBehaviourPunCallbacks
{
    [Header("Timer")]
    public float gameDuration = 300f;
    public TMP_Text timerText;

    [Header("Win Panel")]
    public GameObject winPanel;
    public Image winImage;
    public Sprite winSprite;

    [Header("Lose / Game-Over Panel")]
    public GameObject losePanel;
    public Image loseImage;
    public Sprite loseSprite;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip winClip;
    public AudioClip loseClip;

    [Header("HUD objects to hide when game ends")]
    public GameObject[] hideOnEnd;

    [Header("Auto-return to lobby (0 = stay on result screen)")]
    public float returnToLobbyDelay = 8f;

    private float _timeRemaining;
    private bool _timerRunning = false;
    private bool _gameEnded = false;

    // ─────────────────────────────────────────────────────────────────────────
    private void Start()
    {
        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);

        if (winImage != null && winSprite != null) winImage.sprite = winSprite;
        if (loseImage != null && loseSprite != null) loseImage.sprite = loseSprite;

        if (PhotonNetwork.CurrentRoom.CustomProperties
            .TryGetValue("game_start_time", out object st))
        {
            double elapsed = (PhotonNetwork.ServerTimestamp - (int)st) / 1000.0;
            _timeRemaining = Mathf.Max(gameDuration - (float)elapsed, 0f);
        }
        else
        {
            _timeRemaining = gameDuration;
            if (PhotonNetwork.IsMasterClient)
            {
                var rp = new ExitGames.Client.Photon.Hashtable
                    { { "game_start_time", PhotonNetwork.ServerTimestamp } };
                PhotonNetwork.CurrentRoom.SetCustomProperties(rp);
            }
        }

        _timerRunning = false;
        UpdateTimerDisplay();
    }

    // ─────────────────────────────────────────────────────────────────────────
    public void OnIntroFinished()
    {
        _timerRunning = true;
    }

    // ─────────────────────────────────────────────────────────────────────────
    private void Update()
    {
        if (!_timerRunning || _gameEnded) return;

        _timeRemaining -= Time.deltaTime;
        UpdateTimerDisplay();

        if (_timeRemaining <= 0f)
        {
            _timerRunning = false;
            _timeRemaining = 0f;
            UpdateTimerDisplay();

            if (PhotonNetwork.IsMasterClient)
                photonView.RPC(nameof(RPC_TriggerLose), RpcTarget.All);
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    public void TriggerWin()
    {
        if (_gameEnded) return;
        if (PhotonNetwork.IsMasterClient)
            photonView.RPC(nameof(RPC_TriggerWin), RpcTarget.All);
    }

    // ─────────────────────────────────────────────────────────────────────────
    [PunRPC]
    private void RPC_TriggerWin()
    {
        if (_gameEnded) return;
        _gameEnded = true;
        _timerRunning = false;

        HideHUD();  // ← cursor unlocked here

        if (winPanel != null) winPanel.SetActive(true);
        PlayClip(winClip);

        if (returnToLobbyDelay > 0f)
        {
            float waitTime = winClip != null
                ? Mathf.Max(returnToLobbyDelay, winClip.length)
                : returnToLobbyDelay;
            Invoke(nameof(ReturnToLobby), waitTime);
        }
    }

    [PunRPC]
    private void RPC_TriggerLose()
    {
        if (_gameEnded) return;
        _gameEnded = true;
        _timerRunning = false;

        HideHUD();  // ← cursor unlocked here

        if (losePanel != null) losePanel.SetActive(true);
        PlayClip(loseClip);

        if (returnToLobbyDelay > 0f)
        {
            float waitTime = loseClip != null
                ? Mathf.Max(returnToLobbyDelay, loseClip.length)
                : returnToLobbyDelay;
            Invoke(nameof(ReturnToLobby), waitTime);
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    private void HideHUD()
    {
        // ── Unlock and show cursor immediately ────────────────────────────
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Disable manual hideOnEnd list
        if (hideOnEnd != null)
            foreach (GameObject obj in hideOnEnd)
                if (obj != null) obj.SetActive(false);

        // Disable ALL GameObjects tagged "HUD"
        foreach (GameObject hud in GameObject.FindGameObjectsWithTag("HUD"))
            if (hud != null) hud.SetActive(false);
    }

    // ─────────────────────────────────────────────────────────────────────────
    private void PlayClip(AudioClip clip)
    {
        if (audioSource == null || clip == null) return;
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }

    private void UpdateTimerDisplay()
    {
        if (timerText == null) return;
        int mins = Mathf.FloorToInt(_timeRemaining / 60f);
        int secs = Mathf.FloorToInt(_timeRemaining % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", mins, secs);
    }

    // ─────────────────────────────────────────────────────────────────────────
    private void ReturnToLobby()
    {
        // Safety net — make sure cursor is free before any scene loads
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("LobbyScene");
    }
}