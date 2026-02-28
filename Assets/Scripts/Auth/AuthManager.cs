using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using Newtonsoft.Json.Linq;

public class AuthManager : MonoBehaviour
{
    public static AuthManager Instance;

    [Header("Supabase Settings")]
    public string supabaseURL = "https://qpdjntvucmcpxhuehbqu.supabase.co";
    public string supabaseKey = "your_anon_key_here";

    // Player info
    public static string playerID;
    public static string playerName;
    public static string accessToken;

    void Awake()
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

    // ─── REGISTER ───────────────────────────────────────
    public void Register(string username, string email, string password,
                         System.Action<bool, string> callback)
    {
        StartCoroutine(RegisterRequest(username, email, password, callback));
    }

    IEnumerator RegisterRequest(string username, string email, string password,
                             System.Action<bool, string> callback)
    {
        string authJson = "{\"email\":\"" + email + "\"," +
                          "\"password\":\"" + password + "\"}";

        string uid = "";
        string token = "";
        bool error = false;
        string errorMsg = "";

        // Step 1 — Create auth account
        yield return SendRequest(
            "/auth/v1/signup",
            authJson,
            "POST",
            false,
            (response) =>
            {
                Debug.Log("Register RAW: " + response);
                var data = JObject.Parse(response);

                if (data["code"] != null)
                {
                    error = true;
                    errorMsg = data["msg"] != null ? data["msg"].ToString() : "Registration failed";
                    return;
                }

                uid = data["user"]["id"].ToString();
                token = data["access_token"].ToString();
                accessToken = token;
            });

        // Stop here if error
        if (error)
        {
            callback(false, errorMsg);
            yield break;
        }

        Debug.Log("Saving username for uid: " + uid);

        // Step 2 — Save username AFTER coroutine completes
        string json = "{\"id\":\"" + uid + "\"," +
                      "\"username\":\"" + username + "\"}";

        string url = supabaseURL + "/rest/v1/users";
        var request = new UnityWebRequest(url, "POST");

        byte[] body = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(body);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("apikey", supabaseKey);
        request.SetRequestHeader("Authorization", "Bearer " + token);
        request.SetRequestHeader("Prefer", "return=minimal");

        yield return request.SendWebRequest();

        Debug.Log("SaveUsername code: " + request.responseCode);
        Debug.Log("SaveUsername RAW: " + request.downloadHandler.text);

        callback(true, "Registered successfully!");
    }

    IEnumerator SaveUsername(string uid, string username,
                          string token,
                          System.Action<bool, string> callback)
    {
        Debug.Log("SaveUsername called with uid: " + uid + " username: " + username);

        string json = "{\"id\":\"" + uid + "\"," +
                      "\"username\":\"" + username + "\"}";

        string url = supabaseURL + "/rest/v1/users";
        var request = new UnityWebRequest(url, "POST");

        byte[] body = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(body);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("apikey", supabaseKey);
        request.SetRequestHeader("Authorization", "Bearer " + token);
        request.SetRequestHeader("Prefer", "return=minimal");

        yield return request.SendWebRequest();

        Debug.Log("SaveUsername response code: " + request.responseCode);
        Debug.Log("SaveUsername RAW: " + request.downloadHandler.text);

        if (request.result == UnityWebRequest.Result.Success ||
            request.responseCode == 201 ||
            request.responseCode == 200)
        {
            Debug.Log("Username saved successfully!");
            callback(true, "Registered successfully!");
        }
        else
        {
            Debug.LogError("SaveUsername failed: " + request.error);
            Debug.LogError("SaveUsername error body: " + request.downloadHandler.text);
            // Still return success since auth account was created
            callback(true, "Registered successfully!");
        }
    }

    // ─── LOGIN ──────────────────────────────────────────
    public void Login(string email, string password,
                      System.Action<bool, string> callback)
    {
        StartCoroutine(LoginRequest(email, password, callback));
    }

    IEnumerator LoginRequest(string email, string password,
                             System.Action<bool, string> callback)
    {
        string json = "{\"email\":\"" + email + "\"," +
                      "\"password\":\"" + password + "\"}";

        yield return SendRequest(
            "/auth/v1/token?grant_type=password",
            json,
            "POST",
            false,
            (response) =>
            {
                Debug.Log("Login RAW: " + response);
                var data = JObject.Parse(response);

                
                if (data["code"] != null)
                {
                    string errorMsg = data["msg"] != null
                        ? data["msg"].ToString()
                        : "Login failed";
                    callback(false, errorMsg);
                    return;
                }

                // Save player info
                accessToken = data["access_token"].ToString();
                playerID = data["user"]["id"].ToString();

                // Get username from users table
                StartCoroutine(GetUsername(playerID, accessToken, callback));
            });
    }

    IEnumerator GetUsername(string uid, string token,
                            System.Action<bool, string> callback)
    {
        yield return SendRequest(
            "/rest/v1/users?id=eq." + uid + "&select=username",
            "",
            "GET",
            true,
            (response) =>
            {
                Debug.Log("GetUsername RAW: " + response);

                try
                {
                    var data = JArray.Parse(response);
                    if (data.Count > 0)
                        playerName = data[0]["username"].ToString();
                    else
                        playerName = "Player";
                }
                catch
                {
                    playerName = "Player";
                }

                callback(true, "Login successful");
            });
    }

    // ─── FORGOT PASSWORD ────────────────────────────────
    public void ForgotPassword(string email,
                               System.Action<bool, string> callback)
    {
        StartCoroutine(ForgotRequest(email, callback));
    }

    IEnumerator ForgotRequest(string email,
                              System.Action<bool, string> callback)
    {
        string json = "{\"email\":\"" + email + "\"}";

        yield return SendRequest(
            "/auth/v1/recover",
            json,
            "POST",
            false,
            (response) =>
            {
                callback(true, "Password reset email sent!");
            });
    }

    
    // ─── SHARED REQUEST HANDLER ─────────────────────────
    IEnumerator SendRequest(string endpoint, string json,
                            string method, bool useToken,
                            System.Action<string> callback)
    {
        string url = supabaseURL + endpoint;
        var request = new UnityWebRequest(url, method);

        if (json != "")
        {
            byte[] body = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(body);
        }

        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("apikey", supabaseKey);

        if (useToken && accessToken != null)
            request.SetRequestHeader("Authorization", "Bearer " + accessToken);
        else
            request.SetRequestHeader("Authorization", "Bearer " + supabaseKey);

        yield return request.SendWebRequest();

        string responseText = request.downloadHandler.text;
        Debug.Log("RAW: " + responseText);

        // Treat 400/401/422 as valid responses not network errors
        if (request.result == UnityWebRequest.Result.Success ||
            request.responseCode == 400 ||
            request.responseCode == 401 ||
            request.responseCode == 422)
        {
            callback(responseText);
        }
        else
        {
            Debug.LogError("Error: " + request.error);
            callback("{\"error\":{\"message\":\"" + request.error + "\"}}");
        }
    }
}