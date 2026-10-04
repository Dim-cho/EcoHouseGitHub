using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;




// Sends progress to the website. Currencies go as deltas because the site also
// writes them (daily bonus, blog rewards); lifetime totals go as absolutes.
public class GameSync : MonoBehaviour
{
    public static GameSync Instance { get; private set; }




    [Tooltip("Seconds between automatic syncs.")]
    public float syncInterval = 60f;




    // Pending deltas, cleared only once the server confirms the write.
    int pendingCoins;
    int pendingEcoPoints;




    int kwhSaved;
    int housesCompleted;
    int upgradesOwned;




    bool syncing;




    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }




        Instance = this;
        DontDestroyOnLoad(gameObject);
    }




    void Start()
    {
        StartCoroutine(SyncLoop());
    }




    // --- Call these from gameplay code ---




    public void EarnCoins(int amount) => ChangeCoins(Mathf.Abs(amount));
    public void SpendCoins(int amount) => ChangeCoins(-Mathf.Abs(amount));
    public void EarnEcoPoints(int amount) => ChangeEcoPoints(Mathf.Abs(amount));
    public void SpendEcoPoints(int amount) => ChangeEcoPoints(-Mathf.Abs(amount));




    // The HUD updates here rather than at every call site, so a display can't
    // drift out of step with what will actually be sent.
    void ChangeCoins(int delta)
    {
        pendingCoins += delta;
        if (CurrencyHUD.Instance != null) CurrencyHUD.Instance.AddCoins(delta);
    }




    void ChangeEcoPoints(int delta)
    {
        pendingEcoPoints += delta;
        if (CurrencyHUD.Instance != null) CurrencyHUD.Instance.AddEcoPoints(delta);
    }




    // After a server-side reset: drop anything still queued, or the next push
    // would re-apply it and undo the reset.
    public void DiscardPending()
    {
        pendingCoins = 0;
        pendingEcoPoints = 0;
        upgradesOwned = 0;
    }




    public void ReportLifetime(int kwh, int houses, int upgrades)
    {
        kwhSaved = Mathf.Max(kwhSaved, kwh);
        housesCompleted = Mathf.Max(housesCompleted, houses);
        upgradesOwned = Mathf.Max(upgradesOwned, upgrades);
    }




    IEnumerator SyncLoop()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(syncInterval);
            yield return Push();
        }
    }




    public IEnumerator Push()
    {
        var api = EcoHouseApi.Instance;
        if (api == null || !api.IsLoggedIn || syncing) yield break;




        // Nothing changed since the last successful push.
        if (pendingCoins == 0 && pendingEcoPoints == 0 && kwhSaved == 0) yield break;




        syncing = true;




        // Snapshot before sending: gameplay may add more while the request is in flight.
        var sent = new SyncPayload
        {
            coinsDelta = pendingCoins,
            ecoPointsDelta = pendingEcoPoints,
            kwhSaved = kwhSaved,
            housesCompleted = housesCompleted,
            upgradesOwned = upgradesOwned,
        };




        using var req = new UnityWebRequest($"{api.baseUrl}/api/game/sync", "POST");
        req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(JsonUtility.ToJson(sent)));
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        req.SetRequestHeader("Authorization", $"Bearer {api.Token}");




        yield return req.SendWebRequest();




        syncing = false;




        if (req.result != UnityWebRequest.Result.Success)
        {
            // Keep the deltas so the next attempt resends them.
            Debug.LogWarning($"[GameSync] Sync failed ({req.responseCode}); will retry.");
            yield break;
        }




        // Subtract only what we sent, so anything earned mid-request survives.
        pendingCoins -= sent.coinsDelta;
        pendingEcoPoints -= sent.ecoPointsDelta;




        OnSynced?.Invoke(JsonUtility.FromJson<EcoHouseApi.PlayerStats>(req.downloadHandler.text));
    }




    public event Action<EcoHouseApi.PlayerStats> OnSynced;




    // Quitting kills coroutines mid-flight, so the final push has to block.
    void OnApplicationQuit()
    {
        var api = EcoHouseApi.Instance;
        if (api == null || !api.IsLoggedIn) return;
        if (pendingCoins == 0 && pendingEcoPoints == 0 && kwhSaved == 0) return;




        var payload = JsonUtility.ToJson(new SyncPayload
        {
            coinsDelta = pendingCoins,
            ecoPointsDelta = pendingEcoPoints,
            kwhSaved = kwhSaved,
            housesCompleted = housesCompleted,
            upgradesOwned = upgradesOwned,
        });




        using var req = new UnityWebRequest($"{api.baseUrl}/api/game/sync", "POST");
        req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(payload));
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        req.SetRequestHeader("Authorization", $"Bearer {api.Token}");




        req.SendWebRequest();
        while (!req.isDone) { /* hold the quit until the save lands */ }
    }




    // Alt-tabbing away on mobile, or minimising, is the last chance to save.
    void OnApplicationPause(bool paused)
    {
        if (paused) StartCoroutine(Push());
    }




    [Serializable]
    class SyncPayload
    {
        public int coinsDelta;
        public int ecoPointsDelta;
        public int kwhSaved;
        public int housesCompleted;
        public int upgradesOwned;
    }
}