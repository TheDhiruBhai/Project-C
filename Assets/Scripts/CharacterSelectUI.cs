using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

// ─────────────────────────────────────────────────────────────────────────────
// CharacterSelectUI  —  Scene: "CharacterSelectScene"
//
// CHANGES FROM PREVIOUS VERSION:
//   • Selection highlight  → uses the Button's own highlightedColor (no extra GO)
//   • Taken overlay        → button is simply disabled (no overlay panel needed)
//   • Removed              → playerSlotTexts[], playerReadyIcons[]
//   • Added                → characterStatusTexts[]  (one TMP_Text per portrait)
//       shows nothing when free, "Taken\n[Name] is ready" when another player
//       confirmed, your own name when YOU selected/confirmed.
//
// SCENE SETUP:
//   • GameObject with this component + PhotonView
//   • 4 characterButtons  → Buttons whose Image = your character PNG
//       In each Button's Colors block set Highlighted Color to your glow colour
//   • 4 characterStatusTexts → TMP_Text placed just below each button
//   • confirmButton, statusText, timerText
// ─────────────────────────────────────────────────────────────────────────────
public class CharacterSelectUI : MonoBehaviourPunCallbacks
{
    public static readonly string[] CharacterNames = { "Fire", "Water", "Earth", "Wind" };

    [Header("Portrait Buttons (4 — Fire, Water, Earth, Wind)")]
    public Button[] characterButtons;

    [Header("Status Labels (4 — one below each button)")]
    public TMP_Text[] characterStatusTexts;

    [Header("UI")]
    public TMP_Text statusText;
    public TMP_Text timerText;
    public Button confirmButton;
    public TMP_Text confirmButtonText;

    [Header("Settings")]
    public float selectionTime = 30f;

    private const string K_CHAR = "cs_char";
    private const string K_READY = "cs_ready";

    private Color[] _originalNormalColors;

    private int _myChar = -1;
    private bool _confirmed = false;
    private bool _selectionDone = false;
    private float _timer;
    private bool _timerRunning = false;

    private void Start()
    {
        _originalNormalColors = new Color[characterButtons.Length];
        for (int i = 0; i < characterButtons.Length; i++)
            _originalNormalColors[i] = characterButtons[i].colors.normalColor;

        SetMyProps(-1, false);

        for (int i = 0; i < characterButtons.Length; i++)
        {
            int idx = i;
            characterButtons[i].onClick.AddListener(() => OnPortraitClicked(idx));
        }

        confirmButton.onClick.AddListener(OnConfirmClicked);
        confirmButton.interactable = false;

        if (PhotonNetwork.CurrentRoom.CustomProperties
            .TryGetValue("cs_start_time", out object st))
        {
            double elapsed = (PhotonNetwork.ServerTimestamp - (int)st) / 1000.0;
            float remaining = selectionTime - (float)elapsed;
            _timer = remaining > 0f ? remaining : 0f;
            _timerRunning = remaining > 0f;
        }
        else
        {
            _timer = selectionTime;
            _timerRunning = true;
            if (PhotonNetwork.IsMasterClient)
            {
                Hashtable rp = new Hashtable
                    { { "cs_start_time", PhotonNetwork.ServerTimestamp } };
                PhotonNetwork.CurrentRoom.SetCustomProperties(rp);
            }
        }

        statusText.text = "Pick your character!";
        RefreshPortraits();
    }

    private void Update()
    {
        if (!_timerRunning || _selectionDone) return;

        _timer -= Time.deltaTime;
        timerText.text = Mathf.CeilToInt(Mathf.Max(_timer, 0f)).ToString();

        if (_timer <= 0f)
        {
            _timerRunning = false;
            timerText.text = "0";
            if (PhotonNetwork.IsMasterClient)
                RunAutoAssign();
        }
    }

    private void OnPortraitClicked(int idx)
    {
        if (_confirmed || _selectionDone) return;
        if (IsCharTakenByOther(idx))
        {
            statusText.text = CharacterNames[idx] + " is taken!";
            return;
        }

        _myChar = idx;
        SetMyProps(idx, false);
        confirmButton.interactable = true;
        statusText.text = "Selected: " + CharacterNames[idx] + "  —  press Confirm!";
        RefreshPortraits();
    }

    private void OnConfirmClicked()
    {
        if (_myChar < 0 || _confirmed || _selectionDone) return;

        if (IsCharTakenByOther(_myChar))
        {
            statusText.text = CharacterNames[_myChar] + " was just taken! Pick another.";
            _myChar = -1;
            SetMyProps(-1, false);
            confirmButton.interactable = false;
            RefreshPortraits();
            return;
        }

        _confirmed = true;
        SetMyProps(_myChar, true);
        confirmButton.interactable = false;
        if (confirmButtonText != null) confirmButtonText.text = "Confirmed!";
        statusText.text = "Confirmed! Waiting for others...";
        RefreshPortraits();
        CheckAllReady();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (_selectionDone) return;
        RefreshPortraits();
        CheckAllReady();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RefreshPortraits();
    }

