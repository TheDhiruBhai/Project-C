using UnityEngine;
using UnityEngine.Video;
using TMPro;

// ─────────────────────────────────────────────────────────────────────────────
// IntroManager  —  add to GameScene. Plays an intro video as a full-screen
//                  overlay before the game begins. Any key or click skips it.
// ─────────────────────────────────────────────────────────────────────────────
public class IntroManager : MonoBehaviour
{
    [Header("Video")]
    public VideoPlayer videoPlayer;
    public VideoClip introClip;

    [Header("UI")]
    public GameObject introCanvas;
    public TMP_Text skipHintText;
    public string skipHintMessage = "Press any key to skip";

    [Header("References")]
    public GameTimerManager gameTimerManager;

    [Header("Settings")]
    public float skipHintDelay = 1f;
    public bool skipOnAnyKey = true;
    public bool skipOnMouseClick = true;

    private bool _introDone = false;
    private bool _hintVisible = false;
    private float _elapsed = 0f;

    public bool IsIntroPlaying => !_introDone;


    private void Start()
    {
        if (skipHintText != null) skipHintText.gameObject.SetActive(false);
        if (introCanvas != null) introCanvas.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;

        if (introClip != null)
            videoPlayer.clip = introClip;

        videoPlayer.loopPointReached += OnVideoFinished;
        videoPlayer.Play();
    }

    private void Update()
    {
        if (_introDone) return;

        _elapsed += Time.deltaTime;

        if (!_hintVisible && _elapsed >= skipHintDelay)
        {
            _hintVisible = true;
            if (skipHintText != null)
            {
                skipHintText.gameObject.SetActive(true);
                skipHintText.text = skipHintMessage;
            }
        }

        if (skipOnAnyKey && Input.anyKeyDown)
        {
            SkipIntro();
            return;
        }

        if (skipOnMouseClick && Input.GetMouseButtonDown(0))
            SkipIntro();
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        SkipIntro();
    }

    public void SkipIntro()
    {
        if (_introDone) return;
        _introDone = true;

        videoPlayer.Stop();

        if (introCanvas != null) introCanvas.SetActive(false);

        if (gameTimerManager != null)
            gameTimerManager.OnIntroFinished();

        Debug.Log("[IntroManager] Intro finished. Game started.");
    }
}