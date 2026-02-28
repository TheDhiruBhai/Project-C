using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class AuthUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject loginPanel;
    public GameObject registerPanel;
    public GameObject forgotPanel;

    [Header("Login")]
    public TMP_InputField loginEmail;
    public TMP_InputField loginPassword;
    public TextMeshProUGUI loginMessage;

    [Header("Register")]
    public TMP_InputField regUsername;
    public TMP_InputField regEmail;
    public TMP_InputField regPassword;
    public TextMeshProUGUI registerMessage;

    [Header("Forgot")]
    public TMP_InputField forgotEmail;
    public TMP_InputField otpInput;         
    public TMP_InputField newPasswordInput;  
    public TextMeshProUGUI forgotMessage;

    private AuthManager auth;

    void Start()
    {
        auth = FindFirstObjectByType<AuthManager>();
        ShowLogin();
    }

    // ─── LOGIN BUTTON ────────────────────────────────────
    public void OnLoginClick()
    {
        loginMessage.text = "Logging in...";

        auth.Login(loginEmail.text, loginPassword.text,
        (success, message) =>
        {
            if (success)
            {
                loginMessage.text = "Welcome " + AuthManager.playerName + "!";
                Debug.Log("Login success! Loading lobby...");
                // UnityEngine.SceneManagement.SceneManager.LoadScene("LobbyScene");
            }
            else
            {
                loginMessage.text = message;
            }
        });
    }

    // ─── REGISTER BUTTON ─────────────────────────────────
    public void OnRegisterClick()
    {
        registerMessage.text = "Registering...";

        auth.Register(regUsername.text, regEmail.text, regPassword.text,
        (success, message) =>
        {
            registerMessage.text = message;
            if (success) ShowLogin();
        });
    }

    // ─── FORGOT PASSWORD ─────────────────────────────────
    // Supabase handles OTP email automatically
    // No EmailJS needed anymore!
    public void OnSendOTPClick()
    {
        forgotMessage.text = "Sending OTP...";

        auth.ForgotPassword(forgotEmail.text.Trim(),
        (success, message) =>
        {
            forgotMessage.text = success
                ? "OTP sent! Check your email."
                : message;
        });
    }

    public void OnVerifyOTPClick()
    {
        forgotMessage.text = "Verifying...";

        auth.VerifyOTPAndReset(
            forgotEmail.text.Trim(),
            otpInput.text.Trim(),
            newPasswordInput.text,
        (success, message) =>
        {
            forgotMessage.text = message;
            if (success) ShowLogin();
        });
    }

    // ─── PANEL SWITCHING ─────────────────────────────────
    public void ShowLogin()
    {
        loginPanel.SetActive(true);
        registerPanel.SetActive(false);
        forgotPanel.SetActive(false);
    }

    public void ShowRegister()
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
        forgotPanel.SetActive(false);
    }

    public void ShowForgot()
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(false);
        forgotPanel.SetActive(true);
    }
}