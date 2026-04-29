using UnityEngine;
using UnityEngine.UI;
using TMPro;

// ─────────────────────────────────────────────────────────────────────────────
// GuestLoginUI  —  attach to a GameObject in LoginScene
//
// Add a "Guest" Button to your login UI. When clicked it shows a small panel
// where the user types only their display name (no password / email needed).
// On confirm, sets AuthManager.playerName and connects to Photon.
//
// SCENE SETUP:
//   • guestButton       → the new "Guest" button on your main login UI
//   • guestPanel        → a separate panel (starts hidden) with:
//       - guestNameInput  (TMP_InputField)
//       - confirmButton   ("Play as Guest")
//       - cancelButton    ("Back")
//       - statusText      (error / info messages)
//
// IMPORTANT: wire guestButton.onClick → GuestLoginUI.OnGuestButtonClick()
// ─────────────────────────────────────────────────────────────────────────────
public class GuestLoginUI : MonoBehaviour
{
    [Header("Main Login")]
    public Button       guestButton;          // "Guest" button on login screen

    [Header("Guest Panel")]
    public GameObject   guestPanel;           // Hidden by default
    public TMP_InputField guestNameInput;
    public Button       confirmButton;
    public Button       cancelButton;
    public TMP_Text     statusText;

    [Header("Settings")]
    public int          minNameLength = 2;
    public int          maxNameLength = 16;
    public string       guestSuffix   = " (Guest)";  // appended to nickname

    // ─────────────────────────────────────────────────────────────────────────
    private void Start()
    {
        if (guestPanel   != null) guestPanel.SetActive(false);
        if (statusText   != null) statusText.text = string.Empty;

        confirmButton.onClick.AddListener(OnConfirmGuest);
        cancelButton.onClick.AddListener(OnCancelGuest);

        // Allow pressing Enter in the name field to confirm
        guestNameInput.onSubmit.AddListener(_ => OnConfirmGuest());
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Called by the "Guest" button
    // ─────────────────────────────────────────────────────────────────────────
    public void OnGuestButtonClick()
    {
        guestPanel.SetActive(true);
        guestNameInput.text = string.Empty;
        statusText.text     = string.Empty;
        guestNameInput.ActivateInputField();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Confirm guest name
    // ─────────────────────────────────────────────────────────────────────────
    private void OnConfirmGuest()
    {
        string name = guestNameInput.text.Trim();

        // ── Validation ───────────────────────────────────────────────────────
        if (string.IsNullOrEmpty(name))
        {
            statusText.text = "Please enter a name.";
            return;
        }
        if (name.Length < minNameLength)
        {
            statusText.text = "Name must be at least " + minNameLength + " characters.";
            return;
        }
        if (name.Length > maxNameLength)
        {
            statusText.text = "Name must be " + maxNameLength + " characters or fewer.";
            return;
        }

        // ── Set player name and connect ──────────────────────────────────────
        AuthManager.playerName = name + guestSuffix;

        confirmButton.interactable = false;
        statusText.text = "Joining as " + name + "...";

        PhotonManager.Instance.ConnectToPhoton();
    }

    // ─────────────────────────────────────────────────────────────────────────
    private void OnCancelGuest()
    {
        guestPanel.SetActive(false);
    }
}
