using UnityEngine;
using TMPro;


// Shows both balances. Updates instantly when the player earns or spends, then
// reconciles to the server figure after each sync — the site can grant coins
// too (daily bonus, blog rewards), so the server is the authority.
public class CurrencyHUD : MonoBehaviour
{
    [Header("Labels")]
    public TMP_Text coinsText;
    public TMP_Text ecoPointsText;


    [Header("Prefixes")]
    public string coinsPrefix = "Coins: ";
    public string ecoPointsPrefix = "Eco points: ";


    public static CurrencyHUD Instance { get; private set; }


    int coins;
    int ecoPoints;


    // Read-only so shop code can check affordability without being able to
    // set a balance behind GameSync's back.
    public int Coins => coins;
    public int EcoPoints => ecoPoints;


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }


        Instance = this;
    }


    void Start()
    {
        Redraw();


        if (GameSync.Instance != null) GameSync.Instance.OnSynced += OnSynced;


        // Pull the opening balance so the HUD isn't zeroed until the first sync.
        if (EcoHouseApi.Instance != null && EcoHouseApi.Instance.IsLoggedIn)
        {
            StartCoroutine(EcoHouseApi.Instance.GetStats((ok, stats, _) =>
            {
                if (!ok || stats == null) return;
                coins = stats.coins;
                ecoPoints = stats.ecoPoints;
                Redraw();
            }));
        }
    }


    void OnDestroy()
    {
        if (GameSync.Instance != null) GameSync.Instance.OnSynced -= OnSynced;
    }


    // --- Call these alongside the GameSync equivalents ---


    public void AddCoins(int amount)
    {
        coins += amount;
        Redraw();
    }


    public void AddEcoPoints(int amount)
    {
        ecoPoints += amount;
        Redraw();
    }


    // After a server-side reset: the HUD's in-memory totals are stale and no
    // sync is coming to correct them.
    public void SetBalances(int newCoins, int newEcoPoints)
    {
        coins = newCoins;
        ecoPoints = newEcoPoints;
        Redraw();
    }

    void OnSynced(EcoHouseApi.PlayerStats stats)
    {
        if (stats == null) return;


        coins = stats.coins;
        ecoPoints = stats.ecoPoints;
        Redraw();
    }


    void Redraw()
    {
        if (coinsText != null) coinsText.text = coinsPrefix + coins;
        if (ecoPointsText != null) ecoPointsText.text = ecoPointsPrefix + ecoPoints;
    }
}