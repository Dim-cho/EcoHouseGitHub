using UnityEngine;


/// <summary>
/// Pays out a finished minigame run. Both minigames call this so the rate and
/// the daily cap live in one place.
/// </summary>
public static class MinigameReward
{
    const string DayKey = "ecohouse_minigame_day";
    const string EarnedKey = "ecohouse_minigame_earned";


    /// <summary>Coins a player can win from minigames per day. 0 = no cap.</summary>
    public const int DailyCap = 500;


    static string Today => System.DateTime.UtcNow.ToString("yyyy-MM-dd");


    /// <summary>
    /// What to tell the player after a run, given what Pay() actually awarded.
    /// </summary>
    public static string Describe(int paid)
    {
        if (paid > 0) return $"+{paid} coins";
        if (GameSync.Instance == null) return "log in to earn coins";
        return "daily coin limit reached";
    }


    public static int EarnedToday
    {
        get
        {
            // A new day resets the running total.
            if (PlayerPrefs.GetString(DayKey, "") != Today) return 0;
            return PlayerPrefs.GetInt(EarnedKey, 0);
        }
    }


    public static int RemainingToday => DailyCap <= 0 ? int.MaxValue : Mathf.Max(0, DailyCap - EarnedToday);


    /// <summary>
    /// Awards coins for a run and returns what was actually paid, which may be
    /// less than asked for once the daily cap is reached.
    /// </summary>
    public static int Pay(int coins)
    {
        if (coins <= 0) return 0;


        // Nothing to pay into: the player reached this scene without going
        // through the menu, so GameSync was never created. Say so rather than
        // reporting coins that were never awarded — and don't spend their
        // daily allowance on a payout that didn't happen.
        if (GameSync.Instance == null)
        {
            Debug.LogWarning(
                "[MinigameReward] No GameSync in the scene, so no coins were awarded. " +
                "Start from the MainMenu scene for rewards to reach the server.");
            return 0;
        }


        int paid = Mathf.Min(coins, RemainingToday);
        if (paid <= 0) return 0;


        PlayerPrefs.SetString(DayKey, Today);
        PlayerPrefs.SetInt(EarnedKey, EarnedToday + paid);
        PlayerPrefs.Save();


        // Updates the HUD now and the server on the next push.
        GameSync.Instance.EarnCoins(paid);


        return paid;
    }
}