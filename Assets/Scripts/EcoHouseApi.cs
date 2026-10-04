using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;




// Talks to the website's /api/game endpoints. Survives scene loads so the
// token stays available after the menu hands off to gameplay.
public class EcoHouseApi : MonoBehaviour
{
    public static EcoHouseApi Instance { get; private set; }




    [Tooltip("Website base URL, no trailing slash. Use your PC's LAN IP, not localhost, when testing from another machine.")]
    public string baseUrl = "http://localhost:3000";




    public string Token { get; private set; }
    public string Username { get; private set; }
    public bool IsLoggedIn => !string.IsNullOrEmpty(Token);




    const string TokenKey = "ecohouse_token";
    const string UserKey = "ecohouse_username";




    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }




        Instance = this;
        DontDestroyOnLoad(gameObject);




        Token = PlayerPrefs.GetString(TokenKey, "");
        Username = PlayerPrefs.GetString(UserKey, "");
    }




    public IEnumerator SignUp(string username, string password, Action<bool, string> done)
    {
        yield return Auth("signup", username, password, done);
    }




    public IEnumerator LogIn(string username, string password, Action<bool, string> done)
    {
        yield return Auth("login", username, password, done);
    }




    public void LogOut()
    {
        Token = "";
        Username = "";
        PlayerPrefs.DeleteKey(TokenKey);
        PlayerPrefs.DeleteKey(UserKey);
        PlayerPrefs.Save();
    }




    IEnumerator Auth(string endpoint, string username, string password, Action<bool, string> done)
    {
        var payload = JsonUtility.ToJson(new Credentials { username = username, password = password });




        using var req = new UnityWebRequest($"{baseUrl}/api/game/{endpoint}", "POST");
        req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(payload));
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");




        yield return req.SendWebRequest();




        // A 401/409 still carries a JSON error body worth showing the player.
        var body = req.downloadHandler.text;




        if (req.result == UnityWebRequest.Result.ConnectionError)
        {
            done(false, "Can't reach the server. Is the website running?");
            yield break;
        }




        if (req.responseCode >= 400)
        {
            done(false, ParseError(body));
            yield break;
        }




        var session = JsonUtility.FromJson<AuthResponse>(body);
        if (session == null || string.IsNullOrEmpty(session.token))
        {
            done(false, "Unexpected reply from the server.");
            yield break;
        }




        Token = session.token;
        Username = session.username;
        PlayerPrefs.SetString(TokenKey, Token);
        PlayerPrefs.SetString(UserKey, Username);
        PlayerPrefs.Save();




        done(true, null);
    }




    public IEnumerator GetStats(Action<bool, PlayerStats, string> done)
    {
        using var req = UnityWebRequest.Get($"{baseUrl}/api/game/stats");
        req.SetRequestHeader("Authorization", $"Bearer {Token}");




        yield return req.SendWebRequest();




        if (req.responseCode == 401)
        {
            LogOut();
            done(false, null, "Session expired. Log in again.");
            yield break;
        }




        if (req.result != UnityWebRequest.Result.Success)
        {
            done(false, null, "Couldn't load your stats.");
            yield break;
        }




        done(true, JsonUtility.FromJson<PlayerStats>(req.downloadHandler.text), null);
    }




    // Wipes coins, eco points and upgrades server-side. Lifetime kWh is kept so
    // the leaderboard still reflects energy the player really saved.
    public IEnumerator ResetProgress(Action<bool, string> done)
    {
        using var req = new UnityWebRequest($"{baseUrl}/api/game/reset", "POST");
        req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes("{\"confirm\":\"reset\"}"));
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        req.SetRequestHeader("Authorization", $"Bearer {Token}");


        yield return req.SendWebRequest();


        if (req.responseCode == 401)
        {
            LogOut();
            done(false, "Session expired. Log in again.");
            yield break;
        }


        if (req.result != UnityWebRequest.Result.Success)
        {
            done(false, "Couldn't start a new game. Check your connection.");
            yield break;
        }


        done(true, null);
    }




    static string ParseError(string body)
    {
        try
        {
            var parsed = JsonUtility.FromJson<ErrorResponse>(body);
            if (!string.IsNullOrEmpty(parsed?.error)) return parsed.error;
        }
        catch { /* fall through to the generic message */ }




        return "Something went wrong.";
    }




    [Serializable] class Credentials { public string username; public string password; }
    [Serializable] class AuthResponse { public string username; public string token; }
    [Serializable] class ErrorResponse { public string error; }




    [Serializable]
    public class PlayerStats
    {
        public string username;
        public int kwhSaved;
        public int coins;
        public int ecoPoints;
        public int housesCompleted;
        public int upgradesOwned;
        public string tier;
        public int playTimeMinutes;
    }
}