    private void RefreshPortraits()
    {
        var ownerOf = new Dictionary<int, Player>();
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (p.IsLocal) continue;
            int c = GetInt(p, K_CHAR, -1);
            if (c >= 0 && c < 4) ownerOf[c] = p;
        }

        for (int i = 0; i < 4; i++)
        {
            bool takenByOther = ownerOf.ContainsKey(i);
            bool isMine = _myChar == i;

            characterButtons[i].interactable = !takenByOther && !_confirmed;

            ColorBlock cb = characterButtons[i].colors;
            cb.normalColor = isMine
                ? characterButtons[i].colors.highlightedColor
                : _originalNormalColors[i];
            characterButtons[i].colors = cb;

            if (characterStatusTexts[i] == null) continue;

            if (takenByOther)
            {
                Player owner = ownerOf[i];
                bool ownerReady = GetBool(owner, K_READY, false);
                characterStatusTexts[i].text = ownerReady
                    ? owner.NickName + "\nis ready!"
                    : "Taken\n" + owner.NickName;
            }
            else if (isMine)
            {
                characterStatusTexts[i].text = _confirmed
                    ? PhotonNetwork.LocalPlayer.NickName + "\nis ready!"
                    : PhotonNetwork.LocalPlayer.NickName + "\n(confirm?)";
            }
            else
            {
                characterStatusTexts[i].text = string.Empty;
            }
        }
    }

    private void CheckAllReady()
    {
        if (_selectionDone || !PhotonNetwork.IsMasterClient) return;
        if (PhotonNetwork.CurrentRoom.PlayerCount < 1) return;

        if (PhotonNetwork.PlayerList.All(p => GetBool(p, K_READY, false)))
        {
            _timerRunning = false;
            RunAutoAssign();
        }
    }

    private void RunAutoAssign()
    {
        if (_selectionDone) return;
        _selectionDone = true;

        Player[] players = PhotonNetwork.PlayerList;
        var final = new Dictionary<int, int>();
        var usedChars = new HashSet<int>();

        foreach (Player p in players)
        {
            if (!GetBool(p, K_READY, false)) continue;
            int c = GetInt(p, K_CHAR, -1);
            if (c < 0 || usedChars.Contains(c)) continue;
            final[p.ActorNumber] = c;
            usedChars.Add(c);
        }

        List<int> pool = Enumerable.Range(0, 4)
            .Where(c => !usedChars.Contains(c))
            .OrderBy(_ => Random.value)
            .ToList();

        foreach (Player p in players)
        {
            if (final.ContainsKey(p.ActorNumber)) continue;
            int c = GetInt(p, K_CHAR, -1);
            if (c >= 0 && !usedChars.Contains(c))
            {
                final[p.ActorNumber] = c;
                usedChars.Add(c);
                pool.Remove(c);
            }
            else if (pool.Count > 0)
            {
                final[p.ActorNumber] = pool[0];
                usedChars.Add(pool[0]);
                pool.RemoveAt(0);
            }
        }

        int[] actors = final.Keys.ToArray();
        int[] chars = actors.Select(a => final[a]).ToArray();
        photonView.RPC(nameof(RPC_FinalizeCharacters), RpcTarget.All, actors, chars);
    }

    [PunRPC]
    private void RPC_FinalizeCharacters(int[] actors, int[] chars)
    {
        _selectionDone = true;
        _timerRunning = false;

        for (int i = 0; i < actors.Length; i++)
        {
            if (actors[i] != PhotonNetwork.LocalPlayer.ActorNumber) continue;

            _myChar = chars[i];
            SetMyProps(_myChar, true);
            statusText.text = "You are: " + CharacterNames[_myChar] + "!";

            if (PhotonNetwork.IsMasterClient)
            {
                Hashtable rp = new Hashtable();
                for (int j = 0; j < actors.Length; j++)
                    rp["final_" + actors[j]] = chars[j];
                PhotonNetwork.CurrentRoom.SetCustomProperties(rp);
            }
            break;
        }

        StartCoroutine(LoadGameAfterDelay(2f));
    }

    private IEnumerator LoadGameAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel("GameScene");
    }

    private bool IsCharTakenByOther(int idx)
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (p.IsLocal) continue;
            if (GetInt(p, K_CHAR, -1) == idx) return true;
        }
        return false;
    }

    private void SetMyProps(int charIdx, bool ready)
    {
        Hashtable h = new Hashtable { { K_CHAR, charIdx }, { K_READY, ready } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(h);
    }

    private static int GetInt(Player p, string key, int fallback) =>
        p.CustomProperties.TryGetValue(key, out object v) && v is int i ? i : fallback;

    private static bool GetBool(Player p, string key, bool fallback) =>
        p.CustomProperties.TryGetValue(key, out object v) && v is bool b ? b : fallback;
}