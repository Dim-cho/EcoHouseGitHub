using UnityEngine;


/// <summary>
/// Lifetime energy the player has saved by upgrading appliances. This is the
/// figure the website's leaderboard ranks by, so it only ever goes up — it
/// measures what was saved, not what is currently being drawn.
/// </summary>
public static class EnergySaved
{
    const string Key = "ecohouse_kwh_saved";


    static float total = -1f;


    public static int Total
    {
        get
        {
            if (total < 0f) total = PlayerPrefs.GetFloat(Key, 0f);
            return Mathf.FloorToInt(total);
        }
    }


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Reload()
    {
        total = -1f;
    }


    public static void Report(float amount)
    {
        if (amount <= 0f) return;


        if (total < 0f) total = PlayerPrefs.GetFloat(Key, 0f);
        total += amount;


        PlayerPrefs.SetFloat(Key, total);
        PlayerPrefs.Save();


        // GameSync keeps the highest value it's seen, so sending the running
        // total here is safe even if a sync is missed.
        if (GameSync.Instance != null) GameSync.Instance.ReportLifetime(Total, 0, 0);
    }


    // Called after a server-side reset so the counter doesn't re-report old progress.
    public static void Clear()
    {
        total = 0f;
        PlayerPrefs.SetFloat(Key, 0f);
        PlayerPrefs.Save();
    }
}